public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCart  {
    private ApplicationDbContext _db;
    public ShoppingCartRepository(ApplicationDbContext db) :base(db){
        _db = db;
    }
    public void Update(ShoppingCart obj) {
        _db.ShoppingCarts.Update(obj);
    }
}