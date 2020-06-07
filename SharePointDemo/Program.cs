using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace SharePointDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //UploadFile();

            //here provide the sitepoint url,path
            DownloadFilesFromSharePoint("http://sp.frk.com/teams/ResearchSystemsSupport/GT%20Resources/Forms/AllItems.aspx", "/Documents", @"c:\");
        }


        public static void UploadFile()
        {


            String FilePath = @"C:\Project\Sample.txt";
            string Libirary = "Documents";
            string SiteUrl = "";
            string FileName = FilePath.Substring(FilePath.LastIndexOf("\\") + 1);

            using (ClientContext ctx=new ClientContext(SiteUrl))
            {
                FileCreationInformation fileCreation = new FileCreationInformation();
                fileCreation.Url = FileName;
                fileCreation.Overwrite = true;
                fileCreation.Content = System.IO.File.ReadAllBytes(FilePath);
                Web MyLibirary = ctx.Web;
                List list = MyLibirary.Lists.GetByTitle(Libirary);
                MyLibirary.RootFolder.Files.Add(fileCreation);
                ctx.ExecuteQuery();
            }
        }

        static void DownloadFilesFromSharePoint(string siteUrl, string siteFolderPath, string localTempLocation)
        {
            ClientContext ctx = new ClientContext(siteUrl);
            ctx.Credentials = new NetworkCredential("username", "password", "Domain");

            FileCollection files = ctx.Web.GetFolderByServerRelativeUrl(siteFolderPath).Files;

            ctx.Load(files);
            if (ctx.HasPendingRequest)
            {
                ctx.ExecuteQuery();
            }

            foreach (Microsoft.SharePoint.Client.File file in files)
            {
                FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(ctx, file.ServerRelativeUrl);
                ctx.ExecuteQuery();

                var filePath = localTempLocation + "\\" + file.Name;
                System.IO.FileStream fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite);

                fileInfo.Stream.CopyTo(fileStream);
                

            }
        }
 
    }

  
}
