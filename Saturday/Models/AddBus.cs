using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Saturday.Models
{
    public class AddBus
    {
       
        public int Id { get; set; }
        [Required]
        public string BusName { get; set; }
        [Required]
        public string BusNumber { get; set; }
        [Required]
       
        public string Source { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Departuare { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public TimeSpan DepartuareTime { get; set; }
        [Required]
        public string Destination { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Arriaval { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public TimeSpan ArrivalTime { get; set; }
        [Required]
        public int MaxCapisity { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdationDate { get; set; }
        public bool IsClose { get; set; }

    }
}
