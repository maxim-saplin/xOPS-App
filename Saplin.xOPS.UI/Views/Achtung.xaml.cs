using Xamarin.Forms;

namespace Saplin.xOPS.UI.Views
{
    public partial class Achtung : StackLayout
    {
        public Achtung()
        {
            InitializeComponent();
        }

        void Cancel_Clicked(System.Object sender, System.EventArgs e)
        {
            IsVisible = false;
        }
    }
}
