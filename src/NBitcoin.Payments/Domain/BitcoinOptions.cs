using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XyzBitcoin.Domain
{
    public class BitcoinOptions
    {
        public string OrderMasterExtPubKey { get; set; }
        public string RpcServerUrl { get; set; }
        public string RpcUser { get; set; }
        public string RpcPassword { get; set; }
    }
}
