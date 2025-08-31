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
        public async Task<JsonResult> ExportExcel(int? surveyBatchId = 3, int? activeStatusId = null)
        {
            M_JResult jResult = new M_JResult();
            try
            {
                var templatePath = Path.Combine(_webHostEnvironment.WebRootPath, "template/bieumaukiemke.xlsx");
                using (var workbook = new XLWorkbook(templatePath))
                {
                    var surFarmRes = await _surveyFarmService.GetListSurveyFarmFullData(
                        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJBY2NvdW50SWQiOiI3MyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6Imh1eXF1b2N2bzI0MDdAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIwODYyMDU0MzI3IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6IjA4NjIwNTQzMjciLCJTdXBwbGllcklkIjoiMSIsIkZ1bGxOYW1lIjoiSHV5IEh1eVZvRGV2MmEiLCJleHAiOjE4MTYxOTI2NzYsImlzcyI6Imh0dHA6Ly90YW5pcnVjby5jb20vIiwiYXVkIjoiaHR0cDovL3RhbmlydWNvLmNvbS8ifQ.VS-3vcomcbfvPSQLMfapUI1rIoPjXjZx7UBh2qh75Vc",
                        surveyBatchId,
                        activeStatusId
                    );

                    if (surFarmRes.result != 1 || surFarmRes.data == null)
                    {
                        jResult.result = 0;
                        jResult.error = surFarmRes.error ?? new error(0, "Không lấy được dữ liệu kiểm kê.");
                        return Json(jResult);
                    }
                    var allItems = surFarmRes.data ?? new List<M_SurveyFarm>();
                    var itemsList = allItems.ToList();

                    var staticSurFarmRes = await _surveyFarmService.GetListStatictis(
                        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJBY2NvdW50SWQiOiI3MyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6Imh1eXF1b2N2bzI0MDdAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIwODYyMDU0MzI3IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6IjA4NjIwNTQzMjciLCJTdXBwbGllcklkIjoiMSIsIkZ1bGxOYW1lIjoiSHV5IEh1eVZvRGV2MmEiLCJleHAiOjE4MTYxOTI2NzYsImlzcyI6Imh0dHA6Ly90YW5pcnVjby5jb20vIiwiYXVkIjoiaHR0cDovL3RhbmlydWNvLmNvbS8ifQ.VS-3vcomcbfvPSQLMfapUI1rIoPjXjZx7UBh2qh75Vc",
                        surveyBatchId,
                        activeStatusId
                    );

                    if (staticSurFarmRes.result != 1 || staticSurFarmRes.data == null)
                    {
                        jResult.result = 0;
                        jResult.error = staticSurFarmRes.error ?? new error(0, "Không lấy được dữ liệu thống kê.");
                        return Json(jResult);
                    }

                    var allStaticItems = staticSurFarmRes.data ?? new M_SurveyFarmStatistic();

                    var listKDByYear = allStaticItems.ListPlaceMarkKDByYearObjs ?? new List<M_SurveyFarmBusinessFarm>();
                    var listKDByFarmGroup = allStaticItems.ListPlaceMarkKDByFarmGroupObjs ?? new List<M_SurveyFarmBusinessFarmByFarmGroup>();
                    var listKTCB = allStaticItems.ListPlaceMarkKTCBObjs ?? new List<M_SurveyFarmKTCB>();
                    var listKDByTypeOfTree = allStaticItems.ListPlaceMarkKDByTypeOfTreeObjs ?? new List<M_SurveyFarmTypeOfTree>();
                    var listKDByLandLevel = allStaticItems.ListPlaceMarkKDByLandLevelObjs ?? new List<M_SurveyFarmKDLandLevel>();
                    var listKDByGardenRating = allStaticItems.ListPlaceMarkKDByGardenRatingObjs ?? new List<M_SurveyFarmKDGardenRating>();
                    var listKTCBByGardenRatingYear = allStaticItems.GetListPlaceMarkKTCBByGardenRatingYearObjs ?? new List<M_SurveyFarmKTCBGardenRating>();
                    var listKTCBByGardenRatingFarmGroup = allStaticItems.GetListPlaceMarkKTCBByGardenRatingFarmGroupObjs ?? new List<M_SurveyFarmKTCBGardenRating>();

                    // Process each sheet for Statictis SurveyFarm Model
                    Process2aKDSheet(workbook, listKDByYear);
                    Process2bKDSheet(workbook, listKDByFarmGroup);
                    Process3KTCBSheet(workbook, listKTCB);
                    Process3aKDSheet(workbook, listKDByTypeOfTree);
                    Process3bKDSheet(workbook, listKDByLandLevel);
                    Process4KDSheet(workbook, listKDByGardenRating);
                    Process2aKTCBSheet(workbook, listKTCBByGardenRatingYear);
                    Process2bKTCBSheet(workbook, listKTCBByGardenRatingFarmGroup);

                    // Process each sheet for SurveyFarm Model
                    ProcessTMTCSheet(workbook, itemsList);
                    ProcessKTCBSheet(workbook, itemsList);
                    ProcessKDSheet(workbook, itemsList);
                    ProcessTXASheet(workbook, itemsList);
                    ProcessTXBSheet(workbook, itemsList);
                    ProcessSummarySheet(workbook, itemsList);

                    // Auto-resize columns
                    AutoResizeColumns(workbook);

                    var exportsDir = Path.Combine(_webHostEnvironment.WebRootPath, "exports");
                    if (!Directory.Exists(exportsDir))
                        Directory.CreateDirectory(exportsDir);

                    var fileName = "export_kiemke.xlsx";
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
                jResult.error = new error(500, $"Xuất file thất bại. {ex.Message}");
            }
            await Task.CompletedTask;
            return Json(jResult);
        }

        private void ProcessTMTCSheet(XLWorkbook workbook, List<M_SurveyFarm> itemsList)
        {
            var ws = workbook.Worksheet("1.TM-TC") ?? workbook.Worksheet("1.TC-TM");
            if (ws == null) return;

            var mapTMTCFields = new Dictionary<int, string>
            {
                { 1, "idPrivate" },
                { 5, "plotName" },
                { 6, "landLevelObj.code" },
                { 7, "altitudeAverage" },
                { 8, "plantingMethodObj.code" },
                { 9, "plantingDistanceObj.code" },
                { 10, "plantingDesignDensity" },
                { 11, "TypeOfTreeObj.code" },
                { 12, "area" },
                { 13, "holeQuantity" },
                { 14, "graftedTreeCorrectQuantity" },
                { 16, "graftedTreeMixedQuantity" },
                { 18, "emptyHoleQuantity" },
                { 20, "densityOfGraftedTree" },
                { 21, "averageNumberLeafLayer" },
                { 22, "classifyObj.code"},
                { 23, "plantingEndDate" },
                { 24, "remark" },
            };

            int row = 11;
            int stt = 1;

            var tmtcItems = itemsList.Where(item =>
            {
                string active = item.ActiveStatusObj?.Code;
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
                return active == "TMTC";
            }).ToList();

            foreach (var item in tmtcItems)
            {
                ws.Row(row).InsertRowsBelow(1);
                var sttCell = ws.Cell(row, 1);
                sttCell.Value = stt;
                sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sttCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                foreach (var kv in mapTMTCFields)
                {
                    var cell = ws.Cell(row, kv.Key + 1);
                    WriteObjectToCell(cell, GetValue(item, kv.Value));
                }

                SetFormula(ws, row, 15 + 1, $"=ROUND(O{row}/N{row}%,1)");
                SetFormula(ws, row, 17 + 1, $"=ROUND(Q{row}/N{row}%,1)");
                SetFormula(ws, row, 19 + 1, $"=ROUND(S{row}/N{row}%,1)");

                row++;
                stt++;
            }
        }

        private void ProcessKTCBSheet(XLWorkbook workbook, List<M_SurveyFarm> itemsList)
        {
            var ws = workbook.Worksheet("1.KTCB");
            if (ws == null) return;

            var mapKTCBFields = new Dictionary<int, string>
            {
                { 1, "idPrivate" },
                { 5, "plotOldName" },
                { 6, "plotNewName" },
                { 7, "yearOfPlanting" },
                { 8, "landLevelObj.code" },
                { 9, "altitudeAverage" },
                { 10, "plantingMethodObj.code" },
                { 11, "plantingDistanceObj.code" },
                { 12, "plantingDesignDensity" },
                { 13, "TypeOfTreeObj.code" },
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
                { 31, "expectedExploitationDate" },
                { 32, "gardenRatingObj.code" },
                { 33, "remark" },
            };

            var ktcbData = itemsList.Where(item =>
            {
                string active = item.ActiveStatusObj?.Code;
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
                return active == "KTCB";
            }).ToList();

            if (!ktcbData.Any()) return;

            int row = 11;
            int stt = 1;

            var groups = ktcbData
                .GroupBy(x => x.YearOfPlanting)
                .OrderBy(g => g.Key);

            foreach (var g in groups)
            {
                var plantingYear = g.Key;
                var yearItems = g.ToList();

                foreach (var item in yearItems)
                {
                    ws.Row(row).InsertRowsBelow(1);
                    var sttCell = ws.Cell(row, 1);
                    sttCell.Value = stt;
                    sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    sttCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    foreach (var kv in mapKTCBFields)
                    {
                        var cell = ws.Cell(row, kv.Key + 1);
                        WriteObjectToCell(cell, GetValue(item, kv.Value));
                    }

                    SetFormula(ws, row, 19, $"=ROUND(R{row}/Q{row}%,1)");
                    SetFormula(ws, row, 21, $"=ROUND(T{row}/Q{row}%,1)");
                    SetFormula(ws, row, 23, $"=ROUND(V{row}/Q{row}%,1)");
                    SetFormula(ws, row, 25, $"=ROUND(X{row}/Q{row}%,1)");

                    var markedX = (item.PlannedExtendedGarden ?? false) ? "x" : string.Empty;
                    ws.Cell(row, 30 + 1).Value = markedX;
                    ws.Cell(row, 30 + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(row, 30 + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    row++;
                    stt++;
                }

                // Summary row for the year group
                ws.Row(row).InsertRowsBelow(1);
                ws.Cell(row, 8).Value = $"Cộng {plantingYear}";
                ws.Cell(row, 8).Style.Font.Bold = true;

                var startRow = row - yearItems.Count;
                var endRow = row - 1;

                SetFormula(ws, row, 16, $"=ROUND(SUM(P{startRow}:P{endRow}), 5)");
                SetFormula(ws, row, 17, $"=SUM(Q{startRow}:Q{endRow})");
                SetFormula(ws, row, 18, $"=SUM(R{startRow}:R{endRow})");
                SetFormula(ws, row, 19, $"=ROUND(AVERAGE(S{startRow}:S{endRow}),1)");
                SetFormula(ws, row, 20, $"=SUM(T{startRow}:T{endRow})");
                SetFormula(ws, row, 21, $"=ROUND(AVERAGE(U{startRow}:U{endRow}),1)");
                SetFormula(ws, row, 23, $"=ROUND(AVERAGE(W{startRow}:W{endRow}),1)");
                SetFormula(ws, row, 24, $"=SUM(X{startRow}:X{endRow})");
                SetFormula(ws, row, 25, $"=ROUND(AVERAGE(Y{startRow}:Y{endRow}),1)");
                SetFormula(ws, row, 26, $"=ROUND(AVERAGE(Z{startRow}:Z{endRow}),1)");
                SetFormula(ws, row, 28, $"=ROUND(AVERAGE(AB{startRow}:AB{endRow}),2)");
                SetFormula(ws, row, 29, $"=ROUND(AVERAGE(AC{startRow}:AC{endRow}),2)");
                SetFormula(ws, row, 30, $"=ROUND(AVERAGE(AD{startRow}:AD{endRow}),2)");

                var summaryRow = ws.Row(row);
                summaryRow.Style.Font.Bold = true;
                summaryRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                row++;
            }
        }

        private void ProcessKDSheet(XLWorkbook workbook, List<M_SurveyFarm> itemsList)
        {
            var ws = workbook.Worksheet("1.KD");
            if (ws == null) return;

            var mapKDFields = new Dictionary<int, string>
            {
                { 1, "idPrivate" },
                { 5, "plotOldName" },
                { 6, "plotNewName" },
                { 7, "yearOfPlanting" },
                { 8, "landLevelObj.code" },
                { 9, "altitudeAverage" },
                { 10, "plantingMethodObj.code" },
                { 11, "plantingDistanceObj.code" },
                { 12, "plantingDesignDensity" },
                { 13, "TypeOfTreeObj.code" },
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
                { 36, "gardenRatingObj.code" },
                { 37, "remark" },
            };

            var kdData = itemsList.Where(item =>
            {
                string active = item.ActiveStatusObj?.Code;
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
                return active == "KD";
            }).ToList();

            if (!kdData.Any()) return;

            int row = 11;
            int stt = 1;

            var groups = kdData
                .GroupBy(x => x.YearOfPlanting)
                .OrderBy(g => g.Key);

            foreach (var g in groups)
            {
                var plantingYear = g.Key;
                var yearItems = g.ToList();

                foreach (var item in yearItems)
                {
                    ws.Row(row).InsertRowsBelow(1);
                    var sttCell = ws.Cell(row, 1);
                    sttCell.Value = stt;
                    sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    sttCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    foreach (var kv in mapKDFields)
                    {
                        var cell = ws.Cell(row, kv.Key + 1);
                        WriteObjectToCell(cell, GetValue(item, kv.Value));
                    }

                    SetFormula(ws, row, 19, $"=ROUND(R{row}/Q{row}%,1)");
                    SetFormula(ws, row, 21, $"=ROUND(T{row}/Q{row}%,1)");
                    SetFormula(ws, row, 23, $"=ROUND(V{row}/Q{row}%,1)");
                    SetFormula(ws, row, 25, $"=ROUND(X{row}/Q{row}%,1)");
                    SetFormula(ws, row, 27, $"=ROUND((Z{row}/Q{row}*100),2)");

                    row++;
                    stt++;
                }

                // Summary row for the year group
                ws.Row(row).InsertRowsBelow(1);
                ws.Cell(row, 8).Value = $"Cộng {plantingYear}";
                ws.Cell(row, 8).Style.Font.Bold = true;

                var startRow = row - yearItems.Count;
                var endRow = row - 1;

                SetFormula(ws, row, 16, $"=SUM(P{startRow}:P{endRow})");
                SetFormula(ws, row, 17, $"=SUM(Q{startRow}:Q{endRow})");
                SetFormula(ws, row, 18, $"=SUM(R{startRow}:R{endRow})");
                SetFormula(ws, row, 19, $"=ROUND(AVERAGE(S{startRow}:S{endRow}),1)");
                SetFormula(ws, row, 20, $"=SUM(T{startRow}:T{endRow})");
                SetFormula(ws, row, 21, $"=ROUND(AVERAGE(U{startRow}:U{endRow}),1)");
                SetFormula(ws, row, 22, $"=SUM(V{startRow}:V{endRow})");
                SetFormula(ws, row, 23, $"=ROUND(AVERAGE(W{startRow}:W{endRow}),1)");
                SetFormula(ws, row, 24, $"=SUM(X{startRow}:X{endRow})");
                SetFormula(ws, row, 25, $"=ROUND(AVERAGE(Y{startRow}:Y{endRow}),1)");
                SetFormula(ws, row, 26, $"=SUM(Z{startRow}:Z{endRow})");
                SetFormula(ws, row, 27, $"=ROUND(AVERAGE(AA{startRow}:AA{endRow}),1)");
                SetFormula(ws, row, 28, $"=ROUND(AVERAGE(AB{startRow}:AB{endRow}),1)");
                SetFormula(ws, row, 34, $"=ROUND(AVERAGE(AH{startRow}:AH{endRow}),1)");
                SetFormula(ws, row, 35, $"=ROUND(AVERAGE(AI{startRow}:AI{endRow}),1)");
                SetFormula(ws, row, 36, $"=ROUND(AVERAGE(AJ{startRow}:AJ{endRow}),1)");

                var summaryRow = ws.Row(row);
                summaryRow.Style.Font.Bold = true;
                summaryRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                row++;
            }
        }

        private void ProcessTXASheet(XLWorkbook workbook, List<M_SurveyFarm> itemsList)
        {
            var ws = workbook.Worksheet("1.TXA");
            if (ws == null) return;

            var mapTXAFields = new Dictionary<int, string>
            {
                { 1, "idPrivate" },
                { 2, "plotName" },
                { 3, "yearOfPlanting" },
                { 4, "plantingDistanceObj.code" },
                { 5, "plantingDesignDensity" },
                { 6, "area" },
                { 7, "intercropName" },
                { 8, "intercroppingYear" },
                { 9, "intercroppingArea" },
                { 10, "careContract" },
                { 11, "productContract" },
                { 12, "financialIncome" },
                { 13, "intercroppingOther" },
                { 14, "intercroppingCompany" },
                { 15, "noContribEcon" },
                { 16, "noContribPers" },
                { 17, "partContribEcon" },
                { 18, "partContribPers" },
                { 19, "shavingTreeDensity" },
                { 20, "vanhAverage" },
                { 21, "ratioTreeObtain" },
                { 22, "classifyObj.code" },
            };

            var txaItems = itemsList.Where(item =>
            {
                string active = item.ActiveStatusObj?.Code;
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
                return active == "TX" && (item.IntercropType ?? 0) == 0;
            }).ToList();

            int row = 10;
            int stt = 1;

            foreach (var item in txaItems)
            {
                ws.Row(row).InsertRowsBelow(1);
                var sttCell = ws.Cell(row, 1);
                sttCell.Value = stt;
                sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sttCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                foreach (var kv in mapTXAFields)
                {
                    var cell = ws.Cell(row, kv.Key + 1);
                    WriteObjectToCell(cell, GetValue(item, kv.Value));
                }

                row++;
                stt++;
            }
        }

        private void ProcessTXBSheet(XLWorkbook workbook, List<M_SurveyFarm> itemsList)
        {
            var ws = workbook.Worksheet("1.TXB");
            if (ws == null) return;

            var mapTXBFields = new Dictionary<int, string>
            {
                { 1, "idPrivate" },
                { 2, "plotName" },
                { 3, "yearOfPlanting" },
                { 4, "plantingDistanceObj.code" },
                { 5, "plantingDesignDensity" },
                { 6, "area" },
                { 7, "intercropName" },
                { 8, "intercroppingYear" },
                { 9, "intercroppingArea" },
                { 10, "careContract" },
                { 11, "productContract" },
                { 12, "financialIncome" },
                { 13, "intercroppingOther" },
                { 14, "intercroppingCompany" },
                { 15, "noContribEcon" },
                { 16, "noContribPers" },
                { 17, "partContribEcon" },
                { 18, "partContribPers" },
                { 19, "shavingTreeDensity" },
                { 20, "vanhAverage" },
                { 21, "ratioTreeObtain" },
            };

            var txbItems = itemsList.Where(item =>
            {
                string? active = item.ActiveStatusObj?.Code;
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
                return active == "TX" && (item.IntercropType ?? 0) == 1;
            }).ToList();

            int row = 10;
            int stt = 1;

            foreach (var item in txbItems)
            {
                ws.Row(row).InsertRowsBelow(1);
                var sttCell = ws.Cell(row, 1);
                sttCell.Value = stt;
                sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sttCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                foreach (var kv in mapTXBFields)
                {
                    var cell = ws.Cell(row, kv.Key + 1);
                    WriteObjectToCell(cell, GetValue(item, kv.Value));
                }

                row++;
                stt++;
            }
        }

        private void ProcessSummarySheet(XLWorkbook workbook, List<M_SurveyFarm> itemsList)
        {
            try
            {
                var ws = workbook.Worksheet(1);
                if (ws == null) return;

                int row = 13;
                foreach (var item in itemsList)
                {
                    if (!string.IsNullOrWhiteSpace(item.IdPrivate))
                    {
                        ws.Cell(row, 2).Value = item.IdPrivate;
                        row++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing summary sheet: {ex.Message}");
            }
        }

        private void Process2aKDSheet(XLWorkbook workbook, List<M_SurveyFarmBusinessFarm> placeMarkKDByYearList)
        {
            var ws = workbook.Worksheet("2a.KD");
            if (ws == null) return;

            var map2aKDFields = new Dictionary<int, string>
            {
                { 1, "tappingAge" },
                { 2, "period" },
                { 3, "areaOld" },
                { 4, "area" },
                { 5, "treeQuantity" },
                { 6, "effectiveTreeShavingQuantity" },
                { 7, "shavingTreeDensity" },
                { 8, "effectiveTreeDensity" },
                { 9, "totalShavingSlice" },
                { 10, "productivityByTree" },
                { 11, "productivityByArea" },
                { 12, "remark" },
            };

            int row = 9;
            int stt = 1;

            foreach (var item in placeMarkKDByYearList)
            {
                ws.Row(row).InsertRowsBelow(1);
                var sttCell = ws.Cell(row, 1);
                sttCell.Value = stt;
                sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sttCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                foreach (var kv in map2aKDFields)
                {
                    var cell = ws.Cell(row, kv.Key + 1);
                    WriteObjectToCell(cell, GetValue(item, kv.Value));
                }

                row++;
                stt++;
            }
        }

        private void Process2bKDSheet(XLWorkbook workbook, List<M_SurveyFarmBusinessFarmByFarmGroup> placeMarkKDByFarmGroupList)
        {
            var ws = workbook.Worksheet("2b.KD");
            if (ws == null) return;

            var map2bKDFields = new Dictionary<int, string>
            {
                { 1, "farmName" },
                { 2, "areaOld" },
                { 3, "area" },
                { 4, "treeQuantity" },
                { 5, "effectiveTreeShavingQuantity" },
                { 6, "shavingTreeDensity" },
                { 7, "effectiveTreeDensity" },
                { 8, "totalShavingSlice" },
                { 9, "actualLaborQuantity" },
                { 10, "laborProductivity" },
                { 11, "productivityByTree" },
                { 12, "productivityByArea" },
                { 13, "remark" },
            };

            int row = 9;
            int stt = 1;

            foreach (var item in placeMarkKDByFarmGroupList)
            {
                ws.Row(row).InsertRowsBelow(1);
                var sttCell = ws.Cell(row, 1);
                sttCell.Value = stt;
                sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sttCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                foreach (var kv in map2bKDFields)
                {
                    var cell = ws.Cell(row, kv.Key + 1);
                    WriteObjectToCell(cell, GetValue(item, kv.Value));
                }

                row++;
                stt++;
            }
        }

        private void Process3KTCBSheet(XLWorkbook workbook, List<M_SurveyFarmKTCB> placeMarkKTCBList)
        {
            var ws = workbook.Worksheet("3.KTCB.MC");
            if (ws == null) return;

            var map3KTCBFields = new Dictionary<int, string>
            {
                { 1, "idPrivate" },
                { 2, "farmCode" },
                { 3, "farmGroupCode" },
                { 4, "plotName" },
                { 6, "yearOfPlanting" },
                { 7, "landLevelCode" },
                { 8, "altitudeAverage" },
                { 9, "plantingMethodCode" },
                { 10, "plantingDistanceCode" },
                { 11, "typeOfTreeCode" },
                { 12, "area" },
                { 13, "treeQuantity" },
                { 14, "effectiveTreeQuantity" },
                { 15, "vanh_50" },
                { 16, "vanh_45_49" },
                { 17, "vanh_40_44" },
                { 18, "vanh_35_39" },
                { 19, "vanh_34" },
                { 20, "barkThickness" },
                { 21, "ratioTreeObtain_50" },
                { 22, "ratioTreeObtain_45_49" },
                { 23, "totalVanh" },
                { 24, "effectiveTreeDensity" },
                { 25, "productivityByArea" },
            };

            int row = 12;

            foreach (var item in placeMarkKTCBList)
            {
                ws.Row(row).InsertRowsBelow(1);

                foreach (var kv in map3KTCBFields)
                {
                    var cell = ws.Cell(row, kv.Key);
                    WriteObjectToCell(cell, GetValue(item, kv.Value));
                }

                row++;
            }
        }

        private void Process3aKDSheet(XLWorkbook workbook, List<M_SurveyFarmTypeOfTree> placeMarkKDByTypeOfTreeList)
        {
            var ws = workbook.Worksheet("3a.KD");
            if (ws == null) return;
        }

        private void Process3bKDSheet(XLWorkbook workbook, List<M_SurveyFarmKDLandLevel> placeMarkKDByLandLevelList)
        {
            var ws = workbook.Worksheet("3b.KD");
            if (ws == null) return;
        }

        private void Process4KDSheet(XLWorkbook workbook, List<M_SurveyFarmKDGardenRating> placeMarkKDByGardenRatingList)
        {
            var ws = workbook.Worksheet("4.KD");
            if (ws == null) return;
        }

        private void Process2aKTCBSheet(XLWorkbook workbook, List<M_SurveyFarmKTCBGardenRating> placeMarkKTCBByGardenRatingYearList)
        {
            var ws = workbook.Worksheet("2a.KTCB");
            if (ws == null) return;
        }

        private void Process2bKTCBSheet(XLWorkbook workbook, List<M_SurveyFarmKTCBGardenRating> placeMarkKTCBByGardenRatingFarmGroupList)
        {
            var ws = workbook.Worksheet("2b.KTCB");
            if (ws == null) return;
        }

        private void AutoResizeColumns(XLWorkbook workbook)
        {
            foreach (var ws in workbook.Worksheets)
            {
                Dictionary<int, string>? mapFields = ws.Name switch
                {
                    "1.TM-TC" or "1.TC-TM" => new Dictionary<int, string>
                    {
                        { 1, "idPrivate" }, { 5, "plotName" }, { 6, "landLevelObj.code" }, { 7, "altitudeAverage" },
                        { 8, "plantingMethodObj.code" }, { 9, "plantingDistanceObj.code" }, { 10, "plantingDesignDensity" },
                        { 11, "TypeOfTreeObj.code" }, { 12, "area" }, { 13, "holeQuantity" }, { 14, "graftedTreeCorrectQuantity" },
                        { 16, "graftedTreeMixedQuantity" }, { 18, "emptyHoleQuantity" }, { 20, "densityOfGraftedTree" },
                        { 21, "averageNumberLeafLayer" }, { 22, "classifyObj.code"}, { 23, "plantingEndDate" }, { 24, "remark" }
                    },
                    "1.KTCB" => new Dictionary<int, string>
                    {
                        { 1, "idPrivate" }, { 5, "plotOldName" }, { 6, "plotNewName" }, { 7, "yearOfPlanting" },
                        { 8, "landLevelObj.code" }, { 9, "altitudeAverage" }, { 10, "plantingMethodObj.code" },
                        { 11, "plantingDistanceObj.code" }, { 12, "plantingDesignDensity" }, { 13, "TypeOfTreeObj.code" },
                        { 15, "area" }, { 16, "holeQuantity" }, { 17, "effectiveTreeCorrectQuantity" },
                        { 19, "effectiveTreeMixedQuantity" }, { 21, "ineffectiveTreeNotgrowQuantity" }, { 23, "emptyHoleQuantity" },
                        { 25, "effectiveTreeDensity" }, { 27, "vanhAverage" }, { 28, "standardDeviation" },
                        { 29, "ratioTreeObtain" }, { 31, "expectedExploitationDate" }, { 32, "gardenRatingObj.code" }, { 33, "remark" }
                    },
                    "1.KD" => new Dictionary<int, string>
                    {
                        { 1, "idPrivate" }, { 5, "plotOldName" }, { 6, "plotNewName" }, { 7, "yearOfPlanting" },
                        { 8, "landLevelObj.code" }, { 9, "altitudeAverage" }, { 10, "plantingMethodObj.code" },
                        { 11, "plantingDistanceObj.code" }, { 12, "plantingDesignDensity" }, { 13, "TypeOfTreeObj.code" },
                        { 15, "area" }, { 16, "holeQuantity" }, { 17, "effectiveTreeShavingQuantity" },
                        { 19, "effectiveTreeNotshavingQuantity" }, { 21, "ineffectiveTreeDryQuantity" },
                        { 23, "ineffectiveTreeNotgrowQuantity" }, { 25, "emptyHoleQuantity" }, { 27, "shavingTreeDensity" },
                        { 28, "shavingModeCode" }, { 29, "startExploitationDate" }, { 30, "tappingAge" },
                        { 31, "yearOfShaving" }, { 32, "shavingFaceConditionCode" }, { 33, "productivityByArea" },
                        { 34, "productivityByTree" }, { 35, "totalShavingSlice" }, { 36, "gardenRatingObj.code" }, { 37, "remark" }
                    },
                    "1.TXA" => new Dictionary<int, string>
                    {
                        { 1, "idPrivate" }, { 2, "plotName" }, { 3, "yearOfPlanting" }, { 4, "plantingDistanceObj.code" },
                        { 5, "plantingDesignDensity" }, { 6, "area" }, { 7, "intercropName" }, { 8, "intercroppingYear" },
                        { 9, "intercroppingArea" }, { 10, "careContract" }, { 11, "productContract" }, { 12, "financialIncome" },
                        { 13, "intercroppingOther" }, { 14, "intercroppingCompany" }, { 15, "noContribEcon" },
                        { 16, "noContribPers" }, { 17, "partContribEcon" }, { 18, "partContribPers" },
                        { 19, "shavingTreeDensity" }, { 20, "vanhAverage" }, { 21, "ratioTreeObtain" }, { 22, "classifyObj.code" }
                    },
                    "1.TXB" => new Dictionary<int, string>
                    {
                        { 1, "idPrivate" }, { 2, "plotName" }, { 3, "yearOfPlanting" }, { 4, "plantingDistanceObj.code" },
                        { 5, "plantingDesignDensity" }, { 6, "area" }, { 7, "intercropName" }, { 8, "intercroppingYear" },
                        { 9, "intercroppingArea" }, { 10, "careContract" }, { 11, "productContract" }, { 12, "financialIncome" },
                        { 13, "intercroppingOther" }, { 14, "intercroppingCompany" }, { 15, "noContribEcon" },
                        { 16, "noContribPers" }, { 17, "partContribEcon" }, { 18, "partContribPers" },
                        { 19, "shavingTreeDensity" }, { 20, "vanhAverage" }, { 21, "ratioTreeObtain" }
                    },
                    _ => null
                };

                if (mapFields != null)
                {
                    foreach (var colIndex in mapFields.Keys)
                    {
                        var col = ws.Column(colIndex + 1);
                        double maxWidth = 15;

                        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 10;
                        for (int row = 10; row <= lastRow; row++)
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
        }

        private static string GetActiveStatus(M_SurveyFarm item)
        {
            string active = item.ActiveStatusObj?.Code;
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
            return active;
        }

        private static void WriteObjectToCell(IXLCell cell, object? value)
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

        private static object? GetValue(object item, string propertyName)
        {
            if (propertyName.Contains('.'))
            {
                var parts = propertyName.Split('.');
                object? currentObj = item;

                foreach (var part in parts)
                {
                    if (currentObj == null) return null;

                    var prop = currentObj.GetType().GetProperty(part, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
                    if (prop == null) return null;

                    currentObj = prop.GetValue(currentObj);
                }

                return currentObj;
            }
            else
            {
                var prop = item.GetType().GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
                return prop?.GetValue(item);
            }
        }

        private static void SetFormula(IXLWorksheet ws, int row, int col, string formula)
        {
            var formulaCell = ws.Cell(row, col);
            formulaCell.FormulaA1 = formula;
            formulaCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            formulaCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            formulaCell.Style.NumberFormat.Format = "#,##0";
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