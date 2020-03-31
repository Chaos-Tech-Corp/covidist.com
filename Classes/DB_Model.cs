using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Country : ITable
{
    public string TableName => "Countries";
    public string code { get; set; }
    public string name { get; set; }
}

public class Province : ITable
{
    public string TableName => "Provinces";
    public int id { get; set; }
    public string country_id { get; set; }
    public string name { get; set; }

    public Country country { get; set; }
}

public class City : ITable
{
    public string TableName => "Cities";
    public int id { get; set; }
    public int province_id { get; set; }
    public string name { get; set; }
    public Province province { get; set; }
}

public class Entry : ITable
{
    public string TableName => "Entries";
    public int id { get; set; }
    public int city_id { get; set; }
    public int province_id { get; set; }
    public City city { get; set; }
    public DateTime date { get; set; }
    public int infected { get; set; }
    public int recovered { get; set; }
    public int critical { get; set; }
    public int lost { get; set; }
}

public class time_chart
{
    public string name { get; set; }
    public List<List<object>> data { get; set; }
    
    public string type { get; set; }
    public int yAxis { get; set; }
}
    