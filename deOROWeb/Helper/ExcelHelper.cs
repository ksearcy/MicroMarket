﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using deORODataAccess;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace deOROWeb.Helper
{
    public static class ExcelHelper
    {
        public static void ExportDataTable(DataTable table, string exportFile)
        {
            //create the empty spreadsheet template and save the file //using the class generated by the Productivity tool  
            ExcelDocument excelDocument = new ExcelDocument();
            excelDocument.CreatePackage(exportFile);

            //populate the data into the spreadsheet  
            using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.Open(exportFile, true))
            {
                WorkbookPart workbook = spreadsheet.WorkbookPart;
                //create a reference to Sheet1  
                WorksheetPart worksheet = workbook.WorksheetParts.Last();
                SheetData data = worksheet.Worksheet.GetFirstChild<SheetData>();

                //loop through each data row  
                DataRow contentRow;
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    contentRow = table.Rows[i];
                    data.AppendChild(createContentRow(contentRow, i + 2));
                }

            }

        }

        private static Cell createTextCell(int columnIndex, int rowIndex, object cellValue)
        {
            Cell cell = new Cell();

            cell.DataType = CellValues.InlineString;
            cell.CellReference = getColumnName(columnIndex) + rowIndex;

            InlineString inlineString = new InlineString();
            Text t = new Text();

            t.Text = cellValue.ToString();
            inlineString.AppendChild(t);
            cell.AppendChild(inlineString);

            return cell;
        }

        private static Row createContentRow(DataRow dataRow, int rowIndex)
        {
            Row row = new Row
            {
                RowIndex = (UInt32)rowIndex
            };

            for (int i = 0; i < dataRow.Table.Columns.Count; i++)
            {
                Cell dataCell = createTextCell(i + 1, rowIndex, dataRow[i]);
                row.AppendChild(dataCell);
            }
            return row;
        }

        private static string getColumnName(int columnIndex)
        {
            int dividend = columnIndex;
            string columnName = String.Empty;
            int modifier;

            while (dividend > 0)
            {
                modifier = (dividend - 1) % 26;
                columnName =
                    Convert.ToChar(65 + modifier).ToString() + columnName;
                dividend = (int)((dividend - modifier) / 26);
            }

            return columnName;
        }

        private static IEnumerator<Cell> GetExcelCellEnumerator(Row row)
        {
            int currentCount = 0;
            foreach (Cell cell in row.Descendants<Cell>())
            {
                string columnName = GetColumnName(cell.CellReference);

                int currentColumnIndex = ConvertColumnNameToNumber(columnName);

                for (; currentCount < currentColumnIndex; currentCount++)
                {
                    var emptycell = new Cell() { DataType = null, CellValue = new CellValue(string.Empty) };
                    yield return emptycell;
                }

                yield return cell;
                currentCount++;
            }
        }

        private static string GetColumnName(string cellReference)
        {
            var regex = new Regex("[A-Za-z]+");
            var match = regex.Match(cellReference);

            return match.Value;
        }

        private static int ConvertColumnNameToNumber(string columnName)
        {
            var alpha = new Regex("^[A-Z]+$");
            if (!alpha.IsMatch(columnName)) throw new ArgumentException();

            char[] colLetters = columnName.ToCharArray();
            Array.Reverse(colLetters);

            var convertedValue = 0;
            for (int i = 0; i < colLetters.Length; i++)
            {
                char letter = colLetters[i];
                int current = i == 0 ? letter - 65 : letter - 64; // ASCII 'A' = 65
                convertedValue += current * (int)Math.Pow(26, i);
            }

            return convertedValue;
        }

        private static string ReadExcelCell(Cell cell, WorkbookPart workbookPart)
        {
            var cellValue = cell.CellValue;
            var text = (cellValue == null) ? cell.InnerText : cellValue.Text;
            if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
            {
                text = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(
                        Convert.ToInt32(cell.CellValue.Text)).InnerText;
            }

            return (text ?? string.Empty).Trim();
        }

        public static string ImportItemsFromExcel(string fileName, string[] columnNames)
        {
            try
            {
                ItemRepository itemRepo = new ItemRepository();
                ManufacutrerRepository manuRepo = new ManufacutrerRepository();
                CategoryRepository catRepo = new CategoryRepository();

                ImportSummary summary = new ImportSummary();
                summary.adds = 0;
                summary.updates = 0;
                summary.mismatchCategories = 0;
                summary.mismatchManufacturers = 0;
                summary.mismatchSubCategories = 0;


                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fs, false))
                    {
                        WorkbookPart workbookPart = doc.WorkbookPart;
                        SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
                        SharedStringTable sst = sstpart.SharedStringTable;

                        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                        Worksheet sheet = worksheetPart.Worksheet;

                        var rows = sheet.Descendants<Row>();
                        Console.WriteLine("Row count = {0}", rows.LongCount());

                        bool firstRow = true;
                        int i = 0;
                        foreach (Row row in rows)
                        {
                            i++;
                            if (firstRow)
                            {
                                firstRow = false;
                                continue;
                            }

                            item item = null;

                            int cellIndex = 0;
                            bool edit = false;
                            bool error = false;
                            bool add = false;
                            int categoryid = 0;

                            foreach (Cell c in row.Elements<Cell>())
                            {
                                if (c.HasChildren)
                                {
                                    if (cellIndex == 0) //Id
                                    {
                                        try
                                        {
                                            int id = Convert.ToInt32(ReadExcelCell(c, workbookPart).Trim());
                                            item = itemRepo.GetSingleById(x => x.id == id);
                                        }
                                        catch { }

                                        if (item == null)
                                        {
                                            item = new deORODataAccess.item();
                                            itemRepo.Add(item);
                                            add = true;
                                            summary.adds++;
                                        }
                                        else
                                        {
                                            //itemRepo.Edit(item);
                                            edit = true;
                                            summary.updates++;
                                        }

                                    }
                                    else
                                    {

                                        string cellValue = ReadExcelCell(c, workbookPart).Trim();
                                        System.Diagnostics.Debug.Write(cellValue + " \t");

                                        if (!cellValue.Equals(""))
                                        {
                                            string colName = columnNames[cellIndex].Substring(0, columnNames[cellIndex].IndexOf(":"));
                                            string typeString = columnNames[cellIndex].Substring(columnNames[cellIndex].IndexOf(":") + 1);

                                            if (colName == "manufacturerid")
                                            {
                                                var m = manuRepo.GetSingleById(x => x.name == cellValue);

                                                if (m != null)
                                                    cellValue = m.id.ToString();
                                                else
                                                {
                                                    cellValue = "";
                                                    summary.mismatchManufacturers++;
                                                }
                                            }
                                            else if (colName == "categoryid")
                                            {
                                                var c1 = catRepo.GetSingleById(x => x.name == cellValue.Trim() && x.parentid == null);

                                                if (c1 != null)
                                                {
                                                    categoryid = c1.id;
                                                    cellValue = c1.id.ToString();
                                                }
                                                else
                                                {
                                                    cellValue = "";
                                                    summary.mismatchCategories++;
                                                }
                                            }
                                            else if (colName == "subcategoryid")
                                            {
                                                var c2 = catRepo.GetSingleById(x => x.name == cellValue.Trim() && x.parentid == categoryid && x.parentid != null);

                                                if (c2 != null)
                                                    cellValue = c2.id.ToString();
                                                else
                                                {
                                                    cellValue = "";
                                                    summary.mismatchSubCategories++;
                                                }
                                            }
                                            else if (colName == "barcode" && add)
                                            {
                                                var b = itemRepo.GetSingleById(x => x.barcode == cellValue.Trim());

                                                if (b != null)
                                                {
                                                    error = true;
                                                    summary.error += string.Format("<font color='navy'>Duplicate Error</font> - <b>Barcode: </b>{0}<br>", b.barcode);
                                                    break;
                                                }
                                            }

                                            try
                                            {
                                                if (cellValue != null && cellValue.Trim() != "")
                                                    itemRepo.Edit(item, colName, typeString, cellValue);
                                            }
                                            catch
                                            {
                                                error = true;
                                                summary.error += string.Format("<font color='red'>Import Error</font> - <b>Barcode: </b>{0}, <b>Column: </b>{1},               <b>Value: </b>{2}<br>", item.barcode, colName, cellValue);
                                                break;
                                            }
                                        }
                                    }
                                }

                                cellIndex++;
                            }

                            if (add && error)
                            {
                                summary.adds--;
                                itemRepo.Delete(item);
                            }
                            else if (edit && !error)
                            {
                                itemRepo.Edit(item);
                            }

                            System.Diagnostics.Debug.WriteLine("");
                            Console.WriteLine(i);
                        }
                        itemRepo.Save();
                    }
                }

                return summary.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public static string ImportLocationItemsFromExcel(string fileName, string[] columnNames)
        {
            try
            {
                LocationItemRepository locItemRepo = new LocationItemRepository();

                ImportSummary summary = new ImportSummary();
                summary.adds = 0;
                summary.updates = 0;
                summary.mismatchCategories = 0;
                summary.mismatchManufacturers = 0;
                summary.mismatchSubCategories = 0;


                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fs, false))
                    {
                        WorkbookPart workbookPart = doc.WorkbookPart;
                        SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
                        SharedStringTable sst = sstpart.SharedStringTable;

                        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                        Worksheet sheet = worksheetPart.Worksheet;

                        var rows = sheet.Descendants<Row>();
                        Console.WriteLine("Row count = {0}", rows.LongCount());

                        bool firstRow = true;
                        int i = 0;
                        foreach (Row row in rows)
                        {
                            i++;
                            if (firstRow)
                            {
                                firstRow = false;
                                continue;
                            }

                            location_item location_item = null;

                            int cellIndex = 0;
                            bool edit = false;
                            bool error = false;
                            bool add = false;
                            int id = 0;
                            decimal price = 0;
                            byte is_taxable = 0;
                            decimal tax_percent = 0;

                            foreach (Cell c in row.Elements<Cell>())
                            {

                                if (c.HasChildren)
                                {


                                    if (cellIndex == 0) //Id
                                    {
                                        try
                                        {
                                            id = Convert.ToInt32(ReadExcelCell(c, workbookPart).Trim());
                                            location_item = locItemRepo.GetSingleById(x => x.id == id);
                                        }
                                        catch { }

                                        if (location_item == null)
                                        {
                                            location_item = new deORODataAccess.location_item();
                                            locItemRepo.Add(location_item);
                                            add = true;
                                            summary.adds++;
                                        }
                                        else
                                        {
                                            //itemRepo.Edit(item);
                                            edit = true;
                                            summary.updates++;
                                        }

                                    }
                                    else
                                    {

                                        string cellValue = ReadExcelCell(c, workbookPart).Trim();
                                        System.Diagnostics.Debug.Write(cellValue + " \t");
                                        bool update_item = false;



                                        if (!cellValue.Equals(""))
                                        {
                                            string colName = columnNames[cellIndex].Substring(0, columnNames[cellIndex].IndexOf(":"));
                                            string typeString = columnNames[cellIndex].Substring(columnNames[cellIndex].IndexOf(":") + 1);

                                            if (colName == "is_taxable")
                                            {

                                                is_taxable = Convert.ToByte(cellValue);

                                                //var t1 = locItemRepo.GetSingleById(x => x.id == id);

                                                //t1.is_taxable = is_taxable;

                                                //if (t1 != null)
                                                //{
                                                //    //cellValue = t1.id.ToString();
                                                //}
                                                //else
                                                //{
                                                //    cellValue = "";
                                                //    summary.mismatchManufacturers++;
                                                //}

                                                update_item = true;
                                            }
                                            else if (colName == "price")
                                            {
                                                price = Convert.ToDecimal(cellValue.Trim());

                                                //var p = locItemRepo.GetSingleById(x => x.id == id);

                                                //p.price = price;

                                                //if (p != null)
                                                //{
                                                //    //cellValue = p.id.ToString();
                                                //}
                                                //else
                                                //{
                                                //    cellValue = "";
                                                //    summary.mismatchCategories++;
                                                //}

                                                update_item = true;

                                            }
                                            else if (colName == "tax_percent")
                                            {
                                                tax_percent = Convert.ToDecimal(cellValue.Trim());

                                                //var t2 = locItemRepo.GetSingleById(x => x.id == id);

                                                decimal price_tax_included = 0;

                                                if (is_taxable != 0)
                                                {
                                                    price_tax_included = (price * ((tax_percent / 100) + 1));
                                                }
                                                else
                                                {
                                                    price_tax_included = 0;
                                                    cellValue = "0.000";
                                                }

                                                try
                                                {
                                                    locItemRepo.Edit(location_item, "price_tax_included", "System.Decimal", price_tax_included);
                                                }
                                                catch
                                                {
                                                    error = true;
                                                    summary.error += string.Format("<font color='red'>Import Error</font> - <b>Barcode: </b>{0}, <b>Column: </b>{1},               <b>Value: </b>{2}<br>", location_item.itemid, colName, cellValue);
                                                    break;
                                                }


                                                update_item = true;
                                            }
                                            else if (colName == "crv")
                                            {
                                                var crv = Convert.ToDecimal(cellValue.Trim());

                                                //var crvdata = locItemRepo.GetSingleById(x => x.id == id);

                                                //crvdata.crv = crv;

                                                //if (crvdata != null)
                                                //{
                                                //    //cellValue = crv.id.ToString();
                                                //}

                                                update_item = true;

                                            }
                                            else if (colName == "par")
                                            {
                                                try
                                                {
                                                    var par = Convert.ToInt32(cellValue.Trim());
                                                }
                                                catch (Exception)
                                                {
                                                    cellValue = "0";
                                                    throw;
                                                }


                                                //var pardata = locItemRepo.GetSingleById(x => x.id == id);

                                                //pardata.par = par;

                                                //if (pardata != null)
                                                //{
                                                //    //cellValue = par.id.ToString();
                                                //}

                                                update_item = true;

                                            }

                                            //else if (colName == "price_tax_included")
                                            //{
                                            //    var price_tax_included = Convert.ToDecimal(cellValue.Trim());

                                            //    var p2 = locItemRepo.GetSingleById(x => x.id == id);

                                            //    p2.price_tax_included = price_tax_included;

                                            //    if (p2 != null)
                                            //    {
                                            //        cellValue = p2.id.ToString();
                                            //    }
                                            //    else
                                            //    {
                                            //        cellValue = "";
                                            //        summary.mismatchSubCategories++;
                                            //    }

                                            //    update_item = true;

                                            //}
                                            //else if (colName == "tax" && add)
                                            //{

                                            //    var tax = Convert.ToDecimal(cellValue.Trim());

                                            //    var t = locItemRepo.GetSingleById(x => x.id == id);

                                            //    t.tax = tax;

                                            //    if (t != null)
                                            //    {
                                            //        //cellValue = t.id.ToString();
                                            //    }

                                            //    update_item = true;
                                            //}


                                            if (update_item == true)
                                            {
                                                try
                                                {
                                                    if (cellValue != null && cellValue.Trim() != "")
                                                        locItemRepo.Edit(location_item, colName, typeString, cellValue);
                                                }
                                                catch
                                                {
                                                    error = true;
                                                    summary.error += string.Format("<font color='red'>Import Error</font> - <b>Barcode: </b>{0}, <b>Column: </b>{1},               <b>Value: </b>{2}<br>", location_item.itemid, colName, cellValue);
                                                    break;
                                                }
                                            }


                                        }
                                    }
                                }

                                cellIndex++;
                            }

                            if (add && error)
                            {
                                summary.adds--;
                                locItemRepo.Delete(location_item);
                            }
                            else if (edit && !error)
                            {
                                if (Convert.ToBoolean(location_item.is_taxable))
                                {
                                    location_item.tax = ((location_item.price) + (location_item.crv == null ? 0 : location_item.crv.Value)) * (location_item.tax_percent.Value * 0.01m);
                                    location_item.price_tax_included = Math.Round(((location_item.price) + (location_item.crv == null ? 0 : location_item.crv.Value)).Value + location_item.tax.Value, 2);
                                }
                                else
                                {
                                    location_item.price_tax_included = Math.Round(((location_item.price) + (location_item.crv == null ? 0 : location_item.crv.Value)).Value, 2);
                                }

                                locItemRepo.Edit(location_item);
                            }

                            System.Diagnostics.Debug.WriteLine("");
                            Console.WriteLine(i);
                        }
                        locItemRepo.Save();
                    }
                }

                return summary.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

    }

    public struct ImportSummary
    {
        public int adds { get; set; }
        public int updates { get; set; }
        public int mismatchCategories { get; set; }
        public int mismatchSubCategories { get; set; }
        public int mismatchManufacturers { get; set; }
        public int mismatchLocationItems { get; set; }
        public string error { get; set; }

        public override string ToString()
        {
            string str = @"<table align='center'>
                         <tr><td style='width:200px'>Total Adds</td><td>{0}</td></tr>
                         <tr><td style='width:200px'>Total Updates</td><td>{1}</td></tr>
                         <tr><td style='width:200px'>Total Category Mismatches</td><td>{2}</td></tr>
                         <tr><td style='width:200px'>Total SubCategory Mismatches</td><td>{3}</td></tr>
                         <tr><td style='width:200px'>Total Manufacturer Mismatches</td><td>{4}</td></tr>
                         <tr><td style='width:200px'>Total Location Items Mismatches</td><td>{5}</td></tr>
                         <tr><td style='width:200px'>Errors</td><td>{6}</td></tr>
                         </table>";

            return String.Format(str, adds, updates, mismatchCategories, mismatchSubCategories, mismatchManufacturers, mismatchLocationItems, error);
        }
    }
}