using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Lanymy.Common.Models
{
    public class IpInfoModel
    {

        public IPAddress IpAddress { get; set; }

        public string IpMacAddress { get; set; }

        public string HostName { get; set; }

        private string _Digest;
        public string Digest => _Digest ?? (_Digest = string.Concat(IpAddress.ToString().PadRight(15, ' '), IpMacAddress.PadRight(20, ' '), HostName));

    }
}
