namespace Alyas.Commerce.Plugin.CouponsDashboard.Policies
{
    public class KnownCouponActionsPolicy : Sitecore.Commerce.Plugin.Coupons.KnownCouponActionsPolicy
    {
        public string PaginatePublicCoupons { get; set; } = nameof(PaginatePublicCoupons);
        public string PaginatePrivateCoupons { get; set; } = nameof(PaginatePrivateCoupons);
    }
}
