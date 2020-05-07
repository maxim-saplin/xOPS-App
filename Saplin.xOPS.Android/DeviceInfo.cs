using System;
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

        RandomAccessFile cpuTempFile = null;

        public DeviceInfo()
        {
            var paths = new string[] {
                "/sys/devices/system/cpu/cpu0/cpufreq/cpu_temp",
                "/sys/devices/system/cpu/cpu0/cpufreq/FakeShmoo_cpu_temp",

                "/sys/devices/virtual/thermal/thermal_zone1/temp",
                "/sys/devices/virtual/thermal/thermal_zone0/temp",
                "/sys/devices/virtual/hwmon/hwmon1/temp1_input", //Nokia N1, sensor name in 'sensor'

                "/sys/devices/platform/omap/omap_temp_sensor.0/temperature",

                "/sys/devices/platform/tegra-i2c.3/i2c-4/4-004c/temperature", 
                "/sys/devices/platform/tegra_tmon/temp1_input",
                "/sys/devices/platform/tegra-i2c.3/i2c-4/4-004c/ext_temperature",
                "/sys/devices/platform/tegra-tsensor/tsensor_temperature",

                "/sys/devices/platform/s5p-tmu/temperature",
                "/sys/devices/platform/s5p-tmu/curr_temp",
                "/sys/devices/platform/s5p-tmu/temperature",

                // /sys/devices/virtual/thermal/thermal_zone1/temp  - miui.eu custom rom

                "/sys/class/thermal/thermal_zone0/temp",
                "/sys/class/thermal/thermal_zone1/temp",
                "/sys/class/thermal/thermal_zone3/temp",
                "/sys/class/thermal/thermal_zone4/temp",

                "/sys/class/i2c-adapter/i2c-4/4-004c/temperature",

                "/sys/class/hwmon/hwmon0/temp1_input",
                "/sys/class/hwmon/hwmonX/temp1_input", 
                "/sys/class/hwmon/hwmon0/device/temp1_input",

                "/sys/kernel/debug/tegra_thermal/temp_tj",
               
                "/sys/htc/cpu_temp"
            };

            foreach (var p in paths)
            {
                try
                {
                    cpuTempFile = new RandomAccessFile(p, "r");
                    //cpuTempFile.Seek(0);
                    //var s = cpuTempFile?.ReadLine();
                    break;
                }
                catch { }
            }
        }

        public bool IsChromeOs
        {
            get
            {
                if (isChromeOs == null)
                    try { isChromeOs = Android.App.Application.Context.PackageManager.HasSystemFeature("org.chromium.arc.device_management"); } catch { isChromeOs = false; }

                return isChromeOs.Value;
            }
        }

        public double GetCpuTemp()
        {
            if (cpuTempFile == null) throw new NotSupportedException("CPU Temp not available");

            cpuTempFile.Seek(0);
            var s = cpuTempFile?.ReadLine();

            if (string.IsNullOrWhiteSpace(s)) throw new NotSupportedException("CPU Temp not available");

            double val;

            if (!double.TryParse(s, out val)) throw new NotSupportedException("CPU Temp not available");

            if (val < -30 || val > 300) val = val / 1000;

            return val;
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
