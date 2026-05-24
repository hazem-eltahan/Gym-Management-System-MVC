using GymSys.DAL.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.DAL.Data.Models
{
    public class Trainer : GymUser
    {
        public Speciality Speciality { get; set; }

        //HireDate = CreatedAt in BaseEntity
        #region Relationships
        public ICollection<Session> Sessions { get; set; } = default!;
        #endregion
    }
}
