using System;

namespace TestWpfApplication
{
    public class DataItem
    {
        public string Severity { get; set; }

        public DateTime Date { get; set; }

        public int Thread { get; set; }

        public string Source { get; set; }

        public string Header { get; set; }

        public string Message { get; set; }
    }
}