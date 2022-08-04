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
    using Sitecore.Framework.Conditions;

    public class GetCouponOrdersViewBlock : GetTableViewBlock
    {
        private readonly FindEntitiesInListCommand _findEntitiesInListCommand;
        private readonly GetOnHoldOrderCartCommand _getOnHoldOrderCartCommand;
        public GetCouponOrdersViewBlock(FindEntitiesInListCommand findEntitiesInListPipeline, GetOnHoldOrderCartCommand getOnHoldOrderCartCommand)
        {
            this._findEntitiesInListCommand = findEntitiesInListPipeline;
            this._getOnHoldOrderCartCommand = getOnHoldOrderCartCommand;
        }
        public override async Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull(Name + ": The argument cannot be null");
            var entityViewArgument = context.CommerceContext.GetObject<EntityViewArgument>();
            var viewsPolicy = context.GetPolicy<Policies.KnownCouponViewsPolicy>();
            var orderViewsPolicy = context.GetPolicy<KnownOrderViewsPolicy>();
            if (string.IsNullOrEmpty(entityViewArgument?.ViewName) || !entityViewArgument.ViewName.EqualsOrdinalIgnoreCase(viewsPolicy.CouponOrders) && !entityViewArgument.ViewName.EqualsOrdinalIgnoreCase(viewsPolicy.Master) || !(entityViewArgument.Entity is Coupon))
                return arg;

            var entity = (Coupon)entityViewArgument.Entity;
            var ordersView = arg;
            if (entityViewArgument.ViewName.EqualsOrdinalIgnoreCase(viewsPolicy.Master))
            {
                ordersView = new EntityView
                {
                    EntityId = arg.EntityId,
                    Name = viewsPolicy.CouponOrders,
                    UiHint = "Table",
                    DisplayName = "Coupon Orders"
                };
                
                arg.ChildViews.Add(ordersView);
            }

            var listName = $"ORDER_{entity.Code}";
            var paginationPolicy = context.GetPolicy<PaginationPolicy>();
            var result = await this._findEntitiesInListCommand.Process<Order>(context.CommerceContext, listName, 0, paginationPolicy.PageSize).ConfigureAwait(false);
            if (result == null || !result.Items.Any())
                return arg;
            foreach (var order in result.Items)
                await ordersView.AddOrderChildViewAsync(this._getOnHoldOrderCartCommand, context.CommerceContext, orderViewsPolicy.Summary, arg.EntityId, order).ConfigureAwait(false);
            this.SetPagingMetadata(ordersView, result.TotalItemCount, "DoActionPaginateCouponOrders", context);
            return arg;
        }
    }
}
