using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Shop.Orders.Queries.GetMyOrders;

public class MyOrderDto
{
    public Guid Id { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public int ItemsCount { get; set; }
    public DateTime CreatedDate { get; set; }
}