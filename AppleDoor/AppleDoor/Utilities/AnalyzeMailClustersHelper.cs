using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace AppleDoor.Utilities
{
    class AnalyzeMailClustersHelper
    {
        //export input and assigned cluster to excel for manual analysis
        public void ExportToExcel(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {

                //open file
                StreamWriter wr = new StreamWriter(@"D:\RND\Outlook\AppleDoor\DownloadMobileNoExcel.xls");

                try
                {

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        wr.Write(dt.Columns[i].ToString().ToUpper() + "\t");
                    }

                    wr.WriteLine();

                    //write rows to excel file
                    for (int i = 0; i < (dt.Rows.Count); i++)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            if (dt.Rows[i][j] != null)
                            {
                                wr.Write(Convert.ToString(dt.Rows[i][j]) + "\t");
                            }
                            else
                            {
                                wr.Write("\t");
                            }
                        }
                        //go to next line
                        wr.WriteLine();
                    }
                    //close file
                    wr.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
