namespace Alyas.Commerce.Plugin.CouponsDashboard.Commands
{
    using System.Threading.Tasks;
    using Pipelines;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;

    public class CreatePublicAndPrivateListsCommand : CommerceCommand
    {
        private readonly ICreatePublicAndPrivateListsPipeline _pipeline;

        public CreatePublicAndPrivateListsCommand(ICreatePublicAndPrivateListsPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        public virtual async Task<CommerceCommand> Process(CommerceContext commerceContext)
        {
            await this._pipeline.Run(string.Empty, commerceContext.PipelineContext);
            return this;
        }
    }
}
