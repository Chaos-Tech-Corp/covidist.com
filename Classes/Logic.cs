using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

    private Dictionary<string, List<time_chart>> _charts = new Dictionary<string, List<time_chart>>();
    private DateTime _lastCheck = DateTime.MinValue;
    private Dictionary<string, Dictionary<DateTime, int>> _recoveries;
    private List<time_event> _events;

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
            //refresh every hour
            if (_charts.Count == 0 || _lastCheck < DateTime.Now.AddHours(-2))
            {
                DownloadFile();
                _charts = new Dictionary<string, List<time_chart>>();
                _charts.Add("infected", readAllData(null));
                _charts.Add("lost", readAllData("lost"));
                _recoveries = readRecoveries();
                _events = GetEvents();
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
    }

    public string[] GetFile()
    {
        DateTime when = DateTime.Today;
        string fileName = "c:\\temp\\" + when.ToString("yyyy-MM-dd") + ".csv";
        while (!System.IO.File.Exists(fileName))
        {
            when.AddDays(-1);
            fileName = "c:\\temp\\" + when.ToString("yyyy-MM-dd") + ".csv";
        }
        return System.IO.File.ReadAllLines(fileName);
    }

    public string[] GetRecoveries()
    {
        var when = DateTime.Today;
        string fileName = "c:\\temp\\recover-" + when.ToString("yyyy-MM-dd") + ".csv";
        while (!System.IO.File.Exists(fileName))
        {
            when.AddDays(-1);
            fileName = "c:\\temp\\" + when.ToString("yyyy-MM-dd") + ".csv";
        }
        return System.IO.File.ReadAllLines(fileName);
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

        lineIx = 0;
        foreach (string line in GetFile())
        {
            lineIx++;
            if (lineIx == 1) continue;

            var values = line.Split(',');
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

    private Dictionary<string, Dictionary<DateTime, int>> readRecoveries()
    {
        var data = new Dictionary<string, Dictionary<DateTime, int>>();
        DateTime triggerDate = new DateTime(2020, 1, 20);

        int lineIx = 0;
        foreach (string line in GetRecoveries())
        {
            lineIx++;
            if (lineIx <= 3) continue;

            string[] values = line.Split(',');
            DateTime when;
            string country;
            int recovered = 0;
            if (line[1] == '"')
            {
                when = DateTime.ParseExact(values[5], new string[] { "yyyy-MM-dd", "dd/MM/yyyy", "dd-MM-yyyy" }, CultureInfo.InvariantCulture);
                recovered = int.Parse(values[6]);
                country = values[1].Replace("\"", "").Trim() + ", " + values[2].Replace("\"", "").Trim();
            }
            else
            {
                when = DateTime.ParseExact(values[4], new string[] { "yyyy-MM-dd", "dd/MM/yyyy", "dd-MM-yyyy" }, CultureInfo.InvariantCulture);
                recovered = int.Parse(values[5]);
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

        if (type == "a")
        {
            return GetCountryDataActiveOnly(country);
        } else if (type == "e")
        {
            return new List<time_chart>() { Estimate_Infection(GetLost(country), 14, 1.5) };
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

        var est = Estimate_Value(i, 2, 4); //2 groups of 2 items = last 4 days for sample, 2 days estimation
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

    public List<time_chart> GetCountryDataActiveOnly(string country)
    {
        var i = new time_chart();
        i.name = "Infected";
        i.type = "spline";
        i.yAxis = 0;
        i.data = new List<List<object>>();
        if (recoveries.ContainsKey(country))
        {
            var recoveries = _recoveries[country];
            Dictionary<double, int> recUnix = new Dictionary<double, int>();
            foreach (var e in recoveries)
            {
                recUnix.Add(e.Key.ToUnixTime(), e.Value);
            }
            foreach (var e in charts["infected"].First(I => I.name == country).data)
            {
                //filter to start only when there are cases
                if ((int)e[1] > 0)
                {
                    if (recUnix.ContainsKey((double)e[0]))
                    {
                        i.data.Add(new List<object>() { e[0], (int)e[1] - recUnix[(double)e[0]] });
                    }
                }
            }
        }
        else
        {
            i.name = "No recovey data available";
        }

        List<time_chart> c = new List<time_chart>();
        c.Add(i);

        return c;
    }

    /// <summary>
    /// Generates an estimation based on the current cases by day
    /// </summary>
    /// <param name="m">series with the infected cases</param>
    /// <param name="groupLengh">length of the estimation group</param>
    /// <param name="length">how many days to estimate</param>
    /// <returns></returns>
    public List<time_chart> Estimate_Value(time_chart m, int groupLengh, int length)
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
        int firstValue = (int)m.data.Last()[1];
        foreach (var e in etm.data)
        {
            firstValue += Convert.ToInt32(e[1]);
            etl.data.Add(new List<object>() { e[0], firstValue });
        }

        charts.Add(etm);
        charts.Add(etl);
        return charts;
    }

    /// <summary>
    /// Estimates the number of infected people based on the number of deaths and the casualty %
    /// </summary>
    /// <param name="m">series with the lost</param>
    /// <param name="period">how long take an infected to die</param>
    /// <param name="rate">% death rate</param>
    /// <param name="length">how many days to estimate</param>
    /// <returns></returns>
    public time_chart Estimate_Infection(time_chart m, int period, double rate)
    {
        var l = new time_chart();
        l.name = "Estimated Infected";
        l.type = "spline";
        l.yAxis = 0;
        l.data = new List<List<object>>();

        foreach (var e in m.data)
        {
            double date = Convert.ToDouble(e[0]);
            double value = Convert.ToDouble(e[1]);
            if (value <= 0) continue;
            value = Math.Round(value * 100 / rate);
            date = date - (oneUnixDay * period);

            l.data.Add(new List<object>() { date, value });
        }

        return l;
    }
}