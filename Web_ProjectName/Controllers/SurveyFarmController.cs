using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Globalization;
using System.Numerics;
using System.Text;
using Web_VrgTayNinh.Controllers;
using Web_VrgTayNinh.EditModels;
using Web_VrgTayNinh.ExtensionMethods;
using Web_VrgTayNinh.Lib;
using Web_VrgTayNinh.Models;
using Web_VrgTayNinh.Services;
using Web_VrgTayNinh.ViewModels;
using static Web_VrgTayNinh.Lib.RolesData;
using static Web_VrgTayNinh.ViewModels.VM_DataTableFilter;

namespace Web_VrgTayNinh.Areas.TreeDiary.Controllers
{
    [Area("TreeDiary")]
    [Authorize(Roles = Ro_Functions.TREEDIARY)]
    [PermissionAuthorize($"{Ro_Functions.TREEDIARY}.{Ro_Controllers.TD_SURVEY_FARM}.{Ro_Actions.READ}")]
    public class SurveyFarmController : BaseControllerArea<SurveyFarmController>
    {
        private readonly IS_SurveyFarm _s_SurveyFarm;
        private readonly IS_PlaceMark _s_PlaceMark;
        private readonly IS_Farmer _s_Farmer;
        private readonly IS_SurveyTree _s_SurveyTree;
        private readonly IS_SurveyBatch _s_SurveyBatch;
        private readonly IS_ManagedAccount _s_ManagedAccount;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _iHttpContextAccessor;
        private readonly IS_LocalisationViewInject _s_LocalisationViewInject;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SurveyFarmController(IS_SurveyFarm surveyFarm, IS_PlaceMark placeMark, IS_Farmer farmer, IS_SurveyTree surveyTree, IS_SurveyBatch surveyBatch, IS_ManagedAccount managedAccount, IConfiguration configuration, IHttpContextAccessor iHttpContextAccessor, IWebHostEnvironment webHostEnvironment, IS_LocalisationViewInject localisationViewInject)
        {
            _s_SurveyFarm = surveyFarm;
            _s_PlaceMark = placeMark;
            _s_Farmer = farmer;
            _s_SurveyTree = surveyTree;
            _s_SurveyBatch = surveyBatch;
            _s_ManagedAccount = managedAccount;
            _configuration = configuration;
            _iHttpContextAccessor = iHttpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _s_LocalisationViewInject = localisationViewInject;
        }

        public async Task<IActionResult> Index()
        {
            await SetDropDownProvinceArea();
            await SetDropDownProvince(0);
            await SetDropDownLandCertificate(0);
            await SetDropDownSurveyBatch(0);
            await SetDropDownTypeOfTree(0);
            await SetDropDownActiveStatus(0);

            ViewData["KeyGoogleMap"] = _configuration.GetValue<string>("KeyGoogleMap");
            ViewData["KeyGoongMap"] = _configuration.GetValue<string>("KeyGoongMap");
            return View();
        }

        public async Task<IActionResult> Map()
        {
            await SetDropDownProvince(0);
            await SetDropDownSurveyBatch(0);
            //SetDropDownPlaceMarkType();

            //var resSurveyBatch = await _s_SurveyBatch.GetLastActive(_accessToken);
            //ViewBag.SurveyBatchId = resSurveyBatch.data?.Id ?? 0;
            ViewData["KeyGoogleMap"] = _configuration.GetValue<string>("KeyGoogleMap");
            return View();
        }

