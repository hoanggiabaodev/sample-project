using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Web_ProjectName.Lib;
using ClosedXML.Excel;
using System.Threading;
using System.Text.Json;

namespace Web_ProjectName.Controllers
{
    public class TableController : BaseController<TableController>
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TableController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetList()
        {
            var data = new List<object>
        {
            new { Lo = "N15", NamTrong = 2016, Giong = "RRIV 209", DienTich = 18.91862, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "ABC" },
            new { Lo = "P15", NamTrong = 2017, Giong = "RRIV 106", DienTich = 11.85678, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "XYZ" },
            new { Lo = "Q15", NamTrong = 2018, Giong = "PB 235", DienTich = 9.45111, GayNgang = 1, BatGoc = 0, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "APC" },
            new { Lo = "L8", NamTrong = 2015, Giong = "RRIV 124", DienTich = 12.765, GayNgang = 0, BatGoc = 0, SetDanh = 1, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "APZ" },
            new { Lo = "M12", NamTrong = 2019, Giong = "RRIV 209", DienTich = 15.327, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 1, GhiChu = "APD" },
            new { Lo = "T7", NamTrong = 2014, Giong = "PB 235", DienTich = 10.854, GayNgang = 0, BatGoc = 0, SetDanh = 0, Cong = 1, Nhanh = 1, TanCay = 0, GhiChu = "ASD" },
            new { Lo = "R10", NamTrong = 2020, Giong = "RRIV 106", DienTich = 14.129, GayNgang = 1, BatGoc = 0, SetDanh = 0, Cong = 0, Nhanh = 0, TanCay = 1, GhiChu = "ADA" },
            new { Lo = "S5", NamTrong = 2013, Giong = "RRIV 209", DienTich = 13.777, GayNgang = 0, BatGoc = 0, SetDanh = 1, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "FAS" },
            new { Lo = "U2", NamTrong = 2021, Giong = "RRIV 124", DienTich = 16.908, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "QWS" },
            new { Lo = "V9", NamTrong = 2012, Giong = "PB 235", DienTich = 11.542, GayNgang = 0, BatGoc = 0, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 1, GhiChu = "SDA" },

            new { Lo = "W3", NamTrong = 2022, Giong = "RRIV 209", DienTich = 17.234, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "WXY" },
            new { Lo = "X4", NamTrong = 2011, Giong = "RRIV 106", DienTich = 13.456, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "YZA" },
            new { Lo = "Y5", NamTrong = 2023, Giong = "PB 235", DienTich = 10.123, GayNgang = 1, BatGoc = 0, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "ZBC" },
            new { Lo = "Z6", NamTrong = 2010, Giong = "RRIV 124", DienTich = 14.789, GayNgang = 0, BatGoc = 0, SetDanh = 1, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "CDE" },
            new { Lo = "AA7", NamTrong = 2024, Giong = "RRIV 209", DienTich = 16.101, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 1, GhiChu = "DEF" },
            new { Lo = "BB8", NamTrong = 2009, Giong = "PB 235", DienTich = 11.987, GayNgang = 0, BatGoc = 0, SetDanh = 0, Cong = 1, Nhanh = 1, TanCay = 0, GhiChu = "EFG" },
            new { Lo = "CC9", NamTrong = 2025, Giong = "RRIV 106", DienTich = 15.234, GayNgang = 1, BatGoc = 0, SetDanh = 0, Cong = 0, Nhanh = 0, TanCay = 1, GhiChu = "FGH" },
            new { Lo = "DD10", NamTrong = 2008, Giong = "RRIV 209", DienTich = 13.456, GayNgang = 0, BatGoc = 0, SetDanh = 1, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "GHI" },
            new { Lo = "EE11", NamTrong = 2026, Giong = "RRIV 124", DienTich = 17.890, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "HIJ" },
            new { Lo = "FF12", NamTrong = 2007, Giong = "PB 235", DienTich = 12.345, GayNgang = 0, BatGoc = 0, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 1, GhiChu = "IJK" },
            new { Lo = "GG13", NamTrong = 2027, Giong = "RRIV 209", DienTich = 18.567, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "JKL" },
            new { Lo = "HH14", NamTrong = 2006, Giong = "RRIV 106", DienTich = 11.234, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "KLM" },
            new { Lo = "II15", NamTrong = 2028, Giong = "PB 235", DienTich = 9.876, GayNgang = 1, BatGoc = 0, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "LMN" },
            new { Lo = "JJ16", NamTrong = 2005, Giong = "RRIV 124", DienTich = 13.678, GayNgang = 0, BatGoc = 0, SetDanh = 1, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "MNO" },
            new { Lo = "KK17", NamTrong = 2029, Giong = "RRIV 209", DienTich = 16.345, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 1, GhiChu = "NOP" },
            new { Lo = "LL18", NamTrong = 2004, Giong = "PB 235", DienTich = 10.987, GayNgang = 0, BatGoc = 0, SetDanh = 0, Cong = 1, Nhanh = 1, TanCay = 0, GhiChu = "OPQ" },
            new { Lo = "MM19", NamTrong = 2030, Giong = "RRIV 106", DienTich = 15.678, GayNgang = 1, BatGoc = 0, SetDanh = 0, Cong = 0, Nhanh = 0, TanCay = 1, GhiChu = "PQR" },
            new { Lo = "NN20", NamTrong = 2003, Giong = "RRIV 209", DienTich = 14.234, GayNgang = 0, BatGoc = 0, SetDanh = 1, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "QRS" },
            new { Lo = "OO21", NamTrong = 2031, Giong = "RRIV 124", DienTich = 17.890, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "RST" },
            new { Lo = "PP22", NamTrong = 2002, Giong = "PB 235", DienTich = 12.456, GayNgang = 0, BatGoc = 0, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 1, GhiChu = "STU" },
            new { Lo = "QQ23", NamTrong = 2032, Giong = "RRIV 209", DienTich = 19.012, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "TUV" },
            new { Lo = "RR24", NamTrong = 2001, Giong = "RRIV 106", DienTich = 11.789, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "UVW" },
            new { Lo = "SS25", NamTrong = 2033, Giong = "PB 235", DienTich = 9.345, GayNgang = 1, BatGoc = 0, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "VWX" },
            new { Lo = "TT26", NamTrong = 2000, Giong = "RRIV 124", DienTich = 14.567, GayNgang = 0, BatGoc = 0, SetDanh = 1, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "WXY" },
            new { Lo = "UU27", NamTrong = 2034, Giong = "RRIV 209", DienTich = 16.789, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 1, GhiChu = "XYZ" },
            new { Lo = "VV28", NamTrong = 1999, Giong = "RRIV 106", DienTich = 13.123, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "YZA" },
            new { Lo = "WW29", NamTrong = 2035, Giong = "PB 235", DienTich = 10.678, GayNgang = 1, BatGoc = 0, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "ZAB" },
            new { Lo = "XX30", NamTrong = 1998, Giong = "RRIV 124", DienTich = 15.234, GayNgang = 0, BatGoc = 0, SetDanh = 1, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "ABC" },
            new { Lo = "YY31", NamTrong = 2036, Giong = "RRIV 209", DienTich = 18.456, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "BCD" },
            new { Lo = "ZZ32", NamTrong = 1997, Giong = "RRIV 106", DienTich = 12.345, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "CDE" },
            new { Lo = "AAA33", NamTrong = 2037, Giong = "PB 235", DienTich = 9.876, GayNgang = 1, BatGoc = 0, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "DEF" },
            new { Lo = "BBB34", NamTrong = 1996, Giong = "RRIV 124", DienTich = 14.678, GayNgang = 0, BatGoc = 0, SetDanh = 1, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "EFG" },
            new { Lo = "CCC35", NamTrong = 2038, Giong = "RRIV 209", DienTich = 17.234, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 1, GhiChu = "FGH" },
            new { Lo = "DDD36", NamTrong = 1995, Giong = "RRIV 106", DienTich = 11.890, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "GHI" },
            new { Lo = "EEE37", NamTrong = 2039, Giong = "RRIV 124", DienTich = 16.567, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "HIJ" },
            new { Lo = "FFF38", NamTrong = 1994, Giong = "PB 235", DienTich = 10.234, GayNgang = 0, BatGoc = 0, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 1, GhiChu = "IJK" },
            new { Lo = "GGG39", NamTrong = 2040, Giong = "RRIV 209", DienTich = 19.012, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "JKL" },
            new { Lo = "HHH40", NamTrong = 1993, Giong = "RRIV 106", DienTich = 13.456, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "KLM" },
            new { Lo = "III41", NamTrong = 2041, Giong = "PB 235", DienTich = 9.678, GayNgang = 1, BatGoc = 0, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "LMN" },
            new { Lo = "JJJ42", NamTrong = 1992, Giong = "RRIV 124", DienTich = 15.234, GayNgang = 0, BatGoc = 0, SetDanh = 1, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "MNO" },
            new { Lo = "KKK43", NamTrong = 2042, Giong = "RRIV 209", DienTich = 18.345, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 1, GhiChu = "NOP" },
            new { Lo = "LLL44", NamTrong = 1991, Giong = "RRIV 106", DienTich = 12.678, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "OPQ" },
            new { Lo = "MMM45", NamTrong = 2043, Giong = "PB 235", DienTich = 9.234, GayNgang = 1, BatGoc = 0, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "PQR" },
            new { Lo = "NNN46", NamTrong = 1990, Giong = "RRIV 124", DienTich = 14.890, GayNgang = 0, BatGoc = 0, SetDanh = 1, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "QRS" },
            new { Lo = "OOO47", NamTrong = 2044, Giong = "RRIV 209", DienTich = 17.678, GayNgang = 0, BatGoc = 1, SetDanh = 0, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "RST" },
            new { Lo = "PPP48", NamTrong = 1989, Giong = "RRIV 106", DienTich = 11.234, GayNgang = 0, BatGoc = 0, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 1, GhiChu = "STU" },
            new { Lo = "QQQ49", NamTrong = 2045, Giong = "PB 235", DienTich = 9.890, GayNgang = 1, BatGoc = 0, SetDanh = 0, Cong = 1, Nhanh = 0, TanCay = 0, GhiChu = "TUV" },
            new { Lo = "RRR50", NamTrong = 1988, Giong = "RRIV 124", DienTich = 15.456, GayNgang = 0, BatGoc = 0, SetDanh = 1, Cong = 0, Nhanh = 1, TanCay = 0, GhiChu = "UVW" }
        };

            return Json(new { result = 1, data });
        }


