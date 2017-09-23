using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;


namespace RecordValidation
{
    public struct RecordContext : IRecordContext 
    {
        public List<object> Objects { get; set; }

        public RecordContext(params object[] context)
        {
            Objects = new List<object>();
            AddContext(context);
        }

        public object Clone()
        {
            return new RecordContext(Objects.ToArray());
        }

        public void AddContext(params object[] context)
        {
            foreach(var ob in context)
            {
                Objects.Add(ob);
            }
        }
    }
}
