using GoogleMapsUnofficial.Interfaces;

public class InitializeSwitch
{
    public InitializeSwitch(INotificationManager notificationManager, IDispatcher dispatcher)
    {
        NotificationManager = notificationManager;
        Dispatcher = dispatcher;
    }
    public static INotificationManager NotificationManager { get; private set; }
    public static IDispatcher Dispatcher { get; private set; }
}
