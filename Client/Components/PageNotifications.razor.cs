namespace BlazorRepl.Client.Components
{
    using System.Collections.Generic;
    using BlazorRepl.Client.Components.Models;
    using Microsoft.AspNetCore.Components;

    public partial class PageNotifications
    {
        [Parameter]
        public ICollection<PageNotification> Notifications { get; set; }

        [Parameter]
        public EventCallback<ICollection<PageNotification>> NotificationsChanged { get; set; }

        private string GetAlertClass(NotificationType type) =>
            type switch
            {
                NotificationType.Info => "alert-info",
                NotificationType.Warning => "alert-warning",
                NotificationType.Error => "alert-danger",
            };

        private void RemoveNotification(PageNotification notification)
        {
            this.Notifications.Remove(notification);
            this.NotificationsChanged?.InvokeAsync(this.Notifications);
        }
    }
}
