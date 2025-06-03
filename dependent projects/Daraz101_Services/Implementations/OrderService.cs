using AutoMapper;
using Daraz101_Data;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Daraz101_Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;


        public OrderService(ApplicationDbContext context, IMapper mapper, ILogger<OrderService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> PlaceOrderAsync(string userId, string deliveryAddress)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            if (string.IsNullOrWhiteSpace(deliveryAddress))
                throw new ArgumentException("Delivery address cannot be empty", nameof(deliveryAddress));

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Get cart items without tracking Product to avoid EF conflict
                var cartItems = await _context.CartItems
                    .Where(ci => ci.UserId == userId)
                    .ToListAsync();

                if (!cartItems.Any())
                    throw new InvalidOperationException("Cannot place order - cart is empty");

                var productIds = cartItems.Select(ci => ci.ProductId).ToList();

                // Fetch all related products once, from EF context
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);

                decimal orderTotal = 0;
                foreach (var item in cartItems)
                {
                    if (!products.TryGetValue(item.ProductId, out var product))
                        throw new InvalidOperationException($"Product {item.ProductId} not found");

                    if (product.StockQuantity < item.Quantity)
                        throw new InvalidOperationException(
                            $"Not enough stock for {product.Name}. Available: {product.StockQuantity}, Requested: {item.Quantity}");

                    orderTotal += product.Price * item.Quantity;
                }

                // Create and save address
                var address = new Address
                {
                    FullAddress = deliveryAddress,
                    UserId = userId,
                    IsDefault = true
                };
                await _context.Addresses.AddAsync(address);
                await _context.SaveChangesAsync();

                // Create order
                var order = new Order
                {
                    UserId = userId,
                    AddressId = address.Id,
                    Address = address,
                    OrderDate = DateTime.UtcNow,
                    Status = "Processing",
                    Items = cartItems.Select(ci => {
                        var product = products[ci.ProductId];
                        return new OrderItem
                        {
                            ProductId = product.Id,
                            Quantity = ci.Quantity,
                            Price = product.Price,
                            TotalPrice = product.Price * ci.Quantity
                        };
                    }).ToList()
                };

                // Update stock quantities
                foreach (var product in products.Values)
                {
                    var quantityOrdered = cartItems.First(ci => ci.ProductId == product.Id).Quantity;
                    product.StockQuantity -= quantityOrdered;
                    product.UpdatedAt = DateTime.UtcNow;
                }

                // Save order and clear cart
                await _context.Orders.AddAsync(order);
                _context.CartItems.RemoveRange(cartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error placing order for user {UserId}", userId);
                throw;
            }
        }


        public async Task<List<OrderDTO>> GetOrdersByUserAsync(string userId)
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.UserId == userId)
                .ToListAsync(); // Changed to ToListAsync()

            return _mapper.Map<List<OrderDTO>>(orders);
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);  // Changed to FirstOrDefaultAsync

            return order != null ? _mapper.Map<OrderDTO>(order) : null;
        }
    }
}
