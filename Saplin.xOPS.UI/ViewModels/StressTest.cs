using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Saplin.xOPS.UI.Misc;
using Xamarin.Forms;

namespace Saplin.xOPS.UI.ViewModels
{
    public class StressTest : BaseViewModel
    {
        public StressTest()
        {
        }

        public Command Retry => new Command(StartTest);

        private volatile bool stopTest = false;

        Saplin.xOPS.StressTest stressTest;
        Stopwatch sw = new Stopwatch();

        const int samplingMs = 500;
        const int smoothing = 2;
        const int warmUpSample = 7;

        IDeviceInfo di;
        public void StartTest()
        {
            VmLocator.OnlineDb.SendPageHit("stressStart");
            ScreenOn.Enable();
            TestNotStarted = false;
            UpdateCounter = 0;

            stressTest = new Saplin.xOPS.StressTest(samplingMs, smoothing, warmUpSample, Environment.ProcessorCount, true, true);

            Gflops = stressTest.GflopsResults?.SmoothResults;
            Ginops = stressTest.GinopsResults?.SmoothResults;
            Temp = null;

            di = DependencyService.Get<IDeviceInfo>();

            try
            {
                di.GetCpuTemp();
                Temp = new List<double>();
                RaisePropertyChanged(nameof(Temp));
            }
            catch { }

            var tempText = string.Empty;

            if (Temp == null)
            {
                tempText = TempLabel = VmLocator.L11n.TempNotAvailable + 
                    (Device.RuntimePlatform == Device.WPF && !di.IsAdmin ?  " - " + VmLocator.L11n.TryAdmin : "")
                    + "\n";
            }

            RaisePropertyChanged(nameof(Gflops));
            RaisePropertyChanged(nameof(Ginops));

            var label1 = VmLocator.L11n.Start + ": {1:0.00} {0}\n" + VmLocator.L11n.Now + ": {2:0.00} {0}";
            var label2 = "{1:0.00} {0}\n{2:0.00}%";

            var prevCount = 0;

            stressTest.ResultsUpdated += (e) =>
            {
                if (stressTest.WarmpingUp)
                {
                    GflopsLabel = GinopsLabel = Environment.ProcessorCount + " "+VmLocator.L11n.threads+" \n" + VmLocator.L11n.WarmingUp + "...";
                    TempLabel = VmLocator.L11n.WarmingUp + "...";
                }
                else
                {
                    var count = Gflops != null ? Gflops.Count
                        : Ginops != null ? Ginops.Count : 0;

                    if (prevCount != count)
                    {
                        prevCount = count;

                        if (stressTest.GflopsResults != null)
                        {
                            GflopsLabel = UpdateCounter < 10 ?
                                string.Format(label1,
                                    "GFLOPS",
                                    stressTest.GflopsResults.StartSmooth,
                                    stressTest.GflopsResults.CurrentSmooth) :
                                string.Format(label2,
                                    "GFLOPS",
                                    stressTest.GflopsResults.CurrentSmooth,
                                    ((stressTest.GflopsResults.CurrentSmooth - stressTest.GflopsResults.StartSmooth) / stressTest.GflopsResults.StartSmooth * 100));
                        }

                        if (stressTest.GinopsResults != null)
                        {
                            GinopsLabel = UpdateCounter < 14 ?
                                string.Format(label1,
                                    "GINOPS",
                                    stressTest.GinopsResults.StartSmooth,
                                    stressTest.GinopsResults.CurrentSmooth) :
                                string.Format(label2,
                                    "GINOPS",
                                    stressTest.GinopsResults.CurrentSmooth,
                                    ((stressTest.GinopsResults.CurrentSmooth - stressTest.GinopsResults.StartSmooth) / stressTest.GinopsResults.StartSmooth * 100));
                        }

                        if (Temp != null)
                        {
                            try
                            {
                                var temp = di.GetCpuTemp();
                                Temp.Add(temp);
                                tempText = "CPU " + temp.ToString("0.0") + "°C "
                                 + (temp > Temp[0] ? "↑" : "↓")
                                 + (temp - Temp[0]).ToString("0.0") + "°C";

                            }
                            catch { };
                        }                            

                        Update();
                    }

                    TempLabel = tempText + "\n" + sw.Elapsed.Minutes + (UpdateCounter % 2 == 0 ? ":" : ".") + sw.Elapsed.Seconds.ToString("00"); ;
                }

                RaisePropertyChanged(nameof(GflopsLabel));
                RaisePropertyChanged(nameof(GinopsLabel));
                RaisePropertyChanged(nameof(TempLabel));
            };

            stressTest.Start();
            sw.Restart();
        }

