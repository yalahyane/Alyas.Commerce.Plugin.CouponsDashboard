namespace Alyas.Commerce.Plugin.CouponsDashboard.Pipelines
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;

    public interface ICreatePublicAndPrivateListsPipeline : IPipeline<string, string, CommercePipelineExecutionContext>
    {
    }
}
