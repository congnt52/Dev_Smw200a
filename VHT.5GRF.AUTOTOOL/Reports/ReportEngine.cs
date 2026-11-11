using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.Windows.Forms;

namespace _5GAutoTool.Reports
{
    /// <summary>
    /// Lớp ReportEngine chịu trách nhiệm xuất dữ liệu báo cáo ra các định dạng khác nhau như HTML, CSV, Excel.
    /// </summary>
    public class ReportEngine
    {
        private readonly string _rootReportFolder;

        public ReportEngine(string rootReportFolder)
        {
            _rootReportFolder = rootReportFolder;
        }

        //khai báo các key trong template Excel để mapping với dữ liệu
        private readonly HashSet<string> _reservedTags = new HashSet<string>()
            {
                "tc_name",
                "tc_specs",
                "tc_min",
                "tc_max",
                "tc_unit",
                "port",
                "meas",
                "verdict",
                "tc_description"
            };

        #region CÁC METHODS
        /// <summary>
        /// Tạo thư mục: rootReportFolder/yyyyMMdd/serialNumber
        /// </summary>
        public string CreateReportFolder(string serial, DateTime startTime)
        {
            
            string dateFolder = startTime.ToString("ddMMyyyy");

            string baseFolder = Path.Combine(_rootReportFolder, dateFolder, serial);
            string folder = baseFolder;
            int index = 1;

            while (Directory.Exists(folder))
            {
                folder = string.Format("{0}({1})", baseFolder, index);
                index++;
            }

            Directory.CreateDirectory(folder);

            return folder;
        }

        private string MakeSafeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }

