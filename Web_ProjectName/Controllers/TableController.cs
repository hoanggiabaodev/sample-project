using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Web_ProjectName.Lib;
using ClosedXML.Excel;
using System.Threading;
using System.Text.Json;
using Web_ProjectName.Services;
using Web_ProjectName.Models;
using System.Text;
using System.Globalization;

namespace Web_ProjectName.Controllers
{
    public class TableController : BaseController<TableController>
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IS_DiaryTree _diaryTreeService;
        private readonly IS_SurveyFarm _surveyFarmService;
        public TableController(IWebHostEnvironment webHostEnvironment, IS_DiaryTree diaryTreeService, IS_SurveyFarm surveyFarm)
        {
            _webHostEnvironment = webHostEnvironment;
            _diaryTreeService = diaryTreeService;
            _surveyFarmService = surveyFarm;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred: " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetList()
        {
            try
            {
                string _accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJBY2NvdW50SWQiOiI3MyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6Imh1eXF1b2N2bzI0MDdAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIwODYyMDU0MzI3IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6IjA4NjIwNTQzMjciLCJTdXBwbGllcklkIjoiMSIsIkZ1bGxOYW1lIjoiSHV5IEh1eVZvRGV2MmEiLCJleHAiOjE4MTU3Njc1OTMsImlzcyI6Imh0dHA6Ly90YW5pcnVjby5jb20vIiwiYXVkIjoiaHR0cDovL3RhbmlydWNvLmNvbS8ifQ.NeBM07DNwcolZ9T11_8ogVvDjdyvmTY98pwwUOBLKh8";
                var response = await _diaryTreeService.GetListDataByPlaceMark(_accessToken);

                if (response.result == 1 && response.data != null)
                {
                    return Json(new M_JResult(response));
                }
                else
                {
                    return Json(new M_JResult(response.result, response.error, new List<M_DiaryTreeByPlaceMark>()));
                }
            }
            catch (Exception ex)
            {
                return Json(new M_JResult(0, new error(500, ex.Message), new List<M_DiaryTreeByPlaceMark>()));
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetYears()
        {
            try
            {
                string _accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJBY2NvdW50SWQiOiI3MyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6Imh1eXF1b2N2bzI0MDdAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIwODYyMDU0MzI3IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6IjA4NjIwNTQzMjciLCJTdXBwbGllcklkIjoiMSIsIkZ1bGxOYW1lIjoiSHV5IEh1eVZvRGV2MmEiLCJleHAiOjE4MTU3Njc1OTMsImlzcyI6Imh0dHA6Ly90YW5pcnVjby5jb20vIiwiYXVkIjoiaHR0cDovL3RhbmlydWNvLmNvbS8ifQ.NeBM07DNwcolZ9T11_8ogVvDjdyvmTY98pwwUOBLKh8";
                var response = await _diaryTreeService.GetListDataByPlaceMark(_accessToken);

                if (response.result == 1 && response.data != null)
                {
                    var years = response.data
                        .Where(x => !string.IsNullOrEmpty(x.YearOfPlanting))
                        .Select(x => x.YearOfPlanting)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList();

                    return Json(new { result = 1, data = years });
                }
                else
                {
                    return Json(new { result = 1, data = new List<string>() });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = 1, data = new List<string>() });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVarieties()
        {
            try
            {
                string _accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJBY2NvdW50SWQiOiI3MyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6Imh1eXF1b2N2bzI0MDdAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIwODYyMDU0MzI3IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6IjA4NjIwNTQzMjciLCJTdXBwbGllcklkIjoiMSIsIkZ1bGxOYW1lIjoiSHV5IEh1eVZvRGV2MmEiLCJleHAiOjE4MTU3Njc1OTMsImlzcyI6Imh0dHA6Ly90YW5pcnVjby5jb20vIiwiYXVkIjoiaHR0cDovL3RhbmlydWNvLmNvbS8ifQ.NeBM07DNwcolZ9T11_8ogVvDjdyvmTY98pwwUOBLKh8";
                var response = await _diaryTreeService.GetListDataByPlaceMark(_accessToken);

                if (response.result == 1 && response.data != null)
                {
                    var varieties = response.data
                        .Where(x => !string.IsNullOrEmpty(x.TypeOfTreeName))
                        .Select(x => x.TypeOfTreeName)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList();

                    return Json(new { result = 1, data = varieties });
                }
                else
                {
                    return Json(new { result = 1, data = new List<string>() });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = 1, data = new List<string>() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ImportExcel(IFormFile? file)
        {
            var jResult = new M_JResult();
            try
            {
                if (file == null || file.Length == 0)
                    return Json(new M_JResult { result = 0, error = new error(0, "Vui lòng chọn file cần import.") });

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (ext is not (".xlsx" or ".xls"))
                    return Json(new M_JResult { result = 0, error = new error(0, "Định dạng file không hỗ trợ. Vui lòng chọn .xlsx hoặc .xls.") });

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                // Helpers
                string S(object v) => v?.ToString()?.Trim() ?? "";
                int I(object v) => int.TryParse(S(v), out var i) ? i : (int)Math.Round(D(v));
                double D(object v) => double.TryParse(S(v), NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0d;
                string Dt(object v) => v is DateTime dt ? dt.ToString("yyyy-MM-dd") :
                                       DateTime.TryParse(S(v), out var d) ? d.ToString("yyyy-MM-dd") : S(v);
                bool B(object v) => !string.IsNullOrWhiteSpace(v?.ToString());

                var data = new List<object>();
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using var reader = ExcelReaderFactory.CreateReader(stream);
                do
                {
                    var sheet = reader.Name.ToUpperInvariant();
                    if (!new[] { "1.TM-TC", "1.TC-TM", "1.KTCB", "1.KD", "1.TXA", "1.TXB" }.Contains(sheet)) continue;

                    int rowStart = sheet switch
                    {
                        "1.KD" or "1.KTCB" or "1.TM-TC" or "1.TC-TM" => 10,
                        _ => 9
                    };

                    while (reader.Read())
                    {
                        if (reader.Depth < rowStart) continue;
                        object Cell(int i) => i < reader.FieldCount ? reader.GetValue(i) : null;

                        // bắt buộc có IdPrivate (cột 1) và cột D (cột 4)
                        if (string.IsNullOrWhiteSpace(S(Cell(1))) || string.IsNullOrWhiteSpace(S(Cell(3))))
                        {
                            continue;
                        }

                        // TM-TC
                        if (sheet is "1.TM-TC" or "1.TC-TM")
                            data.Add(new
                            {
                                ActiveStatusCode = "TMTC",
                                IdPrivate = S(Cell(1)),
                                PlotName = S(Cell(5)),
                                LandLevelCode = S(Cell(6)),
                                AltitudeAverage = D(Cell(7)),
                                PlantingMethodCode = S(Cell(8)),
                                PlantingDistanceCode = S(Cell(9)),
                                PlantingDesignDensity = D(Cell(10)),
                                TypeOfTreeCode = S(Cell(11)),
                                Area = D(Cell(12)),
                                HoleQuantity = I(Cell(13)),
                                GraftedTreeCorrectQuantity = I(Cell(14)),
                                GraftedTreeMixedQuantity = I(Cell(16)),
                                EmptyHoleQuantity = I(Cell(18)),
                                DensityOfGraftedTree = I(Cell(20)),
                                AverageNumberLeafLayer = D(Cell(21)),
                                ClassifyCode = S(Cell(22)),
                                PlantingEndDate = Dt(Cell(23)),
                                Remark = S(Cell(24))
                            });

                        // KTCB
                        else if (sheet == "1.KTCB")
                            data.Add(new
                            {
                                ActiveStatusCode = "KTCB",
                                IdPrivate = S(Cell(1)),
                                PlotOldName = S(Cell(5)),
                                PlotNewName = S(Cell(6)),
                                YearOfPlanting = S(Cell(7)),
                                LandLevelCode = S(Cell(8)),
                                AltitudeAverage = D(Cell(9)),
                                PlantingMethodCode = S(Cell(10)),
                                PlantingDistanceCode = S(Cell(11)),
                                PlantingDesignDensity = D(Cell(12)),
                                TypeOfTreeCode = S(Cell(13)),
                                Area = D(Cell(15)),
                                HoleQuantity = I(Cell(16)),
                                EffectiveTreeCorrectQuantity = I(Cell(17)),
                                EffectiveTreeMixedQuantity = I(Cell(19)),
                                IneffectiveTreeNotgrowQuantity = I(Cell(21)),
                                EmptyHoleQuantity = I(Cell(23)),
                                EffectiveTreeDensity = I(Cell(25)),
                                VanhAverage = D(Cell(27)),
                                StandardDeviation = D(Cell(28)),
                                RatioTreeObtain = D(Cell(29)),
                                MarkedExtendedGarden = B(Cell(30)),
                                ExpectedExploitationDate = Dt(Cell(31)),
                                ClassifyCode = S(Cell(32)),
                                Remark = S(Cell(33))
                            });

                        // KD
                        else if (sheet == "1.KD")
                            data.Add(new
                            {
                                ActiveStatusCode = "KD",
                                IdPrivate = S(Cell(1)),
                                PlotOldName = S(Cell(5)),
                                PlotNewName = S(Cell(6)),
                                YearOfPlanting = S(Cell(7)),
                                LandLevelCode = S(Cell(8)),
                                AltitudeAverage = D(Cell(9)),
                                PlantingMethodCode = S(Cell(10)),
                                PlantingDistanceCode = S(Cell(11)),
                                PlantingDesignDensity = D(Cell(12)),
                                TypeOfTreeCode = S(Cell(13)),
                                Area = D(Cell(15)),
                                HoleQuantity = I(Cell(16)),
                                EffectiveTreeShavingQuantity = I(Cell(17)),
                                EffectiveTreeNotshavingQuantity = I(Cell(19)),
                                IneffectiveTreeDryQuantity = I(Cell(21)),
                                IneffectiveTreeNotgrowQuantity = I(Cell(23)),
                                EmptyHoleQuantity = I(Cell(25)),
                                ShavingTreeDensity = I(Cell(27)),
                                ShavingModeCode = S(Cell(28)),
                                StartExploitationDate = Dt(Cell(29)),
                                TappingAge = I(Cell(30)),
                                YearOfShaving = S(Cell(31)),
                                ShavingFaceConditionCode = S(Cell(32)),
                                ProductivityByArea = D(Cell(33)),
                                ProductivityByTree = D(Cell(34)),
                                TotalShavingSlice = I(Cell(35)),
                                ClassifyCode = S(Cell(36)),
                                Remark = S(Cell(37))
                            });

                        // TXA
                        else if (sheet == "1.TXA")
                            data.Add(new
                            {
                                ActiveStatusCode = "TX",
                                IntercropType = 0,
                                IdPrivate = S(Cell(1)),
                                PlotName = S(Cell(2)),
                                YearOfPlanting = S(Cell(3)),
                                PlantingDistanceCode = S(Cell(4)),
                                PlantingDesignDensity = D(Cell(5)),
                                Area = D(Cell(6)),
                                IntercropName = S(Cell(7)),
                                IntercroppingYear = S(Cell(8)),
                                IntercroppingArea = D(Cell(9)),
                                CareContract = S(Cell(10)),
                                ProductContract = S(Cell(11)),
                                FinancialIncome = D(Cell(12)),
                                IntercroppingOther = D(Cell(13)),
                                IntercroppingCompany = S(Cell(14)),
                                NoContribEcon = S(Cell(15)),
                                NoContribPers = D(Cell(16)),
                                PartContribEcon = S(Cell(17)),
                                PartContribPers = D(Cell(18)),
                                ShavingTreeDensity = I(Cell(19)),
                                VanhAverage = D(Cell(20)),
                                RatioTreeObtain = D(Cell(21)),
                                ClassifyCode = S(Cell(22))
                            });

                        // TXB
                        else if (sheet == "1.TXB")
                            data.Add(new
                            {
                                ActiveStatusCode = "TX",
                                IntercropType = 1,
                                IdPrivate = S(Cell(1)),
                                PlotName = S(Cell(2)),
                                YearOfPlanting = S(Cell(3)),
                                PlantingDistanceCode = S(Cell(4)),
                                PlantingDesignDensity = D(Cell(5)),
                                Area = D(Cell(6)),
                                IntercropName = S(Cell(7)),
                                IntercroppingYear = S(Cell(8)),
                                IntercroppingArea = D(Cell(9)),
                                CareContract = S(Cell(10)),
                                ProductContract = S(Cell(11)),
                                FinancialIncome = D(Cell(12)),
                                IntercroppingOther = D(Cell(13)),
                                IntercroppingCompany = S(Cell(14)),
                                NoContribEcon = S(Cell(15)),
                                NoContribPers = D(Cell(16)),
                                PartContribEcon = S(Cell(17)),
                                PartContribPers = D(Cell(18))
                            });
                    }
                } while (reader.NextResult());

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
                var templatePath = Path.Combine(_webHostEnvironment.WebRootPath, "template/bieumaukiemke.xlsx");
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
                        { 23, "ineffectiveTreeNotgrowQuantity" },
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

                    var response = await _surveyFarmService.GetListSurveyFarmFullData(
                        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJBY2NvdW50SWQiOiI3MyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6Imh1eXF1b2N2bzI0MDdAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIwODYyMDU0MzI3IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6IjA4NjIwNTQzMjciLCJTdXBwbGllcklkIjoiMSIsIkZ1bGxOYW1lIjoiSHV5IEh1eVZvRGV2MmEiLCJleHAiOjE4MTYxOTI2NzYsImlzcyI6Imh0dHA6Ly90YW5pcnVjby5jb20vIiwiYXVkIjoiaHR0cDovL3RhbmlydWNvLmNvbS8ifQ.VS-3vcomcbfvPSQLMfapUI1rIoPjXjZx7UBh2qh75Vc",
                        null,
                        year?.ToString(),
                        null
                    );
                    if (response.result != 1 || response.data == null)
                    {
                        jResult.result = 0;
                        jResult.error = response.error ?? new error(0, "Không lấy được dữ liệu kiểm kê.");
                        return Json(jResult);
                    }
                    var allItems = response.data ?? new List<M_SurveyFarm>();
                    IEnumerable<M_SurveyFarm> items = allItems;
                    if (!string.IsNullOrWhiteSpace(variety))
                        items = items.Where(x =>
                            (!string.IsNullOrWhiteSpace(x.TypeOfTreeObj?.Name) && x.TypeOfTreeObj.Name.Equals(variety, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrWhiteSpace(x.TypeOfTreeObj?.Code) && x.TypeOfTreeObj.Code.Equals(variety, StringComparison.OrdinalIgnoreCase)));
                    if (!string.IsNullOrWhiteSpace(ids))
                    {
                        var idSet = new HashSet<string>(ids.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
                        items = items.Where(x => !string.IsNullOrEmpty(x.IdPrivate) && idSet.Contains(x.IdPrivate));
                    }
                    var itemsList = items.ToList();
                    if (itemsList.Count == 0)
                    {
                        itemsList = allItems;
                    }

                    int rowTMTC = 10;
                    int rowKTCB = 10;
                    int rowKD = 10;
                    int rowTXA = 10;
                    int rowTXB = 10;
                    int sttTMTC = 1;
                    int sttKTCB = 1;
                    int sttKD = 1;
                    int sttTXA = 1;
                    int sttTXB = 1;

                    foreach (var item in itemsList)
                    {
                        string active = string.Empty;
                        if (item.ActiveStatusId.HasValue)
                        {
                            switch (item.ActiveStatusId.Value)
                            {
                                case 14: active = "TMTC"; break;
                                case 5: active = "KTCB"; break;
                                case 6: active = "KD"; break;
                                case 15: active = "TXA"; break;
                                case 0: active = "TXB"; break;
                                default: active = item.ActiveStatusObj?.Code?.ToUpperInvariant() ?? string.Empty; break;
                            }
                        }
                        if (string.IsNullOrWhiteSpace(active))
                        {
                            if (item.IntercropType.HasValue)
                                active = (item.IntercropType.Value == 0) ? "TXA" : "TXB";
                            else if (item.EffectiveTreeShavingQuantity.HasValue || item.StartExploitationDate.HasValue || item.TappingAge.HasValue)
                                active = "KD";
                            else if (item.EffectiveTreeCorrectQuantity.HasValue || item.ExpectedExploitationDate.HasValue || item.VanhAverage.HasValue)
                                active = "KTCB";
                            else
                                active = "TMTC";
                        }
                        if (active == "TMTC")
                        {
                            var ws = workbook.Worksheet("1.TM-TC") ?? workbook.Worksheet("1.TC-TM");
                            ws.Row(rowTMTC).InsertRowsBelow(1);
                            var sttCell = ws.Cell(rowTMTC, 1);
                            sttCell.Value = sttTMTC;
                            sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            sttCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            foreach (var kv in mapTMTCFields)
                            {
                                var cell = ws.Cell(rowTMTC, kv.Key + 1);
                                WriteObjectToCell(cell, GetValue(item, kv.Value));
                            }
                            SetFormula(ws, rowTMTC, 15 + 1, $"=ROUND(O{rowTMTC}/N{rowTMTC}%,1)");
                            SetFormula(ws, rowTMTC, 17 + 1, $"=ROUND(Q{rowTMTC}/N{rowTMTC}%,1)");
                            SetFormula(ws, rowTMTC, 19 + 1, $"=ROUND(S{rowTMTC}/N{rowTMTC}%,1)");
                            rowTMTC++;
                            sttTMTC++;
                        }
                        else if (active == "KTCB")
                        {
                            var ws = workbook.Worksheet("1.KTCB");
                            ws.Row(rowKTCB).InsertRowsBelow(1);
                            var sttCell = ws.Cell(rowKTCB, 1);
                            sttCell.Value = sttKTCB;
                            sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            sttCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            foreach (var kv in mapKTCBFields)
                            {
                                var cell = ws.Cell(rowKTCB, kv.Key + 1);
                                WriteObjectToCell(cell, GetValue(item, kv.Value));
                            }
                            SetFormula(ws, rowKTCB, 19, $"=ROUND(R{rowKTCB}/Q{rowKTCB}%,1)");
                            SetFormula(ws, rowKTCB, 21, $"=ROUND(T{rowKTCB}/Q{rowKTCB}%,1)");
                            SetFormula(ws, rowKTCB, 23, $"=ROUND(V{rowKTCB}/Q{rowKTCB}%,1)");
                            SetFormula(ws, rowKTCB, 25, $"=ROUND(X{rowKTCB}/Q{rowKTCB}%,1)");
                            var markedX = (item.PlannedExtendedGarden ?? false) ? "x" : string.Empty;
                            ws.Cell(rowKTCB, 30 + 1).Value = markedX;
                            ws.Cell(rowKTCB, 30 + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(rowKTCB, 30 + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            rowKTCB++;
                            sttKTCB++;
                        }
                        else if (active == "KD")
                        {
                            var ws = workbook.Worksheet("1.KD");
                            ws.Row(rowKD).InsertRowsBelow(1);
                            var sttCell = ws.Cell(rowKD, 1);
                            sttCell.Value = sttKD;
                            sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            sttCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            foreach (var kv in mapKDFields)
                            {
                                var cell = ws.Cell(rowKD, kv.Key + 1);
                                WriteObjectToCell(cell, GetValue(item, kv.Value));
                            }
                            SetFormula(ws, rowKD, 19, $"=ROUND(R{rowKD}/Q{rowKD}%,1)");
                            SetFormula(ws, rowKD, 21, $"=ROUND(T{rowKD}/Q{rowKD}%,1)");
                            SetFormula(ws, rowKD, 23, $"=ROUND(V{rowKD}/Q{rowKD}%,1)");
                            SetFormula(ws, rowKD, 25, $"=ROUND(X{rowKD}/Q{rowKD}%,1)");
                            SetFormula(ws, rowKD, 27, $"=ROUND((Z{rowKD}/Q{rowKD}*100),2)");
                            rowKD++;
                            sttKD++;
                        }
                        else if (active == "TX" || active == "TXA" || active == "TXB")
                        {
                            bool isTXA = active == "TXA" ? true : active == "TXB" ? false : ((item.IntercropType ?? 0) == 0);
                            var ws = workbook.Worksheet(isTXA ? "1.TXA" : "1.TXB");
                            int currentRow = isTXA ? rowTXA : rowTXB;
                            ws.Row(currentRow).InsertRowsBelow(1);
                            var sttCell = ws.Cell(currentRow, 1);
                            sttCell.Value = isTXA ? sttTXA : sttTXB;
                            sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            sttCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                            var mapTXFields = new Dictionary<int, string>
                            {
                                { 1, "IdPrivate" },
                                { 2, "PlotName" },
                                { 3, "YearOfPlanting" },
                                { 4, "PlantingDistanceId" },
                                { 5, "PlantingDesignDensity" },
                                { 6, "Area" },
                                { 7, "IntercropName" },
                                { 8, "IntercroppingYear" },
                                { 9, "IntercroppingArea" },
                                { 10, "CareContract" },
                                { 11, "ProductContract" },
                                { 12, "FinancialIncome" },
                                { 13, "IntercroppingOther" },
                                { 14, "IntercroppingCompany" },
                                { 15, "NoContribEcon" },
                                { 16, "NoContribPers" },
                                { 17, "PartContribEcon" },
                                { 18, "PartContribPers" },
                                { 19, "ShavingTreeDensity" },
                                { 20, "VanhAverage" },
                                { 21, "RatioTreeObtain" }
                            };
                            foreach (var kv in mapTXFields)
                            {
                                var cell = ws.Cell(currentRow, kv.Key + 1);
                                WriteObjectToCell(cell, GetValue(item, kv.Value));
                            }

                            if (isTXA)
                            {
                                rowTXA++;
                                sttTXA++;
                            }
                            else
                            {
                                rowTXB++;
                                sttTXB++;
                            }
                        }
                    }

                    try
                    {
                        var ws0 = workbook.Worksheet(1);
                        int rB = 13;
                        foreach (var item in itemsList)
                        {
                            if (!string.IsNullOrWhiteSpace(item.IdPrivate))
                            {
                                ws0.Cell(rB, 2).Value = item.IdPrivate;
                                rB++;
                            }
                        }
                    }
                    catch { }

                    static void WriteObjectToCell(IXLCell cell, object value)
                    {
                        if (value == null)
                        {
                            cell.SetValue(string.Empty);
                        }
                        else if (value is bool b)
                        {
                            cell.SetValue(b);
                        }
                        else if (value is DateOnly dateOnly)
                        {
                            cell.SetValue(dateOnly.ToString("yyyy-MM-dd"));
                        }
                        else if (value is DateTime dt)
                        {
                            cell.SetValue(dt.ToString("yyyy-MM-dd"));
                        }
                        else if (value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong)
                        {
                            cell.SetValue(Convert.ToInt64(value));
                        }
                        else if (value is float f)
                        {
                            cell.SetValue((double)f);
                        }
                        else if (value is double dbl)
                        {
                            cell.SetValue(dbl);
                        }
                        else if (value is decimal dec)
                        {
                            cell.SetValue(dec);
                        }
                        else if (value is string s)
                        {
                            cell.SetValue(s);
                        }
                        else
                        {
                            cell.SetValue(value.ToString());
                        }
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    }

                    static object GetValue(M_SurveyFarm item, string propertyName)
                    {
                        var prop = typeof(M_SurveyFarm).GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
                        return prop?.GetValue(item);
                    }

                    var exportsDir = Path.Combine(_webHostEnvironment.WebRootPath, "exports");
                    if (!Directory.Exists(exportsDir))
                        Directory.CreateDirectory(exportsDir);

                    var timestamp = Utilities.CurrentTimeSeconds();
                    var fileName = $"template_import_{timestamp}.xlsx";
                    var filePath = Path.Combine(exportsDir, fileName);
                    foreach (var ws in workbook.Worksheets)
                    {
                        Dictionary<int, string>? mapFields = ws.Name switch
                        {
                            "1.TM-TC" or "1.TC-TM" => mapTMTCFields,
                            "1.KTCB" => mapKTCBFields,
                            "1.KD" => mapKDFields,
                            "1.TXA" => new Dictionary<int, string>
                                {
                                    { 1, "IdPrivate" }, { 2, "PlotName" }, { 3, "YearOfPlanting" },
                                    { 4, "PlantingDistanceId" }, { 5, "PlantingDesignDensity" }, { 6, "Area" },
                                    { 7, "IntercropName" }, { 8, "IntercroppingYear" }, { 9, "IntercroppingArea" },
                                    { 10, "CareContract" }, { 11, "ProductContract" }, { 12, "FinancialIncome" },
                                    { 13, "IntercroppingOther" }, { 14, "IntercroppingCompany" },
                                    { 15, "NoContribEcon" }, { 16, "NoContribPers" }, { 17, "PartContribEcon" },
                                    { 18, "PartContribPers" }, { 19, "ShavingTreeDensity" },
                                    { 20, "VanhAverage" }, { 21, "RatioTreeObtain" }, { 22, "classifyCode" }
                                },
                            "1.TXB" => new Dictionary<int, string>
                                {
                                    { 1, "IdPrivate" }, { 2, "PlotName" }, { 3, "YearOfPlanting" },
                                    { 4, "PlantingDistanceId" }, { 5, "PlantingDesignDensity" }, { 6, "Area" },
                                    { 7, "IntercropName" }, { 8, "IntercroppingYear" }, { 9, "IntercroppingArea" },
                                    { 10, "CareContract" }, { 11, "ProductContract" }, { 12, "FinancialIncome" },
                                    { 13, "IntercroppingOther" }, { 14, "IntercroppingCompany" },
                                    { 15, "NoContribEcon" }, { 16, "NoContribPers" },
                                    { 17, "PartContribEcon" }, { 18, "PartContribPers" }
                                },
                            _ => null
                        };


                        if (mapFields != null)
                        {
                            foreach (var colIndex in mapFields.Keys)
                            {
                                var col = ws.Column(colIndex + 1);

                                double maxWidth = 15;

                                for (int row = 10; row <= ws.LastRowUsed().RowNumber(); row++)
                                {
                                    var cell = ws.Cell(row, colIndex + 1);
                                    try
                                    {
                                        var cellValue = cell.Value.ToString();
                                        if (!string.IsNullOrWhiteSpace(cellValue))
                                        {
                                            double contentWidth = EstimateContentWidth(cellValue);
                                            if (contentWidth > maxWidth)
                                            {
                                                maxWidth = contentWidth;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                                col.Width = maxWidth;
                            }
                        }
                    }


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
                jResult.error = new error(500, $"Xuất file thất bại. {ex.Message}");
            }
            await Task.CompletedTask;
            return Json(jResult);
        }

        private static void SetFormula(IXLWorksheet ws, int row, int col, string formula)
        {
            var formulaCell = ws.Cell(row, col);
            formulaCell.FormulaA1 = formula;
            formulaCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            formulaCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        private static double EstimateContentWidth(string content)
        {
            if (string.IsNullOrEmpty(content))
                return 0;

            double estimatedWidth = content.Length * 1.2 + 2;
            if (estimatedWidth > 50)
                estimatedWidth = 50;

            return estimatedWidth;
        }
    }
}