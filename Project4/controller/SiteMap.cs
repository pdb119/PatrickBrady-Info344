using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace controller
{
    class SiteMap
    {
        public List<string> sitemaps { get; private set; }
        public List<string> pages { get; private set; }
        public SiteMap(string url)
        {
            WebRequest req = WebRequest.Create(url);
            WebResponse res = req.GetResponse();
            Stream remoteFileStream = res.GetResponseStream();
            XmlDocument xr = new XmlDocument();
            xr.Load(remoteFileStream);
            sitemaps = new List<string>();
            pages = new List<string>();
            //XmlNamespaceManager ns = new XmlNamespaceManager(results.NameTable);
            //ns.AddNamespace("ns","http://halo.com/schemas/custom/users/GetUser_V1");
            //XmlNode sitemapindex = xr.SelectSingleNode("sitemapindex");
            //if (sitemapindex != null)
            //{            
            if (xr.GetElementsByTagName("sitemap").Count > 0)
            {
                foreach (XmlNode node in xr.GetElementsByTagName("loc"))
                {
                    string sitemapUrl = node.InnerText;
                    sitemaps.Add(sitemapUrl);
                }
            }
            else if (xr.GetElementsByTagName("url").Count > 0)
            //}
            //XmlNode urlset = xr.SelectSingleNode("urlset");
            //if (urlset != null)
            //{
                foreach (XmlNode node in xr.GetElementsByTagName("loc"))
                {
                    string sitemapUrl = node.InnerText;
                    pages.Add(sitemapUrl);
                }
            //}
        }
    }
}
