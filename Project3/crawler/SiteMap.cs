using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace crawler
{
    class SiteMap
    {
        public List<string> sitemaps { get; private set; }
        public SiteMap(string url)
        {
            WebRequest req = WebRequest.Create(url);
            WebResponse res = req.GetResponse();
            Stream remoteFileStream = res.GetResponseStream();           
            XmlDocument xr = new XmlDocument();
            xr.Load(remoteFileStream);
            foreach (XmlNode node in xr.SelectNodes("/sitemapindex/sitemap/loc"))
            {
                string sitemapUrl = node.FirstChild.Value;
                sitemaps.Add(sitemapUrl);
            }
            foreach (XmlNode node in xr.SelectNodes("/sitemapindex/sitemap/loc"))
            {
                string sitemapUrl = node.FirstChild.Value;
                sitemaps.Add(sitemapUrl);
            }
        }
    }
}
