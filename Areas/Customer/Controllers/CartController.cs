using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

            // ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now.Date;  // it won't be passed to th post view because it's not in the html file as a asp-for 
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

             // Retrieve and convert errors from TempData
            if (TempData["errors"] is string errorsString)
            {
                var errorsList = errorsString.Split(new[] { ", " }, StringSplitOptions.None).ToList();
                ViewBag.Errors = errorsList;
            }

			return View(ShoppingCartVM);
    }	
    [HttpPost]	
    public IActionResult Summary(ShoppingCartVM shoppingCartVM ) { // it won't pass the shopping cart's because i havn't make it asp-for in the index.html

        
        var errors = new List<string>();
        if (ModelState.IsValid) {
            if ( shoppingCartVM.OrderHeader.OrderTotal != 0) {

                shoppingCartVM.OrderHeader.OrderDate = DateTime.Now.Date;
                // this related to the one who will use the app (it's customized)
                shoppingCartVM.OrderHeader.ShippingDate = DateTime.Now.AddDays(2);
                shoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(2);


                _unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
                _unitOfWork.Save();
                TempData["success"] = "The order has been made sucessfully ";
                TempData.Remove("errors");  
                _logger.LogInformation("The order has been made sucessfully.");
            }
            else {
                errors.Add("There's no orders to add .");
            }
        }
        
        else {
            foreach(var obj in ModelState.Values) {
                foreach (var error in obj.Errors)
                {
                    errors.Add(error.ErrorMessage);
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }
            }
        }
        TempData["errors"] = string.Join(", ", errors);
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

}
