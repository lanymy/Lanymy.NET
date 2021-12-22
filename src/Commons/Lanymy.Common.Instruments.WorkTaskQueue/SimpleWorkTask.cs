using System;

namespace Lanymy.Common.Instruments
{


    public class SimpleWorkTask<TData> : BaseSimpleWorkTask<TData>
    {



        public SimpleWorkTask(Action<TData> workAction) : base(workAction)
        {


        }


    }

}
