namespace BlazorRepl.Client.Components
{
    using System.Collections.Generic;
    using BlazorRepl.Client.Components.Models;
    using Microsoft.AspNetCore.Components;

    public partial class PageNotifications
    {
        [Parameter]
        public IEnumerable<PageNotification> Notifications { get; set; }

        public string GetAlertClass(NotificationType type) =>
            type switch
            {
                NotificationType.Info => "alert-info",
                NotificationType.Warning => "alert-warning",
                NotificationType.Error => "alert-danger",
            };
    }
}
