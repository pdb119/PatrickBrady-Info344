using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace controller
{
    class Robots
    {
        //private string[] disallowed;
        public List<string> disallowed { get; private set; }
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
                    Uri uri = new Uri(line.Trim().Substring(8).Trim());
                    sites.Add(uri.AbsoluteUri);
                    //sites.Add(line);
                }
                else if (line.StartsWith("Disallow:"))
                {
                    Uri uri;
                    Uri robotsUri = new Uri(url);
                    if (Uri.TryCreate(line.Trim().Substring(9).Trim(), UriKind.RelativeOrAbsolute, out uri))
                    {
                        if (!uri.IsAbsoluteUri)
                        {
                            string temp = robotsUri.Scheme + "://" + robotsUri.DnsSafeHost;
                            if (robotsUri.Segments[robotsUri.Segments.Length-1].Contains('.'))
                            {
                                for (int i = 0; i < robotsUri.Segments.Length - 1; i++)
                                {
                                    temp += robotsUri.Segments[i];
                                }
                            }
                            else
                            {
                                for (int i = 0; i < robotsUri.Segments.Length; i++)
                                {
                                    temp += robotsUri.Segments[i];
                                }
                            }
                            if (line.Trim().Substring(9).Trim().StartsWith("/"))
                            {
                                temp += line.Trim().Substring(9).Trim().Substring(1);
                            }
                            else
                            {
                                temp += line.Trim().Substring(9).Trim();
                            }
                            if (temp.Substring(temp.Length - 1) != "/") { temp += "/"; }
                            if (Uri.TryCreate(temp, UriKind.Absolute, out uri))
                            {
                                disallowed.Add(uri.AbsoluteUri);
                            }
                        }
                    }                                   
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
