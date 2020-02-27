using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saplin.xOPS.UI.ViewModels;
using Xamarin.Forms;

namespace Saplin.xOPS.UI
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            //lbl.RotateTo(90);

            //macOS workaround, Rotation property on Element is not working
            stLabel.RotateTo(-90);
            mtLabel.RotateTo(-90);

            var started = false;

            this.SizeChanged += (s, e) => { if (!started) { started = true; VmLocator.TestRun.StartTest(); } };

        }
    }
}
