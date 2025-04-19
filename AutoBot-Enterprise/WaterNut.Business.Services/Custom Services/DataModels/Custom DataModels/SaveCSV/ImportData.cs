using System.Collections.Generic;

internal class ImportData
{
    public IDictionary<string, object> res { get; set; }
    public Dictionary<string, int> mapping { get; set; }
    public string[] splits { get; set; }
}