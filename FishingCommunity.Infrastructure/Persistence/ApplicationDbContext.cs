using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Entities.Identity;
using FishingCommunity.Domain.Entities.Notifications;
using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Entities.Trips;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FishingCommunity.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    // Trip module
    public DbSet<Boat> Boats => Set<Boat>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<TripBooking> TripBookings => Set<TripBooking>();
    public DbSet<TripWaitingListEntry> TripWaitingListEntries => Set<TripWaitingListEntry>();
    public DbSet<TripReview> TripReviews => Set<TripReview>();

    // Community module
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<PostReaction> PostReactions => Set<PostReaction>();
    public DbSet<PostReport> PostReports => Set<PostReport>();
    public DbSet<SavedPost> SavedPosts => Set<SavedPost>();
    public DbSet<Follow> Follows => Set<Follow>();


    // Notification module
    public DbSet<Notification> Notifications => Set<Notification>();



    // Shop module
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductReview> ProductReviews => Set<ProductReview>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<ShippingAddress> ShippingAddresses => Set<ShippingAddress>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Ignore<DomainEvent>();

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        ConfigureIdentityTableNames(builder);
    }

    private static void ConfigureIdentityTableNames(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<ApplicationRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
    }
}