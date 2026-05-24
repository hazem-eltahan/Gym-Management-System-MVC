using GymSys.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.DAL.Models
{
    public class Trainer : GymUser
    {
        public Speciality Speciality { get; set; }

        //HireDate = CreatedAt in BaseEntity
    }
}
