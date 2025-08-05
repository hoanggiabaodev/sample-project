using Web_ProjectName.Lib;
using Web_ProjectName.Models;
using static System.String;

namespace Web_ProjectName.Services
{
    public interface IS_News
    {
        Task<ResponseData<List<M_News>>> GetListBySequenceStatusSupplierIdNewsCategoryId(string accessToken, string sequenceStatus, string supplierId, int? newsCategoryId, DateTime? fdate, DateTime? tdate);
        Task<ResponseData<List<M_News>>> GetListBySequenceStatusSupplierIdSearchText(string accessToken, string sequenceStatus, string supplierId, string searchText);
        Task<ResponseData<List<M_News>>> GetListByPaging(string sequenceStatus, string supplierId, int? newsCategoryId, string? keyword, int page = 1, int record = 10);
        Task<ResponseData<List<M_News>>> GetListByStatus(int status);
        Task<ResponseData<M_News>> GetById(string? accessToken, int id);
        Task<ResponseData<M_News>> GetByMetaUrl(string metaUrl);
        Task<ResponseData<M_News>> Create(string accessToken, EM_News model, string refCode, string createdBy);
        Task<ResponseData<M_News>> Update(string accessToken, EM_News model, string refCode, string updatedBy);
        Task<ResponseData<M_News>> Delete(string accessToken, int id, string updatedBy);
        Task<ResponseData<M_News>> UpdateStatus(string accessToken, int id, int status, string updatedBy);
        Task<ResponseData<List<M_News>>> UpdateStatusList(string accessToken, string sequenceIds, int status, string updatedBy);
    }
    public class S_News : IS_News
    {
        private readonly ICallBaseApi _callApi;
        private readonly IS_Image _s_Image;
        public S_News(ICallBaseApi callApi, IS_Image image)
        {
            _callApi = callApi;
            _s_Image = image;
        }

        public async Task<ResponseData<List<M_News>>> GetListBySequenceStatusSupplierIdNewsCategoryId(string accessToken, string sequenceStatus, string supplierId, int? newsCategoryId, DateTime? fdate, DateTime? tdate)
        {
            Dictionary<string, dynamic> dictPars = new Dictionary<string, dynamic>
            {
                {"sequenceStatus", sequenceStatus},
                {"supplierId", supplierId},
                {"newsCategoryId", newsCategoryId ?? 0},
                {"fdate", fdate?.ToString("O") ?? ""},
                {"tdate", tdate?.ToString("O") ?? ""},
            };
            return await _callApi.GetResponseDataAsync<List<M_News>>("News/GetListBySequenceStatusSupplierIdNewsCategoryId", dictPars, accessToken);
        }
        public async Task<ResponseData<List<M_News>>> GetListBySequenceStatusSupplierIdSearchText(string accessToken, string sequenceStatus, string supplierId, string searchText)
        {
            Dictionary<string, dynamic> dictPars = new Dictionary<string, dynamic>
            {
                {"sequenceStatus", sequenceStatus},
                {"supplierId", supplierId},
                {"searchText", searchText},
            };
            return await _callApi.GetResponseDataAsync<List<M_News>>("News/GetListBySequenceStatusSupplierIdSearchText", dictPars, accessToken);
        }
        public async Task<ResponseData<List<M_News>>> GetListByPaging(string sequenceStatus, string supplierId, int? newsCategoryId, string? keyword, int page = 1, int record = 10)
        {
            Dictionary<string, dynamic> dictPars = new Dictionary<string, dynamic>
            {
                {"sequenceStatus", sequenceStatus},
                {"supplierId", supplierId},
                {"newsCategoryId", newsCategoryId ?? 0},
                {"keyword", keyword ?? ""},
                {"page", page},
                {"record", record},
            };
            return await _callApi.GetResponseDataAsync<List<M_News>>("News/GetListByPaging", dictPars);
        }

        public async Task<ResponseData<List<M_News>>> GetListByStatus(int status)
        {
            Dictionary<string, dynamic> dictPars = new Dictionary<string, dynamic>
            {
                {"status", status},
            };
            return await _callApi.GetResponseDataAsync<List<M_News>>("News/GetListByStatus", dictPars);
        }

