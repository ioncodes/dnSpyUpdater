using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            WebClient client = new WebClient();
            Console.WriteLine("Connecting...");
            string jsonString = client.DownloadString("https://ci.appveyor.com/api/projects/0xd4d/dnspy/");
            Console.WriteLine("Getting jobId...");
            int pFrom = jsonString.IndexOf("\"jobId\":\"") + "\"jobId\":\"".Length;
            int pTo = jsonString.LastIndexOf("\",\"name\":");
            string jobId = jsonString.Substring(pFrom, pTo - pFrom);
            Console.WriteLine("JobId: "+ jobId);
            Console.WriteLine("Downloading File...");
            client.DownloadFile("https://ci.appveyor.com/api/buildjobs/"+jobId+"/artifacts/dnSpy/bin/dnSpy.zip", "dnSpy.zip");
            Console.WriteLine("Downloaded!");
            Console.Read();
        }
    }
}
