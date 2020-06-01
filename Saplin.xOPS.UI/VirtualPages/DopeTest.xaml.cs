using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Saplin.xOPS.UI.Misc;
using Saplin.xOPS.UI.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Saplin.xOPS.UI.VirtualPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DopeTest : Grid, IAppearing
    {
        public DopeTest()
        {
            InitializeComponent();
        }

        void Back_Clicked(System.Object sender, System.EventArgs e)
        {
            Pages.ShowPage(Pages.About);
        }

        volatile bool breakTest = false;

        void StartTest()
        {
            VmLocator.OnlineDb.SendPageHit("dopeStart");

            var rand = new Random2(0);

            breakTest = false;

            var width = absolute.Width;
            var height = absolute.Height;

            retry.IsVisible = back.IsVisible = false;
            stop.IsVisible = true;

            const int max = 600;

            absolute.Children.Clear();

            var i = 0;
            var processed = 0;

            var thread = new Thread( () =>
            {
                while(true)
                {
                    if (processed < i - 20)
                    {
                        Thread.Sleep(1);
                        continue;
                    }

                    var label = new Label()
                    {
                        Text = "Dope",
                        TextColor = new Color(rand.NextDouble(), rand.NextDouble(), rand.NextDouble()),
                        AnchorX = 0.5,
                        AnchorY = 0.5,
                        Rotation = rand.NextDouble() * 360
                    };

                    AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.PositionProportional);
                    AbsoluteLayout.SetLayoutBounds(label, new Rectangle(rand.NextDouble(), rand.NextDouble(), 80, 24));

                    absolute.Dispatcher.BeginInvokeOnMainThread(() =>
                    {
                        if (i > max)
                        {
                            absolute.Children.RemoveAt(0);
                        }

                        absolute.Children.Add(label);

                        processed++;
                    });

                    if (breakTest) break;

                    i++;
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    stop.IsVisible = false;
                    retry.IsVisible = back.IsVisible = true;
                });
            });

            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Lowest;
            thread.Start();

            var sw = new Stopwatch();
            sw.Start();
            long prevTicks = 0;
            int prevProcessed = 0;
            double avgSum = 0;
            int avgN = 0;

            Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
            {
                if (retry.IsVisible)
                {
                    var avg = avgSum / avgN;
                    dopes.Text = string.Format("{0:0.00} Dopes/s (AVG)", avg).PadLeft(21);
                    VmLocator.OnlineDb.SendPageHit("dopeAvg", avg.ToString("0.00", new NumberFormatInfo() {NumberDecimalSeparator = "." }));
                    return false;
                }

                var r = (double)(processed - prevProcessed) / ((double)(sw.ElapsedTicks - prevTicks) / Stopwatch.Frequency);
                dopes.Text = string.Format("{0:0.00} Dopes/s", r).PadLeft(15);
                prevTicks = sw.ElapsedTicks;
                prevProcessed = processed;

                if (i > max)
                {
                    avgSum += r;
                    avgN++;
                }

                return true;                
            });
        }

        void Retry_Clicked(System.Object sender, System.EventArgs e)
        {
            StartTest();
        }

        void Stop_Clicked(System.Object sender, System.EventArgs e)
        {
            breakTest = true;
        }

        bool startedBefore = false;

        public void OnAppearing()
        {
            if (!startedBefore)
            {
                startedBefore = true;
                StartTest();
            }
        }
    }
}