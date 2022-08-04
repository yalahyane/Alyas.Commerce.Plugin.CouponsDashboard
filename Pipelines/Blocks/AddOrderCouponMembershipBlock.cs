namespace Alyas.Commerce.Plugin.CouponsDashboard.Pipelines.Blocks
{
    using System.Linq;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Coupons;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Framework.Pipelines;

    public class AddOrderCouponMembershipBlock : PipelineBlock<Order, Order, CommercePipelineExecutionContext>
    {
        public override Task<Order> Run(Order order, CommercePipelineExecutionContext context)
        {
            if (order.HasComponent<CartCouponsComponent>())
            {
                var couponsComponent = order.GetComponent<CartCouponsComponent>();
                if (couponsComponent.List != null && couponsComponent.List.Any())
                {
                    var membershipsComponent = order.GetComponent<ListMembershipsComponent>();
                    foreach (var coupon in couponsComponent.List)
                    {
                        membershipsComponent.Memberships.Add($"Order_{coupon.CouponId}");
                    }
                }
            }

            return Task.FromResult(order);
        }
    }
}
