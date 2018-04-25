using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChoixResto.Models
{
    [Table("Restos")]
    public class Resto
    {
        public int Id { get; set; }
        [Required]
        public string Nom { get; set; }
        public string Telephone { get; set; }
    }
}