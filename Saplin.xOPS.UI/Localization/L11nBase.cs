//Cope-Paste from generated resx helper class with properties converted from static to instance
//TODO - use T4 template for code generation
namespace Saplin.xOPS.UI.ViewModels
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class L11nBase : BaseViewModel
    {

        private static global::System.Resources.ResourceManager resourceMan;

        protected static global::System.Globalization.CultureInfo resourceCulture;

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Saplin.xOPS.UI.Localization.L11ns", typeof(Saplin.xOPS.UI.Localization.L11ns).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }


        public string About
        {
            get
            {
                return ResourceManager.GetString("About", resourceCulture);
            }
        }

        public string Options
        {
            get
            {
                return ResourceManager.GetString("Options", resourceCulture);
            }
        }

        public string Close
        {
            get
            {
                return ResourceManager.GetString("Close", resourceCulture);
            }
        }
    }
}
