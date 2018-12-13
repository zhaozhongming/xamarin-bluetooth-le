using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE.Client.Helpers
{
        public interface IAppVersion
        {
            string GetVersion();
            int GetBuild();
        }
}
