namespace ErpCloud.Api.DTOs;

public record CreatePurchaseRequestDto(
    string ProductName,
    int Quantity
);