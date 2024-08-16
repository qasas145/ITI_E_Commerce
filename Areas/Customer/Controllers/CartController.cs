using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Area("Customer")]
[Authorize]

public class CartController : Controller
{
    private readonly ILogger<HomeController> _logger;

    [BindProperty]
    private ShoppingCartVM ShoppingCartVM{get;set;}
    private readonly IUnitOfWork _unitOfWork;

    public CartController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        // Console.WriteLine(ca)
        ShoppingCartVM = new() {
            ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(u=>u.ApplicationUserId == userId, includeProperties : "Product"),
            OrderHeader = new(),
        };
    
        IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

        ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u=>u.Id == userId);
        foreach (var cart in ShoppingCartVM.ShoppingCarts)
        {
            cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        }
        return View(ShoppingCartVM);
    }

    public IActionResult Summary() {
        
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM = new()
			{
				ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
				includeProperties: "Product"),
				OrderHeader = new()
			};

			ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            ShoppingCartVM.OrderHeader.ApplicaionUserId = ShoppingCartVM.OrderHeader.ApplicationUser.Id;

			ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
			ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
			ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.PaymentStatusDelayedPayment;
			ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;



			foreach (var cart in ShoppingCartVM.ShoppingCarts)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}
			return View(ShoppingCartVM);
    }	
    [HttpPost]	
    public IActionResult Summary(ShoppingCartVM shoppingCartVM ) { // it won't pass the shopping cart's because i havn't make it asp-for in the index.html
        if (shoppingCartVM.OrderHeader != null && shoppingCartVM.OrderHeader.OrderTotal != 0) {
                _unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
                _unitOfWork.Save();
                TempData["sucess"] = "The order has been made sucessfully ";
        }
        return RedirectToAction("Summary");
    }
    public IActionResult Plus(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
        cartFromDb.Count += 1;
        _unitOfWork.ShoppingCart.Update(cartFromDb);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Minus(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
        if (cartFromDb.Count <= 1)
        {

            //remove that from cart

            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
    
        }
        else
        {
            cartFromDb.Count -= 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
        }

        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Remove(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);

        _unitOfWork.ShoppingCart.Remove(cartFromDb);

        HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
            .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);

        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }



    private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
    {
        if (shoppingCart.Count <= 50)
        {
            return shoppingCart.Product.Price;
        }
        else
        {
            if (shoppingCart.Count <= 100)
            {
                return shoppingCart.Product.Price50;
            }
            else
            {
                return shoppingCart.Product.Price100;
            }
        }
    }

    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
}
