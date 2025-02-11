using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Domain.Models
{
    public abstract class SoftDeletableEntity
    {
        public DateTime? DeletedAt { get; set; }
    }
}
