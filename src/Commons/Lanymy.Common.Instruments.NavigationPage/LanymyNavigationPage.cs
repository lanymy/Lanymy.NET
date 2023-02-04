using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.Common.Instruments
{
    public class LanymyNavigationPage : BaseNavigationPage
    {
        public LanymyNavigationPage(Action<BaseNavigationPage> pageingAction) : base(pageingAction)
        {
        }
    }

}
