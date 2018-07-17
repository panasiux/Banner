using System;
using System.ComponentModel.DataAnnotations;
using BannerWebApp.Validation;
using Declarations.Interfaces;

namespace BannerWebApp.Models
{
    public class BannerModel : IBanner
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [HtmlCheck]
        public string Html { get; set; }
        [Required]
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}
