using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Samba
{
    public class SambaConnection
    {
        public IPAddress IP { get; init; }
        public string Domain { get; init; }
        public string Name { get; init; }
        public string Password { get; init; }

    }
}
