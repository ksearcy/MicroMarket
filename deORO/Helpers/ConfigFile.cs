using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using deORODataAccessApp.Models;

namespace deORO.Helpers
{
    public class ConfigFile
    {
        public static void EncryptConfigSections(string section)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var objAppsettings = config.GetSection(section);

            if (!objAppsettings.SectionInformation.IsProtected)
            {
                objAppsettings.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                objAppsettings.SectionInformation.ForceSave = true;
                config.Save(ConfigurationSaveMode.Full);
            }
        }

        public static Dictionary<string, List<KeyValue>> LoadConfigSections()
        {
            Dictionary<string, List<KeyValue>> list = new Dictionary<string, List<KeyValue>>();
            list.Add("application", ConvertToKeyValue(ConfigurationManager.GetSection("application") as NameValueCollection));
            list.Add("mail", ConvertToKeyValue(ConfigurationManager.GetSection("mail") as NameValueCollection));
            list.Add("coin", ConvertToKeyValue(ConfigurationManager.GetSection("coin") as NameValueCollection));
            list.Add("bill", ConvertToKeyValue(ConfigurationManager.GetSection("bill") as NameValueCollection));
            list.Add("cardreader", ConvertToKeyValue(ConfigurationManager.GetSection("cardreader") as NameValueCollection));
            list.Add("cardprocessor", ConvertToKeyValue(ConfigurationManager.GetSection("cardprocessor") as NameValueCollection));
            list.Add("barcodereader", ConvertToKeyValue(ConfigurationManager.GetSection("barcodereader") as NameValueCollection));
            list.Add("usbrelay", ConvertToKeyValue(ConfigurationManager.GetSection("usbrelay") as NameValueCollection));
            list.Add("camerafeed", ConvertToKeyValue(ConfigurationManager.GetSection("camerafeed") as NameValueCollection));
            list.Add("refillrewards", ConvertToKeyValue(ConfigurationManager.GetSection("refillrewards") as NameValueCollection));
            list.Add("marshall", ConvertToKeyValue(ConfigurationManager.GetSection("marshall") as NameValueCollection));
            list.Add("dispense", ConvertToKeyValue(ConfigurationManager.GetSection("dispense") as NameValueCollection));
            list.Add("ftp", ConvertToKeyValue(ConfigurationManager.GetSection("ftp") as NameValueCollection));
            list.Add("printer", ConvertToKeyValue(ConfigurationManager.GetSection("printer") as NameValueCollection));

            return list;
        }

        public static List<KeyValue> ConvertToKeyValue(NameValueCollection collection)
        {
            List<KeyValue> list = new List<KeyValue>();
            foreach (var key in collection)
            {
                list.Add(new KeyValue { Key = key.ToString(), Value = collection[key.ToString()] });
            }

            return list;
        }

        public static bool SaveSettings(string section, List<KeyValue> list)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

                foreach (KeyValue k in list)
                {
                    xmlDoc.SelectSingleNode(string.Format("//{0}/add[@key='{1}']", section, k.Key)).Attributes["value"].Value = k.Value;
                }

                xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                ConfigurationManager.RefreshSection(section);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool SaveSettings(string section, KeyValue keyValue)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                xmlDoc.SelectSingleNode(string.Format("//{0}/add[@key='{1}']", section, keyValue.Key)).Attributes["value"].Value = keyValue.Value;

                xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                ConfigurationManager.RefreshSection(section);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
