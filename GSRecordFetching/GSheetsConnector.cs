using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Net;
using System;

namespace GSheetsActions
{
    public class GSheetsConnector
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        private string[] Scopes = { SheetsService.Scope.Spreadsheets };

        public SheetsService Connect()
        {
            UserCredential credential;

            using (var stream = new FileStream(Properties.Settings.Default.ClientSecretFile, FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, Properties.Settings.Default.CredentialDir);

                ClientSecrets secrets = null;
                try
                {
                    secrets = GoogleClientSecrets.Load(stream).Secrets;
                }
                catch(Exception ex)
                {
                    throw new Exception("GoogleClientSecrets.Load(stream) error", ex);
                }

                try
                {
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        secrets, // GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }
                catch (Exception ex)
                {
                    throw new Exception("GoogleWebAuthorizationBroker.AuthorizeAsync() error", ex);
                }
                //LogWriter.Log("Credential file saved to: " + credPath);                    
            }

            // Create Google Sheets API service.
            BaseClientService.Initializer initializer;
            try
            {
                initializer = new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = Properties.Settings.Default.ApplicationName,
                };
            }
            catch (Exception ex)
            {
                throw new Exception("new BaseClientService.Initializer() error", ex);
            }

            SheetsService service;
            try
            {
                service = new SheetsService(initializer);
            }
            catch (Exception ex)
            {
                throw new Exception("new SheetsService() error", ex);
            }

            return service;
        }
    }
}
