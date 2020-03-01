using System;

using Xamarin.Forms;

namespace Saplin.xOPS.UI
{
    public class About : ContentPage
    {
        public About()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}

