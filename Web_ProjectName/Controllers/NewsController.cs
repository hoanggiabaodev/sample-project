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
            // Tạo dữ liệu mẫu để test
            var mockNews = new List<M_News>
            {
                new M_News
                {
                    id = 1,
                    name = "Tin tức mới nhất",
                    description = "Mô tả tin tức mới nhất với nhiều thông tin hấp dẫn",
                    detail = "Chi tiết tin tức mới nhất",
                    publishedAt = DateTime.Now.AddDays(-1),
                    viewNumber = 100,
                    isHot = true,
                    metaUrl = "tin-tuc-moi-nhat",
                    newsCategoryId = 1,
                    status = 1,
                    newsCategoryObj = new M_NewsCategory { id = 1, name = "Tin công nghệ" }
                },
                new M_News
                {
                    id = 2,
                    name = "Tin tức thứ hai",
                    description = "Mô tả tin tức thứ hai với nội dung thú vị",
                    detail = "Chi tiết tin tức thứ hai",
                    publishedAt = DateTime.Now.AddDays(-2),
                    viewNumber = 80,
                    isHot = false,
                    metaUrl = "tin-tuc-thu-hai",
                    newsCategoryId = 2,
                    status = 1,
                    newsCategoryObj = new M_NewsCategory { id = 2, name = "Tin thể thao" }
                },
                new M_News
                {
                    id = 3,
                    name = "Tin tức thứ ba",
                    description = "Mô tả tin tức thứ ba với thông tin mới nhất",
                    detail = "Chi tiết tin tức thứ ba",
                    publishedAt = DateTime.Now.AddDays(-3),
                    viewNumber = 120,
                    isHot = true,
                    metaUrl = "tin-tuc-thu-ba",
                    newsCategoryId = 3,
                    status = 1,
                    newsCategoryObj = new M_NewsCategory { id = 3, name = "Tin giải trí" }
                }
            };

            var mockCategories = new List<M_NewsCategory>
            {
                new M_NewsCategory { id = 1, name = "Tin công nghệ" },
                new M_NewsCategory { id = 2, name = "Tin thể thao" },
                new M_NewsCategory { id = 3, name = "Tin giải trí" }
            };

            ViewBag.NewsList = mockNews;
            ViewBag.Categories = mockCategories;
            ViewBag.HotNews = mockNews.Where(n => n.isHot).ToList();
            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = 1;
            ViewBag.CategoryId = categoryId;
            ViewBag.Keyword = keyword;
            ViewBag.TotalCount = mockNews.Count;

            return View();
        }

        public async Task<IActionResult> Detail(string metaUrl)
        {
            // Fallback với dữ liệu mẫu
            var mockNews = new M_News
            {
                id = 1,
                name = "Tin tức chi tiết",
                description = "Mô tả chi tiết tin tức với nhiều thông tin hấp dẫn",
                detail = "<p>Đây là nội dung chi tiết của tin tức. Có thể chứa HTML và các thẻ định dạng.</p><p>Đoạn văn thứ hai với nhiều thông tin hơn.</p><p>Đoạn văn thứ ba với các thông tin bổ sung.</p>",
                publishedAt = DateTime.Now.AddDays(-1),
                viewNumber = 150,
                isHot = true,
                metaUrl = metaUrl,
                newsCategoryId = 1,
                status = 1,
                newsCategoryObj = new M_NewsCategory { id = 1, name = "Tin công nghệ" }
            };

            ViewBag.NewsDetail = mockNews;
            return View();
        }

        public async Task<IActionResult> Category(int id, int page = 1)
        {
            var mockNews = new List<M_News>
            {
                new M_News
                {
                    id = 1,
                    name = "Tin tức danh mục",
                    description = "Mô tả tin tức trong danh mục",
                    detail = "Chi tiết tin tức trong danh mục",
                    publishedAt = DateTime.Now.AddDays(-1),
                    viewNumber = 100,
                    isHot = true,
                    metaUrl = "tin-tuc-danh-muc",
                    newsCategoryId = id,
                    status = 1,
                    newsCategoryObj = new M_NewsCategory { id = id, name = "Danh mục " + id }
                }
            };

            var category = new M_NewsCategory { id = id, name = "Danh mục " + id };

            ViewBag.NewsList = mockNews;
            ViewBag.Category = category;
            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = 1;
            ViewBag.TotalCount = mockNews.Count;

            return View();
        }

        public async Task<IActionResult> Latest(int page = 1)
        {
            var mockNews = new List<M_News>
            {
                new M_News
                {
                    id = 1,
                    name = "Tin tức mới nhất",
                    description = "Mô tả tin tức mới nhất",
                    detail = "Chi tiết tin tức mới nhất",
                    publishedAt = DateTime.Now.AddDays(-1),
                    viewNumber = 100,
                    isHot = true,
                    metaUrl = "tin-tuc-moi-nhat",
                    newsCategoryId = 1,
                    status = 1,
                    newsCategoryObj = new M_NewsCategory { id = 1, name = "Tin công nghệ" }
                }
            };

            ViewBag.NewsList = mockNews;
            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = 1;
            ViewBag.TotalCount = mockNews.Count;

            return View();
        }
    }
} 