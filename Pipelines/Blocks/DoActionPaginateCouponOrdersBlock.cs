namespace Alyas.Commerce.Plugin.CouponsDashboard.Pipelines.Blocks
{
    using System.Linq;
    using System.Threading.Tasks;
    using Extensions;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Coupons;
    using Sitecore.Commerce.Plugin.Orders;

    public class DoActionPaginateCouponOrdersBlock : DoActionPaginateEntityBlock<Coupon>
    {
        private readonly GetOnHoldOrderCartCommand _getOnHoldOrderCartCommand;
        private readonly FindEntitiesInListCommand _findEntitiesInListCommand;

        public DoActionPaginateCouponOrdersBlock(FindEntitiesInListCommand findEntitiesInListCommand, GetOnHoldOrderCartCommand getOnHoldOrderCartCommand, CommerceCommander commerceCommander) : base("DoActionPaginateCouponOrders", commerceCommander)
        {
            this._getOnHoldOrderCartCommand = getOnHoldOrderCartCommand;
            this._findEntitiesInListCommand = findEntitiesInListCommand;
        }

        protected override async Task PopulateEntityViewAsync(EntityView view, Coupon entity, CommercePipelineExecutionContext context)
        {
            var listName = $"ORDER_{entity.Code}";
            var commerceList = await _findEntitiesInListCommand.Process<Order>(context.CommerceContext, listName, Skip, Top).ConfigureAwait(false);
            if (commerceList == null || !commerceList.Items.Any())
                return;
            foreach (Order order in commerceList.Items)
                await view.AddOrderChildViewAsync(_getOnHoldOrderCartCommand, context.CommerceContext, context.GetPolicy<KnownOrderViewsPolicy>().Summary, view.EntityId, order).ConfigureAwait(false);
        }
    }
}
