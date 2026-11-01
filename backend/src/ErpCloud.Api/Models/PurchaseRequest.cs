namespace ErpCloud.Api.Models;

public class PurchaseRequest
{
    public int Id { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }

    public PurchaseRequest(int id, string productName, int quantity)
    {
        Id = id;
        ProductName = productName;
        Quantity = quantity;
    }
}