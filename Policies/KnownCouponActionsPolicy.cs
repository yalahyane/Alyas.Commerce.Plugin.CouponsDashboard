namespace Alyas.Commerce.Plugin.CouponsDashboard.Policies
{
    public class KnownCouponActionsPolicy : Sitecore.Commerce.Plugin.Coupons.KnownCouponActionsPolicy
    {
        public string PaginateCouponList { get; set; } = nameof(PaginateCouponList);
    }
}
