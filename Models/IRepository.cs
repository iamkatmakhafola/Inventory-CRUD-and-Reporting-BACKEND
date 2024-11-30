namespace Assignment3_Backend.Models
{
    public interface IRepository
    {
        Task<bool> SaveChanges();
        Task<Product[]> GetProducts();
        Task<ProductType[]> GetProductTypes();
        Task<Brand[]> GetBrands();

        void Add<T>(T entity) where T : class;
    }
}