        public async Task<IActionResult> MapGoong()
        {
            await SetDropDownProvince(0);
            await SetDropDownSurveyBatch(0);
            //SetDropDownPlaceMarkType();

            //var resSurveyBatch = await _s_SurveyBatch.GetLastActive(_accessToken);
            //ViewBag.SurveyBatchId = resSurveyBatch.data?.Id ?? 0;
            ViewData["KeyGoongMap"] = _configuration.GetValue<string>("KeyGoongMap");
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetList(VM_DataTableFilter request, int? provinceAreaId, int? districtAreaId, int? wardAreaId, int? farmId, int? landCertificateId, int? typeOfTreeId, int? activeStatusId, int type, int status, int surveyBatchId, bool isAllData, int accountId, EN_Search_PlaceMark searchBy)
        {
            if (surveyBatchId == 0)
            {
                return Json(new
                {
                    request.draw,
                    data = new List<M_PlaceMarkWithSurveyFarm>(),
                    recordsFiltered = 0,
                    recordsTotal = 0,
                    result = 0,
                    error = "Không có đợt kiểm kê đang hoạt động!"
                });
            }

            //Check permiss type data map
           /* if (!CheckPermissDataMap(type))
                return Json(new
                {
                    request.draw,
                    data = new List<M_PlaceMarkWithSurveyFarm>(),
                    recordsFiltered = 0,
                    recordsTotal = 0,
                    result = 0,
                    error = "Tài khoản này chưa được cấp quyền xem dữ liệu đối với loại bản đồ này, vui lòng liên hệ đến ban quản trị để được cấp quyền !"
                });*/

            var searchText = request.search["value"];
            var idxColumn = request.order["0"]["column"];
            var descString = request.order["0"]["dir"];
            int record = request.length;
            int page = (request.start + record) / record;
            string column = request.columns[idxColumn]["data"];
            bool desc = descString == "desc";

            //Set accountId = 0: no check createdBy on SurveyFarm
            //if (!isAllData)
            //{
            //    var accountType = _iHttpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccountType")?.Value;
            //    accountId = int.Parse(_accountId);
            //}

            string columnSearch = default;
            switch (searchBy)
            {
                case EN_Search_PlaceMark.PlotOldName:
                    columnSearch = "plot_old_name";
                    break;
                case EN_Search_PlaceMark.IdPrivate:
                    columnSearch = "id_private";
                    break;
                case EN_Search_PlaceMark.PlotId:
                    columnSearch = "plot_id";
                    break;
                case EN_Search_PlaceMark.PhoneNumber:
                    columnSearch = "phone_number";
                    break;
                case EN_Search_PlaceMark.PlotNewName:
                    columnSearch = "plot_new_name";
                    break;
                default:
                    columnSearch = default;
                    break;
            }

            var res = await _s_PlaceMark.GetListByPagingWithSurveyFarmFaster(_accessToken, status, searchText, surveyBatchId, provinceAreaId, districtAreaId, wardAreaId, farmId, landCertificateId, typeOfTreeId, activeStatusId, type, default, desc, column, columnSearch, accountId, page, record);
            var data2nd = res.data2nd.ToObject<VM_DataTableFilter.ResponseData2nd>();
            return Json(new
            {
                request.draw,
                res.data,
                data2nd.recordsFiltered,
                data2nd.recordsTotal,
                error = res.error.message
            });
        }

        [HttpGet]
        public async Task<JsonResult> GetListSurveyFarmOlder(int placeMarkId, int surveyBatchId)
        {
            var res = await _s_SurveyFarm.GetListBySequenceStatusPlaceMarkIdExcludeSurveyBatchId(_accessToken, "0,1", placeMarkId, surveyBatchId);
            return Json(new M_JResult(res));
        }

        [HttpGet]
        public async Task<JsonResult> GetListBySpWithSurveyFarm(int surveyBatchId, int? provinceAreaId, int? districtAreaId, int? wardAreaId, int? activeStatusId, int? typeOfTreeId, short type, int farmId, string status, string searchText, EN_Search_PlaceMark searchBy)
        {
            M_JResult jResult = new M_JResult();
            if (surveyBatchId == 0)
            {
                jResult.error.message = "Không có đợt kiểm kê đang hoạt động!";
                return Json(jResult);
            }

            //Check permiss type data map
            if (!CheckPermissDataMap(type))
            {
                jResult.error.message = "Tài khoản này chưa được cấp quyền xem dữ liệu, vui lòng liên hệ đến ban quản trị để được cấp quyền !";
                return Json(jResult);
            }

            string columnSearch = default;
            switch (searchBy)
            {
                case EN_Search_PlaceMark.PlotOldName:
                    columnSearch = "plot_old_name";
                    break;
                case EN_Search_PlaceMark.IdPrivate:
                    columnSearch = "id_private";
                    break;
                case EN_Search_PlaceMark.PlotId:
                    columnSearch = "plot_id";
                    break;
                case EN_Search_PlaceMark.PhoneNumber:
                    columnSearch = "phone_number";
                    break;
                case EN_Search_PlaceMark.PlotNewName:
                    columnSearch = "plot_new_name";
                    break;
                case EN_Search_PlaceMark.FarmerChildName:
                    columnSearch = "farmerChild_name";
                    break;
                case EN_Search_PlaceMark.FarmerChildCode:
                    columnSearch = "farmerChild_code";
                    break;
                default:
                    columnSearch = default;
                    break;
            }

            var res = await _s_PlaceMark.GetListBySpWithSurveyFarm(_accessToken, surveyBatchId, provinceAreaId, districtAreaId, wardAreaId, activeStatusId, typeOfTreeId, type, farmId, status, searchText, default, columnSearch);
            return Json(new M_JResult(res));
        }

        [HttpGet]
        public async Task<JsonResult> SearchMap(string searchText, int? wardAreaId, int status, int surveyBatchId, int page = 1, int record = 10)
        {
            M_JResult jResult = new M_JResult();
            if (surveyBatchId == 0)
            {
                jResult.error.message = "Không có đợt kiểm kê đang hoạt động!";
                return Json(jResult);
            }

            //Set accountId = 0: no check createdBy on SurveyFarm
            int accountId = 0;
            //var accountType = _iHttpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccountType")?.Value;
            //int accountId = accountType == RolesConstants.STAFF ? Int32.Parse(_accountId) : 0;
            var res = await _s_PlaceMark.GetListByPagingWithSurveyFarmAdvancedFaster(_accessToken, status, searchText, surveyBatchId, wardAreaId, accountId, page, record);
            return Json(jResult.MapData(res));
        }

        [PermissionAuthorize($"{Ro_Functions.TREEDIARY}.{Ro_Controllers.TD_SURVEY_FARM}.{Ro_Actions.READ}")]
        [HttpGet]
        public async Task<JsonResult> GetListNearlyDistance(int surveyBatchId, int? type, string latitude, string longitude, int radius)
        {
            M_JResult jResult = new M_JResult();
            if (surveyBatchId == 0)
            {
                jResult.error.message = "Không có đợt kiểm kê đang hoạt động!";
                return Json(jResult);
            }
            if (radius > 5000)
            {
                jResult.error.message = "Khoảng cách tra cứu tối đa là 5km so với tọa độ hiện tại!";
                return Json(jResult);
            }

            //Check type of data have permiss
            int typeOfDataMap = 0;
            typeOfDataMap = CheckTypeOfMapHavePermiss();
            if (typeOfDataMap == -1)
            {
                jResult.error.message = "Tài khoản này chưa được cấp quyền xem dữ liệu xung quanh, vui lòng liên hệ đến ban quản trị để được cấp quyền !";
                return Json(jResult);
            }

            var res = await _s_PlaceMark.GetListNearlyDistanceWithFullObject(_accessToken, surveyBatchId, typeOfDataMap, latitude, longitude, radius);
            return Json(jResult.MapData(res));
        }

        [PermissionAuthorize($"{Ro_Functions.TREEDIARY}.{Ro_Controllers.TD_SURVEY_FARM}.{Ro_Actions.READ}")]
        [HttpGet]
        public async Task<JsonResult> GetListNearlyDistanceByMarker(int surveyBatchId, int? type, string latitude, string longitude, int radius)
        {
            M_JResult jResult = new M_JResult();
            if (surveyBatchId == 0)
            {
                jResult.error.message = "Không có đợt kiểm kê đang hoạt động!";
                return Json(jResult);
            }
            if (radius > 5000)
            {
                jResult.error.message = "Khoảng cách tra cứu tối đa là 5km so với tọa độ hiện tại!";
                return Json(jResult);
            }

            //Check permiss type data map
            if (!CheckPermissDataMap(type) || type < 0)
            {
                jResult.result = 0;
                jResult.error.message = "Không có đợt kiểm kê đang hoạt động!";
                return Json(jResult);
            }

            //Check type of data have permiss
            //int typeOfDataMap = 0;
            //typeOfDataMap = CheckTypeOfMapHavePermiss();
            //if (typeOfDataMap == -1)
            //{
            //    jResult.error.message = "Tài khoản này chưa được cấp quyền xem dữ liệu xung quanh, vui lòng liên hệ đến ban quản trị để được cấp quyền !";
            //    return Json(jResult);
            //}

            var res = await _s_PlaceMark.GetListNearlyDistanceWithFullObject(_accessToken, surveyBatchId, type ?? 0, latitude, longitude, radius);
            return Json(jResult.MapData(res));
        }

        [PermissionAuthorize($"{Ro_Functions.TREEDIARY}.{Ro_Controllers.TD_SURVEY_FARM}.{Ro_Actions.READ}")]
        [HttpGet]
        public async Task<JsonResult> GetListNearlyDistanceByPolygon(int surveyBatchId, int? type, string latitude, string longitude, int radius)
        {
            M_JResult jResult = new M_JResult();
            if (surveyBatchId == 0)
            {
                jResult.result = 0;
                jResult.error.message = "Không có đợt kiểm kê đang hoạt động!";
                return Json(jResult);
            }

            if (radius > 5000)
            {
                jResult.result = 0;
                jResult.error.message = "Khoảng cách tra cứu tối đa là 5km so với tọa độ hiện tại!";
            }

            //Check type of data have permiss
            //int typeOfDataMap = 0;
            //typeOfDataMap = CheckTypeOfMapHavePermiss();
            //if (typeOfDataMap == -1)
            //{
            //    jResult.error.message = "Tài khoản này chưa được cấp quyền xem dữ liệu xung quanh, vui lòng liên hệ đến ban quản trị để được cấp quyền !";
            //    return Json(jResult);
            //}

            if (!CheckPermissDataMap(type) || type < 0)
            {
                jResult.result = 0;
                jResult.error.message = "Tài khoản này chưa được cấp quyền xem dữ liệu đối với loại bản đồ này, vui lòng liên hệ đến ban quản trị để được cấp quyền !";
                return Json(jResult);
            }

            var res = await _s_PlaceMark.GetListNearlyDistanceWithFullObject(_accessToken, surveyBatchId, type ?? 0, latitude, longitude, radius);
            return Json(jResult.MapData(res));
        }

        [HttpGet]
        public async Task<IActionResult> P_View(int id, int surveyBatchId)
        {
            var res = await _s_PlaceMark.GetByIdSurveyBatchIdWithSurveyFarmFullParam(_accessToken, id, surveyBatchId);
            if (res.result == 1 && res.data != null)
            {
                EM_SurveyFarm surveyFarm = new EM_SurveyFarm();
                //EM_SurveyTree surveyTree = new EM_SurveyTree();
                if (res.data.surveyFarmObj != null)
                {
                    surveyFarm = _mapper.Map<EM_SurveyFarm>(res.data.surveyFarmObj);
                }
                else
                {
                    surveyFarm = new EM_SurveyFarm
                    {
                        Id = 0,
                        PlaceMarkId = id,
                        FarmGroupId = 0,
                        SurveyBatchId = surveyBatchId,
                        LandLevelId = 0,
                        CultivatorId = 0,
                        OwnerId = 0,
                        PlotId = res.data.PlotId,
                        IdPrivate = res.data.IdPrivate ?? string.Empty,
                        RiskLevel = res.data.RiskLevel ?? string.Empty,
                        PlotOldName = res.data.PlotOldName ?? string.Empty,
                        PlotNewName = res.data.PlotNewName ?? string.Empty,
                        PlotName = res.data.PlotName ?? string.Empty,
                        LandType = res.data.LandType ?? string.Empty,
                        Area = res.data.Area ?? 0,
                        Area1 = res.data.Area1 > 0 ? res.data.Area1 : 0,
                        Area2 = res.data.Area2 > 0 ? res.data.Area2 : 0,
                        Area3 = res.data.Area3 > 0 ? res.data.Area3 : 0,
                        AreaCultivated = res.data.AreaCultivated ?? 0,
                        AltitudeHighest = 0,
                        AltitudeLowest = 0,
                        AreaManagementChange = 0,
                        ImageId = 0,
                        Remark = res.data.Remark ?? string.Empty,
                        status = 1,
                        createdAt = null,
                        // Thuộc tính cây trồng (TreeData)
                        Type = 0,
                        TypeOfTreeId = 0,
                        ActiveStatusId = null,
                        ClassifyId = null,
                        GardenRatingId = null,
                        PlantingMethodId = 0,
                        PlantingDistanceId = 0,
                        YearOfPlanting = null,
                        PlantingDesignDensity = null,
                        TreeQuantity = null,
                        HoleQuantity = null,
                        RootTreeCorrectQuantity = null,
                        RootTreeMixedQuantity = null,
                        GrowingTreeQuantity = null,
                        GraftedTreeCorrectQuantity = null,
                        GraftedTreeMixedQuantity = null,
                        DensityOfGraftedTree = null,
                        AverageNumberLeafLayer = null,
                        PlantingEndDate = null,
                        VanhAverage = null,
                        EffectiveTreeCorrectQuantity = null,
                        EffectiveTreeMixedQuantity = null,
                        IneffectiveTreeQuantity = null,
                        EmptyHoleQuantity = null,
                        EffectiveTreeDensity = null,
                        StandardDeviation = null,
                        RatioTreeObtain = null,
                        ExpectedExploitationDate = null,
                        StartExploitationDate = null,
                        EndExploitationDate = null,
                        EffectiveTreeShavingQuantity = null,
                        EffectiveTreeNotshavingQuantity = null,
                        IneffectiveTreeDryQuantity = null,
                        IneffectiveTreeNotgrowQuantity = null,
                        ShavingTreeDensity = null,
                        ShavingModeId = null,
                        ShavingFaceConditionId = 0,
                        TotalOutput = null,
                        TotalShavingSlice = null,
                        ProductivityByArea = null,
                        ProductivityByTree = null,
                    };
                }

                //if(res.data.surveyTreeObj)
                double? area = 0;
                area = res.data.Area > 0 ? res.data.Area : (res.data.Area1 ?? 0) + (res.data.Area2 ?? 0) + (res.data.Area3 ?? 0);
                res.data.Area = area > 0 ? Math.Round(area ?? 0, 2, MidpointRounding.ToNegativeInfinity) : 0;
                ViewBag.SurveyBatch = surveyBatchId;
                ViewBag.SurveyFarm = surveyFarm;
                //ViewBag.SurveyTree = surveyTree;
                ViewBag.PlaceMark = res.data;
                ViewBag.SurveyFarmJson = JsonConvert.SerializeObject(surveyFarm);

                SetDropDownRiskLevel();
                await SetDropDownLandLevel(0);
                await SetDropDownTypeOfTree(0);
                await SetDropDownPlantingMethod(0);
                await SetDropDownShavingMode(0);
                await SetDropDownShavingFaceCondition(0);

                var resHistory = await _s_Account.GetInfoUserCreatedUpdated(_accessToken, surveyFarm?.createdBy, surveyFarm?.updatedBy);
                ViewBag.History = resHistory;
                return PartialView(res.data);
            }
            return Json(new M_JResult(res));
        }

        [HttpGet]
        public async Task<JsonResult> GetDetailSurveyFarm(int id)
        {
            var res = await _s_SurveyFarm.GetById(_accessToken, id);
            return Json(new M_JResult(res));
        }

        [HttpGet]
        public async Task<IActionResult> ChooseItemSearchMain(int id, int surveyBatchId)
        {
            M_JResult jResult = new M_JResult();
            var res = await _s_PlaceMark.GetByIdSurveyBatchIdWithSurveyFarm(_accessToken, id, surveyBatchId);
            if (res.result == 1 && res.data != null)
            {
                jResult.data = _mapper.Map<M_PlaceMark_SpWithSurveyFarm>(res.data);
                jResult.error = res.error;
                jResult.result = res.result;
            }
            return Json(jResult);
        }

        [PermissionAuthorize($"{Ro_Functions.TREEDIARY}.{Ro_Controllers.TD_SURVEY_FARM}.{Ro_Actions.UPDATE}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> CreateOrUpdate(EM_SurveyFarm model)
        {
            M_JResult jResult = new M_JResult();
            if (!ModelState.IsValid)
            {
                jResult.error = new error(0, DataAnnotationExtensionMethod.GetErrorMessage(ModelState));
                return Json(jResult);
            }
            if (model.Id > 0)
            {
                var res = await _s_SurveyFarm.UpdateFormSurveyFarm(_accessToken, model, _accountId);
                model.status = 1;
                return Json(jResult.MapData(res));
            }
            else
            {
                var res = await _s_SurveyFarm.CreateFormSurveyFarm(_accessToken, model, _accountId);
                return Json(jResult.MapData(res));
            }
        }

        [PermissionAuthorize($"{Ro_Functions.TREEDIARY}.{Ro_Controllers.TD_SURVEY_FARM}.{Ro_Actions.DELETE}")]
        [HttpDelete]
        public async Task<JsonResult> DeleteAllSurveyBySurveyBatch(int id, int surveyBatchId)
        {
            M_JResult jResult = new M_JResult();
            var res = await _s_PlaceMark.DeleteAllSurveyBySurveyBatch(_accessToken, id, surveyBatchId, _accountId);
            return Json(jResult.MapData(res));
        }

        [PermissionAuthorize($"{Ro_Functions.TREEDIARY}.{Ro_Controllers.TD_SURVEY_FARM}.{Ro_Actions.UPDATE}")]
        [HttpPut]
        public async Task<JsonResult> ChangeStatus(int id, int status)
        {
            M_JResult jResult = new M_JResult();
            var res = await _s_SurveyFarm.UpdateStatus(_accessToken, id, status, _accountId);
            return Json(jResult.MapData(res));
        }

        [PermissionAuthorize($"{Ro_Functions.TREEDIARY}.{Ro_Controllers.TD_SURVEY_FARM}.{Ro_Actions.UPDATE}")]
        [HttpPut]
        public async Task<JsonResult> UpdateType(int id, short type)
        {
            M_JResult jResult = new M_JResult();
            //Check permiss type data map
            if (type < 0)
                return Json(new
                {
                    result = 0,
                    error = "Có lỗi trong quá trình chuyển đổi loại bản đồ, hãy tải lại trang!"
                });
            var res = await _s_PlaceMark.UpdateType(_accessToken, id, type, _accountId);
            return Json(jResult.MapData(res));
        }

        [PermissionAuthorize($"{Ro_Functions.TREEDIARY}.{Ro_Controllers.TD_SURVEY_FARM}.{Ro_Actions.UPDATE}")]
        [HttpPut]
        public async Task<JsonResult> ChangeStatusList(string jsonId, int status)
        {
            M_JResult jResult = new M_JResult();
            if (string.IsNullOrEmpty(jsonId))
            {
                jResult.error = new error(0, "Please complete all information!");
                return Json(jResult);
            }
            var res = await _s_SurveyFarm.UpdateStatusList(_accessToken, jsonId, status, _accountId);
            return Json(jResult.MapData(res));
        }

        [PermissionAuthorize($"{Ro_Functions.TREEDIARY}.{Ro_Controllers.TD_SURVEY_FARM}.{Ro_Actions.UPDATE}")]
        [HttpPut]
        public async Task<JsonResult> ChooseFarmerInTable(int farmerId, int typeShowModalFarmer, int surveyFarmId)
        {
            M_JResult jResult = new M_JResult();
            if (surveyFarmId == 0)
            {
                jResult.error = new error(0, "Vui lòng nhập ID vườn!");
                return Json(jResult);
            }
            if (typeShowModalFarmer == 1) //Owner
            {
                var res = await _s_SurveyFarm.UpdateOwnerId(_accessToken, surveyFarmId, farmerId, _accountId);
                return Json(jResult.MapData(res));
            }
            else //Cultivator
            {
                var res = await _s_SurveyFarm.UpdateCultivatorId(_accessToken, surveyFarmId, farmerId, _accountId);
                return Json(jResult.MapData(res));
            }
        }

        [PermissionAuthorize($"{Ro_Functions.TREEDIARY}.{Ro_Controllers.TD_SURVEY_FARM}.{Ro_Actions.CREATE}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> ImportExcel(IFormFile file, int surveyBatchId)
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
                string Dt(object v)
                {
                    if (v is DateTime dt)
                        return dt.ToString("yyyy-MM-dd");

                    var s = S(v)?.Trim();
                    if (string.IsNullOrEmpty(s))
                        return $"{DateTime.Now.Year}-01-01"; // mặc định

                    // Nếu có dạng chuỗi range "18-23/6/30" => tách lấy ngày đầu tiên (18)
                    // hoặc lấy phần bên phải sau dấu '-' nếu ngày không rõ ràng
                    if (s.Contains("-"))
                        s = s.Split('-').First().Trim();

                    // TH1: dd/MM/yyyy
                    if (DateTime.TryParseExact(s, new[] { "dd/M/yyyy", "d/M/yyyy", "dd/MM/yyyy", "d/MM/yyyy" },
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var fullDate))
                        return fullDate.ToString("yyyy-MM-dd");

                    // TH2: MM/yyyy
                    if (DateTime.TryParseExact("01/" + s, new[] { "dd/M/yyyy", "dd/MM/yyyy" },
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var monthYear))
                        return monthYear.ToString("yyyy-MM-dd");

                    // TH3: yyyy
                    if (DateTime.TryParseExact("01/01/" + s, new[] { "dd/MM/yyyy" },
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var yearOnly))
                        return yearOnly.ToString("yyyy-MM-dd");

                    // Nếu vẫn fail → mặc định
                    return $"{DateTime.Now.Year}-01-01";
                }

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
                                ActiveStatusCode = "TMTC", // Hiện trạng vườn
                                IdPrivate = S(Cell(1)), // 1.12TN.GD.24.190 - Mã lô
                                PlotName = S(Cell(5)), // C1 - Tên lô
                                LandLevelCode = S(Cell(6)), // TC-III.3 - Hạng đất
                                //AltitudeLowest = D(Cell(7)), //9.71 - cao trình thấp nhất
                                //AltitudeHighest = D(Cell(8)), // 10.22 - cao trình cao nhất
                                AltitudeAverage = D(Cell(7)), // 1.5 - Chiều cao trung bình
                                PlantingMethodCode = S(Cell(8)), // B1 - Phương pháp trồng
                                PlantingDistanceCode = S(Cell(9)), // 6 x 3 - Khoảng cách trồng
                                PlantingDesignDensity = D(Cell(10)), // 555 - Mật độ thiết kế
                                TypeOfTreeCode = S(Cell(11)), // RRIV 209 - Giống cây
                                Area = D(Cell(12)), // 24.00336 - Diện tích
                                HoleQuantity = I(Cell(13)), // 13,640 - Tổng số hố kk
                                GraftedTreeCorrectQuantity = I(Cell(14)), //13,640 - Số cây ghép đúng giống
                                GraftedTreeMixedQuantity = I(Cell(16)), // 0 - Số cây ghép lẫn giống
                                EmptyHoleQuantity = I(Cell(18)), // 0 - Số hố trống
                                DensityOfGraftedTree = I(Cell(20)), // 568 - Mật độ cây ghép
                                AverageNumberLeafLayer = D(Cell(21)), // 4.37 - Số lá tầng bình quân
                                GardenRatingCode = S(Cell(22)), // TC1 - Phân loại
                                PlantingEndDate = S(Cell(23)), // 18-23/6/24 - Ngày kết thúc trồng
                                Remark = S(Cell(24)) // Ghi chú
                            });

                        // KTCB
                        else if (sheet == "1.KTCB")
                            data.Add(new
                            {
                                ActiveStatusCode = "KTCB", // Hiện trạng vườn
                                IdPrivate = S(Cell(1)), // 1.12TN.GD.24.190 - Mã lô
                                PlotOldName = S(Cell(5)),  // C1 - Tên lô cũ
                                PlotNewName = S(Cell(6)), // C1 - Tên lô mới
                                YearOfPlanting = S(Cell(7)), // 2024 - Năm trồng
                                LandLevelCode = S(Cell(8)), // TC-III.3 - Hạng đất
                                AltitudeAverage = D(Cell(9)), // 9.965 - cao trình trung bình
                                PlantingMethodCode = S(Cell(10)),   // B1 - Phương pháp trồng
                                PlantingDistanceCode = S(Cell(11)), // 6 x 3 - Khoảng cách trồng
                                PlantingDesignDensity = D(Cell(12)), // 555 - Mật độ thiết kế
                                TypeOfTreeCode = S(Cell(13)), // RRIV 209 - Giống cây
                                AreaOld = S(Cell(14)), //Diện tích cũ
                                Area = D(Cell(15)), // 24.00336 - Diện tích
                                HoleQuantity = I(Cell(16)), // 13,640 - Tổng số hố kk
                                EffectiveTreeCorrectQuantity = I(Cell(17)), // 13,640 - Số cây đúng giống
                                EffectiveTreeMixedQuantity = I(Cell(19)), // 0 - Số cây lẫn giống
                                IneffectiveTreeQuantity = I(Cell(21)), // 3 - Cây không hiệu quả
                                EmptyHoleQuantity = I(Cell(23)), // 0 - Số hố trống
                                EffectiveTreeDensity = I(Cell(25)), // 568 - Mật độ cây hiệu quả
                                VanhAverage = D(Cell(27)),  // 12.7 - Vành bình quân
                                StandardDeviation = D(Cell(28)), // 1.9 - Độ lệch chuẩn
                                RatioTreeObtain = D(Cell(29)), // 100 - Tỷ lệ cây đạt T/C vanh

                                Vanh50 = I(Cell(30)), // Vườn cây dự kiến kéo dài
                                Vanh4549 = I(Cell(31)), // Vườn cây dự kiến kéo dài
                                Vanh4044 = I(Cell(32)), // Vườn cây dự kiến kéo dài
                                Vanh3539 = I(Cell(33)), // Vườn cây dự kiến kéo dài
                                Vanh34 = I(Cell(34)), // Vườn cây dự kiến kéo dài
                                BarkThickness = I(Cell(35)), // Vườn cây dự kiến kéo dài

                                PlannedExtendedGarden = B(Cell(36)), // Vườn cây dự kiến kéo dài
                                ExpectedExploitationDate = Dt(Cell(37)), // 18-23/6/30 - Ngày dự kiến khai thác
                                GardenRatingCode = S(Cell(38)),  // Phân loại vườn cây
                                Remark = S(Cell(39)) // Ghi chú
                            });

                        // KD
                        else if (sheet == "1.KD")
                            data.Add(new
                            {
                                ActiveStatusCode = "KD", // Hiện trạng vườn
                                IdPrivate = S(Cell(1)), // 1.12TN.GD.24.190 - Mã lô
                                PlotOldName = S(Cell(5)), // C1 - Tên lô cũ
                                PlotNewName = S(Cell(6)), // C1 - Tên lô mới
                                YearOfPlanting = S(Cell(7)), // 2024 - Năm trồng
                                LandLevelCode = S(Cell(8)), // TC-III.3 - Hạng đất
                                AltitudeAverage = D(Cell(9)), // 1.5 - Chiều cao trung bình
                                PlantingMethodCode = S(Cell(10)), // B1 - Phương pháp trồng
                                PlantingDistanceCode = S(Cell(11)), // 6 x 3 - Khoảng cách trồng
                                PlantingDesignDensity = D(Cell(12)), // 555 - Mật độ thiết kế
                                TypeOfTreeCode = S(Cell(13)), // RRIV 209 - Giống cây
                                AreaOld = S(Cell(14)), //Diện tích cũ
                                Area = D(Cell(15)), // 24.00336 - Diện tích
                                TreeQuantity = I(Cell(16)), // 13,640 - Tổng số cây

                                EffectiveTreeShavingQuantity = I(Cell(17)), // Cây cạo hữu hiệu
                                EffectiveTreeNotshavingQuantity = I(Cell(19)), // Cây cạo không hữu hiệu
                                IneffectiveTreeDryQuantity = I(Cell(21)), // Cây không hữu hiệu 
                                IneffectiveTreeNotgrowQuantity = I(Cell(23)), // Cây không phát triển

                                EmptyHoleQuantity = I(Cell(25)), // Số hố trống
                                ShavingTreeDensity = I(Cell(27)), // Mật độ cây cạo
                                ShavingModeCode = S(Cell(28)), // H1 -Chế độ cạo
                                StartExploitationDate = Dt(Cell(29)), // 18-23/6/30 - Tháng/Nam mở cạo
                                TappingAge = I(Cell(30)), // 6 - Tuổi cạo
                                EndExploitationDate = S(Cell(31)), // Năm cạo up
                                ShavingFaceConditionCode = S(Cell(32)), // T1 - Tình trạng mặt cạo
                                TotalOutput = D(Cell(33)), // Sản lượng
                                TotalStaff = I(Cell(34)), // Tổng số lao động
                                ProductivityByArea = D(Cell(35)), // 1.5 - Năng suất kg/ha
                                ProductivityByTree = D(Cell(36)), // 2.5 - Năng suất kg/cây
                                TotalShavingSlice = I(Cell(37)), // 40 - Tổng số lát cạo 
                                GardenRatingCode = S(Cell(38)), // TC1 - Phân loại vườn cây
                                Remark = S(Cell(39)) // Ghi chú
                            });
                        // TXA
                        else if (sheet == "1.TXA")
                            data.Add(new
                            {
                                ActiveStatusCode = "TX", // Hiện trạng vườn
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
                                EffectiveTreeDensity = I(Cell(19)),
                                VanhAverage = D(Cell(20)),
                                RatioTreeObtain = D(Cell(21)),
                                GardenRatingCode = S(Cell(22))
                            });

                        // TXB
                        else if (sheet == "1.TXB")
                            data.Add(new
                            {
                                ActiveStatusCode = "TX", // Hiện trạng vườn
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

                var jsonString = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = "yyyy-MM-dd"
                });

                var res = await _s_SurveyFarm.ImportData(_accessToken, surveyBatchId, jsonString, _accountId);
                jResult = new M_JResult { result = res.result, data = res.data, error = res.error };
            }
            catch (Exception ex)
            {
                jResult.result = -1;
                jResult.error = new error(-1, $"Lỗi khi đọc file: {ex.Message}");
            }

            return Json(jResult);
        }

        public async Task<JsonResult> ExportExcel(int? surveyBatchId = 3, int? activeStatusId = null)
        {
            M_JResult jResult = new M_JResult();
            try
            {
                var templatePath = Path.Combine(_webHostEnvironment.WebRootPath, "exports/treediary/bieumaukiemkeimport.xlsx");
                using (var workbook = new XLWorkbook(templatePath))
                {
                    var surFarmRes = await _s_SurveyFarm.GetListSurveyFarmFullData(
                        _accessToken,
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

                    // Process each sheet for SurveyFarm Model
                    ProcessTMTCSheet(workbook, itemsList);
                    ProcessKTCBSheet(workbook, itemsList);
                    ProcessKDSheet(workbook, itemsList);
                    ProcessTXASheet(workbook, itemsList);
                    ProcessTXBSheet(workbook, itemsList);
                    // ProcessSummarySheet(workbook, itemsList);

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
                { 3, "farmObj.Code" },
                { 4, "farmGroupObj.Name" },
                { 5, "plotName" },
                { 6, "landLevelObj.Code" },
                { 7, "altitudeAverage" },
                { 8, "plantingMethodObj.Code" },
                { 9, "plantingDistanceObj.Code" },
                { 10, "plantingDesignDensity" },
                { 11, "typeOfTreeObj.Code" },
                { 12, "area" },
                { 13, "holeQuantity" },
                { 14, "graftedTreeCorrectQuantity" },
                { 16, "graftedTreeMixedQuantity" },
                { 18, "emptyHoleQuantity" },
                { 20, "densityOfGraftedTree" },
                { 21, "averageNumberLeafLayer" },
                { 22, "classifyObj.Code"},
                { 23, "plantingEndDate" },
                { 24, "remark" },
            };

            int row = 11;
            int stt = 1;

            var tmtcItems = itemsList.Where(item =>
            {
                string active = item.ActiveStatusObj?.code;
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

                SetFormula(ws, row, 15 + 1, $"=ROUND(O{row}/N{row}%,1)", "0.#");
                SetFormula(ws, row, 17 + 1, $"=ROUND(Q{row}/N{row}%,1)", "0.#");
                SetFormula(ws, row, 19 + 1, $"=ROUND(S{row}/N{row}%,1)", "0.#");

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
                { 3, "farmObj.Code" },
                { 4, "farmGroupObj.Name" },
                { 5, "plotOldName" },
                { 6, "plotNewName" },
                { 7, "yearOfPlanting" },
                { 8, "landLevelObj.Code" },
                { 9, "altitudeAverage" },
                { 10, "plantingMethodObj.Code" },
                { 11, "plantingDistanceObj.Code" },
                { 12, "plantingDesignDensity" },
                { 13, "typeOfTreeObj.Code" },
                { 14, "areaOld" },
                { 15, "area" },
                { 16, "holeQuantity" },
                { 17, "effectiveTreeCorrectQuantity" },
                { 19, "effectiveTreeMixedQuantity" },
                { 21, "ineffectiveTreeQuantity" },
                { 23, "emptyHoleQuantity" },
                { 25, "effectiveTreeDensity" },
                { 27, "vanhAverage" },
                { 28, "standardDeviation" },
                { 29, "ratioTreeObtain" },
                { 30, "vanh50" },
                { 31, "vanh4549" },
                { 32, "vanh4044" },
                { 33, "vanh3539" },
                { 34, "vanh34" },
                { 35, "barkThickness" },
                { 36, "plannedExtendedGarden"},
                { 37, "expectedExploitationDate" },
                { 38, "gardenRatingObj.Code" },
                { 39, "remark" },
            };

            var ktcbData = itemsList.Where(item =>
            {
                string? active = item.ActiveStatusObj?.code;
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

                    SetFormula(ws, row, 19, $"=ROUND(R{row}/Q{row}%,1)", "0.#");
                    SetFormula(ws, row, 21, $"=ROUND(T{row}/Q{row}%,1)", "0.#");
                    SetFormula(ws, row, 23, $"=ROUND(V{row}/Q{row}%,1)", "0.#");
                    SetFormula(ws, row, 25, $"=ROUND(X{row}/Q{row}%,1)", "0.#");

                    var markedX = (item.PlannedExtendedGarden ?? false) ? "x" : string.Empty;
                    ws.Cell(row, 37).Value = markedX;
                    ws.Cell(row, 37).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(row, 37).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    row++;
                    stt++;
                }

                ws.Row(row).InsertRowsBelow(1);
                ws.Cell(row, 8).Value = $"Cộng {plantingYear}";
                ws.Cell(row, 8).Style.Font.Bold = true;
                ws.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(row, 8).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;


                var startRow = row - yearItems.Count;
                var endRow = row - 1;

                SetFormula(ws, row, 16, $"=ROUND(SUM(P{startRow}:P{endRow}), 5)", "0.#####");
                SetFormula(ws, row, 17, $"=SUM(Q{startRow}:Q{endRow})");
                SetFormula(ws, row, 18, $"=SUM(R{startRow}:R{endRow})");
                SetFormula(ws, row, 19, $"=ROUND(AVERAGE(S{startRow}:S{endRow}),1)", "0.#");
                SetFormula(ws, row, 20, $"=SUM(T{startRow}:T{endRow})");
                SetFormula(ws, row, 21, $"=ROUND(AVERAGE(U{startRow}:U{endRow}),1)", "0.#");
                SetFormula(ws, row, 22, $"=SUM(V{startRow}:V{endRow})");
                SetFormula(ws, row, 23, $"=ROUND(AVERAGE(W{startRow}:W{endRow}),1)", "0.#");
                SetFormula(ws, row, 24, $"=SUM(X{startRow}:X{endRow})");
                SetFormula(ws, row, 25, $"=ROUND(AVERAGE(Y{startRow}:Y{endRow}),1)", "0.#");
                SetFormula(ws, row, 26, $"=ROUND(AVERAGE(Z{startRow}:Z{endRow}),1)", "0.#");
                SetFormula(ws, row, 28, $"=ROUND(AVERAGE(AB{startRow}:AB{endRow}),2)", "0.##");
                SetFormula(ws, row, 29, $"=ROUND(AVERAGE(AC{startRow}:AC{endRow}),2)", "0.##");
                SetFormula(ws, row, 30, $"=ROUND(AVERAGE(AD{startRow}:AD{endRow}),2)", "0.##");

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
                { 3, "farmObj.Code" },
                { 4, "farmGroupObj.Name" },
                { 5, "plotOldName" },
                { 6, "plotNewName" },
                { 7, "yearOfPlanting" },
                { 8, "landLevelObj.Code" },
                { 9, "altitudeAverage" },
                { 10, "plantingMethodObj.Code" },
                { 11, "plantingDistanceObj.Code" },
                { 12, "plantingDesignDensity" },
                { 13, "typeOfTreeObj.Code" },
                { 14, "areaOld" },
                { 15, "area" },
                { 16, "treeQuantity" },
                { 17, "effectiveTreeShavingQuantity" },
                { 19, "effectiveTreeNotshavingQuantity" },
                { 21, "ineffectiveTreeDryQuantity" },
                { 23, "ineffectiveTreeNotgrowQuantity" },
                { 25, "emptyHoleQuantity" },
                { 27, "shavingTreeDensity" },
                { 28, "shavingModeObj.Code" },
                { 29, "startExploitationDate" },
                { 30, "tappingAge" },
                { 31, "yearOfShaving" },
                { 32, "shavingFaceConditionObj.Code" },
                { 33, "totalOutput"},
                { 34, "totalStaff"},
                { 35, "productivityByArea" },
                { 36, "productivityByTree" },
                { 37, "totalShavingSlice" },
                { 38, "gardenRatingObj.Code" },
                { 39, "remark" },
            };

            var kdData = itemsList.Where(item =>
            {
                string active = item.ActiveStatusObj?.code;
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

                    SetFormula(ws, row, 19, $"=ROUND(R{row}/Q{row}%,1)", "0.#");
                    SetFormula(ws, row, 21, $"=ROUND(T{row}/Q{row}%,1)", "0.#");
                    SetFormula(ws, row, 23, $"=ROUND(V{row}/Q{row}%,1)", "0.#");
                    SetFormula(ws, row, 25, $"=ROUND(X{row}/Q{row}%,1)", "0.#");
                    SetFormula(ws, row, 27, $"=ROUND((Z{row}/Q{row}*100),2)", "0.##");

                    row++;
                    stt++;
                }

                ws.Row(row).InsertRowsBelow(1);
                ws.Cell(row, 8).Value = $"Cộng {plantingYear}";
                ws.Cell(row, 8).Style.Font.Bold = true;

                var startRow = row - yearItems.Count;
                var endRow = row - 1;

                SetFormula(ws, row, 16, $"=SUM(P{startRow}:P{endRow})", "0");
                SetFormula(ws, row, 17, $"=SUM(Q{startRow}:Q{endRow})", "0");
                SetFormula(ws, row, 18, $"=SUM(R{startRow}:R{endRow})", "0");
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

        private void ProcessTXSheet(XLWorkbook workbook, List<M_SurveyFarm> itemsList)
        {
            var ws = workbook.Worksheet("1.TX");
            if (ws == null) return;

            int nextRow = ProcessTXASheet(workbook, itemsList, ws, 10);

            nextRow += 9;
            ProcessTXBSheet(workbook, itemsList, ws, nextRow);
        }

        private int ProcessTXASheet(XLWorkbook workbook, List<M_SurveyFarm> itemsList, IXLWorksheet? wsOverride = null, int startRow = 10)
        {
            var ws = wsOverride ?? workbook.Worksheet("1.TXA");
            if (ws == null) return startRow;

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
                { 22, "gardenRatingObj.code" },
            };

            var txaItems = itemsList.Where(item =>
            {
                string? active = item.ActiveStatusObj?.code;
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

            int row = startRow;
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

            return row;
        }

        private int ProcessTXBSheet(XLWorkbook workbook, List<M_SurveyFarm> itemsList, IXLWorksheet? wsOverride = null, int startRow = 10)
        {
            var ws = wsOverride ?? workbook.Worksheet("1.TXB");
            if (ws == null) return startRow;

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
            };

            var txbItems = itemsList.Where(item =>
            {
                string? active = item.ActiveStatusObj?.code;
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

            int row = startRow;
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

            return row;
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

            int row = 10;
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

                var dataRange = ws.Range(row, 1, row, map2aKDFields.Count + 1);
                dataRange.Style.Border.TopBorder = XLBorderStyleValues.Dashed;
                dataRange.Style.Border.BottomBorder = XLBorderStyleValues.Dashed;

                row++;
                stt++;
            }

            // var totalRow = row;
            // ws.Row(totalRow).InsertRowsBelow(1);

            // ws.Cell(totalRow, 2).Value = "Cộng";
            // ws.Cell(totalRow, 2).Style.Font.Bold = true;
            // ws.Cell(totalRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // SetFormula(ws, totalRow, 4, $"SUM(D10:D{row - 1})", "0.00000");
            // SetFormula(ws, totalRow, 5, $"SUM(E10:E{row - 1})", "0.00000");
            // SetFormula(ws, totalRow, 6, $"SUM(F10:F{row - 1})", "0.00000");

            // SetFormula(ws, totalRow, 7, $"SUM(G10:G{row - 1})");

            // SetFormula(ws, totalRow, 8, $"AVERAGE(H10:H{row - 1})");
            // SetFormula(ws, totalRow, 9, $"AVERAGE(I10:I{row - 1})");
            // SetFormula(ws, totalRow, 11, $"AVERAGE(K10:K{row - 1})");
            // SetFormula(ws, totalRow, 12, $"AVERAGE(L10:L{row - 1})");
            // SetFormula(ws, totalRow, 12, $"AVERAGE(L10:L{row - 1})");
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

            int row = 10;
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

                var dataRange = ws.Range(row, 1, row, map2bKDFields.Count + 1);
                dataRange.Style.Border.TopBorder = XLBorderStyleValues.Dashed;
                dataRange.Style.Border.BottomBorder = XLBorderStyleValues.Dashed;

                row++;
                stt++;
            }

            // var totalRow = row;
            // ws.Row(totalRow).InsertRowsBelow(1);

            // ws.Cell(totalRow, 2).Value = "Cộng";

            // ws.Row(totalRow).Style.Font.Bold = true;
            // ws.Row(totalRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // SetFormula(ws, totalRow, 3, $"SUM(C10:C{row - 1})", "0.00000");
            // SetFormula(ws, totalRow, 4, $"SUM(D10:D{row - 1})", "0.00000");

            // SetFormula(ws, totalRow, 5, $"SUM(E10:E{row - 1})");
            // SetFormula(ws, totalRow, 6, $"SUM(F10:F{row - 1})");

            // SetFormula(ws, totalRow, 7, $"SUM(F10:F{row - 1})/SUM(D10:D{row - 1})", "0.00000");
            // SetFormula(ws, totalRow, 8, $"AVERAGE(H10:H{row - 1})");
            // SetFormula(ws, totalRow, 10, $"SUM(J10:J{row - 1})", "0");
            // SetFormula(ws, totalRow, 11, $"AVERAGE(K10:K{row - 1})");
            // SetFormula(ws, totalRow, 12, $"AVERAGE(L10:L{row - 1})");
            // SetFormula(ws, totalRow, 13, $"AVERAGE(M10:M{row - 1})");
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
            int startRowOfYear = row;
            string? currentYear = null;

            foreach (var item in placeMarkKTCBList)
            {
                var year = GetValue(item, "yearOfPlanting")?.ToString();

                if (currentYear != null && currentYear != year)
                {
                    ws.Row(row).InsertRowsBelow(1);

                    ws.Range(row, 1, row, 5).Merge();
                    var labelCell = ws.Cell(row, 1);
                    labelCell.Value = $"Cộng {currentYear}";
                    labelCell.Style.Font.Bold = true;
                    labelCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    labelCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    SetFormula(ws, row, 12, $"SUM(L{startRowOfYear}:L{row - 1})");
                    SetFormula(ws, row, 13, $"SUM(M{startRowOfYear}:M{row - 1})");
                    SetFormula(ws, row, 14, $"SUM(N{startRowOfYear}:N{row - 1})");

                    SetFormula(ws, row, 20, $"AVERAGE(T{startRowOfYear}:T{row - 1})");
                    SetFormula(ws, row, 21, $"AVERAGE(U{startRowOfYear}:U{row - 1})");
                    SetFormula(ws, row, 22, $"AVERAGE(V{startRowOfYear}:V{row - 1})");
                    SetFormula(ws, row, 24, $"AVERAGE(X{startRowOfYear}:X{row - 1})");
                    SetFormula(ws, row, 25, $"AVERAGE(Y{startRowOfYear}:Y{row - 1})");

                    row++;
                    startRowOfYear = row;
                }

                ws.Row(row).InsertRowsBelow(1);

                foreach (var kv in map3KTCBFields)
                {
                    var cell = ws.Cell(row, kv.Key);
                    WriteObjectToCell(cell, GetValue(item, kv.Value));
                }

                var dataRange = ws.Range(row, 1, row, map3KTCBFields.Keys.Max());
                dataRange.Style.Border.TopBorder = XLBorderStyleValues.Dashed;
                dataRange.Style.Border.BottomBorder = XLBorderStyleValues.Dashed;

                currentYear = year;
                row++;
            }

            if (currentYear != null)
            {
                ws.Row(row).InsertRowsBelow(1);

                ws.Range(row, 1, row, 5).Merge();
                var labelCell = ws.Cell(row, 1);
                labelCell.Value = $"Cộng {currentYear}";
                labelCell.Style.Font.Bold = true;
                labelCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                labelCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                SetFormula(ws, row, 12, $"SUM(L{startRowOfYear}:L{row - 1})");
                SetFormula(ws, row, 13, $"SUM(M{startRowOfYear}:M{row - 1})");
                SetFormula(ws, row, 14, $"SUM(N{startRowOfYear}:N{row - 1})");

                SetFormula(ws, row, 20, $"AVERAGE(T{startRowOfYear}:T{row - 1})");
                SetFormula(ws, row, 21, $"AVERAGE(U{startRowOfYear}:U{row - 1})");
                SetFormula(ws, row, 22, $"AVERAGE(V{startRowOfYear}:V{row - 1})");
                SetFormula(ws, row, 24, $"AVERAGE(X{startRowOfYear}:X{row - 1})");
                SetFormula(ws, row, 25, $"AVERAGE(Y{startRowOfYear}:Y{row - 1})");

                row++;
            }

            var tableRange = ws.Range(12, 1, row - 1, map3KTCBFields.Keys.Max());
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }

        private void Process3aKDSheet(XLWorkbook workbook, List<M_SurveyFarmTypeOfTree> placeMarkKDByTypeOfTreeList)
        {
            var ws = workbook.Worksheet("3a.KD");
            if (ws == null) return;

            int headerRow = 6;
            int subHeaderRow = 7;
            int metricsRow = 8;
            int col = 3;

            // STT, Tuổi cạo
            var sttHeaderRange = ws.Range(headerRow, 1, metricsRow, 1);
            sttHeaderRange.Merge();
            sttHeaderRange.Value = "STT";
            sttHeaderRange.Style.Font.Bold = true;
            sttHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sttHeaderRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            var tuoiCaoHeaderRange = ws.Range(headerRow, 2, metricsRow, 2);
            tuoiCaoHeaderRange.Merge();
            tuoiCaoHeaderRange.Value = "Tuổi cạo";
            tuoiCaoHeaderRange.Style.Font.Bold = true;
            tuoiCaoHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            tuoiCaoHeaderRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            var allTreeTypes = placeMarkKDByTypeOfTreeList
                .Where(x => x.TypeOfTreeObjs != null)
                .SelectMany(x => x.TypeOfTreeObjs)
                .Select(x => x.TypeOfTreeName)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            // Các header theo giống
            foreach (var treeType in allTreeTypes)
            {
                var treeTypeRange = ws.Range(subHeaderRow, col, subHeaderRow, col + 1);
                treeTypeRange.Merge();
                treeTypeRange.Value = treeType;
                treeTypeRange.Style.Font.Bold = true;
                treeTypeRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                var dtCell = ws.Cell(metricsRow, col);
                dtCell.Value = "DT\n(ha)";
                dtCell.Style.Font.Bold = true;
                dtCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                dtCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                dtCell.Style.Alignment.WrapText = true;

                var nsCell = ws.Cell(metricsRow, col + 1);
                nsCell.Value = "NS\n(kg/ha)";
                nsCell.Style.Font.Bold = true;
                nsCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                nsCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                nsCell.Style.Alignment.WrapText = true;

                col += 2;
            }

            // Cột Công ty
            var congTyRange = ws.Range(subHeaderRow, col, subHeaderRow, col + 1);
            congTyRange.Merge();
            congTyRange.Value = "Công ty";
            congTyRange.Style.Font.Bold = true;
            congTyRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var congTyDtCell = ws.Cell(metricsRow, col);
            congTyDtCell.Value = "DT";
            congTyDtCell.Style.Font.Bold = true;
            congTyDtCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var congTyNsCell = ws.Cell(metricsRow, col + 1);
            congTyNsCell.Value = "NS";
            congTyNsCell.Style.Font.Bold = true;
            congTyNsCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            col += 2;

            // GHI CHÚ
            var ghiChuRange = ws.Range(headerRow, col, metricsRow, col);
            ghiChuRange.Merge();
            ghiChuRange.Value = "GHI CHÚ";
            ghiChuRange.Style.Font.Bold = true;
            ghiChuRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ghiChuRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // "Giống" header (ở hàng 6 ghép ngang)
            int giongEndCol = col - 1;
            var giongHeaderRange = ws.Range(headerRow, 3, headerRow, giongEndCol);
            giongHeaderRange.Merge();
            giongHeaderRange.Value = "Giống";
            giongHeaderRange.Style.Font.Bold = true;
            giongHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            giongHeaderRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // LƯU lastCol để dùng sau (quan trọng)
            int lastCol = col;

            int dataRow = 10;
            int stt = 1;

            foreach (var item in placeMarkKDByTypeOfTreeList)
            {
                // Insert 1 dòng (theo pattern bạn dùng ở chỗ khác)
                ws.Row(dataRow).InsertRowsBelow(1);

                var sttCell = ws.Cell(dataRow, 1);
                sttCell.Value = stt;
                sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sttCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                var tappingAgeCell = ws.Cell(dataRow, 2);
                WriteObjectToCell(tappingAgeCell, item.TappingAge);

                col = 3;
                foreach (var treeType in allTreeTypes)
                {
                    var matchingTreeType = item.TypeOfTreeObjs?.FirstOrDefault(x => x.TypeOfTreeName == treeType);

                    var areaCell = ws.Cell(dataRow, col);
                    WriteObjectToCell(areaCell, matchingTreeType?.Area ?? 0);

                    var productivityCell = ws.Cell(dataRow, col + 1);
                    WriteObjectToCell(productivityCell, matchingTreeType?.ProductivityByArea ?? 0);

                    col += 2;
                }

                var congTyAreaCell = ws.Cell(dataRow, col);
                WriteObjectToCell(congTyAreaCell, 0);

                var congTyProductivityCell = ws.Cell(dataRow, col + 1);
                WriteObjectToCell(congTyProductivityCell, 0);

                col += 2;

                var ghiChuCell = ws.Cell(dataRow, lastCol); // ghi chú ở lastCol
                ghiChuCell.Value = "";

                // DÙNG lastCol ở đây (không dùng 'col' đã bị mutate)
                var dataRange = ws.Range(dataRow, 1, dataRow, lastCol);
                dataRange.Style.Border.TopBorder = XLBorderStyleValues.Dashed;
                dataRange.Style.Border.BottomBorder = XLBorderStyleValues.Dashed;

                dataRow++;
                stt++;
            }

            var tableRange = ws.Range(headerRow, 1, dataRow - 1, lastCol);
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            var emptyRow = dataRow;
            int totalRow = dataRow + 1;

            var emptyRowBorderRange = ws.Range(emptyRow, 1, emptyRow, lastCol);
            emptyRowBorderRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            emptyRowBorderRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            emptyRowBorderRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            emptyRowBorderRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var tongCongBorderRange = ws.Range(totalRow, 1, totalRow, lastCol);
            tongCongBorderRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            tongCongBorderRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            tongCongBorderRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            tongCongBorderRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            // ws.Row(totalRow).InsertRowsBelow(1);

            // var tongCongRange = ws.Range(totalRow, 1, totalRow, 2);
            // tongCongRange.Merge();
            // tongCongRange.Value = "TỔNG CỘNG";
            // tongCongRange.Style.Font.Bold = true;
            // tongCongRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            // tongCongRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // int formulaCol = 3;
            // while (formulaCol < col)
            // {
            //     SetFormula(ws, totalRow, formulaCol, $"SUM({ws.Cell(10, formulaCol).Address}:{ws.Cell(dataRow - 1, formulaCol).Address})", "0.000");
            //     SetFormula(ws, totalRow, formulaCol + 1, $"AVERAGE({ws.Cell(10, formulaCol + 1).Address}:{ws.Cell(dataRow - 1, formulaCol + 1).Address})", "0.000");
            //     formulaCol += 2;
            // }

            // int lastCol = col;
            // var range = ws.Range(headerRow, 1, totalRow, lastCol);
            // range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            // range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }

        private void Process3bKDSheet(XLWorkbook workbook, List<M_SurveyFarmKDLandLevel> placeMarkKDByLandLevelList)
        {
            var ws = workbook.Worksheet("3b.KD");
            if (ws == null) return;

            var map3KDFields = new Dictionary<int, string>
            {
                { 1, "tappingAge" },
                { 2, "rank1" },
                { 3, "rank2" },
                { 4, "rank3" },
                { 5, "company"},
                { 6, "productivityRank1"},
                { 7, "productivityRank2"},
                { 8, "productivityRank3"},
                { 9, "productivityCompany"},
            };

            int startRow = 9;
            int row = startRow;
            int stt = 1;

            foreach (var item in placeMarkKDByLandLevelList)
            {
                var landLevel = item.LandLevelObjs;
                if (landLevel != null)
                {
                    ws.Row(row).InsertRowsBelow(1);

                    foreach (var kv in map3KDFields)
                    {
                        var cell = ws.Cell(row, kv.Key + 1);
                        object? value = null;

                        if (kv.Value == "tappingAge")
                        {
                            value = item.TappingAge;
                        }
                        else
                        {
                            value = GetValue(landLevel, kv.Value);
                        }

                        WriteObjectToCell(cell, value);
                    }

                    var dataRange = ws.Range(row, 1, row, map3KDFields.Keys.Max() + 1);
                    dataRange.Style.Border.TopBorder = XLBorderStyleValues.Dashed;
                    dataRange.Style.Border.BottomBorder = XLBorderStyleValues.Dashed;

                    row++;
                    stt++;
                }
            }

            // var tongCongRow = row + 1;

            // var tongCongHeaderRange = ws.Range(tongCongRow, 1, tongCongRow, 2);
            // tongCongHeaderRange.Merge();
            // tongCongHeaderRange.Value = "TỔNG CỘNG";
            // tongCongHeaderRange.Style.Font.Bold = true;
            // tongCongHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            // tongCongHeaderRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // int dataEndRow = row - 1;

            // foreach (var colIndex in new[] { 2, 3, 4, 5 })
            // {
            //     string colLetter = ws.Column(colIndex + 1).ColumnLetter();
            //     SetFormula(ws, tongCongRow, colIndex + 1, $"SUM({colLetter}{startRow}:{colLetter}{dataEndRow})");
            // }

            // var mapping = new Dictionary<int, int>
            // {
            //     { 6, 2 },
            //     { 7, 3 },
            //     { 8, 4 },
            //     { 9, 5 },
            // };

            // foreach (var kv in mapping)
            // {
            //     string areaCol = ws.Column(kv.Value + 1).ColumnLetter();
            //     string prodCol = ws.Column(kv.Key + 1).ColumnLetter();
            //     SetFormula(ws, tongCongRow, kv.Key + 1,
            //         $"IF(SUM({areaCol}{startRow}:{areaCol}{dataEndRow})=0,\"\",SUMPRODUCT({areaCol}{startRow}:{areaCol}{dataEndRow},{prodCol}{startRow}:{prodCol}{dataEndRow})/SUM({areaCol}{startRow}:{areaCol}{dataEndRow}))",
            //         "0.00000");
            // }
        }

        private void Process4KDSheet(XLWorkbook workbook, List<M_SurveyFarmBusinessFarmByYear> placeMarkKDByGardenRatingList)
        {
            var ws = workbook.Worksheet("4.KD");
            if (ws == null) return;

            var map4KDFields = new Dictionary<int, string>
            {
                { 1, "tappingAge" },
                { 2, "totalHecta" },
                { 3, "q1" },
                { 4, "q2" },
                { 5, "q3" },
                { 6, "q4" },
                { 7, "percentQ1" },
                { 8, "percentQ2" },
                { 9, "percentQ3" },
                { 10, "percentQ4" }
            };

            int startRow = 10;
            int row = startRow;
            int stt = 1;

            foreach (var item in placeMarkKDByGardenRatingList)
            {
                ws.Row(row).InsertRowsBelow(1);

                var sttCell = ws.Cell(row, 1);
                sttCell.Value = stt;
                sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sttCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                foreach (var kv in map4KDFields)
                {
                    var cell = ws.Cell(row, kv.Key + 1);

                    object? value = null;
                    switch (kv.Value)
                    {
                        case "tappingAge":
                            value = item.TappingAge;
                            break;
                        case "totalHecta":
                            value = item.TotalHecta;
                            break;
                        default:
                            if (item.RatingObjs != null)
                                value = GetValue(item.RatingObjs, kv.Value);
                            break;
                    }

                    WriteObjectToCell(cell, value);
                }

                var dataRange = ws.Range(row, 1, row, map4KDFields.Keys.Max() + 1);
                dataRange.Style.Border.TopBorder = XLBorderStyleValues.Dashed;
                dataRange.Style.Border.BottomBorder = XLBorderStyleValues.Dashed;

                row++;
                stt++;
            }

            // int tongCongRow = row + 1;
            // int dataEndRow = row - 1;

            // var tongCongHeaderRange = ws.Range(tongCongRow, 1, tongCongRow, 2);
            // tongCongHeaderRange.Merge();
            // tongCongHeaderRange.Value = "TỔNG CỘNG";
            // tongCongHeaderRange.Style.Font.Bold = true;
            // tongCongHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            // tongCongHeaderRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // foreach (var colIndex in new[] { 2, 3, 4, 5, 6 })
            // {
            //     string colLetter = ws.Column(colIndex + 1).ColumnLetter();
            //     SetFormula(ws, tongCongRow, colIndex + 1, $"SUM({colLetter}{startRow}:{colLetter}{dataEndRow})");
            // }

            // SetFormula(ws, tongCongRow, 8, $"C{tongCongRow}/C{tongCongRow}", "0.00%");
            // SetFormula(ws, tongCongRow, 9, $"D{tongCongRow}/C{tongCongRow}", "0.00%");
            // SetFormula(ws, tongCongRow, 10, $"E{tongCongRow}/C{tongCongRow}", "0.00%");
            // SetFormula(ws, tongCongRow, 11, $"F{tongCongRow}/C{tongCongRow}", "0.00%");
        }

        private void Process2aKTCBSheet(XLWorkbook workbook, List<M_SurveyFarmKTCBYearOfFarm> placeMarkKTCBByGardenRatingYearList)
        {
            var ws = workbook.Worksheet("2a.KTCB");
            if (ws == null) return;

            var map2aKTCBFields = new Dictionary<int, string>
            {
                { 1, "yearOfPlanting" },
                { 2, "totalArea" },
                { 3, "tc1" },
                { 4, "percentTC1" },
                { 5, "tc2" },
                { 6, "percentTC2" },
                { 7, "tc3" },
                { 8, "percentTC3" },
                { 9, "k1" },
                { 10, "percentK1" },
                { 11, "k2" },
                { 12, "percentK2" },
                { 13, "k3" },
                { 14, "percentK3" },
                { 15, "k4" },
                { 16, "percentK4" },
                { 17, "k5" },
                { 18, "percentK5" }
            };

            int startRow = 10;
            int row = startRow;
            int stt = 1;

            foreach (var item in placeMarkKTCBByGardenRatingYearList)
            {
                ws.Row(row).InsertRowsBelow(1);
                var sttCell = ws.Cell(row, 1);
                sttCell.Value = stt;
                sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sttCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                foreach (var kv in map2aKTCBFields)
                {
                    var cell = ws.Cell(row, kv.Key + 1);

                    object? value = null;
                    switch (kv.Value)
                    {
                        case "yearOfPlanting":
                            value = item.YearOfPlanting;
                            break;
                        case "totalArea":
                            value = item.TotalArea;
                            break;
                        default:
                            if (item.RatingObjs != null)
                                value = GetValue(item.RatingObjs, kv.Value);
                            break;
                    }

                    WriteObjectToCell(cell, value);
                }

                var dataRange = ws.Range(row, 1, row, map2aKTCBFields.Keys.Max() + 1);
                dataRange.Style.Border.TopBorder = XLBorderStyleValues.Dashed;
                dataRange.Style.Border.BottomBorder = XLBorderStyleValues.Dashed;

                row++;
                stt++;
            }

            int tongCongRow = row + 1;

            var headerRange = ws.Range(tongCongRow, 1, tongCongRow, 2);
            headerRange.Merge();
            headerRange.Value = "TỔNG CỘNG";
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // int dataEndRow = row - 1;
            // string totalAreaCol = ws.Column(3).ColumnLetter();

            // foreach (var kv in map2aKTCBFields)
            // {
            //     int colIndex = kv.Key + 1;
            //     string colLetter = ws.Column(colIndex).ColumnLetter();

            //     if (kv.Value.StartsWith("percent", StringComparison.OrdinalIgnoreCase))
            //     {
            //         string correspondingCol = ws.Column(colIndex - 1).ColumnLetter();
            //         SetFormula(ws, tongCongRow, colIndex,
            //             $"SUM({correspondingCol}{startRow}:{correspondingCol}{dataEndRow})/SUM({totalAreaCol}{startRow}:{totalAreaCol}{dataEndRow})", "0.00%");
            //     }
            //     else if (kv.Value != "yearOfPlanting")
            //     {
            //         SetFormula(ws, tongCongRow, colIndex,
            //             $"SUM({colLetter}{startRow}:{colLetter}{dataEndRow})");
            //     }
            // }
        }

        private void Process2bKTCBSheet(XLWorkbook workbook, List<M_SurveyFarmKTCBYearOfFarm> placeMarkKTCBByGardenRatingFarmGroupList)
        {
            var ws = workbook.Worksheet("2b.KTCB");
            if (ws == null) return;

            var map2bKTCBFields = new Dictionary<int, string>
            {
                { 1, "yearOfPlanting" },
                { 2, "totalArea" },
                { 3, "tc1" },
                { 4, "percentTC1" },
                { 5, "tc2" },
                { 6, "percentTC2" },
                { 7, "tc3" },
                { 8, "percentTC3" },
                { 9, "k1" },
                { 10, "percentK1" },
                { 11, "k2" },
                { 12, "percentK2" },
                { 13, "k3" },
                { 14, "percentK3" },
                { 15, "k4" },
                { 16, "percentK4" },
                { 17, "k5" },
                { 18, "percentK5" }
            };

            int row = 10;
            int stt = 1;

            foreach (var item in placeMarkKTCBByGardenRatingFarmGroupList)
            {
                ws.Row(row).InsertRowsBelow(1);
                var sttCell = ws.Cell(row, 1);
                sttCell.Value = stt;
                sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sttCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                foreach (var kv in map2bKTCBFields)
                {
                    var cell = ws.Cell(row, kv.Key + 1);

                    object? value = null;
                    switch (kv.Value)
                    {
                        case "farmName":
                            value = item.FarmName;
                            break;
                        case "yearOfPlanting":
                            value = item.YearOfPlanting;
                            break;
                        case "totalArea":
                            value = item.TotalArea;
                            break;
                        default:
                            if (item.RatingObjs != null)
                                value = GetValue(item.RatingObjs, kv.Value);
                            break;
                    }

                    WriteObjectToCell(cell, value);
                }

                var dataRange = ws.Range(row, 1, row, map2bKTCBFields.Keys.Max() + 1);
                dataRange.Style.Border.TopBorder = XLBorderStyleValues.Dashed;
                dataRange.Style.Border.BottomBorder = XLBorderStyleValues.Dashed;

                row++;
                stt++;
            }

            // var totalRow = row;
            // ws.Cell(totalRow, 1).Value = "Tổng cộng";
            // ws.Range(totalRow, 1, totalRow, 2).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // foreach (var kv in map2bKTCBFields)
            // {
            //     int col = kv.Key + 1;

            //     if (kv.Value.StartsWith("percent"))
            //     {
            //         string hạng = kv.Value.Replace("percent", "").ToLowerInvariant();
            //         var hạngCol = map2bKTCBFields.FirstOrDefault(x => x.Value.Equals(hạng, StringComparison.OrdinalIgnoreCase)).Key + 1;

            //         string formula = $"SUM(R{10}C{hạngCol}:R{row - 1}C{hạngCol})/SUM(R{10}C2:R{row - 1}C2)";
            //         SetFormula(ws, totalRow, col, formula);
            //     }
            //     else if (kv.Value != "yearOfPlanting" && kv.Value != "farmName")
            //     {
            //         string formula = $"SUM(R{10}C{col}:R{row - 1}C{col})";
            //         SetFormula(ws, totalRow, col, formula);
            //     }
            // }
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
                        { 11, "typeOfTreeObj.code" }, { 12, "area" }, { 13, "holeQuantity" }, { 14, "graftedTreeCorrectQuantity" },
                        { 16, "graftedTreeMixedQuantity" }, { 18, "emptyHoleQuantity" }, { 20, "densityOfGraftedTree" },
                        { 21, "averageNumberLeafLayer" }, { 22, "gardenRatingObj.code"}, { 23, "plantingEndDate" }, { 24, "remark" }
                    },
                    "1.KTCB" => new Dictionary<int, string>
                    {
                        { 1, "idPrivate" }, { 5, "plotOldName" }, { 6, "plotNewName" }, { 7, "yearOfPlanting" },
                        { 8, "landLevelObj.code" }, { 9, "altitudeAverage" }, { 10, "plantingMethodObj.code" },
                        { 11, "plantingDistanceObj.code" }, { 12, "plantingDesignDensity" }, { 13, "typeOfTreeObj.code" },
                        { 15, "area" }, { 16, "holeQuantity" }, { 17, "effectiveTreeCorrectQuantity" },
                        { 19, "effectiveTreeMixedQuantity" }, { 21, "ineffectiveTreeNotgrowQuantity" }, { 23, "emptyHoleQuantity" },
                        { 25, "effectiveTreeDensity" }, { 27, "vanhAverage" }, { 28, "standardDeviation" },
                        { 29, "ratioTreeObtain" }, { 31, "expectedExploitationDate" }, { 32, "gardenRatingObj.code" }, { 33, "remark" }
                    },
                    "1.KD" => new Dictionary<int, string>
                    {
                        { 1, "idPrivate" }, { 5, "plotOldName" }, { 6, "plotNewName" }, { 7, "yearOfPlanting" },
                        { 8, "landLevelObj.code" }, { 9, "altitudeAverage" }, { 10, "plantingMethodObj.code" },
                        { 11, "plantingDistanceObj.code" }, { 12, "plantingDesignDensity" }, { 13, "typeOfTreeObj.code" },
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
                        { 19, "shavingTreeDensity" }, { 20, "vanhAverage" }, { 21, "ratioTreeObtain" }, { 22, "gardenRatingObj.code" }
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
            string active = item.ActiveStatusObj?.code;
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

        private static void SetFormula(IXLWorksheet ws, int row, int col, string formula, string numberFormat = "#,##0")
        {
            var formulaCell = ws.Cell(row, col);
            formulaCell.FormulaA1 = formula;
            formulaCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            formulaCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            formulaCell.Style.NumberFormat.Format = numberFormat;
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


        #region Farmer
        [PermissionAuthorize($"{Ro_Functions.TREEDIARY}.{Ro_Controllers.TD_SURVEY_FARM}.{Ro_Actions.READ}")]
        [HttpGet]
        public async Task<JsonResult> GetListFarmer(VM_DataTableFilter request)
        {
            var searchText = request.search["value"];
            var idxColumn = request.order["0"]["column"];
            var descString = request.order["0"]["dir"];
            int record = request.length;
            int page = (request.start + record) / record;
            string column = request.columns[idxColumn]["data"];
            bool desc = descString == "desc";
            var res = await _s_Farmer.GetListByPaging(_accessToken, "1", searchText, default, default, default, desc, column, default, page, record);
            var data2nd = res.data2nd.ToObject<VM_DataTableFilter.ResponseData2nd>();
            return Json(new
            {
                request.draw,
                res.data,
                data2nd.recordsFiltered,
                data2nd.recordsTotal,
                error = res.error.message
            });
        }

        [PermissionAuthorize($"{Ro_Functions.TREEDIARY}.{Ro_Controllers.TD_SURVEY_FARM}.{Ro_Actions.UPDATE}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> P_FarmerCreateOrUpdate(EM_Farmer model)
        {
            M_JResult jResult = new M_JResult();
            if (!ModelState.IsValid)
            {
                jResult.error = new error(0, DataAnnotationExtensionMethod.GetErrorMessage(ModelState));
                return Json(jResult);
            }
            if (model.Id > 0)
            {
                var accountType = _iHttpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccountType")?.Value;
                if (accountType == RolesConstants.STAFF)
                {
                    jResult.error.message = "Tài khoản Khảo Sát Viên không được phép sửa thông tin này!";
                    return Json(jResult);
                }
                var res = await _s_Farmer.Update(_accessToken, model, _accountId);
                return Json(jResult.MapData(res));
            }
            else
            {
                model.status = 1;
                var res = await _s_Farmer.Create(_accessToken, model, _accountId);
                return Json(jResult.MapData(res));
            }
        }

        [PermissionAuthorize($"{Ro_Functions.TREEDIARY}.{Ro_Controllers.TD_SURVEY_FARM}.{Ro_Actions.DELETE}")]
        [HttpDelete]
        public async Task<JsonResult> DeleteFarmer(int id)
        {
            M_JResult jResult = new M_JResult();
            var res = await _s_Farmer.Delete(_accessToken, id, _accountId);
            return Json(jResult.MapData(res));
        }

        //[PermissionAuthorize($"{Ro_Functions.TREEDIARY}.{Ro_Controllers.TD_SURVEY_FARM}.{Ro_Actions.UPDATE}")]
        //[HttpPut]
        //public async Task<JsonResult> UpdateRemark(int surveyFarmId, string remark)
        //{
        //    M_JResult jResult = new M_JResult();
        //    var res = await _s_SurveyFarm.UpdateRemark(_accessToken, surveyFarmId, remark, _accountId);
        //    return Json(jResult.MapData(res));
        //}

        [HttpGet]
        public async Task<JsonResult> GetListBySequenceStatusPhoneNumber(string phoneNumber)
        {
            M_JResult jResult = new M_JResult();
            var res = await _s_Farmer.GetListBySequenceStatusPhoneNumber(_accessToken, "0,1", phoneNumber);
            return Json(jResult.MapData(res));
        }
        #endregion

        #region SurveyTree
        //[HttpGet]
        //public async Task<JsonResult> GetListSurveyTree(int surveyFarmId)
        //{
        //    var res = await _s_SurveyTree.GetListBySequenceStatusPlaceMarkId(_accessToken, "1", surveyFarmId);
        //    return Json(new M_JResult(res));
        //}

        [PermissionAuthorize($"{Ro_Functions.TREEDIARY}.{Ro_Controllers.TD_SURVEY_FARM}.{Ro_Actions.UPDATE}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> CreateOrUpdateSurveyTree(EM_SurveyTree model)
        {
            M_JResult jResult = new M_JResult();
            if (!ModelState.IsValid)
            {
                jResult.error = new error(0, DataAnnotationExtensionMethod.GetErrorMessage(ModelState));
                return Json(jResult);
            }
            if (model.Id > 0)
            {
                var res = await _s_SurveyTree.Update(_accessToken, model, _accountId);
                return Json(jResult.MapData(res));
            }
            else
            {
                model.status = 1;
                var res = await _s_SurveyTree.Create(_accessToken, model, _accountId);
                return Json(jResult.MapData(res));
            }
        }

        [PermissionAuthorize($"{Ro_Functions.TREEDIARY}.{Ro_Controllers.TD_SURVEY_FARM}.{Ro_Actions.UPDATE}")]
        [HttpDelete]
        public async Task<JsonResult> DeleteSurveyTree(int id)
        {
            M_JResult jResult = new M_JResult();
            var res = await _s_SurveyTree.Delete(_accessToken, id, _accountId);
            return Json(jResult.MapData(res));
        }

        [HttpGet]
        public async Task<JsonResult> GetListSurveyTreeOlder(int placeMarkId, int surveyBatchId)
        {
            var res = await _s_SurveyTree.GetListBySequenceStatusPlaceMarkIdExcludeSurveyBatchId(_accessToken, "0,1", placeMarkId, surveyBatchId);
            return Json(new M_JResult(res));
        }
        #endregion
    }
}
