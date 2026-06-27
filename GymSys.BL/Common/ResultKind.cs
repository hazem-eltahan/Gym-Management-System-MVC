using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.BLL.Common
{
    public enum ResultKind
    {
        OK,
        NOT_FOUND,
        CONFLICT,
        VALIDATION_FAILED,
        FORBIDDEN
    }
}
