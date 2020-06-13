using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


public class Logic
{

    const double oneUnixDay = 24 * 60 * 60 * 1000;


    public Dictionary<string, string> _codeMappings = new Dictionary<string, string>() {
    {"AF","Afghanistan"},
{"AX","Åland Islands"},
{"AL","Albania"},
{"DZ","Algeria"},
{"AS","American Samoa"},
{"AD","Andorra"},
{"AO","Angola"},
{"AI","Anguilla"},
{"AQ","Antarctica"},
{"AG","Antigua and Barbuda"},
{"AR","Argentina"},
{"AM","Armenia"},
{"AW","Aruba"},
{"AU","Australia"},
{"AT","Austria"},
{"AZ","Azerbaijan"},
{"BH","Bahrain"},
{"BS","Bahamas"},
{"BD","Bangladesh"},
{"BB","Barbados"},
{"BY","Belarus"},
{"BE","Belgium"},
{"BZ","Belize"},
{"BJ","Benin"},
{"BM","Bermuda"},
{"BT","Bhutan"},
{"BO","Bolivia, Plurinational State of"},
{"BQ","Bonaire, Sint Eustatius and Saba"},
{"BA","Bosnia and Herzegovina"},
{"BW","Botswana"},
{"BV","Bouvet Island"},
{"BR","Brazil"},
{"IO","British Indian Ocean Territory"},
{"BN","Brunei Darussalam"},
{"BG","Bulgaria"},
{"BF","Burkina Faso"},
{"BI","Burundi"},
{"KH","Cambodia"},
{"CM","Cameroon"},
{"CA","Canada"},
{"CV","Cape Verde"},
{"KY","Cayman Islands"},
{"CF","Central African Republic"},
{"TD","Chad"},
{"CL","Chile"},
{"CN","China"},
{"CX","Christmas Island"},
{"CC","Cocos (Keeling) Islands"},
{"CO","Colombia"},
{"KM","Comoros"},
{"CG","Congo"},
{"CD","Congo, the Democratic Republic of the"},
{"CK","Cook Islands"},
{"CR","Costa Rica"},
{"CI","Côte d'Ivoire"},
{"HR","Croatia"},
{"CU","Cuba"},
{"CW","Curaçao"},
{"CY","Cyprus"},
{"CZ","Czech Republic"},
{"DK","Denmark"},
{"DJ","Djibouti"},
{"DM","Dominica"},
{"DO","Dominican Republic"},
{"EC","Ecuador"},
{"EG","Egypt"},
{"SV","El Salvador"},
{"GQ","Equatorial Guinea"},
{"ER","Eritrea"},
{"EE","Estonia"},
{"ET","Ethiopia"},
{"FK","Falkland Islands (Malvinas)"},
{"FO","Faroe Islands"},
{"FJ","Fiji"},
{"FI","Finland"},
{"FR","France"},
{"GF","French Guiana"},
{"PF","French Polynesia"},
{"TF","French Southern Territories"},
{"GA","Gabon"},
{"GM","Gambia"},
{"GE","Georgia"},
{"DE","Germany"},
{"GH","Ghana"},
{"GI","Gibraltar"},
{"GR","Greece"},
{"GL","Greenland"},
{"GD","Grenada"},
{"GP","Guadeloupe"},
{"GU","Guam"},
{"GT","Guatemala"},
{"GG","Guernsey"},
{"GN","Guinea"},
{"GW","Guinea-Bissau"},
{"GY","Guyana"},
{"HT","Haiti"},
{"HM","Heard Island and McDonald Islands"},
{"VA","Holy See (Vatican City State)"},
{"HN","Honduras"},
{"HK","Hong Kong"},
{"HU","Hungary"},
{"XK","Kosovo"},
{"IS","Iceland"},
{"IN","India"},
{"ID","Indonesia"},
{"IR","Iran, Islamic Republic of"},
{"IQ","Iraq"},
{"IE","Ireland"},
{"IM","Isle of Man"},
{"IL","Israel"},
{"IT","Italy"},
{"JM","Jamaica"},
{"JP","Japan"},
{"JE","Jersey"},
{"JO","Jordan"},
{"KZ","Kazakhstan"},
{"KE","Kenya"},
{"KI","Kiribati"},
{"KP","Korea, Democratic People's Republic of"},
{"KR","Korea, Republic of"},
{"KW","Kuwait"},
{"KG","Kyrgyzstan"},
{"LA","Lao People's Democratic Republic"},
{"LV","Latvia"},
{"LB","Lebanon"},
{"LS","Lesotho"},
{"LR","Liberia"},
{"LY","Libya"},
{"LI","Liechtenstein"},
{"LT","Lithuania"},
{"LU","Luxembourg"},
{"MO","Macao"},
{"MK","Macedonia, the Former Yugoslav Republic of"},
{"MG","Madagascar"},
{"MW","Malawi"},
{"MY","Malaysia"},
{"MV","Maldives"},
{"ML","Mali"},
{"MT","Malta"},
{"MH","Marshall Islands"},
{"MQ","Martinique"},
{"MR","Mauritania"},
{"MU","Mauritius"},
{"YT","Mayotte"},
{"MX","Mexico"},
{"FM","Micronesia, Federated States of"},
{"MD","Moldova, Republic of"},
{"MC","Monaco"},
{"MN","Mongolia"},
{"ME","Montenegro"},
{"MS","Montserrat"},
{"MA","Morocco"},
{"MZ","Mozambique"},
{"MM","Myanmar"},
{"NA","Namibia"},
{"NR","Nauru"},
{"NP","Nepal"},
{"NL","Netherlands"},
{"NC","New Caledonia"},
{"NZ","New Zealand"},
{"NI","Nicaragua"},
{"NE","Niger"},
{"NG","Nigeria"},
{"NU","Niue"},
{"NF","Norfolk Island"},
{"MP","Northern Mariana Islands"},
{"NO","Norway"},
{"OM","Oman"},
{"PK","Pakistan"},
{"PW","Palau"},
{"PS","Palestine, State of"},
{"PA","Panama"},
{"PG","Papua New Guinea"},
{"PY","Paraguay"},
{"PE","Peru"},
{"PH","Philippines"},
{"PN","Pitcairn"},
{"PL","Poland"},
{"PT","Portugal"},
{"PR","Puerto Rico"},
{"QA","Qatar"},
{"RE","Réunion"},
{"RO","Romania"},
{"RU","Russian Federation"},
{"RW","Rwanda"},
{"BL","Saint Barthélemy"},
{"SH","Saint Helena, Ascension and Tristan da Cunha"},
{"KN","Saint Kitts and Nevis"},
{"LC","Saint Lucia"},
{"MF","Saint Martin (French part)"},
{"PM","Saint Pierre and Miquelon"},
{"VC","Saint Vincent and the Grenadines"},
{"WS","Samoa"},
{"SM","San Marino"},
{"ST","Sao Tome and Principe"},
{"SA","Saudi Arabia"},
{"SN","Senegal"},
{"RS","Serbia"},
{"SC","Seychelles"},
{"SL","Sierra Leone"},
{"SG","Singapore"},
{"SX","Sint Maarten (Dutch part)"},
{"SK","Slovakia"},
{"SI","Slovenia"},
{"SB","Solomon Islands"},
{"SO","Somalia"},
{"ZA","South Africa"},
{"GS","South Georgia and the South Sandwich Islands"},
{"SS","South Sudan"},
{"ES","Spain"},
{"LK","Sri Lanka"},
{"SD","Sudan"},
{"SR","Suriname"},
{"SJ","Svalbard and Jan Mayen"},
{"SZ","Swaziland"},
{"SE","Sweden"},
{"CH","Switzerland"},
{"SY","Syrian Arab Republic"},
{"TW","Taiwan, Province of China"},
{"TJ","Tajikistan"},
{"TZ","Tanzania, United Republic of"},
{"TH","Thailand"},
{"TL","Timor-Leste"},
{"TG","Togo"},
{"TK","Tokelau"},
{"TO","Tonga"},
{"TT","Trinidad and Tobago"},
{"TN","Tunisia"},
{"TR","Turkey"},
{"TM","Turkmenistan"},
{"TC","Turks and Caicos Islands"},
{"TV","Tuvalu"},
{"UG","Uganda"},
{"UA","Ukraine"},
{"AE","United Arab Emirates"},
{"GB","United Kingdom"},
{"US","United States"},
{"UM","United States Minor Outlying Islands"},
{"UY","Uruguay"},
{"UZ","Uzbekistan"},
{"VU","Vanuatu"},
{"VE","Venezuela, Bolivarian Republic of"},
{"VN","Viet Nam"},
{"VG","Virgin Islands, British"},
{"VI","Virgin Islands, U.S."},
{"WF","Wallis and Futuna"},
{"EH","Western Sahara"},
{"YE","Yemen"},
{"ZM","Zambia"},
{"ZW","Zimbabwe"}
};

