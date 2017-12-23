using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace AppleDoor.Utilities
{
    class OutlookRulesUtilityHelper
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

     }
}
