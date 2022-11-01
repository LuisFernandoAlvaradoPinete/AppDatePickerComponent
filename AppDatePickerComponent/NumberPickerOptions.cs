using System;
using System.Collections.Generic;
using System.Text;

namespace AppDatePickerComponent
{
    public class NumberPickerOptions
    {
        public int Initial { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public int Step { get; set; }
        public string DisplaySuffix { get; set; }
    }
}
