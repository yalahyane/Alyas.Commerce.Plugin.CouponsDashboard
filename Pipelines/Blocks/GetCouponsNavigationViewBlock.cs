namespace Alyas.Commerce.Plugin.CouponsDashboard.Pipelines.Blocks
{
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    public class GetCouponsNavigationViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull(Name + ": The argument cannot be null.");
            var couponDashboardEntityView = new EntityView
            {
                Name = context.GetPolicy<Policies.KnownCouponViewsPolicy>().CouponDashboard,
                ItemId = context.GetPolicy<Policies.KnownCouponViewsPolicy>().CouponDashboard,
                Icon = "receipt_book",
                DisplayRank = 6,
                DisplayName = "Coupons"
            };
            arg.ChildViews.Add(couponDashboardEntityView);
            return Task.FromResult(arg);
        }
    }
}
;