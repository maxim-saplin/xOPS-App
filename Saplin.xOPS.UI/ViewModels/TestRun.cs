using System;
using System.Threading.Tasks;
using Saplin.xOPS.UI.Misc;
using Xamarin.Forms;

namespace Saplin.xOPS.UI.ViewModels
{
    public class TestRun : BaseViewModel
    {
        Compute compute = new Compute();

        public TestRun()
        {
            NumberOfRepeats = 0;

            VmLocator.Options.OptionsChanged += (s, e) =>
            {
                NumberOfRepeats = 0;
                sumIntSingleThreaded = sumIntMultiThreaded
                 = sumFloatSingleThreaded = sumFloatMultiThreaded = 0;
            };

            VmLocator.L11n.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(L11n._Locale))
                {
                    RaisePropertyChanged(nameof(NumberOfRepeatsText));
                }
            };
        }

        private void ResetValues()
        {
            IntMultiThreaded = IntSingleThreaded
                = FloatMultiThreaded = FloatSingleThreaded
                = RecentIntMultiThreaded = RecentIntSingleThreaded
                = RecentFloatMultiThreaded = RecentFloatSingleThreaded = null;

            RaisePropertyChanged(nameof(FloatSingleThreaded));
            RaisePropertyChanged(nameof(IntSingleThreaded));
            RaisePropertyChanged(nameof(FloatMultiThreaded));
            RaisePropertyChanged(nameof(IntMultiThreaded));

            RaisePropertyChanged(nameof(RecentFloatSingleThreaded));
            RaisePropertyChanged(nameof(RecentIntSingleThreaded));
            RaisePropertyChanged(nameof(RecentFloatMultiThreaded));
            RaisePropertyChanged(nameof(RecentIntMultiThreaded));
        }

        public Command Retry => new Command(StartTest);
        private volatile bool breakTest = false;

        public void StartTest()
        {
            VmLocator.OnlineDb.SendPageHit("start");
            ScreenOn.Enable();

            const int iterations = 50 * 1000 * 1000;

            TestNotStarted = false;
            TestInterrupted = false;
            breakTest = false;

            ResetValues();

            var options = VmLocator.Options;

            Task.Run(() => {
                try
                {
                    compute.RunXops(iterations, inops: false, options.Float64Bit); 
                    RecentFloatSingleThreaded = breakTest ? (double?)null : compute.LastResultSTGigaOPSAveraged;
                    Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(RecentFloatSingleThreaded)));

                    if (breakTest) return;

                    compute.RunXops(iterations, inops: true, options.Int64Bit);
                    RecentIntSingleThreaded = breakTest ? (double?)null : compute.LastResultSTGigaOPSAveraged;
                    Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(RecentIntSingleThreaded)));

                    if (breakTest) return;

                    compute.RunXopsMultiThreaded(iterations, options.FloatThreads, inops: false, precision64Bit: options.Float64Bit);
                    RecentFloatMultiThreaded = breakTest ? (double?)null : compute.LastResultGigaOPS;
                    Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(RecentFloatMultiThreaded)));

                    if (breakTest) return;

                    compute.RunXopsMultiThreaded(iterations, options.IntThreads, inops: true, precision64Bit: options.Int64Bit);
                    RecentIntMultiThreaded = breakTest ? (double?)null : compute.LastResultGigaOPS;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        RaisePropertyChanged(nameof(RecentIntMultiThreaded));
                    });
                 }
                 catch(Exception ex)
                 {

                 }
                 finally
                 {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ScreenOn.Disable();
                        TestNotStarted = true;
                        if (!breakTest && RecentFloatSingleThreaded.HasValue)
                        {
                            NumberOfRepeats++;

                            sumFloatMultiThreaded += RecentFloatMultiThreaded.Value;
                            sumFloatSingleThreaded += RecentFloatSingleThreaded.Value;
                            sumIntMultiThreaded += RecentIntMultiThreaded.Value;
                            sumIntSingleThreaded += RecentIntSingleThreaded.Value;

                            FloatMultiThreaded = sumFloatMultiThreaded / NumberOfRepeats;
                            RaisePropertyChanged(nameof(FloatMultiThreaded));

                            FloatSingleThreaded = sumFloatSingleThreaded / NumberOfRepeats;
                            RaisePropertyChanged(nameof(FloatSingleThreaded));

                            IntMultiThreaded = sumIntMultiThreaded / NumberOfRepeats;
                            RaisePropertyChanged(nameof(IntMultiThreaded));

                            IntSingleThreaded = sumIntSingleThreaded / NumberOfRepeats;
                            RaisePropertyChanged(nameof(IntSingleThreaded));

                            RaisePropertyChanged(nameof(NumberOfRepeats));
                            RaisePropertyChanged(nameof(NumberOfRepeatsText));

                            VmLocator.QuickComparison.Compare.Execute(new SingleResult()
                            {
                                Value = FloatSingleThreaded.Value,
                                Int = false,
                                MultiThreaded = false
                            });

                            VmLocator.OnlineDb.PreLoadComparison(this, VmLocator.Options);
                        }
                    });
                 }
            });

        }

        public Command Break => new Command(BreakTest);

        public void BreakTest()
        {
            breakTest = true;
            compute?.BreakExecution();
            TestInterrupted = true;
            VmLocator.OnlineDb.SendPageHit("break");
            ScreenOn.Disable();
        }

        private bool testNotStarted = true;
        public bool TestNotStarted
        {
            get { return testNotStarted; }
            set 
            { 
                testNotStarted = value; 
                RaisePropertyChanged(); 
                RaisePropertyChanged(nameof(TestStarted)); 
                RaisePropertyChanged(nameof(ShowQuickComparison));
                RaisePropertyChanged(nameof(ShowRunning));
            } 
        }

        public bool TestStarted => !TestNotStarted;

        private bool testInterrupted = false;
        public bool TestInterrupted
        {
            get { return testInterrupted; }
            set 
            { 
                testInterrupted = value; 
                RaisePropertyChanged(); 
                RaisePropertyChanged(nameof(ShowRunning)); 
            }
        }

        public bool ShowQuickComparison => TestNotStarted && !TestInterrupted;

        public bool ShowRunning => TestStarted && !TestInterrupted;

        private double sumFloatSingleThreaded = 0;
        private double sumFloatMultiThreaded = 0;
        private double sumIntSingleThreaded = 0;
        private double sumIntMultiThreaded = 0;

        public int NumberOfRepeats { get; private set; }

        public double? FloatSingleThreaded { get; private set; }
        public double? FloatMultiThreaded { get; private set; }
        public double? IntSingleThreaded { get; private set; }
        public double? IntMultiThreaded { get; private set; }

        public double? RecentFloatSingleThreaded { get; private set; }
        public double? RecentFloatMultiThreaded { get; private set; }
        public double? RecentIntSingleThreaded { get; private set; }
        public double? RecentIntMultiThreaded { get; private set; }

        public string NumberOfRepeatsText
        {
            get
            {
                var s = "";

                if (NumberOfRepeats == 1)
                {
                    s = VmLocator.L11n.AveragesOne;
                }
                else if (NumberOfRepeats > 1)
                {
                    s = string.Format(VmLocator.L11n.AveragesMany, NumberOfRepeats);

                    if (VmLocator.L11n._Locale == Locales.ru && (NumberOfRepeats < 5 || NumberOfRepeats > 20))
                    {
                        var nor = NumberOfRepeats.ToString();

                        if (nor.EndsWith("1"))
                        {
                            s = string.Format(VmLocator.L11n.AveragesOneRu, NumberOfRepeats);
                        }
                        else if (nor.EndsWith("2") || nor.EndsWith("3") || nor.EndsWith("4"))
                        {
                            s = string.Format(VmLocator.L11n.AveragesTwoRu, NumberOfRepeats);
                        }
                    }
                }

                return s;
            }
        }
    }
}
