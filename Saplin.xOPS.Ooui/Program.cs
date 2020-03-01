using xxui = Ooui;
using Saplin.xOPS.UI;
using Xamarin.Forms;

namespace Saplin.xOPS.Ooui
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Forms.Init();

            var page = new StartPage();

            xxui.UI.Publish("/", page.GetOouiElement());
        }
    }
}
