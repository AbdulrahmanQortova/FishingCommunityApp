using FishingCommunity.Application.Features.Shop.Checkout.Commands.Checkout;
using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Infrastructure.Persistence;
using FishingCommunity.Infrastructure.Persistence.Repositories;
using FishingCommunity.UnitTests.Common;
using FluentAssertions;
using Moq;
using Xunit;
using FishingCommunity.Domain.Interfaces;
using Cart = FishingCommunity.Domain.Entities.Shop.Cart;
using Order = FishingCommunity.Domain.Entities.Shop.Order;

namespace FishingCommunity.UnitTests.Application.Shop;

public class CheckoutCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly CheckoutCommandHandler _handler;

    public CheckoutCommandHandlerTests()
    {
        _dbContext = InMemoryDbContextFactory.Create();

        _unitOfWorkMock.Setup(u => u.Repository<Cart>()).Returns(new Repository<Cart>(_dbContext));
        _unitOfWorkMock.Setup(u => u.Repository<Product>()).Returns(new Repository<Product>(_dbContext));
        _unitOfWorkMock.Setup(u => u.Repository<Order>()).Returns(new Repository<Order>(_dbContext));
        _unitOfWorkMock.Setup(u => u.Repository<Coupon>()).Returns(new Repository<Coupon>(_dbContext));
        _unitOfWorkMock.Setup(u => u.Repository<ShippingAddress>()).Returns(new Repository<ShippingAddress>(_dbContext));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(() => _dbContext.SaveChangesAsync());

        _handler = new CheckoutCommandHandler(_unitOfWorkMock.Object);
    }

    private async Task<(Product product, Cart cart, ShippingAddress address)> SeedCartWithOneItemAsync(Guid userId, int stock = 10, int quantity = 2, decimal price = 50m)
    {
        var store = new Store(Guid.NewGuid(), "Test Store");
        store.Approve();
        var category = new Category("Test Category");
        _dbContext.Stores.Add(store);
        _dbContext.Categories.Add(category);

        var product = new Product(store.Id, category.Id, "Fishing Rod", price, stock);
        _dbContext.Products.Add(product);

        var address = new ShippingAddress(userId, "Ahmed", "01000000000", "123 Street", "Cairo", "Egypt");
        _dbContext.ShippingAddresses.Add(address);

        var cart = new Cart(userId);
        cart.AddItem(product.Id, quantity, price);
        _dbContext.Carts.Add(cart);

        await _dbContext.SaveChangesAsync();

        return (product, cart, address);
    }

    [Fact]
    public async Task Handle_WithValidCart_CreatesOrderAndReservesStock()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var (product, _, address) = await SeedCartWithOneItemAsync(userId, stock: 10, quantity: 3);

        var command = new CheckoutCommand
        {
            UserId = userId,
            ShippingAddressId = address.Id,
            PaymentMethod = PaymentMethod.CashOnDelivery
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Data!.TotalAmount.Should().Be(150m); // 3 * 50

        var reloadedProduct = await _dbContext.Products.FindAsync(product.Id);
        reloadedProduct!.StockQuantity.Should().Be(7); // 10 - 3

        var reloadedCart = _dbContext.Carts.First(c => c.UserId == userId);
        reloadedCart.Items.Should().BeEmpty(); // Cart cleared after checkout.
    }

    [Fact]
    public async Task Handle_WhenRequestedQuantityExceedsStock_ReturnsFailureAndDoesNotReserveStock()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var (product, _, address) = await SeedCartWithOneItemAsync(userId, stock: 1, quantity: 5); // Cart wants 5, only 1 in stock.

        var command = new CheckoutCommand { UserId = userId, ShippingAddressId = address.Id };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Only 1 unit(s)"));

        var reloadedProduct = await _dbContext.Products.FindAsync(product.Id);
        reloadedProduct!.StockQuantity.Should().Be(1); // Unchanged — no partial reservation happened.
    }

    [Fact]
    public async Task Handle_WhenCartIsEmpty_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var address = new ShippingAddress(userId, "Ahmed", "01000000000", "123 Street", "Cairo", "Egypt");
        _dbContext.ShippingAddresses.Add(address);
        await _dbContext.SaveChangesAsync();

        var command = new CheckoutCommand { UserId = userId, ShippingAddressId = address.Id };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("Your cart is empty.");
    }

    [Fact]
    public async Task Handle_WithValidCoupon_AppliesDiscountCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var (_, _, address) = await SeedCartWithOneItemAsync(userId, stock: 10, quantity: 2, price: 100m); // Subtotal = 200

        var coupon = new Coupon("SAVE10", DiscountType.Percentage, 10m, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        _dbContext.Coupons.Add(coupon);
        await _dbContext.SaveChangesAsync();

        var command = new CheckoutCommand { UserId = userId, ShippingAddressId = address.Id, CouponCode = "SAVE10" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Data!.SubtotalAmount.Should().Be(200m);
        result.Data.DiscountAmount.Should().Be(20m); // 10% of 200
        result.Data.TotalAmount.Should().Be(180m);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}