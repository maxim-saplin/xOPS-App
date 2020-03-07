using System;
using Saplin.xOPS.UI.ViewModels;
using Xamarin.Forms;

namespace Saplin.xOPS.UI
{
    public partial class StartPage : ContentPage
    {
        private int countdown = 3;

        public StartPage()
        {
            InitializeComponent();

        }

        private string[] roseTexts = { "Три Миссисипи", "Два Миссисипи", "Раз Миссисипи" };

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (countdown == 0) App.Current.MainPage = Pages.MainPage;
            else
            {

                Func<bool> func = () =>
                {
                    if (countdown == 0)
                    {
                        countdownLabel.Text = "� � �";
                        App.Current.MainPage = Pages.MainPage;
                        return false;
                    }

                    if (!((Saplin.xOPS.UI.App)App.Current).Rose)
                    {
                        if (countdown == 1)
                            countdownLabel.Text = VmLocator.L11n.CountdownOne;
                        else countdownLabel.Text = string.Format(VmLocator.L11n.CountdownMany, countdown);
                    }
                    else countdownLabel.Text = roseTexts[countdown - 1];

                    countdown--;

                    return true;
                };

                func();

                Device.StartTimer(TimeSpan.FromSeconds(1), func);
            }
        }
    }
}
