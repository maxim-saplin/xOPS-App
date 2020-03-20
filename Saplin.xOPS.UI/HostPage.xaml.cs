using Saplin.xOPS.UI.ViewModels;
using Saplin.xOPS.UI.VirtualPages;
using Xamarin.Forms;

namespace Saplin.xOPS.UI
{
    public partial class HostPage : ContentPage
    {
        private AbsoluteLayout layout;

        public HostPage()
        {
            InitializeComponent();

            layout = (AbsoluteLayout)Content;
        }

        protected override bool OnBackButtonPressed()
        {
            if (VmLocator.TestRun.TestStarted)
            {
                VmLocator.TestRun.BreakTest();
                return true;
            }

            if (VmLocator.Options.IsVisible)
            {
                VmLocator.Options.IsVisible = false;
                return true;
            }

            if (CurrentPage != HomePage)
            {
                ShowVirtualPage(HomePage);
                return true;
            }

            return base.OnBackButtonPressed();
        }

        public void AddVirtualPage(Layout page)
        {
            page.IsVisible = false;
            layout.Children.Add(page);
            AbsoluteLayout.SetLayoutFlags(page, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(page, new Rectangle(1.0, 1.0, 1.0, 1.0));
        }

        public Layout CurrentPage { get; protected set; }

        public Layout HomePage { get; protected internal set; }

        public void ShowVirtualPage(Layout page)
        {
            foreach(var c in layout.Children)
            {
                if (c == page)
                {
                    c.IsVisible = true;
                    if (c is IAppearing)
                        (c as IAppearing).OnAppearing();
                    CurrentPage = page;
                }
                else c.IsVisible = false;
            }
        }

    }
}
