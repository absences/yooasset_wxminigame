#if UNITY_IOS
using System;
using UnityEngine;
using UnityEngine.iOS;
using CalendarUnit = UnityEngine.iOS.CalendarUnit;
using LocalNotification = UnityEngine.iOS.LocalNotification;

public class NotificationService
{
    public static bool Initialized { get; private set; }


    public static void Init()
    {
        if (Initialized)
        {
            return;
        }

        UnityEngine.iOS.NotificationServices.RegisterForNotifications(NotificationType.Alert |
                                                                      NotificationType.Badge |
                                                                      NotificationType.Sound, false);
        Debug.Log(UnityEngine.iOS.NotificationServices.enabledNotificationTypes);
        Initialized = true;
    }


    public static LocalNotification SendNotification(TimeSpan delay, string title, string message)
    {
        if (!Initialized)
            Init();

#if UNITY_ANDROID
        //        Assets.SimpleAndroidNotifications.NotificationManager.SendWithAppIcon(delay, title, message);
#elif UNITY_IOS
        var localNotification = new UnityEngine.iOS.LocalNotification
        {
            alertBody = message,
            fireDate = DateTime.Now.Add(delay),
            soundName = LocalNotification.defaultSoundName,
            hasAction = false
        };
        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(localNotification);


        Debug.Log($"Schedule Notification  count:{UnityEngine.iOS.NotificationServices.scheduledLocalNotifications.Length} Registation error:{UnityEngine.iOS.NotificationServices.registrationError}");

        return localNotification;
#endif

        return null;
    }

    public static void CanCelLocalNotification(LocalNotification notification)
    {
        if (!Initialized)
            Init();

        UnityEngine.iOS.NotificationServices.CancelLocalNotification(notification);

    }

    public static void CancelAll()
    {
        if (!Initialized)
            Init();

#if UNITY_ANDROID
        //        Assets.SimpleAndroidNotifications.NotificationManager.CancelAll();

#elif UNITY_IOS
        UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
#endif

    }
}
#endif