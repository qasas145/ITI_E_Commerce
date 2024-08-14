public class ProductImageRespository : Repository<ProductImage>, IProductImage {
    private ApplicationDbContext _db;
    public ProductImageRespository(ApplicationDbContext db) : base(db) {
        _db = db;
    }
    public void Update(ProductImage obj) {
        _db.ProductImages.Update(obj);
    }
}