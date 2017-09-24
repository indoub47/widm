using System.Collections.Generic;
using System.Linq;

namespace TagMaking
{
    public class PirmiejiRecordTagMaker : RecordTagMaker
    {
        public PirmiejiRecordTagMaker()
        {
            mapping = Properties.Settings.Default.PirmiejiMapping.Split(',').Select(x => x.Trim()).ToArray();
        }

        public override string Make(IList<object> record)
        {
            return dataRowToVietosKodas(record);
        }
    }
}
