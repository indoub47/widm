using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widm
{
    public class InspTagMaker : IInspTagMaker
    {
        public string Make(Insp insp)
        {
            if (insp.Id == null)
            {
                return insp.VietosKodas;
            }
            else
            {
                return $"ID: {insp.Id} {insp.VietosKodas}";
            }
        }
    }
}
