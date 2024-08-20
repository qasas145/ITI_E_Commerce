using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class DbInitializer : IDbInitializer {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _db;
    public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db) {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
    }
    public void Initialize() {
        Console.WriteLine("we are in the iniliazer context");
        try {
            Console.WriteLine("in the mig");
            if (_db.Database.GetPendingMigrations().Any()){
                Console.WriteLine("if migration is done");
                _db.Database.Migrate();
            }

        }catch(Exception e) {Console.WriteLine(e.Message);}

        if (!_roleManager.RoleExistsAsync("Customer").GetAwaiter().GetResult()) {

            _roleManager.CreateAsync(new IdentityRole("Customer")).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole("Company")).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole("Employee")).GetAwaiter().GetResult();
            
            var res = _userManager.CreateAsync(new ApplicationUser{
                UserName = "mohamed@yahoo.com",
                Email = "mohamed@yahoo.com",
                PhoneNumber = "01708119559",
                Name = "Muhammad elsayed "
            }, "Admin1234@").GetAwaiter().GetResult();
            foreach (var item in res.Errors)
            {
                Console.WriteLine(item.Description);
            }


            ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u=>u.Email == "mohamed.sayed14527@yahoo.com");
            Console.WriteLine("Teh usrer email is {0}", user.Email);
            _userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
        }

        return ;
    }   
}
