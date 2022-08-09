namespace Alyas.Commerce.Plugin.CouponsDashboard.Pipelines.Blocks
{
    using System;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Coupons;
    using Sitecore.Framework.Conditions;
    using KnownCouponViewsPolicy = Policies.KnownCouponViewsPolicy;

    public class DoActionPaginatePublicCouponsBlock : DoActionPaginateListBlock
    {
        public DoActionPaginatePublicCouponsBlock(CommerceCommander commerceCommander) : base(commerceCommander)
        {
        }

        public override async Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull(this.Name + ": The argument cannot be null.");
            if (string.IsNullOrEmpty(arg.Name) || !arg.Name.Equals(context.GetPolicy<KnownCouponViewsPolicy>().PublicCouponUsage, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(arg.Action) || !arg.Action.Equals(context.GetPolicy<Policies.KnownCouponActionsPolicy>().PaginatePublicCoupons, StringComparison.OrdinalIgnoreCase) || !this.Validate(arg, context.CommerceContext))
                return arg;

            foreach (var coupon in await this.GetEntities<Coupon>(arg, context).ConfigureAwait(false))
            {
                var summaryEntityView = new EntityView
                {
                    EntityId = coupon.Id,
                    ItemId = coupon.Id,
                    DisplayName = coupon.DisplayName,
                    Name = context.GetPolicy<KnownCouponViewsPolicy>().Summary
                };
                var itemIdProperty = new ViewProperty
                {
                    Name = "ItemId",
                    RawValue = coupon.Id,
                    IsHidden = true,
                    IsReadOnly = true,
                    DisplayName = "Item Id"
                };
                summaryEntityView.Properties.Add(itemIdProperty);

                var codeProperty = new ViewProperty
                {
                    Name = "Code",
                    RawValue = coupon.Code,
                    IsReadOnly = true,
                    UiType = "EntityLink",
                    DisplayName = "Coupon Code"
                };
                summaryEntityView.Properties.Add(codeProperty);

                var couponTypeProperty = new ViewProperty
                {
                    Name = "CouponType",
                    RawValue = coupon.CouponType,
                    IsReadOnly = true,
                    DisplayName = "Coupon Type"
                };
                summaryEntityView.Properties.Add(couponTypeProperty);

                var promotionProperty = new ViewProperty
                {
                    Name = "Promotion",
                    RawValue = coupon.Promotion.Name,
                    IsReadOnly = true,
                    DisplayName = "Promotion Name",
                };
                summaryEntityView.Properties.Add(promotionProperty);

                var usageCountProperty = new ViewProperty
                {
                    Name = "UsageCount",
                    RawValue = coupon.UsageCount,
                    IsReadOnly = true,
                    DisplayName = "Usage Count"
                };
                summaryEntityView.Properties.Add(usageCountProperty);

                var dateCreatedProperty = new ViewProperty
                {
                    Name = "DateCreated",
                    RawValue = coupon.DateCreated,
                    IsReadOnly = true,
                    DisplayName = "Date Created"
                };
                summaryEntityView.Properties.Add(dateCreatedProperty);


                var dateUpdatedProperty = new ViewProperty
                {
                    Name = "DateUpdated",
                    RawValue = coupon.DateUpdated,
                    IsReadOnly = true,
                    DisplayName = "Date Updated"
                };
                summaryEntityView.Properties.Add(dateUpdatedProperty);

                arg.ChildViews.Add(summaryEntityView);
            }

            return arg;
        }
    }
}