        [HttpGet]
        public IActionResult GetYears()
        {
            var data = new List<int> { 2015, 2016, 2017, 2018, 2019, 2020 };
            return Json(new { result = 1, data });
        }

        [HttpGet]
        public IActionResult GetVarieties()
        {
            var data = new List<string> { "RRIV 209", "RRIV 106", "PB 235", "RRIV 124" };
            return Json(new { result = 1, data });
        }

        [HttpGet]
        public IActionResult ImportExcel(int? year, string? variety, string? ids)
        {
            ViewBag.Year = year;
            ViewBag.Variety = variety;
            ViewBag.Ids = ids;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ImportExcel(IFormFile? file)
        {
            M_JResult jResult = new M_JResult();
            try
            {
                if (file == null || file.Length == 0)
                {
                    jResult.result = 0;
                    jResult.error = new error(0, "Vui lòng chọn file cần import.");
                    return Json(jResult);
                }

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (ext != ".xlsx" && ext != ".xls")
                {
                    jResult.result = 0;
                    jResult.error = new error(0, "Định dạng file không hỗ trợ. Vui lòng chọn .xlsx hoặc .xls.");
                    return Json(jResult);
                }

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                var data = new List<object>();
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var sheetNamesToRead = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase) { "1.TM-TC", "1.KTCB", "1.KD" };

                        do
                        {
                            if (!sheetNamesToRead.Contains(reader.Name)) continue;

                            int rowStart = -1;

                            while (reader.Read())
                            {
                                if (rowStart == -1)
                                {
                                    var colA = reader.GetValue(0)?.ToString();
                                    var colB = reader.GetValue(1)?.ToString();

                                    if (colA == "1" && !string.IsNullOrWhiteSpace(colB) && !int.TryParse(colB, out _))
                                    {
                                        rowStart = reader.Depth;
                                    }
                                }

                                if (rowStart != -1 && reader.Depth >= rowStart)
                                {
                                    bool rowIsEmpty = true;
                                    for (int c = 0; c < reader.FieldCount; c++)
                                    {
                                        var v = reader.GetValue(c);
                                        if (v != null && !string.IsNullOrWhiteSpace(v.ToString()))
                                        {
                                            rowIsEmpty = false;
                                            break;
                                        }
                                    }
                                    if (rowIsEmpty) continue;

                                    object? GetCell(int idx) => idx < reader.FieldCount ? reader.GetValue(idx) : null;

                                    var idPrivate = GetCell(1)?.ToString();
                                    if (string.IsNullOrWhiteSpace(idPrivate)) continue;

                                    var colD = GetCell(3)?.ToString();
                                    if (string.IsNullOrWhiteSpace(colD)) continue;

                                    double TryDouble(object? v) => double.TryParse(v?.ToString(), out var d) ? d : 0d;
                                    int TryInt(object? v) => int.TryParse(v?.ToString(), out var i) ? i : 0;
                                    DateTime? TryDate(object? v)
                                    {
                                        if (v is DateTime dt) return dt;
                                        var s = v?.ToString();
                                        if (string.IsNullOrWhiteSpace(s)) return null;
                                        if (double.TryParse(s, out var oa))
                                        {
                                            try { return DateTime.FromOADate(oa); } catch { }
                                        }
                                        return DateTime.TryParse(s, out var parsed) ? parsed : null;
                                    }
                                    bool? TryBoolX(object? v)
                                    {
                                        var s = v?.ToString()?.Trim();
                                        if (string.IsNullOrEmpty(s)) return false;

                                        if (string.Equals(s, "x", StringComparison.OrdinalIgnoreCase))
                                            return true;

                                        return false;
                                    }

                                    if (reader.Name.Equals("1.TM-TC", StringComparison.OrdinalIgnoreCase) || reader.Name.Equals("1.TC-TM", StringComparison.OrdinalIgnoreCase))
                                    {
                                        data.Add(new
                                        {
                                            // Fields read from Excel (ordered by column index)
                                            IdPrivate = idPrivate,
                                            PlotName = GetCell(5)?.ToString() ?? "",
                                            LandLevelCode = GetCell(6)?.ToString() ?? "",
                                            AltitudeLowest = TryDouble(GetCell(7)),
                                            AltitudeHighest = TryDouble(GetCell(8)),
                                            PlantingMethodCode = GetCell(9)?.ToString() ?? "",
                                            PlantingDistanceCode = GetCell(10)?.ToString() ?? "",
                                            PlantingDesignDensity = TryDouble(GetCell(11)),
                                            TypeOfTreeCode = GetCell(12)?.ToString() ?? "",
                                            Area = TryDouble(GetCell(13)),
                                            HoleQuantity = TryInt(GetCell(14)),
                                            GraftedTreeCorrectQuantity = TryInt(GetCell(15)),
                                            GraftedTreeMixedQuantity = TryInt(GetCell(17)),
                                            EmptyHoleQuantity = TryInt(GetCell(19)),
                                            DensityOfGraftedTree = TryInt(GetCell(21)),
                                            AverageNumberLeafLayer = TryDouble(GetCell(22)),
                                            ClassifyCode = GetCell(23)?.ToString() ?? "",
                                            PlantingEndDate = TryDate(GetCell(24))?.ToString("yyyy-MM-dd") ?? GetCell(24)?.ToString() ?? "",
                                            Remark = GetCell(25)?.ToString() ?? "",

                                            // Defaulted/computed fields
                                            SurveyBatchId = 0,
                                            OwnerId = 0,
                                            CultivatorId = 0,
                                            PlotId = "",
                                            RiskLevel = "",
                                            PlotOldName = "",
                                            PlotNewName = "",
                                            LandType = "",
                                            AreaCultivated = 0.0f,
                                            Area1 = 0.0f,
                                            Area2 = 0.0f,
                                            Area3 = 0.0f,
                                            AreaManagementChange = 0.0f,
                                            AltitudeAverage = 0,
                                            ActiveStatusId = 14,
                                            Hecta = 0,
                                            ProductivityByArea = 0,
                                            TreeQuantity = 0,
                                            TreeQuantityShaving = 0,
                                            EffectiveTreeNotshavingQuantity = 0,
                                            EffectiveTreeShavingQuantity = 0,
                                            IneffectiveTreeNotgrowQuantity = 0,
                                            IneffectiveTreeDryQuantity = 0,
                                            ShavingTreeDensity = 0,
                                            ShavingModeCode = "",
                                            EndExploitationDate = "",
                                            StartExploitationDate = "",
                                            TotalShavingSlice = 0,
                                            ShavingFaceConditionId = 0,
                                            ShavingFaceConditionCode = "",
                                            TappingAge = 0,
                                            ProductivityByTree = 0,
                                            IneffectiveTreeQuantity = 0,
                                            EffectiveTreeCorrectQuantity = 0,
                                            EffectiveTreeMixedQuantity = 0,
                                            EffectiveTreeDensity = 0,
                                            StandardDeviation = 0,
                                            RatioTreeObtain = 0,
                                            MarkedExtendedGarden = false,
                                            ExpectedExploitationDate = "",
                                            YearOfPlanting = "",
                                            YearOfShaving = "",
                                            GardenRatingId = 0,
                                            VanhAverage = 0.0f,
                                            RootTreeMixedQuantity = 0,
                                            RootTreeCorrectQuantity = 0,
                                            Count = 0,
                                            TotalOutput = 0.0f
                                        });
                                    }
                                    else if (reader.Name.Equals("1.KTCB", StringComparison.OrdinalIgnoreCase))
                                    {
                                        data.Add(new
                                        {
                                            // Fields read from Excel (ordered by column index)
                                            IdPrivate = idPrivate,
                                            PlotOldName = GetCell(5)?.ToString() ?? "",
                                            PlotNewName = GetCell(6)?.ToString() ?? "",
                                            YearOfPlanting = GetCell(7)?.ToString() ?? "",
                                            LandLevelCode = GetCell(8)?.ToString() ?? "",
                                            AltitudeAverage = TryDouble(GetCell(9)),
                                            PlantingMethodCode = GetCell(10)?.ToString() ?? "",
                                            PlantingDistanceCode = GetCell(11)?.ToString() ?? "",
                                            PlantingDesignDensity = TryDouble(GetCell(12)),
                                            TypeOfTreeCode = GetCell(13)?.ToString() ?? "",
                                            Area = TryDouble(GetCell(15)),
                                            HoleQuantity = TryInt(GetCell(16)),
                                            EffectiveTreeCorrectQuantity = TryInt(GetCell(17)),
                                            EffectiveTreeMixedQuantity = TryInt(GetCell(19)),
                                            IneffectiveTreeNotgrowQuantity = TryInt(GetCell(21)),
                                            EmptyHoleQuantity = TryInt(GetCell(23)),
                                            EffectiveTreeDensity = TryInt(GetCell(25)),
                                            VanhAverage = TryDouble(GetCell(27)),
                                            StandardDeviation = TryDouble(GetCell(28)),
                                            RatioTreeObtain = TryDouble(GetCell(29)),
                                            MarkedExtendedGarden = TryBoolX(GetCell(30)),
                                            ExpectedExploitationDate = TryDate(GetCell(31))?.ToString("yyyy-MM-dd") ?? GetCell(31)?.ToString() ?? "",
                                            ClassifyCode = GetCell(32)?.ToString() ?? "",
                                            Remark = GetCell(33)?.ToString() ?? "",

                                            // Defaulted/computed fields
                                            SurveyBatchId = 0,
                                            OwnerId = 0,
                                            CultivatorId = 0,
                                            PlotId = "",
                                            RiskLevel = "",
                                            PlotName = "",
                                            LandType = "",
                                            AreaCultivated = 0.0f,
                                            Area1 = 0.0f,
                                            Area2 = 0.0f,
                                            Area3 = 0.0f,
                                            AltitudeLowest = 0.0f,
                                            AltitudeHighest = 0.0f,
                                            AreaManagementChange = 0.0f,
                                            ActiveStatusId = 5,
                                            Hecta = 0,
                                            DensityOfGraftedTree = 0,
                                            AverageNumberLeafLayer = 0,
                                            PlantingEndDate = "",
                                            ProductivityByArea = 0,
                                            TreeQuantity = 0,
                                            TreeQuantityShaving = 0,
                                            EffectiveTreeNotshavingQuantity = 0,
                                            EffectiveTreeShavingQuantity = 0,
                                            IneffectiveTreeDryQuantity = 0,
                                            ShavingTreeDensity = 0,
                                            ShavingModeCode = "",
                                            EndExploitationDate = "",
                                            StartExploitationDate = "",
                                            TotalShavingSlice = 0,
                                            ShavingFaceConditionId = 0,
                                            ShavingFaceConditionCode = "",
                                            TappingAge = 0,
                                            ProductivityByTree = 0,
                                            IneffectiveTreeQuantity = 0,
                                            GardenRatingId = 0,
                                            RootTreeMixedQuantity = 0,
                                            RootTreeCorrectQuantity = 0,
                                            Count = 0,
                                            TotalOutput = 0.0f
                                        });
                                    }
                                    else if (reader.Name.Equals("1.KD", StringComparison.OrdinalIgnoreCase))
                                    {
                                        data.Add(new
                                        {
                                            // Fields read from Excel (ordered by column index)
                                            IdPrivate = idPrivate,                       // Mã Lô
                                            PlotOldName = GetCell(5)?.ToString() ?? "",                            // Tên lô cũ
                                            PlotNewName = GetCell(6)?.ToString() ?? "",                            // Tên lô mới
                                            YearOfPlanting = GetCell(7)?.ToString() ?? "",                         // Năm trồng
                                            LandLevelCode = GetCell(8)?.ToString() ?? "",                          // Hạng đất (string)
                                            AltitudeAverage = TryDouble(GetCell(9)),       // Cao trình trung bình (m)
                                            PlantingMethodCode = GetCell(10)?.ToString() ?? "", // Phương pháp trồng (string)
                                            PlantingDistanceCode = GetCell(11)?.ToString() ?? "", // Khoảng cách trồng
                                            PlantingDesignDensity = TryDouble(GetCell(12)), // Mật độ thiết kế (cây/ha)
                                            TypeOfTreeCode = GetCell(13)?.ToString() ?? "", // Giống cây trồng (mã)
                                            Area = TryDouble(GetCell(15)),                                 // Diện tích (tổng)
                                            HoleQuantity = TryInt(GetCell(16)),          // Tổng số hố khảo sát
                                            EffectiveTreeShavingQuantity = TryInt(GetCell(17)),            // SL cây hữu hiệu đã cạo
                                            EffectiveTreeNotshavingQuantity = TryInt(GetCell(19)),         // SL cây hữu hiệu chưa cạo
                                            IneffectiveTreeDryQuantity = TryInt(GetCell(21)),              // SL cây khô mủ
                                            IneffectiveTreeNotgrowQuantity = TryInt(GetCell(23)),          // SL cây không phát triển
                                            EmptyHoleQuantity = TryInt(GetCell(25)),     // Số hố trống
                                            ShavingTreeDensity = TryInt(GetCell(27)),                      // Mật độ cây cạo
                                            ShavingModeCode = GetCell(28)?.ToString() ?? "",                        // Chế độ cạo
                                            StartExploitationDate = GetCell(29)?.ToString() ?? "",                  // Ngày bắt đầu khai thác
                                            TappingAge = TryInt(GetCell(30)),                              // Tuổi khai thác (năm cạo)
                                            YearOfShaving = GetCell(31)?.ToString() ?? "",                          // Năm cạo úp
                                            ShavingFaceConditionCode = GetCell(32)?.ToString() ?? "",               // Mã tình trạng mặt cạo
                                            ProductivityByArea = TryDouble(GetCell(33)),                      // Năng suất theo diện tích
                                            ProductivityByTree = TryInt(GetCell(34)),                      // Năng suất theo cây
                                            TotalShavingSlice = TryInt(GetCell(35)),                       // Tổng số lát cạo
                                            ClassifyCode = GetCell(36)?.ToString() ?? "",  // Xếp hạng
                                            Remark = GetCell(37)?.ToString() ?? "",      // Ghi chú

                                            // Defaulted/computed fields
                                            SurveyBatchId = 0,                           // ID đợt khảo sát
                                            OwnerId = 0,                                 // Chủ sở hữu
                                            CultivatorId = 0,                            // Người canh tác
                                            PlotId = "",                          // Mã lô
                                            RiskLevel = "",                              // Mức độ rủi ro
                                            PlotName = "",                               // Tên lô
                                            LandType = "",                               // Loại đất
                                            AreaCultivated = 0.0f,                       // Diện tích đã canh tác
                                            Area1 = 0.0f,                                // Diện tích theo cách 1
                                            Area2 = 0.0f,                                // Diện tích theo cách 2
                                            Area3 = 0.0f,                                // Diện tích theo cách 3
                                            AltitudeLowest = 0.0f,                       // Cao trình thấp nhất (double)
                                            AltitudeHighest = 0.0f,                      // Cao trình cao nhất  (double)
                                            AreaManagementChange = 0.0f,                   // Diện tích 
                                            ActiveStatusId = 6,                          // Trạng thái hoạt động (mã)
                                            Hecta = 0,                                   // Số hecta (nếu có)
                                            GraftedTreeCorrectQuantity = 0, // SL cây ghép đúng giống
                                            GraftedTreeMixedQuantity = 0,   // SL cây ghép lẫn giống
                                            DensityOfGraftedTree = 0, // Mật độ cây ghép
                                            AverageNumberLeafLayer = 0, // Số tầng lá trung bình
                                            PlantingEndDate = "", // Ngày kết thúc trồng
                                            TreeQuantity = 0,                            // Tổng số cây
                                            TreeQuantityShaving = 0,                     // SL cây đã mở cạo
                                            EndExploitationDate = "",                    // Ngày kết thúc khai thác
                                            ShavingFaceConditionId = 0,                  // ID tình trạng mặt cạo
                                            StandardDeviation = 0,                       // Độ lệch chuẩn (vd: vanh)
                                            RatioTreeObtain = 0,                         // % cây đạt tiêu chuẩn
                                            MarkedExtendedGarden = false,                    // Đánh dấu vườn kéo dài (0/1)
                                            ExpectedExploitationDate = "",               // Tháng mở cạo
                                            GardenRatingId = 0,                          // ID đánh giá vườn
                                            VanhAverage = 0.0f,        // Vanh bình quân 2025
                                            RootTreeMixedQuantity = 0,                   // SL cây gốc lẫn
                                            RootTreeCorrectQuantity = 0,                 // SL cây gốc đúng
                                            Count = 0,                                   // Đếm số bản ghi (nếu cần)
                                            TotalOutput = 0.0f                           // Tổng sản lượng
                                        });
                                    }
                                }
                            }

                        } while (reader.NextResult());
                    }
                }

