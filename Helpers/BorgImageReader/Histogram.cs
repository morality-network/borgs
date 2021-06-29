using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BorgImageReader
{
    public class Histogram
    {
        public Dictionary<string, List<int>> Data { get; set; }
        public List<KeyValuePair<string, string>> FormattedData => Format();

        public Histogram()
        {
            Data = new Dictionary<string, List<int>>();
        }

        public void AddData(string key, int value)
        {
            if (Data.TryGetValue(key, out var data))
                Data[key].Add(value);
            else
                Data.Add(key, new List<int>() { value });
        }

        private List<KeyValuePair<string, string>> Format()
        {
            return Data.AsEnumerable()
                .Select(x => new KeyValuePair<string, string>(x.Key, string.Join(",", x.Value)))
                .Where(x => x.Key != "000000")
                .ToList();
        }
    }
}
