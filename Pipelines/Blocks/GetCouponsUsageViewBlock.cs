﻿namespace Alyas.Commerce.Plugin.CouponsDashboard.Pipelines.Blocks
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Coupons;
    using Sitecore.Framework.Conditions;

    public class GetCouponsUsageViewBlock : GetListViewBlock
    {
        public GetCouponsUsageViewBlock(CommerceCommander commerceCommander) : base(commerceCommander)
        {
        }
        public override async Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull(Name + ": The argument cannot be null.");
            var entityViewArgument = context.CommerceContext.GetObjects<EntityViewArgument>().FirstOrDefault();

            if (string.IsNullOrEmpty(entityViewArgument?.ViewName) || !entityViewArgument.ViewName.Equals(context.GetPolicy<Policies.KnownCouponViewsPolicy>().CouponUsage, StringComparison.OrdinalIgnoreCase) &&  !entityViewArgument.ViewName.Equals(context.GetPolicy<Policies.KnownCouponViewsPolicy>().CouponDashboard, StringComparison.OrdinalIgnoreCase))
                return arg;

            var entityView = new EntityView
            {
                EntityId = string.Empty,
                ItemId = string.Empty,
                Name = context.GetPolicy<Policies.KnownCouponViewsPolicy>().CouponUsage,
                DisplayName = "Coupons Usage"
            };
            arg.ChildViews.Add(entityView);
            var listName = context.GetPolicy<Policies.KnownCouponsListsPolicy>().Coupons;
            await this.SetListMetadata(entityView, listName, context.GetPolicy<Policies.KnownCouponActionsPolicy>().PaginateCouponList, context).ConfigureAwait(false);
            foreach (var coupon in (await this.GetEntities<Coupon>(entityView, listName, context).ConfigureAwait(false)).OfType<Coupon>())
            {
                var summaryEntityView = new EntityView
                {
                    EntityId = coupon.Id,
                    ItemId = coupon.Id,
                    DisplayName = coupon.DisplayName,
                    Name = context.GetPolicy<Policies.KnownCouponViewsPolicy>().Summary
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

                entityView.ChildViews.Add(summaryEntityView);
            }

            return arg;
        }
    }
}
