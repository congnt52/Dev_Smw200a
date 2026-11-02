using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5GAutoTool.Reports
{
    public class ReportManager
    {
        //public string ReportFolder { get; private set; }

        private Report _report = new Report();

        public Report Report => _report;

        private TestCaseResult _currentTestCase;

        public TestCaseResult CurrentTestCase
        {
            get { return _currentTestCase; }
        }

        public void BeginReport(string serial, string productType, string operatingFrequency, string bandwidth, string power, string stationName)
        {
            Clear();

            _report.Summary.SerialNumber = serial;
            _report.Summary.ProductType = productType;
            _report.Summary.OperatingFrequency = operatingFrequency;
            _report.Summary.Bandwidth = bandwidth;
            _report.Summary.DeclarePower = power;
            _report.Summary.StationName = stationName;
            _report.Summary.StartTime = DateTime.Now;
        }

        public void EndReport()
        {
            _report.Summary.EndTime = DateTime.Now;

            bool fail = _report.TestCases.Any(x => x.Verdict == "FAIL");

            _report.Summary.OverallResult = fail ? "FAIL" : "PASS";
        }

        public void BeginTestCase(string testCaseName, List<string> headers)
        {
            _currentTestCase = new TestCaseResult
                {
                    Name = testCaseName,
                    Headers = headers,
                    StartTime = DateTime.Now,

                    // mặc định Fail
                    Verdict = "FAIL"
                };

            _report.TestCases.Add(_currentTestCase);
        }

        public void AddResult(TestRow testRow)
        {
            if (_currentTestCase == null)
                throw new Exception("BeginTestCase chưa được gọi.");

            _currentTestCase.Rows.Add(testRow);

            string result;

            if (testRow.Values.TryGetValue("PASS/FAIL", out result))
            {
                if (string.Equals(result, "PASS", StringComparison.OrdinalIgnoreCase))
                {
                    _currentTestCase.numPass++;
                }
                else
                {
                    _currentTestCase.numFail++;
                }
            }

            _currentTestCase.Verdict = (_currentTestCase.numFail > 0)? "FAIL" : "PASS";
        }

        public TestCaseResult EndTestCase()
        {
            if (_currentTestCase == null)
                return null;

            _currentTestCase.EndTime = DateTime.Now;
            TestCaseResult finishedTc = _currentTestCase;
            _currentTestCase = null;

            return finishedTc;
        }

        public void Clear()
        {
            _report = new Report();
            _currentTestCase = null;
        }

        //public string GetReportFileName(string extension)
        //{
        //    return string.Format(
        //        "{0}_{1}_{2}_{3}.{4}",
        //        _report.Summary.SerialNumber,
        //        _report.Summary.StartTime.ToString("yyyyMMdd-HHmmss"),
        //        _report.Summary.EndTime.ToString("yyyyMMdd-HHmmss"),
        //        _report.Summary.OverallResult,
        //        extension.TrimStart('.'));
        //}

        //public string GetTestCaseFileName(TestCaseResult tc, string extension)
        //{
        //    string safeName = tc.Name;

        //    foreach (char c in Path.GetInvalidFileNameChars())
        //    {
        //        safeName = safeName.Replace(c, '_');
        //    }

        //    return string.Format(
        //        "{0}_{1}_{2}_{3}_{4}.{5}",
        //        _report.Summary.SerialNumber,
        //        safeName,
        //        tc.StartTime.ToString("yyyyMMdd_HHmmss"),
        //        tc.EndTime.ToString("yyyyMMdd_HHmmss"),
        //        tc.Verdict,
        //        extension.TrimStart('.'));
        //}

    }
    //==========================================
    // CÁC MODEL HỖ TRỢ CHO BÁO CÁO
    //==========================================
    public class Report
    {
        public ReportSummary Summary { get; set; }
            = new ReportSummary();

        public List<TestCaseResult> TestCases { get; set; }
            = new List<TestCaseResult>();

        public string ReportFolder { get; set; }
    }

    public class ReportSummary
    {
        public string SerialNumber { get; set; }

        public string ProductType { get; set; }
        public string OperatingFrequency { get; set; }

        public string Bandwidth { get; set; }
        public string DeclarePower { get; set; }

        public string Operator { get; set; }

        public string StationName { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime TactTime { get; set; }
        public string OverallResult { get; set; } = "FAIL";
    }

    public class TestCaseResult
    {
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string Verdict { get; set; } = "PASS";
        public int numPass = 0;
        public int numFail = 0;

        public List<string> Headers { get; set; }
            = new List<string>();

        public List<TestRow> Rows { get; set; }
            = new List<TestRow>();

        // Chart
        public List<string> XLabels { get; set; }
            = new List<string>();

        public List<ChartSeries> Series { get; set; }
            = new List<ChartSeries>();
    }

    public class TestRow
    {
        public Dictionary<string, string> Values { get; set; }
            = new Dictionary<string, string>();

        public void Clear()
        {
            Values.Clear();
        }
    }

    public class ChartSeries
    {
        public string Name { get; set; }

        public List<double> Values { get; set; }
            = new List<double>();
    }
}
