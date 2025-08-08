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
            return View();
        }

        public async Task<IActionResult> Category(int id, int page = 1)
        {
            return View();
        }

        public async Task<IActionResult> Latest(int page = 1)
        {
            return View();
        }
    }
} 