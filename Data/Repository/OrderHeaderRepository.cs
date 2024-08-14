public class OrderHeaderRespository :  Repository<OrderHeader>,IOrderHeader {
    private ApplicationDbContext _db;
    public OrderHeaderRespository(ApplicationDbContext db) : base(db) {
        _db = db;
    }
    public void Update(OrderHeader obj) {
        _db.OrderHeaders.Update(obj);
    }
}