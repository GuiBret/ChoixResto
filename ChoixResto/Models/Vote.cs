using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChoixResto.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public virtual Resto Resto { get; set; }
        public virtual Utilisateur Utilisateur { get; set; }
    }
}