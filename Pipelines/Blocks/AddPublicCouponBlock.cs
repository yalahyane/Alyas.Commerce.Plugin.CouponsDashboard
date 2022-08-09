namespace Alyas.Commerce.Plugin.CouponsDashboard.Pipelines.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Coupons;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    public class AddPublicCouponBlock : PipelineBlock<AddPublicCouponArgument, Coupon, CommercePipelineExecutionContext>
    {
        private readonly IFindEntityPipeline _findEntityPipeline;

        public AddPublicCouponBlock(IFindEntityPipeline findEntityPipeline)
        {
            _findEntityPipeline = findEntityPipeline;
        }

        public override async Task<Coupon> Run(
          AddPublicCouponArgument arg,
          CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull(Name + ": The block argument cannot be null.");
            Condition.Requires(arg.Promotion).IsNotNull(Name + ": The promotion cannot be null.");
            Condition.Requires(arg.Code).IsNotNullOrEmpty(Name + ": The coupon code cannot be null or empty.");
            var promotion = arg.Promotion;
            var couponCodeLength = context.GetPolicy<GlobalCouponsPolicy>().MaxCouponCodeLength;
            CommercePipelineExecutionContext executionContext;
            if (arg.Code.Length > couponCodeLength)
            {
                executionContext = context;
                var commerceContext = context.CommerceContext;
                var error = context.GetPolicy<KnownResultCodes>().Error;
                var args = new object[]
                {
          arg.Code,
          couponCodeLength
                };
                var defaultMessage = $"'{arg.Code}' exceeds {couponCodeLength} characters.";
                executionContext.Abort(await commerceContext.AddMessage(error, "NameTooLong", args, defaultMessage).ConfigureAwait(false), context);
                return null;
            }
            var couponId = CommerceEntity.IdPrefix<Coupon>() + arg.Code;
            if (await this._findEntityPipeline.Run(new FindEntityArgument(typeof(Coupon), couponId), context).ConfigureAwait(false) is Coupon)
            {
                executionContext = context;
                var commerceContext = context.CommerceContext;
                var error = context.GetPolicy<KnownResultCodes>().Error;
                var args = new object[]
                {
          arg.Code,
          promotion.Name
                };
                var defaultMessage = "Coupon code " + arg.Code + " is already in use.";
                executionContext.Abort(await commerceContext.AddMessage(error, "CouponCodeAlreadyInUse", args, defaultMessage).ConfigureAwait(false), context);
                return null;
            }
            var coupon1 = new Coupon();
            coupon1.Id = couponId;
            coupon1.Code = arg.Code;
            coupon1.FriendlyId = arg.Code;
            coupon1.Name = arg.Code;
            coupon1.DisplayName = arg.Code;
            coupon1.CouponType = "Public";
            coupon1.DateCreated = DateTimeOffset.UtcNow;
            coupon1.DateUpdated = DateTimeOffset.UtcNow;
            coupon1.Promotion = new EntityReference
            {
                EntityTarget = promotion.Id,
                Name = promotion.Name,
                EntityTargetUniqueId = promotion.UniqueId
            };
            var coupon2 = coupon1;
            coupon2.AddPolicies(new LimitUsagesPolicy
            {
                LimitCount = -999
            });
            coupon2.SetComponent(new ListMembershipsComponent
            {
                Memberships = new List<string>
                {
                    CommerceEntity.ListName<Coupon>(),
                    context.GetPolicy<Policies.KnownCouponsListsPolicy>().PublicCouponsList,
                    string.Format(CultureInfo.InvariantCulture, context.GetPolicy<KnownCouponsListsPolicy>().PromotionCoupons, promotion.FriendlyId),
                    string.Format(CultureInfo.InvariantCulture, context.GetPolicy<KnownCouponsListsPolicy>().PublicCoupons, promotion.FriendlyId)
                }
            });
            var commerceContext1 = context.CommerceContext;
            var publicCouponAdded = new PublicCouponAdded(coupon2.FriendlyId);
            publicCouponAdded.Name = coupon2.Name;
            commerceContext1.AddModel(publicCouponAdded);
            context.CommerceContext.AddUniqueObjectByType(arg);
            return coupon2;
        }
    }
}
