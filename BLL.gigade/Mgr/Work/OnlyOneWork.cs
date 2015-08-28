using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.gigade.Model;

namespace Working
{
    class OnlyOneWork : Work
    {

        public OnlyOneWork(Schedule fst)
        {
            Initial(fst);
        }

        public override DateTime CurrentExecuteDate()
        {
            return StartDate;
        }

    }
}