        public int UpdateCounter { get; private set; } = 0;
        void Update() { UpdateCounter++; RaisePropertyChanged(nameof(UpdateCounter)); }

        [DataContract]
        internal class StressSummary
        {
            [DataMember] public double FLT_START; 
            [DataMember] public double FLT_END;
            [DataMember] public double FLT_DIFP;
            [DataMember] public double INT_START;
            [DataMember] public double INT_END;
            [DataMember] public double INT_DIFP;
            [DataMember] public double TEMP_START;
            [DataMember] public double TEMP_END;
            [DataMember] public double TEMP_DELT;
            [DataMember] public double SECONDS;
        }

        public Command Stop => new Command(StopTest);

        public void StopTest()
        {
            stressTest?.Stop();
            sw.Stop();
            TestNotStarted = true;

            var label1 = string.Empty;

            if (stressTest.GflopsResults?.SmoothResults?.Count > 10 || stressTest.GflopsResults?.SmoothResults?.Count > 10)
                label1 = VmLocator.L11n.First5Secs + ": {1:0.00} {0}\n" + VmLocator.L11n.Last5Secs + ": {2:0.00} {0}\n{3} {4:0.00}%";
            else label1 = VmLocator.L11n.Start + ": {1:0.00} {0}\n" + VmLocator.L11n.End + ": {2:0.00} {0}\n{3} {4:0.00}%";

            var ss = new StressSummary() { SECONDS = Math.Round(sw.Elapsed.TotalSeconds,2)};

            if (stressTest.GflopsResults != null)
            {
                GflopsLabel = GetResultLabel(stressTest.GflopsResults, label1, "GFLOPS",
                    out ss.FLT_START, out ss.FLT_END, out ss.FLT_DIFP);
            }

            if (stressTest.GinopsResults != null)
            {
                GinopsLabel = GetResultLabel(stressTest.GinopsResults, label1, "GINOPS",
                    out ss.INT_START, out ss.INT_END, out ss.INT_DIFP);
            }

            if (Temp != null && Temp.Count > 2)
            {
                ss.TEMP_START = Temp[0];
                ss.TEMP_END = Temp.Last();
                ss.TEMP_DELT = Math.Round(ss.TEMP_END - ss.TEMP_START, 2);
            }

            RaisePropertyChanged(nameof(GflopsLabel));
            RaisePropertyChanged(nameof(GinopsLabel));
            RaisePropertyChanged(nameof(TempLabel));

            if (di is IDisposable) (di as IDisposable).Dispose(); // WPF CPU temp uses some hard staff for CPU temp monitoring

            ScreenOn.Disable();
            VmLocator.OnlineDb.SendPageHit("stressStop", ss);
        }

        private string GetResultLabel(TimeSeries ts, string label1, string unit, out double start, out double end, out double diff)
        {
            GetStartEnd(ts, out start, out end);

            start = Math.Round(start, 2);
            end = Math.Round(end, 2);

            diff = (end - start) / start * 100;

            diff = Math.Round(diff, 2);

            var result =
                string.Format(label1,
                    unit,
                    start,
                    end,
                    (diff < -5 ? "↓" : diff > 5 ? "↑" : "⇆"),
                    diff
                   );
            return result;
        }

        private void GetStartEnd(TimeSeries ts, out double start, out double end)
        {
            if (sw.Elapsed.Seconds < 10)
            {
                start = ts.StartSmooth;
                end = ts.CurrentSmooth;
            }
            else
            {
                start = ts.SmoothResults.Take(5).Average();
                end = ts.SmoothResults.Skip(ts.SmoothResults.Count - 5).Take(5).Average();
            }
        }

        bool testNotStarted = true;

        public bool TestNotStarted
        {
            get { return testNotStarted; }
            set
            {
                testNotStarted = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(TestStarted));
            }
        }

        public bool TestStarted => !TestNotStarted;

        public IList<double> Gflops { get; set; }
        public IList<double> Ginops { get; set; }
        public IList<double> Temp { get; set; }

        public string GflopsLabel { get; set; }
        public string GinopsLabel { get; set; }
        public string TempLabel { get; set; }
    }
}