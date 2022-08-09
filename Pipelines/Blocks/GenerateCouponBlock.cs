namespace Alyas.Commerce.Plugin.CouponsDashboard.Pipelines.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Coupons;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    public class GenerateCouponBlock : PipelineBlock<PrivateCouponGroup, PrivateCouponGroup, CommercePipelineExecutionContext>
    {
        internal const string Alphabet = "AG8FOLE2WVTCPY5ZH3NIUDBXSMQK7946";
        internal const int Bitcount = 30;
        internal const int Bitmask = 32767;
        private readonly IAddEntitiesPipeline _addEntitiesPipeline;

        public GenerateCouponBlock(IAddEntitiesPipeline addEntitiesPipeline)
        {
            _addEntitiesPipeline = addEntitiesPipeline;
        }

        public override async Task<PrivateCouponGroup> Run(
          PrivateCouponGroup arg,
          CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull(Name + ": The block argument cannot be null.");
            Condition.Requires(arg.Promotion).IsNotNull(Name + ": The promotion cannot be null.");
            Condition.Requires(arg.Prefix).IsNotNull(Name + ": The coupon prefix cannot be null.");
            Condition.Requires(arg.Suffix).IsNotNull(Name + ": The coupon suffix cannot be null.");
            Condition.Requires(arg.Total).IsGreaterThan(0, Name + ": The coupon total must be greater than 0.");
            var promotion = context.CommerceContext.GetObject<AddPrivateCouponArgument>()?.Promotion;
            Condition.Requires(promotion).IsNotNull(Name + ": The promotion cannot be null.");
            var persistEntityArguments = new List<PersistEntityArgument>();
            for (var index = 0; index < arg.Total; ++index)
            {
                var str = arg.Prefix + GenerateCouponCode((uint)(Environment.TickCount + index), (index + 1) * arg.Total, context.GetPolicy<GlobalCouponsPolicy>().GeneratedCouponCodeLength) + arg.Suffix;
                var coupon1 = new Coupon();
                coupon1.Id = CommerceEntity.IdPrefix<Coupon>() + str;
                coupon1.Code = str;
                coupon1.Name = str;
                coupon1.FriendlyId = str;
                coupon1.DisplayName = str;
                coupon1.CouponType = "Private";
                coupon1.DateCreated = DateTimeOffset.UtcNow;
                coupon1.DateUpdated = DateTimeOffset.UtcNow;
                coupon1.Promotion = new EntityReference
                {
                    EntityTarget = promotion.Id,
                    Name = promotion.Name,
                    EntityTargetUniqueId = promotion.UniqueId
                };
                coupon1.PrivateCouponGroupReference = new EntityReference
                {
                    EntityTarget = arg.Id,
                    Name = arg.Name
                };
                var coupon2 = coupon1;
                coupon2.AddPolicies(new LimitUsagesPolicy
                {
                    LimitCount = 1
                });
                coupon2.SetComponent(new ListMembershipsComponent
                {
                    Memberships = new List<string>
                    {
                        CommerceEntity.ListName<Coupon>(),
                        context.GetPolicy<Policies.KnownCouponsListsPolicy>().PrivateCouponsList,
                        string.Format(CultureInfo.InvariantCulture, context.GetPolicy<KnownCouponsListsPolicy>().PromotionCoupons, promotion.FriendlyId)
                    }
                });
                var coupon3 = coupon2;
                var membershipsComponent = new TransientListMembershipsComponent();
                membershipsComponent.Memberships = new List<string>
                {
                    string.Format(CultureInfo.InvariantCulture, context.GetPolicy<KnownCouponsListsPolicy>().UnallocatedCoupons, arg.FriendlyId)
                };
                coupon3.SetComponent(membershipsComponent);
                persistEntityArguments.Add(new PersistEntityArgument(coupon2));
            }
            await _addEntitiesPipeline.Run(new PersistEntitiesArgument(persistEntityArguments), context).ConfigureAwait(false);
            await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().Information, null, null,
                $"Generated {arg.Total} unallocated private coupons").ConfigureAwait(false);
            return arg;
        }

        internal static string GenerateCouponCode(uint n, int cycle, int length)
        {
            var stringBuilder = new StringBuilder();
            var num1 = n;
            var num2 = 0;
            do
            {
                n = Mask(n);
                for (var index = 0; index < 6; ++index)
                {
                    stringBuilder.Append("AG8FOLE2WVTCPY5ZH3NIUDBXSMQK7946"[(int)n & 31]);
                    n >>= 5;
                }
                ++num2;
                n = (uint)(num1 + (ulong)cycle + (ulong)num2);
            }
            while (stringBuilder.Length < length);
            return stringBuilder.Remove(length, stringBuilder.Length - length).ToString();
        }

        internal static uint RoundFunction(uint n) => (uint)(((int)n ^ 47894) + 25 << 1 & short.MaxValue);

        internal static uint Mask(uint n)
        {
            var num1 = n >> 15;
            var n1 = n & (uint)short.MaxValue;
            for (var index = 0; index < 10; ++index)
            {
                var num2 = (int)(num1 ^ RoundFunction(n1));
                num1 = n1;
                n1 = (uint)num2;
            }
            return num1 | n1 << 15;
        }
    }
}
