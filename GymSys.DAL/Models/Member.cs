using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.DAL.Models
{
    public class Member : GymUser
    {
        public string? Photo { get; set; }

        //JoinDate = CreatedAt of BaseEntity
    }
}