            return name.Replace(" ", "_");
        }

        //public void ExportAll(Report report, string excelTemplatePath, ListView lvwLog)
        //{
        //    // Tạo tên cho file
        //    string filename = BuildReportFileName(report);

        //    ExportHtml(report.ReportFolder, filename + ".html", report);

        //    //ExportCsv(report.ReportFolder, filename + ".csv", report);

        //    ExportExcel(excelTemplatePath, report.ReportFolder, filename + ".xlsx", report);

        //    ExportLogTxt(report.ReportFolder, filename + ".txt", lvwLog);

        //    try
        //    {
        //        ExportListViewResult(
        //            report.ReportFolder,
        //            filename + ".xlsx",
        //            lvwResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Export ListView got an error! + {ex.Message}");
        //    }
        //}

        public void ExportAll(Report report, string excelTemplatePath, ListView lvwLog)
        {
            // Tạo tên cho file
            string filename = BuildReportFileName(report);

            ExportHtml(report.ReportFolder, filename + ".html", report);

            // Xuất CSV tổng hợp
            ExportCsv(report.ReportFolder, filename + ".csv", report);

            ExportExcel(excelTemplatePath, report.ReportFolder, filename + ".xlsx", report);

            ExportLogTxt(report.ReportFolder, filename + ".txt", lvwLog);
        }


        public string BuildReportFileName(Report report)
        {
            return string.Format(
                "{0}_{1}_{2}_{3}",
                report.Summary.SerialNumber,
                report.Summary.StartTime.ToString("yyyyMMdd-HHmmss"),
                report.Summary.EndTime.ToString("yyyyMMdd-HHmmss"),
                report.Summary.OverallResult
                );
        }

        #endregion

        #region HÀM XUẤT HTML
        // =========================================================================
        // HÀM XUẤT HTML - không sửa
        // =========================================================================
        public void ExportHtml(string reportFolder, string fileName, Report report)
        {
            StringBuilder html = new StringBuilder();

            html.Append(@"
            <!DOCTYPE html>
            <html>
            <head>
            <meta charset='UTF-8'>
            <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
            <style>
            body {
                font-family: Arial, sans-serif;
                font-size: 14px;
                margin: 20px;
            }

            h2 {
                background-color: #f2f2f2;
                padding: 8px;
                border: 1px solid #ccc;
                cursor: pointer;
                margin-top: 25px;
            }

            table {
                border-collapse: collapse;
                width: 100%;
                margin-bottom: 15px;
            }

            th, td {
                border: 1px solid #ccc;
                padding: 6px 10px;
                text-align: center;
            }

            th {
                background-color: #e2e2e2;
            }

            tr:nth-child(even) {
                background-color: #f9f9f9;
            }

            .info-table {
                width: auto;
                min-width: 500px;
                display: inline-table;
            }

            .info-table th {
                text-align: left;
                padding-right: 20px;
            }

            .pass-text {
                color: green;
                font-weight: bold;
            }

            .fail-text {
                color: red;
                font-weight: bold;
            }
            </style>

            <script>
            function toggleContent(id)
            {
                var obj = document.getElementById(id);

                if(obj.style.display === 'none')
                    obj.style.display = 'block';
                else
                    obj.style.display = 'none';
            }
            </script>

            </head>
            <body>
            ");

            //------------------------------------
            // SUMMARY
            //------------------------------------

            var s = report.Summary;

            html.Append($"<h2>THÔNG TIN ĐO KIỂM: {s.SerialNumber}</h2>");

            html.Append("<table class='info-table'>");

            html.Append($"<tr><th>Serial Number</th><td>{s.SerialNumber}</td></tr>");
            html.Append($"<tr><th>Product Type</th><td>{s.ProductType}</td></tr>");
            html.Append($"<tr><th>Operating Frequency</th><td>{s.OperatingFrequency}</td></tr>");
            html.Append($"<tr><th>Bandwidth</th><td>{s.Bandwidth}</td></tr>");
            html.Append($"<tr><th>Power</th><td>{s.DeclarePower}</td></tr>");            
            html.Append($"<tr><th>Station</th><td>{s.StationName}</td></tr>");
            html.Append($"<tr><th>Start Time</th><td>{s.StartTime:yyyy-MM-dd HH:mm:ss}</td></tr>");
            html.Append($"<tr><th>End Time</th><td>{s.EndTime:yyyy-MM-dd HH:mm:ss}</td></tr>");
            html.Append($"<tr><th>Operator</th><td>{s.Operator}</td></tr>");

            string resultClass =
                s.OverallResult == "PASS"
                ? "pass-text"
                : "fail-text";

            html.Append(
                $"<tr><th>Overall Result</th><td class='{resultClass}'>{s.OverallResult}</td></tr>");

            html.Append("</table>");

            //------------------------------------
            // TEST CASES
            //------------------------------------

            int counter = 0;

            foreach (var tc in report.TestCases)
            {
                counter++;

                //==== Đoạn này chỉ hiện Pass/Fail =====
                //string verdictClass =
                //    tc.Verdict == "PASS"
                //    ? "pass-text"
                //    : "fail-text";

                //html.Append($@"<h2 onclick=""toggleContent('tc_{counter}')"">{tc.Name}<span class='{verdictClass}'>[{tc.Verdict}]</span></h2>");
                //==== Đoạn này hiện Pass=..., Fail = ...
                int totalPF = tc.numPass + tc.numFail;
                string passConv = (tc.numPass > 0)? $"Pass = {tc.numPass}/{totalPF} " : "";
                string failConv = (tc.numFail > 0) ? $"Fail = {tc.numFail}/{totalPF} " : "";

                html.Append($@"<h2 onclick=""toggleContent('tc_{counter}')"">{tc.Name} <span class='pass-text'>{passConv}</span> <span class='fail-text'>{failConv}</span></h2>");
                //------------------------------------
                // CHUẨN BỊ DỮ LIỆU CHART
                //------------------------------------

                List<string> labels = new List<string>();

                foreach (var row in tc.Rows)
                {
                    string port = "";

                    if (row.Values.ContainsKey("PORT"))
                        port = row.Values["PORT"];

                    labels.Add(port);
                }

                StringBuilder datasets = new StringBuilder();

                string[] chartColumns =
                {
                    "LOW LIMIT",
                    "HIGH LIMIT",
                    "RESULT"
                };

               
                List<string> chartDatasets = new List<string>();

                // ===================================================
                // DATASET KẾT QUẢ ĐO
                // ===================================================
                foreach (string column in chartColumns)
                {
                    List<double> values =
                        new List<double>();

                    bool validColumn = true;

                    foreach (var row in tc.Rows)
                    {
                        string text = "";

                        if (row.Values.ContainsKey(column))
                            text = row.Values[column];

                        double number;

                        if (!TryExtractNumber(text, out number))
                        {
                            validColumn = false;
                            break;
                        }

                        values.Add(number);
                    }

                    if (!validColumn)
                        continue;

                    chartDatasets.Add($@"
                    {{
                        label:'{column}',
                        data:{JsonConvert.SerializeObject(values)},
                        borderColor:'blue',
                        fill:false,
                        tension:0.1
                    }}");
                }

                // ===================================================
                // LOW LIMIT
                // ===================================================
                List<double> lowLimitValues =
                    new List<double>();

                bool hasLowLimit = true;

                foreach (var row in tc.Rows)
                {
                    string text = "";

                    if (row.Values.ContainsKey("MIN"))
                        text = row.Values["MIN"];

                    double number;

                    if (!TryExtractNumber(text, out number))
                    {
                        hasLowLimit = false;
                        break;
                    }

                    lowLimitValues.Add(number);
                }

                if (hasLowLimit)
                {
                    chartDatasets.Add($@"
                    {{
                        label:'LimitMin',
                        data:{JsonConvert.SerializeObject(lowLimitValues)},
                        borderColor:'red',
                        borderDash:[8,4],
                        pointRadius:0,
                        borderWidth:2,
                        fill:false
                    }}");
                }

                // ===================================================
                // HIGH LIMIT
                // ===================================================
                List<double> highLimitValues =
                    new List<double>();

                bool hasHighLimit = true;

                foreach (var row in tc.Rows)
                {
                    string text = "";

                    if (row.Values.ContainsKey("MAX"))
                        text = row.Values["MAX"];

                    double number;

                    if (!TryExtractNumber(text, out number))
                    {
                        hasHighLimit = false;
                        break;
                    }

                    highLimitValues.Add(number);
                }

                if (hasHighLimit)
                {
                    chartDatasets.Add($@"
                    {{
                        label:'LimitMax',
                        data:{JsonConvert.SerializeObject(highLimitValues)},
                        borderColor:'red',
                        borderDash:[8,4],
                        pointRadius:0,
                        borderWidth:2,
                        fill:false
                    }}");
                }

                // ===================================================
                // RENDER CHART
                // ===================================================
                if (chartDatasets.Count > 0)
                {
                    string labelsJson =
                        JsonConvert.SerializeObject(labels);

                    string datasetsJson =
                        string.Join(",",
                            chartDatasets);

                    html.Append($@"
                    <div style='width:900px;height:250px;margin-bottom:10px;'>
                        <canvas id='chart_{counter}'></canvas>
                    </div>

                    <script>

                    new Chart(
                        document.getElementById('chart_{counter}'),
                        {{
                            type:'line',

                            data:
                            {{
                                labels:{labelsJson},

                                datasets:[
                                    {datasetsJson}
                                ]
                            }},

                            options:
                            {{
                                responsive:true,
                                maintainAspectRatio:false,

                                interaction:
                                {{
                                    intersect:false,
                                    mode:'index'
                                }},

                                plugins:
                                {{
                                    title:
                                    {{
                                        display:true,
                                        text:'{tc.Name}'
                                    }},

                                    legend:
                                    {{
                                        position:'top'
                                    }}
                                }},

                                scales:
                                {{
                                    y:
                                    {{
                                        beginAtZero:false
                                    }}
                                }}
                            }}
                        }}
                    );

                    </script>");
                }

                //------------------------------------
                // TABLE
                //------------------------------------

                html.Append("<table>");

                html.Append("<tr>");

                foreach (var header in tc.Headers)
                {
                    html.Append($"<th>{header}</th>");
                }

                html.Append("</tr>");

                foreach (var row in tc.Rows)
                {
                    html.Append("<tr>");

                    foreach (var header in tc.Headers)
                    {
                        string value = "";

                        row.Values.TryGetValue(
                            header,
                            out value);

                        value = value ?? "";

                        string cssClass = "";

                        if (value.Equals(
                            "PASS",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            cssClass = "pass-text";
                        }
                        else if (value.Equals(
                            "FAIL",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            cssClass = "fail-text";
                        }

                        html.Append(
                            $"<td class='{cssClass}'>{value}</td>");
                    }

                    html.Append("</tr>");
                }

                html.Append("</table>");

                //------------------------------------
                // END CONTAINER
                //------------------------------------

                html.Append("</div>");
            }

            html.Append(@"</body></html>");
           // string reportFolder = "@C:\Reports";
            string filePath = Path.Combine(reportFolder, fileName);

            File.WriteAllText(filePath, html.ToString(), Encoding.UTF8);
        }
        #endregion

        #region HÀM XUẤT CSV
        // =========================================================================
        // HÀM XUẤT CSV - không sửa
        // =========================================================================
        public void ExportCsv(string reportFolder, string fileName, Report report)
        {
            if (report == null)
                return;

            StringBuilder sb = new StringBuilder();

            foreach (var tc in report.TestCases)
            {
                //----------------------------------
                // TÊN TESTCASE
                //----------------------------------

                sb.AppendLine(tc.Name);

                //----------------------------------
                // HEADER
                //----------------------------------

                sb.AppendLine(string.Join(",", tc.Headers.Select(h => $"\"{h}\"")));

                //----------------------------------
                // DATA
                //----------------------------------

                foreach (var row in tc.Rows)
                {
                    List<string> values = new List<string>();

                    foreach (string header in tc.Headers)
                    {
                        string value = "";

                        row.Values.TryGetValue(header, out value);

                        value = value ?? "";
                        value = value.Replace("\"","\"\"");

                        values.Add($"\"{value}\"");
                    }

                    sb.AppendLine(string.Join(",", values));
                }

                sb.AppendLine();
            }

            string filePath = Path.Combine(reportFolder, fileName);

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }
        
        // Hàm hỗ trợ
        public CsvSession BeginCsvTestCase(string reportFolder, string serial, string testCaseName, List<string> headers)
        {
            string safeName = MakeSafeFileName(testCaseName);

            string fileName = string.Format("{0}_{1}_RUNNING.csv", serial, safeName);

            string filePath = Path.Combine(reportFolder, fileName);

            StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8);

            writer.WriteLine(string.Join(",", headers));

            writer.Flush();

            return new CsvSession {TempFilePath = filePath, Writer = writer };
        }

        public void AppendCsvRow(CsvSession session, List<string> headers, Dictionary<string, string> values)
        {
            if (session == null)
                return;

            List<string> row = new List<string>();

            foreach (string header in headers)
            {
                string value = "";

                values.TryGetValue(header, out value);
                value = value ?? "";
                value = value.Replace("\"", "\"\"");
                row.Add("\"" + value + "\"");
            }

            session.Writer.WriteLine(string.Join(",", row));
            session.Writer.Flush();
        }

        public void EndCsvTestCase(CsvSession session, string reportFolder, string serial, TestCaseResult tc)
        {
            if (session == null)
                return;

            session.Writer.Flush();
            session.Writer.Close();
            session.Writer.Dispose();

            string safeName = MakeSafeFileName(tc.Name);

            string newFileName = string.Format(
                    "{0}_{1}_{2}_{3}_{4}.csv",
                    serial,
                    safeName,
                    tc.StartTime.ToString("yyyyMMdd_HHmmss"),
                    tc.EndTime.ToString("yyyyMMdd_HHmmss"),
                    tc.Verdict);

            string newPath = Path.Combine(reportFolder, newFileName);

            if (File.Exists(session.TempFilePath))
            {
                File.Move(session.TempFilePath, newPath);
            }
        }
        #endregion

        #region HÀM XUẤT EXCEL THEO TEMPLATE
        // =========================================================================
        // HÀM XUẤT EXCEL
        // =========================================================================
        public void ExportExcel(string templateFile, string reportFolder, string filename, Report report)
        {
            using (var wb = new XLWorkbook(templateFile))
            {
                var ws = wb.Worksheet(1);

                int templateRow = FindTemplateRow(ws);

                if (templateRow < 0)
                    throw new Exception("Không tìm thấy <$data_from_here$>");

                var colMap = BuildColumnMap(ws, templateRow);

                FillTestCaseRows(ws, templateRow, colMap, report);

                ReplaceAllTags(ws, report);

                //tạo file path
                string outputFile = Path.Combine(reportFolder, filename);

                wb.SaveAs(outputFile);
            }
        }
        // Hàm hỗ trợ
        private void ReplaceAllTags(IXLWorksheet ws, Report report)
        {
            var context = BuildContext(report);
            Regex regex = new Regex(@"<([^>]+)>");

            foreach (var cell in ws.CellsUsed())
            {
                string text = cell.GetString();

                if (string.IsNullOrEmpty(text))
                    continue;

                string newText = regex.Replace(text, m =>
                {
                    string key = m.Groups[1].Value;
                    if (_reservedTags.Contains(key)) return m.Value;

                    string value;

                    if (context.TryGetValue(key, out value))
                    {
                        return value;
                    }
                    return "";
                });

                cell.DataType = XLDataType.Text;
                cell.Value = newText;
            }
        }
        private Dictionary<string, string> BuildContext(Report report)
        {
            var ctx = new Dictionary<string, string>();
            ctx["dut_serial"] = report.Summary.SerialNumber;
            ctx["dut_type"] = report.Summary.ProductType;
            ctx["dut_bw"] = report.Summary.Bandwidth;
            ctx["dut_freq"] = report.Summary.OperatingFrequency;
            ctx["sys_testplan_name"] = report.Summary.StationName;
            ctx["sys_tester"] = report.Summary.Operator;
            ctx["sys_testplan_start_time"] = "'" + report.Summary.StartTime.ToString("dd/MM/yyyy HH:mm:ss");
            ctx["sys_testplan_stop_time"] = "'" + report.Summary.EndTime.ToString("dd/MM/yyyy HH:mm:ss");
            //ctx["sys_tact_time"] = string.Format("{0:hh\\:mm\\:ss}", report.Summary.EndTime);
            ctx["overall_result"] = report.Summary.OverallResult;

            return ctx;
        }

        private bool TryExtractNumber(string text, out double value)
        {
            value = 0;

            if (string.IsNullOrWhiteSpace(text))
                return false;

            Match m = Regex.Match(text, @"-?\d+(\.\d+)?");

            if (!m.Success)
                return false;

            return double.TryParse(
                m.Value,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out value);
        }

        private static string GetValue(Dictionary<string, string> values, string key)
        {
            string value;

            if (values.TryGetValue(key, out value))
                return value;

            return "";
        }

        private int FindTemplateRow(IXLWorksheet ws)
        {
            foreach (var cell in ws.CellsUsed())
            {
                if (cell.GetString().Trim() == "<$data_from_here$>")
                {
                    return cell.Address.RowNumber;
                }
            }

            return -1;
        }

        private Dictionary<string, int> BuildColumnMap(IXLWorksheet ws, int templateRow)
        {
            var map = new Dictionary<string, int>();

            int lastCol = ws.LastColumnUsed().ColumnNumber();

            for (int c = 1; c <= lastCol; c++)
            {
                string text = ws.Cell(templateRow, c).GetString().Trim();

                if (string.IsNullOrWhiteSpace(text))
                    continue;

                map[text] = c;
            }

            return map;
        }

        private void FillTestCaseRows(IXLWorksheet ws, int templateRow, Dictionary<string, int> colMap, Report report)
        {
            int currentRow = templateRow + 1;
            int stt = 1;

            foreach (var tc in report.TestCases)
            {
                foreach (var row in tc.Rows)
                {
                    ws.Row(currentRow).InsertRowsAbove(1);
                    CopyRowStyle(ws, templateRow, currentRow);
                    ws.Cell(currentRow, 1).Value = stt++;

                    SetValue(ws, currentRow, colMap, "<tc_name>", GetValue(row.Values, "CLAUSE"));
                    SetValue(ws, currentRow, colMap, "<tc_unit>", GetValue(row.Values, "UNIT"));
                    SetValue(ws, currentRow, colMap, "<tc_config>", GetValue(row.Values, "CONFIG"));
                    SetValue(ws, currentRow, colMap, "<tc_param>", GetValue(row.Values, "PARAM"));
                    SetValue(ws, currentRow, colMap, "<tc_specs>", GetValue(row.Values, "SPECS"));
                    //SetValue(ws, currentRow, colMap, "<tc_min>", GetValue(row.Values, "MIN"));
                    //SetValue(ws, currentRow, colMap, "<tc_max>", GetValue(row.Values, "MAX"));
                    //SetValue(ws, currentRow, colMap, "<port>", "PORT " + GetValue(row.Values, "PORT"));
                    string port = GetValue(row.Values, "PORT");

                    SetValue(
                        ws,
                        currentRow,
                        colMap,
                        "<port>",
                        port.StartsWith("PORT", StringComparison.OrdinalIgnoreCase)
                            ? port
                            : "PORT " + port);
                    SetValue(ws, currentRow, colMap, "<meas>", GetValue(row.Values, "RESULT"));
                    SetValue(ws, currentRow, colMap, "<tc_description>", GetValue(row.Values, "DESC"));

                    string verdict = GetValue(row.Values, "PASS/FAIL");
                    SetValue(ws, currentRow, colMap, "<verdict>", verdict);
                    var verdictCell = ws.Cell(currentRow, colMap["<verdict>"]);

                    if (verdict == "PASS")
                    {
                        verdictCell.Style.Font.FontColor = XLColor.Green;
                    }
                    else
                    {
                        verdictCell.Style.Font.FontColor = XLColor.Red;
                    }

                    currentRow++;
                }
            }

            ws.Row(templateRow).Delete();
        }

        private void SetValue(IXLWorksheet ws, int row, Dictionary<string, int> map, string tag, string value)
        {
            int col;

            if (!map.TryGetValue(tag, out col))
                return;

            ws.Cell(row, col).Value = value ?? "";
        }

        private void CopyRowStyle(IXLWorksheet ws, int sourceRow, int targetRow)
        {
            int lastCol = ws.LastColumnUsed().ColumnNumber();

            for (int c = 1; c <= lastCol; c++)
            {
                ws.Cell(targetRow, c).Style = ws.Cell(sourceRow, c).Style;
            }

            ws.Row(targetRow).Height = ws.Row(sourceRow).Height;
        }
        #endregion

        #region HÀM XUẤT LOG ĐO
        // =========================================================================
        // HÀM XUẤT TXT
        // =========================================================================
        public void ExportLogTxt(string folder, string fileName, ListView log)
        {
            try
            {
                string outputFile = Path.Combine(folder, fileName);

                using (StreamWriter writer = new StreamWriter(outputFile))
                {

                    foreach (ListViewItem item in log.Items)
                    {
                        writer.Write(item.Text);
                        for (int i = 1; i < log.Columns.Count; i++)
                        {
                            writer.Write("\t" + item.SubItems[i].Text);
                        }
                        writer.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xuất log: " + ex.Message);
            }

        }

        #endregion
    }

    //=========================================================================
    // CÁC LỚP HỖ TRỢ
    //=========================================================================
    public class CsvSession
    {
        public string TempFilePath { get; set; }

        public StreamWriter Writer { get; set; }

        public string TestCaseName { get; set; }
    }
}
