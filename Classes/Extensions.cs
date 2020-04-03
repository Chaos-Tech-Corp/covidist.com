using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public static class Extensions
{
    public static double ToUnixTime(this DateTime value)
    {
        double unixTimestamp = (value.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        return unixTimestamp * 1000;
    }

    public static List<string[]> SplitCSV(string fieldName)
    {
        var content = new List<string[]>();
        using (TextFieldParser csvReader = new TextFieldParser(fieldName))
        {
            csvReader.SetDelimiters(new string[] { "," });
            csvReader.HasFieldsEnclosedInQuotes = true;
            
            while (!csvReader.EndOfData)
            {
                content.Add(csvReader.ReadFields());
            }
        }
        return content;
    }
}
