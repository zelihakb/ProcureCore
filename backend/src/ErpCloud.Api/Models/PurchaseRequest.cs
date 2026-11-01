namespace ErpCloud.Api.Models;

public class PurchaseRequest
{
    public int Id { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }

    public PurchaseRequest(string productName, int quantity)
    {
        ProductName = productName;
        Quantity = quantity;
    }
}