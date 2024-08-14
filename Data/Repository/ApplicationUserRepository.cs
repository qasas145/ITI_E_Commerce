public class ApplicationUserRepository : Repository<ApplicationUser>,IApplicationUser {
    private readonly ApplicationDbContext _db;
    public ApplicationUserRepository(ApplicationDbContext db):base(db) {
        _db = db;
    }
    public void Update(ApplicationUser obj) {
        _db.Update(obj);
    }
}