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
            var res = await _s_News.GetListByPaging("1", _supplierId, null, "", 1, 10);

            System.Diagnostics.Debug.WriteLine($"API Response Result: {res.result}");
            System.Diagnostics.Debug.WriteLine($"API Response Data Type: {res.data?.GetType()}");
            System.Diagnostics.Debug.WriteLine($"API Response Data: {res.data}");

            if (res.result == 1 && res.data != null)
            {
                if (res.data is List<Models.M_News> newsList)
                {
                    ViewBag.ListNews = newsList;
                    System.Diagnostics.Debug.WriteLine($"Found {newsList.Count} news items");
                }
                else
                {
                    // If it's a single object, wrap it in a list
                    ViewBag.ListNews = new List<Models.M_News>();
                    System.Diagnostics.Debug.WriteLine("Data is not a list, creating empty list");
                }
            }
            else
            {
                ViewBag.ListNews = new List<Models.M_News>();
                System.Diagnostics.Debug.WriteLine("No data returned from API");
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetListNewsByAjax(int page = 1, int record = 10, int? newsCategoryId = null, string? keyword = null, string dateFrom = null, string dateTo = null)
        {
            var res = await _s_News.GetListByPaging("1", _supplierId, newsCategoryId, keyword, page, record);
            return Json(res.data);
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
                    // Return mock data for testing
                    var mockNews = new Models.M_News
                    {
                        id = id,
                        name = $"Tin tức chi tiết {id}",
                        description = $"Mô tả chi tiết cho tin tức {id}",
                        detail = $"<p>Đây là nội dung chi tiết của tin tức {id}.</p><p>Bao gồm nhiều thông tin hữu ích cho người đọc.</p>",
                        publishedAt = DateTime.Now.AddDays(-id),
                        viewNumber = 100 + (id * 10),
                        isHot = id % 2 == 0,
                        metaUrl = $"tin-tuc-chi-tiet-{id}",
                        newsCategoryId = (id % 3) + 1,
                        newsCategoryObj = new Models.M_NewsCategory
                        {
                            id = (id % 3) + 1,
                            name = ((id % 3) + 1) switch
                            {
                                1 => "Tin công nghệ",
                                2 => "Tin thể thao",
                                _ => "Tin giải trí"
                            }
                        }
                    };

                    return Json(new { result = 1, data = mockNews });
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

                // Handle date parsing
                if (news.publishedAt == default)
                {
                    news.publishedAt = DateTime.Now;
                }
                else
                {
                    // Ensure the date is properly set
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

                // Generate meta URL if not provided
                if (string.IsNullOrEmpty(news.metaUrl))
                {
                    news.metaUrl = GenerateMetaUrl(news.name);
                }

                // Set default values for required fields
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

                // Convert EM_News_API to EM_News for service call
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
                    return Json(new { result = 1, message = "Đã tạo tin tức thành công", data = res.data });
                }
                else
                {
                    return Json(new { result = 0, error = res.error?.message ?? "Có lỗi xảy ra khi tạo tin tức" });
                }
                // return Json(new
                // {
                //     result = 1,
                //     message = news.id == 0 ? "Đã tạo tin tức thành công" : "Đã cập nhật tin tức thành công",
                //     data = news
                // });
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

                // Set default values
                news.supplierId = Convert.ToInt32(_supplierId);
                news.status = 1;

                // Handle date parsing
                if (news.publishedAt == default)
                {
                    news.publishedAt = DateTime.Now;
                }
                else
                {
                    // Ensure the date is properly set
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

                // Generate meta URL if not provided
                if (string.IsNullOrEmpty(news.metaUrl))
                {
                    news.metaUrl = GenerateMetaUrl(news.name);
                }

                // Set default values for required fields
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

                System.Diagnostics.Debug.WriteLine($"Controller Update - News ID: {news.id}");
                System.Diagnostics.Debug.WriteLine($"Controller Update - News Name: {news.name}");
                System.Diagnostics.Debug.WriteLine($"Controller Update - Published At: {news.publishedAt}");

                // Convert EM_News_API to EM_News for service call
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

                // Call the service to update
                var res = await _s_News.Update("", newsForService, "", "0");

                if (res.result == 1)
                {
                    return Json(new
                    {
                        result = 1,
                        message = "Đã cập nhật tin tức thành công",
                        data = res.data
                    });
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
        public JsonResult Delete(int id)
        {
            try
            {
                return Json(new
                {
                    result = 1,
                    message = "Đã xóa tin tức thành công",
                    data = new { id = id }
                });
            }
            catch (Exception ex)
            {
                return Json(new { result = 0, error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCategories()
        {
            System.Diagnostics.Debug.WriteLine("GetCategories method called");
            try
            {
                // Get categories with status = 0 and status = 1 using GetListByPaging
                var res = await _s_NewsCategory.GetListByPaging("0,1");

                System.Diagnostics.Debug.WriteLine($"GetCategories API Response Result: {res.result}");
                System.Diagnostics.Debug.WriteLine($"GetCategories API Response Data Count: {res.data?.Count ?? 0}");
                System.Diagnostics.Debug.WriteLine($"GetCategories API Response Error: {res.error?.message}");

                if (res.result == 1 && res.data != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Returning {res.data.Count} categories from API");
                    return Json(new { result = 1, data = res.data });
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("API failed, returning mock data");
                    // Return mock data if API fails
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
                System.Diagnostics.Debug.WriteLine($"Exception in GetCategories: {ex.Message}");
                // Return mock data if exception occurs
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

                System.Diagnostics.Debug.WriteLine($"GetListByStatus API Response Result: {res.result}");
                System.Diagnostics.Debug.WriteLine($"GetListByStatus API Response Data Count: {res.data?.Count ?? 0}");
                System.Diagnostics.Debug.WriteLine($"Status parameter: {status}");

                if (res.result == 1 && res.data != null)
                {
                    return Json(new
                    {
                        result = 1,
                        data = res.data,
                        dataCount = res.data.Count,
                        status = status,
                        error = (string)null
                    });
                }
                else
                {
                    // Return mock data if API fails
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
                        data = mockData,
                        dataCount = mockData.Count,
                        status = status,
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
                    status = status,
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

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var fileExtension = Path.GetExtension(upload.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return Json(new { uploaded = 0, error = new { message = "Chỉ cho phép upload file ảnh (jpg, jpeg, png, gif, bmp)" } });
                }

                // Validate file size (max 5MB)
                if (upload.Length > 5 * 1024 * 1024)
                {
                    return Json(new { uploaded = 0, error = new { message = "File quá lớn. Kích thước tối đa là 5MB" } });
                }

                // Generate unique filename
                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var uploadPath = Path.Combine("wwwroot", "uploads", "images");

                // Create directory if not exists
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await upload.CopyToAsync(stream);
                }

                // Return success response for CKEditor
                return Json(new
                {
                    uploaded = 1,
                    fileName = fileName,
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