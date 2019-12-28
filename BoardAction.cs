using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject
{
    abstract class BoardAction
    {
        public string Name { get; set; }
        public string Description { get; set; }

        abstract public bool DoAction();
    }
}
