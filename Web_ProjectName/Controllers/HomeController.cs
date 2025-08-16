using Microsoft.AspNetCore.Mvc;
using Web_ProjectName.Services;
using Web_ProjectName.Models;

namespace Web_ProjectName.Controllers
{
    public class HomeController : BaseController<HomeController>
    {
        private readonly IS_News _s_News;
        private readonly IS_NewsCategory _s_NewsCategory;

        public HomeController(IS_News s_News, IS_NewsCategory s_NewsCategory)
        {
            _s_News = s_News;
            _s_NewsCategory = s_NewsCategory;
        }

        public async Task<IActionResult> Index(int page = 1, int? categoryId = null, string? keyword = null)
        {
            const int pageSize = 5;

            try
            {
                var catRes = await _s_NewsCategory.GetListByPaging("0,1");
                ViewBag.Categories = (catRes.result == 1 && catRes.data != null) ? catRes.data : new List<M_NewsCategory>();
            }
            catch
            {
                ViewBag.Categories = new List<M_NewsCategory>();
            }

            var res = await _s_News.GetListByStatus(1);
            var all = (res.result == 1 && res.data != null) ? res.data : new List<M_News>();

            if (categoryId.HasValue)
            {
                all = all.Where(n => n.newsCategoryId == categoryId.Value).ToList();
            }
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLowerInvariant();
                all = all.Where(n => (n.name ?? string.Empty).ToLowerInvariant().Contains(kw)
                                   || (n.description ?? string.Empty).ToLowerInvariant().Contains(kw))
                         .ToList();
            }

            all = all.OrderByDescending(n => n.publishedAt).ToList();

            var totalCount = all.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.NewsList = items;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Keyword = keyword;
            ViewBag.CategoryId = categoryId;

            return View();
        }

        public async Task<IActionResult> GetList(int page = 1, int? categoryId = null, string? categoryIds = null, string? keyword = null)
        {
            const int pageSize = 5;
            var res = await _s_News.GetListByStatus(1);
            var all = (res.result == 1 && res.data != null) ? res.data : new List<M_News>();

            if (!string.IsNullOrWhiteSpace(categoryIds))
            {
                var categoryIdList = categoryIds.Split(',')
                                                .Select(id => int.TryParse(id, out var parsedId) ? parsedId : 0)
                                                .Where(id => id > 0)
                                                .ToList();
                if (categoryIdList.Any())
                {
                    all = all.Where(n => n.newsCategoryId.HasValue && categoryIdList.Contains(n.newsCategoryId.Value)).ToList();
                }
            }
            else if (categoryId.HasValue)
            {
                all = all.Where(n => n.newsCategoryId == categoryId.Value).ToList();
            }

            // Lọc keyword
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLowerInvariant();
                all = all.Where(n => (n.name ?? string.Empty).ToLowerInvariant().Contains(kw)
                                || (n.description ?? string.Empty).ToLowerInvariant().Contains(kw))
                        .ToList();
            }

            // Sắp xếp & phân trang
            all = all.OrderByDescending(n => n.publishedAt).ToList();
            var totalCount = all.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;
            var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Json(new
            {
                result = 1,
                data = items,
                totalPages,
                currentPage = page
            });
        }


        public async Task<IActionResult> Detail(string metaUrl)
        {
            if (string.IsNullOrEmpty(metaUrl))
                return View(null);

            var res = await _s_News.GetListByStatus(1);
            var newsDetail = res.data?.FirstOrDefault(x => x.metaUrl == metaUrl);
            var detail = await _s_News.GetById(null, newsDetail?.id ?? 0);
            if (res == null || res.result != 1 || res.data == null)
                return View(null);

            return View("P_View", detail.data);
        }

        public async Task<IActionResult> Category(int id, int page = 1)
        {
            return View();
        }

        public async Task<IActionResult> Latest(int page = 1)
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetMostViewed()
        {
            var res = await _s_News.GetListByStatus(1);

            if (res == null || res.result != 1 || res.data == null)
            {
                return Json(new
                {
                    result = 0,
                    message = "Không lấy được dữ liệu"
                });
            }

            var mostViewed = res.data
                .OrderByDescending(x => x.viewNumber)
                .Take(3)
                .ToList();

            return Json(new
            {
                result = 1,
                data = mostViewed
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetRelatedNews(string metaUrl)
        {
            var res = await _s_News.GetListByStatus(1);
            var newsDetail = res.data?.FirstOrDefault(x => x.metaUrl == metaUrl);

            if (newsDetail == null || res.data == null)
            {
                return Json(new
                {
                    result = 0,
                    message = "Không tìm thấy bài viết"
                });
            }

            var relatedNews = res.data
                .Where(x => x.newsCategoryId == newsDetail.newsCategoryId && x.id != newsDetail.id)
                // .Take(2)
                .ToList();

            return Json(new
            {
                result = 1,
                data = relatedNews
            });
        }

    }
}