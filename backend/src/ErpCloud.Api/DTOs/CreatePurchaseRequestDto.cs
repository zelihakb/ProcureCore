namespace ErpCloud.Api.DTOs;

public record CreatePurchaseRequestDto(
    int Id,
    string ProductName,
    int Quantity
);