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
        WebView webView;

        public OnlineDb()
        {
            InitializeComponent();

            try
            {
                webView = new WebView();
                if (Device.RuntimePlatform != Device.WPF)
                {
                    this.Children.Add(webView, 0, 0);
                    Grid.SetColumnSpan(webView, 2);
                }
                else //WPF has issues with WebView, it always stays on top of label. If moved to separate Grid column then there's white vertical stripe between webview and back label - some layout issues
                {
                    this.Children.Add(webView, 0, 0);
                    Grid.SetColumnSpan(webView, 2);
                    webView.TranslationX = +30;
                    webView.Margin = new Thickness(0, 0, 30, 0);
                }

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