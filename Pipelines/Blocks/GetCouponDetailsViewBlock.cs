namespace Alyas.Commerce.Plugin.CouponsDashboard.Pipelines.Blocks
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Coupons;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    public class GetCouponDetailsViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly FindEntitiesInListCommand _findEntitiesInListCommand;

        public GetCouponDetailsViewBlock(FindEntitiesInListCommand findEntitiesInListCommand)
        {
            this._findEntitiesInListCommand = findEntitiesInListCommand;
        }

        public override async Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull(Name + ": The argument cannot be null");
            var entityViewArgument = context.CommerceContext.GetObject<EntityViewArgument>();
            var viewsPolicy = context.GetPolicy<Policies.KnownCouponViewsPolicy>();
            context.GetPolicy<KnownOrderViewsPolicy>();
            if (string.IsNullOrEmpty(entityViewArgument?.ViewName) || !entityViewArgument.ViewName.EqualsOrdinalIgnoreCase(viewsPolicy.CouponUsageDetails) && !entityViewArgument.ViewName.EqualsOrdinalIgnoreCase(viewsPolicy.Master) || !(entityViewArgument.Entity is Coupon))
                return arg;

            var coupon = (Coupon)entityViewArgument.Entity;
            var entityView = new EntityView
            {
                EntityId = coupon.Promotion.EntityTarget,
                ItemId = string.Empty,
                Name = context.GetPolicy<Policies.KnownCouponViewsPolicy>().CouponUsageDetails,
                DisplayName = "Coupon Details"
            };
            var codeProperty = new ViewProperty
            {
                Name = "Code",
                RawValue = coupon.Code,
                IsReadOnly = true,
                DisplayName = "Coupon Code"
            };
            entityView.Properties.Add(codeProperty);

            var couponTypeProperty = new ViewProperty
            {
                Name = "CouponType",
                RawValue = coupon.CouponType,
                IsReadOnly = true,
                DisplayName = "Coupon Type"
            };
            entityView.Properties.Add(couponTypeProperty);

            var promotionProperty = new ViewProperty
            {
                Name = "Promotion",
                RawValue = coupon.Promotion.Name,
                IsReadOnly = true,
                DisplayName = "Promotion Name",
                UiType = "EntityLink"
            };
            entityView.Properties.Add(promotionProperty);

            var usageCountProperty = new ViewProperty
            {
                Name = "UsageCount",
                RawValue = coupon.UsageCount,
                IsReadOnly = true,
                DisplayName = "Usage Count"
            };
            entityView.Properties.Add(usageCountProperty);

            var listName = $"ORDER_{coupon.Code}";
            var result = await this._findEntitiesInListCommand.Process<Order>(context.CommerceContext, listName, 0, int.MaxValue).ConfigureAwait(false);
            if (result != null && result.Items.Any())
            {
                var currencyCode = result.Items.FirstOrDefault()?.Totals.GrandTotal.CurrencyCode;
                var total = result.Items.Sum(x => x.Totals.GrandTotal.Amount);
                var cartLevelDiscountTotal = result.Items.Sum(x =>
                    x.Adjustments.Where(a => a.AdjustmentType.Equals("Discount", StringComparison.OrdinalIgnoreCase))
                        .Sum(y => y.Adjustment.Amount));

                var cartLineLevelDiscountTotal = result.Items.Sum(x => x.Lines.Sum(l=> l.Adjustments.Where(a => a.AdjustmentType.Equals("Discount", StringComparison.OrdinalIgnoreCase))
                        .Sum(y => y.Adjustment.Amount)));

                var discountTotal = cartLevelDiscountTotal + cartLineLevelDiscountTotal;

                var totalBeforeDiscountProperty = new ViewProperty
                {
                    Name = "TotalBeforeDiscount",
                    RawValue = new Money(currencyCode, total - discountTotal),
                    IsReadOnly = true,
                    DisplayName = "Total Before Discount"
                };
                entityView.Properties.Add(totalBeforeDiscountProperty);

                var totalDiscountProperty = new ViewProperty
                {
                    Name = "TotalDiscount",
                    RawValue = new Money(currencyCode, discountTotal),
                    IsReadOnly = true,
                    DisplayName = "Total Discount"
                };
                entityView.Properties.Add(totalDiscountProperty);

                var totalAfterDiscountProperty = new ViewProperty
                {
                    Name = "TotalAfterDiscount",
                    RawValue = new Money(currencyCode, total),
                    IsReadOnly = true,
                    DisplayName = "Total After Discount"
                };
                entityView.Properties.Add(totalAfterDiscountProperty);
            }


            arg.ChildViews.Add(entityView);
            return arg;
        }
    }
}
