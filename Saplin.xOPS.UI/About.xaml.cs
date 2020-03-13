using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Saplin.xOPS.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class About : ContentPage
    {
        public About()
        {
            InitializeComponent();

            if (((Saplin.xOPS.UI.App)App.Current).Rose) rose.IsVisible = true;

            var a = Assembly.GetExecutingAssembly();
            var v = a.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            version.Text = v?.InformationalVersion;
        }

        private void BackLabel_Clicked(object sender, EventArgs e)
        {
            Pages.About.Navigation.PopModalAsync();
        }
    }
}