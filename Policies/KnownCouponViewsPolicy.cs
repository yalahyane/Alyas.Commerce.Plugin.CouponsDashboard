namespace Alyas.Commerce.Plugin.CouponsDashboard.Policies
{
    using Sitecore.Commerce.Plugin.Coupons;

    public class KnownCouponViewsPolicy : Sitecore.Commerce.Plugin.Coupons.KnownCouponViewsPolicy
    {
        public string CouponDashboard { get; set; } = nameof(CouponDashboard);
        public string CouponUsage { get; set; } = nameof(CouponUsage);
        public string CouponUsageDetails { get; set; } = nameof(CouponUsageDetails);
        public string Summary { get; set; } = nameof(Summary);
        public string CouponOrders { get; set; } = nameof(CouponOrders);
        public string Master { get; set; } = nameof(Master);
    }
}
