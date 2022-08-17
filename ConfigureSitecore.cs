namespace Alyas.Commerce.Plugin.CouponsDashboard
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Pipelines;
    using Pipelines.Blocks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.BusinessUsers;
    using Sitecore.Commerce.Plugin.Coupons;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<IOrderPlacedPipeline>(configure =>
                {
                    configure.Add<AddOrderCouponMembershipBlock>().After<OrderPlacedAssignConfirmationIdBlock>();
                })
                .ConfigurePipeline<IBizFxNavigationPipeline>(c => c.Add<GetCouponsNavigationViewBlock>().After<GetNavigationViewBlock>())
                .ConfigurePipeline<IGetEntityViewPipeline>(c =>
                {
                    c.Add<GetPublicCouponsUsageViewBlock>().After<PopulateEntityVersionBlock>();
                    c.Add<GetPrivateCouponsUsageViewBlock>().After<GetPublicCouponsUsageViewBlock>();
                    c.Add<GetCouponDetailsViewBlock>().After<PopulateEntityVersionBlock>();
                    c.Add<GetCouponOrdersViewBlock>().After<GetCouponDetailsViewBlock>();
                })
                .ConfigurePipeline<IDoActionPipeline>(c =>
                {
                    c.Add<DoActionPaginatePublicCouponsBlock>().After<ValidateEntityVersionBlock>();
                    c.Add<DoActionPaginatePrivateCouponsBlock>().After<ValidateEntityVersionBlock>();
                    c.Add<DoActionPaginateCouponOrdersBlock>().After<ValidateEntityVersionBlock>();
                })
                .AddPipeline<ICreatePublicAndPrivateListsPipeline, CreatePublicAndPrivateListsPipeline>(c => c.Add<CreatePublicAndPrivateListsBlock>())
                .ConfigurePipeline<IAddPublicCouponPipeline>(c => c.Replace<Sitecore.Commerce.Plugin.Coupons.AddPublicCouponBlock, Pipelines.Blocks.AddPublicCouponBlock>())
                .ConfigurePipeline<IAddPrivateCouponPipeline>(c => c.Replace<Sitecore.Commerce.Plugin.Coupons.GenerateCouponBlock, Pipelines.Blocks.GenerateCouponBlock>())
                .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure.Add<ConfigureServiceApiBlock>())
            );
            services.RegisterAllCommands(assembly);
        }
    }
}
