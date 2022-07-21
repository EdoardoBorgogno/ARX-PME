using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARX_PME_Updater.Logger
{
    /// <summary>
    /// Handler logger file.
    /// </summary>
    internal class LogHandler
    {
        /// <summary>
        /// Describe message. 
        /// </summary>
        internal class Message
        {
            //Fields
            private string messageTitle;
            private string messageContent;
            private DateTime messageDateTime;

            //Properties
            public string MessageTitle { get => messageTitle; set => messageTitle = value; }
            public string MessageContent { get => messageContent; set => messageContent = value; }
            public DateTime MessageDateTime { get => messageDateTime; set => messageDateTime = value; }

            //Methods

            /// <summary>
            /// Create string to write on logger file.
            /// </summary>
            /// <returns></returns>
            public string WriteMessage()
            {
                string message = string.Empty;

                message += string.Concat(Enumerable.Repeat("--", 20));
                message += "\n\nINFO \nDATE: " + messageDateTime.ToString() + "\n\n";
                message += messageTitle != "" ? messageTitle.ToUpper() + "\n\n" : "";
                message += messageContent != "" ? messageContent + "\n\n" : "";
                message += string.Concat(Enumerable.Repeat("--", 20));
                message += "\n\n";

                return message;
            }
        }

        // Contains message to write
        private static Queue<Message> messages = new Queue<Message>();

        //Constructor
        static LogHandler()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(writeMessages!);
        }

        /// <summary>
        /// Write message to log.txt 
        /// </summary>
        /// <param name="message">Message Object</param>
        public static void addMessage(Message message)
        {
            messages.Enqueue(message); //==> add message to queue
        }

        /// <summary>
        /// On program close, write all messages to log.txt
        /// </summary>
        public static void writeMessages(object sender, EventArgs e)
        {
            //path
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                       "ARX-PME", "Activities", "Update", "log.txt");

            while (messages.Count > 0)
                if(!IsFileLocked(new FileInfo(path)))
                {
                    Message message = messages.Dequeue(); //==> get message from queue
                    string messageToWrite = message.WriteMessage(); //==> create string to write on log.txt
                    writeLog(messageToWrite); //==> write message on log.txt
                }
        }

        /// <summary>
        /// Write message in ProgramData/Activities/Update/log.txt
        /// </summary>
        /// <param name="message"></param>
        private static void writeLog(string message)
        {
            //path
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                       "ARX-PME", "Activities", "Update", "log.txt");

            //If file or directory doesn't exist, create it
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            }

            //Write message on log.txt
            File.AppendAllText(path, message);
        }

        /// <summary>
        /// Check if file is already open in another process
        /// </summary>
        private static bool IsFileLocked(FileInfo file)
        {
            if (!file.Exists)
                return false; 

            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }
    }
}
