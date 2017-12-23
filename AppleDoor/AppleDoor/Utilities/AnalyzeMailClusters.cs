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
using AppleDoor.Utilities;

namespace AppleDoor
{
    class AnalyzeMailClusters
    {

        AnalyzeMailClustersHelper AH = new AnalyzeMailClustersHelper();
        public static void main()
        {
        }

        public void analyseClusters(DataTable dataTable)
        {
           // AH.ExportToExcel(dataTable);
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
    
      }
}
