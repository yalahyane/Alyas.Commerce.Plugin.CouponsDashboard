namespace Alyas.Commerce.Plugin.CouponsDashboard.Pipelines.Blocks
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Coupons;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Framework.Pipelines;
    using KnownCouponsListsPolicy = Policies.KnownCouponsListsPolicy;

    public class CreatePublicAndPrivateListsBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commander;
        public CreatePublicAndPrivateListsBlock(CommerceCommander commander)
        {
            this._commander = commander;
        }
        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            var coupons = await this._commander.Pipeline<IFindEntitiesInListPipeline>().Run(new FindEntitiesInListArgument(typeof(Coupon), context.GetPolicy<KnownCouponsListsPolicy>().Coupons, 0, int.MaxValue), context).ConfigureAwait(false);
            foreach (var entity in coupons.List.Items)
            {
                var coupon = (Coupon)entity;
                try
                {
                    var membershipsComponent = coupon.GetComponent<ListMembershipsComponent>();
                    
                    
                    if (coupon.CouponType.Equals("Private", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!membershipsComponent.Memberships.Any(m =>
                                m.Equals(context.GetPolicy<KnownCouponsListsPolicy>().PrivateCouponsList,
                                    StringComparison.CurrentCultureIgnoreCase)))
                        {
                            membershipsComponent.Memberships.Add(context.GetPolicy<KnownCouponsListsPolicy>().PrivateCouponsList);
                        }
                    }
                    else
                    {
                        if (!membershipsComponent.Memberships.Any(m =>
                                m.Equals(context.GetPolicy<KnownCouponsListsPolicy>().PrivateCouponsList,
                                    StringComparison.CurrentCultureIgnoreCase)))
                        {
                            membershipsComponent.Memberships.Add(context.GetPolicy<KnownCouponsListsPolicy>().PublicCouponsList);
                        }
                    }

                    coupon.IsPersisted = false;
                    await this._commander.Pipeline<IPersistEntityPipeline>().Run(new PersistEntityArgument(coupon), context);
                }
                catch (Exception e)
                {
                    context.Logger.LogError($"Failed to move Coupon {coupon.CouponType}. Exception: {e}");
                }
                
            }

            return arg;
        }
    }
}
