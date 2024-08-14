public class CompanyRepository : Repository<Company>,ICompany {
    private ApplicationDbContext _db;
    public CompanyRepository(ApplicationDbContext db) : base(db) {
        _db = db;
    }
    public void Update(Company obj) {
        _db.Companies.Update(obj);
    }
}