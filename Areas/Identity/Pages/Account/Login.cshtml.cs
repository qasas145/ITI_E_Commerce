using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LoginModel  : PageModel{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input{get;set;}
    public string ReturnUrl{get;set;}


    [TempData]
    public string ErrorMessage{get;set;}
    public class InputModel {

        [EmailAddress]
        public string Email{get;set;}
        [DataType(DataType.Password)]
        public string Password{get;set;}

        [Display(Name = "Remember me?")]
        public bool RememberMe{get;set;}
    }
    public async Task OnGetAsync(string returnUrl = null ) {
        
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

    
        returnUrl ??= Url.Content("~/");
        ReturnUrl = returnUrl;
    }
    public async Task<IActionResult> OnPostAsync(string returnUrl = null) {
        
        Console.WriteLine("The return url is {0}", ReturnUrl);
        returnUrl ??= Url.Content("~/");
        if(ModelState.IsValid) {
            var res = await _signInManager.PasswordSignInAsync(Input.Email,Input.Password,Input.RememberMe, lockoutOnFailure: false);
            if (res.Succeeded) {
                
                _logger.LogInformation("User logged in.");
                return LocalRedirect(returnUrl);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }
        }
        return Page();
    }
}