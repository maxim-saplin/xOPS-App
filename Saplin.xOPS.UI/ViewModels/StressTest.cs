using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public void StartTest()
        {
            ScreenOn.Enable();
            TestNotStarted = false;
            UpdateCounter = 0;

            stressTest = new Saplin.xOPS.StressTest(Environment.ProcessorCount, true, true) { SamplingPeriodMs = 500, WarmpUpSamples = 7 };

            Gflops = stressTest.GflopsResults?.Results;
            Ginops = stressTest.GinopsResults?.Results;
            Temp = null;

            var di = DependencyService.Get<IDeviceInfo>();

            try
            {
                di.GetCpuTemp();
                Temp = new List<double>();
                RaisePropertyChanged(nameof(Temp));
            }
            catch { }

            RaisePropertyChanged(nameof(Gflops));
            RaisePropertyChanged(nameof(Ginops));

            var label1 = "Start: {1:0.00} {0}\nNow: {2:0.00} {0}\nSmoothing by {3} points";
            var label2 = "{1:0.00} {0}\n{2:0.00}%";

            stressTest.ResultsUpdated += (e) =>
            {
                if (stressTest.WarmpingUp)
                {
                    GflopsLabel = GinopsLabel = TempLabel = "Warming up...";
                }
                else
                {
                    

                    if (stressTest.GflopsResults != null)
                    {
                        GflopsLabel = UpdateCounter < 10 ?
                            string.Format(label1,
                                "GFLOPS",
                                stressTest.GflopsResults.StartValue,
                                stressTest.GflopsResults.CurrentValue,
                                stressTest.SmoothingPoints) :
                            string.Format(label2,
                                "GFLOPS",
                                stressTest.GflopsResults.CurrentValue,
                                ((stressTest.GflopsResults.CurrentValue - stressTest.GflopsResults.StartValue) / stressTest.GflopsResults.StartValue * 100));
                    }

                    if (stressTest.GinopsResults != null)
                    {
                        GinopsLabel = UpdateCounter < 14 ?
                            string.Format(label1,
                                "GINOPS",
                                stressTest.GinopsResults.StartValue,
                                stressTest.GinopsResults.CurrentValue,
                                stressTest.SmoothingPoints) :
                            string.Format(label2,
                                "GINOPS",
                                stressTest.GinopsResults.CurrentValue,
                                ((stressTest.GinopsResults.CurrentValue - stressTest.GinopsResults.StartValue) / stressTest.GinopsResults.StartValue * 100));
                    }

                    if (Temp != null)
                    {
                        try
                        {
                            var temp = di.GetCpuTemp();
                            Temp.Add(temp);
                            TempLabel = "CPU "+ temp.ToString("0.0") + "°C "
                             + (temp > Temp[0] ? "↑"+(temp-Temp[0]).ToString("0.0") + "°C"
                             : "↓" + (Temp[0]- temp).ToString("0.0") + "°C")
                              +"\n"+sw.Elapsed.Minutes + ":" + sw.Elapsed.Seconds.ToString("00");
                        }
                        catch { };
                    }
                    else
                        TempLabel = "Temperature not available\n"
                            + sw.Elapsed.Minutes + ":" + sw.Elapsed.Seconds.ToString("00");

                    Update();
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

        public Command Stop => new Command(StopTest);

        public void StopTest()
        {
            stressTest?.Stop();
            TestNotStarted = true;
            VmLocator.OnlineDb.SendPageHit("breakStress");

            var label1 = "Start: {1:0.00} {0}\nEnd: {2:0.00} {0}\nDifference: {3:0.00}%";

            if (stressTest.GflopsResults != null)
            {
                GflopsLabel =
                    string.Format(label1,
                        "GFLOPS",
                        stressTest.GflopsResults.StartValue,
                        stressTest.GflopsResults.CurrentValue,
                        (stressTest.GflopsResults.CurrentValue - stressTest.GflopsResults.StartValue) / stressTest.GflopsResults.StartValue * 100);
            }

            if (stressTest.GinopsResults != null)
            {
                GinopsLabel = 
                    string.Format(label1,
                        "GINOPS",
                        stressTest.GinopsResults.StartValue,
                        stressTest.GinopsResults.CurrentValue,
                        (stressTest.GinopsResults.CurrentValue - stressTest.GinopsResults.StartValue) / stressTest.GinopsResults.StartValue * 100);
            }

            RaisePropertyChanged(nameof(GflopsLabel));
            RaisePropertyChanged(nameof(GinopsLabel));
            RaisePropertyChanged(nameof(TempLabel));

            ScreenOn.Disable();
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