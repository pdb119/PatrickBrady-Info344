using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace crawler
{
    class Robots
    {
        //private string[] disallowed;
        private List<string> disallowed;
        public List<string> sites { get; private set; }
        public Robots(string url)
        {
            WebRequest req = WebRequest.Create(url);
            WebResponse res = req.GetResponse();
            Stream remoteFileStream = res.GetResponseStream();
            StreamReader fr = new StreamReader(remoteFileStream);
            //List<string> disallowTemp = new List<string>();
            sites = new List<string>();
            disallowed = new List<string>();
            while (!fr.EndOfStream)
            {
                string line = fr.ReadLine();
                if (line.StartsWith("Sitemap:"))
                {
                    //sites.Add(line.Split(':')[1].Trim());
                    sites.Add(line);
                }
                else if (line.StartsWith("Disallow:"))
                {
                    //disallowed.Add(line.Split(':')[1].Trim());
                    disallowed.Add(line);
                }
            }
            fr.Close();
            remoteFileStream.Close();
            res.Close();
        }
        public bool isAllowed(string site){
            foreach(String s in disallowed){
                if (s.Equals(site))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
