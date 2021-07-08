namespace Plugin.FirebasePushNotification
{
    public class NotificationUserReplyAction : NotificationUserAction
    {
        public string Placeholder { get; }
        public NotificationUserReplyAction(string id, string title, NotificationActionType type = NotificationActionType.Default, string icon = "", string placeholder = "") : base(id, title, type, icon)
        {
            Placeholder = placeholder;
        }
    }
}
