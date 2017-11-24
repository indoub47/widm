using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widm
{
    public interface IBatchInspValidator
    {
        IList<InvalidInfo> Validate(IList<Insp> inspections);
    }
}
