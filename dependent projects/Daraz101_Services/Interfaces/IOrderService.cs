namespace Daraz101_Services
{
    public interface IOrderService
    {
        Task<int> PlaceOrderAsync(string userId, string deliveryAddress);
        Task<List<OrderDTO>> GetOrdersByUserAsync(string userId); 
        Task<OrderDTO> GetOrderByIdAsync(int orderId);
       
        
    }
}
