using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Net;

namespace dashboard
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            try
            {
                WebRequest req = WebRequest.Create("http://bradypat.cloudapp.net/dash.asmx/buildTrie");
                //WebRequest req = WebRequest.Create("http://localhost:50196/serv.asmx/buildTrie");
                //WebRequest req = WebRequest.Create(new Uri("index.html", UriKind.Relative));
                WebResponse res = req.GetResponse();
                res.Close();
            }
            catch (WebException e)
            {

            }
            return base.OnStart();
        }
    }
}
