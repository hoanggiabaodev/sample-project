using Microsoft.AspNetCore.Mvc;
using Web_ProjectName.Services;
using Web_ProjectName.Models;

namespace Web_ProjectName.Controllers
{
    public class NewCategoryController : BaseController<NewCategoryController>
    {
        private readonly IS_NewsCategory _s_NewsCategory;

        public NewCategoryController(IS_NewsCategory s_NewsCategory)
        {
            _s_NewsCategory = s_NewsCategory;
        }

        public async Task<IActionResult> Index()
        {
            await GetList();
            return View();
        }

        [HttpGet]
        public async Task GetList()
        {
            var res = await _s_NewsCategory.GetListByPaging("0,1");
            if (res.result == 1 && res.data != null)
                ViewBag.ListCategories = res.data;
            else
                ViewBag.ListCategories = new List<M_NewsCategory>();
        }

        [HttpGet]
        public async Task<JsonResult> P_View(int id)
        {
            var res = await _s_NewsCategory.GetById(id);
            if (res.result == 1 && res.data != null)
                return Json(new { result = 1, data = res.data });
            return Json(new { result = 0, error = "Không tìm thấy danh mục" });
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<JsonResult> P_Add([FromBody] EM_NewsCategory category)
        {
            try
            {
                if (string.IsNullOrEmpty(category.name))
                    return Json(new { result = 0, error = "Tên danh mục không được để trống" });

                category.supplierId = Convert.ToInt32(_supplierId);
                category.status = category.status ?? 1;
                category.createdAt = DateTime.Now;
                category.updatedAt = DateTime.Now;

                var res = await _s_NewsCategory.Create("", category, "");
                if (res.result == 1)
                    return Json(new { result = 1, message = "Tạo danh mục thành công", res.data });

                return Json(new { result = 0, error = res.error?.message ?? "Lỗi khi tạo danh mục" });
            }
            catch (Exception ex)
            {
                return Json(new { result = 0, error = ex.Message });
            }
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<JsonResult> P_Edit([FromBody] EM_NewsCategory category)
        {
            try
            {
                if (category.id <= 0)
                    return Json(new { result = 0, error = "ID không hợp lệ" });

                if (string.IsNullOrEmpty(category.name))
                    return Json(new { result = 0, error = "Tên danh mục không được để trống" });

                category.supplierId = Convert.ToInt32(_supplierId);
                category.updatedAt = DateTime.Now;

                var res = await _s_NewsCategory.Update("", category, "");
                if (res.result == 1)
                    return Json(new { result = 1, message = "Cập nhật danh mục thành công", res.data });

                return Json(new { result = 0, error = res.error?.message ?? "Lỗi khi cập nhật danh mục" });
            }
            catch (Exception ex)
            {
                return Json(new { result = 0, error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var res = await _s_NewsCategory.Delete("", id, "");
                if (res.result == 1)
                    return Json(new { result = 1, message = "Xóa danh mục thành công" });

                return Json(new { result = 0, error = res.error?.message ?? "Lỗi khi xóa danh mục" });
            }
            catch (Exception ex)
            {
                return Json(new { result = 0, error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetListByStatus(int? status = null)
        {
            if (status == null)
            {
                var res0 = await _s_NewsCategory.GetListByStatus(0);
                var res1 = await _s_NewsCategory.GetListByStatus(1);

                var all = new List<M_NewsCategory>();
                if (res0.result == 1 && res0.data != null) all.AddRange(res0.data);
                if (res1.result == 1 && res1.data != null) all.AddRange(res1.data);

                return Json(new { result = 1, data = all, count = all.Count, status = "all" });
            }
            else
            {
                var res = await _s_NewsCategory.GetListByStatus(status.Value);
                return Json(new
                {
                    result = 1,
                    data = res.data ?? new List<M_NewsCategory>(),
                    count = res.data?.Count ?? 0,
                    status
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> P_EditStatus(int id, int status)
        {
            var res = await _s_NewsCategory.UpdateStatus("", id, status, "");
            return Json(res);
        }
    }
}
