using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Saplin.xOPS.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestRun : Grid
    {
        public TestRun()
        {
            InitializeComponent();

            //macOS workaround, Rotation property on Element is not working
            if (Device.RuntimePlatform == Device.macOS)
            {
                stLabel.RotateTo(-90);
                mtLabel.RotateTo(-90);
            }

            try
            {
                if ((bool)App.Current.Resources["Rose"])
                {
                    var img = new Image
                    {
                        Source = ImageSource.FromResource(
                        "Saplin.xOPS.UI.Misc.rose.png",
                        typeof(TestRun).GetTypeInfo().Assembly
                      )
                    };

                    img.Margin = new Thickness(-10, -10, -10, -10);

                    placeholder.Children.Add(img);
                }
            }
            catch { }
        }

        public Grid Core => core;
    }
}