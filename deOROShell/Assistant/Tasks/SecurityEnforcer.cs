using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Threading;


namespace deOROMonitor.Tasks
{
    internal class SecurityEnforcer : TsmTask
    {
        private bool isRunning;

        private static string[] serviceList;

        private static string[] processList;

        static SecurityEnforcer()
        {
            SecurityEnforcer.serviceList = new string[] { "Application Experience", "Diagnostic Policy Service", "Distributed Link Tracking Client", "IP Helper", "Offline Files", "Portable Device Enumerator Service", "Protected Storage", "Remote Registry", "Secondary Logon", "Security Center", "TCP/IP NetBIOS Helper", "Windows Error Reporting Service", "Windows Media Center Service Launcher", "Windows Search", "Windows Time", "Fax", "Bluetooth Support Service", "Remote Desktop Configuration", "Remote Desktop Services", "Media Center Extender Service", "Net.Tcp Port Sharing Service", "Remote Desktop Services UserMode Port Redirector", "Routing and Remote Access", "SeaPort", "SSDP Discovery", "LogMeIn", "keyboard" };
            SecurityEnforcer.processList = new string[] { "LogMeIn", "lmi", "ra", "lmisystray", "ragui", "lmitoolkit", "LogMeInRC", "On-Screen Keyboard", "tabtip", "ehshell", "wmplayer", "notepad", "helppane", "spotify", "iexplore", "chrome", "firefox", "opera" };
        }

        public SecurityEnforcer()
        {
        }

        private bool IsProcessBlacklisted(string name)
        {
            bool flag = false;
            string[] strArrays = SecurityEnforcer.processList;
            int num = 0;
            while (num < (int)strArrays.Length)
            {
                if (!strArrays[num].ToLower().Equals(name.ToLower()))
                {
                    num++;
                }
                else
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        public override bool IsRunning()
        {
            return this.isRunning;
        }

        private bool IsServiceBlacklisted(string name)
        {
            bool flag = false;
            string[] strArrays = SecurityEnforcer.serviceList;
            int num = 0;
            while (num < (int)strArrays.Length)
            {
                string lower = strArrays[num].ToLower();
                if (name.ToLower().Contains(lower) || lower.Equals(name.ToLower()))
                {
                    flag = true;
                    break;
                }
                else
                {
                    num++;
                }
            }
            return flag;
        }

        public override void Start()
        {
            this.isRunning = true;
            (new Thread((object fn) => {
                while (this.isRunning)
                {
                    try
                    {
                        Firewall firewall = new Firewall();
                        if (!firewall.IsEnabled())
                        {
                            firewall.Enable();
                        }
                        if (!firewall.IsInboundBlocked())
                        {
                            firewall.BlockInbound();
                        }
                        Process[] processes = Process.GetProcesses();
                        for (int i = 0; i < (int)processes.Length; i++)
                        {
                            Process process = processes[i];
                            if (this.IsProcessBlacklisted(process.ProcessName.ToLower()))
                            {
                                process.Kill();
                            }
                        }
                        foreach (ServiceController list in ServiceController.GetServices().ToList<ServiceController>())
                        {
                            if (!this.IsServiceBlacklisted(list.DisplayName))
                            {
                                continue;
                            }
                            try
                            {
                                list.Stop();
                                list.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 2, 0));
                                Process.Start("sc.exe", string.Format("config {0} start= disabled", list.ServiceName));
                            }
                            catch (Exception exception)
                            {
                            }
                        }
                        Utilities.LogEvent("Security enforcer completed audit");
                    }
                    catch (Exception exception2)
                    {
                        Exception exception1 = exception2;
                        Utilities.LogEvent(string.Concat("SecurityEnforcer: ", exception1.Message, " ", exception1.StackTrace));
                    }
                    Thread.Sleep(3600000);
                }
            })
            {
                IsBackground = true
            }).Start();
        }

        public override void Stop()
        {
            this.isRunning = false;
        }
    }
}
