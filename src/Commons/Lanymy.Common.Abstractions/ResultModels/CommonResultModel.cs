using System;

namespace Lanymy.Common.Abstractions.ResultModels
{
    public class CommonResultModel
    {

        public bool IsSuccess { get; set; }

        public Exception Exception { get; set; }

    }
}
