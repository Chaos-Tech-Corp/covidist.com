using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class time_chart
{
    public string name { get; set; }
    public List<List<object>> data { get; set; }
    public string type { get; set; }
    public int yAxis { get; set; }
}

public class time_event
{
    public string country { get; set; }
    public DateTime when { get; set; }
    public string title { get; set; }
}

public class plotLine
{
    public plotLine()
    {
        zIndex = 5; //defaults to 5 --> Above plot lines and series
    }

    public int width { get; set; }
    public double value { get; set; }
    public label label { get; set; }
    public int zIndex { get; set; }
}

public class label
{
    public string text { get; set; }
}