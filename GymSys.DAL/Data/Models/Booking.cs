using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.DAL.Data.Models
{
    public class Booking : BaseEntity
    {
        public bool IsAttended { get; set; }

        //BookingDate = CreatedAt of BaseEntity

        #region Relationships
        public Session Session { get; set; } = default!;
        public int SessionId { get; set; }

        public Member Member { get; set; } = default!;
        public int MemberId { get; set; }
        #endregion
    }
}
