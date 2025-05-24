using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSK2025.Models.DTOs
{
    public class TimeSeriesPointDto
    {
        public DateTime Period { get; set; }
        public int Count { get; set; }
    }
}