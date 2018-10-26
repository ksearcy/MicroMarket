using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using JsonFormatterPlus;
using RestSharp;
using RestSharp.Authenticators;
using Couchbase;
using Couchbase.Configuration;
using System.Net.Http;
using Couchbase.Configuration.Client;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SQLite;




namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string SQLiteDBConection = ConfigurationManager.ConnectionStrings["SQLiteDBConection"].ConnectionString;
        string FMDFilesPath = ConfigurationManager.ConnectionStrings["FMDFilesPath"].ConnectionString;

        private void button1_Click(object sender, EventArgs e)
        {

            SaveJSonToSQL(textBox1.Text, textBox2.Text);

        }

        private void SaveJSonToSQL(string json, string connectionString)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            RootObject theObject = JsonConvert.DeserializeObject<RootObject>(json, settings);

            if ((theObject == null) || (theObject.rows == null))
                return;

            using (SqlConnection sqlConnection1 = new SqlConnection(connectionString))
            {
                String query = "INSERT INTO [user] ([pkid],[username],[first_name],[last_name],[email] ," +
                "[password] ,[is_staff] ,[is_active] ,[is_superuser] ,[applicationname] ," +
                "[is_approved] ,[is_online] ,[last_updated_on] ,[lastlockedoutdate] ,[enrolled_fmd1] ," +
                "[finger_id1] ,[enrolled_fmd2] ,[finger_id2],[enrolled_fmd3] ,[finger_id3]," +
                "[enrolled_fmd4],[finger_id4],[lastfmdchangeddate] ,[account_balance] ," +
                "[lastaccountbalancechangeddate] ,[barcode], [salt], [payrollbalance])" +
                "VALUES(@pkid,@username,@first_name,@last_name,@email ,@password ,@is_staff ," +
                "@is_active ,@is_superuser ,@applicationname ,@is_approved ,@is_online ," +
                "@last_updated_on ,@lastlockedoutdate ,@enrolled_fmd1 ,@finger_id1 ,@enrolled_fmd2 ," +
                "@finger_id2,@enrolled_fmd3 ,@finger_id3,@enrolled_fmd4,@finger_id4," +
                "@lastfmdchangeddate ,@account_balance ,@lastaccountbalancechangeddate ,@tags, @salt, @payroll_balance)";

                sqlConnection1.Open();
                using (SqlCommand command = new SqlCommand(query, sqlConnection1))
                {

                    foreach (Row row in theObject.rows)
                    {
                        if (row.value == null)
                            continue;
                        command.Parameters.AddWithValue("@pkid", Guid.NewGuid());
                        command.Parameters.AddWithValue("@username", ((object)row.value.user.user.idUser ?? DBNull.Value));
                        command.Parameters.AddWithValue("@first_name", ((object)row.value.user.user.firstName ?? DBNull.Value));
                        command.Parameters.AddWithValue("@last_name", ((object)row.value.user.user.lastName ?? DBNull.Value));
                        if ((row.value.user.credentials != null) && (row.value.user.credentials.Count > 0))
                        {
                            command.Parameters.AddWithValue("@email", ((object)row.value.user.credentials[0].email ?? DBNull.Value));
                            command.Parameters.AddWithValue("@password", ((object)row.value.user.credentials[0].password ?? DBNull.Value));
                            command.Parameters.AddWithValue("@is_active", ((object)row.value.user.credentials[0].isEnabled ?? DBNull.Value));
                            command.Parameters.AddWithValue("@salt", ((object)row.value.user.credentials[0].salt ?? DBNull.Value));
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@email", DBNull.Value);
                            command.Parameters.AddWithValue("@password", DBNull.Value);
                            command.Parameters.AddWithValue("@is_active", DBNull.Value);
                            command.Parameters.AddWithValue("@salt", DBNull.Value);
                        }

                        command.Parameters.AddWithValue("@is_staff", ((object)row.value.user.user.isEmployee ?? DBNull.Value));
                        command.Parameters.AddWithValue("@is_superuser", 0);
                        command.Parameters.AddWithValue("@applicationname", 32);
                        command.Parameters.AddWithValue("@is_approved", 1);
                        command.Parameters.AddWithValue("@is_online", 0);
                        command.Parameters.AddWithValue("@last_updated_on", ((object)row.value.user.user.lastActivity ?? DBNull.Value));
                        command.Parameters.AddWithValue("@lastlockedoutdate", 0);
                        command.Parameters.AddWithValue("@finger_id1", 0);
                        if ((row.value.fingerPrints != null) && (row.value.fingerPrints.Count >= 1))
                            command.Parameters.AddWithValue("@enrolled_fmd1", row.value.fingerPrints[0].data);
                        else
                            command.Parameters.AddWithValue("@enrolled_fmd1", DBNull.Value);
                        command.Parameters.AddWithValue("@finger_id2", 0);
                        if ((row.value.fingerPrints != null) && (row.value.fingerPrints.Count >= 2))
                            command.Parameters.AddWithValue("@enrolled_fmd2", row.value.fingerPrints[1].data);
                        else
                            command.Parameters.AddWithValue("@enrolled_fmd2", DBNull.Value);
                        command.Parameters.AddWithValue("@finger_id3", 0);
                        if ((row.value.fingerPrints != null) && (row.value.fingerPrints.Count >= 3))
                            command.Parameters.AddWithValue("@enrolled_fmd3", row.value.fingerPrints[2].data);
                        else
                            command.Parameters.AddWithValue("@enrolled_fmd3", DBNull.Value);
                        command.Parameters.AddWithValue("@finger_id4", 0);
                        if ((row.value.fingerPrints != null) && (row.value.fingerPrints.Count >= 4))
                            command.Parameters.AddWithValue("@enrolled_fmd4", row.value.fingerPrints[3].data);
                        else
                            command.Parameters.AddWithValue("@enrolled_fmd4", DBNull.Value);
                        command.Parameters.AddWithValue("@lastfmdchangeddate", DBNull.Value);
                        command.Parameters.AddWithValue("@payroll_balance", ((object)row.value.payrollBalance ?? DBNull.Value));
                        command.Parameters.AddWithValue("@account_balance", ((object)row.value.marketBalance ?? DBNull.Value));
                        command.Parameters.AddWithValue("@lastaccountbalancechangeddate", ((object)row.value.lastDeposit ?? DBNull.Value));
                        if ((row.value.user.tags != null) && (row.value.user.tags.Count > 0))
                            command.Parameters.AddWithValue("@tags", ((object)row.value.user.tags[0].upc ?? DBNull.Value));
                        else
                            command.Parameters.AddWithValue("@tags", DBNull.Value);

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();
                    }
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Load32_Click(object sender, EventArgs e)
        {
            Stream myStream;
            OpenFileDialog openFileDialog32 = new OpenFileDialog();
            if (openFileDialog32.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if ((myStream = openFileDialog32.OpenFile()) != null)
                {
                    string strfilename = openFileDialog32.FileName;
                    string filetext32 = File.ReadAllText(strfilename);
                    textBox1.Text = JsonFormatter.Format(filetext32);
                }
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {


        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var client = new RestClient("http://" + textBox4.Text + ":" + textBox5.Text);

            client.Authenticator = new HttpBasicAuthenticator("kiosk", "password");
            var request = new RestRequest("tsm-sync/_design/Users/_view/getUserById", Method.GET);
            var response = client.Execute(request);
            textBox1.Text = JsonFormatter.Format(response.Content);
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string config_file = @"C:\Program Files (x86)\Apache Software Foundation\CouchDB\etc\couchdb\local.ini";

            try
            {
                textBox1.Text = "Making a backup of Local.ini";
                File.Copy(config_file, config_file + ".backup");
                textBox1.Text = "Trying to overwrite file";
                using (StreamWriter newTask = new StreamWriter(config_file, false))
                {
                    newTask.WriteLine("[couchdb]\r" +
                    "uuid = f302d85532633da841ee02e8889e9dd7\r" +
                    "[httpd]\r" +
                    "port = " + textBox5.Text + "\r" +
                    "bind_address = " + textBox4.Text + "\r" +
                    "[query_servers]\r" +
                    "[couch_httpd_auth]\r" +
                    "require_valid_user = true\r" +
                    "[log]\r" +
                    "[log_level_by_odule]\r" +
                    "[os_daemons]\r" +
                    "[daemons]\r" +
                    "[ssl]\r" +
                    "verify_ssl_certificates = false\r" +
                    "ssl_certificate_max_depth = 1\r" +
                    "[admins]\r" +
                    "kiosk = -pbkdf2-4f9350193331421ca9f5dd8167401bbc93cb4046,1b70b4aec3a129203ddb76727f774d19,10\r");
                }
                textBox1.AppendText("Backup made and password changed\r\n");
                textBox1.AppendText("Address = " + textBox4.Text + "\r\n");
                textBox1.AppendText("port = " + textBox5.Text + "\r\n");
                textBox1.AppendText("Username: kiosk\r\n");
                textBox1.AppendText("Password: password\r\n");
            }
            catch (IOException)
            {
                textBox1.Text = "Overwrite Failed";
            }
        }

        private void openFileDialog32Couch_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            Stream myStream;
            OpenFileDialog openFileDialog32Couch = new OpenFileDialog();
            if (openFileDialog32Couch.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if ((myStream = openFileDialog32Couch.OpenFile()) != null)
                {
                    string strfilename = openFileDialog32Couch.FileName;
                    textBox3.Text = strfilename;
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }



        private void button5_Click(object sender, EventArgs e)
        {
            SQLiteConnection Connection = new SQLiteConnection(@SQLiteDBConection);

            string TableName = "";
            int AdminUsersCount = 0;
            int MarketUsersCount = 0;
            string IssuesLog = "No Issues";

            for (int i = 1; i < 3; i++)
            {

                Connection.Open();

                using (SQLiteCommand SQLiteCommand = Connection.CreateCommand())
                {


                    switch (i)
                    {
                        case 1:
                            TableName = "AdminUser";
                            break;
                        case 2:
                            TableName = "MarketUser";
                            break;
                        default:
                            return;
                    }

                    SQLiteCommand.CommandText = @"SELECT * FROM " + TableName;
                    SQLiteCommand.CommandType = CommandType.Text;
                    SQLiteDataReader Data = SQLiteCommand.ExecuteReader();
                    while (Data.Read())
                    {
                        try
                        {
                            switch (i)
                            {
                                case 1:
                                    InsertNewUserToDB(Convert.ToInt32(Data["Id"]), Data["First"].ToString(), Data["Last"].ToString(), Data["Username"].ToString(), "0.00", Data["Scancode"].ToString(), "NULL", true);
                                    AdminUsersCount = AdminUsersCount + 1;
                                    break;
                                case 2:
                                    InsertNewUserToDB(Convert.ToInt32(Data["Id"]), Data["FirstName"].ToString(), Data["LastName"].ToString(), Data["Email"].ToString(), Data["Balance"].ToString(), Data["Scancode"].ToString(), Data["KioskUserIdentifier"].ToString(), false);
                                    MarketUsersCount = MarketUsersCount + 1;
                                    break;
                                default:
                                    return;
                            }
                        }
                        catch (Exception ex)
                        {
                            IssuesLog = "";
                            IssuesLog = ex.ToString() + "\r\n" + IssuesLog;
                        }

                    }


                }

                Connection.Close();
            }

            txtResultsLog.Text = "Admin users exported: " + AdminUsersCount.ToString() + "\r\n" + txtResultsLog.Text;

            txtResultsLog.Text = "Market users exported: " + MarketUsersCount.ToString() + "\r\n" + txtResultsLog.Text;

            txtResultsLog.Text = txtResultsLog.Text + "\r\n" + "Registered Issues: " + "\r\n\r\n" + IssuesLog;

        }

        string connectionString = ConfigurationManager.ConnectionStrings["deOROLocalDBConection"].ConnectionString;

        private void InsertNewUserToDB(int id, string first_name, string last_name, string email, string account_balance, string barcode, string fingerprintfilename, bool admin)
        {

            SqlConnection LocalDBConnection = new SqlConnection(connectionString);

            string SQLCommand = "INSERT INTO [user] ([id],[pkid],[first_name],[last_name],[email],[is_staff],[is_active],[is_superuser],[is_approved],[created_date_time],[last_updated_on],[account_balance],[barcode],[enrolled_fmd1]) " +
                "VALUES (@id,@pkid,@first_name,@last_name,@email,@is_staff,@is_active,@is_superuser,@is_approved,@created_date_time,@last_updated_on,@account_balance,@barcode, @enrolled_fmd1)";

            SqlCommand Insertdata = new SqlCommand(SQLCommand, LocalDBConnection);

            LocalDBConnection.Open();
            Insertdata.Parameters.AddWithValue("@id", id);
            Insertdata.Parameters.AddWithValue("@pkid", Guid.NewGuid());
            Insertdata.Parameters.AddWithValue("@first_name", first_name);
            Insertdata.Parameters.AddWithValue("@last_name", last_name);
            Insertdata.Parameters.AddWithValue("@email", email);

            if (admin == true)
            {
                Insertdata.Parameters.AddWithValue("@is_staff", 1);
                Insertdata.Parameters.AddWithValue("@is_active", 1);
                Insertdata.Parameters.AddWithValue("@is_superuser", 1);
            }
            else
            {
                Insertdata.Parameters.AddWithValue("@is_staff", 0);
                Insertdata.Parameters.AddWithValue("@is_active", 1);
                Insertdata.Parameters.AddWithValue("@is_superuser", 0);
            }

            Insertdata.Parameters.AddWithValue("@is_approved", 1);

            Insertdata.Parameters.AddWithValue("@created_date_time", DateTime.Now);
            Insertdata.Parameters.AddWithValue("@last_updated_on", DateTime.Now);

            Insertdata.Parameters.AddWithValue("@account_balance", account_balance);

            if (fingerprintfilename != "NULL")
            {
                fingerprintfilename = fingerprintfilename.Insert(8, "-");
                fingerprintfilename = fingerprintfilename.Insert(13, "-");
                fingerprintfilename = fingerprintfilename.Insert(18, "-");
                fingerprintfilename = fingerprintfilename.Insert(23, "-");

                string FMDData = @GetFingerPrintData(fingerprintfilename);

                if (FMDData != "NULL")
                {
                    Insertdata.Parameters.AddWithValue("@enrolled_fmd1", FMDData);
                }
                else
                {
                    Insertdata.Parameters.AddWithValue("@enrolled_fmd1", DBNull.Value);
                }

            }
            else
            {
                Insertdata.Parameters.AddWithValue("@enrolled_fmd1", DBNull.Value);
            }




            string BarcodeData = barcode;

            if (BarcodeData != "")
            {
                Insertdata.Parameters.AddWithValue("@barcode", @barcode);
            }
            else
            {
                Insertdata.Parameters.AddWithValue("@barcode", DBNull.Value);
            }

            //if (identificatorcode.Length > 12)
            //{
            //    Insertdata.Parameters.AddWithValue("@enrolled_fmd1", @GetFingerPrintData(identificatorcode));
            //    Insertdata.Parameters.AddWithValue("@barcode", Convert.DBNull);
            //}
            //else
            //{
            //    Insertdata.Parameters.AddWithValue("@enrolled_fmd1", Convert.DBNull);
            //    Insertdata.Parameters.AddWithValue("@barcode", identificatorcode);
            //}

            Insertdata.ExecuteNonQuery();
            LocalDBConnection.Close();

        }

        private string GetFingerPrintData(string FileName)
        {
            string PathFile = @FMDFilesPath + FileName;

            System.IO.FileInfo FileExists = new System.IO.FileInfo(PathFile);

            if (FileExists.Exists != false)
            {

                string File = PathFile;
                System.IO.StreamReader StreamReader = null;

                string FingerPrintData = "NULL";
                string Line = "";

                StreamReader = new System.IO.StreamReader(File);

                while (FingerPrintData == "NULL")
                {
                    Line = StreamReader.ReadLine();
                    FingerPrintData = @"<?xml version='1.0' encoding='UTF - 8'?><Fid><Byt​​es>" + Line + "</Bytes><Format>1769473</Format><Version>1.0.0</Version></Fid>";
                }

                return FingerPrintData;

            }
            else
            {
                return "NULL";
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged_2(object sender, EventArgs e)
        {

        }
    }
}
