using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Web_ProjectName.Lib;
using ClosedXML.Excel;
using System.Threading;
using System.Text.Json;
using Web_ProjectName.Services;
using Web_ProjectName.Models;

namespace Web_ProjectName.Controllers
{
    public class TableController : BaseController<TableController>
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IS_DiaryTree _diaryTreeService;
        public TableController(IWebHostEnvironment webHostEnvironment, IS_DiaryTree diaryTreeService)
        {
            _webHostEnvironment = webHostEnvironment;
            _diaryTreeService = diaryTreeService;
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
                            var ws = workbook.Worksheet("1.TM-TC") ?? workbook.Worksheet("1.TC-TM");
                            ws.Row(rowTMTC).InsertRowsBelow(1);
                            var sttCell = ws.Cell(rowTMTC, 1);
                            sttCell.Value = sttTMTC;
                            sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            sttCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            foreach (var kv in mapTMTCFields)
                            {
                                if (item.TryGetProperty(kv.Value, out var v))
                                {
                                    var cell = ws.Cell(rowTMTC, kv.Key + 1);
                                    WriteJsonElementToCell(cell, v);
                                }
                            }
                            SetFormula(ws, rowTMTC, 15 + 1, $"=ROUND(O{rowTMTC}/N{rowTMTC}%,1)");
                            SetFormula(ws, rowTMTC, 17 + 1, $"=ROUND(Q{rowTMTC}/N{rowTMTC}%,1)");
                            SetFormula(ws, rowTMTC, 19 + 1, $"=ROUND(S{rowTMTC}/N{rowTMTC}%,1)");
                            rowTMTC++;
                            sttTMTC++;
                        }
                        else if (active == 5)
                        {
                            var ws = workbook.Worksheet("1.KTCB");
                            ws.Row(rowKTCB).InsertRowsBelow(1);
                            var sttCell = ws.Cell(rowKTCB, 1);
                            sttCell.Value = sttKTCB;
                            sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            sttCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            foreach (var kv in mapKTCBFields)
                            {
                                if (item.TryGetProperty(kv.Value, out var v))
                                {
                                    var cell = ws.Cell(rowKTCB, kv.Key + 1);
                                    WriteJsonElementToCell(cell, v);
                                }
                            }
                            SetFormula(ws, rowKTCB, 19, $"=ROUND(R{rowKTCB}/Q{rowKTCB}%,1)");
                            SetFormula(ws, rowKTCB, 21, $"=ROUND(T{rowKTCB}/Q{rowKTCB}%,1)");
                            SetFormula(ws, rowKTCB, 23, $"=ROUND(V{rowKTCB}/Q{rowKTCB}%,1)");
                            SetFormula(ws, rowKTCB, 25, $"=ROUND(X{rowKTCB}/Q{rowKTCB}%,1)");
                            // special column 30: markedExtendedGarden -> "x" if truthy
                            var markedX = "";
                            if (item.TryGetProperty("markedExtendedGarden", out var markEl))
                            {
                                if (markEl.ValueKind == JsonValueKind.True || (markEl.ValueKind == JsonValueKind.Number && markEl.TryGetInt32(out var mi) && mi != 0))
                                    markedX = "x";
                            }
                            ws.Cell(rowKTCB, 30 + 1).Value = markedX;
                            ws.Cell(rowKTCB, 30 + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(rowKTCB, 30 + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            rowKTCB++;
                            sttKTCB++;
                        }
                        else if (active == 6)
                        {
                            var ws = workbook.Worksheet("1.KD");
                            ws.Row(rowKD).InsertRowsBelow(1);
                            var sttCell = ws.Cell(rowKD, 1);
                            sttCell.Value = sttKD;
                            sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            sttCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            foreach (var kv in mapKDFields)
                            {
                                if (item.TryGetProperty(kv.Value, out var v))
                                {
                                    var cell = ws.Cell(rowKD, kv.Key + 1);
                                    WriteJsonElementToCell(cell, v);
                                }
                            }
                            SetFormula(ws, rowKD, 19, $"=ROUND(R{rowKD}/Q{rowKD}%,1)");
                            SetFormula(ws, rowKD, 21, $"=ROUND(T{rowKD}/Q{rowKD}%,1)");
                            SetFormula(ws, rowKD, 23, $"=ROUND(V{rowKD}/Q{rowKD}%,1)");
                            SetFormula(ws, rowKD, 25, $"=ROUND(X{rowKD}/Q{rowKD}%,1)");
                            SetFormula(ws, rowKD, 27, $"=ROUND((Z{rowKD}/Q{rowKD}*100),2)");
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
                    foreach (var ws in workbook.Worksheets)
                    {
                        int headerRow = 10;
                        var lastRow = ws.LastRowUsed();

                        if (lastRow != null)
                        {
                            int lastRowNumber = lastRow.RowNumber();

                            ws.Columns().AdjustToContents(headerRow, lastRowNumber);

                            foreach (var col in ws.ColumnsUsed())
                            {
                                if (col.Width < 15)
                                    col.Width = 15;
                            }
                        }
                        else
                        {
                            ws.Columns().AdjustToContents(headerRow, headerRow);
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
    }
}