using Microsoft.AspNetCore.Mvc;
using Web_ProjectName.Services;
using Web_ProjectName.Models;
using Web_ProjectName.Lib;
using System.Threading.Tasks;

namespace Web_ProjectName.Controllers
{
    public class IntroduceController : BaseController<IntroduceController>
    {
        private readonly IS_Introduce _S_Introduce;

        public IntroduceController(IS_Introduce S_Introduce)
        {
            _S_Introduce = S_Introduce;
        }

        public async Task<IActionResult> Index()
        {
            var introduce = await _S_Introduce.GetById("", 1);
            if (introduce.result == 1 && introduce.data != null)
            {
                ViewBag.Introduce = introduce.data;
            }
            return View();
        }
    }
}