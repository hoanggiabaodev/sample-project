using Microsoft.AspNetCore.Mvc;
using Web_ProjectName.Services;
using Web_ProjectName.Models;
using Web_ProjectName.Lib;

namespace Web_ProjectName.Controllers
{
    public class ContactController : BaseController<ContactController>
    {
        private readonly IS_School _S_School;
        private readonly IS_Contact _S_Contact;

        public ContactController(IS_School S_School, IS_Contact S_Contact)
        {
            _S_School = S_School;
            _S_Contact = S_Contact;
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

        [HttpPost]
        public async Task<IActionResult> Create(EM_Contact model)
        {
            // Bỏ qua validate cho các trường audit (server sẽ tự set)
            ModelState.Remove("createdByObj");
            ModelState.Remove("updatedByObj");
            ModelState.Remove("Remark");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { result = 0, errors });
            }

            try
            {
                var res = await _S_Contact.Create(model, "1");
                return Json(new M_JResult(res));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi server: " + ex.Message);
            }
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
