using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGameVoice.Domain.Common.Interfaces
{
    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }

        Guid? CreatedBy { get; set; }
        Guid? UpdatedBy { get; set; }
    }
}
