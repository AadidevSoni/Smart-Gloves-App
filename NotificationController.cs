using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;
#endif

public class NotificationController : MonoBehaviour
{
    [SerializeField]
    AndroidNotifications androidNotifications;


    // Start is called before the first frame update
    private void Start()
    {
        #if UNITY_ANDROID
        androidNotifications.RequestAuthorization();
        androidNotifications.RegisterNotificationChannel();
        androidNotifications.SendNotification("NEED WATER", "Wearer needs water!", 10);
        #endif
    }

    //to know if the app is actively being used
    private void OnApplicationPause(bool pause)
{
    if (pause)
    {
        #if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllNotifications();
        #endif
    }
}
}