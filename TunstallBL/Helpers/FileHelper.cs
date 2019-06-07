using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace TunstallBL.Helpers
{
    public static class FileHelper
    {
        public static DataTable ReadCSVFileToTable(string file, char separator, bool hasHeaders = true, string[] header = null)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(file))
            {
                string[] headers = null;
                if (hasHeaders)
                {
                    headers = sr.ReadLine().Split(separator);
                }
                else
                {
                    headers = header;
                }

                foreach (string h in headers)
                {
                    dt.Columns.Add(h);
                }

                while (!sr.EndOfStream)
                {
                    string[] rows = Regex.Split(sr.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }
    }
}
