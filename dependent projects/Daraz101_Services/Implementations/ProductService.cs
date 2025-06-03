using AutoMapper;
using Daraz101_Data;
using Daraz101_Services;
using Microsoft.EntityFrameworkCore;

namespace Daraz101_Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<List<ProductDTO>>(products);
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<List<ProductDTO>> SearchProductsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllProductsAsync();

            var normalizedSearchTerm = searchTerm.ToLower();
            var products = await _context.Products
                .AsNoTracking()
                .Where(p => p.Name.ToLower().Contains(normalizedSearchTerm) ||
                           (p.Description != null && p.Description.ToLower().Contains(normalizedSearchTerm)))
                .ToListAsync();

            return _mapper.Map<List<ProductDTO>>(products);
        }

        public async Task CreateProductAsync(ProductDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(ProductDTO productDto)
        {
            var existingProduct = await _context.Products.FindAsync(productDto.Id);
            if (existingProduct == null)
                throw new KeyNotFoundException($"Product with ID {productDto.Id} not found");

            _mapper.Map(productDto, existingProduct);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {productId} not found");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}