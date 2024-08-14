public class UnitOfWork :IUnitOfWork{
    private ApplicationDbContext _db;
    public ICategory Category{get;private set;}
    public IOrderDetails OrderDetails{get;private set;}
    public IOrderHeader OrderHeader{get;private set;}
    public IProduct Product{get;private set;}
    public IProductImage ProductImage{get;private set;}

    public IShoppingCart ShoppingCart{get;private set;}
    public ICompany Company{get;}

    public IApplicationUser ApplicationUser{get;private set;}

    public UnitOfWork(ApplicationDbContext db) {
        _db = db;
        Category = new CategoryRepository(_db);
        OrderDetails = new OrderDetailsRepository(_db);
        OrderHeader = new OrderHeaderRespository(_db);
        Product = new ProductRepository(_db);
        ShoppingCart = new ShoppingCartRepository(_db);
        Company = new CompanyRepository(_db);
        ProductImage = new ProductImageRespository(db);
        ApplicationUser = new ApplicationUserRepository(db);
    }
    public void Save() {
        _db.SaveChanges();
    }

}