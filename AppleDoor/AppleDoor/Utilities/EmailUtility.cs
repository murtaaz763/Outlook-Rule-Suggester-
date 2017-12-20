using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.IO;
using System.Diagnostics;

namespace AppleDoor
{
    class EmailUtility
    {
        static List<string> listOfMailIDs;

        public void ScanAllMailItems()
        {
            listOfMailIDs = new List<string>();
            int count = 0;
            string mailItem = string.Empty;
            Outlook.MAPIFolder inbox = Globals.ThisAddIn.Application.Session.DefaultStore.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);

            Outlook.Recipient recip;
            Outlook.ExchangeUser exUser;
            string sAddress;
            string reciepient;
            Outlook.Attachments attachments = null;


            foreach (object item in inbox.Items)
            {
                Outlook.MailItem mail = item as Outlook.MailItem;

                if (mail != null && mail.Subject != null)
                {
                    try
                    {
                        mailItem = mail.Subject;
                        recip = Globals.ThisAddIn.Application.GetNamespace("MAPI").CreateRecipient(mail.SenderEmailAddress);
                        exUser = recip.AddressEntry.GetExchangeUser();
                        sAddress = Convert.ToString(exUser.PrimarySmtpAddress);

                        reciepient = GetSMTPAddressForRecipients(mail);
                        mailItem = sAddress + "\t" + mail.Subject + "\t" + reciepient;
                        listOfMailIDs.Add(mailItem);

                        //  attachments = mail.Attachments;

                        //  listOfMailIDs.Add(encryptMailData(mailItem));
                        count++;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }
            }
            saveTicketID(listOfMailIDs);
        }

        private String GetSMTPAddressForRecipients(Outlook.MailItem mail)
        {
            string smtpAddress = string.Empty;
            try
            {

                const string PR_SMTP_ADDRESS =
                    "http://schemas.microsoft.com/mapi/proptag/0x39FE001E";
                Outlook.Recipients recips = mail.Recipients;
                foreach (Outlook.Recipient recip in recips)
                {
                    Outlook.PropertyAccessor pa = recip.PropertyAccessor;
                    smtpAddress = smtpAddress + " " + pa.GetProperty(PR_SMTP_ADDRESS).ToString();
                }
                smtpAddress = smtpAddress.Trim();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return (smtpAddress);
        }


        public static void saveTicketID(List<string> listOfTickets)
        {
            String filePath = string.Copy(@"D:\RND\Outlook\AppleDoor\AppleDoor\listOfMailIDs.txt");

            System.Diagnostics.EventLog eventLogger = new EventLog("Application");

            try
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Create);
                StreamWriter streamWriter = new StreamWriter(fileStream);
                try
                {

                    foreach (string TicketID in listOfTickets)
                    {
                        streamWriter.WriteLine(TicketID);
                    }

                }
                catch (ArgumentNullException exception)
                {
                    eventLogger.WriteEntry(exception.Message + ". Stack Trace:" + exception.StackTrace, EventLogEntryType.Error);
                }
                finally
                {
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
            catch (System.IO.IOException exception)
            {
                eventLogger.WriteEntry(exception.Message + ". Stack Trace:" + exception.StackTrace + " Could not access TicketIDs file!", EventLogEntryType.Error);
            }
            catch (Exception exception)
            {
                eventLogger.WriteEntry(exception.Message + ". Stack Trace:" + exception.StackTrace, EventLogEntryType.Error);
            }
        }

       

    }
}
