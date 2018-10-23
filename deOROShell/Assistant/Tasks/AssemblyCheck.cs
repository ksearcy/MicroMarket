using RestSharp;
using StokedProtoBuf;
using StokedProtoBuf.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using TSMKioskAssistant;
using TSMKioskAssistant.Properties;
using TSMKioskAssistant.Utils;
using TsmMasterClient;
using TsmNPContracts;
using WinUpdateClient;
using WinUpdateClient.Models;

namespace deOROMonitor.Assistant.Tasks
{
    class AssemblyCheck
    {
        private bool isRunning;

        private DateTime nextUpdateCheck = DateTime.MinValue;

        private List<Timer> installTimers = new List<Timer>();
        public AssemblyCheck()
        {
        }



        private void WinUpdates()
        {
            if (DateTime.Now.Hour > 18 && DateTime.Now.Hour < 6 && DateTime.Now > this.nextUpdateCheck)
            {
                Utilities.LogEvent("Checking for windows updates");
                (new Thread(() =>
                {
                    try
                    {
                        Random random = new Random();
                        this.nextUpdateCheck = DateTime.Now.AddHours((double)random.Next(1, 12));
                        AutUserRequest autUserRequest = new AutUserRequest();
                        autUserRequest.set_Company(2);
                        autUserRequest.set_Location((uint)ServiceGlobals.Instance.idWarehouse);
                        autUserRequest.set_Username("updates@32market.com");
                        autUserRequest.set_Password("updates");
                        WinUpdateClient.Models.Station station = new WinUpdateClient.Models.Station();
                        station.set_CompanyStationId(ServiceGlobals.Instance.idStation);
                        station.set_HostName(ServiceGlobals.Instance.PCHostName);
                        station.set_idMarketClient(ServiceGlobals.Instance.marketClient.get_idMarketClient());
                        station.set_idCompany(2);
                        station.set_idWarehouse(ServiceGlobals.Instance.idWarehouse);
                        WinUpdateClient.Models.Station station1 = station;
                        WinUpdate winUpdate = new WinUpdate(autUserRequest, station1);
                        winUpdate.DisableAutomaticUpdates();
                        Result result = winUpdate.ReportAvailableUpdates().Result;
                        if (!result.get_Success())
                        {
                            Utilities.LogEvent(result.get_Error());
                        }
                        else
                        {
                            Utilities.LogEvent(string.Format("Reporting {0} updates", result.get_IntVal()));
                        }
                        List<StationUpdate> list = winUpdate.GetApprovedUpdates(station1.get_CompanyStationId(), station1.get_idWarehouse(), 2).Result.Where<StationUpdate>((StationUpdate o) =>
                        {
                            if (!o.get_UpdateWindow().HasValue)
                            {
                                return false;
                            }
                            return o.get_UpdateWindow().Value.ToLocalTime() < DateTime.Now;
                        }).ToList<StationUpdate>();
                        if (list.Count > 0)
                        {
                            List<Update> updates = new List<Update>();
                            foreach (StationUpdate stationUpdate in list)
                            {
                                updates.Add(stationUpdate);
                            }
                            List<Result> results = winUpdate.InstallUpdates(updates);
                            DateTime now = DateTime.Now;
                            bool flag = false;
                            List<InstallAttempt> installAttempts = new List<InstallAttempt>();
                            for (int i = 0; i < updates.Count; i++)
                            {
                                InstallAttempt installAttempt = new InstallAttempt();
                                installAttempt.set_DownloadDate(now);
                                installAttempt.set_ErrorMessage(results[i].get_Error());
                                installAttempt.set_idAvailableUpdate(updates[i].get_idAvailableUpdate());
                                installAttempt.set_idStation(list[i].get_idStation());
                                installAttempt.set_InstallDate(DateTime.Now);
                                installAttempt.set_IsSuccess(results[i].get_Success());
                                installAttempts.Add(installAttempt);
                                if (results[i].get_Success())
                                {
                                    flag = true;
                                }
                            }
                            winUpdate.ReportInstallResults(installAttempts).Wait();
                            if (flag)
                            {
                                Utilities.LogEvent("Rebooting after updates");
                                Process.Start("shutdown.exe", "-r -t 0");
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Exception innerException = exception;
                        Utilities.LogEvent(string.Concat("CheckWinUpdates: ", innerException.Message, " ", innerException.StackTrace));
                        while (innerException.InnerException != null)
                        {
                            innerException = innerException.InnerException;
                            Utilities.LogEvent(string.Concat("CheckWinUpdates: ", innerException.Message, " ", innerException.StackTrace));
                        }
                    }
                })
                {
                    IsBackground = true
                }).Start();
            }
        }
    }
}
