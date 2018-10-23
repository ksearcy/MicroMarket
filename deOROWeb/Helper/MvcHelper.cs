using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace deOROWeb.Helper
{
    public static class MvcHelper
    {
        public class TrueFlaseHelper
        {
            public static string Label(byte? text)
            {
                if (!text.HasValue)
                    return "False";

                return text.ToString() == "1" ? "True" : "False";
            }
        }

        public class YesNoHelper
        {
            public static string Label(byte? text)
            {
                if (!text.HasValue)
                    return "No";

                return text.ToString() == "1" ? "Yes" : "No";
            }
        }

        public class DateFormatHelper
        {
            public static string ToShortDate(DateTime? dateTime)
            {
                if (dateTime.HasValue)
                    return dateTime.Value.ToString("d");
                else
                    return "";
            }
        }

        public class ActiveInActiveHelper
        {
            public static string Label(byte? text)
            {
                if (!text.HasValue)
                    return "Inactive";

                return text.ToString() == "1" ? "Active" : "Inactive";
            }
        }

        public static IHtmlString DropDownListFiltered(this HtmlHelper helper, string name, string columName, string value, SelectList selectList)
        {
            StringBuilder output = new StringBuilder();

            //output.Append("<select id='" + name + "' name='" + name + "' class='planogram-filter' multiple='multiple'>");
            //output.Append("<option value=0>Select One</option>");
            //output.Append("<option value=1>Select Two</option>");
            //output.Append("<option value=2>Select Three</option>");

            //output.Append("</select>");

            return MvcHtmlString.Create(output.ToString());
        }

        public static IHtmlString DropDownListSelected(this HtmlHelper helper, string name, string selectedValue, SelectList selectList, object htmlAttributes)
        {
            var dictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            StringBuilder output = new StringBuilder();

            string[] array = { };

            if (selectedValue != null && selectedValue != "null")
            {
                array = selectedValue.Split(',');
            }

            output.Append("<select id='{0}' name='{1}' class='{2}' multiple='multiple'>");
            foreach (var item in selectList)
            {
                bool found = false;
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] == item.Value)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                    output.Append("<option value=" + item.Value.ToString() + " selected=selected>" + item.Text + "</option>");
                else
                    output.Append("<option value=" + item.Value.ToString() + ">" + item.Text + "</option>");
            }

            output.Append("</select>");

            string finalString = output.ToString();
            finalString = string.Format(finalString, dictionary["id"], name, dictionary["class"]);

            return MvcHtmlString.Create(finalString);
        }

        public static IHtmlString DropDownListSelected(this HtmlHelper helper, string name, string selectedValue, SelectList selectList)
        {
            StringBuilder output = new StringBuilder();

            output.Append("<select id='" + name + "' name='" + name + "' class='form-control'>");
            output.Append("<option value=0>Select One</option>");

            foreach (var item in selectList)
            {
                if (item.Value == selectedValue)
                    output.Append("<option value=" + item.Value.ToString() + " selected=selected>" + item.Text + "</option>");
                else
                    output.Append("<option value=" + item.Value.ToString() + ">" + item.Text + "</option>");
            }

            output.Append("</select>");

            return MvcHtmlString.Create(output.ToString());
        }

        public static IHtmlString Image(this HtmlHelper helper, string name, string imagePath)
        {
            StringBuilder output = new StringBuilder();
            output.Append(string.Format("<img id='{0}' name='{0}' src='{1}'/>", name, HttpContext.Current.Request.ApplicationPath + "/Images/" + imagePath));
            return MvcHtmlString.Create(output.ToString());
        }
    }
}