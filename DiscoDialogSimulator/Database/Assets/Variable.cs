﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoDialogSimulator.Database.Assets
{
    public class Variable : Asset
    {
        public override string ToString()
        {
            return LookupValue("Name");
        }
    }
}