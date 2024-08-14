public interface IShoppingCart : IRepository<ShoppingCart> {
    public void Update(ShoppingCart obj);
}