using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace deOROWeb.Helper
{
    public static class DbHelper
    {
        public static DataTable ExecuteDataTable(string connectionString, string commandText)
        {
            //ULTRA IMPORTANT NOTE: YOU HAVE TO COMMENT THE NEXT  LINE OF CODE BEFORE WEB DEPLOY  BECAUSE IT IS USED JUST FOR DEVELOPING, FOR SOME REASON IT NEEDS TO BE USED BECAUS WHEN VISUAL STUIDIO COMPILE THE CODE THIS HELPER IS NOT ABLE TO HAV ACCESS TO THE SERVER BECAUSE IS GETTING ANOTHER STRING CONNECTION.

            //connectionString = "data source=209.159.152.234;initial catalog=deORO_santaclarita;uid=sa;pwd=Polaris*~;multipleactiveresultsets=True;";

         //===================================================================================================================================================================================================================================================================================================================

            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt;
        }

        public static DataTable ToDataTable<T>(this IList<T> data, string tableName = "")
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable(tableName);
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }
}