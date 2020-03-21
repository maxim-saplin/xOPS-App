using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace Saplin.xOPS.UI.ViewModels
{

    public static class Locales
    {
        public const string en = "en";
        public const string ru = "ru";
        public const string fr = "fr";
        public const string zh = "zh";

        public static bool IsValid(string locale)
        {
            if (locale == en) return true;
            if (locale == ru) return true;
            if (locale == fr) return true;
            if (locale == zh) return true;

            return false;
        }
    };

    public class L11n : L11nBase 
    {
        public string locale = Locales.en;
        public bool needInit = true;
        private readonly NumberFormatInfo nfi = new NumberFormatInfo() { NumberDecimalSeparator = "." };

        public L11n()
        {
            if (!App.Current.Properties.ContainsKey(nameof(_Locale)) || !Locales.IsValid(App.Current.Properties[nameof(_Locale)].ToString()))
            {
                var sysLocale = string.Empty;

                // Google Firebase allows to define locale with 2 letters, e.g. Fr, in this case the app
                // has NULL culture. Though I didn't see that phones allow to set locale without a country,
                // didn't see a real phone where the below approach didn't work
                try
                {
                    sysLocale = CultureInfo.CurrentUICulture.Name.ToLower().Substring(0, 2);
                }
                catch { };

                if (Locales.IsValid(sysLocale)) App.Current.Properties[nameof(_Locale)] = sysLocale;
                else App.Current.Properties[nameof(_Locale)] = Locales.en;
            }

            _Locale = App.Current.Properties[nameof(_Locale)].ToString();

        }


        public string _Locale
        {
            get
            {
                return locale;
            }
            set
            {
                if ((value != locale || needInit) && Locales.IsValid(value))
                {
                    needInit = false;
                    locale = value;
                    App.Current.Properties[nameof(_Locale)] = value;
                    App.Current.SavePropertiesAsync();

                    L11nBase.Culture = new System.Globalization.CultureInfo(locale);
                    Thread.CurrentThread.CurrentCulture = L11nBase.Culture;
                    Thread.CurrentThread.CurrentUICulture = L11nBase.Culture;

                    var properties = typeof(L11nBase).GetProperties().Where(p => p.PropertyType == typeof(string));

                    RaisePropertyChanged();

                    foreach (var p in properties)
                    {
                        RaisePropertyChanged(p.Name);
                    }

                    RaisePropertyChanged(nameof(Ru));
                    RaisePropertyChanged(nameof(En));
                    RaisePropertyChanged(nameof(Fr));
                    RaisePropertyChanged(nameof(Zh));
                }
            }
        }

        public ICommand ChangeLocale => new Command((object locale) => { _Locale = (locale as string); });

        public ICommand Next => new Command(() =>
        {
            if (_Locale == Locales.ru) _Locale = Locales.en;
            else if (_Locale == Locales.en) _Locale = Locales.fr;
            else if (_Locale == Locales.fr) _Locale = Locales.zh;
            else _Locale = Locales.ru; 
        });

        public ICommand Previous => new Command(() =>
        {
            if (_Locale == Locales.ru) _Locale = Locales.zh;
            else if (_Locale == Locales.zh) _Locale = Locales.fr;
            else if (_Locale == Locales.fr) _Locale = Locales.en;
            else _Locale = Locales.ru; 
        });

        public string this[string key]
        {
            get
            {
                return ResourceManager.GetString(key, resourceCulture);
            }
        }

        public bool Ru { get {return _Locale == Locales.ru;} }
        public bool En { get { return _Locale == Locales.en; } }
        public bool Fr { get { return _Locale == Locales.fr; } }
        public bool Zh { get { return _Locale == Locales.zh; } }
    }
}
