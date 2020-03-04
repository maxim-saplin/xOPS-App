using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Saplin.xOPS.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Options : Grid
    {
        public Options()
        {
            InitializeComponent();

            //macOS workaround, Rotation property on Element is not working
            if (Device.RuntimePlatform == Device.macOS)
            {
                stLabel.RotateTo(-90);
                mtLabel.RotateTo(-90);
            }
        }
    }
}