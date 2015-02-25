using HtmlAgilityPack;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace crawler
{
    class PageReader
    {
        public string title { get; private set; }
        public DateTime date { get; private set; }
        public List<string> nextPages { get; private set; }
        public Uri pageUrl;

        public PageReader(string url)
        {
            nextPages = new List<string>();
            WebRequest req = WebRequest.Create(url);
            WebResponse res = req.GetResponse();
            pageUrl = res.ResponseUri;
            Stream remoteFileStream = res.GetResponseStream();
            StreamReader sr = new StreamReader(remoteFileStream);
            HtmlDocument page = new HtmlDocument();
            page.LoadHtml(sr.ReadToEnd());
            foreach(HtmlNode titleNode in page.DocumentNode.Descendants("title")){
                title = titleNode.InnerText;
            }
            foreach (HtmlNode linkNode in page.DocumentNode.Descendants("a"))
            {
                if (linkNode.Attributes.Contains("href"))
                {
                    nextPages.Add(linkNode.Attributes["href"].Value);
                }
            }
            /*
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (line.Contains("<title>"))
                {
                    title = line.Substring(line.IndexOf("<title>") + 7, line.IndexOf("</title>") - line.IndexOf("<title>"));
                }
                else if (line.Contains("<a"))
                {

                }
            }
             */
        }
    }
}
