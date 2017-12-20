using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppleDoor.Utilities
{
    class EmailUtilityHelper
    {
        private string encryptMailData(string mailItem)
        {
            string output = "";
            try
            {
                char[] readChar = mailItem.ToCharArray();

                for (int i = 0; i < readChar.Length; i++)
                {
                    if (!readChar[i].Equals('\t'))
                    {
                        int no = Convert.ToInt32(readChar[i]) + 10;
                        string r = Convert.ToChar(no).ToString();
                        output += r;
                    }
                    else
                    {
                        int no = Convert.ToInt32(readChar[i]);
                        string r = Convert.ToChar(no).ToString();
                        output += r;

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return output;

        }

        private string decryptMailData(String mailItem)
        {
            //decryption  
            string output = "";
            char[] readChar = mailItem.ToCharArray();
            for (int i = 0; i < readChar.Length; i++)
            {
                int no = Convert.ToInt32(readChar[i]) - 10;
                string r = Convert.ToChar(no).ToString();
                output += r;
            }
            return output;
        }

    }
}
