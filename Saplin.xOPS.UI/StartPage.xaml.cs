using System;

using Xamarin.Forms;

namespace Saplin.xOPS.UI
{
    public partial class StartPage : ContentPage
    {
        private int countdown = 1;

        public StartPage()
        {
            InitializeComponent();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (countdown == 0) App.Current.MainPage = Pages.MainPage;
            else
            {

                countdownLabel.Text = "Starting in " + countdown + " seconds";

                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    countdown--;
                    countdownLabel.Text = "Starting in " + countdown + " seconds";

                    if (countdown == 0)
                    {
                        countdownLabel.Text = "� � �";
                        App.Current.MainPage = Pages.MainPage;
                    }

                    return countdown > 0;
                });
            }
        }
    }
}
