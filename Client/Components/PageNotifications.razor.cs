namespace BlazorRepl.Client.Components
{
    using System.Collections.Generic;
    using BlazorRepl.Client.Components.Models;

    public partial class PageNotifications
    {
        private readonly IList<PageNotification> notifications = new List<PageNotification>();

        public void AddNotification(NotificationType type, string content, string title = null)
        {
            if (!string.IsNullOrWhiteSpace(content))
            {
                this.notifications.Add(new PageNotification { Type = type, Title = title, Content = content });

                this.StateHasChanged();
            }
        }

        public void Clear()
        {
            this.notifications.Clear();

            this.StateHasChanged();
        }

        private static string GetAlertClass(NotificationType type) =>
            type switch
            {
                NotificationType.Info => "alert-info",
                NotificationType.Warning => "alert-warning",
                NotificationType.Error => "alert-danger",
                _ => "alert-info",
            };

        private void RemoveNotification(PageNotification notification) => this.notifications.Remove(notification);
    }
}
