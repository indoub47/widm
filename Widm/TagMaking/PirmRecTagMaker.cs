using System.Collections.Generic;
using System.Linq;

namespace Widm
{
    public class PirmRecTagMaker : RecTagMaker
    {
        public PirmRecTagMaker(string[] mapping)
        {
            this.mapping = mapping;
        }

        public override string Make(IList<object> record)
        {
            return dataRowToVietosKodas(record);
        }
    }
}
