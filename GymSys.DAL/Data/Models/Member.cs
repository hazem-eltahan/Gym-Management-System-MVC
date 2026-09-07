using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.DAL.Data.Models
{
    public class Member : GymUser
    {
        public string? Photo { get; set; }

        //JoinDate = CreatedAt of BaseEntity

        #region Relationships
        public HealthRecord HealthRecord { get; set; } = default!;
        public ICollection<Membership> Memberships { get; set; } = default!;

        public ICollection<Booking> MemberSessions { get; set; } = default!;
        #endregion
    }
}
