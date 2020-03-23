using System;
using System.ComponentModel;
using Saplin.xOPS.UI.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Saplin.xOPS.UI.VirtualPages
{
    [DesignTimeVisible(false)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OnlineDb : Grid
    {
        public OnlineDb()
        {
            InitializeComponent();

            try
            {
                var webView = new WebView();
                this.Children.Add(webView, 0, 0);
                Grid.SetColumnSpan(webView, 2);
                Grid.SetRowSpan(webView, 2);
                VmLocator.OnlineDb.BindWebView(webView);
                var lbl = this.Children[0];
                this.Children.Remove(lbl);
                this.Children.Add(lbl);
            }
            catch(Exception ex) { Pages.SetOnlineDbOk(false); }
        }

        private void BackLabel_Clicked(object sender, EventArgs e)
        {
            Pages.ShowPage(Pages.MainPage);
        }
    }
}