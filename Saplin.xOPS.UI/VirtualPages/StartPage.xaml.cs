using System;
using Saplin.xOPS.UI.ViewModels;
using Xamarin.Forms;

namespace Saplin.xOPS.UI.VirtualPages
{
    public partial class StartPage : StackLayout, IAppearing
    {
        private int countdown = 3;

        public StartPage()
        {
            InitializeComponent();

        }

        public bool Skip { get; protected set; }

        private string[] roseTexts = { "Три Миссисипи", "Два Миссисипи", "Раз Миссисипи" };

        public void OnAppearing()
        {
            var padTo = 0;

            if (countdown != 0)
            {

                Func<bool> func = () =>
                {
                    if (countdown == 0)
                    {
                        countdownLabel.Text = "� � �";
                        Pages.ShowPage(Pages.MainPage);
                        Skip = true;
                        return false;
                    }

                    if (!((Saplin.xOPS.UI.App)App.Current).Rose)
                    {
                        if (countdown == 1)
                            countdownLabel.Text = VmLocator.L11n.CountdownOne;
                        else countdownLabel.Text = string.Format(VmLocator.L11n.CountdownMany, countdown);

                        if (padTo == 0) padTo = countdownLabel.Text.Length + countdown;

                        for (int i = 0; i < countdown; i++)
                            countdownLabel.Text += ".";

                        if (padTo != countdownLabel.Text.Length)
                            countdownLabel.Text = countdownLabel.Text.PadRight(padTo, ' ');
                    }
                    else
                    {

                        countdownLabel.Text = roseTexts[countdown - 1];
                    }

                    countdown--;

                    return true;
                };

                func();

                Device.StartTimer(TimeSpan.FromSeconds(1), func);
            }
        }
    }
}
