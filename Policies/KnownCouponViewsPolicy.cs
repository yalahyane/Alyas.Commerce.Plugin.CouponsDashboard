namespace Alyas.Commerce.Plugin.CouponsDashboard.Policies
{
    public class KnownCouponViewsPolicy : Sitecore.Commerce.Plugin.Coupons.KnownCouponViewsPolicy
    {
        public string CouponDashboard { get; set; } = nameof(CouponDashboard);
        public string PublicCouponUsage { get; set; } = nameof(PublicCouponUsage);
        public string PrivateCouponUsage { get; set; } = nameof(PrivateCouponUsage);
        public string CouponUsageDetails { get; set; } = nameof(CouponUsageDetails);
        public string Summary { get; set; } = nameof(Summary);
        public string CouponOrders { get; set; } = nameof(CouponOrders);
        public string Master { get; set; } = nameof(Master);
    }
}
