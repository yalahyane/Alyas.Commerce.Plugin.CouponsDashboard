namespace Alyas.Commerce.Plugin.CouponsDashboard.Extensions
{
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Framework.Conditions;

    internal static class EntityViewExtensions
    {
        internal static async Task AddOrderChildViewAsync(
          this EntityView entityView,
          GetOnHoldOrderCartCommand getOnHoldOrderCartCommand,
          CommerceContext context,
          string childViewName,
          string childViewEntityId,
          Order order)
        {
            Condition.Requires(order, nameof(order)).IsNotNull();
            Condition.Requires(entityView, nameof(entityView)).IsNotNull();
            var orderView = new EntityView
            {
                EntityId = childViewEntityId,
                ItemId = order.Id,
                Name = childViewName
            };
            var itemIdProperty = new ViewProperty
            {
                Name = "ItemId",
                IsReadOnly = true,
                RawValue = order.Id,
                IsHidden = true
            };
            orderView.Properties.Add(itemIdProperty);

            var confirmationIdProperty = new ViewProperty
            {
                Name = "OrderConfirmationId",
                IsReadOnly = true,
                UiType = "EntityLink",
                RawValue = order.OrderConfirmationId
            };
            orderView.Properties.Add(confirmationIdProperty);

            var orderPlacedDateProperty = new ViewProperty
            {
                Name = "OrderPlacedDate",
                IsReadOnly = true,
                RawValue = order.OrderPlacedDate
            };
            orderView.Properties.Add(orderPlacedDateProperty);
            var dateUpdatedProperty = new ViewProperty
            {
                Name = "DateUpdated",
                IsReadOnly = true,
                RawValue = order.DateUpdated
            };
            orderView.Properties.Add(dateUpdatedProperty);
            var statusProperty = new ViewProperty
            {
                Name = "Status",
                IsReadOnly = true,
                RawValue = order.Status
            };
            orderView.Properties.Add(statusProperty);
            var shopNameProperty = new ViewProperty
            {
                Name = "ShopName",
                IsReadOnly = true,
                RawValue = order.ShopName
            };
            orderView.Properties.Add(shopNameProperty);
            var customerEmailProperty = new ViewProperty
            {
                Name = "CustomerEmail",
                IsReadOnly = true,
                RawValue = order.GetComponent<ContactComponent>().Email
            };
            orderView.Properties.Add(customerEmailProperty);
            if (order.HasComponent<OnHoldOrderComponent>())
            {
                Condition.Requires(getOnHoldOrderCartCommand, nameof(getOnHoldOrderCartCommand)).IsNotNull();
                var cart = await getOnHoldOrderCartCommand.Process(context, order).ConfigureAwait(false);
                var orderSubTotalProperty = new ViewProperty
                {
                    Name = "OrderSubTotal",
                    IsReadOnly = true,
                    RawValue = cart?.Totals.SubTotal
                };
                orderView.Properties.Add(orderSubTotalProperty);
                var orderAdjustmentsTotalProperty = new ViewProperty
                {
                    Name = "OrderAdjustmentsTotal",
                    IsReadOnly = true,
                    RawValue = cart?.Totals.AdjustmentsTotal
                };
                orderView.Properties.Add(orderAdjustmentsTotalProperty);
                var orderGrandTotalProperty = new ViewProperty
                {
                    Name = "OrderGrandTotal",
                    IsReadOnly = true,
                    RawValue = cart?.Totals.GrandTotal
                };
                orderView.Properties.Add(orderGrandTotalProperty);
                var orderPaymentsTotalProperty = new ViewProperty
                {
                    Name = "OrderPaymentsTotal",
                    IsReadOnly = true,
                    RawValue = cart?.Totals.PaymentsTotal
                };
                orderView.Properties.Add(orderPaymentsTotalProperty);
            }
            else
            {
                var orderSubTotalProperty = new ViewProperty
                {
                    Name = "OrderSubTotal",
                    IsReadOnly = true,
                    RawValue = order.Totals.SubTotal
                };
                orderView.Properties.Add(orderSubTotalProperty);
                var orderAdjustmentsTotalProperty = new ViewProperty
                {
                    Name = "OrderAdjustmentsTotal",
                    IsReadOnly = true,
                    RawValue = order.Totals.AdjustmentsTotal
                };
                orderView.Properties.Add(orderAdjustmentsTotalProperty);
                var orderGrandTotalProperty = new ViewProperty
                {
                    Name = "OrderGrandTotal",
                    IsReadOnly = true,
                    RawValue = order.Totals.GrandTotal
                };
                orderView.Properties.Add(orderGrandTotalProperty);
                var orderPaymentsTotalProperty = new ViewProperty
                {
                    Name = "OrderPaymentsTotal",
                    IsReadOnly = true,
                    RawValue = order.Totals.PaymentsTotal
                };
                orderView.Properties.Add(orderPaymentsTotalProperty);
            }
            entityView.ChildViews.Add(orderView);
        }
    }
}
