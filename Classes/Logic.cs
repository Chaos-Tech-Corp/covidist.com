﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Logic
{

    const double oneUnixDay = 24 * 60 * 60 * 1000;

    private Dictionary<string, string> countryMappings = new Dictionary<string, string>()
        {
            {"US", "United_States_of_America"},
            {"Antigua and Barbuda","Antigua_and_Barbuda" },
            {"Bosnia and Herzegovina","Bosnia_and_Herzegovina" },
            {"Brunei","Brunei_Darussalam" },
            {"Burkina Faso","Burkina_Faso" },
            {"Cabo Verde","Cape_Verde" },
            {"Central African Republic","Central_African_Republic" },
            {"Cayman Islands","Cayman_Islands" },
            {"Costa Rica","Costa_Rica" },
            {"Cote d'Ivoire","Cote_dIvoire" },
            {"Czechia","Czech_Republic" },
            {"Congo (Kinshasa)","Democratic_Republic_of_the_Congo" },
            {"Congo (Brazzaville)","Democratic_Republic_of_the_Congo" },
            {"Dominican Republic","Dominican_Republic" },
            {"El Salvador","El_Salvador" },
            {"Equatorial Guinea","Equatorial_Guinea" },
            {"Faroe Islands","Faroe_Islands" },
            {"French Polynesia","French_Polynesia" },
            {"Guinea-Bissau","Guinea_Bissau" },
            {"Holy See","Holy_See" },
            {"Isle of Man","Isle_of_Man" },
            {"New Zealand","New_Zealand" },
            {"North Macedonia","North_Macedonia" },
            {"Papua New Guinea","Papua_New_Guinea" },
            {"Puerto Rico","Puerto_Rico" },
            {"Saint Kitts and Nevis","Saint_Kitts_and_Nevis" },
            {"Saint Lucia","Saint_Lucia" },
            {"Saint Vincent and the Grenadines","Saint_Vincent_and_the_Grenadines" },
            {"San Marino","San_Marino" },
            {"Saudi Arabia","Saudi_Arabia" },
            {"South Africa","South_Africa" },
            {"South Korea","South_Korea" },
            {"Korea, South","South_Korea" },
            {"Sri Lanka","Sri_Lanka" },
            {"Taiwan*","Taiwan" },
            {"Timor-Leste","Timor_Leste" },
            {"United Arab Emirates","United_Arab_Emirates" },
            {"United Kingdom","United_Kingdom" },
            {"Tanzania","United_Republic_of_Tanzania" }
        };
    private Dictionary<string, string> _countryCodes = new Dictionary<string, string>();
    private Dictionary<string, List<time_chart>> _charts = new Dictionary<string, List<time_chart>>();
    private Dictionary<string, List<time_chart>> _mobility = new Dictionary<string, List<time_chart>>();
    private DateTime _lastCheck = DateTime.MinValue;
    private Dictionary<string, Dictionary<DateTime, int>> _recoveries;
    private List<time_event> _events;
    private List<string> _countries = new List<string>();
    private DateTime _lastUpdate;

    public Dictionary<string, List<time_chart>> charts
    {
        get { return _charts; }
    }

    public Dictionary<string, Dictionary<DateTime, int>> recoveries
    {
        get { return _recoveries; }
    }

    public List<time_event> Events
    {
        get { return _events; }
    }

    public Logic()
    {
        Initialize();
    }

    public void Initialize()
    {
        lock (_charts)
        {
            //refresh every 2 hour aprox
            if (_charts.Count == 0 || _lastCheck < DateTime.Now.AddHours(-2))
            {
#if !DEBUG
                DownloadFile();
#endif
                _countries = new List<string>();
                _charts = new Dictionary<string, List<time_chart>>();
                _charts.Add("infected", readAllData(null));
                _charts.Add("lost", readAllData("lost"));
                _recoveries = readRecoveries();
                _events = GetEvents();
                _mobility = readMovilityData();
                _lastCheck = DateTime.Now;
            }
        }
    }


    public void DownloadFile()
    {
        //new cases/losts
        string url = "https://opendata.ecdc.europa.eu/covid19/casedistribution/csv";
        var net = new System.Net.WebClient();
        var data = net.DownloadData(url);
        System.IO.File.WriteAllBytes("c:\\temp\\" + DateTime.Today.ToString("yyyy-MM-dd") + ".csv", data);
        //recoveries
        url = "https://data.humdata.org/hxlproxy/data/download/time_series_covid19_recovered_global_narrow.csv?dest=data_edit&filter01=explode&explode-header-att01=date&explode-value-att01=value&filter02=rename&rename-oldtag02=%23affected%2Bdate&rename-newtag02=%23date&rename-header02=Date&filter03=rename&rename-oldtag03=%23affected%2Bvalue&rename-newtag03=%23affected%2Binfected%2Bvalue%2Bnum&rename-header03=Value&filter04=clean&clean-date-tags04=%23date&filter05=sort&sort-tags05=%23date&sort-reverse05=on&filter06=sort&sort-tags06=%23country%2Bname%2C%23adm1%2Bname&tagger-match-all=on&tagger-default-tag=%23affected%2Blabel&tagger-01-header=province%2Fstate&tagger-01-tag=%23adm1%2Bname&tagger-02-header=country%2Fregion&tagger-02-tag=%23country%2Bname&tagger-03-header=lat&tagger-03-tag=%23geo%2Blat&tagger-04-header=long&tagger-04-tag=%23geo%2Blon&header-row=1&url=https%3A%2F%2Fraw.githubusercontent.com%2FCSSEGISandData%2FCOVID-19%2Fmaster%2Fcsse_covid_19_data%2Fcsse_covid_19_time_series%2Ftime_series_covid19_recovered_global.csv";
        data = net.DownloadData(url);
        System.IO.File.WriteAllBytes("c:\\temp\\recover-" + DateTime.Today.ToString("yyyy-MM-dd") + ".csv", data);
        //mobility report
        //https://pastelsky.github.io/covid-19-mobility-tracker/output/<ISO-COUNTRY-CODE>/mobility-<social-place>.csv
        //do it only once a day
        if (!System.IO.File.Exists("c:\\temp\\mobility-" + DateTime.Today.ToString("yyyy-MM-dd") + ".csv"))
        {
            Dictionary<string, string> countries = new Dictionary<string, string>();
            List<string> places = new List<string>() { "residential", "parks", "retail-and-recreation", "transit-stations", "workplaces", "grocery-and-pharmacy" };
            StringBuilder fileContent = new StringBuilder();
            foreach (var line in GetFile().Skip(1))
            {
                if (!countries.ContainsKey(line[7]))
                {
                    countries.Add(line[7], line[7]);
                    try
                    {
                        foreach (var place in places)
                        {
                            //download the country data
                            url = "https://pastelsky.github.io/covid-19-mobility-tracker/output/" + line[7] + "/mobility-" + place + ".csv";

                            data = net.DownloadData(url);
                            var texto = Encoding.Default.GetString(data).Replace("date,value", "").Replace("\"", "").Trim().Split('\r');
                            foreach (var l in texto)
                            {
                                fileContent.Append(Environment.NewLine + line[7] + "," + place + "," + l.Trim());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //error downloading a country, skip it
                    }
                }
            }
            System.IO.File.AppendAllText("c:\\temp\\mobility-" + DateTime.Today.ToString("yyyy-MM-dd") + ".csv", fileContent.ToString());
        }
        _lastUpdate = GetLastUpdate();
    }

    public List<string[]> GetFile()
    {
        DateTime when = DateTime.Today;
        string fileName = "c:\\temp\\" + when.ToString("yyyy-MM-dd") + ".csv";
        while (!System.IO.File.Exists(fileName))
        {
            when = when.AddDays(-1);
            fileName = "c:\\temp\\" + when.ToString("yyyy-MM-dd") + ".csv";
        }
        return Extensions.SplitCSV(fileName);
    }

    public List<string[]> GetMobilityFile()
    {
        DateTime when = DateTime.Today;
        string fileName = "c:\\temp\\mobility-" + when.ToString("yyyy-MM-dd") + ".csv";
        while (!System.IO.File.Exists(fileName))
        {
            when = when.AddDays(-1);
            fileName = "c:\\temp\\mobility-" + when.ToString("yyyy-MM-dd") + ".csv";
        }
        return Extensions.SplitCSV(fileName);
    }

    public List<string[]> GetRecoveries()
    {
        var when = DateTime.Today;
        string fileName = "c:\\temp\\recover-" + when.ToString("yyyy-MM-dd") + ".csv";
        while (!System.IO.File.Exists(fileName))
        {
            when = when.AddDays(-1);
            fileName = "c:\\temp\\recover-" + when.ToString("yyyy-MM-dd") + ".csv";
        }
        return Extensions.SplitCSV(fileName);
    }

    public List<time_event> GetEvents()
    {
        List<time_event> events = new List<time_event>();
        foreach (string line in System.IO.File.ReadAllLines("c:\\temp\\events.csv"))
        {
            if (string.IsNullOrEmpty(line.Trim())) continue;
            var values = line.Split(',');
            events.Add(new time_event()
            {
                country = values[0],
                when = DateTime.ParseExact(values[1], new string[] { "yyyy-MM-dd", "dd/MM/yyyy", "dd-MM-yyyy" }, CultureInfo.InvariantCulture),
                title = values[2]
            });
        }
        return events;
    }

    public List<plotLine> GetEventLines(string country)
    {
        List<plotLine> lines = new List<plotLine>();
        foreach (var e in _events.Where(E => E.country == country))
        {
            lines.Add(new plotLine()
            {
                label = new label() { text = e.title },
                value = e.when.ToUnixTime(),
                width = 1
            });
        }
        return lines;
    }

    private List<time_chart> readAllData(string field)
    {
        int lineIx = 0;
        var charts = new List<time_chart>();
        _countryCodes = new Dictionary<string, string>();
        var fileLines = GetFile();
        foreach (string[] values in fileLines)
        {
            lineIx++;
            if (lineIx == 1) continue;
            if (string.IsNullOrEmpty(values[8]) || values[8] == "N/A")
            {
                continue;
            }
            if (!_countryCodes.ContainsKey(values[7]))
            {
                _countryCodes.Add(values[7],values[6]);
                charts.Add(new time_chart() { name = values[6], population = double.Parse("0" + values[9]), yAxis = 0, type = "spline", data = new List<List<object>>() });
            }
        }

        DateTime triggerDate = new DateTime(2020, 1, 20);
        int fieldIndex = 4;
        if (field == "lost")
        {
            fieldIndex = 5;
        }

        lineIx = 0;
        foreach (string[] values in fileLines)
        {
            lineIx++;
            if (lineIx == 1) continue;

            DateTime when = DateTime.ParseExact(values[0], new string[] { "dd/MM/yyyy", "dd-MM-yyyy" }, CultureInfo.InvariantCulture);
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


        }
        //minimum amount of cases to render data
        int minimumAmount = 1;
        List<int> allZero = new List<int>();
        lineIx = 0;
        foreach (var chart in charts)
        {
            chart.data = chart.data.OrderBy(D => (double)D[0]).ToList();
            for (var i = 1; i < chart.data.Count; i++)
            {
                chart.data[i][1] = (int)chart.data[i][1] + (int)chart.data[i - 1][1];
            }
            if ((int)chart.data.Last()[1] <= minimumAmount)
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

    private Dictionary<string, List<time_chart>> readMovilityData()
    {
        var charts = new Dictionary<string, List<time_chart>>();
        var fileLines = GetMobilityFile();
        foreach (string[] values in fileLines)
        {
            if (_countryCodes.ContainsKey(values[0]))
            {
                var countryName = _countryCodes[values[0]];
                if (!charts.ContainsKey(countryName))
                {
                    charts.Add(countryName, new List<time_chart>() { new time_chart() { name = values[1], marker = new { enabled = false } , yAxis = 0, type = "spline", data = new List<List<object>>() } });
                }
                else
                {
                    if (!charts[countryName].Any(A => A.name == values[1]))
                    {
                        charts[countryName].Add(new time_chart() { name = values[1], marker = new { enabled = false }, yAxis = 0, type = "spline", data = new List<List<object>>() });
                    }
                }
            }
        }

        DateTime triggerDate = new DateTime(2020, 1, 20);
        foreach (string[] values in fileLines)
        {
            DateTime when = DateTime.ParseExact(values[2], new string[] { "yyyy-MM-dd", "dd/MM/yyyy", "dd-MM-yyyy" }, CultureInfo.InvariantCulture);
            int value = int.Parse(values[3]);
            string country = values[0];
            if (_countryCodes.ContainsKey(country))
            {
                country = _countryCodes[country];
            }
            else
            {
                continue;
            }

            if (when < triggerDate) continue;

            double unixTimestamp = (when.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            unixTimestamp = unixTimestamp * 1000;

            var who = charts[country].First(F => F.name == values[1]);
            who.data.Add(new List<object>() { unixTimestamp, value });
        }

        return charts;
    }

    private Dictionary<string, Dictionary<DateTime, int>> readRecoveries()
    {
        var data = new Dictionary<string, Dictionary<DateTime, int>>();
        DateTime triggerDate = new DateTime(2020, 1, 20);

        if (_countries == null || _countries.Count ==0)
        {
            _countries = _charts["infected"].Select(S => S.name).ToList();
        }

        int lineIx = 0;
        foreach (string[] values in GetRecoveries())
        {
            lineIx++;
            if (lineIx <= 3) continue;

            
            DateTime when;
            string country;
            int recovered = 0;

            when = DateTime.ParseExact(values[4], new string[] { "yyyy-MM-dd", "dd/MM/yyyy", "dd-MM-yyyy" }, CultureInfo.InvariantCulture);
            recovered = int.Parse(values[5]);
            country = values[0];

            if (countryMappings.ContainsKey(country))
            {
                country = countryMappings[country];
            } else if (!_countries.Contains(country.Replace(" ","_"))) {
                country = values[1];
            }
            



            if (countryMappings.ContainsKey(country))
            {
                country = countryMappings[country];
            }

            if (when < triggerDate) continue;

            if (data.ContainsKey(country))
            {
                if (data[country].ContainsKey(when))
                {
                    data[country][when] += recovered;
                }
                else
                {
                    data[country].Add(when, recovered);
                }
            }
            else
            {
                data.Add(country, new Dictionary<DateTime, int>() { { when, recovered } });
            }
        }
        return data;
    }

    public List<time_chart> GetCountryData(string country, string type)
    {
        if (string.IsNullOrEmpty(type))
        {
            type = "c";
        }
        if (type != "c" && type != "d" && type != "a" && type != "e")
        {
            type = "c";
        }

        //infected
        var i = GetInfected(country);
        //recovered
        var r = GetRecovered(country);
        //lost
        var l = GetLost(country);

        //case multiplier data

        var m = new time_chart();
        var ml = new time_chart();
        var mr = new time_chart();
        m.data = new List<List<object>>();
        m.type = "column";
        m.yAxis = 1;
        if (type == "c")
        {
            //only for infected
            m = CaseMultiplier(i);
        }
        else
        {
            m = CasesByDay(i);
            ml = CasesByDay(l);
            mr = CasesByDay(r);
        }


        List<time_chart> c = new List<time_chart>();
        c.Add(m);

        //estimate of the multiplier
        int lengthEstimation = 4;
        if (type == "c")
        {
            var me = Estimate_Value(m, 2, lengthEstimation);
            if (me.data != null && me.data.Count > 0) {
                me.name = "Multiplier Estimate";
                me.yAxis = 1;
                me.type = "column";

                //when this will be 1 or very close to 1
                var t1 = (double)me.data.First()[1];
                var t2 = (double)me.data.Last()[1];
                double threshold = 1.01;
                if (t2 < t1 && t2 > threshold)
                {
                    
                    var temp = Estimate_Value(m, 2, 11);
                    if ((double)temp.data.Last()[1] <= threshold)
                    {
                        for (int ix = 0; ix < temp.data.Count; ix++)
                        {
                            if ((double)temp.data[ix][1] <= threshold)
                            {
                                lengthEstimation = ix-1;
                                me = Estimate_Value(m, 2, lengthEstimation);
                                me.name = "Case Estimate";
                                me.yAxis = 1;
                                me.type = "column";
                                break;
                            }
                        }
                        if (lengthEstimation > 0)
                        {
                            //get the linear estimation
                            var mline = Estimate_Value(m, 2, lengthEstimation);
                            mline.name = "Estimate Infected";
                            mline.yAxis = 0;
                            mline.type = "spline";
                            mline.data = new List<List<object>>();
                            for (int ix = 0; ix < me.data.Count; ix++)
                            {
                                double value = 0;
                                if (ix < 4)
                                {
                                    value = Convert.ToDouble(i.data[i.data.Count - (4 - ix)][1]);
                                }
                                else
                                {
                                    value = (double)mline.data[ix - 4][1];
                                }
                                mline.data.Add(new List<object>() { me.data[ix][0], value * (double)me.data[ix][1] });
                            }
                            c.Add(mline);
                        }
                    }
                }
                c.Add(me);
            }
        }

        if (ml.data != null && ml.data.Count > 0)
        {
            c.Add(ml);
        }
        if (mr.data != null && mr.data.Count > 0)
        {
            c.Add(mr);
        }
        c.Add(i);
        if (l.data.Count > 0)
        {
            c.Add(l);
        }
        if (r.data != null && r.data.Count > 0)
        {
            c.Add(r);
        }

        var est = Estimate_Series(i, 2, 4); //2 groups of 2 items = last 4 days for sample, 2 days estimation
        //lengthEstimation = until end of spike
        if (est.First().data != null && est.First().data.Count > 0)
        {
            if (type == "d")
            {
                c.Add(est.First());
            }
            c.Add(est.Last());
        }
        //c.Add(et);
        return c;
    }

    public time_chart GetInfected(string country)
    {
        var i = new time_chart();
        i.name = "Infected";
        i.type = "spline";
        i.yAxis = 0;
        i.data = new List<List<object>>();
        foreach (var e in charts["infected"].First(I => I.name == country).data)
        {
            //filter to start only when there are cases
            if ((int)e[1] > 0)
            {
                i.data.Add(new List<object>() { e[0], e[1] });
            }
        }
        i.data = i.data.OrderBy(O => (double)O[0]).ToList();
        return i;
    }

    public time_chart GetLost(string country)
    {
        var l = new time_chart();
        l.name = "Lost";
        l.type = "spline";
        l.yAxis = 0;
        l.data = new List<List<object>>();
        if (charts["lost"].Any(I => I.name == country))
        {
            foreach (var e in charts["lost"].First(I => I.name == country).data)
            {
                //filter to start only when there are cases
                if ((int)e[1] > 0)
                {
                    l.data.Add(new List<object>() { e[0], e[1] });
                }
            }
        }
        l.data = l.data.OrderBy(O => (double)O[0]).ToList();
        return l;
    }

    public time_chart GetRecovered(string country)
    {
        var r = new time_chart();
        if (recoveries.ContainsKey(country))
        {
            r.name = "Recovered";
            r.type = "spline";
            r.yAxis = 0;
            r.data = new List<List<object>>();
            foreach (var e in recoveries[country].OrderBy(O => O.Key))
            {
                if (e.Value > 0)
                {
                    r.data.Add(new List<object>() { e.Key.ToUnixTime(), e.Value });
                }
            }
            r.data = r.data.OrderBy(O => (double)O[0]).ToList();
        }
        return r;
    }

    public time_chart CasesByDay(time_chart l)
    {
        var m = new time_chart();
        m.data = new List<List<object>>();
        if (l.data != null && l.data.Count > 0)
        {
            m.type = "column";
            m.yAxis = 1;
            m.name = l.name + " by Day";
            m.data.Add(new List<object>() { l.data[0][0], l.data[0][1] });
            for (var ix = 1; ix < l.data.Count; ix++)
            {

                m.data.Add(new List<object>() { l.data[ix][0], (int)l.data[ix][1] - (int)l.data[ix - 1][1] });
            }
        }
        return m;
    }

    public time_chart CaseMultiplier(time_chart l)
    {
        var m = new time_chart();

        m.data = new List<List<object>>();
        m.type = "column";
        m.yAxis = 1;

        m.name = "Case Multiplier";
        for (var ix = 0; ix < l.data.Count; ix++)
        {
            if (ix > 3)
            {
                var t = Convert.ToDouble(l.data[ix][1]);
                var t4 = Convert.ToDouble(l.data[ix - 4][1]);
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
                m.data.Add(new List<object>() { l.data[ix][0], nV });
            }
            else
            {
                m.data.Add(new List<object>() { l.data[ix][0], 0 });
            }
        }
        return m;
    }

    public time_chart GetCountryDataActiveOnly(string country)
    {
        var i = new time_chart();
        i.name = "Active Cases";
        i.type = "spline";
        i.yAxis = 0;
        i.data = new List<List<object>>();
        i.marker = new { enabled = false };
        if (recoveries.ContainsKey(country))
        {
            var recoveries = _recoveries[country];
            Dictionary<double, int> recUnix = new Dictionary<double, int>();
            Dictionary<double, int> lostUnix = new Dictionary<double, int>();
            foreach (var e in recoveries)
            {
                recUnix.Add(e.Key.ToUnixTime(), e.Value);
            }
            foreach(var l in charts["lost"].First(I => I.name == country).data)
            {
                lostUnix.Add((double)l[0], (int)l[1]);
            }
            foreach (var e in charts["infected"].First(I => I.name == country).data)
            {
                //filter to start only when there are cases
                if ((int)e[1] > 0)
                {
                    int recValue = 0;
                    int lostValue = 0;
                    if (recUnix.ContainsKey((double)e[0]))
                    {
                        recValue = recUnix[(double)e[0]];

                        if (lostUnix.ContainsKey((double)e[0]))
                        {
                            lostValue = lostUnix[(double)e[0]];
                        }
                        i.data.Add(new List<object>() { e[0], (int)e[1] - recValue - lostValue });
                    }
                }
            }
        }
        else
        {
            i.name = "No recovey data available";
        }

        return i;
    }

    /// <summary>
    /// Generates an estimation based on the current cases by day
    /// </summary>
    /// <param name="m">series with the infected cases</param>
    /// <param name="groupLengh">length of the estimation group</param>
    /// <param name="length">how many days to estimate</param>
    /// <returns></returns>
    public List<time_chart> Estimate_Series(time_chart m, int groupLengh, int length)
    {
        List<time_chart> charts = new List<time_chart>();

        var etm = new time_chart();
        etm.name = "Estimate Cases by Day";
        etm.type = "column";
        etm.yAxis = 1;
        etm.data = new List<List<object>>();



        //it needs at least n+1 values
        if (m.data.Count > groupLengh + 1)
        {

            double maxValue = 0;
            for (var ix = m.data.Count - (groupLengh * 2); ix < m.data.Count; ix++)
            {
                maxValue += Math.Abs(Convert.ToDouble(m.data[ix][1]) - Convert.ToDouble(m.data[ix - 1][1]));
            }
            maxValue = maxValue / (groupLengh * 2);

            //get the last 4 entries
            double t2 = 0;
            double y2 = 0;
            for (var ix = m.data.Count - groupLengh; ix < m.data.Count; ix++)
            {
                t2 += (double)m.data[ix][0];
                //cases by day, not the sum
                double value = Math.Abs(Convert.ToDouble(m.data[ix][1]) - Convert.ToDouble(m.data[ix - 1][1]));
                y2 += value > maxValue ? maxValue : value;
            }
            t2 = t2 / groupLengh;
            y2 = y2 / groupLengh;

            //get the first 4 from the last 8
            double t1 = 0;
            double y1 = 0;
            for (var ix = m.data.Count - (groupLengh * 2); ix < m.data.Count - groupLengh; ix++)
            {
                t1 += (double)m.data[ix][0];
                //y1 += Math.Abs(Convert.ToDouble(m.data[ix][1]) - Convert.ToDouble(m.data[ix - 1][1]));
                double value = Math.Abs(Convert.ToDouble(m.data[ix][1]) - Convert.ToDouble(m.data[ix - 1][1]));
                y1 += value > maxValue ? maxValue : value;
            }
            t1 = t1 / groupLengh;
            y1 = y1 / groupLengh;

            //last day in the series
            double lastDay = (double)m.data.Last()[0];

            //etm.data.Add(m.data.Last());
            for (var ix = 1; ix <= length; ix++)
            {
                //double unixTimestamp = (DateTime.Today.AddDays(ix).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                //unixTimestamp = unixTimestamp * 1000;
                lastDay += oneUnixDay;

                var x = lastDay;
                var r = ((y2 - y1) / (t2 - t1)) * (x - t1) + y1;
                etm.data.Add(new List<object>() { x, r });
            }
        }

        var etl = new time_chart();
        etl.name = "Estimate Infected";
        etl.type = "spline";
        etl.yAxis = 0;
        etl.data = new List<List<object>>();
        if (m.data.Count > groupLengh + 1)
        {
            int firstValue = (int)m.data.Last()[1];
            foreach (var e in etm.data)
            {
                firstValue += Convert.ToInt32(e[1]);
                etl.data.Add(new List<object>() { e[0], firstValue });
            }
        }
        charts.Add(etm);
        charts.Add(etl);
        return charts;
    }

    public time_chart Estimate_Value(time_chart m, int groupLengh, int length)
    {

        var etm = new time_chart();
        etm.data = new List<List<object>>();
        //it needs at least n+1 values
        if (m.data.Count > groupLengh + 1)
        {

            //double maxValue = 0;
            //if (useMaxValue)
            //{
            //    for (var ix = m.data.Count - (groupLengh * 2); ix < m.data.Count; ix++)
            //    {
            //        maxValue += Convert.ToDouble(m.data[ix][1]);
            //    }
            //    maxValue = maxValue / (groupLengh * 2);
            //} else
            //{
            //    maxValue = double.MaxValue;
            //}

            //get the last 4 entries
            double t2 = 0;
            double y2 = 0;
            for (var ix = m.data.Count - groupLengh; ix < m.data.Count; ix++)
            {
                t2 += (double)m.data[ix][0];
                //cases by day, not the sum
                double value = Convert.ToDouble(m.data[ix][1]);
                y2 += value; //> maxValue ? maxValue : value;
            }
            t2 = t2 / groupLengh;
            y2 = y2 / groupLengh;

            //get the first 4 from the last 8
            double t1 = 0;
            double y1 = 0;
            for (var ix = m.data.Count - (groupLengh * 2); ix < m.data.Count - groupLengh; ix++)
            {
                t1 += (double)m.data[ix][0];
                //y1 += Math.Abs(Convert.ToDouble(m.data[ix][1]) - Convert.ToDouble(m.data[ix - 1][1]));
                double value =Convert.ToDouble(m.data[ix][1]);
                y1 += value;// > maxValue ? maxValue : value;
            }
            t1 = t1 / groupLengh;
            y1 = y1 / groupLengh;

            //last day in the series
            double lastDay = (double)m.data.Last()[0];

            //etm.data.Add(m.data.Last());
            for (var ix = 1; ix <= length; ix++)
            {
                //double unixTimestamp = (DateTime.Today.AddDays(ix).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                //unixTimestamp = unixTimestamp * 1000;
                lastDay += oneUnixDay;

                var x = lastDay;
                var r = ((y2 - y1) / (t2 - t1)) * (x - t1) + y1;
                etm.data.Add(new List<object>() { x, r });
            }
        }
        return etm;
    }

    /// <summary>
    /// Estimates the number of infected people based on the number of deaths and the casualty %
    /// </summary>
    /// <param name="m">series with the lost</param>
    /// <param name="period">how long take an infected to die</param>
    /// <param name="rate">% death rate</param>
    /// <param name="length">how many days to estimate</param>
    /// <returns></returns>
    public List<time_chart> Estimate_Infection(time_chart m, int period, double rate)
    {
        var l = new time_chart();
        l.name = "Estimated Infected";
        l.type = "spline";
        l.yAxis = 0;
        l.data = new List<List<object>>();
        l.marker = new { enabled = false };

        var lr = new time_chart();
        lr.name = "Estimated Range";
        lr.type = "arearange";
        lr.yAxis = 0;
        lr.data = new List<List<object>>();
        lr.zIndex = 0;
        lr.fillOpacity = 0.3;
        lr.color = "#7cb5ec";
        lr.marker = new { enabled = false };

        //need to be done using the daily cases, not the total value
        for(var ix = 1; ix < m.data.Count; ix++)
        {
            var e = m.data[ix];
            double date = Convert.ToDouble(e[0]);
            double value = Convert.ToDouble(e[1]) - Convert.ToDouble(m.data[ix-1][1]);
            if (value <= 0) continue;
            date = date - (oneUnixDay * period);

            l.data.Add(new List<object>() { date, Math.Round(value * 100 / rate) });
            lr.data.Add(new List<object>() { date, Math.Round(value * 100 / (rate+0.5)), Math.Round(value * 100 / (rate-0.5)) });
        }

        return new List<time_chart>() {lr, l };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="m">active cases</param>
    /// <param name="s">transmisibilidad</param>
    public time_chart Estimte_Propagation(time_chart m, double s, string type = "value")
    {

        //add 2 days estimation
        time_chart tmp = new time_chart();
        tmp.data = m.data.ToList();
        if (type == "series")
        {
            tmp.data.AddRange(Estimate_Series(m, 2, 4)[1].data);
        } else
        {
            tmp.data.AddRange(Estimate_Value(m, 2, 4).data);
        }

        double aMax = 0; //A(max) -- max numbers of persons infected in a day
        double tMax = 0; //tm -- day of max numbers of persons infected
        int ix = 1;
        double day1 = Convert.ToDouble(tmp.data[0][0]);

        foreach (var e in tmp.data.OrderBy(O => O[0]))
        {
            double date = Convert.ToDouble(e[0]);
            double value = Convert.ToDouble(e[1]);
            if (value > aMax)
            {
                aMax = value;
                tMax = ix;
            }
            ix++;
        }

        //calculate the initial function
        time_chart c = PropagationFunction(tmp, aMax, tMax, s, day1);

        //try to adjust the chart
        //when is the 30%
        var teP = tmp.data.Where(D => Convert.ToDouble(D[1]) <= (aMax * 3 / 10) && Convert.ToDouble(D[0]) <= day1  + tMax * oneUnixDay).OrderBy(O => (double)O[0]).Last();
        var tmP = c.data.Where(D => Convert.ToDouble(D[1]) < Convert.ToDouble(teP[1]) && Convert.ToDouble(D[0]) <= day1 + tMax * oneUnixDay).OrderBy(O => (double)O[0]).Last();
        //adjust the day difference
        if (Math.Abs((double)teP[0] - (double)tmP[0])> 0)
        {
            var dayDiff = ((double)teP[0] - (double)tmP[0]) / oneUnixDay;

            //when is the 60%
            teP = tmp.data.Where(D => Convert.ToDouble(D[1]) <= (aMax * 6 / 10) && Convert.ToDouble(D[0]) <= day1 + tMax * oneUnixDay).OrderBy(O => (double)O[0]).Last();
            tmP = c.data.Where(D => Convert.ToDouble(D[1]) < Convert.ToDouble(teP[1]) && Convert.ToDouble(D[0]) <= day1 + tMax * oneUnixDay).OrderBy(O => (double)O[0]).Last();
            dayDiff += ((double)teP[0] - (double)tmP[0]) / oneUnixDay;

            //when is the 90%
            teP = tmp.data.Where(D => Convert.ToDouble(D[1]) <= (aMax * 9 / 10) && Convert.ToDouble(D[0]) <= day1 + tMax * oneUnixDay).OrderBy(O => (double)O[0]).Last();
            tmP = c.data.Where(D => Convert.ToDouble(D[1]) < Convert.ToDouble(teP[1]) && Convert.ToDouble(D[0]) <= day1 + tMax * oneUnixDay).OrderBy(O => (double)O[0]).Last();
            dayDiff += ((double)teP[0] - (double)tmP[0]) / oneUnixDay;

            //average?
            dayDiff = dayDiff / 3;

            c = PropagationFunction(tmp, aMax, tMax + dayDiff, s, day1);
        }

        return c;
    }

    time_chart PropagationFunction(time_chart m, double aMax, double tMax, double s, double day1)
    {
        time_chart c = new time_chart();
        c.type = "spline";
        c.yAxis = 0;
        c.name = "Estimation";
        c.marker = new { enabled = false };
        c.data = new List<List<object>>();
        var ix = 1;
        while (true)
        {
            var value =  fValue(aMax, tMax, s, ix);
            c.data.Add(new List<object>() { day1 + oneUnixDay * (ix - 1), Math.Round( value) });
            ix++;
            if (ix > tMax && value <= 10)
            {
                break;
            }
        }
        return c;
    }

    public time_chart Convert2Population(time_chart m)
    {
        var t = new time_chart();
        t.marker = m.marker;
        t.data = new List<List<object>>();
        t.color = m.color;
        t.name = m.name;
        t.population = m.population;
        t.type = m.type;
        t.yAxis = m.yAxis;
        t.zIndex = m.zIndex;

        double population = m.population;
        //no population value, return empty
        if (population <= 0) return t;

        var multiplier = 100 / population;
        foreach (var e in m.data)
        {
            double val = Math.Round(Convert.ToDouble(e[1]) * multiplier, 2);
            t.data.Add(new List<object>() { e[0], val });
        }


        return t;
    }

    public time_chart CasesByMillion(List<time_chart> m) {

        var t = new time_chart();
        t.type = "bar";
        t.name = "Cases by Million";
        t.data = new List<List<object>>();

        foreach(var l in m.Where(M => M.population > 0))
        {
            var finalValue = Convert.ToDouble(l.data.Last()[1]);
            var perMillion = Math.Round( finalValue * 1000000 / l.population);
            if (perMillion > 0)
            {
                t.data.Add(new List<object>() { l.name, perMillion });
            }
        }

        return t;
    
    }

    public List<time_chart> GetMobility(string country)
    {
        if (_mobility.ContainsKey(country))
        {
            return _mobility[country];
        }
        else
        {
            return new List<time_chart>() { new time_chart() { name = "No data available", type = "spline", yAxis = 0 } };
        }
    }

    public time_chart GetMobility(string country, string type)
    {
        if (_mobility.ContainsKey(country)) {
            return _mobility[country].First(T => T.name == type);
        } else
        {
            return new time_chart() { name = "No data available", type="spline", yAxis=0 };
        }
    }

    double fValue(double aMax, double tMax, double s, int day)
    {
        return aMax * Math.Exp((-1 / (2 * s)) * (Math.Pow(day - tMax, 2) / day));
    }

    public DateTime LastUpdate()
    {
        if (_lastUpdate == DateTime.MinValue)
        {
            _lastUpdate = GetLastUpdate();
        }
        return _lastUpdate;
    }

    public DateTime GetLastUpdate()
    {
        var when = DateTime.Today;
        string fileName = "c:\\temp\\" + when.ToString("yyyy-MM-dd") + ".csv";
        while (!System.IO.File.Exists(fileName))
        {
            when = when.AddDays(-1);
            fileName = "c:\\temp\\" + when.ToString("yyyy-MM-dd") + ".csv";
        }
        return System.IO.File.GetLastWriteTime(fileName);
    }
}