    private Dictionary<string, string> _countryMappings = new Dictionary<string, string>()
        {
           {"US","United_States_of_America"},
{"United States","US"},
{"United States of America","USA"},
{"Antigua and Barbuda","AG"},
{"Bosnia and Herzegovina","BA"},
{"Brunei","BN"},
{"Burkina Faso","BF"},
{"Cabo Verde","CV"},
{"Central African Republic","CF"},
{"Cayman Islands","KY"},
{"Costa Rica","CR"},
{"Cote d'Ivoire","CI"},
{"Czechia","CZ"},
{"Czech Republic","CZ"},
{"Czech_Republic","CZ"},
{"Congo (Kinshasa)","CD"},
{"Congo (Brazzaville)","CD"},
{"Congo(Kinshasa)","CD"},
{"Congo(Brazzaville)","CD"},
{"Dominican Republic","DO"},
{"Dominican_Republic","DO"},
{"El Salvador","SV"},
{"El_Salvador","SV"},
{"Equatorial Guinea","GQ"},
{"Faroe Islands","FO"},
{"French Polynesia","PF"},
{"Guinea-Bissau","GW"},
{"Holy See","VA"},
{"Isle of Man","IM"},
{"New Zealand","NZ"},
{"Kosovo","XK"},
{"North Macedonia","MK"},
{"Papua New Guinea","PG"},
{"Puerto Rico","PR"},
{"Saint Kitts and Nevis","KN"},
{"Saint Lucia","LC"},
{"Saint Vincent and the Grenadines","VC"},
{"San Marino","SM"},
{"Saudi Arabia","SA"},
{"South Africa","ZA"},
{"South Korea","KR"},
{"Republic of Korea","KR"},
{"Korea, South","KR"},
{"Sri Lanka","LK"},
{"Taiwan*","TW"},
{"Timor-Leste","TL"},
{"United Arab Emirates","AE"},
{"United Kingdom","GB"},
{"UK","GB"},
{"Tanzania","TZ"},
{"Afghanistan","AF"},
{"Åland Islands","AX"},
{"Albania","AL"},
{"Algeria","DZ"},
{"American Samoa","AS"},
{"Andorra","AD"},
{"Angola","AO"},
{"Anguilla","AI"},
{"Antarctica","AQ"},
{"Argentina","AR"},
{"Armenia","AM"},
{"Aruba","AW"},
{"Australia","AU"},
{"Austria","AT"},
{"Azerbaijan","AZ"},
{"Bahrain","BH"},
{"Bahamas","BS"},
{"Bangladesh","BD"},
{"Barbados","BB"},
{"Belarus","BY"},
{"Belgium","BE"},
{"Belize","BZ"},
{"Benin","BJ"},
{"Bermuda","BM"},
{"Bhutan","BT"},
{"Bolivia, Plurinational State of","BO"},
{"Bonaire, Sint Eustatius and Saba","BQ"},
{"Botswana","BW"},
{"Bouvet Island","BV"},
{"Brazil","BR"},
{"British Indian Ocean Territory","IO"},
{"Brunei Darussalam","BN"},
{"Bulgaria","BG"},
{"Burundi","BI"},
{"Cambodia","KH"},
{"Cameroon","CM"},
{"Canada","CA"},
{"Cape Verde","CV"},
{"Chad","TD"},
{"Chile","CL"},
{"China","CN"},
{"Christmas Island","CX"},
{"Cocos (Keeling) Islands","CC"},
{"Colombia","CO"},
{"Comoros","KM"},
{"Congo","CG"},
{"Congo, the Democratic Republic of the","CD"},
{"Cook Islands","CK"},
{"Côte d'Ivoire","CI"},
{"Croatia","HR"},
{"Cuba","CU"},
{"Curaçao","CW"},
{"Cyprus","CY"},
{"Denmark","DK"},
{"Djibouti","DJ"},
{"Dominica","DM"},
{"Ecuador","EC"},
{"Egypt","EG"},
{"Eritrea","ER"},
{"Estonia","EE"},
{"Ethiopia","ET"},
{"Falkland Islands (Malvinas)","FK"},
{"Fiji","FJ"},
{"Finland","FI"},
{"France","FR"},
{"French Guiana","GF"},
{"French Southern Territories","TF"},
{"Gabon","GA"},
{"Gambia","GM"},
{"Georgia","GE"},
{"Germany","DE"},
{"Ghana","GH"},
{"Gibraltar","GI"},
{"Greece","GR"},
{"Greenland","GL"},
{"Grenada","GD"},
{"Guadeloupe","GP"},
{"Guam","GU"},
{"Guatemala","GT"},
{"Guernsey","GG"},
{"Guinea","GN"},
{"Guyana","GY"},
{"Haiti","HT"},
{"Heard Island and McDonald Islands","HM"},
{"Holy See (Vatican City State)","VA"},
{"Honduras","HN"},
{"Hong Kong","HK"},
{"Hungary","HU"},
{"Iceland","IS"},
{"India","IN"},
{"Indonesia","ID"},
{"Iran, Islamic Republic of","IR"},
{"Iraq","IQ"},
{"Ireland","IE"},
{"Israel","IL"},
{"Italy","IT"},
{"Jamaica","JM"},
{"Japan","JP"},
{"Jersey","JE"},
{"Jordan","JO"},
{"Kazakhstan","KZ"},
{"Kenya","KE"},
{"Kiribati","KI"},
{"Korea, Democratic People's Republic of","KP"},
{"Korea, Republic of","KR"},
{"Kuwait","KW"},
{"Kyrgyzstan","KG"},
{"Lao People's Democratic Republic","LA"},
{"Latvia","LV"},
{"Lebanon","LB"},
{"Lesotho","LS"},
{"Liberia","LR"},
{"Libya","LY"},
{"Liechtenstein","LI"},
{"Lithuania","LT"},
{"Luxembourg","LU"},
{"Macao","MO"},
{"Macedonia, the Former Yugoslav Republic of","MK"},
{"Madagascar","MG"},
{"Malawi","MW"},
{"Malaysia","MY"},
{"Maldives","MV"},
{"Mali","ML"},
{"Malta","MT"},
{"Marshall Islands","MH"},
{"Martinique","MQ"},
{"Mauritania","MR"},
{"Mauritius","MU"},
{"Mayotte","YT"},
{"Mexico","MX"},
{"Micronesia, Federated States of","FM"},
{"Moldova, Republic of","MD"},
{"Monaco","MC"},
{"Mongolia","MN"},
{"Montenegro","ME"},
{"Montserrat","MS"},
{"Morocco","MA"},
{"Mozambique","MZ"},
{"Myanmar","MM"},
{"Namibia","NA"},
{"Nauru","NR"},
{"Nepal","NP"},
{"Netherlands","NL"},
{"New Caledonia","NC"},
{"Nicaragua","NI"},
{"Niger","NE"},
{"Nigeria","NG"},
{"Niue","NU"},
{"Norfolk Island","NF"},
{"Northern Mariana Islands","MP"},
{"Norway","NO"},
{"Oman","OM"},
{"Pakistan","PK"},
{"Palau","PW"},
{"Palestine, State of","PS"},
{"Panama","PA"},
{"Paraguay","PY"},
{"Peru","PE"},
{"Philippines","PH"},
{"Pitcairn","PN"},
{"Poland","PL"},
{"Portugal","PT"},
{"Qatar","QA"},
{"Réunion","RE"},
{"Romania","RO"},
{"Russian Federation","RU"},
{"Rwanda","RW"},
{"Saint Barthélemy","BL"},
{"Saint Helena, Ascension and Tristan da Cunha","SH"},
{"Saint Martin (French part)","MF"},
{"Saint Pierre and Miquelon","PM"},
{"Samoa","WS"},
{"Sao Tome and Principe","ST"},
{"Senegal","SN"},
{"Serbia","RS"},
{"Seychelles","SC"},
{"Sierra Leone","SL"},
{"Singapore","SG"},
{"Sint Maarten (Dutch part)","SX"},
{"Slovakia","SK"},
{"Slovenia","SI"},
{"Solomon Islands","SB"},
{"Somalia","SO"},
{"South Georgia and the South Sandwich Islands","GS"},
{"South Sudan","SS"},
{"Spain","ES"},
{"Sudan","SD"},
{"Suriname","SR"},
{"Svalbard and Jan Mayen","SJ"},
{"Swaziland","SZ"},
{"Sweden","SE"},
{"Switzerland","CH"},
{"Syrian Arab Republic","SY"},
{"Taiwan, Province of China","TW"},
{"Tajikistan","TJ"},
{"Tanzania, United Republic of","TZ"},
{"Thailand","TH"},
{"Togo","TG"},
{"Tokelau","TK"},
{"Tonga","TO"},
{"Trinidad and Tobago","TT"},
{"Tunisia","TN"},
{"Turkey","TR"},
{"Turkmenistan","TM"},
{"Turks and Caicos Islands","TC"},
{"Tuvalu","TV"},
{"Uganda","UG"},
{"Ukraine","UA"},
{"United_Kingdom","GB"},
{"United States Minor Outlying Islands","UM"},
{"Uruguay","UY"},
{"Uzbekistan","UZ"},
{"Vanuatu","VU"},
{"Venezuela, Bolivarian Republic of","VE"},
{"Venezuela","VE"},
{"Viet Nam","VN"},
{"Vietnam","VN"},
{"Virgin Islands, British","VG"},
{"Virgin Islands, U.S.","VI"},
{"Wallis and Futuna","WF"},
{"Western Sahara","EH"},
{"Yemen","YE"},
{"Zambia","ZM"},
{"Zimbabwe","ZW"}
    };

