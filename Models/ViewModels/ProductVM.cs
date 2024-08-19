using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ProductVM {
    public Product Product{get;set;}
    [ValidateNever]
    public IEnumerable<SelectListItem> CategoriesList{get;set;}
}