                jResult.result = 1;
                jResult.data = data;
                jResult.error = new error(1, $"Đã đọc {data.Count} dòng từ Excel.");
            }
            catch (Exception ex)
            {
                jResult.result = -1;
                jResult.error = new error(-1, $"Xảy ra lỗi trong quá trình đọc file. {ex.Message}");
            }

            return Json(jResult);
        }

        [HttpPost]
        public async Task<JsonResult> ExportExcel(int? year, string? variety, string? ids)
        {
            M_JResult jResult = new M_JResult();
            try
            {
                var candidateTemplatePaths = new List<string>
                {
                    Path.Combine(_webHostEnvironment.WebRootPath, "template", "bieumaukiemke.xlsx"),
                    Path.Combine(_webHostEnvironment.ContentRootPath, "bieumaukiemke.xlsx"),
                    Path.GetFullPath(Path.Combine(_webHostEnvironment.ContentRootPath, "..", "bieumaukiemke.xlsx"))
                };
                var templatePath = candidateTemplatePaths.FirstOrDefault(System.IO.File.Exists);
                if (string.IsNullOrEmpty(templatePath))
                {
                    jResult.result = 0;
                    jResult.error = new error(0, "Không tìm thấy file biểu mẫu kiểm kê (bieumaukiemke.xlsx). Vui lòng đặt vào wwwroot/template hoặc thư mục gốc dự án.");
                    return Json(jResult);
                }

                using (var workbook = new XLWorkbook(templatePath))
                {

                    var mapTMTCFields = new Dictionary<int, string>
                    {
                        { 1, "idPrivate" },
                        { 5, "plotName" },
                        { 6, "landLevelCode" },
                        { 7, "altitudeAverage" },
                        { 8, "plantingMethodCode" },
                        { 9, "plantingDistanceCode" },
                        { 10, "plantingDesignDensity" },
                        { 11, "typeOfTreeCode" },
                        { 12, "area" },
                        { 13, "holeQuantity" },
                        { 14, "graftedTreeCorrectQuantity" },
                        { 16, "graftedTreeMixedQuantity" },
                        { 18, "emptyHoleQuantity" },
                        { 20, "densityOfGraftedTree" },
                        { 21, "averageNumberLeafLayer" },
                        { 22, "classifyCode"},
                        { 23, "plantingEndDate" },
                        { 24, "remark" },
                    };

                    var mapKTCBFields = new Dictionary<int, string>
                    {
                        { 1, "idPrivate" },
                        { 5, "plotOldName" },
                        { 6, "plotNewName" },
                        { 7, "yearOfPlanting" },
                        { 8, "landLevelCode" },
                        { 9, "altitudeAverage" },
                        { 10, "plantingMethodCode" },
                        { 11, "plantingDistanceCode" },
                        { 12, "plantingDesignDensity" },
                        { 13, "typeOfTreeCode" },
                        { 15, "area" },
                        { 16, "holeQuantity" },
                        { 17, "effectiveTreeCorrectQuantity" },
                        { 19, "effectiveTreeMixedQuantity" },
                        { 21, "ineffectiveTreeNotgrowQuantity" },
                        { 23, "emptyHoleQuantity" },
                        { 25, "effectiveTreeDensity" },
                        { 27, "vanhAverage" },
                        { 28, "standardDeviation" },
                        { 29, "ratioTreeObtain" },
                        // 30 special: markedExtendedGarden -> "x"
                        { 31, "expectedExploitationDate" },
                        { 32, "classifyCode" },
                        { 33, "remark" },
                    };

                    var mapKDFields = new Dictionary<int, string>
                    {
                        { 1, "idPrivate" },
                        { 5, "plotOldName" },
                        { 6, "plotNewName" },
                        { 7, "yearOfPlanting" },
                        { 8, "landLevelCode" },
                        { 9, "altitudeAverage" },
                        { 10, "plantingMethodCode" },
                        { 11, "plantingDistanceCode" },
                        { 12, "plantingDesignDensity" },
                        { 13, "typeOfTreeCode" },
                        { 15, "area" },
                        { 16, "holeQuantity" },
                        { 17, "effectiveTreeShavingQuantity" },
                        { 19, "effectiveTreeNotshavingQuantity" },
                        { 21, "ineffectiveTreeDryQuantity" },
                        { 25, "emptyHoleQuantity" },
                        { 27, "shavingTreeDensity" },
                        { 28, "shavingModeCode" },
                        { 29, "startExploitationDate" },
                        { 30, "tappingAge" },
                        { 31, "yearOfShaving" },
                        { 32, "shavingFaceConditionCode" },
                        { 33, "productivityByArea" },
                        { 34, "productivityByTree" },
                        { 35, "totalShavingSlice" },
                        { 36, "classifyCode" },
                        { 37, "remark" },
                    };

                    // Load data from data.json
                    var candidateJsonPaths = new List<string>
                    {
                        Path.GetFullPath(Path.Combine(_webHostEnvironment.ContentRootPath, "..", "data.json")),
                        Path.Combine(_webHostEnvironment.ContentRootPath, "data.json"),
                        Path.Combine(_webHostEnvironment.WebRootPath, "data.json"),
                    };
                    var jsonPath = candidateJsonPaths.FirstOrDefault(System.IO.File.Exists);
                    if (string.IsNullOrEmpty(jsonPath))
                    {
                        jResult.result = 0;
                        jResult.error = new error(0, "Không tìm thấy data.json để xuất dữ liệu.");
                        return Json(jResult);
                    }

                    var jsonText = System.IO.File.ReadAllText(jsonPath);
                    using var doc = JsonDocument.Parse(jsonText);
                    var items = doc.RootElement.ValueKind == JsonValueKind.Array ? doc.RootElement.EnumerateArray() : Enumerable.Empty<JsonElement>();

                    int rowTMTC = 11;
                    int rowKTCB = 11;
                    int rowKD = 11;
                    int sttTMTC = 1;
                    int sttKTCB = 1;
                    int sttKD = 1;

                    foreach (var item in items)
                    {
                        int active = item.TryGetProperty("activeStatusId", out var actEl) && actEl.TryGetInt32(out var a) ? a : 0;
                        if (active == 14)
                        {
                            var ws = workbook.Worksheet("1.TM-TC");
                            ws.Row(rowTMTC).InsertRowsBelow(1);
                            ws.Cell(rowTMTC, 1).Value = sttTMTC;
                            foreach (var kv in mapTMTCFields)
                            {
                                if (item.TryGetProperty(kv.Value, out var v))
                                {
                                    var cell = ws.Cell(rowTMTC, kv.Key + 1);
                                    WriteJsonElementToCell(cell, v);
                                }
                            }
                            rowTMTC++;
                            sttTMTC++;
                        }
                        else if (active == 5)
                        {
                            var ws = workbook.Worksheet("1.KTCB");
                            ws.Row(rowKTCB).InsertRowsBelow(1);
                            // Thêm số thứ tự vào cột A
                            ws.Cell(rowKTCB, 1).Value = sttKTCB;
                            foreach (var kv in mapKTCBFields)
                            {
                                if (item.TryGetProperty(kv.Value, out var v))
                                {
                                    var cell = ws.Cell(rowKTCB, kv.Key + 1);
                                    WriteJsonElementToCell(cell, v);
                                }
                            }
                            // special column 30: markedExtendedGarden -> "x" if truthy
                            var markedX = "";
                            if (item.TryGetProperty("markedExtendedGarden", out var markEl))
                            {
                                if (markEl.ValueKind == JsonValueKind.True || (markEl.ValueKind == JsonValueKind.Number && markEl.TryGetInt32(out var mi) && mi != 0))
                                    markedX = "x";
                            }
                            ws.Cell(rowKTCB, 30 + 1).Value = markedX;
                            rowKTCB++;
                            sttKTCB++;
                        }
                        else if (active == 6)
                        {
                            var ws = workbook.Worksheet("1.KD");
                            ws.Row(rowKD).InsertRowsBelow(1);
                            ws.Cell(rowKD, 1).Value = sttKD;
                            foreach (var kv in mapKDFields)
                            {
                                if (item.TryGetProperty(kv.Value, out var v))
                                {
                                    var cell = ws.Cell(rowKD, kv.Key + 1);
                                    WriteJsonElementToCell(cell, v);
                                }
                            }
                            rowKD++;
                            sttKD++;
                        }
                    }

                    try
                    {
                        var ws0 = workbook.Worksheet(1);
                        int rB = 13;
                        foreach (var item in doc.RootElement.EnumerateArray())
                        {
                            if (item.TryGetProperty("idPrivate", out var idEl))
                            {
                                ws0.Cell(rB, 2).Value = idEl.GetString();
                                rB++;
                            }
                        }
                    }
                    catch { }

                    static void WriteJsonElementToCell(IXLCell cell, JsonElement el)
                    {
                        switch (el.ValueKind)
                        {
                            case JsonValueKind.String:
                                cell.Value = el.GetString();
                                break;
                            case JsonValueKind.Number:
                                if (el.TryGetInt64(out var l)) cell.Value = l;
                                else if (el.TryGetDouble(out var d)) cell.Value = d;
                                else cell.Value = el.ToString();
                                break;
                            case JsonValueKind.True:
                            case JsonValueKind.False:
                                cell.Value = el.GetBoolean();
                                break;
                            default:
                                cell.Value = el.ToString();
                                break;
                        }
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    }

                    var exportsDir = Path.Combine(_webHostEnvironment.WebRootPath, "exports");
                    if (!Directory.Exists(exportsDir))
                        Directory.CreateDirectory(exportsDir);

                    var timestamp = Utilities.CurrentTimeSeconds();
                    var fileName = $"template_import_{timestamp}.xlsx";
                    var filePath = Path.Combine(exportsDir, fileName);

                    workbook.SaveAs(filePath);

                    jResult.result = 1;
                    jResult.data = $"/exports/{fileName}";

                    var threadDeleteFile = new Thread(() =>
                    {
                        try
                        {
                            Thread.Sleep(5000);
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Unable to delete file: {ex.Message}");
                        }
                    });
                    threadDeleteFile.Start();
                }
            }
            catch (Exception ex)
            {
                jResult.result = -1;
                jResult.error = new error(500, $"Export failed. {ex.Message}");
            }
            await Task.CompletedTask;
            return Json(jResult);
        }
    }
}