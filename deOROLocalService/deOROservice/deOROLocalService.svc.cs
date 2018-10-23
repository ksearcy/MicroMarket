using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using deOROservice.Classes;

namespace deOROservice
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "deORO" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select deORO.svc or deORO.svc.cs at the Solution Explorer and start debugging.
    public class deORO : IdeORO
    {
        #region Global Variable Declaration

        public string _ConnectionString = ConfigurationManager.ConnectionStrings["deOROconnectionString"].ConnectionString;
        public SqlConnection _SqlConnection;
        public SqlCommand _SqlCommand;
        public SqlDataAdapter _SqlDataAdapter;
        public DataTable _DataTable;

        #endregion

        #region Read To End

        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        #endregion

        #region Log In

        public UserData LogIn(Stream ReceivedData)
        {
            UserData _UserData = new UserData();
            try
            {
                byte[] data = ReadToEnd(ReceivedData);
                string jsonstring = Encoding.UTF8.GetString(data);
                _UserData = JsonConvert.DeserializeObject<UserData>(jsonstring);

                if (string.IsNullOrWhiteSpace(_UserData.UserName) || string.IsNullOrWhiteSpace(_UserData.Password))
                {
                    _UserData.Status = "0";
                    _UserData.Message = "Credential hasn't been provided";
                }
                else
                {
                    _SqlConnection = new SqlConnection(_ConnectionString);
                    if (_SqlConnection.State == ConnectionState.Closed)
                        _SqlConnection.Open();

                    string _Query = "SELECT [id], [pkid], [username], [password], [salt] " +
                                    "FROM [user] WHERE [username]= '" + _UserData.UserName + "'";
                    _SqlCommand = new SqlCommand(_Query, _SqlConnection);
                    _SqlDataAdapter = new SqlDataAdapter(_SqlCommand);
                    _DataTable = new DataTable();
                    _SqlDataAdapter.Fill(_DataTable);
                    if (_DataTable != null && _DataTable.Rows.Count > 0)
                    {
                        string _Password = _UserData.Password;
                        bool _isValidCredential = false;
                        for (int i = 0; i < _DataTable.Rows.Count; i++)
                        {
                            string _EncryptedPasswordFromDB = Convert.ToString(_DataTable.Rows[i]["password"]);
                            string _SALTkey = Convert.ToString(_DataTable.Rows[i]["salt"]);
                            #region Encrypt Password using SHA256 and SALT
                            /****** The encryption techniques used here is SHA256 of (Password+SALT key) ******/
                            string _EncryptedPassword = EncryptionHelper.SHA256Encrypt(_Password, _SALTkey);
                            #endregion

                            if (_EncryptedPassword == _EncryptedPasswordFromDB)
                            {
                                _UserData.CustomerId = Convert.ToString(_DataTable.Rows[i]["id"]);
                                _UserData.ApiKey = Convert.ToString(_DataTable.Rows[i]["pkid"]);
                                _UserData.Status = "1";
                                _UserData.Message = "Successfully logged in";
                                _isValidCredential = true;
                                break;
                            }
                        }
                        if (!_isValidCredential)
                        {
                            _UserData.Status = "0";
                            _UserData.Message = "Invalid credential";
                        }
                    }
                    else
                    {
                        _UserData.Status = "0";
                        _UserData.Message = "Invalid credential";
                    }
                }
            }
            catch (Exception ex)
            {
                _UserData.Status = "0";
                //_UserData.Message = "Error on performing Log In operation";
                _UserData.Message = ex.ToString();
            }
            finally { _SqlConnection.Close(); }

            _UserData.Type = "Response";
            return _UserData;
        }

        #endregion

        #region Get User Profile

        public UserData GetUserProfile(Stream ReceivedData)
        {
            UserData _UserData = new UserData();
            try
            {
                byte[] data = ReadToEnd(ReceivedData);
                string jsonstring = Encoding.UTF8.GetString(data);
                _UserData = JsonConvert.DeserializeObject<UserData>(jsonstring);

                if (string.IsNullOrWhiteSpace(_UserData.UserName) || string.IsNullOrWhiteSpace(_UserData.ApiKey))
                {
                    _UserData.Status = "0";
                    _UserData.Message = "Credential hasn't been provided";
                }
                else
                {
                    _SqlConnection = new SqlConnection(_ConnectionString);
                    if (_SqlConnection.State == ConnectionState.Closed)
                        _SqlConnection.Open();

                    string _Query = "SELECT [first_name], [last_name], [email] FROM [user] " +
                                    "WHERE [username]= '" + _UserData.UserName + "' AND [pkid] = '" + _UserData.ApiKey + "'";
                    _SqlCommand = new SqlCommand(_Query, _SqlConnection);
                    _SqlDataAdapter = new SqlDataAdapter(_SqlCommand);
                    _DataTable = new DataTable();
                    _SqlDataAdapter.Fill(_DataTable);
                    if (_DataTable != null && _DataTable.Rows.Count > 0)
                    {
                        _UserData.FirstName = Convert.ToString(_DataTable.Rows[0]["first_name"]);
                        _UserData.LastName = Convert.ToString(_DataTable.Rows[0]["last_name"]);
                        _UserData.Email = Convert.ToString(_DataTable.Rows[0]["email"]);
                        _UserData.Status = "1";
                        _UserData.Message = "Successfully got user profile";
                    }
                    else
                    {
                        _UserData.Status = "0";
                        _UserData.Message = "Invalid credential";
                    }
                }
            }
            catch (Exception ex)
            {
                _UserData.Status = "0";
                //_UserData.Message = "Error on performing Get User Profile operation";
                _UserData.Message = ex.ToString();
            }
            finally { _SqlConnection.Close(); }

            _UserData.Type = "Response";
            return _UserData;
        }

        #endregion

        #region Update User Profile

        public UserData UpdateUserProfile(Stream ReceivedData)
        {
            UserData _UserData = new UserData();
            try
            {
                byte[] data = ReadToEnd(ReceivedData);
                string jsonstring = Encoding.UTF8.GetString(data);
                _UserData = JsonConvert.DeserializeObject<UserData>(jsonstring);

                if (string.IsNullOrWhiteSpace(_UserData.UserName) || string.IsNullOrWhiteSpace(_UserData.ApiKey))
                {
                    _UserData.Status = "0";
                    _UserData.Message = "Credential hasn't been provided";
                }
                else
                {
                    _SqlConnection = new SqlConnection(_ConnectionString);
                    if (_SqlConnection.State == ConnectionState.Closed)
                        _SqlConnection.Open();

                    string _Query = "SELECT [first_name], [last_name], [email], [salt] FROM [user] " +
                                    "WHERE [username]= '" + _UserData.UserName + "' AND [pkid] = '" + _UserData.ApiKey + "'";
                    _SqlCommand = new SqlCommand(_Query, _SqlConnection);
                    _SqlDataAdapter = new SqlDataAdapter(_SqlCommand);
                    _DataTable = new DataTable();
                    _SqlDataAdapter.Fill(_DataTable);
                    if (_DataTable != null && _DataTable.Rows.Count > 0)
                    {
                        if (_SqlConnection.State == ConnectionState.Closed)
                            _SqlConnection.Open();

                        string _Password = _UserData.Password;
                        string _EncryptedPassword = "";
                        if (!string.IsNullOrWhiteSpace(_Password))
                        {
                            string _SALTkey = Convert.ToString(_DataTable.Rows[0]["salt"]);
                            #region Encrypt Password using SHA256 and SALT
                            /****** The encryption techniques used here is SHA256 of (Password+SALT key) ******/
                            _EncryptedPassword = EncryptionHelper.SHA256Encrypt(_Password, _SALTkey);
                            #endregion
                        }

                        string _LastUpdatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        _Query = "UPDATE [user] SET " +
                                 "[first_name] = '" + _UserData.FirstName + "', [last_name] = '" + _UserData.LastName + "', " +
                                 "[email] = '" + _UserData.Email + "', [last_updated_on] = '" + _LastUpdatedOn + "'" +
                                 (string.IsNullOrWhiteSpace(_Password) ? " " : ", [password] = '" + _EncryptedPassword + "' ") +
                                 "WHERE [username]= '" + _UserData.UserName + "' AND [pkid] = '" + _UserData.ApiKey + "'";
                        _SqlCommand = new SqlCommand(_Query, _SqlConnection);
                        int iSuccess = _SqlCommand.ExecuteNonQuery();
                        _SqlConnection.Close();

                        if (iSuccess > 0)
                        {
                            _UserData.Status = "1";
                            _UserData.Message = "Successfully updated user profile";
                        }
                        else
                        {
                            _UserData.Status = "0";
                            _UserData.Message = "Couldn't update user profile";
                        }
                    }
                    else
                    {
                        _UserData.Status = "0";
                        _UserData.Message = "Invalid credential";
                    }
                }
            }
            catch (Exception ex)
            {
                _UserData.Status = "0";
                //_UserData.Message = "Error on performing Update User Profile operation";
                _UserData.Message = ex.ToString();
            }
            finally { _SqlConnection.Close(); }

            _UserData.Type = "Response";
            return _UserData;
        }

        #endregion

        #region Log Out

        public UserData LogOut(Stream ReceivedData)
        {
            UserData _UserData = new UserData();
            try
            {
                byte[] data = ReadToEnd(ReceivedData);
                string jsonstring = Encoding.UTF8.GetString(data);
                _UserData = JsonConvert.DeserializeObject<UserData>(jsonstring);

                if (string.IsNullOrWhiteSpace(_UserData.UserName) || string.IsNullOrWhiteSpace(_UserData.ApiKey))
                {
                    _UserData.Status = "0";
                    _UserData.Message = "Credential hasn't been provided";
                }
                else
                {
                    _SqlConnection = new SqlConnection(_ConnectionString);
                    if (_SqlConnection.State == ConnectionState.Closed)
                        _SqlConnection.Open();

                    string _Query = "SELECT [id], [first_name], [last_name], [email], [salt] FROM [user] " +
                                    "WHERE [username]= '" + _UserData.UserName + "' AND [pkid] = '" + _UserData.ApiKey + "'";
                    _SqlCommand = new SqlCommand(_Query, _SqlConnection);
                    _SqlDataAdapter = new SqlDataAdapter(_SqlCommand);
                    _DataTable = new DataTable();
                    _SqlDataAdapter.Fill(_DataTable);
                    if (_DataTable != null && _DataTable.Rows.Count > 0)
                    {
                        _UserData.CustomerId = Convert.ToString(_DataTable.Rows[0]["id"]);
                        _UserData.Status = "1";
                        _UserData.Message = "Successfully logged out";
                    }
                    else
                    {
                        _UserData.Status = "0";
                        _UserData.Message = "Invalid credential";
                    }
                }
            }
            catch (Exception ex)
            {
                _UserData.Status = "0";
                //_UserData.Message = "Error on performing Log Out operation";
                _UserData.Message = ex.ToString();
            }
            finally { _SqlConnection.Close(); }

            _UserData.Type = "Response";
            return _UserData;
        }

        #endregion

        #region Delete User

        public UserData DeleteUser(Stream ReceivedData)
        {
            UserData _UserData = new UserData();
            try
            {
                byte[] data = ReadToEnd(ReceivedData);
                string jsonstring = Encoding.UTF8.GetString(data);
                _UserData = JsonConvert.DeserializeObject<UserData>(jsonstring);

                if (string.IsNullOrWhiteSpace(_UserData.UserName) || string.IsNullOrWhiteSpace(_UserData.ApiKey))
                {
                    _UserData.Status = "0";
                    _UserData.Message = "Credential hasn't been provided";
                }
                else
                {
                    _SqlConnection = new SqlConnection(_ConnectionString);
                    if (_SqlConnection.State == ConnectionState.Closed)
                        _SqlConnection.Open();

                    string _Query = "SELECT [id], [pkid], [first_name], [last_name], [email], [account_balance] FROM [user] " +
                                    "WHERE [username]= '" + _UserData.UserName + "' AND [pkid] = '" + _UserData.ApiKey + "'";
                    _SqlCommand = new SqlCommand(_Query, _SqlConnection);
                    _SqlDataAdapter = new SqlDataAdapter(_SqlCommand);
                    _DataTable = new DataTable();
                    _SqlDataAdapter.Fill(_DataTable);
                    if (_DataTable != null && _DataTable.Rows.Count > 0)
                    {
                        string _UserID = Convert.ToString(_DataTable.Rows[0]["id"]);
                        string _userpkid = Convert.ToString(_DataTable.Rows[0]["pkid"]);
                        string _email = Convert.ToString(_DataTable.Rows[0]["email"]);
                        string _first_name = Convert.ToString(_DataTable.Rows[0]["first_name"]);
                        string _last_name = Convert.ToString(_DataTable.Rows[0]["last_name"]);
                        string _amount_to_refund = Convert.ToString(_DataTable.Rows[0]["account_balance"]);
                        string _pkid = Convert.ToString(Guid.NewGuid());
                        string _created_date_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                        while (string.IsNullOrWhiteSpace(_pkid))
                            _pkid = Convert.ToString(Guid.NewGuid());

                        _Query = "INSERT INTO [users_deleted] " +
                                 "(" +
                                     "[pkid], [userpkid], [email], [first_name], [last_name], " +
                                     "[amount_to_refund], [created_date_time]" +
                                 ") " +
                                 "VALUES " +
                                 "(" +
                                      "'" + _pkid + "', '" + _userpkid + "', '" + _email + "', " +
                                      "'" + _first_name + "', '" + _last_name + "', " + _amount_to_refund + ", " +
                                      "'" + _created_date_time + "'" +
                                 ")";
                        _SqlCommand = new SqlCommand(_Query, _SqlConnection);
                        int iSuccess = _SqlCommand.ExecuteNonQuery();
                        if (iSuccess > 0)
                        {
                            _Query = "UPDATE [user] SET [is_active] = 0 WHERE [id] = " + _UserID;
                            _SqlCommand = new SqlCommand(_Query, _SqlConnection);
                            iSuccess = _SqlCommand.ExecuteNonQuery();
                            if (iSuccess > 0)
                            {
                                _UserData.Status = "1";
                                _UserData.Message = "User deleted successfully";
                            }
                            else
                            {
                                _UserData.Status = "0";
                                _UserData.Message = "Couldn't delete user";
                            }
                        }
                        else
                        {
                            _UserData.Status = "0";
                            _UserData.Message = "Couldn't delete user";
                        }
                    }
                    else
                    {
                        _UserData.Status = "0";
                        _UserData.Message = "Invalid credential";
                    }
                }
            }
            catch (Exception ex)
            {
                _UserData.Status = "0";
                //_UserData.Message = "Error on performing Delete User operation";
                _UserData.Message = ex.ToString();
            }
            finally { _SqlConnection.Close(); }

            _UserData.Type = "Response";
            return _UserData;
        }

        #endregion
    }
}