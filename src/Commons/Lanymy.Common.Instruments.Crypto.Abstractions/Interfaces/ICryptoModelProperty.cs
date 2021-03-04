using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.Interfaces
{
    public interface ICryptoModelProperty<T>
        where T : class
    {
        string ModelTypeName { get; set; }
        string ModelTypeFullName { get; set; }
        T SourceModel { get; set; }

    }
}