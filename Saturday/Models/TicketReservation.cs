using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Saturday.Models
{
    public class TicketReservation
    {
       
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string BusNumber { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateTime { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public TimeSpan TimeDetails { get; set; }
        public List<bool> SeatAvability { get; set; }
        public List<int> SelectedSeat { get; set; }
        public DateTime JourneyDate { get; set; }
    }
}
