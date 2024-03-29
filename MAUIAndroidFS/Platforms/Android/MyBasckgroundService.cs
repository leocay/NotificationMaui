using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media.Session;
using Android.OS;
using Android.Support.V4.Media.Session;
using AndroidX.Core.App;
using Microsoft.AspNetCore.SignalR.Client;
using xx = AndroidX.Media;

namespace MAUIAndroidFS.Platforms.Android;

[Service(ForegroundServiceType = ForegroundService.TypeNone, Enabled = true,
    DirectBootAware = true)]
internal class MyBackgroundService : Service
{
    Timer timer = null;
    int myId = (new object()).GetHashCode();
    int BadgeNumber = 0;
    private readonly IBinder binder = new LocalBinder();
    NotificationCompat.Builder notification;
    //HubConnection hubConnection;

    public class LocalBinder : Binder
    {
        public MyBackgroundService GetService()
        {
            return this.GetService();
        }
    }

    public override IBinder OnBind(Intent intent)
    {
        return binder;
    }

    public override StartCommandResult OnStartCommand(Intent intent,
        StartCommandFlags flags, int startId)
    {
        var mediaSession = new MediaSessionCompat(this, "MediaSession");
        mediaSession.SetFlags(MediaSessionCompat.FlagHandlesMediaButtons | MediaSessionCompat.FlagHandlesTransportControls);

        var input = intent.GetStringExtra("inputExtra");

        var notificationIntent = new Intent(this, typeof(BootReceiver));
        notificationIntent.SetAction("ACTION_PLAY");

        var pendingIntent = PendingIntent.GetBroadcast(this, 0, notificationIntent, PendingIntentFlags.Immutable);

        //var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent,
        //    PendingIntentFlags.Immutable);

        var notificationIntent1 = new Intent(this, typeof(BootReceiver));
        notificationIntent1.SetAction("ACTION_PAUSE");

        var pendingIntent1 = PendingIntent.GetBroadcast(this, 0, notificationIntent1, PendingIntentFlags.Immutable);

        var notificationIntent2 = new Intent(this, typeof(BootReceiver));
        notificationIntent2.SetAction("ACTION_NEXT");

        var pendingIntent2 = PendingIntent.GetBroadcast(this, 0, notificationIntent2, PendingIntentFlags.Immutable);

        // Increment the BadgeNumber
        BadgeNumber++;

        notification = new NotificationCompat.Builder(this,
                MainApplication.ChannelId)
            .SetContentText(input)
            .SetSmallIcon(Resource.Drawable.AppIcon)
            .SetAutoCancel(false)
            .SetContentTitle("Service Running")
            .SetPriority(NotificationCompat.PriorityDefault)
            .SetContentIntent(pendingIntent)
            .SetVisibility(NotificationCompat.VisibilityPublic)
            .SetStyle(new AndroidX.Media.App.NotificationCompat.MediaStyle()
            //.SetShowActionsInCompactView(0, 1, 2) // Chỉ định vị trí của các nút trong view thu gọn
            .SetMediaSession(mediaSession.SessionToken)) // Liên kết với MediaSession
            .AddAction(Resource.Drawable.material_ic_keyboard_arrow_next_black_24dp, "Previous", pendingIntent)
            .AddAction(Resource.Drawable.material_ic_keyboard_arrow_previous_black_24dp, "Play",pendingIntent1) // Sử dụng PendingIntent đã tạo
            .AddAction(Resource.Drawable.material_ic_keyboard_arrow_previous_black_24dp, "Next",pendingIntent2);

        var notificationManager = NotificationManagerCompat.From(this);
        notificationManager.Notify(myId, notification.Build());

        // build and notify
        StartForeground(myId, notification.Build(), ForegroundService.TypeNone);

        // timer to ensure hub connection
        //timer = new Timer(Timer_Elapsed, notification, 0, 10000);

        // You can stop the service from inside the service by calling StopSelf();

        return StartCommandResult.Sticky;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="state"></param>
    async void Timer_Elapsed(object state)
    {
        //AndroidServiceManager.IsRunning = true;

        //await EnsureHubConnection();
        AndroidServiceManager.IsRunning = true;
        BadgeNumber++;
        string timeString = $"Time : {DateTime.Now.ToLongTimeString()}";
        var notification = (NotificationCompat.Builder)state;
        notification.SetNumber(BadgeNumber);
        notification.SetContentTitle(timeString);
        StartForeground(myId, notification.Build(), ForegroundService.TypeNone);
    }
}