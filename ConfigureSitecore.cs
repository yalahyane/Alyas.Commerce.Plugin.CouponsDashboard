namespace Alyas.Commerce.Plugin.CouponsDashboard
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Pipelines.Blocks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.BusinessUsers;
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
                .ConfigurePipeline<IBizFxNavigationPipeline>(c=>c.Add<GetCouponsNavigationViewBlock>().After<GetNavigationViewBlock>())
                .ConfigurePipeline<IGetEntityViewPipeline>(c=>
                {
                    c.Add<GetCouponsUsageViewBlock>().After<PopulateEntityVersionBlock>();
                    c.Add<GetCouponDetailsViewBlock>().After<PopulateEntityVersionBlock>();
                    c.Add<GetCouponOrdersViewBlock>().After<GetCouponDetailsViewBlock>();
                })
                .ConfigurePipeline<IDoActionPipeline>(c=>
                {
                    c.Add<DoActionPaginateCouponsListBlock>().After<ValidateEntityVersionBlock>();
                    c.Add<DoActionPaginateCouponOrdersBlock>().After<ValidateEntityVersionBlock>();
                })
                .ConfigurePipeline<IOrderPlacedPipeline>(configure =>
                {
                    configure.Add<AddOrderCouponMembershipBlock>().After<OrderPlacedAssignConfirmationIdBlock>();
                })
            );
        }
    }
}
