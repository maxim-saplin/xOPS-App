using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Saplin.xOPS.UI.ViewModels
{
    public class QuickComparison : BaseViewModel
    {
        public QuickComparison()
        {
            VmLocator.TestRun.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(TestRun.TestStarted) && VmLocator.TestRun.TestStarted)
                {
                    FS = FM = IS = IM = false;
                }
            };
        }

        public class ReferenceRecord
        {
            public string Name { get; set; }
            public double GFlopsST { get; set; }
            public double GFlopsMT { get; set; }
            public double GInopsST { get; set; }
            public double GInopsMT { get; set; }
        }

        private ReferenceRecord[] ReferenceRecords = new ReferenceRecord[]
        {
            new ReferenceRecord() { Name = "1966 Apollo Guidance Computer", GFlopsST = 0.000085, GFlopsMT = 0.000085, GInopsST = 0.000085, GInopsMT = 0.000085 },
            new ReferenceRecord() { Name = "1993 Cray 3 Supercomputer", GFlopsST = 16, GFlopsMT = 16, GInopsST = 16, GInopsMT = 16 },
            new ReferenceRecord() { Name = "2013 Tianhe-2 Supercomputer", GFlopsST = 33862.7*1000, GFlopsMT = 33862.7*1000, GInopsST = 33862.7*1000, GInopsMT = 33862.7*1000 },
            new ReferenceRecord() { Name = "2018 Core i7 MacBook Pro", GFlopsST = 4.23, GFlopsMT = 36.1, GInopsST = 5.1, GInopsMT = 36.4 },
            new ReferenceRecord() { Name = "2019 Samsung Galaxy Note 10", GFlopsST = 3.38, GFlopsMT = 14.57, GInopsST = 3, GInopsMT = 13 },
        };

        private double? comparedValue;

        public double? ComparedValue
        {
            get
            {
                return comparedValue;
            }
            set
            {
                if (value != comparedValue)
                {
                    comparedValue = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(Comparisons));
                }
            }
        }

        public bool UseInt { get; set; }

        public bool UseMultiThreaded { get; set; }

        private string GetComparisonValue(ReferenceRecord reference)
        {
            const int pad = 7;

            var val = ComparedValue.Value / (UseInt ? (UseMultiThreaded ? reference.GInopsMT : reference.GInopsST)
                : (UseMultiThreaded ? reference.GFlopsMT : reference.GFlopsST));

            if (val > 0.01 && val < 10) return (val.ToString("F2") + "x").PadRight(pad);

            if (val > 10 && val < 1000) return (val.ToString("F0") + "x").PadRight(pad);

            var s = val.ToString("E0");

            try
            {
                var ss = s.Split('E');

                s = ss[1].TrimStart(new char[] { '+', '0' });

                if (s.StartsWith("-"))
                {
                    s = "-"+s.TrimStart(new char[] { '-', '0' });
                }

                s = s.Replace("-", "⁻")
                    .Replace("0", "⁰")
                    .Replace("1", "¹")
                    .Replace("2", "²")
                    .Replace("3", "³")
                    .Replace("4", "⁴")
                    .Replace("5", "⁵")
                    .Replace("6", "⁶")
                    .Replace("7", "⁷")
                    .Replace("8", "⁸")
                    .Replace("9", "⁹");

                s = ss[0] + "·10" + s;
            }
            catch { }

            return s.PadRight(pad);
        }

        public IEnumerable<string> Comparisons
        {
            get
            {
                if (ComparedValue == null) return null;

                var res = ReferenceRecords.Select(i => GetComparisonValue(i) + " " + i.Name);

                return res;
            }
        }

        public ICommand Compare => new Command((val) =>
        {
            var v = (SingleResult)val;
            UseInt = v.Int;
            UseMultiThreaded = v.MultiThreaded;
            ComparedValue = v.Value;

            FS = FM = IS = IM = false;

            if (!UseInt && !UseMultiThreaded) FS = true;
            else if (!UseInt && UseMultiThreaded) FM = true;
            else if (UseInt && !UseMultiThreaded) IS = true;
            else if (UseInt && UseMultiThreaded) IM = true;
        });

        private bool fs = false;

        public bool FS
        {
            get { return fs; }
            set { if (value != fs) { fs = value; RaisePropertyChanged(nameof(FS)); } }
        }

        private bool fm = false;

        public bool FM
        {
            get { return fm; }
            set { if (value != fm) { fm = value; RaisePropertyChanged(nameof(FM)); } }
        }

        private bool _is = false;

        public bool IS
        {
            get { return _is; }
            set { if (value != _is) { _is = value; RaisePropertyChanged(nameof(IS)); } }
        }

        private bool im = false;

        public bool IM
        {
            get { return im; }
            set { if (value != im) { im = value; RaisePropertyChanged(nameof(IM)); } }
        }

    }
}
