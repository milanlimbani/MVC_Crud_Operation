using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Saturday.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string BusNumber { get; set; }
       
        public string selectedseat { get; set; }

        public DateTime JourneyDate { get; set; }
        public DateTime DateTime { get; set; }
    }
}
