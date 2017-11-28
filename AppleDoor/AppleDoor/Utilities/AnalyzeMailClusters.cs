using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using System.IO;

namespace AppleDoor
{
    class AnalyzeMailClusters
    {
        public static void main()
        {
        }

        public void analyseClusters(DataTable dataTable)
        {
            // ExportToExcel(dataTable);
            AnalyzeClustersBasedOnSender(dataTable);
        }

        public void AnalyzeClustersBasedOnSender(DataTable dataTable)
        {
            int numberOfClusters = 10;
            int columnIndexOfAssignmentColumn = 3;
            int biggestCluster = 1;
            int[] numberOfMailsInEachCluster = new int[numberOfClusters];

            //assign number of items present in each cluster bucket in the array
            for (int i = 0; i < (dataTable.Rows.Count); i++)
            {
                numberOfMailsInEachCluster[Convert.ToInt16(dataTable.Rows[i][columnIndexOfAssignmentColumn])]++;
            }

            //find out biggest cluster apart from 0th cluster
            for (int i = 1; i < numberOfClusters; i++)
            {
                if (numberOfMailsInEachCluster[biggestCluster] < numberOfMailsInEachCluster[i])
                {
                    biggestCluster = i;
                }
            }


            //count occurence of each sender
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (Convert.ToInt16(dataTable.Rows[i][columnIndexOfAssignmentColumn]) == biggestCluster)
                {
                    if (dictionary.ContainsKey(dataTable.Rows[i][0].ToString()))
                    {
                        dictionary[dataTable.Rows[i][0].ToString()] += 1;
                    }
                    else
                    {
                        dictionary.Add(dataTable.Rows[i][0].ToString(), 1);
                    }
                }
            }

            //Suggest rules based on senders count
            OutlookRulesUtility.suggestRulesBasedOnCountOfSender(dictionary);

        }


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
