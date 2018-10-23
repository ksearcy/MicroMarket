using ExcelLibrary.SpreadSheet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace deOROAlerts
{
    public static class Helper
    {
        public static DataTable ExecuteDataTable(string connectionString, string commandText)
        {
            //connectionString = "data source=209.159.152.234;initial catalog=deORO_ArcaPeru;uid=sa;pwd=Polaris*~;multipleactiveresultsets=True;";
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

        public static string FormatHTML(DataTable dt,string fromDate, string toDate, string title)
        {
            if (dt == null) throw new ArgumentNullException("dt");
            string tab = "\t";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<h1>" + title + "<h1>");
            sb.AppendLine(string.Format("<h3>Date Range: {0} - {1} <h3>", fromDate, toDate));
            sb.Append("<br>");

            sb.AppendLine(tab + tab + "<table border=1>");
            // headers.
            sb.Append(tab + tab + tab + "<thead style='background-color:gray;color:white'><tr>");

            foreach (DataColumn dc in dt.Columns)
            {
                sb.AppendFormat("<td>{0}</td>", dc.ColumnName);
            }

            sb.AppendLine("</thead></tr>");

            // data rows
            foreach (DataRow dr in dt.Rows)
            {

                sb.Append(tab + tab + tab + "<tr>");
                foreach (DataColumn dc in dt.Columns)
                {
                    string cellValue = dr[dc] != null ? dr[dc].ToString() : "";
                    sb.AppendFormat("<td>{0}</td>", cellValue);
                }

                sb.AppendLine("</tr>");
            }

            sb.AppendLine(tab + tab + "</table>");
            return sb.ToString();
        }

        public static string CreateExcelReport(DataTable dt,  string fileName, string startRow = "0", string startCell = "0", string fromDate= "", string toDate = "")
        {

            fileName = fileName + " " + DateTime.Now.ToShortDateString().Replace("/", "_");
            string file = fileName + ".xls"; 
            Workbook workbook = new Workbook();
            Worksheet worksheet = new Worksheet(fileName);
            int startRowNumber = Convert.ToInt32(startRow);
            int startCellNumber = Convert.ToInt32(startCell);
            int cellAccumulator = startCellNumber;

            foreach (DataColumn dc in dt.Columns)
            {
                worksheet.Cells[startRowNumber, cellAccumulator] = new Cell(dc.ColumnName);

                cellAccumulator = cellAccumulator + 1;
                
            }          
            
            foreach (DataRow dr in dt.Rows)
            {
                cellAccumulator = startCellNumber;
                startRowNumber = startRowNumber + 1;
               
                foreach (DataColumn dc in dt.Columns)
                {
                    string cellValue = dr[dc] != null ? dr[dc].ToString() : "";
                    worksheet.Cells[startRowNumber, cellAccumulator] = new Cell(cellValue); 
                    cellAccumulator = cellAccumulator + 1;
                }

            }


            //=====WE ADD SOME EMPTY CELLS BECAUSE THE FILE IS DETECTED LIKE DAMAGED BY EXCEL IF IS LESS THAN 7KB
            for (int i = 0; i < 50; i++)
            {
                startRowNumber = startRowNumber + 1;

                cellAccumulator = 1;

                for (int j = 0; j < 10; j++)
                {
                    worksheet.Cells[startRowNumber, cellAccumulator] = new Cell("");
                    cellAccumulator = cellAccumulator + 1; ;
                }
            
            }

            //worksheet.Cells[0, 1] = new Cell((short)1);
            //worksheet.Cells[2, 0] = new Cell(9999999);
            //worksheet.Cells[3, 3] = new Cell((decimal)3.45); 
            //worksheet.Cells[2, 2] = new Cell("Text string"); 
            //worksheet.Cells[2, 4] = new Cell("Second string"); 
            //worksheet.Cells[4, 0] = new Cell(32764.5, "#,##0.00"); 
            //worksheet.Cells[5, 1] = new Cell(DateTime.Now, @"YYYY\-MM\-DD"); 

            worksheet.Cells.ColumnWidth[0, 1] = 3000; 
            workbook.Worksheets.Add(worksheet);

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TempFiles\" + file);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            workbook.Save(path);

            return path;
            
           

        }
       
    }
}