    public Dictionary<string, string> _continentMappings = new Dictionary<string, string>();

    private Dictionary<string, string> _countryCodes2 = new Dictionary<string, string>();
    private Dictionary<string, string> _countryCodes3 = new Dictionary<string, string>();
    private Dictionary<string, List<time_chart>> _charts = new Dictionary<string, List<time_chart>>();
    private Dictionary<string, List<time_chart>> _mobility = new Dictionary<string, List<time_chart>>();
    private Dictionary<string, List<time_chart>> _country = new Dictionary<string, List<time_chart>>();
    //private Dictionary<string, Dictionary<DateTime, int>> _tests = new Dictionary<string, Dictionary<DateTime, int>>();
    private DateTime _lastCheck = DateTime.MinValue;
    private Dictionary<string, Dictionary<DateTime, int>> _recoveries;
    private List<time_event> _events;
    private List<string> _countries = new List<string>();
    private DateTime _lastUpdate;

    //private Dictionary<string, string> _countryData = new Dictionary<string, string>()
    //{
    //    { "ES","https://covid19.isciii.es/resources/serie_historica_acumulados.csv"}
    //};

    public List<List<string>> Countries
    {
        get
        {
            List<List<string>> values = new List<List<string>>();
            var tmpCountries = _charts["infected"].Select(C => C.code).ToList();
            foreach (var c in tmpCountries)
            {
                values.Add(new List<string>() { c, _codeMappings[c] });
            }
            return values;
        }
    }

