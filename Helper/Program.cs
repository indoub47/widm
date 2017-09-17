using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecordValidation;

namespace Helper
{
    class Program
    {
        static void Main(string[] args)
        {
            object ob = "   78  ";
            int inte = Convert.ToInt32(ob.ToString().Trim());
            Console.Write(inte);
            Console.ReadKey();
        }
    }
}
