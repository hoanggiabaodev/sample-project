using Microsoft.AspNetCore.Mvc;
using Web_ProjectName.Services;

namespace Web_ProjectName.Controllers
{
    public class SchoolController : BaseController<SchoolController>
    {
        private readonly IS_School _S_School;

        public SchoolController(IS_School S_School)
        {
            _S_School = S_School;
        }

        public IActionResult Index()
        {
            var school = ViewBag.School;
            return View();
        }

        public async Task<IActionResult> GetById(int id)
        {
            var res = await _S_School.GetById(null, id);
            if (res.result == 1 && res.data != null)
            {
                return View("Index", res.data);
            }
            TempData["Error"] = "Không tìm thấy trường!";
            return RedirectToAction("Index");
        }
    }
}
