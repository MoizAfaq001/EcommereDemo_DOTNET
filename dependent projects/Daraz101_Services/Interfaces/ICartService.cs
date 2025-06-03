using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daraz101_Services
{
    public interface ICartService
    {
        Task<List<CartItemDTO>> GetCartItemsAsync(string userId);
        Task AddToCartAsync(string userId, int productId, int quantity);
        Task RemoveFromCartAsync(string userId, int productId);
        Task ClearCartAsync(string userId);
        Task<decimal> GetCartTotalAsync(string userId);
        Task UpdateQuantityAsync(string userId, int productId, int quantity);
    }
}
