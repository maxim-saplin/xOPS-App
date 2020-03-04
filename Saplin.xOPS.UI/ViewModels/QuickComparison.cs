namespace Saplin.xOPS.UI.ViewModels
{
    public class QuickComparison : BaseViewModel
    {
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
            new ReferenceRecord() { Name = "1993 Cray 3 Supercomputer", GFlopsST = 16, GFlopsMT = 16, GInopsST = 16, GInopsMT = 16 },
            new ReferenceRecord() { Name = "1966 Appolo Guidance Computer", GFlopsST = 0.000085, GFlopsMT = 0.000085, GInopsST = 0.000085, GInopsMT = 0.000085 },
            new ReferenceRecord() { Name = "2019 Samsung Galaxy Note 10", GFlopsST = 6.1, GFlopsMT = 22.2, GInopsST = 6.97, GInopsMT = 28.11 },
            new ReferenceRecord() { Name = "2018 Core i7 MacBook Pro", GFlopsST = 9.75, GFlopsMT = 53.98, GInopsST = 8.1, GInopsMT = 49.7 },
        };
    }
}
