using Saplin.xOPS.Extra.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(Saplin.xOPS.Droid.ShareViewProxy))]
namespace Saplin.xOPS.Droid
{
    public class ShareViewProxy : ShareViewAsImage
    {
        public ShareViewProxy() : base(MainActivity.Instance)
        {
        }
    }
}