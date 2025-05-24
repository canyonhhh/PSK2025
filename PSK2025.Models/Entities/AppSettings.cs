using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSK2025.Models.Entities
{
    public class AppSettings
    {
        [Key] public int Id { get; set; }
        public bool OrderingPaused { get; set; } = false;
        // ateityje galetu butu daugiau flagu, kaip MaintenenaceMode, etc.
    }
}