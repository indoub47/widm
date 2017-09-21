using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using InspectionLib;

namespace InspPoolCommunication
{
    public interface IInspPoolCommunicator
    {
        DataTable FetchById(long id);
        DataTable FetchByVieta(Insp insp);
    }
}
