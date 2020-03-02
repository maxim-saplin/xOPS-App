using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saplin.xOPS.UI.ViewModels;
using Xamarin.Forms;

namespace Saplin.xOPS.UI
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private bool startedBefore = false;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!startedBefore)
            {
                startedBefore = true;
                VmLocator.TestRun.StartTest();
            }
        }

        void AboutLabel_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(Pages.About);
        }
    }
}
