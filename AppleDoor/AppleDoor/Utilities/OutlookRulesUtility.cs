using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;


using System.Xml.Linq;

using System.Diagnostics;
using System.IO;
using AppleDoor;

namespace AppleDoor
{
    class OutlookRulesUtility
    {

        public static bool RuleExist(string ruleName, Outlook.Rules rules)
        {
            foreach (Outlook.Rule rule in rules)
            {
                if (rule.Name == ruleName)
                {
                    return true;
                }
            }
            return false;
        }

        public static void suggestSimpleRulesBasedOnCountOfSender(Dictionary<string, int> dictionary)
        {

        }

        public static void suggestRulesBasedOnCountOfSender(Dictionary<string, int> dictionary)
        {
            Console.WriteLine("hi");

            Outlook.NameSpace session = null;
            Outlook.Store store = null;
            Outlook.Rules rules = null;
            Outlook.MAPIFolder destinationFolder = null;
            Outlook.MAPIFolder rootFolder = null;
            Outlook.Folders rootFolderFolders = null;

            Outlook.Rule rule = null;
            Outlook.RuleConditions ruleConditions = null;
            Outlook.TextRuleCondition subjectTextRuleCondition = null;

            Outlook.RuleActions ruleActions = null;
            Outlook.MoveOrCopyRuleAction moveRuleAction = null;

            string ruleName = string.Empty;

            try
            {
                ruleName = "Move Order Mails Rule";
                //session = Application.Session; // have to correct this error, it was working in 10102017 build when everything was in ThisAddin.cs
                store = session.DefaultStore;
                rules = store.GetRules();

                if (!RuleExist(ruleName, rules))
                {
                    rootFolder = store.GetRootFolder();
                    // destinationFolder = GetFolder(rootFolder.FolderPath + "\\Orders", this);


                    if (destinationFolder == null)
                    {
                        rootFolderFolders = rootFolder.Folders;
                        destinationFolder = rootFolderFolders.Add("Orders");
                    }

                    rule = rules.Create(ruleName, Outlook.OlRuleType.olRuleReceive);
                    ruleConditions = rule.Conditions;

                    subjectTextRuleCondition = ruleConditions.Subject;
                    subjectTextRuleCondition.Text = new string[]
                        { "Orders", "orders", "Order", "order" };
                    subjectTextRuleCondition.Enabled = true;

                    ruleActions = rule.Actions;
                    moveRuleAction = ruleActions.MoveToFolder;
                    moveRuleAction.Folder = destinationFolder;
                    moveRuleAction.Enabled = true;

                    ruleActions.DesktopAlert.Enabled = true;

                    rules.Save(true);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
            }
        }
       
    }
}
