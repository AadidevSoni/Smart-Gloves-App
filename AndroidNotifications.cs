using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using UnityEngine.Android; 

public class AndroidNotifications : MonoBehaviour
{
    // Request notification permission for Android 13+
    public void RequestAuthorization()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
    }

    // Register a notification channel
    public void RegisterNotificationChannel()
    {
        var channel = new AndroidNotificationChannel
        {
            Id = "default_channel",
            Name = "Default Channel",
            Importance = Importance.High, // Set to High so it pops up on screen
            Description = "Notifications for Smart Gloves App"
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    // Send a notification
    public void SendNotification(string title, string text, int fireTimeInSeconds)
    {
        var notification = new AndroidNotification
        {
            Title = title,
            Text = text,
            FireTime = System.DateTime.Now.AddSeconds(fireTimeInSeconds),
            SmallIcon = "icon_0", // Ensure icons are correctly added in Plugins/Android/res/drawable/
            LargeIcon = "icon_1"
        };

        AndroidNotificationCenter.SendNotification(notification, "default_channel");
    }
}
