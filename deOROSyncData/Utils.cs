using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp
{
    public class SyncTimeRet {
        public bool status;
        public string description;
    }

    public static class SyncTime
    {
        public static DateTime GetNetworkTime()
        {
            //throw new Exception("test"); // for testing
            const string ntpServer = "pool.ntp.org";
            var ntpData = new byte[48];
            ntpData[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                ReceiveTimeout = 2000
            };
            socket.Connect(ipEndPoint);
            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();

            ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
            ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
            var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

            return networkDateTime;
        }
        public static DateTime? GetSQLServerTime()
        {
            DateTime? server_utc_time = null;
            using (SqlConnection conn = new SqlConnection("data source=209.159.152.234;initial catalog=master;uid=sa;pwd=Polaris*~;"))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT SYSUTCDATETIME();", conn);
                    conn.Open();
                    server_utc_time = (DateTime)cmd.ExecuteScalar();
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                        conn.Close();
                }
            }
            return server_utc_time;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool SetLocalTime(ref SYSTEMTIME lpSystemTime);

        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;    // ignored for the SetLocalTime function
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        public static SyncTimeRet SetWindowsSystemTime()
        {
            DateTime? server_utc_time = null;
            SyncTimeRet ret = new SyncTimeRet() { status=false, description=string.Empty };
            try
            {
                server_utc_time = GetSQLServerTime();
                ret.status = true;
                ret.description = "Synchronized using Live SQL server";
            }
            catch (Exception eo)
            {
                try
                {
                    server_utc_time = GetNetworkTime();
                    ret.status = true;
                    ret.description = "Synchronized using pool.ntp.org";
                }
                catch (Exception e) {
                    ret.status = false;
                    ret.description = eo.StackTrace + "\n" + e.StackTrace;
                }
            }
            if (server_utc_time != null)
            {
                //Console.Out.WriteLine(((DateTime)server_utc_time).ToLocalTime());
                server_utc_time = ((DateTime)server_utc_time).ToLocalTime();
                SYSTEMTIME time = new SYSTEMTIME();
                time.wDay = (ushort)server_utc_time.Value.Day;
                time.wMonth = (ushort)server_utc_time.Value.Month;
                time.wYear = (ushort)server_utc_time.Value.Year;
                time.wHour = (ushort)server_utc_time.Value.Hour;
                time.wMinute = (ushort)server_utc_time.Value.Minute;
                time.wSecond = (ushort)server_utc_time.Value.Second;
                time.wMilliseconds = (ushort)server_utc_time.Value.Millisecond;

                ret.status = SetLocalTime(ref time);
                if (!ret.status)
                {
                    ret.description ="An unhandled exception of type 'System.ComponentModel.Win32Exception' occurred in application. A required privilege is not held by the client.";
                }
                return ret;
            }
            return ret;
        }
    }
}
