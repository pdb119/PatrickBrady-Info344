using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawlCommon
{
    public class CrawlerStats : TableEntity
    {
        public CrawlerStats(string guid)
        {
            this.PartitionKey = "crawlerStats";
            this.RowKey = guid;
        }
        public CrawlerStats() { }
        public float cpu { get; set; }
        public float ram { get; set; }
        public int numberCrawled { get; set; }
        //lastCrawled
        public string status { get; set; }
    }
}
