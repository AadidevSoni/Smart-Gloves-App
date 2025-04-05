using Unity.Notifications.Android;
using UnityEngine;

public class AndroidNotifications : MonoBehaviour
{
    public string channelId = "wearer_alerts";

    public void RequestAuthorization()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Notification))
        {
            Permission.RequestUserPermission(Permission.Notification);
        }
    }

    public void RegisterNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = channelId,
            Name = "Wearer Alert Notifications",
            Importance = Importance.High,  //for pop up
            Description = "Alerts for needs like water, food, help, etc.",
            EnableVibration = true,
            EnableLights = true,
            CanBypassDnd = true,
            LockScreenVisibility = LockScreenVisibility.Public
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    public void SendNotification(string title, string message, int delaySeconds)
    {
        var notification = new AndroidNotification()
        {
            Title = title,
            Text = message,
            FireTime = System.DateTime.Now.AddSeconds(delaySeconds),
            ShouldAutoCancel = true,
            SmallIcon = "icon_0", // Optional: Add custom icon in Plugins/Android/res
            LargeIcon = "icon_0"
        };

        AndroidNotificationCenter.SendNotification(notification, channelId);
    }
}