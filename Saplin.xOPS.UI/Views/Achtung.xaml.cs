using Xamarin.Forms;

namespace Saplin.xOPS.UI.Views
{
    public partial class Achtung : StackLayout
    {
        public Achtung()
        {
            InitializeComponent();
        }

        public bool ProceedClicked { get; private set; } = false;

        void Cancel_Clicked(System.Object sender, System.EventArgs e)
        {
            IsVisible = false;
        }

        void Proceed_Clicked(System.Object sender, System.EventArgs e)
        {
            IsVisible = false;
            ProceedClicked = true;
            Pages.ShowPage(Pages.StressTest);
        }
    }
}
