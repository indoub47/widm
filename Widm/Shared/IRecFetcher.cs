﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widm
{
    public interface IRecFetcher
    {
        List<IList<object>> Fetch();

        string[] GetMapping();
    }
}
