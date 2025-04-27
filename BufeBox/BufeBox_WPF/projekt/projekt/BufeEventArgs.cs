using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt
{
        public class BufeEventArgs : EventArgs
        {
            public Bufe Order { get; }

            public BufeEventArgs(Bufe order)
            {
                Order = order;
            }
        }
    }


