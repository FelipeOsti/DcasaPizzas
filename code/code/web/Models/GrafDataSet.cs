using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppRoma.Models
{
    public class GrafDataSet
    {
        public List<double> data { get; set; }
        public string label { get; set; }
        public string backgroundColor { get; set; }
        public string borderColor { get; set; }
        public int stack { get; set; }
        public string stackLabel { get; set; }
        public bool fill { get; set; }
    }

    public class GrafDataSetPizza
    {
        public List<double> data { get; set; }
        public List<string> label { get; set; }
        public List<string> backgroundColor { get; set; }
        public List<string> borderColor { get; set; }        
    }
}