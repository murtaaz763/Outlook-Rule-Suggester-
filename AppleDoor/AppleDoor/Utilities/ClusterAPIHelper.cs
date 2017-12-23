using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppleDoor.Utilities
{
    class ClusterAPIHelper
    {

        public DataTable ConvertJsonToDataTable(string json)
        {
            DataTable dataTable = new DataTable();

            //create data table from the JSON object recieved from ML API  
            try
            {
                var jsonLinq = JObject.Parse(json);

                int countOfRows = Convert.ToInt32(((Newtonsoft.Json.Linq.JContainer)jsonLinq["Results"]["output1"]["value"]["Values"]).Count);

                if (jsonLinq != null)
                {
                    //add whatever columns you want to read from API
                    dataTable.Columns.Add("Sender", typeof(string));
                    dataTable.Columns.Add("Receiver", typeof(string));
                    dataTable.Columns.Add("Subject", typeof(string));
                    dataTable.Columns.Add("Assignments", typeof(int));

                    for (int i = 0; i < countOfRows; i++)
                    {
                        DataRow newRow = dataTable.NewRow();

                        newRow["Sender"] = Convert.ToString(jsonLinq["Results"]["output1"]["value"]["Values"][i][0]);
                        newRow["Subject"] = Convert.ToString(jsonLinq["Results"]["output1"]["value"]["Values"][i][1]);
                        newRow["Receiver"] = Convert.ToString(jsonLinq["Results"]["output1"]["value"]["Values"][i][2]);
                        newRow["Assignments"] = Convert.ToInt32(jsonLinq["Results"]["output1"]["value"]["Values"][i][4]);

                        dataTable.Rows.Add(newRow);
                    }

                }

            }
            catch (Exception ex)
            {

            }

            //return response from the AML API along with Input parameters
            return dataTable;
        }

    }
}
