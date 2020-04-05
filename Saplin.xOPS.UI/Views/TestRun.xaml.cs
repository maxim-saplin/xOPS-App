using System.Reflection;
using Saplin.xOPS.UI.ViewModels;
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

            VmLocator.TestRun.PropertyChanged += (s,e) =>
            {
                if (e.PropertyName == nameof(VmLocator.TestRun.TestStarted))
                {
                    if (VmLocator.TestRun.TestStarted)
                    {
                        ViewExtensions.CancelAnimations(fltStSmall);
                        ViewExtensions.CancelAnimations(fltMtSmall);
                        ViewExtensions.CancelAnimations(intStSmall);
                        ViewExtensions.CancelAnimations(intMtSmall);
                        fltStSmall.Opacity = fltMtSmall.Opacity
                            = intStSmall.Opacity = intMtSmall.Opacity = 1;
                   }
                }
            };

            VmLocator.QuickComparison.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(VmLocator.QuickComparison.ComparedValue))
                {
                    fltStSmall.Opacity = fltMtSmall.Opacity
                        = intStSmall.Opacity = intMtSmall.Opacity = 1;
                    fltStSmall.FadeTo(0, 5000, Easing.SpringIn);
                    fltMtSmall.FadeTo(0, 5000, Easing.SpringIn);
                    intStSmall.FadeTo(0, 5000, Easing.SpringIn);
                    intMtSmall.FadeTo(0, 5000, Easing.SpringIn);
                }
            };
        }

        public Grid Core => core;
    }
}