namespace Alyas.Commerce.Plugin.CouponsDashboard.Controllers
{
    using System;
    using System.Web.Http.OData;
    using Commands;
    using Microsoft.AspNetCore.Mvc;
    using Sitecore.Commerce.Core;

    [Microsoft.AspNetCore.OData.EnableQuery]
    public class CommandsController : CommerceController
    {
        public CommandsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment) : base(serviceProvider, globalEnvironment)
        {
        }

        [HttpPut]
        [Route("CreatePublicAndPrivateLists()")]
        public IActionResult CreatePublicAndPrivateLists([FromBody] ODataActionParameters value)
        {
            if (!this.ModelState.IsValid)
                return new BadRequestObjectResult(this.ModelState);
            var command = this.Command<CreatePublicAndPrivateListsCommand>();
            return new ObjectResult(ExecuteLongRunningCommand(() => command.Process(this.CurrentContext)));
        }
    }
}
