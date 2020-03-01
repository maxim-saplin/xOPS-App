using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Saplin.xOPS.UI
{
    public partial class StartPage : ContentPage
    {
        private int countdown = 1;

        public StartPage()
        {
            InitializeComponent();

            countdownLabel.Text = "Starting in " + countdown + " seconds";

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                countdown--;
                countdownLabel.Text = "Starting in " + countdown + " seconds";

                if (countdown == 0)
                {
                    //Navigation.PushModalAsync(new MainPage());
                    App.Current.MainPage = Pages.MainPage;
                }

                return countdown > 0;
            });
        }
    }
}
