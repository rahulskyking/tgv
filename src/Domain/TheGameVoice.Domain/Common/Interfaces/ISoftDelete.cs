using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGameVoice.Domain.Common.Interfaces
{
    public interface ISoftDelete
    {
        DateTime? DeletedAt { get; set; }

        bool IsDeleted => DeletedAt.HasValue;
    }
}
