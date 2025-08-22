using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Web_ProjectName.Lib;

namespace Web_ProjectName.Controllers
{
    public class TableController : BaseController<TableController>
    {
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

                            int rowStart = 8;

                            if (reader.Name.Equals("1.KD", StringComparison.OrdinalIgnoreCase))
                            {
                                rowStart = 11;
                            }
                            else if (reader.Name.Equals("1.KTCB", StringComparison.OrdinalIgnoreCase))
                            {
                                rowStart = 9;
                            }
                            else if (reader.Name.Equals("1.TC-TM", StringComparison.OrdinalIgnoreCase) || reader.Name.Equals("1.TM-TC", StringComparison.OrdinalIgnoreCase))
                            {
                                rowStart = 8;
                            }

                            while (reader.Read())
                            {
                                if (reader.Depth >= rowStart)
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
                                    if (reader.Name.Equals("1.TM-TC", StringComparison.OrdinalIgnoreCase) || reader.Name.Equals("1.TC-TM", StringComparison.OrdinalIgnoreCase))
                                    {
                                        data.Add(new
                                        {
                                            SurveyBatchId = 0,
                                            LandLevelCode = "",
                                            OwnerId = 0,
                                            CultivatorId = 0,
                                            PlotId = idPrivate,
                                            IdPrivate = idPrivate,
                                            RiskLevel = "",
                                            PlotOldName = "",
                                            PlotNewName = "",
                                            PlotName = GetCell(5)?.ToString() ?? "",
                                            LandType = GetCell(6)?.ToString() ?? "",
                                            AreaCultivated = 0.0f,
                                            Area = 0.0f,
                                            Area1 = 0.0f,
                                            Area2 = 0.0f,
                                            Area3 = 0.0f,
                                            AltitudeLowest = TryDouble(GetCell(7)),
                                            AltitudeHighest = TryDouble(GetCell(8)),
                                            AreaManagementChange = TryDouble(GetCell(13)),
                                            ClassifyCode = GetCell(23)?.ToString() ?? "",
                                            AverageHeight = 0,
                                            ActiveStatusId = 14,
                                            TypeOfTreeCode = GetCell(12)?.ToString() ?? "",
                                            PlantingMethodCode = GetCell(9)?.ToString() ?? "",
                                            PlantingDistanceCode = GetCell(10)?.ToString() ?? "",
                                            PlantingDesignDensity = TryDouble(GetCell(11)),
                                            Hecta = 0,
                                            HoleQuantity = TryInt(GetCell(14)),
                                            GraftedTreeCorrectQuantity = TryInt(GetCell(15)),
                                            GraftedTreeMixedQuantity = TryInt(GetCell(17)),
                                            EmptyHoleQuantity = TryInt(GetCell(19)),
                                            DensityOfGraftedTree = TryDouble(GetCell(21)),
                                            AverageNumberLeafLayer = TryInt(GetCell(22)),
                                            PlantingEndDate = TryDate(GetCell(24))?.ToString("yyyy/MM/dd") ?? GetCell(24)?.ToString() ?? "",
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
                                            MarkedExtendedGarden = 0,
                                            ExpectedExploitationDate = "",
                                            YearOfPlanting = "",
                                            YearOfShaving = "",
                                            Remark = GetCell(25)?.ToString() ?? "",
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
                                            SurveyBatchId = 0,
                                            LandLevelCode = GetCell(8)?.ToString() ?? "",
                                            OwnerId = 0,
                                            CultivatorId = 0,
                                            PlotId = idPrivate,
                                            IdPrivate = idPrivate,
                                            RiskLevel = "",
                                            PlotOldName = GetCell(5)?.ToString() ?? "",
                                            PlotNewName = GetCell(6)?.ToString() ?? "",
                                            PlotName = "",
                                            LandType = "",
                                            AreaCultivated = 0.0f,
                                            Area = 0.0f,
                                            Area1 = 0.0f,
                                            Area2 = 0.0f,
                                            Area3 = 0.0f,
                                            AltitudeLowest = 0.0f,
                                            AltitudeHighest = 0.0f,
                                            AreaManagementChange = TryDouble(GetCell(14)),
                                            // AreaAt2025 = TryDouble(GetCell(15)),
                                            ClassifyCode = GetCell(32)?.ToString() ?? "",
                                            AverageHeight = TryDouble(GetCell(9)),
                                            ActiveStatusId = 5,
                                            TypeOfTreeCode = GetCell(13)?.ToString() ?? "",
                                            PlantingMethodCode = GetCell(10)?.ToString() ?? "",
                                            PlantingDistanceCode = GetCell(11)?.ToString() ?? "",
                                            PlantingDesignDensity = TryDouble(GetCell(12)),
                                            Hecta = 0,
                                            HoleQuantity = TryInt(GetCell(16)),
                                            GraftedTreeCorrectQuantity = TryInt(GetCell(17)),
                                            GraftedTreeMixedQuantity = TryInt(GetCell(19)),
                                            EmptyHoleQuantity = TryInt(GetCell(23)),
                                            DensityOfGraftedTree = 0.0f,
                                            AverageNumberLeafLayer = 0,
                                            PlantingEndDate = "",
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
                                            IneffectiveTreeQuantity = TryInt(GetCell(21)),
                                            EffectiveTreeCorrectQuantity = TryInt(GetCell(17)),
                                            EffectiveTreeMixedQuantity = TryInt(GetCell(19)),
                                            EffectiveTreeDensity = TryDouble(GetCell(25)),
                                            StandardDeviation = TryInt(GetCell(28)),
                                            RatioTreeObtain = TryInt(GetCell(29)),
                                            MarkedExtendedGarden = TryInt(GetCell(30)),
                                            ExpectedExploitationDate = TryDate(GetCell(31))?.ToString("yyyy-MM-dd") ?? GetCell(31)?.ToString() ?? "",
                                            YearOfPlanting = GetCell(7)?.ToString() ?? "",
                                            YearOfShaving = "",
                                            Remark = GetCell(33)?.ToString() ?? "",
                                            GardenRatingId = 0,
                                            VanhAverage = TryDouble(GetCell(27)),
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
                                            SurveyBatchId = 0,                           // ID đợt khảo sát
                                            LandLevelCode = GetCell(8)?.ToString() ?? "",                          // Hạng đất (mã phân hạng)
                                            OwnerId = 0,                                 // Chủ sở hữu (ID)
                                            CultivatorId = 0,                            // Người canh tác (ID)
                                            PlotId = idPrivate,                          // Mã lô đất
                                            IdPrivate = idPrivate,                       // ID nội bộ, private key
                                            RiskLevel = "",                              // Mức độ rủi ro
                                            PlotOldName = GetCell(5)?.ToString() ?? "",                            // Tên lô cũ
                                            PlotNewName = GetCell(6)?.ToString() ?? "",                            // Tên lô mới
                                            PlotName = "",                               // Tên lô hiện tại
                                            LandType = "",                               // Loại đất
                                            AreaCultivated = 0.0f,                       // Diện tích đã canh tác
                                            Area = 0.0f,                                 // Diện tích (tổng)
                                            Area1 = 0.0f,                                // Diện tích theo cách 1
                                            Area2 = 0.0f,                                // Diện tích theo cách 2
                                            Area3 = 0.0f,                                // Diện tích theo cách 3
                                            AltitudeLowest = 0.0f,                       // Cao độ thấp nhất
                                            AltitudeHighest = 0.0f,                      // Cao độ cao nhất
                                            AreaManagementChange = TryDouble(GetCell(14)), // Diện tích (1/1/2024)
                                            // AreaAt2025 = TryDouble(GetCell(15)),
                                            ClassifyCode = GetCell(36)?.ToString() ?? "",  // Xếp hạng
                                            AverageHeight = TryDouble(GetCell(9)),       // Cao trình trung bình (m)
                                            ActiveStatusId = 6,                          // Trạng thái hoạt động (mã)
                                            TypeOfTreeCode = GetCell(13)?.ToString() ?? "", // Giống cây trồng (mã)
                                            PlantingMethodCode = GetCell(10)?.ToString() ?? "", // Phương pháp trồng
                                            PlantingDistanceCode = GetCell(11)?.ToString() ?? "", // Khoảng cách trồng
                                            PlantingDesignDensity = TryDouble(GetCell(12)), // Mật độ thiết kế (cây/ha)
                                            Hecta = 0,                                   // Số hecta (nếu có)
                                            HoleQuantity = TryInt(GetCell(16)),          // Tổng số hố khảo sát
                                            GraftedTreeCorrectQuantity = 0, // SL cây ghép đúng giống
                                            GraftedTreeMixedQuantity = 0,   // SL cây ghép lẫn giống
                                            EmptyHoleQuantity = TryInt(GetCell(25)),     // Số hố trống
                                            DensityOfGraftedTree = 0, // Mật độ cây ghép
                                            AverageNumberLeafLayer = 0, // Số tầng lá trung bình
                                            PlantingEndDate = "", // Ngày kết thúc trồng
                                            ProductivityByArea = TryDouble(GetCell(33)),                      // Năng suất theo diện tích
                                            TreeQuantity = 0,                            // Tổng số cây
                                            TreeQuantityShaving = 0,                     // SL cây đã mở cạo
                                            EffectiveTreeNotshavingQuantity = TryInt(GetCell(19)),         // SL cây hữu hiệu chưa cạo
                                            EffectiveTreeShavingQuantity = TryInt(GetCell(17)),            // SL cây hữu hiệu đã cạo
                                            IneffectiveTreeNotgrowQuantity = TryInt(GetCell(23)),          // SL cây không phát triển
                                            IneffectiveTreeDryQuantity = TryInt(GetCell(21)),              // SL cây khô mủ
                                            ShavingTreeDensity = TryInt(GetCell(27)),                      // Mật độ cây cạo
                                            ShavingModeCode = GetCell(28)?.ToString() ?? "",                        // Chế độ cạo
                                            EndExploitationDate = "",                    // Ngày kết thúc khai thác
                                            StartExploitationDate = "",                  // Ngày bắt đầu khai thác
                                            TotalShavingSlice = TryInt(GetCell(35)),                       // Tổng số lát cạo
                                            ShavingFaceConditionId = 0,                  // ID tình trạng mặt cạo
                                            ShavingFaceConditionCode = GetCell(32)?.ToString() ?? "",               // Mã tình trạng mặt cạo
                                            TappingAge = TryInt(GetCell(30)),                              // Tuổi khai thác (năm cạo)
                                            ProductivityByTree = TryInt(GetCell(34)),                      // Năng suất theo cây
                                            IneffectiveTreeQuantity = 0,                 // Tổng SL cây không hiệu quả
                                            EffectiveTreeCorrectQuantity = 0,            // Tổng SL cây hữu hiệu đúng giống
                                            EffectiveTreeMixedQuantity = 0,              // Tổng SL cây hữu hiệu lẫn giống
                                            EffectiveTreeDensity = 0,                    // Mật độ cây hữu hiệu (cây/ha)
                                            StandardDeviation = 0,                       // Độ lệch chuẩn (vd: vanh)
                                            RatioTreeObtain = 0,                         // % cây đạt tiêu chuẩn
                                            MarkedExtendedGarden = 0,                    // Đánh dấu vườn kéo dài (0/1)
                                            ExpectedExploitationDate = GetCell(29)?.ToString() ?? "",               // Tháng mở cạo
                                            YearOfPlanting = GetCell(7)?.ToString() ?? "",                         // Năm trồng
                                            YearOfShaving = GetCell(31)?.ToString() ?? "",                          // Năm cạo úp
                                            Remark = GetCell(37)?.ToString() ?? "",      // Ghi chú
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
        public IActionResult ExportExcel(int? year, string? variety, string? ids)
        {
            return View();
        }
    }
}