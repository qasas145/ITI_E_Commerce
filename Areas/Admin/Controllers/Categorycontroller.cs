using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


[Area("Admin")]
[Authorize(Roles ="Admin")]
public class CategoryController : Controller {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    public CategoryController(IUnitOfWork unitOfWork) {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index() {
        // get all categories
        List<Category> res = _unitOfWork.Category.GetAll().ToList();
        return View(res);
    }
    public IActionResult Create() {
        return View();
    }
    [HttpPost]
    public IActionResult Create(Category obj) {
        if (obj.Name == obj.DisplayOrder.ToString())
        {
            ModelState.AddModelError("Name", "The DisplayOrder cannot exactly match the Name.");
        }

        if (obj.Name != null && obj.Name.ToLower() == "test")
        {
            ModelState.AddModelError("Name", "Test is invalid value");
        }
        if (ModelState.IsValid) {
            _unitOfWork.Category.Add(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category Created Successfully";
            return RedirectToAction("Index");
        }
        return View();
    }
    

    public IActionResult Update(int? id ) {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
        /*
        Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u => u.Id == id);
        Category? categoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();*/

        if (categoryFromDb == null)
        {
            return NotFound();
        }
        return View(categoryFromDb);
    }   
    [HttpPost]
    public IActionResult Update(Category cat) {
        if (ModelState.IsValid) {
            _unitOfWork.Category.Update(cat);
            _unitOfWork.Save();
            TempData["success"] = "Category Updated Successfully";
            
            return RedirectToAction("Index");
        }
        return View();
    }
    public IActionResult Delete(int? id ) {
        if (id == null || id == 0) {
            return NotFound();
        }
        Category? res = _unitOfWork.Category.Get(i=>i.Id == id);
        
        if (res == null) {
            return NotFound();
        }
        return View(res);
    }
    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePost(int? id ) {
        Category? res = _unitOfWork.Category.Get(i=>i.Id == id);
        if (res == null) {
            return NotFound();
        }
        _unitOfWork.Category.Remove(res);
        _unitOfWork.Save();
        TempData["success"] = "Category Deleted Successfully";

            return RedirectToAction("Index");
    }
}