        public async Task<ResponseData<M_News>> GetById(string? accessToken, int id)
        {
            Dictionary<string, dynamic> dictPars = new Dictionary<string, dynamic>
            {
                {"id", id},
            };
            return await _callApi.GetResponseDataAsync<M_News>("News/GetById", dictPars, accessToken ?? "");
        }
        public async Task<ResponseData<M_News>> GetByMetaUrl(string metaUrl)
        {
            Dictionary<string, dynamic> dictPars = new Dictionary<string, dynamic>
            {
                {"metaUrl", metaUrl},
            };
            return await _callApi.GetResponseDataAsync<M_News>("News/GetByMetaUrl", dictPars);
        }
        public async Task<ResponseData<M_News>> Create(string accessToken, EM_News model, string refCode, string createdBy)
        {
            model = CleanXSSHelper.CleanXSSObject(model); //Clean XSS
            //ResponseData<M_News> res = new ResponseData<M_News>();
            //if (model.imageFile != null)
            //{
            //    var imgUpload = await _s_Image.UploadResize(model.imageFile, refCode, Empty, createdBy);
            //    if (imgUpload.result == 1 && imgUpload.data != null)
            //    {
            //        model.imageId = imgUpload.data.id;
            //    }
            //    else
            //    {
            //        res.result = imgUpload.result; res.error = imgUpload.error;
            //        return res;
            //    }
            //}
            Dictionary<string, dynamic> dictPars = new Dictionary<string, dynamic>
            {
                {"name", model.name},
                {"newsCategoryId", model.newsCategoryId},
                {"supplierId", model.supplierId},
                {"description", model.description},
                {"detail", model.detail},
                {"isHot", model.isHot},
                {"viewNumber", model.viewNumber},
                {"imageId", model.imageId},
                {"publishedAt", model.publishedAt.ToString("O")},
                {"metaKeywords", model.metaKeywords},
                {"metaDescription", model.metaDescription},
                {"metaTitle", model.metaTitle},
                {"metaUrl", model.metaUrl},
                {"metaImagePreview", model.metaImagePreview},
                {"status", model.status},
                {"createdBy", createdBy},
            };
            return await _callApi.PostResponseDataAsync<M_News>("News/Create", dictPars, accessToken);
        }
        public async Task<ResponseData<M_News>> Update(string accessToken, EM_News model, string refCode, string updatedBy)
        {
            try
            {
                model = CleanXSSHelper.CleanXSSObject(model); //Clean XSS
                                                              //ResponseData<M_News> res = new ResponseData<M_News>();
                                                              //if (model.imageFile != null)
                                                              //{
                                                              //    var imgUpload = await _s_Image.UploadResize(model.imageFile, refCode, Empty, updatedBy);
                                                              //    if (imgUpload.result == 1 && imgUpload.data != null)
                                                              //    {
                                                              //        model.imageId = imgUpload.data.id;
                                                              //    }
                                                              //    else
                                                              //    {
                                                              //        res.result = imgUpload.result; res.error = imgUpload.error;
                                                              //        return res;
                                                              //    }
                                                              //}

                // Validate required fields
                if (model.id <= 0)
                {
                    return new ResponseData<M_News>
                    {
                        result = 0,
                        error = new error { code = -1, message = "ID tin tức không hợp lệ" }
                    };
                }

                Dictionary<string, dynamic> dictPars = new Dictionary<string, dynamic>
                {
                    {"Id", model.id ?? 0},
                    {"SchoolId", model.supplierId ?? 0},
                    {"NewsCategoryId", model.newsCategoryId ?? 0},
                    {"Name", model.name ?? ""},
                    {"Description", model.description ?? ""},
                    {"Detail", model.detail ?? ""},
                    {"IsHot", model.isHot},
                    {"ViewNumber", model.viewNumber ?? 0},
                    {"PublishedAt", model.publishedAt.ToString("yyyy-MM-ddTHH:mm:ss")},
                    {"MetaUrl", model.metaUrl ?? ""},
                    {"Status", model.status ?? 0},
                    {"MetaKeywords", model.metaKeywords ?? ""},
                    {"MetaDescription", model.metaDescription ?? ""},
                    {"MetaTitle", model.metaTitle ?? ""},
                    {"MetaImagePreview", model.metaImagePreview ?? ""},
                    {"ImageId", model.imageId ?? 0}
                };

                System.Diagnostics.Debug.WriteLine($"News Update API Call - URL: News/Update");
                System.Diagnostics.Debug.WriteLine($"News Update Parameters: {System.Text.Json.JsonSerializer.Serialize(dictPars)}");
                System.Diagnostics.Debug.WriteLine($"Access Token: {accessToken ?? "NULL"}");

                try
                {
                    var result = await _callApi.PutResponseDataAsync<M_News>("News/Update", dictPars, accessToken ?? "");
                    System.Diagnostics.Debug.WriteLine($"News Update API Response: {System.Text.Json.JsonSerializer.Serialize(result)}");
                    return result;
                }
                catch (HttpRequestException httpEx)
                {
                    System.Diagnostics.Debug.WriteLine($"HTTP Request Exception: {httpEx.Message}");
                    System.Diagnostics.Debug.WriteLine($"HTTP Status Code: {httpEx.StatusCode}");
                    return new ResponseData<M_News>
                    {
                        result = -1,
                        error = new error { code = -1, message = $"Lỗi kết nối HTTP: {httpEx.Message}" }
                    };
                }
                catch (TaskCanceledException taskEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Task Canceled Exception: {taskEx.Message}");
                    return new ResponseData<M_News>
                    {
                        result = -1,
                        error = new error { code = -1, message = "Lỗi timeout kết nối đến máy chủ" }
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"News Update Exception: {ex.Message}");
                return new ResponseData<M_News>
                {
                    result = -1,
                    error = new error { code = -1, message = $"Lỗi cập nhật tin tức: {ex.Message}" }
                };
            }
        }
        public async Task<ResponseData<M_News>> Delete(string accessToken, int id, string updatedBy)
        {
            Dictionary<string, dynamic> dictPars = new Dictionary<string, dynamic>
            {
                {"id", id},
                {"updatedBy", updatedBy},
            };
            return await _callApi.DeleteResponseDataAsync<M_News>("News/Delete", dictPars, accessToken);
        }
        public async Task<ResponseData<M_News>> UpdateStatus(string accessToken, int id, int status, string updatedBy)
        {
            Dictionary<string, dynamic> dictPars = new Dictionary<string, dynamic>
            {
                {"id", id},
                {"status", status},
                {"updatedBy", updatedBy},
            };
            return await _callApi.PutResponseDataAsync<M_News>("News/UpdateStatus", dictPars, accessToken);
        }
        public async Task<ResponseData<List<M_News>>> UpdateStatusList(string accessToken, string sequenceIds, int status, string updatedBy)
        {
            Dictionary<string, dynamic> dictPars = new Dictionary<string, dynamic>
            {
                {"sequenceIds", sequenceIds},
                {"status", status},
                {"updatedBy", updatedBy},
            };
            return await _callApi.PutResponseDataAsync<List<M_News>>("News/UpdateStatusList", dictPars, accessToken);
        }
    }
}
