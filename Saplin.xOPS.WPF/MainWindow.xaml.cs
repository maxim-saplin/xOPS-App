using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;

namespace Saplin.xOPS.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FormsApplicationPage
    {
        public MainWindow()
        {
            
            InitializeComponent();
            Forms.Init();
            var app = new Saplin.xOPS.UI.App();
            LoadApplication(app);
        }

    }
}