    public List<string> Continents
    {
        get
        {
            return _continentMappings.Select(V => V.Value).Distinct().ToList();
        }
    }

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
            if (_charts.Count == 0 || _lastCheck < DateTime.Now.AddHours(-6))
            {
#if !DEBUG
                try
                {
                    DownloadFile();
                } catch (Exception ex) {
                }
#endif
                _countries = new List<string>();
                _charts = new Dictionary<string, List<time_chart>>();
                _charts.Add("infected", readAllData(null));

                var tmpCountries = _charts["infected"].Select(C => C.name).ToList();
                _countries = _codeMappings.Where(M => tmpCountries.Contains(M.Key)).Select(S => S.Value).ToList();

                _charts.Add("lost", readAllData("lost"));
                _recoveries = readRecoveries();
                _events = GetEvents();
                //_tests = readTests();
                _mobility = readMovilityData();
                //country data
                _country = new Dictionary<string, List<time_chart>>();
                //foreach(var e in _countryData)
                //{
                //    _country.Add(e.Key, readCountryData(e.Key));
                //}
                _lastCheck = DateTime.Now;
            }
        }
    }

    public double PandemicSeriesStart(string countryCode, int value)
    {
        var series = _charts["infected"].FirstOrDefault(F => F.code == countryCode);
        foreach (var d in series.data)
        {
            if ((int)d[1] >= value)
            {
                return (double)d[0];
            }
        }
        return 0;
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
        ////test data
        //url = "https://ourworldindata.org/01cb0ad9-af1e-41b6-bdd5-26cfe17bb142";
        //data = net.DownloadData(url);
        //System.IO.File.WriteAllBytes("c:\\temp\\test-" + DateTime.Today.ToString("yyyy-MM-dd") + ".csv", data);
        //google mobility data
        url = "https://www.gstatic.com/covid19/mobility/Global_Mobility_Report.csv";
        data = net.DownloadData(url);
        System.IO.File.WriteAllBytes("c:\\temp\\mobility-google-" + DateTime.Today.ToString("yyyy-MM-dd") + ".csv", data);
        //data by country
        //foreach (var e in _countryData)
        //{
        //    data = net.DownloadData(e.Value);
        //    System.IO.File.WriteAllBytes("c:\\temp\\" + e.Key + "-" + DateTime.Today.ToString("yyyy-MM-dd") + ".csv", data);
        //}

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

    public List<string[]> GetCountryFile(string countryCode)
    {
        DateTime when = DateTime.Today;
        string fileName = "c:\\temp\\" + countryCode + "-" + when.ToString("yyyy-MM-dd") + ".csv";
        while (!System.IO.File.Exists(fileName))
        {
            when = when.AddDays(-1);
            fileName = "c:\\temp\\" + countryCode + "-" + when.ToString("yyyy-MM-dd") + ".csv";
        }
        return Extensions.SplitCSV(fileName);
    }

    public List<string[]> GetGoogleMobilityFile()
    {
        DateTime when = DateTime.Today;
        string fileName = "c:\\temp\\mobility-google-" + when.ToString("yyyy-MM-dd") + ".csv";
        while (!System.IO.File.Exists(fileName))
        {
            when = when.AddDays(-1);
            fileName = "c:\\temp\\mobility-google-" + when.ToString("yyyy-MM-dd") + ".csv";
        }
        return Extensions.SplitCSV(fileName);
    }

    public List<string[]> GetAppleMobilityFile()
    {
        DateTime when = DateTime.Today;
        string fileName = "c:\\temp\\mobility-apple-" + when.ToString("yyyy-MM-dd") + ".csv";
        while (!System.IO.File.Exists(fileName))
        {
            when = when.AddDays(-1);
            fileName = "c:\\temp\\mobility-apple-" + when.ToString("yyyy-MM-dd") + ".csv";
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

    public List<string[]> GetTests()
    {
        var when = DateTime.Today;
        string fileName = "c:\\temp\\test-" + when.ToString("yyyy-MM-dd") + ".csv";
        while (!System.IO.File.Exists(fileName))
        {
            when = when.AddDays(-1);
            fileName = "c:\\temp\\test-" + when.ToString("yyyy-MM-dd") + ".csv";
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


    private List<time_chart> readCountryData(string countryCode)
    {
        var charts = new List<time_chart>();
        var fileLines = GetCountryFile(countryCode);
        foreach (string[] values in fileLines.Skip(1))
        {

            DateTime when = DateTime.ParseExact(values[1], new string[] { "dd/MM/yyyy", "dd-MM-yyyy" }, CultureInfo.InvariantCulture);
            int infected = int.Parse(string.IsNullOrEmpty(values[2]) ? "0" : values[2]);
            int recovered = int.Parse(string.IsNullOrEmpty(values[2]) ? "0" : values[6]);
            int lost = int.Parse(string.IsNullOrEmpty(values[2]) ? "0" : values[5]);
            //int lost = int.Parse(values[5]);
            string prov = values[0];
            if (string.IsNullOrEmpty(prov) || prov.Length > 2)
            {
                continue;
            }

            double unixTimestamp = (when.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            unixTimestamp = unixTimestamp * 1000;

            var who = charts.FirstOrDefault(C => C.code == "I-" + prov);
            if (who == null)
            {
                charts.Add(new time_chart() { name = prov, code = "I-" + prov, yAxis = 0, type = "spline", data = new List<List<object>>() { new List<object>() { unixTimestamp, infected } } });
                charts.Add(new time_chart() { name = prov, code = "D-" + prov, yAxis = 0, type = "spline", data = new List<List<object>>() { new List<object>() { unixTimestamp, lost } } });
                charts.Add(new time_chart() { name = prov, code = "R-" + prov, yAxis = 0, type = "spline", data = new List<List<object>>() { new List<object>() { unixTimestamp, recovered } } });
            }
            else
            {

            }

            who.data.Add(new List<object>() { unixTimestamp, infected });


        }
        return charts;
    }

    private List<time_chart> readAllData(string field)
    {
        int lineIx = 0;
        var charts = new List<time_chart>();
        _countryCodes2 = new Dictionary<string, string>();
        _countryCodes3 = new Dictionary<string, string>();
        _continentMappings = new Dictionary<string, string>();
        var fileLines = GetFile();
        foreach (string[] values in fileLines)
        {
            lineIx++;
            if (lineIx == 1) continue;
            if (string.IsNullOrEmpty(values[8]) || values[8] == "N/A")
            {
                continue;
            }

            //code2
            if (!_codeMappings.ContainsKey(values[7]))
            {
                if (_countryMappings.ContainsKey(values[6]))
                {
                    values[7] = _countryMappings[values[6]];
                }
            }

            if (!_continentMappings.ContainsKey(values[7]))
            {
                _continentMappings.Add(values[7], values[10]);
            }

            //add values not mapped from the list
            if (!_countryMappings.ContainsKey(values[6]))
            {
                _countryMappings.Add(values[6], values[7]);
            }
            if (_codeMappings.ContainsKey(values[7]))
            {
                if (!charts.Any(A => A.code == values[7]))
                {
                    charts.Add(new time_chart() { name = _codeMappings[values[7]], code = values[7], population = double.Parse("0" + values[9]), yAxis = 0, type = "spline", data = new List<List<object>>() });
                }
            }
            else
            {
                continue;
            }

        }

        DateTime triggerDate = new DateTime(2020, 1, 13);
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
            string country = values[7];

            if (string.IsNullOrEmpty(values[8]) || values[8] == "N/A")
            {
                continue;
            }

            if (when < triggerDate) continue;

            double unixTimestamp = (when.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            unixTimestamp = unixTimestamp * 1000;

            var who = charts.First(F => F.code == country);
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

    private Dictionary<string, Dictionary<DateTime, int>> readTests()
    {
        var data = new Dictionary<string, Dictionary<DateTime, int>>();
        DateTime triggerDate = new DateTime(2020, 1, 13);

        int lineIx = 0;
        foreach (string[] values in GetTests())
        {
            lineIx++;
            if (lineIx <= 3) continue;


            DateTime when;
            string country;
            int recovered = 0;

            when = DateTime.ParseExact(values[4], new string[] { "yyyy-MM-dd", "dd/MM/yyyy", "dd-MM-yyyy" }, CultureInfo.InvariantCulture);
            recovered = int.Parse(values[5]);
            country = values[0];

            if (_countryMappings.ContainsKey(country))
            {
                country = _countryMappings[country];
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

    private Dictionary<string, List<time_chart>> readMovilityData()
    {
        var charts = new Dictionary<string, List<time_chart>>();
        var fileLines = GetGoogleMobilityFile();
        //foreach (string[] values in fileLines)
        //{
        //    if (_codeMappings.ContainsKey(values[0]))
        //    {
        //        var countryName = values[0];
        //        if (!charts.ContainsKey(countryName))
        //        {
        //            charts.Add(countryName, new List<time_chart>() { new time_chart() { name = "[G]-" + values[1], marker = new { enabled = false }, yAxis = 0, type = "spline", data = new List<List<object>>() } });
        //        }
        //        else
        //        {
        //            if (!charts[countryName].Any(A => A.name == "[G]-" + values[1]))
        //            {
        //                charts[countryName].Add(new time_chart() { name = "[G]-" + values[1], marker = new { enabled = false }, yAxis = 0, type = "spline", data = new List<List<object>>() });
        //            }
        //        }
        //    }
        //}

        //DateTime triggerDate = new DateTime(2020, 1, 20);
        //foreach (string[] values in fileLines)
        //{
        //    DateTime when = DateTime.ParseExact(values[2], new string[] { "yyyy-MM-dd", "dd/MM/yyyy", "dd-MM-yyyy" }, CultureInfo.InvariantCulture);
        //    int value = int.Parse(values[3])+100; //level with apple data
        //    string country = values[0];
        //    if (!_codeMappings.ContainsKey(country))
        //    {
        //        continue;
        //    }

        //    if (when < triggerDate) continue;

        //    double unixTimestamp = (when.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        //    unixTimestamp = unixTimestamp * 1000;

        //    var who = charts[country].First(F => F.name == "[G]-" + values[1]);
        //    who.data.Add(new List<object>() { unixTimestamp, value });
        //}


        foreach (string[] values in fileLines.Skip(1))
        {

            if (!string.IsNullOrEmpty(values[2].Trim()))
            {
                continue;
            }

            var countryCode = values[0];

            if (!_codeMappings.ContainsKey(countryCode))
            {
                continue;
            }

            if (!charts.ContainsKey(countryCode))
            {
                charts.Add(countryCode, new List<time_chart>() {
                    new time_chart() { name = "[G]-Retail & recreation", marker = new { enabled = false }, yAxis = 0, type = "spline", data = new List<List<object>>() },
                    new time_chart() { name = "[G]-Grocery & pharmacy" + values[1], marker = new { enabled = false }, yAxis = 0, type = "spline", data = new List<List<object>>() },
                    new time_chart() { name = "[G]-Parks", marker = new { enabled = false }, yAxis = 0, type = "spline", data = new List<List<object>>() },
                    new time_chart() { name = "[G]-Transit stations", marker = new { enabled = false }, yAxis = 0, type = "spline", data = new List<List<object>>() },
                    new time_chart() { name = "[G]-Workplaces", marker = new { enabled = false }, yAxis = 0, type = "spline", data = new List<List<object>>() },
                    new time_chart() { name = "[G]-Residential", marker = new { enabled = false }, yAxis = 0, type = "spline", data = new List<List<object>>() }
                }
                );
            }

            DateTime when = DateTime.ParseExact(values[6], new string[] { "yyyy-MM-dd", "dd/MM/yyyy", "dd-MM-yyyy" }, CultureInfo.InvariantCulture);
            for (var i = 7; i < 11; i++)
            {
                charts[countryCode][i - 7].data.Add(new List<object>() { when.ToUnixTime(), double.Parse(string.IsNullOrEmpty(values[i]) ? "0" : values[i]) + 100 });
            }


        }


        fileLines = GetAppleMobilityFile();
        //get the dates from the frist line
        Dictionary<int, double> dateRange = new Dictionary<int, double>();
        for (var i = 6; i < fileLines[0].Length; i++)
        {
            DateTime when = DateTime.ParseExact(fileLines[0][i], new string[] { "yyyy-MM-dd", "dd/MM/yyyy", "dd-MM-yyyy" }, CultureInfo.InvariantCulture);
            dateRange.Add(i, when.ToUnixTime());
        }

        foreach (string[] values in fileLines.Skip(1))
        {
            var countryCode = values[1];
            if (values[0] != "country/region") continue;
            if (_countryMappings.ContainsKey(countryCode))
            {
                countryCode = _countryMappings[countryCode];
            }
            else if (_countryMappings.ContainsKey(countryCode.Replace(" ", "_")))
            {
                countryCode = _countryMappings[countryCode.Replace(" ", "_")];
            }
            else
            {
                continue;
            }

            if (!charts.ContainsKey(countryCode))
            {
                var data = new List<List<object>>();
                for (var i = 6; i < values.Length; i++)
                {
                    data.Add(new List<object>() { dateRange[i], double.Parse(string.IsNullOrEmpty(values[i]) ? "0" : values[i]) });

                }
                charts.Add(countryCode, new List<time_chart>() { new time_chart() { name = "[A]-" + values[2], marker = new { enabled = false }, yAxis = 0, type = "spline", data = data } });
            }
            else
            {
                if (!charts[countryCode].Any(A => A.name == "[A]-" + values[2]))
                {
                    var data = new List<List<object>>();
                    for (var i = 4; i < values.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(values[i]))
                        {
                            data.Add(new List<object>() { dateRange[i], double.Parse(values[i]) });
                        }

                    }
                    charts[countryCode].Add(new time_chart() { name = "[A]-" + values[2], marker = new { enabled = false }, yAxis = 0, type = "spline", data = data });
                }
            }
        }

        return charts;
    }

    private Dictionary<string, Dictionary<DateTime, int>> readRecoveries()
    {
        var data = new Dictionary<string, Dictionary<DateTime, int>>();
        DateTime triggerDate = new DateTime(2020, 1, 13);

        int lineIx = 0;
        foreach (string[] values in GetRecoveries())
        {
            lineIx++;
            if (lineIx <= 3) continue;


            DateTime when;
            int recovered = 0;

            when = DateTime.ParseExact(values[4], new string[] { "yyyy-MM-dd", "dd/MM/yyyy", "dd-MM-yyyy" }, CultureInfo.InvariantCulture);
            recovered = int.Parse(values[5]);
            //country = values[0];

            var countryCode = values[1];

            if (_countryMappings.ContainsKey(countryCode))
            {
                countryCode = _countryMappings[countryCode];
            }
            else if (_countryMappings.ContainsKey(countryCode.Replace(" ", "_")))
            {
                countryCode = _countryMappings[countryCode.Replace(" ", "_")];
            }
            else
            {
                continue;
            }
            //var country = _codeMappings[countryCode];

            if (when < triggerDate) continue;

            if (data.ContainsKey(countryCode))
            {
                if (data[countryCode].ContainsKey(when))
                {
                    data[countryCode][when] += recovered;
                }
                else
                {
                    data[countryCode].Add(when, recovered);
                }
            }
            else
            {
                data.Add(countryCode, new Dictionary<DateTime, int>() { { when, recovered } });
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
            if (me.data != null && me.data.Count > 0)
            {
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
                                lengthEstimation = ix - 1;
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
        foreach (var e in charts["infected"].First(I => I.code == country).data)
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
        if (charts["lost"].Any(I => I.code == country))
        {
            foreach (var e in charts["lost"].First(I => I.code == country).data)
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
                if (e.Value > 0)
                {
                    recUnix.Add(e.Key.ToUnixTime(), e.Value);
                }
            }
            foreach (var l in charts["lost"].First(I => I.code == country).data)
            {
                lostUnix.Add((double)l[0], (int)l[1]);
            }
            foreach (var e in charts["infected"].First(I => I.code == country).data)
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
                double value = Convert.ToDouble(m.data[ix][1]);
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
        for (var ix = 1; ix < m.data.Count; ix++)
        {
            var e = m.data[ix];
            double date = Convert.ToDouble(e[0]);
            double value = Convert.ToDouble(e[1]) - Convert.ToDouble(m.data[ix - 1][1]);
            if (value <= 0) continue;
            date = date - (oneUnixDay * period);

            l.data.Add(new List<object>() { date, Math.Round(value * 100 / rate) });
            lr.data.Add(new List<object>() { date, Math.Round(value * 100 / (rate + 0.5)), Math.Round(value * 100 / (rate - 0.5)) });
        }

        return new List<time_chart>() { lr, l };
    }


    public List<time_chart> Estimte_Propagation(time_chart m, double s, string type = "value")
    {
        double aMax = 0; //A(max) -- max numbers of persons infected in a day
        double tMax = 0; //tm -- day of max numbers of persons infected
        int ix = 1;
        double day1 = Convert.ToDouble(m.data[0][0]);

        foreach (var e in m.data.OrderBy(O => O[0]))
        {
            //double date = Convert.ToDouble(e[0]);
            double value = Convert.ToDouble(e[1]);
            if (value > aMax)
            {
                aMax = value;
                tMax = ix;
            }
            ix++;
        }


        //add 2 days estimation
        time_chart tmp = new time_chart();
        tmp.data = m.data.ToList();
        if (tMax == m.data.Count)
        {
            if (type == "series")
            {
                tmp.data.AddRange(Estimate_Series(m, 2, 4)[1].data);
            }
            else
            {
                tmp.data.AddRange(Estimate_Value(m, 2, 4).data);
            }


            aMax = 0; //A(max) -- max numbers of persons infected in a day
            tMax = 0; //tm -- day of max numbers of persons infected
            ix = 1;
            foreach (var e in tmp.data.OrderBy(O => O[0]))
            {
                //double date = Convert.ToDouble(e[0]);
                double value = Convert.ToDouble(e[1]);
                if (value > aMax)
                {
                    aMax = value;
                    tMax = ix;
                }
                ix++;
            }
        }



        Dictionary<double, int> picos = new Dictionary<double, int>();
        double oldValue = 0;
        Boolean isUp = true;
        int picoDayDiff = 12;
        ix = 0;
        if (tMax != tmp.data.Count)
        {
            foreach (var e in tmp.data.OrderBy(O => O[0]))
            {

                double value = Convert.ToDouble(e[1]);
                double date = Convert.ToDouble(e[0]);
                if (value < oldValue && isUp)
                {
                    if (picos.Count == 0)
                    {
                        picos.Add(oldValue, ix);
                    }
                    else
                    {
                        if (picos.Last().Value + picoDayDiff <= ix)
                        {
                            picos.Add(oldValue, ix);
                        }
                        else
                        {
                            if (oldValue > picos.Last().Key)
                            {
                                picos.Remove(picos.Last().Key);
                                picos.Add(oldValue, ix);
                            }

                        }
                    }
                    isUp = false;
                }
                else if (value > oldValue)
                {
                    isUp = true;

                }
                oldValue = value;



                ix++;
            }
            if (oldValue == aMax && tMax <= tmp.data.Count)
            {
                picos.Add(oldValue, tmp.data.Count - 1);
            }
        }
        else
        {
            picos.Add(aMax, Convert.ToInt32(tMax));
        }

        List<time_chart> lista = new List<time_chart>();

        foreach (var pico in picos)
        {
            //double vValue = pico.Key;
            double vAdjust = 0;
            foreach (var l in lista)
            {

                if (l.data.Count > pico.Value - 1)
                {
                    vAdjust += (double)l.data[pico.Value][1];
                }

            }
            if (pico.Key - vAdjust > 0)
            {
                var tmpchart = PropagationFunction(tmp, pico.Key - vAdjust, pico.Value, s, day1);
                //substract one from the other and do the same
                lista.Add(tmpchart);
                //tmp = substract(tmp, tmpchart);
            }
        }

        //return lista;
        var c = lista.First();
        foreach (var l in lista.Skip(1))
        {
            c = addchart(c, l);
        }
        //return new List<time_chart>() {c };
        var cMax = 0;
        var ctMax = 0;
        for (var i = 0; i < c.data.Count; i++)
        {
            if (Convert.ToInt32(c.data[i][1]) > cMax)
            {
                cMax = Convert.ToInt32(c.data[i][1]);
                ctMax = i;
            }
        }

        ////calculate the initial function
        //time_chart c = PropagationFunction(tmp, aMax, tMax, s, day1);

        //try to adjust the chart
        //when is the 20%
        var dayAdjustUnix = tMax * oneUnixDay;
        var cDayAdjustUnix = ctMax * oneUnixDay;
        var teP = tmp.data.Where(D => Convert.ToDouble(D[1]) <= (aMax * 2 / 10) && Convert.ToDouble(D[0]) <= day1 + dayAdjustUnix).OrderBy(O => (double)O[0]).Last();
        var tmP = c.data.Where(D => Convert.ToDouble(D[1]) <= Convert.ToDouble(teP[1]) && Convert.ToDouble(D[0]) <= day1 + cDayAdjustUnix).OrderBy(O => (double)O[0]).Last();
        //adjust the day difference
        //if (Math.Abs((double)teP[0] - (double)tmP[0]) > 0)
        {
            var dayDiff = ((double)teP[0] - (double)tmP[0]) / oneUnixDay;

            //when is the 40%
            teP = tmp.data.Where(D => Convert.ToDouble(D[1]) <= (aMax * 4 / 10) && Convert.ToDouble(D[0]) <= day1 + dayAdjustUnix).OrderBy(O => (double)O[0]).Last();
            tmP = c.data.Where(D => Convert.ToDouble(D[1]) <= Convert.ToDouble(teP[1]) && Convert.ToDouble(D[0]) <= day1 + cDayAdjustUnix).OrderBy(O => (double)O[0]).Last();
            dayDiff += ((double)teP[0] - (double)tmP[0]) / oneUnixDay;

            //when is the 60%
            teP = tmp.data.Where(D => Convert.ToDouble(D[1]) <= (aMax * 6 / 10) && Convert.ToDouble(D[0]) <= day1 + dayAdjustUnix).OrderBy(O => (double)O[0]).Last();
            tmP = c.data.Where(D => Convert.ToDouble(D[1]) <= Convert.ToDouble(teP[1]) && Convert.ToDouble(D[0]) <= day1 + cDayAdjustUnix).OrderBy(O => (double)O[0]).Last();
            dayDiff += ((double)teP[0] - (double)tmP[0]) / oneUnixDay;

            //when is the 80%
            teP = tmp.data.Where(D => Convert.ToDouble(D[1]) <= (aMax * 8 / 10) && Convert.ToDouble(D[0]) <= day1 + dayAdjustUnix).OrderBy(O => (double)O[0]).Last();
            tmP = c.data.Where(D => Convert.ToDouble(D[1]) <= Convert.ToDouble(teP[1]) && Convert.ToDouble(D[0]) <= day1 + cDayAdjustUnix).OrderBy(O => (double)O[0]).Last();
            dayDiff += ((double)teP[0] - (double)tmP[0]) / oneUnixDay;

            //average?
            dayDiff = Math.Round(dayDiff / 4);

            lista = new List<time_chart>();

            //move the sesies data these days
            if (dayDiff > 0)
            {
                for (var i = 0; i < c.data.Count; i++)
                {
                    c.data[i][0] = (double)c.data[i][0] + oneUnixDay * dayDiff;
                }
            }
            lista.Add(c);

            if (tMax < tmp.data.Count)
            {
                var d = PropagationFunction(tmp, aMax, tMax + dayDiff, s, day1, true);
                d.name = "Estimation (auto)";
                c.name = "Estimation (" + s.ToString("0.0") + ")";
                lista.Add(d);
            }

        }
        return lista;
        //return new List<time_chart>() { c };
    }

    time_chart substract(time_chart a, time_chart b)
    {
        time_chart c = new time_chart();
        c.type = a.type;
        c.yAxis = a.yAxis;
        c.name = a.name;
        c.code = a.code;
        c.marker = a.marker;
        c.data = new List<List<object>>();
        var maxLength = a.data.Count > b.data.Count ? a.data.Count : b.data.Count;
        for (var i = 0; i < maxLength; i++)
        {
            var aValue = a.data.Count > i ? Convert.ToDouble(a.data[i][1]) : 0;
            var bValue = b.data.Count > i ? Convert.ToDouble(b.data[i][1]) : 0;
            if (bValue > aValue) bValue = aValue;
            var tValue = a.data.Count > i ? a.data[i][0] : b.data[i][0];
            c.data.Add(new List<object>() { tValue, aValue - bValue });
        }
        return c;
    }

    time_chart addchart(time_chart a, time_chart b)
    {
        time_chart c = new time_chart();
        c.type = a.type;
        c.yAxis = a.yAxis;
        c.name = a.name;
        c.code = a.code;
        c.marker = a.marker;
        c.data = new List<List<object>>();
        var maxLength = a.data.Count > b.data.Count ? a.data.Count : b.data.Count;
        for (var i = 0; i < maxLength; i++)
        {
            var aValue = a.data.Count > i ? Convert.ToDouble(a.data[i][1]) : 0;
            var bValue = b.data.Count > i ? Convert.ToDouble(b.data[i][1]) : 0;
            var tValue = a.data.Count > i ? a.data[i][0] : b.data[i][0];
            c.data.Add(new List<object>() { tValue, aValue + bValue });
        }
        return c;
    }

    time_chart PropagationFunction(time_chart m, double aMax, double tMax, double s, double day1, bool adjust = false)
    {
        time_chart c = new time_chart();
        c.type = "spline";
        c.yAxis = 0;
        c.name = "Estimation";
        c.code = m.code;
        c.marker = new { enabled = false };
        c.data = new List<List<object>>();
        var ix = 1;
        List<double> sAverage = new List<double>();
        double sCalculated = 0;
        while (true)
        {
            double value = 0;
            //value += fValue(aMax * 1/3, tMax + 60, s+3, ix);
            if (adjust && ix > tMax)
            {
                value = fValue(aMax, tMax, s, ix);

                //ver el descenso y ajustar 
                if (m.data.Count > ix)
                {
                    var cV = Convert.ToDouble(m.data[ix][1]);
                    if (value < cV)
                    {
                        while (value < cV && s < 10)
                        {
                            s += 0.01;
                            value = fValue(aMax, tMax, s, ix);
                        }
                        sAverage.Add(s);
                    }
                    else
                    {
                        while (value > cV && s > 1)
                        {
                            s -= 0.01;
                            value = fValue(aMax, tMax, s, ix);
                        }
                        sAverage.Add(s);
                    }
                }
                else
                {
                    if (sCalculated <= 0)
                    {
                        //use average S value from last 4 days
                        sAverage = sAverage.TakeLast(4).ToList();
                        foreach (var sValue in sAverage)
                        {
                            sCalculated += sValue;
                        }
                        sCalculated = sCalculated / sAverage.Count;
                    }
                    value = fValue(aMax, tMax, sCalculated, ix);
                }
            }
            else
            {
                value = fValue(aMax, tMax, s, ix);
            }
            value = Math.Round(value);
            c.data.Add(new List<object>() { day1 + oneUnixDay * (ix - 1), Math.Round(value) });
            ix++;
            if ((ix > tMax && value <= 10) || ix > 300)
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
        t.code = m.code;
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

    public time_chart CasesByMillion(List<time_chart> m)
    {

        var t = new time_chart();
        t.type = "bar";
        t.name = "Cases by Million";
        t.data = new List<List<object>>();

        foreach (var l in m.Where(M => M.population > 0))
        {
            var finalValue = Convert.ToDouble(l.data.Last()[1]);
            var perMillion = Math.Round(finalValue * 1000000 / l.population);
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
            var series = _mobility[country];
            //generate the "average" line
            var firstTemplate = series.First(S => S.name.StartsWith("[A]"));
            var average = new time_chart()
            {
                name = "[A]-average",
                type = firstTemplate.type,
                marker = firstTemplate.marker,
                yAxis = firstTemplate.yAxis,
                zIndex = firstTemplate.zIndex,
                data = new List<List<object>>()
            };
            foreach (var l in series.Where(S => S.name.StartsWith("[A]")))
            {
                if (average.data.Count <= 0)
                {
                    foreach (var d in l.data)
                    {
                        if (d != null && d.Count == 2)
                        {
                            average.data.Add(new List<object>() { d[0], d[1] });
                        }
                    }
                } else
                {
                    foreach (var d in l.data)
                    {
                        var point = average.data.FirstOrDefault( O => O != null && Convert.ToDouble(O[0]) == Convert.ToDouble(d[0]));
                        if (point != null)
                        {
                            point[1] = Convert.ToDouble(point[1]) + Convert.ToDouble(d[1]);
                        } else
                        {
                            average.data.Add(new List<object>() { d[0], d[1] });
                        }
                    }
                }
                
            }
            foreach (var d in average.data)
            {
                var point = average.data.First(O => O[0] == d[0]);
                point[1] = Convert.ToDouble(point[1]) / 3;
            }
            series.Add(average);
            return series;
        }
        else
        {
            return new List<time_chart>() { new time_chart() { name = "No data available", type = "spline", yAxis = 0 } };
        }
    }

    double fValue(double aMax, double tMax, double s, int day)
    {
        return aMax * Math.Exp((-1 / (2 * s)) * (Math.Pow(day - tMax, 2) / day));

    }
    double fValue2(double aMax, double tMax, double s, int day)
    {
        return aMax * Math.Exp(-(Math.Pow(day - tMax, 2)) / (2 * s * day));
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