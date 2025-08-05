using Microsoft.AspNetCore.Mvc;
using Web_ProjectName.Services;
namespace Web_ProjectName.Controllers
{
    public class NewController : BaseController<NewController>
    {
        private readonly IS_News _s_News;

        public NewController(IS_News s_News)
        {
            _s_News = s_News;
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
        public async Task<JsonResult> TestApi()
        {
            var res = await _s_News.GetListByPaging("1", _supplierId, null, null, 1, 10);
            return Json(new
            {
                result = res.result,
                dataType = res.data?.GetType()?.Name,
                data = res.data,
                supplierId = _supplierId
            });
        }

        [HttpGet]
        public async Task GetListNews()
        {
            // Get news list with pagination - status "1" means active, page 1, 10 records per page
            // Using schoolId instead of supplierId based on the sample data
            var res = await _s_News.GetListByPaging("1", _supplierId, null, "", 1, 10);

            // Debug logging
            System.Diagnostics.Debug.WriteLine($"API Response Result: {res.result}");
            System.Diagnostics.Debug.WriteLine($"API Response Data Type: {res.data?.GetType()}");
            System.Diagnostics.Debug.WriteLine($"API Response Data: {res.data}");

            if (res.result == 1 && res.data != null)
            {
                // Handle both single object and array responses
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
            try
            {
                var res = await _s_News.GetListByPaging("1", _supplierId, newsCategoryId, keyword, page, record);

                System.Diagnostics.Debug.WriteLine($"AJAX API Response Result: {res.result}");
                System.Diagnostics.Debug.WriteLine($"AJAX API Response Data Type: {res.data?.GetType()}");
                System.Diagnostics.Debug.WriteLine($"SupplierId: {_supplierId}");

                if (res.result != 1 || res.data == null)
                {
                    var mockData = new List<Models.M_News>
                    {
                        new Models.M_News
                        {
                            id = 1,
                            name = "Tin tức test 1",
                            description = "Mô tả tin tức test 1",
                            detail = "Chi tiết tin tức test 1",
                            publishedAt = DateTime.Now.AddDays(-1),
                            viewNumber = 100,
                            isHot = true,
                            metaUrl = "tin-tuc-test-1",
                            newsCategoryId = 1,
                            newsCategoryObj = new Models.M_NewsCategory { id = 1, name = "Tin công nghệ" }
                        },
                        new Models.M_News
                        {
                            id = 2,
                            name = "Tin tức test 2",
                            description = "Mô tả tin tức test 2",
                            detail = "Chi tiết tin tức test 2",
                            publishedAt = DateTime.Now.AddDays(-2),
                            viewNumber = 50,
                            isHot = false,
                            metaUrl = "tin-tuc-test-2",
                            newsCategoryId = 2,
                            newsCategoryObj = new Models.M_NewsCategory { id = 2, name = "Tin thể thao" }
                        },
                        new Models.M_News
                        {
                            id = 3,
                            name = "Tin tức test 3",
                            description = "Mô tả tin tức test 3",
                            detail = "Chi tiết tin tức test 3",
                            publishedAt = DateTime.Now.AddDays(-3),
                            viewNumber = 75,
                            isHot = true,
                            metaUrl = "tin-tuc-test-3",
                            newsCategoryId = 3,
                            newsCategoryObj = new Models.M_NewsCategory { id = 3, name = "Tin giải trí" }
                        }
                    };

                    var mockResponse = new
                    {
                        result = 1,
                        data = mockData,
                        dataCount = mockData.Count,
                        error = (string)null,
                        supplierId = _supplierId,
                        page = page,
                        record = record,
                        newsCategoryId = newsCategoryId,
                        keyword = keyword,
                        isMockData = true
                    };

                    return Json(mockResponse);
                }

                var debugInfo = new
                {
                    result = res.result,
                    data = res.data,
                    dataCount = res.data?.Count ?? 0,
                    error = res.error?.message,
                    supplierId = _supplierId,
                    page = page,
                    record = record,
                    newsCategoryId = newsCategoryId,
                    keyword = keyword,
                    isMockData = false
                };

                return Json(debugInfo);
            }
            catch (Exception ex)
            {
                var mockData = new List<Models.M_News>
                {
                    new Models.M_News
                    {
                        id = 1,
                        name = "Tin tức test (Exception)",
                        description = "Mô tả tin tức test khi có lỗi",
                        detail = "Chi tiết tin tức test khi có lỗi",
                        publishedAt = DateTime.Now.AddDays(-1),
                        viewNumber = 100,
                        isHot = true,
                        metaUrl = "tin-tuc-test-exception",
                        newsCategoryId = 1,
                        newsCategoryObj = new Models.M_NewsCategory { id = 1, name = "Tin công nghệ" }
                    }
                };

                return Json(new
                {
                    result = 1,
                    data = mockData,
                    dataCount = mockData.Count,
                    error = ex.Message,
                    supplierId = _supplierId,
                    page = page,
                    record = record,
                    newsCategoryId = newsCategoryId,
                    keyword = keyword,
                    isMockData = true,
                    exception = ex.Message
                });
            }
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
        public JsonResult Save([FromBody] Models.M_News news)
        {
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

                if (news.id == 0)
                {
                    news.createdAt = DateTime.Now;
                    news.publishedAt = news.publishedAt ?? DateTime.Now;
                }
                else
                {
                    news.updatedAt = DateTime.Now;
                }

                return Json(new
                {
                    result = 1,
                    message = news.id == 0 ? "Đã tạo tin tức thành công" : "Đã cập nhật tin tức thành công",
                    data = news
                });
            }
            catch (Exception ex)
            {
                return Json(new { result = 0, error = ex.Message });
            }
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
        public JsonResult GetNewsCategories()
        {
            try
            {
                var categories = new List<Models.M_NewsCategory>
                {
                    new Models.M_NewsCategory { id = 1, name = "Tin công nghệ" },
                    new Models.M_NewsCategory { id = 2, name = "Tin thể thao" },
                    new Models.M_NewsCategory { id = 3, name = "Tin giải trí" },
                    new Models.M_NewsCategory { id = 4, name = "Tin kinh tế" },
                    new Models.M_NewsCategory { id = 5, name = "Tin giáo dục" }
                };

                return Json(new { result = 1, data = categories });
            }
            catch (Exception ex)
            {
                return Json(new { result = 0, error = ex.Message });
            }
        }
    }
}