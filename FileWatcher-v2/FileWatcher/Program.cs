using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace FileWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessTask();
        }

        /// <summary>
        /// Process the files based on the configuration
        /// </summary>
        private static void ProcessTask()
        {
            string[] files = Directory.GetFiles(GetConfiguration("WatchPath"), GetConfiguration("ProcessFileExtenstions"), SearchOption.AllDirectories);
            FileInfo fileInfo;

            foreach (string file in files)
            {
                fileInfo = new FileInfo(file);

                //File existance check and copy files to working folder
                //GetConfiguration("IsReplaceDestinationFile") == "true"
                if (!CheckFileExistance(fileInfo.FullName, fileInfo.Name))
                {
                    System.IO.File.Copy(file, GetConfiguration("DestinationPath") + "\\" + fileInfo.Name, GetConfiguration("IsReplaceDestinationFile") == "true");
                    FileProcessLog(fileInfo.FullName, fileInfo.Name, "Copied");
                }

                //delete processed file based on config
                if (GetConfiguration("IsDeleteProcessedFiles") == "true")
                {
                    System.IO.File.Delete(file);
                    FileProcessLog(fileInfo.FullName, fileInfo.Name, "Deleted from Source");
                }

                //Process file from working folder
                fileInfo = new FileInfo(GetConfiguration("DestinationPath") + "\\" + fileInfo.Name);

                //Process to service
                FileProcessLog(fileInfo.FullName, fileInfo.Name, "Integration");
                //Rename processed file

                //fileInfo.MoveTo(fileInfo.Name + "_Processed");

                System.IO.File.Move(fileInfo.FullName, fileInfo.FullName.Replace(fileInfo.Extension, "_Processed" + fileInfo.Extension));

                FileProcessLog(fileInfo.FullName, fileInfo.Name, "Completed");
            }

            ////Process file from working folder
            //string[] workingFiles = Directory.GetFiles(GetConfiguration("DestinationPath"));
            //foreach (string workingFile in workingFiles)
            //{
            //    fileInfo = new FileInfo(workingFile);

            //    //Process to service
            //    FileProcessLog(fileInfo.FullName, fileInfo.Name, "Integration");
            //    //Rename processed file

            //    //fileInfo.MoveTo(fileInfo.Name + "_Processed");

            //    System.IO.File.Move(fileInfo.FullName, fileInfo.FullName.Replace(fileInfo.Extension, "_Processed" + fileInfo.Extension));

            //    FileProcessLog(fileInfo.FullName, fileInfo.Name, "Completed");
            //}
        }

        /// <summary>
        /// Writes the activities of the file process and errors
        /// </summary>
        /// <param name="log"></param>
        private void LogWriter(string log)
        {

        }

        private static string GetConfiguration(string Key)
        {
            return ConfigurationManager.AppSettings[Key].ToString().Trim();
        }
        private static bool CheckFileExistance(string FullPath, string FileName)
        {
            // Get the subdirectories for the specified directory.'  
            bool IsFileExist = false;
            //////DirectoryInfo dir = new DirectoryInfo(FullPath);
            //////if (!dir.Exists)
            //////    IsFileExist = false;
            //////else
            //////{
            //////    string FileFullPath = Path.Combine(FullPath, FileName);
            //////    if (File.Exists(FileFullPath))
            //////        IsFileExist = true;
            //////}

            string FileFullPath = Path.Combine(FullPath, FileName);
            if (File.Exists(FileFullPath))
                IsFileExist = true;

            return IsFileExist;
        }

        /// <summary>
        /// Writes the activities of the file process
        /// </summary>
        /// <param name="FullPath"></param>
        /// <param name="FileName"></param>
        private static void FileProcessLog(string FullPath, string FileName, string status)
        {
            StreamWriter SW;
            if (!File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ProcessLog_" + DateTime.Now.ToString("yyyyMMdd") + ".txt")))
            {
                SW = File.CreateText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ProcessLog_" + DateTime.Now.ToString("yyyyMMdd") + ".txt"));
                SW.Close();
            }
            using (SW = File.AppendText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ProcessLog_" + DateTime.Now.ToString("yyyyMMdd") + ".txt")))
            {
                SW.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " File processed with Name: " + FileName + "("+ status + ")" + " at this location: " + FullPath);
                SW.Close();
            }
        }

        //public static void CreateServiceStoptextfile()
        //{
        //    string Destination = "D:\\articleimg\\FileWatcherWinService";
        //    StreamWriter SW;
        //    if (Directory.Exists(Destination))
        //    {
        //        Destination = System.IO.Path.Combine(Destination, "txtServiceStop_" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
        //        if (!File.Exists(Destination))
        //        {
        //            SW = File.CreateText(Destination);
        //            SW.Close();
        //        }
        //    }
        //    using (SW = File.AppendText(Destination))
        //    {
        //        SW.Write("\r\n\n");
        //        SW.WriteLine("Service Stopped at: " + DateTime.Now.ToString("dd-MM-yyyy H:mm:ss"));
        //        SW.Close();
        //    }
        //}
    }
}
