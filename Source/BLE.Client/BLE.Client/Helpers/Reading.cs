using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE.Client.Helpers
{
    public class Reading : TableEntity
    {
        public DateTime ReadingTime { get; set; }

        public string ReadingValue { get; set; }
    }
}
