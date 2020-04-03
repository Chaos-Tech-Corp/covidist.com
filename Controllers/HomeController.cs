using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using covidist.com.Models;
using System.Globalization;
using Newtonsoft.Json;

namespace covidist.com.Controllers
{
    public class HomeController : Controller
    {

        private static Logic _logic = new Logic();

        public HomeController()
        {
            _logic.Initialize();
        }

        public IActionResult Index()
        {
            ViewBag.countries = _logic.charts["infected"].Select(C => C.name).ToList();
            return View();
        }

        public JsonResult AllData(string field, string range, string adjust)
        {
            if (string.IsNullOrEmpty(field))
            {
                field = "infected";
            }
            if (field != "infected" && field != "lost" && field != "active")
            {
                field = "infected";
            }

            List<time_chart> data2Use;

            if(field == "active")
            {
                data2Use = new List<time_chart>();
                foreach (var item in _logic.charts["infected"])
                {
                    if (_logic.recoveries.ContainsKey(item.name))
                    {

                        var t = new time_chart()
                        {
                            name = item.name,
                            type = item.type,
                            yAxis = item.yAxis,
                            data = new List<List<object>>()
                        };
                        var recoveries = _logic.recoveries[item.name];
                        Dictionary<double, int> recUnix = new Dictionary<double, int>();
                        foreach (var e in recoveries)
                        {
                            recUnix.Add(e.Key.ToUnixTime(), e.Value);
                        }
                        foreach (var e in item.data)
                        {
                            //filter to start only when there are cases
                            if ((int)e[1] > 0)
                            {
                                if (recUnix.ContainsKey((double)e[0]))
                                {
                                    t.data.Add(new List<object>() { e[0], (int)e[1] - recUnix[(double)e[0]] });
                                }
                            }
                        }

                        data2Use.Add(t);
                    }
                }

            } else
            {
                data2Use = _logic.charts[field];
            }
            //adjust the ranges by 100 cases or closest figure
            if (range == "pandemic")
            {
                if (string.IsNullOrEmpty(adjust))
                {
                    adjust = "100";
                }
                int caseTrigger = int.Parse(adjust);
                List<time_chart> adjusted = new List<time_chart>();
                foreach (var item in data2Use)
                {
                    var t = new time_chart()
                    {
                        name = item.name,
                        type = item.type,
                        yAxis = item.yAxis,
                        data = new List<List<object>>()
                    };
                    int index = 0;
                    for(var i = 0; i < item.data.Count; i++) { 
                        if ((int)item.data[i][1] >= caseTrigger)
                        {
                            t.data.Add(new List<object>() { index, item.data[i][1] });
                            index++;
                        }
                    }
                    adjusted.Add(t);
                }

                return new JsonResult(adjusted);

            }

            return new JsonResult(data2Use);
        }

        public JsonResult CountryData(string country, string type, string p, string r)
        {
            if (type == "a")
            {
                var lDay = _logic.CasesByDay(_logic.GetLost(country));
                lDay.yAxis = 0;
                return new JsonResult(new { series = new List<time_chart>() { 
                    _logic.GetCountryDataActiveOnly(country),
                    lDay
                }, lines = _logic.GetEventLines(country) });
            }
            else if (type == "e")
            {
                if (string.IsNullOrEmpty(p))
                {
                    p = "14";
                }
                if (string.IsNullOrEmpty(r))
                {
                    r = "1.5";
                }
                var series = new List<time_chart>();
                var l = _logic.GetLost(country);
                l.marker = new { enabled = false };
                series.AddRange(_logic.Estimate_Infection(l, int.Parse(p), double.Parse(r)));
                
                var i = _logic.GetCountryDataActiveOnly(country); //_logic.GetInfected(country);
                i.name = "Active Confirmed Infected";
                i.marker = new { enabled = false };
                series.Add(i);

                l = _logic.CasesByDay(l);
                l.yAxis = 0;

                series.Add(l);
                return new JsonResult(new { series = series, lines = _logic.GetEventLines(country) });
            } else {
                return new JsonResult(new { series = _logic.GetCountryData(country, type), lines = _logic.GetEventLines(country) });
            }
        }

        public JsonResult CountryDataActiveOnly(string country)
        {
            return new JsonResult(new { series = new List<time_chart>() { _logic.GetCountryDataActiveOnly(country) }, lines = _logic.GetEventLines(country) });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Privacy()
        {
            var when = DateTime.Today;
            string fileName = "c:\\temp\\" + when.ToString("yyyy-MM-dd") + ".csv";
            while (!System.IO.File.Exists(fileName))
            {
                when.AddDays(-1);
                fileName = "c:\\temp\\" + when.ToString("yyyy-MM-dd") + ".csv";
            }
            ViewBag.update = System.IO.File.GetLastWriteTime(fileName).ToString("yyyy-MM-dd HH:mm zzz");
            return View();
        }
    }
}
