using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.DAL.Data.Models
{
    public class Membership : BaseEntity
    {
        //StartDate = CreatedAt of BaseEntity
        public DateTime EndDate { get; set; }

        #region UnMapped
        public string Status => EndDate > DateTime.Now ? "Active" : "Expired";
        public bool IsActive => EndDate > DateTime.Now;
        #endregion

        #region Relationships
        public Member Member { get; set; } = default!;
        public int MemberId { get; set; }

        public Plan Plan { get; set; } = default!;
        public int PlanId { get; set; }
        #endregion
    }
}
