using controller;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using crawler;

namespace dashboard
{
    /// <summary>
    /// Summary description for dash
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class dash : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public void addSite(string url)
        {
            CloudStorageAccount storeAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient queueClient = storeAccount.CreateCloudQueueClient();            
            CloudQueue robotQueue = queueClient.GetQueueReference("robotqueue");
            robotQueue.AddMessage(new CloudQueueMessage(url));
        }
    [WebMethod]
        public string getSites()
        {
            List<string> l = new List<string>();
            CloudStorageAccount storeAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient tableClient = storeAccount.CreateCloudTableClient();
            CloudTable siteTable = tableClient.GetTableReference("sites");
            TableQuery<SiteIndex> getDisallowed = new TableQuery<SiteIndex>();
            foreach(SiteIndex s in siteTable.ExecuteQuery(getDisallowed)){
                l.Add(s.title+": "+s.PartitionKey+s.fullUrl);
            }
            return "{\"results\":" + new JavaScriptSerializer().Serialize(l.ToArray()) + "}";            
        }

        [WebMethod]
        public void clearTables(){
            CloudStorageAccount storeAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient tableClient = storeAccount.CreateCloudTableClient();
            CloudTable siteTable = tableClient.GetTableReference("sites");           
        }
        [WebMethod]
        public string getCrawlerStats()
        {
            CloudStorageAccount storeAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient tableClient = storeAccount.CreateCloudTableClient();
            CloudTable siteTable = tableClient.GetTableReference("control");
            CloudQueueClient queueClient = storeAccount.CreateCloudQueueClient();         
            TableQuery<CrawlerStats> getStats = new TableQuery<CrawlerStats>();
            getStats.Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "crawlerStats"));
            IEnumerable<CrawlerStats> data = siteTable.ExecuteQuery(getStats);
            string jsonList = "";
            foreach (CrawlerStats s in data)
            {
                string entry = "[{";
                entry += "\"id\":\"" + s.RowKey + "\"";
                entry += ",\"cpu\":" + s.cpu;
                entry += ",\"ram\":" + s.ram;
                entry += ",\"num\":" + s.numberCrawled;
                entry += ",\"status\":" + s.status + "\"";
                entry += "}]";
                jsonList += entry;

            }
            CloudQueue robotQueue = queueClient.GetQueueReference("robotqueue");
            CloudQueue siteQueue = queueClient.GetQueueReference("sitequeue");
            robotQueue.FetchAttributes();
            siteQueue.FetchAttributes();
            return "{\"results\":{\"robotQueue\":" + robotQueue.ApproximateMessageCount + ",\"siteQueue\":" + siteQueue.ApproximateMessageCount + ",\"crawlers\":" + jsonList + "}";

        }
    }
}
