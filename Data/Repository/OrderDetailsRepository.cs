public class OrderDetailsRepository : Repository<OrderDetails>,IOrderDetails {
    private ApplicationDbContext _db;
    public OrderDetailsRepository(ApplicationDbContext db) : base(db) {
        _db = db;
    }
    public void Update(OrderDetails obj) {
        _db.OrderDetails.Update(obj);
    }
}