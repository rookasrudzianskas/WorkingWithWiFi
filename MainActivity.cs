using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Net;
using Android.Net.Wifi;
using Android.Content;
using System.Collections.Generic;
using Android;
using Android.Support.V4.App;
using Android.Content.PM;
using System;

namespace WorkingWithWifi
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : ListActivity
    {
        WifiManager wiMan;
        WiFiReceiver wiReceiver;
        bool allGranted = false;
        string[] permissions =
            {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation,
            Manifest.Permission.AccessWifiState,
            Manifest.Permission.ChangeWifiState
        };
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            //SetContentView(Resource.Layout.activity_main);

            wiMan = GetSystemService(Context.WifiService) as WifiManager;

            //register new broadcast receiver(listner)
            wiReceiver = new WiFiReceiver();
            wiReceiver.ResultsReceived += WiReceiver_ResultsReceived;

            RegisterReceiver(wiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
            if (CheckAllPermissions())
            {
                PerformScan();
            }
        }

        bool CheckAllPermissions()
        {
            List<string> required = new List<string>();
            foreach (var item in permissions)
            {
                if (ActivityCompat.CheckSelfPermission(this, item) == Permission.Denied)
                    required.Add(item);
            }

            if(required.Count!=0)
            {
                ActivityCompat.RequestPermissions(this, required.ToArray(), 0);
                return false;
            }
            return true;
        }

        private void WiReceiver_ResultsReceived(Context arg1, Intent arg2)
        {
            var allNetworks = wiMan.ScanResults;
            List<string> APinfo = new List<string>();
            foreach (var item in allNetworks)
            {
                APinfo.Add(item.Ssid + " " + item.ChannelWidth + " " + item.Capabilities);
                Toast.MakeText(this, "Test", ToastLength.Short).Show();
            }
            //place info on screen
            this.ListAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, APinfo);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            int denied = 0;
            List<string> deniedPermissions = new List<string>();

            for(int i=0;i<grantResults.Length;i++)
            {
                if(grantResults[i]== Permission.Denied)
                {
                    deniedPermissions.Add(permissions[i]);
                    denied++;
                }
            }
            if (denied == 0) PerformScan();
            else
            {
                foreach (var item in deniedPermissions)
                {
                    if(ActivityCompat.ShouldShowRequestPermissionRationale(this,item))
                    {
                        var builder = new Android.App.AlertDialog.Builder(this);
                        builder.SetTitle("Required permission");
                        builder.SetMessage("Permissions needed. doy yo want to grant permissions?");
                        builder.SetPositiveButton("Yes", delegate { CheckAllPermissions(); });
                        builder.SetNegativeButton("No", delegate { this.Finish(); });
                        builder.SetCancelable(false);
                        builder.Show();
                    }
                    else
                    {
                        var builder = new Android.App.AlertDialog.Builder(this);
                        builder.SetTitle("Required permission");
                        builder.SetMessage("Permissions needed. Go to Settings->Apps and manually enable app permission(s). Go there now?");
                        builder.SetPositiveButton("Yes", delegate {
                            //open app setting view
                            Intent intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings, Android.Net.Uri.FromParts("package",PackageName,null));
                            intent.AddFlags(ActivityFlags.NewTask);
                            StartActivity(intent);
                            this.Finish();

                        });
                        builder.SetNegativeButton("No", delegate { this.Finish(); });
                        builder.SetCancelable(false);
                        builder.Show();
                    }
                }
            }
        }

        private void PerformScan()
        {
            //check if wifi is enabled
            if (!wiMan.IsWifiEnabled)
            {
                var builder = new Android.App.AlertDialog.Builder(this);
                builder.SetTitle("Change Wifi Adapter State");
                builder.SetMessage("Do You want to turn on WIFI adapter?");
                builder.SetPositiveButton("Yes", delegate
                {
                    wiMan.SetWifiEnabled(true);
                    wiMan.StartScan();
                });
                builder.SetNegativeButton("No", delegate { });
                builder.Show();
            }
            else
            {
                wiMan.StartScan();
            }
        }
    }
}