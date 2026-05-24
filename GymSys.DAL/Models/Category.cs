using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.DAL.Models
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; } = default!;
    }
}
