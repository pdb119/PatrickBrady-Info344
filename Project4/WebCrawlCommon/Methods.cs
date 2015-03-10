using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawlCommon
{
    public static class Methods
    {
        public static string tableSafe(string s){
            s = s.Replace("/","_");
            s = s.Replace("\\","_");
            s = s.Replace("#","_");
            s = s.Replace("?","_");
            return s;
        }
    }
}
