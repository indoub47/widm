using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widm
{
    public class InvalidInfo : Dictionary<string, object>
    {
        public InvalidInfo(string errorMessage)
        {
            this["message"] = errorMessage;
        }

        public void Incorporate(Dictionary<string, object> dick)
        {
            foreach (var kvp in dick)
            {
                this[kvp.Key] = kvp.Value;
            }
        }

        public string Message => this["message"].ToString();
    }
}
