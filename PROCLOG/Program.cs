using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.IO;

namespace PROCLOG
{
    class Program
    {
        static void Main(string[] args)
        {
            
            int n = 0;
            //make it loop forever
            while (n == 0)
            { RunManagementEventWatcherForWindowsProcess(); }
            
        }

        //register for start events
        private static void RunManagementEventWatcherForWindowsProcess()
        {
            WqlEventQuery processQuery = new WqlEventQuery("__InstanceCreationEvent", new TimeSpan(0, 0, 2), "targetinstance isa 'Win32_Process'");
            ManagementEventWatcher processWatcher = new ManagementEventWatcher(processQuery);
            processWatcher.Options.Timeout = new TimeSpan(1, 0, 0);
            
            Console.WriteLine("");
            ManagementBaseObject nextEvent = processWatcher.WaitForNextEvent();
            ManagementBaseObject targetInstance = ((ManagementBaseObject)nextEvent["targetinstance"]);
            PropertyDataCollection props = targetInstance.Properties;
            foreach (PropertyData prop in props)
            {
                if (prop.Name == "ExecutablePath")
                {
                    Console.WriteLine("Process started: {0}", prop.Value);
                    using (StreamWriter w = File.AppendText(@"C:\temp\PROCLOG.log"))
                    {
                        Log("Process started: " + prop.Value, w);
                    }
                }
            }
            processWatcher.Stop();
        }
        //logging
        public static void Log(string logMessage, TextWriter w)
        {
            w.WriteLine("{0}: {1}: {2}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), logMessage);
        }
    }
}
