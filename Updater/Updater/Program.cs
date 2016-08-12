using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace Updater
{
    class Program
    {
        static string _path = "";
        static void Main(string[] args)
        {
            _path = AppDomain.CurrentDomain.BaseDirectory;
            if (File.Exists(_path + "dnSpy.exe"))
            {
                int serverBuildId = 0;
                int clientBuildId = 0;
                int pTo = 0;
                int pFrom = 0;
                WebClient client = new WebClient();
                Console.WriteLine("Connecting...");
                string jsonString = client.DownloadString("https://ci.appveyor.com/api/projects/0xd4d/dnspy/");
                
                pFrom = jsonString.IndexOf("\"buildId\":") + "\"buildId\":".Length;
                pTo = jsonString.LastIndexOf(",\"jobs\":[");
                serverBuildId = Convert.ToInt32(jsonString.Substring(pFrom, pTo - pFrom));

                if (File.Exists("buildid.txt"))
                {
                    clientBuildId = Convert.ToInt32(File.ReadAllText("buildid.txt"));
                }
                else
                {
                    Console.WriteLine("First run! Saving buildId...");
                    File.WriteAllText("buildid.txt", serverBuildId.ToString());
                    Console.WriteLine("buildId: " + serverBuildId);
                    Console.Read();
                    return;
                }

                if (serverBuildId > clientBuildId)
                {
                    Console.WriteLine("Getting jobId...");
                    pFrom = jsonString.IndexOf("\"jobId\":\"") + "\"jobId\":\"".Length;
                    pTo = jsonString.LastIndexOf("\",\"name\":");
                    string jobId = jsonString.Substring(pFrom, pTo - pFrom);
                    Console.WriteLine("JobId: " + jobId);
                    Console.WriteLine("Downloading File...");
                    client.DownloadFile("https://ci.appveyor.com/api/buildjobs/" + jobId + "/artifacts/dnSpy/bin/dnSpy.zip", "dnSpy.zip");
                    Console.WriteLine("Downloaded!");
                    DirectoryInfo d = new DirectoryInfo(_path);

                    if (!Directory.Exists("Backup"))
                    {
                        Directory.CreateDirectory("Backup");
                    }

                    Console.WriteLine("Moving Files...");
                    foreach (var file in d.GetFiles())
                    {
                        if (!file.Name.Contains("dnSpy.zip") && !file.Name.Contains("Updater.exe") && !file.Name.Contains("backup.zip"))
                        {
                            Directory.Move(file.FullName, _path + "\\Backup\\" + file.Name);
                        }
                    }

                    Console.WriteLine("Moving Directories...");
                    foreach (var dir in d.GetDirectories())
                    {
                        if (!dir.Name.Contains("Backup"))
                        {
                            Directory.Move(dir.FullName, _path + "\\Backup\\" + dir.Name);
                        }
                    }

                    Console.WriteLine("Extracting dnSpy.zip");
                    ZipFile.ExtractToDirectory("dnSpy.zip", _path);

                    Console.WriteLine("Packing backup");
                    ZipFile.CreateFromDirectory("Backup", _path + "backup.zip");

                    Console.WriteLine("Deleting directory...");
                    Directory.Delete("Backup", true);

                    Console.WriteLine("Done!");
                }
                else
                {
                    Console.WriteLine("No update found!");
                }
            }
            else
            {
                Console.WriteLine("Couldn't find dnSpy.exe");
            }
            Console.Read();
        }
    }
}
