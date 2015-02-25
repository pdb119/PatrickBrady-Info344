using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Table;

namespace controller
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        public HashSet<string> alreadyDone;
        public override void Run()
        {
            CloudStorageAccount storeAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient queueClient = storeAccount.CreateCloudQueueClient();
            CloudQueue siteQueue = queueClient.GetQueueReference("sitequeue");
            CloudQueue robotQueue = queueClient.GetQueueReference("robotqueue");
            CloudTableClient tableClient = storeAccount.CreateCloudTableClient();
            CloudTable control = tableClient.GetTableReference("control");
            while (true)
            {
                CloudQueueMessage robotFile = robotQueue.GetMessage(TimeSpan.FromMinutes(20));                
                if (robotFile != null)
                {
                    robotQueue.DeleteMessage(robotFile);
                    Uri url = new Uri(robotFile.AsString);
                    alreadyDone = new HashSet<string>();
                    Robots parsedRobotFile = new Robots(url.AbsoluteUri);                    
                    //string[] disallowed = parsedRobotFile.disallowed.ToArray();
                    string disallowed = "";
                    foreach (string s in parsedRobotFile.disallowed)
                    {
                        disallowed += s + " ";
                    }
                    disallowed = disallowed.TrimEnd();
                    TableOperation insertDisallowed = TableOperation.InsertOrReplace(new Disallowed(url.Host, disallowed));
                    control.Execute(insertDisallowed);
                    foreach (string siteMapUrl in parsedRobotFile.sites)
                    {
                        //check disallow
                        //need to handle urls
                        siteMapRecurse(siteMapUrl,siteQueue,url);                        
                    }
                }
                else
                {
                    Thread.Sleep(20000);
                }
            }

        }
        private void siteMapRecurse(string siteMapUrl,CloudQueue siteQueue,Uri robotUrl)
        {
            SiteMap sitemap = new SiteMap(siteMapUrl);
            foreach (string nextSiteMapUrl in sitemap.sitemaps)
            {
                siteMapRecurse(nextSiteMapUrl,siteQueue,robotUrl);
            }
            foreach (string pageUrl in sitemap.pages)
            {
                //check already done then...
                Uri url = new Uri(pageUrl);
                if (!alreadyDone.Contains(url.AbsoluteUri) && url.DnsSafeHost == robotUrl.DnsSafeHost)
                {
                    siteQueue.AddMessage(new CloudQueueMessage(url.AbsoluteUri));
                    alreadyDone.Add(url.AbsoluteUri);
                }
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("controller has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("controller is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("controller has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
}
