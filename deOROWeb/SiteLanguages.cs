using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace deOROWeb
{
    public class SiteLanguages
    {
        public static List<Languages> AvailableLanguages = new List<Languages>
        {
             new Languages{ LangFullName = "English", LangCultureName = "en"},
             new Languages{ LangFullName = "Español", LangCultureName = "es"},
         };

        public static bool IsLanguageAvailable(string lang)
        {
            return AvailableLanguages.Where(a => a.LangCultureName.Equals(lang)).FirstOrDefault() != null ? true : false;
        }

        public static string GetDefaultLanguage()
        {
            return AvailableLanguages[0].LangCultureName;
        }

        public void SetLanguage(string lang)
        {
            try
            {
                if (!IsLanguageAvailable(lang))
                    lang = GetDefaultLanguage();
                var cultureInfo = new CultureInfo(lang);

               
     
                Thread.CurrentThread.CurrentUICulture = cultureInfo;

                //==================================This configuration is to change numbers, dates, etc to the selected language=====================================
                //CultureInfo USFormats = new CultureInfo("en");
                //Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);             
                //Thread.CurrentThread.CurrentCulture.DateTimeFormat = USFormats.DateTimeFormat;
                //Thread.CurrentThread.CurrentCulture.NumberFormat = USFormats.NumberFormat;

                HttpCookie langCookie = new HttpCookie("culture", lang);
                langCookie.Expires = DateTime.Now.AddYears(1);
                HttpContext.Current.Response.Cookies.Add(langCookie);

            }
            catch (Exception ex)
            {
                return;
            }
        }
    }

    public class Languages
    {
        public string LangFullName { get; set; }
        public string LangCultureName { get; set; }
    }
}