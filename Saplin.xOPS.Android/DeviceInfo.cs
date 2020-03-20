using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Java.IO;
using Saplin.xOPS.UI.Misc;
using Xamarin.Forms;
using static Android.App.ActivityManager;

[assembly: Dependency(typeof(Saplin.xOPS.Droid.DeviceInfo))]
namespace Saplin.xOPS.Droid
{
    public class DeviceInfo : IDeviceInfo
    {
        private string cpu = null;
        private string model = null;
        private float? ram = null;
        private bool? isChromeOs = null;

        public bool IsChromeOs
        {
            get
            {
                if (isChromeOs == null)
                    try { isChromeOs = Android.App.Application.Context.PackageManager.HasSystemFeature("org.chromium.arc.device_management"); } catch { isChromeOs = false; }

                return isChromeOs.Value;
            }
        }

        public string GetCPU()
        {
            if (cpu == null)
            {
                cpu = "";
                try
                {
                    var br = new BufferedReader(new FileReader("/proc/cpuinfo"));

                    string str;

                    var output = new Dictionary<string, string>();

                    try
                    {
                        while ((str = br.ReadLine()) != null)
                        {

                            var data = str.Split(":");

                            if (data.Length > 1)
                            {

                                var key = data[0].Trim().Replace(" ", "_");
                                if (key.Equals("model_name") || key.Equals("Hardware")) key = "cpu_model";

                                var value = data[1].Trim();

                                if (key.Equals("cpu_model"))
                                {
                                    cpu = value;
                                    break;
                                }
                            }

                        }
                    }
                    finally
                    {
                        br.Close();
                    }

                }
                catch { };
            }

            return cpu;
        }

        public string GetModelName()
        {
            if (model == null)
            {
                model = "";
                try
                {
                    model = Build.Manufacturer + " " + Build.Model;
                }
                catch { };
            }

            return model;
        }

        public float GetRamSizeGb()
        {
            if (ram == null)
            {
                ram = -1;
                try
                {
                    var activityManager = Android.App.Application.Context.GetSystemService(Context.ActivityService) as ActivityManager;

                    MemoryInfo memInfo = new MemoryInfo();
                    activityManager.GetMemoryInfo(memInfo);

                    ram = (float)memInfo.TotalMem / 1024 / 1024 / 1024;
                }
                catch { };
            }

            return ram.Value;
        }
    }
}
