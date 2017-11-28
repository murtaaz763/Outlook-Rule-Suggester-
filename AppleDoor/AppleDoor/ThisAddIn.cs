using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Diagnostics;
using System.IO;
using AppleDoor;
using Newtonsoft.Json;
using System.Data;


namespace AppleDoor
{
    public partial class ThisAddIn
    {
        Outlook.NameSpace outlookNameSpace;
        Outlook.MAPIFolder inbox;
        Outlook.Items items;

       
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
        //    EmailUtility emailUtility = new EmailUtility();
        //    emailUtility.ScanAllMailItems();
          
          ClusterAPI API = new ClusterAPI();

            DataTable dataTable= API.ReadTxtFromListOfMailIDs();


            AnalyzeMailClusters analyzeMailClusters = new AnalyzeMailClusters();

            analyzeMailClusters.analyseClusters(dataTable);

              outlookNameSpace = this.Application.GetNamespace("MAPI");
            inbox = outlookNameSpace.GetDefaultFolder(
                    Microsoft.Office.Interop.Outlook.
                    OlDefaultFolders.olFolderInbox);

            items = inbox.Items;
            items.ItemAdd +=
                new Outlook.ItemsEvents_ItemAddEventHandler(OutlookRulesUtility.suggestRule);

        }

      
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see http://go.microsoft.com/fwlink/?LinkId=506785
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
