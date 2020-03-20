using Xamarin.Forms;
using Saplin.xOPS.UI.Misc;
using System.Management;

[assembly: Dependency(typeof(Saplin.xOPS.WPF.DeviceInfo))]
namespace Saplin.xOPS.WPF
{
    public class DeviceInfo : IDeviceInfo
    {
        private string cpu = null;
        private string model = null;
        private float? ram = null;

        public bool IsChromeOs => false;

        public string GetCPU()
        {
            if (cpu == null)
            {
                cpu = "";
                try
                {
                    var mc = new ManagementClass("Win32_Processor").GetInstances();

                    foreach (var c in mc)
                    {
                        var val = c.Properties["Name"];
                        if (val != null && val.Value != null)
                        {
                            cpu = val.Value.ToString();
                            break;
                        }
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
                    var mc = new ManagementClass("Win32_ComputerSystem").GetInstances();

                    foreach (var c in mc)
                    {
                        var val = c.Properties["Model"];
                        if (val != null && val.Value != null)
                        {
                            model = val.Value.ToString();
                            break;
                        }
                    }

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
                    var mc = new ManagementClass("Win32_ComputerSystem").GetInstances();

                    foreach (var c in mc)
                    {
                        var val = c.Properties["TotalPhysicalMemory"];
                        if (val != null && val.Value != null)
                        {
                            ram = ((ulong)val.Value)/(float)1024/1024/1024;
                            break;
                        }
                    }

                }
                catch { };
            }

            return ram.Value;
        }
    }
}
