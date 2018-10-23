using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace deOROMonitor
{
	internal class Program
	{
		public Program()
		{
		}

		public static void LogEvent(string message)
		{
			string str = "deOROService-Monitor";
			string str1 = "Application";
			try
			{
				EventLog eventLog = new EventLog();
				if (!EventLog.SourceExists(str))
				{
					EventLog.CreateEventSource(str, str1);
				}
				eventLog.Source = str;
				eventLog.Log = str1;
				eventLog.WriteEntry(message);
			}
			catch (Exception exception)
			{
			}
		}

		private static void Main(string[] args)
		{
			bool isNetworkAvailable = false;
			int num = 0;
			while (!isNetworkAvailable)
			{
				num++;
				if (num <= 10)
				{
					Program.LogEvent("Network interfaces down");
					Thread.Sleep(2000);
					isNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
				}
				else
				{
					break;
				}
			}
			string location = Assembly.GetExecutingAssembly().Location;
			location = location.Replace("deOROMonitor.exe", "");
			Thread.Sleep(60000);
			while (true)
			{
				try
				{
					Process[] processes = Process.GetProcesses();
					Process process = null;
					while (process == null)
					{
						processes = Process.GetProcesses();
						process = ((IEnumerable<Process>)processes).FirstOrDefault<Process>((Process o) => o.ProcessName.ToLower().Contains("tsmkioskassistant"));
						Thread.Sleep(1000);
					}
					if (((IEnumerable<Process>)processes).FirstOrDefault<Process>((Process o) => (!o.ProcessName.ToLower().Contains("deORO") ? false : !o.ProcessName.ToLower().Contains("tsmkioskassistant"))) == null)
					{
						if (File.Exists(string.Concat(location, "deORO.exe")))
						{
							Process.Start(string.Concat(location, "deORO.exe"));
						}
					}
				}
				catch (Exception exception)
				{
					Program.LogEvent(exception.Message);
				}
				Thread.Sleep(10000);
			}
		}
	}
}