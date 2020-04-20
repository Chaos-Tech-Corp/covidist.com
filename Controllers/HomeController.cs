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
    public class HomeController : BaseController
    {

        public HomeController()
        {
            //refresh data?
            _logic.Initialize();
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult getCountries()
        {
            return new JsonResult(new { g = _logic.Continents, c = _logic.Countries });
        }

        public JsonResult BarData(string filter, string sort, string dir)
        {
            if (string.IsNullOrEmpty(dir) || (dir.ToLower() != "asc" && dir.ToLower() != "desc"))
            {
                dir = "asc";
            }
            else
            {
                dir = dir.ToLower();
            }
            if (string.IsNullOrEmpty(sort) || (sort != "country" && sort != "value"))
            {
                sort = "country";
            }
            var series = _logic.CasesByMillion(_logic.charts["lost"]);
            var sortIndex = 0;
            if (sort == "value")
            {
                sortIndex = 1;
            }
            if (dir == "asc")
            {
                series.data = series.data.OrderBy(O => O[sortIndex]).ToList();
            }
            else
            {
                series.data = series.data.OrderByDescending(O => O[sortIndex]).ToList();
            }

            var categories = series.data.Select(S => S[0]).ToList();

            return new JsonResult(new { series = new { name = "", data = series.data.Select(S => S[1]).ToList() }, categories = categories });
        }

        public JsonResult AllData(string field, string range, string adjust)
        {
            if (string.IsNullOrEmpty(field))
            {
                field = "infected";
            }
            if (field != "infected" && field != "lost" && field != "active" && field != "pinfected" && field != "plost" && field != "million")
            {
                field = "infected";
            }

            List<time_chart> data2Use;

            if (field =="million")
            {
                var chartData = _logic.CasesByMillion(_logic.charts["lost"]);
                chartData.data = chartData.data.OrderBy(C => C[0]).ToList();
                data2Use = new List<time_chart>() { chartData };
            }
            else if(field == "active")
            {
                data2Use = new List<time_chart>();
                foreach (var item in _logic.charts["infected"])
                {
                    if (_logic.recoveries.ContainsKey(item.code))
                    {

                        var t = new time_chart()
                        {
                            name = item.name,
                            type = item.type,
                            yAxis = item.yAxis,
                            data = new List<List<object>>()
                        };
                        var recoveries = _logic.recoveries[item.code];
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
                data2Use = new List<time_chart>();
                if (field == "pinfected" || field == "plost")
                {
                    foreach(var c in _logic.charts[field.Substring(1)])
                    {
                        data2Use.Add(_logic.Convert2Population(c));
                    }
                }else
                {
                    data2Use = _logic.charts[field];
                }
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
                        code = item.code,
                        yAxis = item.yAxis,
                        data = new List<List<object>>()
                    };
                    int index = 0;

                    //find the first day to use for the series, when there is X infected cases
                    var initPoint = _logic.PandemicSeriesStart(item.code, caseTrigger);
                    if (initPoint > 0)
                    {
                        for (var i = 0; i < item.data.Count; i++)
                        {
                            if (Convert.ToDouble(item.data[i][0]) >= initPoint)
                            {
                                t.data.Add(new List<object>() { index, item.data[i][1] });
                                index++;
                            }
                        }
                        adjusted.Add(t);
                    }
                }

                return new JsonResult(adjusted);

            }

            return new JsonResult(data2Use);
        }

        public JsonResult ContinentData(string continent, string field, string range, string adjust)
        {
            if (string.IsNullOrEmpty(field))
            {
                field = "infected";
            }
            if (field != "infected" && field != "lost" && field != "active" && field != "pinfected" && field != "plost" && field != "million")
            {
                field = "infected";
            }

            List<time_chart> data2Use;

            if (field == "million")
            {
                var chartData = _logic.CasesByMillion(_logic.charts["lost"].Where(C => _logic._continentMappings[C.code] == continent).ToList()); ;
                chartData.data = chartData.data.OrderBy(C => C[0]).ToList();
                data2Use = new List<time_chart>() { chartData };
            }
            else if (field == "active")
            {
                data2Use = new List<time_chart>();
                foreach (var item in _logic.charts["infected"].Where(C => _logic._continentMappings[C.code] == continent).ToList())
                {
                    if (_logic.recoveries.ContainsKey(item.code))
                    {

                        var t = new time_chart()
                        {
                            name = item.name,
                            type = item.type,
                            yAxis = item.yAxis,
                            data = new List<List<object>>()
                        };
                        var recoveries = _logic.recoveries[item.code];
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

            }
            else
            {
                data2Use = new List<time_chart>();
                if (field == "pinfected" || field == "plost")
                {
                    foreach (var c in _logic.charts[field.Substring(1)].Where(C => _logic._continentMappings[C.code] == continent).ToList())
                    {
                        data2Use.Add(_logic.Convert2Population(c));
                    }
                }
                else
                {
                    data2Use = _logic.charts[field].Where(C => _logic._continentMappings[C.code] == continent).ToList();
                }
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
                        code = item.code,
                        data = new List<List<object>>()
                    };
                    int index = 0;
                    var initPoint = _logic.PandemicSeriesStart(item.code, caseTrigger);
                    if (initPoint > 0)
                    {
                        for (var i = 0; i < item.data.Count; i++)
                        {
                            if (Convert.ToDouble(item.data[i][1]) >= initPoint)
                            {
                                t.data.Add(new List<object>() { index, item.data[i][1] });
                                index++;
                            }
                        }
                        adjusted.Add(t);
                    }
                }

                return new JsonResult(adjusted);

            }

            return new JsonResult(data2Use);
        }

        public JsonResult CountryData(string country, string type, string s)
        {
            var continents = _logic.Continents;
            if (continents.Contains(country))
            {
                return ContinentData(country, type, "100", s);
            }

            if (type == "a")
            {
                var lDay = _logic.CasesByDay(_logic.GetLost(country));
                lDay.yAxis = 1;
                var iDay = _logic.CasesByDay(_logic.GetInfected(country));
                iDay.yAxis = 1;
                var rDay = _logic.CasesByDay(_logic.GetRecovered(country));
                rDay.yAxis = 1;
                var active = _logic.GetCountryDataActiveOnly(country);
                return new JsonResult(new { series = new List<time_chart>() { 
                    iDay,
                    rDay,
                    lDay,
                    active
                }, lines = _logic.GetEventLines(country) });
            }
            else if (type == "e")
            {
                if (string.IsNullOrEmpty(s))
                {
                    s = "3.5";
                }
                var series = new List<time_chart>();
                var l = _logic.GetLost(country);
                l.marker = new { enabled = false };
                //series.AddRange(_logic.Estimate_Infection(l, int.Parse(p), double.Parse(r)));
                
                var i = _logic.GetCountryDataActiveOnly(country); //_logic.GetInfected(country);
                i.name = "Active Confirmed Infected";
                i.marker = new { enabled = false };
                series.Add(i);

                l = _logic.CasesByDay(l);
                l.yAxis = 0;

                series.Add(l);

                //series.Add(_logic.Estimate_Series(i, 2, 4)[1]);
                if (i.data.Count > 0) {
                    series.Add(_logic.Estimte_Propagation(i, double.Parse(s), "series"));
                    series.Add(_logic.Estimte_Propagation(i, double.Parse(s), "value"));
                }
                return new JsonResult(new { series = series, lines = _logic.GetEventLines(country) });
            }
            else if(type == "m")
            {
                return new JsonResult(new { series = _logic.GetMobility(country), lines = _logic.GetEventLines(country) });
            }
            else if (type == "yt")
            {
                var series = _logic.GetMobility(country).ToList();
                var active = _logic.GetInfected(country);
                active.yAxis = 1;
                active.marker = new { enabled = false };
                active.name = "Total Infected";
                series.Add(active);

                return new JsonResult(new { series = series, lines = _logic.GetEventLines(country) });
            }
            else if (type == "yd")
            {
                var series = _logic.GetMobility(country).ToList();
                var active = _logic.CasesByDay(_logic.GetInfected(country));
                active.yAxis = 1;
                active.marker = new { enabled = false };
                active.name = "Daily New Infected";
                series.Insert(0,active);

                return new JsonResult(new { series = series, lines = _logic.GetEventLines(country) });
            }
            else {
                return new JsonResult(new { series = _logic.GetCountryData(country, type), lines = _logic.GetEventLines(country) });
            }
        }

        public JsonResult mobile_CountryData(string country, string type, string s)
        {
            if (type == "c")
            {
                var i = _logic.GetInfected(country);
                i.marker = new { enabled = false };
                i.name = "Total Cases";
                var n = _logic.CasesByDay(i);
                n.name = "Daily New Cases";
                return new JsonResult(new { series = new List<time_chart>() { n, i }, lines = _logic.GetEventLines(country) });
            }
            else if (type == "a")
            {
                var i = _logic.GetCountryDataActiveOnly(country);
                i.marker = new { enabled = false };
                i.name = "Active Cases";
                return new JsonResult(new { series = new List<time_chart>() { i }, lines = _logic.GetEventLines(country) });
            }
            else if (type == "d")
            {
                var i = _logic.GetLost(country);
                i.marker = new { enabled = false };
                i.name = "Total Cases";
                var n = _logic.CasesByDay(i);
                n.name = "Daily New Cases";
                return new JsonResult(new { series = new List<time_chart>() { n, i }, lines = _logic.GetEventLines(country) });
            }
            else if (type == "m")
            {
                return new JsonResult(new { series = _logic.GetMobility(country), lines = _logic.GetEventLines(country) });
            }
            else if (type == "l")
            {
                //infected
                var i = _logic.GetInfected(country);
                //case multiplier data
                var m = _logic.CaseMultiplier(i);
                m.type = "column";
                m.yAxis = 0;
                m.name = "Multiplier by 4 days";
                

                List<time_chart> c = new List<time_chart>();
                c.Add(m);

                return new JsonResult(new { series = c, lines = _logic.GetEventLines(country), yLines = new List<plotLine>() {
                    new plotLine() { width = 1, value = 1, label = new label() { text ="No more new cases" } }
                } });
            }
            else if (type == "e")
            {
                if (string.IsNullOrEmpty(s))
                {
                    s = "3.5";
                }
                var series = new List<time_chart>();

                var i = _logic.GetCountryDataActiveOnly(country); //_logic.GetInfected(country);
                i.name = "Active Cases";
                i.marker = new { enabled = false };
                series.Add(i);

                //series.Add(_logic.Estimate_Series(i, 2, 4)[1]);
                if (i.data.Count > 0)
                {
                    series.Add(_logic.Estimte_Propagation(i, double.Parse(s), "series"));
                    series.Add(_logic.Estimte_Propagation(i, double.Parse(s), "value"));
                }
                return new JsonResult(new { series = series, lines = new List<object>() });
            }

            return null;
        }

        

        public JsonResult CountryDataActiveOnly(string country)
        {
            var active = _logic.GetCountryDataActiveOnly(country);
            var estimate = _logic.Estimte_Propagation(active, 6.5);
            return new JsonResult(new { series = new List<time_chart>() { active, estimate }, lines = _logic.GetEventLines(country) });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Privacy()
        {
            ViewBag.isInfo = true;
            ViewBag.update = _logic.GetLastUpdate().ToString("yyyy-MM-dd HH:mm zzz");
            return View();
        }
    }
}
