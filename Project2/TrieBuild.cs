using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Web.Services;
using System.Net;
using System.Diagnostics;

namespace server1
{
    public static class TrieBuild
    {
        private const long MAX_GB_RAM = 11;
        public static string buildTrie()
        {
            long maxRam = MAX_GB_RAM * 1024 * 1024 * 1024;
            TrieNode root;
            //StreamReader sr = new StreamReader("titles.txt.cleared.txt");
            LocalResource store = RoleEnvironment.GetLocalResource("wikiStore");
            FileInfo localFile = new FileInfo(store.RootPath + "\\titles.txt");
            Stream localFileStream = localFile.Create();
            WebRequest req = WebRequest.Create("http://store1.pb119.net/pub/titles.txt");
            WebResponse res = req.GetResponse();
            Stream remoteFileStream = res.GetResponseStream();
            remoteFileStream.CopyTo(localFileStream);
            remoteFileStream.Close();
            res.Close();
            localFileStream.Close();
            StreamReader sr = new StreamReader(store.RootPath + "\\titles.txt");
            //StreamReader sr = new StreamReader("test.txt");
            //StreamReader sr2 = new StreamReader("test2.txt");
            root = new TrieNode((char)0);
            int counter = 0;
            while (!sr.EndOfStream && counter >= 0)
            {
                counter++;
                string line = sr.ReadLine();
                string originalLine = line;
                line = line.ToLower();
                char[] chars = line.ToCharArray();
                //Console.WriteLine(line);
                TrieNode last = root;
                for (int i = 0; i < chars.Length; i++)
                {
                    /*
                    if (line.Substring(0, 1).ToCharArray()[0] > 'd')
                    {

                    }
                     */
                    if (last.branches.Keys.Contains<char>(chars[i]))
                    {
                        TrieNode temp = last.branches[chars[i]];
                        last = temp;
                    }
                    else
                    {
                        TrieNode temp = new TrieNode(chars[i]);
                        last.branches.Add(chars[i], temp);
                        last = temp;
                    }
                    if (line.Substring(0, i + 1) == originalLine.ToLower())
                    {
                        last.word = originalLine;
                    }
                }

                if (counter % 100000 == 0)
                {
                    Process p = System.Diagnostics.Process.GetCurrentProcess();
                    Console.WriteLine(p.WorkingSet64);
                    if (p.WorkingSet64 > maxRam)
                    {
                        counter = -1;
                    }
                }

            }
            //Console.WriteLine("done.");
            //Console.ReadLine();
            HttpApplicationState appState = HttpContext.Current.Application;
            appState["trieRoot"] = root;
            Process pro = System.Diagnostics.Process.GetCurrentProcess();
            return "done. Using "+pro.WorkingSet64 / 1024 / 1024 / 1024+"GB of ram";
        }
    }
}