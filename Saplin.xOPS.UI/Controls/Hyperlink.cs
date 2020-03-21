using System;
using Xamarin.Forms;
using System.Windows.Input;

namespace Saplin.xOPS.UI.Controls
{
    public class Hyperlink: Label
    {
        public static readonly BindableProperty UrlProperty =
            BindableProperty.Create(nameof(Url), typeof(string), typeof(Hyperlink), null);

        public string Url
        {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        public Hyperlink()
        {
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                // Launcher.OpenAsync is provided by Xamarin.Essentials.
                Command = new Command(() =>  Device.OpenUri(new Uri(Url)))
            });
        }
    }
}
