using Xamarin.Forms;
using Saplin.xOPS.UI.Misc;
using System.Management;
using System.Security.Principal;
using OpenHardwareMonitor.Hardware;
using System.Linq;
using System;

[assembly: Dependency(typeof(Saplin.xOPS.WPF.DeviceInfo))]
namespace Saplin.xOPS.WPF
{
    public class DeviceInfo : IDeviceInfo, IDisposable
    {
        private string cpu = null;
        private string model = null;
        private float? ram = null;

        public bool IsChromeOs => false;

        public bool IsAdmin => (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
             .IsInRole(WindowsBuiltInRole.Administrator);

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

        class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }

        UpdateVisitor updateVisitor;
        Computer computer;
        ISensor sensor;

        public double GetCpuTemp()
        {
            if (updateVisitor == null)
                updateVisitor = new UpdateVisitor();

            if (computer == null)
            {
                computer = new Computer();
                computer.Open();
                computer.CPUEnabled = true;
            }

            computer.Accept(updateVisitor);

            if (sensor == null)
                sensor = computer.Hardware.Where(i => i.HardwareType == HardwareType.CPU).FirstOrDefault()?.Sensors.Where(s => s.SensorType == SensorType.Temperature).LastOrDefault();

            return (double)sensor.Value;
        }

        ~DeviceInfo()
        {
            computer?.Close();
        }

        public void Dispose()
        {
            computer?.Close();
            computer = null;
            updateVisitor = null;
            sensor = null;
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
