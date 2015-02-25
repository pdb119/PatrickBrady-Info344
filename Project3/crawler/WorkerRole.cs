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
using controller;

namespace crawler
{
    public class WorkerRole : RoleEntryPoint
    {
        public HashSet<string> alreadyDone;
        //int locks on already done lock
        public int numberCrawled;
        public Dictionary<string, string[]> disallowedTemp = new Dictionary<string, string[]>();
        private object alreadyDoneLock = new Object();
        private object disallowedTempLock = new Object();
        //public 

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        public override void Run()
        {
            //maybe not here
            alreadyDone = new HashSet<string>();
            numberCrawled = 0;
            //while (true)
            //{
            //Robots robots = new Robots("http://cnn.com/robots.txt");          
            CloudStorageAccount storeAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient tableClient = storeAccount.CreateCloudTableClient();
            CloudTable controlTable = tableClient.GetTableReference("control");
            Thread[] threads = new Thread[2];

            Guid sysid = Guid.NewGuid();
            while (true)
            {
                for (int i = 0; i < threads.Length; i++)
                {
                    if (threads[i] == null || threads[i].ThreadState == System.Threading.ThreadState.Stopped)
                    {
                        threads[i] = new Thread(new ParameterizedThreadStart(processThread));
                        string iasstring = i.ToString();
                        threads[i].Start(iasstring);
                        Trace.TraceInformation("thread started");
                    }
                }
                PerformanceCounter cpuCounter;
                PerformanceCounter ramCounter;
                float cpu = 0f;
                try
                {
                    cpuCounter = new PerformanceCounter();
                    cpuCounter.CategoryName = "Processor";
                    cpuCounter.CounterName = "% Processor Time";
                    cpuCounter.InstanceName = "_Total";
                    cpu = cpuCounter.NextValue();
                }
                catch (Exception e)
                {

                }
                float ram = 0f;
                try
                {


                    ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                    ram = ramCounter.NextValue();
                }
                catch (Exception e)
                {

                }
                CrawlerStats cs = new CrawlerStats(sysid.ToString());
                cs.ram = ram;
                cs.cpu = cpu;
                cs.numberCrawled = numberCrawled;
                cs.status = "Probably Crawling";
                TableOperation updateStats = TableOperation.InsertOrReplace(cs);
                controlTable.Execute(updateStats);                
                Thread.Sleep(10000);
            }
        }
        public bool threadsRunning(Thread[] threads)
        {
            for (int i = 0; i < threads.Length; i++)
            {
                if (!(threads[i] == null || threads[i].ThreadState == System.Threading.ThreadState.Stopped))
                {
                    return false;
                }
            }
            return true;
        }
        public void processThread(object threadNumString)
        {
            int threadNum = int.Parse((string)threadNumString);
            Random r = new Random();
            int threadTime = r.Next(20000) + 10000;
            CloudStorageAccount storeAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient queueClient = storeAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("sitequeue");
            CloudTableClient tableClient = storeAccount.CreateCloudTableClient();
            CloudTable siteTable = tableClient.GetTableReference("sites");
            int sleepIterations = 0;
            while (true)
            {
                CloudQueueMessage qmessage = queue.GetMessage(TimeSpan.FromMinutes(20));
                if (qmessage == null)
                {
                    sleepIterations += 1;
                    Thread.Sleep(threadTime);
                }
                else
                {
                    //string[] qmessageParts = qmessage.AsString.Split(' ');
                    //string type = qmessageParts[0];
                    queue.DeleteMessage(qmessage);
                    Uri url = new Uri(qmessage.AsString);
                    bool alreadyProcessed;
                    //maybe lock not needed here?
                    lock (alreadyDoneLock)
                    {
                        alreadyProcessed = alreadyDone.Contains(url.AbsoluteUri);
                    }
                    if (!alreadyProcessed)
                    {
                        lock (disallowedTempLock)
                        {
                            if (!disallowedTemp.Keys.Contains(url.Host))
                            {
                                downloadDisallowed(tableClient, url.Host);
                            }
                        }
                        PageReader page = new PageReader(url.AbsoluteUri);
                        SiteIndex si = new SiteIndex(url.Host, url.AbsolutePath, page.title);
                        TableOperation insertPage = TableOperation.InsertOrReplace(si);
                        siteTable.Execute(insertPage);
                        foreach (string nextUrl in page.nextPages)
                        {
                            bool allowed = true;
                            Uri nextUrlUri = null;
                            //use the url try and fix relative problem
                            if (Uri.TryCreate(nextUrl, UriKind.RelativeOrAbsolute, out nextUrlUri))
                            {
                                //figure this out
                                if (!nextUrlUri.IsAbsoluteUri)
                                {

                                    string temp = url.Scheme + "://" + url.DnsSafeHost;
                                    if (url.Segments[url.Segments.Length - 1].Contains('.'))
                                    {
                                        for (int i = 0; i < url.Segments.Length - 1; i++)
                                        {
                                            temp += url.Segments[i];
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < url.Segments.Length; i++)
                                        {
                                            temp += url.Segments[i];
                                        }
                                    }
                                    if (nextUrl.StartsWith("/"))
                                    {
                                        temp += nextUrl.Substring(1);
                                    }
                                    else
                                    {
                                        temp += nextUrl;
                                    }
                                    if (!Uri.TryCreate(temp, UriKind.Absolute, out nextUrlUri))
                                    {
                                        allowed = false;
                                    }
                                }
                            }
                            else
                            {
                                allowed = false;
                            }
                            if (allowed)
                            {
                                if (nextUrlUri.Host != url.Host)
                                {
                                    allowed = false;
                                }
                            }

                            if (allowed)
                            {
                                foreach (string s in disallowedTemp[url.Host])
                                {
                                    //wrong
                                    Uri disallowedUrl = new Uri(s);
                                    if (disallowedUrl.IsBaseOf(nextUrlUri))
                                    {
                                        allowed = false;
                                    }
                                }
                            }

                            if (allowed)
                            {
                                queue.AddMessage(new CloudQueueMessage(nextUrlUri.AbsoluteUri));
                                lock (alreadyDoneLock)
                                {
                                    alreadyDone.Add(url.AbsoluteUri);
                                    numberCrawled++;
                                }
                            }

                        }
                    }
                    //queue.DeleteMessage(qmessage);
                }
            }
        }

        private void downloadDisallowed(CloudTableClient cli, string host)
        {
            CloudTable disallowed = cli.GetTableReference("control");
            TableQuery<Disallowed> getDisallowed = new TableQuery<Disallowed>();
            getDisallowed.Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "disallowedStore"), TableOperators.And, TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, host)));
            bool foundTable = false;
            foreach (Disallowed d in disallowed.ExecuteQuery(getDisallowed))
            {
                foundTable = true;
                disallowedTemp.Add(host, d.sites.Split(' '));
            }
            if (!foundTable)
            {
                throw new Exception();
            }
        }
        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("crawler has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("crawler is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("crawler has stopped");
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
