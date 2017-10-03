using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionLib;

namespace Widm
{
    public class RepeatFinder
    {
        public Dictionary<string, List<Insp>> FindRepeats(List<Insp> inspList)
        {
            return inspList.OrderBy(x => x.VietosKodas).GroupBy(x => x.VietosKodas, (key, group) => new
            {
                VietosKodas = key,
                GrByVietosKodas = group.OrderBy(y => y.TData).ThenBy(y => y.Kelintas).ToList()
            }).Where(z => z.GrByVietosKodas.Count() > 1).ToDictionary(k => k.VietosKodas, g => g.GrByVietosKodas);
        }

    }
}
