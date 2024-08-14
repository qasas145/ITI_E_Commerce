public class CategoryRepository : Repository<Category>,ICategory {
    private ApplicationDbContext _db;
    public CategoryRepository(ApplicationDbContext db) : base(db) {
        _db = db;
    }
    public void Update(Category obj) {
        _db.Categories.Update(obj);
    }
}