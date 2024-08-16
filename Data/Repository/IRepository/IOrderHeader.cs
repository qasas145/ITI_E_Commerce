public interface IOrderHeader : IRepository<OrderHeader> {
    public void Update(OrderHeader obj);
    
    void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
    void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
}