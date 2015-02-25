using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace controller
{
    public class Disallowed : TableEntity
    {
        public Disallowed(string host, string sites)
        {
            this.PartitionKey = "disallowedStore";
            this.RowKey = host;
            this.sites = sites;
        }
        public Disallowed() { }
        public string sites {get; set;}
    }
}
