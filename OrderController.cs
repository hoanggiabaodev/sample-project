using ClosedXML.Excel;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO.Compression;
using System.Security.Cryptography.Xml;
using System.Text;
using Web_VrgDauTieng.Controllers;
using Web_VrgDauTieng.ExtensionMethods;
using Web_VrgDauTieng.Lib;
using Web_VrgDauTieng.Models;
using Web_VrgDauTieng.Models.Common;
using Web_VrgDauTieng.Services;
using Web_VrgDauTieng.ViewModels;
using static System.String;
using static Web_VrgDauTieng.Lib.RolesData;

namespace Web_VrgDauTieng.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [PermissionAuthorize($"{Ro_Functions.ADMIN}.{Ro_Controllers.AD_ORDER}.{Ro_Actions.READ}")]
    public class OrderController : BaseControllerArea<OrderController>
    {
        private readonly IS_Order _s_Order;
        private readonly IS_TransportOrder _s_TransportOrder;
        private readonly IS_OrderItem _s_OrderItem;
        private readonly IOptions<Config_GeoTrackingOrderConfig> _config_GeoTrackingOrderConfig;
        private readonly IS_OrderVerifyLog _s_OrderVerifyLog;
        private readonly IS_OrderVerifyFail _s_OrderVerifyFail;
        private readonly IS_Supplier _s_Supplier;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrderController(IS_Order order, IS_TransportOrder transportOrder, IS_OrderItem orderItem, IOptions<Config_GeoTrackingOrderConfig> config_GeoTrackingOrderConfig, IS_OrderVerifyLog orderVerifyLog, IS_OrderVerifyFail orderVerifyFail, IS_Supplier supplier, IWebHostEnvironment webHostEnvironment)
        {
            _s_Order = order;
            _s_TransportOrder = transportOrder;
            _s_OrderItem = orderItem;
            _config_GeoTrackingOrderConfig = config_GeoTrackingOrderConfig;
            _s_OrderVerifyLog = orderVerifyLog;
            _s_OrderVerifyFail = orderVerifyFail;
            _s_Supplier = supplier;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            await SetDropDownCountry();
            await SetDropDownFactory(default);
            await SetDropdownCustomer(default);
            await SetDropDownCertificate(default);
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetList(string sequenceStatus, string searchText, int? factoryId, int? customerId, byte? type, byte? transportType, int? countryId, DateOnly? fDate, DateOnly? tDate)
        {
            var res = await _s_Order.GetListByFullParam(_accessToken, sequenceStatus, searchText, factoryId, customerId, type, transportType, countryId, default, fDate, tDate);
            return Json(new M_JResult(res));
        }

        [HttpGet]
        public async Task<JsonResult> GetCode(string code)
        {
            M_JResult jResult = new M_JResult();
            if (string.IsNullOrEmpty(code))
            {
                jResult.error.message = "Vui lòng chọn mã!";
                jResult.result = 0;
                return Json(new M_JResult(jResult));
            }
            var res = await _s_Order.GetByCode(_accessToken, code);
            return Json(jResult.MapData(res));
        }

        [HttpGet]
        public async Task<IActionResult> P_View(int id)
        {
            var res = await _s_Order.GetById(_accessToken, id);
            if (res.result == 1 && res.data != null)
            {
                var resHistory = await _s_Account.GetInfoUserCreatedUpdated(_accessToken, res.data.createdBy, res.data.updatedBy);
                ViewBag.History = resHistory;
                ViewBag.OrderStatus = res.data.status;
                return PartialView(res.data);
            }
            return Json(new M_JResult(res));
        }

        [PermissionAuthorize($"{Ro_Functions.ADMIN}.{Ro_Controllers.AD_ORDER}.{Ro_Actions.CREATE}")]
        [HttpGet]
        public async Task<IActionResult> P_Add()
        {
            await SetDropDownCountry();
            await SetDropDownFactory(default);
            await SetDropdownCustomer(default);
            await SetDropDownCertificate(default);
            return PartialView();
        }

        [PermissionAuthorize($"{Ro_Functions.ADMIN}.{Ro_Controllers.AD_ORDER}.{Ro_Actions.CREATE}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> P_Add(EM_Order model)
        {
            M_JResult jResult = new M_JResult();
            if (!ModelState.IsValid)
            {
                jResult.error = new error(0, DataAnnotationExtensionMethod.GetErrorMessage(ModelState));
                return Json(jResult);
            }
            model.status = 0;
            var res = await _s_Order.Create(_accessToken, model, _accountId);
            return Json(jResult.MapData(res));
        }

        [HttpGet]
        public async Task<IActionResult> P_Edit(int id)
        {
            var res = await _s_Order.GetById(_accessToken, id);
            if (res.result != 1 || res.data == null)
            {
                return Json(new M_JResult(res));
            }
            var getPass = await _s_Order.GetPassCode(_accessToken, id);
            ViewBag.Pass = Encryptor.Decrypt(getPass.data);
            await SetDropDownCountry(res.data.CountryObj?.id ?? null);
            await SetDropdownCustomer(res.data.CustomerObj?.Id ?? null);
            await SetDropDownCertificate(res.data.CertificateObj?.Id ?? null);
            await SetDropDownFactory(res.data.FactoryObj?.Id ?? null);
            return PartialView(_mapper.Map<EM_Order>(res.data));
        }

        [PermissionAuthorize($"{Ro_Functions.ADMIN}.{Ro_Controllers.AD_ORDER}.{Ro_Actions.UPDATE}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> P_Edit(EM_Order model)
        {
            M_JResult jResult = new M_JResult();
            if (!ModelState.IsValid)
            {
                jResult.error = new error(0, DataAnnotationExtensionMethod.GetErrorMessage(ModelState));
                return Json(jResult);
            }
            var res = await _s_Order.Update(_accessToken, model, _accountId);
            return Json(jResult.MapData(res));
        }

        [PermissionAuthorize($"{Ro_Functions.ADMIN}.{Ro_Controllers.AD_ORDER}.{Ro_Actions.DELETE}")]
        [HttpDelete]
        public async Task<JsonResult> Delete(int id)
        {
            M_JResult jResult = new M_JResult();
            var res = await _s_Order.Delete(_accessToken, id, _accountId);
            return Json(jResult.MapData(res));
        }

        [PermissionAuthorize($"{Ro_Functions.ADMIN}.{Ro_Controllers.AD_ORDER}.{Ro_Actions.UPDATE}")]
        [HttpPut]
        public async Task<JsonResult> ChangeStatus(int id, int status)
        {
            M_JResult jResult = new M_JResult();
            var res = await _s_Order.UpdateStatus(_accessToken, id, status, _accountId);
            return Json(jResult.MapData(res));
        }

        [PermissionAuthorize($"{Ro_Functions.ADMIN}.{Ro_Controllers.AD_ORDER}.{Ro_Actions.UPDATE}")]
        [HttpPut]
        public async Task<JsonResult> ChangeStatusList(string jsonId, int status)
        {
            M_JResult jResult = new M_JResult();
            if (IsNullOrEmpty(jsonId))
            {
                jResult.error = new error(0, "Please complete all information!");
                return Json(jResult);
            }
            var res = await _s_Order.UpdateStatusList(_accessToken, jsonId, status, _accountId);
            return Json(jResult.MapData(res));
        }

        [PermissionAuthorize($"{Ro_Functions.ADMIN}.{Ro_Controllers.AD_ORDER}.{Ro_Actions.UPDATE}")]
        [HttpPut]
        public async Task<JsonResult> GenerateQrCode(int id, bool isLogo, string pathLogo, int width, int height, int margin)
        {
            M_JResult jResult = new M_JResult();
            if (!(id > 0))
            {
                jResult.error = new error(0, "Không tìm thấy dữ liệu để tạo QR code, vui lòng thao tác đóng và mở lại form tạo QR Code");
                return Json(jResult);
            }
            var res = await _s_Order.GenerateQrCode(_accessToken, id, _accountId, isLogo, pathLogo, width, height, margin);
            return Json(jResult.MapData(res));
        }

        [PermissionAuthorize($"{Ro_Functions.ADMIN}.{Ro_Controllers.AD_ORDER}.{Ro_Actions.UPDATE}")]
        [HttpDelete]
        public async Task<JsonResult> DeleteQrCode(int id)
        {
            M_JResult jResult = new M_JResult();
            if (!(id > 0))
            {
                jResult.error = new error(0, "Không tìm thấy dữ liệu để xoá QR code, vui lòng thao tác đóng và mở lại form tạo QR Code");
                return Json(jResult);
            }
            var res = await _s_Order.DeleteQrCode(_accessToken, id, _accountId);
            return Json(jResult.MapData(res));
        }

        [HttpGet]
        public async Task<JsonResult> GetPassCode(int id)
        {
            M_JResult jResult = new M_JResult();
            var res = await _s_Order.GetPassCode(_accessToken, id);
            if (res.result == 1 && res.data != null)
            {
                jResult.result = 1;
                jResult.data = Encryptor.Decrypt(res.data);
                return Json(jResult);
            }
            return Json(jResult.MapData(res));
        }


        [HttpPost]
        public async Task<JsonResult> ExportExcel(int id)
        {
            var jResult = new M_JResult();
            try
            {
                var res = await _s_Order.TraceableById(_accessToken, id);
                if (res.result != 1)
                    return Json(jResult.MapData(res));
                var resSupplierInfo = await _s_Supplier.GetById(CommonConstants.SUPPLIER_ID);
                var geoTrackingOrder = _config_GeoTrackingOrderConfig.Value;
                if (res.data != null)
                {
                    var QrUrl = res.data.OrderQrUrl;
                    var SellerName = resSupplierInfo.data.Name;
                    var SellerAddress = resSupplierInfo.data.Address;
                    var orderCode = StringHelper.ToUrlClean(res.data.Code);
                    var data = res.data.PlaceMarkObjs;
                    var transportOrders = res.data.TransportOrderObjs;
                    var orders = res.data.OrderItemObjs;
                    var buyerName = res.data.CustomerObj?.Name;
                    var buyerAddress = res.data.CustomerObj?.Address;
                    var BuyerSI = res.data.ShippingInstruction;
                    var BuyerContractRef = res.data.BuyerContractRef;
                    var request = HttpContext.Request;
                    var domain = $"{request.Scheme}://{request.Host.Value}";
                    var fullLink = $"{domain}/traceable-order?code={orderCode.ToUpper()}";

                    var templatePath = Path.Combine(_webHostEnvironment.WebRootPath, "template/MauDuLieuDonHang.xlsx");
                    using (var workbook = new ClosedXML.Excel.XLWorkbook(templatePath))
                    {
                        var worksheet = workbook.Worksheet(1);
                        worksheet.Cell("C9").Value = SellerName;
                        worksheet.Cell("C10").Value = SellerAddress;
                        worksheet.Cell("G9").Value = buyerName;
                        worksheet.Cell("G10").Value = buyerAddress;
                        worksheet.Cell("G11").Value = BuyerSI;
                        worksheet.Cell("G12").Value = BuyerContractRef;
                        worksheet.Cell("K5").Value = orderCode.ToUpper();
                        worksheet.Cell("K7").Value = fullLink;

                        var uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsDir))
                        {
                            Directory.CreateDirectory(uploadsDir);
                        }

                        if (!string.IsNullOrEmpty(QrUrl) && QrUrl != "--")
                        {
                            var imageUrl = QrUrl;
                            var imagePath = Path.Combine(uploadsDir, "tempImage.png");

                            try
                            {
                                using (HttpClient client = new HttpClient())
                                {
                                    var response = await client.GetAsync(imageUrl);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        var imageBytes = await response.Content.ReadAsByteArrayAsync();
                                        await System.IO.File.WriteAllBytesAsync(imagePath, imageBytes);
                                    }
                                }

                                if (System.IO.File.Exists(imagePath))
                                {
                                    var image = worksheet.AddPicture(imagePath)
                                                         .MoveTo(worksheet.Cell("K1"))
                                                         .Scale(0.45);
                                }
                            }
                            catch (Exception) { }

                            worksheet.Columns().AdjustToContents();
                        }


                        int Order = 15;
                        if (orders != null && orders.Count > 0)
                        {
                            for (int i = 0; i < orders.Count; i++)
                            {
                                worksheet.Cell(Order, 1).Value = i + 1;
                                worksheet.Cell(Order, 2).Value = orders[i].ProductName;
                                worksheet.Cell(Order, 3).Value = orders[i].Quantity + " " + orders[i].UnitName;
                                Order++;
                            }
                        }
                        else
                        {
                            worksheet.Cell(Order, 1).Value = "Không có dữ liệu";
                            worksheet.Range($"A{Order}:C{Order}").Merge();
                            worksheet.Cell(Order, 1).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                            worksheet.Cell(Order, 1).Style.Font.Italic = true;
                            Order++;
                        }
                        var orderRange = worksheet.Range($"A13:C{Order - 1}");
                        orderRange.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                        orderRange.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                        Order += 2;

                        worksheet.Cell(Order, 1).Value = "PALLET DETAIL";
                        worksheet.Range($"A{Order}:E{Order}").Merge();
                        worksheet.Cell(Order, 1).Style.Font.Bold = true;
                        Order++;

                        worksheet.Cell(Order, 1).Value = "NO";
                        worksheet.Cell(Order, 2).Value = "CONT NUMBER";
                        worksheet.Cell(Order, 3).Value = "PALLET CODE";
                        worksheet.Cell(Order, 4).Value = "LOT";
                        worksheet.Cell(Order, 5).Value = "WEIGHT (Kg)";
                        worksheet.Range($"A{Order}:E{Order}").Style.Font.Bold = true;
                        Order++;

                        if (transportOrders != null && transportOrders.Count > 0)
                        {
                            int rowDetailOrder = Order;
                            int rowNumber = 1;
                            foreach (var order in transportOrders)
                            {
                                foreach (var pallet in order.PalletObjs)
                                {
                                    worksheet.Cell(rowDetailOrder, 1).Value = rowNumber++;
                                    worksheet.Cell(rowDetailOrder, 2).Value = order.ContNumber;
                                    worksheet.Cell(rowDetailOrder, 3).Value = pallet.Code;
                                    worksheet.Cell(rowDetailOrder, 4).Value = pallet.Lot;
                                    worksheet.Cell(rowDetailOrder, 5).Value = Math.Ceiling(pallet.Weight ?? 0);
                                    rowDetailOrder++;
                                }
                            }
                            var detailPalletRange = worksheet.Range($"A{Order - 2}:E{rowDetailOrder - 1}");
                            detailPalletRange.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                            detailPalletRange.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                            Order = rowDetailOrder;
                        }
                        else
                        {
                            worksheet.Cell(Order, 1).Value = "Không có dữ liệu";
                            var noDataRangePallet = worksheet.Range($"A{Order}:E{Order}");
                            noDataRangePallet.Merge();
                            noDataRangePallet.Style.Font.Italic = true;
                            noDataRangePallet.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                            noDataRangePallet.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                            noDataRangePallet.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                            Order++;
                        }
                        Order += 2;

                        // "CULTIVATION AREA IDENTIFICATION" Section
                        worksheet.Cell(Order, 1).Value = "CULTIVATION AREA IDENTIFICATION";
                        worksheet.Range($"A{Order}:L{Order}").Merge();
                        worksheet.Cell(Order, 1).Style.Font.Bold = true;
                        Order++;

                        // Column headers for "CULTIVATION AREA IDENTIFICATION" table
                        worksheet.Cell(Order, 1).Value = "NO";
                        worksheet.Cell(Order, 2).Value = "COUNTRY CODE";
                        worksheet.Cell(Order, 3).Value = "PLOT ID";
                        worksheet.Cell(Order, 4).Value = "CERTIFICATE";
                        worksheet.Cell(Order, 5).Value = "CLONE (VARIETY RUBBER)";
                        worksheet.Cell(Order, 6).Value = "AREA (HECTA)";
                        worksheet.Cell(Order, 7).Value = "YEAR PLANTED";
                        worksheet.Cell(Order, 8).Value = "LEGALITY CHECK";
                        worksheet.Cell(Order, 9).Value = "DEFORESTATION FREE ANALYSIS";
                        worksheet.Cell(Order, 10).Value = "ADDRESS";
                        worksheet.Cell(Order, 11).Value = "LATITUDE";
                        worksheet.Cell(Order, 12).Value = "LONGITUDE";

                        var headerRange = worksheet.Range($"A{Order - 1}:L{Order}");
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                        headerRange.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                        Order++;

                        int rowCultivationArea = Order;

                        // Populate "CULTIVATION AREA IDENTIFICATION" data
                        if (data != null && data.Count > 0)
                        {
                            for (int i = 0; i < data.Count; i++)
                            {
                                worksheet.Cell(rowCultivationArea, 1).Value = i + 1;
                                worksheet.Cell(rowCultivationArea, 2).Value = geoTrackingOrder.COUNTRY_CODE;
                                worksheet.Cell(rowCultivationArea, 3).Value = data[i].PlotId;
                                worksheet.Cell(rowCultivationArea, 4).Value = data[i].LandCertificateCode;
                                worksheet.Cell(rowCultivationArea, 5).Value = geoTrackingOrder.TREE_NAME;
                                worksheet.Cell(rowCultivationArea, 6).Value = data[i].Area / 10000;
                                worksheet.Cell(rowCultivationArea, 7).Value = data[i].YearOfPlanting;
                                worksheet.Cell(rowCultivationArea, 8).Value = "x";
                                worksheet.Cell(rowCultivationArea, 9).Value = "x";
                                worksheet.Cell(rowCultivationArea, 10).Value = $"{data[i].District}, {data[i].Commune}, {data[i].Province}";
                                worksheet.Cell(rowCultivationArea, 11).Value = data[i].Latitude;
                                worksheet.Cell(rowCultivationArea, 12).Value = data[i].Longitude;
                                rowCultivationArea++;
                            }
                        }
                        else
                        {
                            // Nếu không có dữ liệu
                            worksheet.Cell(rowCultivationArea, 1).Value = "Không có dữ liệu";
                            var noDataRange = worksheet.Range($"A{rowCultivationArea}:L{rowCultivationArea}");
                            noDataRange.Merge();
                            noDataRange.Style.Font.Italic = true;
                            noDataRange.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                            rowCultivationArea++;
                        }

                        var cultivationDataRange = worksheet.Range($"A{Order - 1}:L{rowCultivationArea - 1}");
                        cultivationDataRange.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                        cultivationDataRange.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                        worksheet.Columns().AdjustToContents();
                        var timestamp = Utilities.CurrentTimeSeconds();
                        var fileName = $"mau1_{orderCode.ToUpper()}_{timestamp}.xlsx";
                        var filePathExport = Path.Combine(_webHostEnvironment.WebRootPath, "exports", fileName);

                        workbook.SaveAs(filePathExport);

                        jResult.result = 1;
                        jResult.data = $"/exports/{fileName}";

                        var threadDeleteFile = new Thread(() =>
                        {
                            try
                            {
                                Thread.Sleep(5000);
                                if (System.IO.File.Exists(filePathExport))
                                {
                                    System.IO.File.Delete(filePathExport);
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
            }
            catch (Exception ex)
            {
                jResult.result = -1;
                jResult.error.code = 500;
                jResult.error.message = $"Invalid data. Error details: {ex.Message}";
            }
            return Json(jResult);
        }

    }
}
