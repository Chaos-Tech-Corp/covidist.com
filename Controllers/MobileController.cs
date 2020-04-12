using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace covidist.com.Controllers
{
    public class MobileController : BaseController
    {

        public MobileController()
        {
            //refresh data?
            _logic.Initialize();
        }

        public IActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = "World";
            }
            ViewBag.countryName = id;
            ViewBag.countries = _logic.charts["infected"].Select(C => C.name).ToList();
            ViewBag.update = _logic.GetLastUpdate();
            return View();
        }

    }
}