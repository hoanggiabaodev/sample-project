using Microsoft.AspNetCore.Mvc;
using Web_ProjectName.Services;
using Web_ProjectName.Models;

namespace Web_ProjectName.Controllers
{
    public class NewsController : BaseController<NewsController>
    {
        private readonly IS_News _s_News;
        private readonly IS_NewsCategory _s_NewsCategory;

        public NewsController(IS_News s_News, IS_NewsCategory s_NewsCategory)
        {
            _s_News = s_News;
            _s_NewsCategory = s_NewsCategory;
        }

        public async Task<IActionResult> Index(int page = 1, int? categoryId = null, string? keyword = null)
        {
            return View();
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

            return View(detail.data);
        }

        public async Task<IActionResult> Category(int id, int page = 1)
        {
            return View();
        }

        public async Task<IActionResult> Latest(int page = 1)
        {
            return View();
        }
        // top 3 view most
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