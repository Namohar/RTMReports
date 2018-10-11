using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BAL
{
    public class clsExport
    {
        public string ExportToCSV(DataTable dt)
        {
            
            //Build the CSV file data as a Comma separated string.
            string csv = string.Empty;

            foreach (DataColumn column in dt.Columns)
            {
                //Add the Header row for CSV file.
                csv += column.ColumnName + ',';
            }

            //Add new line.
            csv += "\r\n";

            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    //Add the Data rows.
                    csv += row[column.ColumnName].ToString().Replace(",", ";") + ',';
                }

                //Add new line.
                csv += "\r\n";
            }

            return csv;
            //Download the CSV file.
            //Response.Clear();
            //Response.Buffer = true;
            //Response.AddHeader("content-disposition", "attachment;filename=SqlExport.csv");
            //Response.Charset = "";
            //Response.ContentType = "application/text";
            //Response.Output.Write(csv);
            //Response.Flush();
            //Response.End();
        }
    }
}
