using Microsoft.AspNetCore.Mvc;
using Web_ProjectName.Services;
namespace Web_ProjectName.Controllers
{
    public class NewController : BaseController<NewController>
    {
        private readonly IS_News _s_News;
        private readonly IS_NewsCategory _s_NewsCategory;

        public NewController(IS_News s_News, IS_NewsCategory s_NewsCategory)
        {
            _s_News = s_News;
            _s_NewsCategory = s_NewsCategory;
        }

        public async Task<IActionResult> Index()
        {
            await GetListNews();
            return View();
        }

        public async Task<IActionResult> ViewDetail(int id)
        {
            var res = await _s_News.GetById(null, id);
            if (res.result == 1 && res.data != null)
            {
                ViewBag.NewsDetail = res.data;
                return View();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task GetListNews()
        {
            var res = await _s_News.GetListByPaging("0,1", _supplierId, null, "", 1, 10);

            if (res.result == 1 && res.data != null)
            {
                if (res.data is List<Models.M_News> newsList)
                {
                    ViewBag.ListNews = newsList;
                }
                else
                {
                    ViewBag.ListNews = new List<Models.M_News>();
                }
            }
            else
            {
                ViewBag.ListNews = new List<Models.M_News>();
                // System.Diagnostics.Debug.WriteLine("No data returned from API");
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetListNewsByAjax(int page = 1, int record = 10, int? newsCategoryId = null, string? keyword = null, string dateFrom = null, string dateTo = null)
        {
            var res = await _s_News.GetListByPaging("0,1", _supplierId, newsCategoryId, keyword, page, record);
            return Json(new { result = 1, res.data });
        }

        [HttpGet]
        public async Task<JsonResult> GetById(int id)
        {
            try
            {
                var res = await _s_News.GetById(null, id);
                if (res.result == 1 && res.data != null)
                {
                    return Json(new { result = 1, data = res.data });
                }
                else
                {
                    return Json(new { result = 0, error = "Không tìm thấy tin tức" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = 0, error = ex.Message });
            }
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<JsonResult> Save([FromBody] Models.EM_News_API news)
        {
            System.Diagnostics.Debug.WriteLine($"Save method called with news: {System.Text.Json.JsonSerializer.Serialize(news)}");
            try
            {
                if (string.IsNullOrEmpty(news.name))
                {
                    return Json(new { result = 0, error = "Tiêu đề tin tức không được để trống" });
                }

                if (news.newsCategoryId <= 0)
                {
                    return Json(new { result = 0, error = "Vui lòng chọn danh mục" });
                }

                news.supplierId = Convert.ToInt32(_supplierId);
                news.status = 1;
                news.createdAt = DateTime.Now;
                news.updatedAt = DateTime.Now;

                if (news.publishedAt == default)
                {
                    news.publishedAt = DateTime.Now;
                }
                else
                {
                    try
                    {
                        if (news.publishedAt.Kind == DateTimeKind.Unspecified)
                        {
                            news.publishedAt = DateTime.SpecifyKind(news.publishedAt, DateTimeKind.Local);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Date parsing error: {ex.Message}");
                        news.publishedAt = DateTime.Now;
                    }
                }

                if (string.IsNullOrEmpty(news.metaUrl))
                {
                    news.metaUrl = GenerateMetaUrl(news.name);
                }

                if (string.IsNullOrEmpty(news.metaKeywords))
                    news.metaKeywords = "";
                if (string.IsNullOrEmpty(news.metaDescription))
                    news.metaDescription = news.description ?? "";
                if (string.IsNullOrEmpty(news.metaTitle))
                    news.metaTitle = news.name ?? "";
                if (string.IsNullOrEmpty(news.metaImagePreview))
                    news.metaImagePreview = "";
                if (news.imageId == null)
                    news.imageId = 0;

                var newsForService = new Models.EM_News
                {
                    id = news.id,
                    name = news.name ?? "",
                    newsCategoryId = news.newsCategoryId,
                    supplierId = news.supplierId,
                    description = news.description ?? "",
                    detail = news.detail,
                    isHot = news.isHot,
                    viewNumber = news.viewNumber,
                    publishedAt = news.publishedAt,
                    imageId = news.imageId,
                    metaKeywords = news.metaKeywords,
                    metaDescription = news.metaDescription,
                    metaTitle = news.metaTitle,
                    metaUrl = news.metaUrl,
                    metaImagePreview = news.metaImagePreview,
                    status = news.status ?? 1
                };

                var res = await _s_News.Create("", newsForService, "", "0");
                if (res.result == 1)
                {
                    return Json(new { result = 1, message = "Đã tạo tin tức thành công", res.data });
                }
                else
                {
                    return Json(new { result = 0, error = res.error?.message ?? "Có lỗi xảy ra khi tạo tin tức" });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Save method error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { result = 0, error = ex.Message });
            }
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<JsonResult> Update([FromBody] Models.EM_News_API news)
        {
            System.Diagnostics.Debug.WriteLine($"Update method called with news: {System.Text.Json.JsonSerializer.Serialize(news)}");
            try
            {
                if (string.IsNullOrEmpty(news.name))
                {
                    return Json(new { result = 0, error = "Tiêu đề tin tức không được để trống" });
                }

                if (news.newsCategoryId <= 0)
                {
                    return Json(new { result = 0, error = "Vui lòng chọn danh mục" });
                }

                if (news.id <= 0)
                {
                    return Json(new { result = 0, error = "ID tin tức không hợp lệ" });
                }

                news.supplierId = Convert.ToInt32(_supplierId);
                news.status = news.status ?? 1;

                if (news.publishedAt == default)
                {
                    news.publishedAt = DateTime.Now;
                }
                else
                {
                    try
                    {
                        if (news.publishedAt.Kind == DateTimeKind.Unspecified)
                        {
                            news.publishedAt = DateTime.SpecifyKind(news.publishedAt, DateTimeKind.Local);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Date parsing error: {ex.Message}");
                        news.publishedAt = DateTime.Now;
                    }
                }

                if (string.IsNullOrEmpty(news.metaUrl))
                {
                    news.metaUrl = GenerateMetaUrl(news.name);
                }

                if (string.IsNullOrEmpty(news.metaKeywords))
                    news.metaKeywords = "";
                if (string.IsNullOrEmpty(news.metaDescription))
                    news.metaDescription = news.description ?? "";
                if (string.IsNullOrEmpty(news.metaTitle))
                    news.metaTitle = news.name ?? "";
                if (string.IsNullOrEmpty(news.metaImagePreview))
                    news.metaImagePreview = "";
                if (news.imageId == null)
                    news.imageId = 0;

                var newsForService = new Models.EM_News
                {
                    id = news.id,
                    name = news.name ?? "",
                    newsCategoryId = news.newsCategoryId,
                    supplierId = news.supplierId,
                    description = news.description ?? "",
                    detail = news.detail,
                    isHot = news.isHot,
                    viewNumber = news.viewNumber,
                    publishedAt = news.publishedAt,
                    imageId = news.imageId,
                    metaKeywords = news.metaKeywords,
                    metaDescription = news.metaDescription,
                    metaTitle = news.metaTitle,
                    metaUrl = news.metaUrl,
                    metaImagePreview = news.metaImagePreview,
                    status = news.status ?? 1
                };

                var res = await _s_News.Update("", newsForService, "", "0");

                if (res.result == 1)
                {
                    return Json(new { result = 1, message = "Đã cập nhật tin tức thành công", res.data });
                }
                else
                {
                    return Json(new
                    {
                        result = 0,
                        error = res.error?.message ?? "Có lỗi xảy ra khi cập nhật tin tức"
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Controller Update Exception: {ex.Message}");
                return Json(new { result = 0, error = ex.Message });
            }
        }

        private string GenerateMetaUrl(string title)
        {
            if (string.IsNullOrEmpty(title)) return "";

            return title
                .ToLower()
                .Replace("à", "a").Replace("á", "a").Replace("ạ", "a").Replace("ả", "a").Replace("ã", "a")
                .Replace("â", "a").Replace("ầ", "a").Replace("ấ", "a").Replace("ậ", "a").Replace("ẩ", "a").Replace("ẫ", "a")
                .Replace("ă", "a").Replace("ằ", "a").Replace("ắ", "a").Replace("ặ", "a").Replace("ẳ", "a").Replace("ẵ", "a")
                .Replace("è", "e").Replace("é", "e").Replace("ẹ", "e").Replace("ẻ", "e").Replace("ẽ", "e")
                .Replace("ê", "e").Replace("ề", "e").Replace("ế", "e").Replace("ệ", "e").Replace("ể", "e").Replace("ễ", "e")
                .Replace("ì", "i").Replace("í", "i").Replace("ị", "i").Replace("ỉ", "i").Replace("ĩ", "i")
                .Replace("ò", "o").Replace("ó", "o").Replace("ọ", "o").Replace("ỏ", "o").Replace("õ", "o")
                .Replace("ô", "o").Replace("ồ", "o").Replace("ố", "o").Replace("ộ", "o").Replace("ổ", "o").Replace("ỗ", "o")
                .Replace("ơ", "o").Replace("ờ", "o").Replace("ớ", "o").Replace("ợ", "o").Replace("ở", "o").Replace("ỡ", "o")
                .Replace("ù", "u").Replace("ú", "u").Replace("ụ", "u").Replace("ủ", "u").Replace("ũ", "u")
                .Replace("ư", "u").Replace("ừ", "u").Replace("ứ", "u").Replace("ự", "u").Replace("ử", "u").Replace("ữ", "u")
                .Replace("ỳ", "y").Replace("ý", "y").Replace("ỵ", "y").Replace("ỷ", "y").Replace("ỹ", "y")
                .Replace("đ", "d")
                .Replace(" ", "-")
                .Replace("--", "-")
                .Trim('-');
        }

        [HttpDelete]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var res = await _s_News.UpdateStatus("", id, 0, "");
                if (res.result == 1)
                {
                    return Json(new { result = 1, message = "Đã xóa tin tức thành công", res.data });
                }
                else
                {
                    return Json(new { result = 0, error = res.error?.message ?? "Có lỗi xảy ra khi xóa tin tức" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = 0, error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCategories()
        {
            try
            {
                var res = await _s_NewsCategory.GetListByPaging("0,1");

                if (res.result == 1 && res.data != null)
                {
                    return Json(new { result = 1, res.data });
                }
                else
                {
                    var mockCategories = new List<Models.M_NewsCategory>
                    {
                        new Models.M_NewsCategory { id = 1, name = "Tin công nghệ" },
                        new Models.M_NewsCategory { id = 2, name = "Tin thể thao" },
                        new Models.M_NewsCategory { id = 3, name = "Tin giải trí" },
                        new Models.M_NewsCategory { id = 4, name = "Tin kinh tế" },
                        new Models.M_NewsCategory { id = 5, name = "Tin giáo dục" }
                    };

                    return Json(new
                    {
                        result = 1,
                        data = mockCategories,
                        error = "API returned no data for both status 0 and 1",
                        isMockData = true
                    });
                }
            }
            catch (Exception ex)
            {
                var mockCategories = new List<Models.M_NewsCategory>
                {
                    new Models.M_NewsCategory { id = 1, name = "Tin công nghệ" },
                    new Models.M_NewsCategory { id = 2, name = "Tin thể thao" },
                    new Models.M_NewsCategory { id = 3, name = "Tin giải trí" },
                    new Models.M_NewsCategory { id = 4, name = "Tin kinh tế" },
                    new Models.M_NewsCategory { id = 5, name = "Tin giáo dục" }
                };

                return Json(new
                {
                    result = 1,
                    data = mockCategories,
                    error = ex.Message,
                    isMockData = true,
                    exception = ex.Message
                });
            }
        }



        [HttpGet]
        public async Task<JsonResult> GetListByStatus(int status = 1)
        {
            try
            {
                var res = await _s_News.GetListByStatus(status);

                if (res.result == 1 && res.data != null)
                {
                    return Json(new
                    {
                        result = 1,
                        res.data,
                        dataCount = res.data?.Count ?? 0,
                        status,
                    });
                }
                else
                {
                    var mockData = new List<Models.M_News>
                    {
                        new Models.M_News
                        {
                            id = 1,
                            name = "Tin tức theo trạng thái",
                            description = "Mô tả tin tức theo trạng thái",
                            detail = "Chi tiết tin tức theo trạng thái",
                            publishedAt = DateTime.Now.AddDays(-1),
                            viewNumber = 100,
                            isHot = true,
                            metaUrl = "tin-tuc-theo-trang-thai",
                            newsCategoryId = 1,
                            status = status,
                            newsCategoryObj = new Models.M_NewsCategory { id = 1, name = "Tin công nghệ" }
                        }
                    };

                    return Json(new
                    {
                        result = 1,
                        res.data,
                        dataCount = res.data?.Count ?? 0,
                        status,
                        error = res.error?.message ?? "API returned no data",
                        isMockData = true
                    });
                }
            }
            catch (Exception ex)
            {
                var mockData = new List<Models.M_News>
                {
                    new Models.M_News
                    {
                        id = 1,
                        name = "Tin tức theo trạng thái (Exception)",
                        description = "Mô tả tin tức khi có lỗi",
                        detail = "Chi tiết tin tức khi có lỗi",
                        publishedAt = DateTime.Now.AddDays(-1),
                        viewNumber = 100,
                        isHot = true,
                        metaUrl = "tin-tuc-theo-trang-thai-exception",
                        newsCategoryId = 1,
                        status = status,
                        newsCategoryObj = new Models.M_NewsCategory { id = 1, name = "Tin công nghệ" }
                    }
                };

                return Json(new
                {
                    result = 1,
                    data = mockData,
                    dataCount = mockData.Count,
                    status,
                    error = ex.Message,
                    isMockData = true,
                    exception = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> UploadImage(IFormFile upload)
        {
            try
            {
                if (upload == null || upload.Length == 0)
                {
                    return Json(new { uploaded = 0, error = new { message = "Không có file được upload" } });
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var fileExtension = Path.GetExtension(upload.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return Json(new { uploaded = 0, error = new { message = "Chỉ cho phép upload file ảnh (jpg, jpeg, png, gif, bmp)" } });
                }

                if (upload.Length > 5 * 1024 * 1024)
                {
                    return Json(new { uploaded = 0, error = new { message = "File quá lớn. Kích thước tối đa là 5MB" } });
                }

                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var uploadPath = Path.Combine("wwwroot", "uploads", "images");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await upload.CopyToAsync(stream);
                }

                return Json(new
                {
                    uploaded = 1,
                    fileName,
                    url = $"/uploads/images/{fileName}"
                });
            }
            catch (Exception ex)
            {
                return Json(new { uploaded = 0, error = new { message = "Lỗi khi upload file: " + ex.Message } });
            }
        }
    }
}