namespace Alyas.Commerce.Plugin.CouponsDashboard.Pipelines
{
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;

    public class CreatePublicAndPrivateListsPipeline : CommercePipeline<string, string>, ICreatePublicAndPrivateListsPipeline
    {
        public CreatePublicAndPrivateListsPipeline(IPipelineConfiguration<ICreatePublicAndPrivateListsPipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}
