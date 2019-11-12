using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Net.Wifi;

namespace WorkingWithWifi
{
    [BroadcastReceiver]
    public class WiFiReceiver : BroadcastReceiver
    {
        public event Action<Context, Intent> ResultsReceived;
        public override void OnReceive(Context context, Intent intent)
        {
            if (this.ResultsReceived!=null && intent!=null && intent.Action == WifiManager.ScanResultsAvailableAction)
            {
                this.ResultsReceived(context, intent);
            }
        }
    }
}