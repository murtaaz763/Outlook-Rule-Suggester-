
// This code requires the Nuget package Microsoft.AspNet.WebApi.Client to be installed.
// Instructions for doing this in Visual Studio:
// Tools -> Nuget Package Manager -> Package Manager Console
// Install-Package Microsoft.AspNet.WebApi.Client

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading;

namespace AppleDoor
{

    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }

    class ClusterAPI
    {
        static string[,] values { get; set; }

        static string[] columnNames;

        static string result;

        DataTable dataTable;

        static void Main(string[] args)
        {
      
        }


        public DataTable ReadTxtFromListOfMailIDs()
        {

            try
            { 
            //all the columns you want to pass on to the API to receive response from
            string str = "Sender,Receiver,Subject,Default assignment";

            //store all the column in array
            columnNames = str.Split(',');

            // Read each line of the file into a string array. Each element
            // of the array is one line of the file.
            string[] lines = System.IO.File.ReadAllLines(@"D:\RND\Outlook\AppleDoor\AppleDoor\listOfMailIDs.txt");

            //store all the rows in array, pass values variable as parameter to API
            values = new string[lines.Length, columnNames.Length];


            int rowCount = 0;
            foreach (string line in lines)
            {
                string[] rows = line.Split('\t');

                values[rowCount, 0] = rows[0];
                values[rowCount, 2] = rows[2];
                values[rowCount, 1] = rows[1];
                values[rowCount, 3] = "0";
                rowCount++;
            }

            //invoke method to call AML API
            InvokeRequestResponseService().Wait();
            Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return dataTable;

        }

        //create data table from the JSON object recieved from ML API       
        public DataTable ConvertJsonToDataTable(string json)
        {
            dataTable = new DataTable();
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
                   
                    for (int i =0; i< countOfRows; i++)
                    {
                        DataRow newRow = dataTable.NewRow();

                        newRow["Sender"]   = Convert.ToString(jsonLinq["Results"]["output1"]["value"]["Values"][i][0]);
                        newRow["Subject"]  = Convert.ToString(jsonLinq["Results"]["output1"]["value"]["Values"][i][1]);
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



        async Task InvokeRequestResponseService()

        {
            try
            {

                Thread.Sleep(2000);
                //create request object to call AML API, intialize with data read from the listOfMailIDs.txt

                using (var client = new HttpClient())
                {


                    var scoreRequest = new
                    {

                        Inputs = new Dictionary<string, StringTable>() {
                           {
                               "input1",
                               new StringTable()
                               {
                                   ColumnNames = columnNames,
                                   Values = values
                               }
                           },
                       },
                        GlobalParameters = new Dictionary<string, string>()
                        {
                        }
                    };



                    // Replace this with the API key for the web service
                    //string apiKey = Convert.ToString(ConfigurationManager.AppSettings["apiKey"]);
                    string apiKey = Convert.ToString("8wmo8y8FbmC2QI49SxUyhVNY+vyF+fHgnB6I61Q48LkWJZ0nPPHOozLY/Sb7hDaSHEskingXzxXGS0F+i6OAIw==");


                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                    // Replace this with the BaseAdress for the web service
                    client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/38d759ac457749f88921ebdd0c7ee6d5/services/7b79b45f01584049a54f9891aba91536/execute?api-version=2.0&details=true");

                    //client.BaseAddress = new Uri(Convert.ToString(ConfigurationManager.AppSettings["BaseAddress"]));

                    // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                    // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                    // For instance, replace code such as:
                    //      result = await DoSomeTask()
                    // with the following:
                    //      result = await DoSomeTask().ConfigureAwait(false)


                    //get response from the AML API
                    HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        //convert response from the AML API into String
                        result = await response.Content.ReadAsStringAsync();

                        //convert string response into Datatable
                        DataTable dataTable = ConvertJsonToDataTable(result);
                    }
                    else
                    {
                        Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                        // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                        Console.WriteLine(response.Headers.ToString());

                        string responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseContent);
                    }

                    //    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            


        }
    }
}
