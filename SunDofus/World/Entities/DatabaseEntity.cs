using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunDofus.World
{
    enum EntityState
    {
        New,
        Modified,
        Deleted,
        Unchanged
    }

    class DatabaseEntity
    {
        public EntityState SaveState;
    }
}
