using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.SharePoint.Client;
using System.IO;

namespace SharePointDemo
{
    public class Class1
    {
        void Method()
        {

            var webUrl = "https://yourtenantgoeshere.sharepoint.com/site/yoursitename";
            var username = "yourusernamegoeshere@yourtenantgoeshere.com";
            var password = "pleasedonteverwriteyourpasswordincode";
            var listTitle = "yourdocumentlibrarytitle";
            var destinationFolder = @"C:temp";

            var securePassword = new SecureString();
            //Convert string to secure string
            foreach (char c in password)
            {
                securePassword.AppendChar(c);
            }
            securePassword.MakeReadOnly();

            using (var context = new ClientContext(webUrl))
            {
                // Connect using credentials -- use the approach that suits you
                context.Credentials = new SharePointOnlineCredentials(username, securePassword);

                // Get a reference to the SharePoint site
                var web = context.Web;

                // Get a reference to the document library
                var list = context.Web.Lists.GetByTitle(listTitle);

                // Get the list of files you want to export. I'm using a query
                // to find all files where the "Status" column is marked as "Approved"
                var camlQuery = new CamlQuery
                {
                    ViewXml = @"
            Approved
            1000
        "
                };

                // Retrieve the items matching the query
                var items = list.GetItems(camlQuery);

                // Make sure to load the File in the context otherwise you won't go far
                context.Load(items, items2 => items2.IncludeWithDefaultProperties
                    (item => item.DisplayName, item => item.File));

                // Execute the query and actually populate the results
                context.ExecuteQuery();

                // Iterate through every file returned and save them
                foreach (var item in items)
                {
                   
                    using (FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(context, item.File.ServerRelativeUrl))
                    {
                        // Combine destination folder with filename -- don't concatenate
                        // it's just wrong!
                        var filePath = Path.Combine(destinationFolder, item.File.Name);

                        // Erase existing files, cause that's how I roll
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }

                        // Create the file
                        using (var fileStream = System.IO.File.Create(filePath))
                        {
                            fileInfo.Stream.CopyTo(fileStream);
                        }
                    }
                }
            }
        }
    }

   
}
