public interface IUnitOfWork {
    public ICategory Category{get;}
    public IOrderDetails OrderDetails{get;}
    public IOrderHeader OrderHeader{get;}
    public IProduct Product{get;}

    public IShoppingCart ShoppingCart{get;}
    public ICompany Company{get;}
    public IProductImage ProductImage{get;}

    public IApplicationUser ApplicationUser{get;}

    public void Save();
}