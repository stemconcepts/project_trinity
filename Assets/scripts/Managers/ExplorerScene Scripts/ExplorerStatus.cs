using AssemblyCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.scripts.Managers.ExplorerScene_Scripts
{
    [Serializable]
    public class ExplorerStatus
    {
        public Guid id = Guid.NewGuid();
        //public List<SingleStatusModel> statuses;
        public SingleStatusModel status;
        public int power;
        public int duration;
        public bool canStack = false;
        public bool dispellable, affectAll = true;
    }
}
