using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;


namespace ftpTest
{
    class Program
    {
        static void Main(string[] args)
        {
            /*string path = @"C:\EIS\Documents\docs\Learning\c#\ftpTest\ftpTest\bin\Debug\Test.txt";*/
            string path = ConfigurationManager.AppSettings["SITE"];
            string targetFolder = ConfigurationManager.AppSettings["INPUT_Q"];
           /* string des = "ftp://129.230.34.26/FTP_LAO_Scale.txt";*/
            string des = ConfigurationManager.AppSettings["FTP_FOLDER"];
            string userName = ConfigurationManager.AppSettings["FTP_USER"];
            string password = ConfigurationManager.AppSettings["FTP_PWD"];

            //timestamp

            string fileOriginal = "OCB";
            string result = fileOriginal+ DateTime.Now.ToString("_yyyyMMdd_HHmmss")+ ".txt"  ;
            path += result;
            targetFolder += result;
            des += result;

            //Create file
            try
            {

                // Delete the file if it exists.
                if (File.Exists(path))
                {
                    // Note that no lock is put on the
                    // file and the possibility exists
                    // that another process could do
                    // something with it between
                    // the calls to Exists and Delete.
                    File.Delete(path);
                }

                // Create the file.
                using (FileStream fs = File.Create(path))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes("Test on config");
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }

                // Open the stream and read it back.
                using (StreamReader sr = File.OpenText(path))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

           

            //FTP code
            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(des);
            request.Method = WebRequestMethods.Ftp.UploadFile;

           

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(userName, password);

            // Copy the contents of the file to the request stream.
            byte[] fileContents;
            using (StreamReader sourceStream = new StreamReader(result)) 
            {
                fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            }

            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            }


            //Move file to another folder
            try
            {
                

                // Ensure that the target does not exist.
                if (File.Exists(targetFolder))
                    File.Delete(targetFolder);

                // Move the file.
                File.Move(path, targetFolder);
                Console.WriteLine("{0} was moved to {1}.", path, targetFolder);

                // See if the original exists now.
                if (File.Exists(path))
                {
                    Console.WriteLine("The original file still exists, which is unexpected.");
                }
                else
                {
                    Console.WriteLine("The original file no longer exists, which is expected.");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }
      
    }
}
