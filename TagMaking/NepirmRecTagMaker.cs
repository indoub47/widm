using System;
using System.Collections.Generic;
using System.Linq;

namespace TagMaking
{
    public class NepirmRecTagMaker : RecTagMaker
    {
        public NepirmRecTagMaker()
        {
            mapping = Properties.Settings.Default.NepirmiejiMapping.Split(',').Select(x => x.Trim()).ToArray();
        }

        public override string Make(IList<object> record)
        {
            object objId = getRowCellObject("id", record);
            string vietoskodas = dataRowToVietosKodas(record);
            try
            {
                long id = Convert.ToInt64(objId);
                return $"ID: {id}, {vietoskodas}";
            }
            catch
            {
                return $"ID: {objId.ToString()}, {vietoskodas}";
            }
        }
    }
}
