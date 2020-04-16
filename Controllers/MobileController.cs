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
            var countries = _logic.Countries;

            if (_logic._codeMappings.ContainsKey(id) || id == "World")
            {
                ViewBag.countryName = id;
                ViewBag.countries = countries;
                ViewBag.update = _logic.GetLastUpdate();
                return View();
            } else
            {
                if (_logic._codeMappings.ContainsValue(id))
                {
                    return Redirect("/mobile/" + _logic._codeMappings.First(V => V.Value.ToLower() == id.ToLower()).Key);
                }
            }
            return Redirect("/mobile");
        }

    }
}