using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class RegisterModel : PageModel {
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserStore<IdentityUser> _userStore;
    private readonly IUserEmailStore<IdentityUser> _emailStore;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _db;

    public RegisterModel(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        
        IUserStore<IdentityUser> userStore,
        
        SignInManager<IdentityUser> signInManager,
        ILogger<RegisterModel> logger,
        IUnitOfWork unitOfWork,
        ApplicationDbContext db
        ) {

            _signInManager = signInManager;
            _roleManager = roleManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
            _unitOfWork = unitOfWork;
            _db =db;
    }

    [BindProperty]

    public InputModel Input{get;set;} 
    
    public string ReturnUrl { get; set; }

    // public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public class InputModel {
        [Required]
        [EmailAddress]
        [Display(Name = "Your Email : ")]
        public string Email{get;set;}

        [Required]
        [Display(Name = "Street address")]
        public string? StreetAddress{get;set;}

        [Required]
        [Display(Name = "City")]
        public string? City{get;set;}

        [Required]
        [Display(Name = "State")]
        public String? State{get;set;}

        [Required]
        [Display(Name = "Postal Code")]
        public string? PostalCode{get;set;}



        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage ="The {0} must be at least {2} and at max {1} characters long.", MinimumLength =6)]
        public string Password {get;set;}

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage ="The password arn't the same ")]
        public string ConfirmPassword{get;set;}

        public string Role { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> RoleList { get; set; }


        [Required]
        public string PhoneNumber{get;set;}

        [Required]
        public string Name { get; set; }
        public int? CompanyId { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CompanyList { get; set; }
    }

    public async Task OnGetAsync(string returnUrl = null) {
        

        Input = new()
            {
                RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i

                }),
                CompanyList = _unitOfWork.Company.GetAll().Select(i=>new SelectListItem{
                    Text  = i.Name,
                    Value = i.Id.ToString()
                })
            };
        ReturnUrl =  returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null) {

        returnUrl ??= Url.Content("~/");
        if (ModelState.IsValid) {

            var user = CreateUser();
            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
            user.Name = Input.Name;
            user.PhoneNumber = Input.PhoneNumber;
            user.City = Input.City;
            user.PostalCode = Input.PostalCode;
            user.State = Input.State;
            user.StreetAddress = Input.StreetAddress;
            if (Input.Role.ToLower() == "company") {
                user.CompanyId = Input.CompanyId;
            }

            
            var result = await _userManager.CreateAsync(user,Input.Password);
            if (result.Succeeded) {
                    _logger.LogInformation("User created a new account with password.");

                    if (!String.IsNullOrEmpty(Input.Role))
                    {
                        await _userManager.AddToRoleAsync(user, Input.Role);

                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "Customer");
                    }
                    if (User.IsInRole("Admin")) {
                        
                        TempData["success"] = "Company Registered successfully";
                    }
                    else {
                        await _signInManager.SignInAsync(user, isPersistent:false);
                    }
                    TempData["success"] = "User created successfully";
                    LocalRedirect(returnUrl);

            }
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        else {

            foreach (var item in ModelState.Values)
            {
                foreach (var item1 in item.Errors)
                {
                    ModelState.AddModelError(string.Empty, item1.ErrorMessage);
                }
            }


        }
        Input.RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
        {
            Text = i,
            Value = i

        });
        Input.CompanyList = _unitOfWork.Company.GetAll().Select(i=>new SelectListItem{
            Text  = i.Name,
            Value = i.Id.ToString()
        });

        return Page();
    }
    private ApplicationUser CreateUser() {
        try {
            return Activator.CreateInstance<ApplicationUser>();
        }catch {
            throw new Exception("Can't create instance ");
        }
    }

    
        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }


}