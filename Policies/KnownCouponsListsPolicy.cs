namespace Alyas.Commerce.Plugin.CouponsDashboard.Policies
{
    public class KnownCouponsListsPolicy : Sitecore.Commerce.Plugin.Coupons.KnownCouponsListsPolicy
    {
        public string Coupons { get; set; } = nameof(Coupons);
        public string PublicCouponsList { get; set; } = "PublicCoupons";
        public string PrivateCouponsList { get; set; } = "PrivateCoupons";
    }
}
