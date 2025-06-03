namespace Daraz101_Services

{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetAllProductsAsync(); 
        Task<ProductDTO> GetProductByIdAsync(int id);
        Task UpdateProductAsync(ProductDTO productDto);
        Task<List<ProductDTO>> SearchProductsAsync(string searchTerm); 
        Task CreateProductAsync(ProductDTO productDto); 
        Task DeleteProductAsync(int productId); 
    }
}
