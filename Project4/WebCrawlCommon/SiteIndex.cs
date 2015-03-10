using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawlCommon
{
    public class SiteIndex : TableEntity
    {

        public SiteIndex() { }
        public SiteIndex(string host, string url, string title)
        {
            this.PartitionKey = Methods.tableSafe(title).ToLower();
            this.RowKey = Methods.tableSafe(host + url);
            this.fullUrl = host+url;
        }

        public string title { get; set; }
        public string fullUrl { get; set; }

    }
}
