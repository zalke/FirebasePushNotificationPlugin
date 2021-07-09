using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Firebase.Messaging;

namespace Plugin.FirebasePushNotification
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class PNFirebaseMessagingService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            var parameters = new Dictionary<string, object>();
            var notification = message.GetNotification();
            if (notification != null)
            {
                if (!string.IsNullOrEmpty(notification.Body))
                {
                    parameters.Add("body", notification.Body);
                }

                if (!string.IsNullOrEmpty(notification.BodyLocalizationKey))
                {
                    parameters.Add("body_loc_key", notification.BodyLocalizationKey);
                }

                var bodyLocArgs = notification.GetBodyLocalizationArgs();
                if (bodyLocArgs?.Any() == true)
                {
                    parameters.Add("body_loc_args", bodyLocArgs);
                }

                if (!string.IsNullOrEmpty(notification.Title))
                {
                    parameters.Add("title", notification.Title);
                }

                if (!string.IsNullOrEmpty(notification.TitleLocalizationKey))
                {
                    parameters.Add("title_loc_key", notification.TitleLocalizationKey);
                }

                var titleLocArgs = notification.GetTitleLocalizationArgs();
                if (titleLocArgs?.Any() == true)
                {
                    parameters.Add("title_loc_args", titleLocArgs);
                }

                if (!string.IsNullOrEmpty(notification.Tag))
                {
                    parameters.Add("tag", notification.Tag);
                }

                if (!string.IsNullOrEmpty(notification.Sound))
                {
                    parameters.Add("sound", notification.Sound);
                }

                if (!string.IsNullOrEmpty(notification.Icon))
                {
                    parameters.Add("icon", notification.Icon);
                }

                if (notification.Link != null)
                {
                    parameters.Add("link_path", notification.Link.Path);
                }

                if (!string.IsNullOrEmpty(notification.ClickAction))
                {
                    parameters.Add("click_action", notification.ClickAction);
                }

                if (!string.IsNullOrEmpty(notification.Color))
                {
                    parameters.Add("color", notification.Color);
                }
            }
            foreach (var d in message.Data)
            {
                if (!parameters.ContainsKey(d.Key))
                {
                    parameters.Add(d.Key, d.Value);
                }
            }

            //Fix localization arguments parsing
            var localizationKeys = new string[] { "title_loc_args", "body_loc_args" };
            foreach (var locKey in localizationKeys)
            {
                if (parameters.ContainsKey(locKey) && parameters[locKey] is string parameterValue)
                {
                    if (parameterValue.StartsWith("[") && parameterValue.EndsWith("]") && parameterValue.Length > 2)
                    {
                        var arrayValues = parameterValue[1..^1];
                        parameters[locKey] = arrayValues.Split(',').Select(t => t.Trim()).ToArray();
                    }
                    else
                    {
                        parameters[locKey] = Array.Empty<string>();
                    }
                }
            }

            FirebasePushNotificationManager.RegisterData(parameters);
            CrossFirebasePushNotification.Current.NotificationHandler?.OnReceived(parameters);
        }

        public override async void OnNewToken(string refreshedToken)
        {
            if (string.IsNullOrWhiteSpace(refreshedToken))
            {
                System.Diagnostics.Debug.WriteLine("Received empty Token...");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Received new token: {refreshedToken}, Starting Re registering existing topics");

            //Resubscribe to topics since the old instance id isn't valid anymore
            foreach (var t in CrossFirebasePushNotification.Current.SubscribedTopics)
            {
                try
                {
                    await FirebaseMessaging.Instance.SubscribeToTopic(t).ToAwaitableTaskVoid().ConfigureAwait(false);
                    System.Diagnostics.Debug.WriteLine($"ReRegistered Topic: {t}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error ReRegistering Topic: {t}, {ex.Message}");
                }
            }

            var editor = Application.Context.GetSharedPreferences(FirebasePushNotificationManager.KeyGroupName, FileCreationMode.Private).Edit();
            editor.PutString(FirebasePushNotificationManager.FirebaseTokenKey, refreshedToken);
            editor.Commit();

            // CrossFirebasePushNotification.Current.OnTokenRefresh?.Invoke(this,refreshedToken);
            FirebasePushNotificationManager.RegisterToken(refreshedToken);
            System.Diagnostics.Debug.WriteLine($"REFRESHED TOKEN: {refreshedToken}");
        }

        private void ScheduleJob()
        {
            // [START dispatch_job]
            /*FirebaseJobDispatcher dispatcher = new FirebaseJobDispatcher(new GooglePlayDriver(this));
            Job myJob = dispatcher.newJobBuilder()
                    .setService(MyJobService.class)
                .setTag("my-job-tag")
                .build();
        dispatcher.schedule(myJob);*/
            // [END dispatch_job]
        }
    }
}
