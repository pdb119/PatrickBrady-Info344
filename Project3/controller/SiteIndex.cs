using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace controller
{
    public class SiteIndex : TableEntity
    {

        public SiteIndex(){}
        public SiteIndex (string host, string url, string title)
        {
            this.PartitionKey = host;
            this.RowKey = url.Replace('/','_');
            this.title = title;
            this.fullUrl = url;
        }

        public string title { get; set; }
        public string fullUrl { get; set; }

    }
}
