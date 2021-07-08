namespace Plugin.FirebasePushNotification
{
    public enum NotificationActionType
    {
        Default,
        AuthenticationRequired, //Only applies for iOS
        Foreground,
        Reply,
        Destructive  //Only applies for iOS
    }
}
