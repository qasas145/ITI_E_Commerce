using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


[Area("Admin")]
[Authorize(Roles ="Admin")]
public class ProductController : Controller{
    
        private readonly IUnitOfWork _unitOfWork; // when he passes how won't pass the interface but an instance from the class that has implemented this interface like new Fileogger();
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index() {
            // get all products 
            List<Product> res = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();

            return View(res);
        }
        public IActionResult UpSert(int? id ) {
            // upsert the product
            ProductVM ProductVM = new() {
                CategoriesList = _unitOfWork.Category.GetAll().Select(i=>new SelectListItem{
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == 0 || id == null) {
                return View(ProductVM); 
            }
            else {
                ProductVM.Product = _unitOfWork.Product.Get(i=>i.Id == id, includeProperties:"Category");
                return View(ProductVM);
            }
        }

        [HttpPost]

        public IActionResult UpSert(ProductVM productVM, List<IFormFile> files) {
            if (ModelState.IsValid) {
                Console.WriteLine("The model is calud ");
                if (productVM.Product.Id==0)
                {
                    _unitOfWork.Product.Add(productVM.Product);

                    TempData["success"] = "Product Created Successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);

                    TempData["success"] = "Product Updated Successfully";
                }
                _unitOfWork.Save();
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (files != null)
                {

                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images/products/product-" + productVM.Product.Id; // if you want for windows then replace the / with \ 
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"/" + productPath + @"/" + fileName,
                            ProductId = productVM.Product.Id,
                        };

                        if (productVM.Product.ProductImages == null)
                            productVM.Product.ProductImages = new List<ProductImage>();

                        productVM.Product.ProductImages.Add(productImage);

                    }

                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();

                }

                TempData["success"] = "Product created/updated successfully";
                
                return RedirectToAction("Index");
            }
            else {
                productVM.CategoriesList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
            
        }
        
        [HttpGet]
        public IActionResult GetAll() {
            List<Product> res = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return Json(new { data = res });
        }
        
        [HttpDelete]
        public IActionResult Delete(int? id ) {
            Product p = _unitOfWork.Product.Get(i=>i.Id == id);
            if ( p == null )
            {
                return Json(new { success = false, Message = "Error While Deleting" });
            };
            _unitOfWork.Product.Remove(p);
            
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }


}