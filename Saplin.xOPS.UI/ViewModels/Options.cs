using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace Saplin.xOPS.UI.ViewModels
{
    public class Options : BaseViewModel
    {
        public Options()
        {
            initThreadOptions();

            VmLocator.L11n.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(L11n._Locale))
                {
                    RaisePropertyChanged(nameof(FloatPrecision));
                    RaisePropertyChanged(nameof(IntPrecision));
                }
            };

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Float64Bit) ||
                e.PropertyName == nameof(Int64Bit) ||
                e.PropertyName == nameof(FloatThreads) ||
                e.PropertyName == nameof(IntThreads))
                {
                    OptionsChanged?.Invoke(this, null);
                }
            };
        }

        public event EventHandler OptionsChanged;

        private bool isVisible = false;

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; RaisePropertyChanged(); RaisePropertyChanged(nameof(ButtonCaption)); }
        }

        public ICommand SwitchOptionsVisibility => new Command(() =>
        {
            IsVisible = !IsVisible;
            if (IsVisible) VmLocator.OnlineDb.SendPageHit("options");
        });

        public string ButtonCaption
        {
            get { return !IsVisible ? "\n ± ⇆\n" : "\n"+VmLocator.L11n.Close+"\n"; }
        }

        public ICommand SwicthFloatPrecision => new Command(() => Float64Bit = !Float64Bit);

        public ICommand SwicthIntPrecision => new Command(() => Int64Bit = !Int64Bit);

        public ICommand SwitchFloatThreads => new Command(() => {
            var i = Array.IndexOf<int>(threadsOptions, FloatThreads)+1;

            if (i > threadsOptions.Length - 1) i = 0;

            FloatThreads = threadsOptions[i];
        });

        public ICommand SwitchIntThreads => new Command(() => {
            var i = Array.IndexOf<int>(threadsOptions, IntThreads)+1;

            if (i > threadsOptions.Length - 1) i = 0;

            IntThreads = threadsOptions[i];
        });

        public bool Float64Bit
        {
            get
            {
                if (!App.Current.Properties.ContainsKey(nameof(Float64Bit))) App.Current.Properties[nameof(Float64Bit)] = false;
                return (bool?)App.Current.Properties[nameof(Float64Bit)] ?? false;
            }
            set
            {
                App.Current.Properties[nameof(Float64Bit)] = value;
                App.Current.SavePropertiesAsync();
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(FloatPrecision));
            }
        }

        public string FloatPrecision
        {
            get
            {
                return Float64Bit ? "64\n" + VmLocator.L11n.Bit : "32\n" + VmLocator.L11n.Bit;
            }
        }

        public bool Int64Bit
        {
            get
            {
                if (!App.Current.Properties.ContainsKey(nameof(Int64Bit))) App.Current.Properties[nameof(Int64Bit)] = false;
                return (bool?)App.Current.Properties[nameof(Int64Bit)] ?? false;
            }
            set
            {
                App.Current.Properties[nameof(Int64Bit)] = value;
                App.Current.SavePropertiesAsync();
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IntPrecision));
            }
        }

        public string IntPrecision
        {
            get
            {
                return Int64Bit ? "64\n" + VmLocator.L11n.Bit : "32\n" + VmLocator.L11n.Bit;
            }
        }


        private int[] threadsOptions = new int[] {2, 8, 16, 32, 48, 64, 128, 256};
        private int suggestedThreads = 2;

        private void initThreadOptions()
        {
            suggestedThreads = Math.Max(2, Environment.ProcessorCount) * 2;
            var needsToAdd = false;
            int i;

            for (i=0; i < threadsOptions.Length; i++)
            {
                if (threadsOptions[i] == suggestedThreads) break;
                if (threadsOptions[i] > suggestedThreads)
                {
                    needsToAdd = true;
                    break;
                }
            }

            if (needsToAdd || i == threadsOptions.Length)
            {
                var arr = new List<int>(threadsOptions);
                arr.Insert(i, suggestedThreads);
                threadsOptions = arr.ToArray();
            }
        }

        private int ValidateAndFixThreads(int val)
        {
            if (Array.IndexOf(threadsOptions, val) < 0) return suggestedThreads;
            return val;
        }

        public int FloatThreads
        {
            get
            {
                if (!App.Current.Properties.ContainsKey(nameof(FloatThreads))) App.Current.Properties[nameof(FloatThreads)] = suggestedThreads;
                return (int?)App.Current.Properties[nameof(FloatThreads)] ?? suggestedThreads;
            }
            set
            {
                App.Current.Properties[nameof(FloatThreads)] = ValidateAndFixThreads(value);
                App.Current.SavePropertiesAsync();
                RaisePropertyChanged();
            }
        }

        public int IntThreads
        {
            get
            {
                if (!App.Current.Properties.ContainsKey(nameof(IntThreads))) App.Current.Properties[nameof(IntThreads)] = suggestedThreads;
                return (int?)App.Current.Properties[nameof(IntThreads)] ?? suggestedThreads;
            }
            set
            {
                App.Current.Properties[nameof(IntThreads)] = ValidateAndFixThreads(value);
                App.Current.SavePropertiesAsync();
                RaisePropertyChanged();
            }
        }

        public string IID
        {
            get
            {
                if (!Application.Current.Properties.ContainsKey("IID")) Application.Current.Properties["IID"] = Guid.NewGuid().ToString("N");
                return Application.Current.Properties["IID"] as string;
            }
        }

        public string Version
        {
            get
            {
                var a = Assembly.GetExecutingAssembly();
                var v = a.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                return v?.InformationalVersion;
            }
        }
    }
}
