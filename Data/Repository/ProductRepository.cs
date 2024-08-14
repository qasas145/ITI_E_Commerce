public class ProductRepository : Repository<Product>,IProduct {
    private ApplicationDbContext _db;
    public ProductRepository(ApplicationDbContext db) : base(db) {
        _db = db;
    }
    public void Update(Product obj) {
        _db.Products.Update(obj);
    }
}