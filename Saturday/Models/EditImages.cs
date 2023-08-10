using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Saturday.Models
{
    public class EditImages
    {
        public int Id { get; set; }
        [Required]
        public string ImageTitle { get; set; }

        public IFormFile ProductImage { get; set; }
        [Required]
        public string ImageDescription { get; set; }
        public bool IsClose { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime UpdatetionDate { get; set; }
    }
}
