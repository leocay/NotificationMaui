using Android.App;
using Android.Content;
using Android.Widget;
using AndroidX.Core.Content;

namespace MAUIAndroidFS.Platforms.Android;

[BroadcastReceiver(Enabled = true, Exported = true, DirectBootAware = true)]
[IntentFilter(new[] { "ACTION_PLAY", "ACTION_PAUSE", "ACTION_NEXT"})]
public class BootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        if (intent.Action == Intent.ActionBootCompleted)
        {
            Toast.MakeText(context, "Boot completed event received",
                ToastLength.Short).Show();

            var serviceIntent = new Intent(context,
                typeof(MyBackgroundService));

            ContextCompat.StartForegroundService(context,
                serviceIntent);
        }
        // Kiểm tra hành động và thực hiện công việc tương ứng
        var action = intent.Action;
        if (action == "ACTION_PLAY")
        {
            // Handle the specific action
            Console.WriteLine("paly");
        }
        if (action == "ACTION_PAUSE")
        {
            // Handle the specific action
            Console.WriteLine("stop");
        }
        if (action == "ACTION_NEXT")
        {
            // Handle the specific action
            Console.WriteLine("back");
        }

    }
}
