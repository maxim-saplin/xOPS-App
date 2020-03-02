using System.Windows.Input;
using Xamarin.Forms;

namespace Saplin.xOPS.UI.ViewModels
{
    public class Options : BaseViewModel
    {
        private bool isVisible = false;
        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; RaisePropertyChanged(); RaisePropertyChanged(nameof(ButtonCaption)); }
        }

        public ICommand SwitchOptionsVisibility => new Command(() => IsVisible = !IsVisible);

        public string ButtonCaption
        {
            get { return IsVisible ? " #❌#" : " # #"; }
        }


    }
}
