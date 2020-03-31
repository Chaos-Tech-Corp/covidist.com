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

        private Database _db;
        private static Dictionary<string, List<time_chart>> _charts = new Dictionary<string, List<time_chart>>();
        private static DateTime _lastCheck = DateTime.MinValue;


        public HomeController()
        {
        }

        public IActionResult Index()
        {
            lock (_charts)
            {
                //refresh every hour
                if (_charts.Count == 0 || _lastCheck < DateTime.Now.AddHours(-1))
                {
                    _charts = new Dictionary<string, List<time_chart>>();
                    _charts.Add("infected", readAllData(null));
                    _charts.Add("lost", readAllData("lost"));
                    _lastCheck = DateTime.Now;
                }
            }
            ViewBag.countries = _charts["infected"].Select(C => C.name).ToList();
            return View();
        }

        public void DownloadFile()
        {
            string url = "https://opendata.ecdc.europa.eu/covid19/casedistribution/csv";
            var net = new System.Net.WebClient();
            var data = net.DownloadData(url);
            System.IO.File.WriteAllBytes("c:\\temp\\" + DateTime.Today.ToString("yyyy-MM-dd") + ".csv", data);
        }

        public string[] GetFile()
        {
            string fileName = "c:\\temp\\" + DateTime.Today.ToString("yyyy-MM-dd") + ".csv";
            if (!System.IO.File.Exists(fileName) || _lastCheck < DateTime.Now.AddHours(-1))
            {
                DownloadFile();
            }
            return System.IO.File.ReadAllLines(fileName);
            
        }

        public JsonResult AllData(string field, string range, string adjust)
        {

            if (string.IsNullOrEmpty(field))
            {
                field = "infected";
            }
            if (field != "infected" && field != "lost")
            {
                field = "infected";
            }
            lock (_charts)
            {
                if (!_charts.ContainsKey(field))
                {
                    _charts.Add(field, readAllData(null));
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
                foreach (var item in _charts[field])
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

            return new JsonResult(_charts[field]);
        }

        public JsonResult CountryData(string country, string type)
        {
            lock (_charts)
            {
                if (!_charts.ContainsKey("infected"))
                {
                    _charts.Add("infected", readAllData(null));
                }
                if (!_charts.ContainsKey("lost"))
                {
                    _charts.Add("lost", readAllData("lost"));
                }
            }

            if (string.IsNullOrEmpty(type))
            {
                type = "c";
            }
            if (type != "c" && type !="d")
            {
                type = "c";
            }

            var i = new time_chart();
            i.name = "Infected";
            i.type = "spline";
            i.yAxis = 0;
            i.data = new List<List<object>>();
            foreach (var e in _charts["infected"].First(I => I.name == country).data)
            {
                //filter to start only when there are cases
                if ((int)e[1] > 0)
                {
                    i.data.Add(new List<object>() { e[0], e[1] });
                }
            }

            var l = new time_chart();
            l.name = "Lost";
            l.type = "spline";
            l.yAxis = 0;
            l.data = new List<List<object>>();
            if (_charts["lost"].Any(I => I.name == country))
            {
                foreach (var e in _charts["lost"].First(I => I.name == country).data)
                {
                    //filter to start only when there are cases
                    if ((int)e[1] > 0)
                    {
                        l.data.Add(new List<object>() { e[0], e[1] });
                    }
                }
            }

            //case multiplier data

            var m = new time_chart();
            var ml = new time_chart();
            m.data = new List<List<object>>();
            m.type = "column";
            m.yAxis = 1;
            if (type == "c")
            {
                m.name = "Case Multiplier";
                for (var ix = 0; ix < i.data.Count; ix++)
                {
                    if (ix > 3)
                    {
                        var t = Convert.ToDouble(i.data[ix][1]);
                        var t4 = Convert.ToDouble(i.data[ix - 4][1]);
                        double nV = 1;
                        if (t4 > 0)
                        {
                            nV = Math.Round(t / t4, 2);
                        }
                        else
                        {
                            nV = 0;
                        }
                        if (nV > 10)
                        {
                            nV = 10;
                        }
                        m.data.Add(new List<object>() { i.data[ix][0], nV });
                    }
                    else
                    {
                        m.data.Add(new List<object>() { i.data[ix][0], 0 });
                    }
                }
            } else
            {
                m.name = "Infected by Day";
                m.data.Add(new List<object>() { i.data[0][0], i.data[0][1]});
                for (var ix = 1; ix < i.data.Count; ix++)
                {

                    m.data.Add(new List<object>() { i.data[ix][0], (int)i.data[ix][1] - (int)i.data[ix-1][1] });
                }

                if (l.data.Count > 0)
                {
                    ml.data = new List<List<object>>();
                    ml.type = "column";
                    ml.yAxis = 1;
                    ml.name = "Lost by Day";
                    ml.data.Add(new List<object>() { l.data[0][0], l.data[0][1] });
                    for (var ix = 1; ix < l.data.Count; ix++)
                    {

                        ml.data.Add(new List<object>() { l.data[ix][0], (int)l.data[ix][1] - (int)l.data[ix - 1][1] });
                    }
                }
            }

            ////add estimation for next 4 days
            //var etm = new time_chart();
            //etm.name = "Mult. Estimate";
            //etm.type = "column";
            //etm.yAxis = 1;
            //etm.data = new List<List<object>>();
            ////get the last 4 entries
            //int groupLengh = 3;
            //double  t2 = 0;
            //double y2 = 0;
            //for (var ix = m.data.Count - groupLengh; ix < m.data.Count; ix++)
            //{
            //    t2 += (double)m.data[ix][0];
            //    y2 += (double)m.data[ix][1];
            //}
            //t2 = t2 / groupLengh;
            //y2 = y2 / groupLengh;

            ////get the first 4 from the last 8
            //double t1 = 0;
            //double y1 = 0;
            //for (var ix = m.data.Count - (groupLengh*2); ix < m.data.Count - groupLengh; ix++)
            //{
            //    t1 += (double)m.data[ix][0];
            //    y1 += (double)m.data[ix][1];
            //}
            //t1 = t1 / groupLengh;
            //y1 = y1 / groupLengh;

            //etm.data.Add(m.data.Last());
            //for (var ix = 1; ix < 5; ix++)
            //{
            //    double unixTimestamp = (DateTime.Today.AddDays(ix).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            //    unixTimestamp = unixTimestamp * 1000;

            //    var x = unixTimestamp;
            //    var r = ((y2 - y1) / (t2 - t1)) * (x - t1) + y1;
            //    etm.data.Add(new List<object>() { x, r });
            //}


            //double lowerLimit = 1.01;
            //int maxDays = 10;
            //if ((double)etm.data.Last()[1] < (double)etm.data.First()[1] && (double)etm.data.Last()[1] > lowerLimit)
            //{
            //    //how long till it gets close to 1? - max 30 days
            //    for(var ix = 5; ix < maxDays; ix++)
            //    {
            //        double unixTimestamp = (DateTime.Today.AddDays(ix).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            //        unixTimestamp = unixTimestamp * 1000;

            //        var x = unixTimestamp;
            //        var r = ((y2 - y1) / (t2 - t1)) * (x - t1) + y1;
            //        etm.data.Add(new List<object>() { x, r });
            //        if (r <= lowerLimit)
            //        {
            //            break;
            //        }
            //    }
            //}

            ////add estimation for next X days - basd on estimated multiplier data
            //var et = new time_chart();
            //et.name = "Infected Estimate";
            //et.type = "spline";
            //et.yAxis = 0;
            //et.data = new List<List<object>>();
            ////get the last 4 entries
            //t2 = 0;
            //y2 = 0;
            //for (var ix = i.data.Count - groupLengh; ix < i.data.Count; ix++)
            //{
            //    t2 += (double)i.data[ix][0];
            //    y2 += (int)i.data[ix][1];
            //}
            //t2 = t2 / groupLengh;
            //y2 = y2 / groupLengh;

            ////get the first 4 from the last 8
            //t1 = 0;
            //y1 = 0;
            //for (var ix = i.data.Count - (groupLengh * 2); ix < i.data.Count - groupLengh; ix++)
            //{
            //    t1 += (double)i.data[ix][0];
            //    y1 += (int)i.data[ix][1];
            //}
            //t1 = t1 / groupLengh;
            //y1 = y1 / groupLengh;

            //et.data.Add(i.data.Last());
            //for (var ix = 1; ix < etm.data.Count; ix++)
            //{
            //    double unixTimestamp = (DateTime.Today.AddDays(ix).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            //    unixTimestamp = unixTimestamp * 1000;

            //    var x = unixTimestamp;
            //    var r = ((y2 - y1) / (t2 - t1)) * (x - t1) + y1;
            //    et.data.Add(new List<object>() { x, r });
            //    //i.data.Add(new List<object>() { x, r });
            //}

            List<time_chart> c = new List<time_chart>();
            c.Add(m);
            if (ml.data != null && ml.data.Count >0)
            {
                c.Add(ml);
            }
            c.Add(i);
            if (l.data.Count > 0)
            {
                c.Add(l);
            }

            //c.Add(etm);
            //c.Add(et);

            return new JsonResult(c);
        }

        private List<time_chart> readAllData(string field)
        {
            int lineIx = 0;
            var charts = new List<time_chart>();
            Dictionary<string, string> countries = new Dictionary<string, string>();
            foreach (string line in GetFile())
            {
                lineIx++;
                if (lineIx == 1) continue;
                var values = line.Split(',');
                if (string.IsNullOrEmpty(values[8]) || values[8] == "N/A")
                {
                    continue;
                }
                if (!countries.ContainsKey(values[6]))
                {
                    countries.Add(values[6], values[6]);
                    charts.Add(new time_chart() { name = values[6], yAxis = 0, type = "spline", data = new List<List<object>>() });
                }
            }

            DateTime triggerDate = new DateTime(2020, 1, 20);
            int fieldIndex = 4;
            if (field == "lost")
            {
                fieldIndex = 5;
            }

            //foreach(Entry e in entries)
            lineIx = 0;
            foreach (string line in GetFile())
            {
                lineIx++;
                if (lineIx == 1) continue;

                var values = line.Split(',');
                DateTime when = DateTime.ParseExact(values[0], new string[] {"dd/MM/yyyy", "dd-MM-yyyy"}, CultureInfo.InvariantCulture);
                int infected = int.Parse(values[fieldIndex]);
                //int lost = int.Parse(values[5]);
                string country = values[6];

                if (string.IsNullOrEmpty(values[8]) || values[8] == "N/A")
                {
                    continue;
                }

                if (when < triggerDate) continue;

                double unixTimestamp = (when.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                unixTimestamp = unixTimestamp * 1000;

                var who = charts.First(F => F.name == country);
                who.data.Add(new List<object>() { unixTimestamp, infected });

                //var cIx = charts[0].data.FindIndex(D => (double)D.First() == unixTimestamp);
                //if (cIx >= 0)
                //{
                //    charts[0].data[cIx][1] = (int)charts[0].data[cIx][1] + infected;
                //    charts[1].data[cIx][1] = (int)charts[1].data[cIx][1] + lost;
                //}
                //else
                //{
                //    charts[0].data.Add(new List<object>() { unixTimestamp, infected });
                //    charts[1].data.Add(new List<object>() { unixTimestamp, lost });
                //}



                //charts[1].data.Add(new List<object>() { unixTimestamp, e.critical });

                //charts[2].data.Add(new List<object>() { unixTimestamp, e.recovered });


            }
            //charts[0].data = charts[0].data.OrderBy(D => (double)D[0]).ToList();
            //charts[1].data = charts[1].data.OrderBy(D => (double)D[0]).ToList();
            List<int> allZero = new List<int>();
            lineIx = 0;
            foreach (var chart in charts)
            {
                chart.data = chart.data.OrderBy(D => (double)D[0]).ToList();
                for (var i = 1; i < chart.data.Count; i++)
                {
                    chart.data[i][1] = (int)chart.data[i][1] + (int)chart.data[i - 1][1];
                }
                if ((int)chart.data.Last()[1] <= 100)
                {
                    allZero.Add(lineIx);
                }
                lineIx++;
            }
            foreach (int ix in allZero.OrderByDescending(O => O))
            {
                charts.RemoveAt(ix);
            }
            return charts;
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
