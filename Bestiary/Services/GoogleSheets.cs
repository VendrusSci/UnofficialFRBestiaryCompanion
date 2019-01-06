using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Bestiary.Services
{
    class GoogleSheets
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "Unofficial Bestiary Companion";
        private SheetsService m_Service;
        private string m_SpreadsheetID;
        private static string range = "A:F";

        void InitService(string[] args)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            m_Service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public string GetSpreadsheet()
        {
            string sheetID = null;
            //check for existing settings item with spreadsheet ID
            if(sheetID == null)
            {
                //request url
                string url = null;
                var firstSplit = url.Split("/d/".ToCharArray());
                var id = firstSplit[1].Split('/')[0];
            }
            //if exists, return ID
            //if none, ask for link to spreadsheet
            return sheetID;
        }

        public string ReadData()
        {
            SpreadsheetsResource.ValuesResource.GetRequest request =
                m_Service.Spreadsheets.Values.Get(m_SpreadsheetID, range);
            ValueRange response = request.Execute();
            return convertRangeToCsvString(response);
        }

        public string convertRangeToCsvString(ValueRange values)
        {
            if(values.Values.Count > 1)
            {
                string csv = "";
                for(int row = 0; row < values.Values.Count; row++)
                {
                    for(int col = 0; col < values.Values[row].Count; col++)
                    {
                        if(values.Values[row][col].ToString().IndexOf(",") != -1)
                        {
                            values.Values[row][col] = "\"" + values.Values[row][col] + "\"";
                        }
                    }
                    if(row < values.Values.Count-1)
                    {
                        csv += String.Join(",", values.Values[row]) + "\r\n";
                    }
                }
            }
            return null;
        }

        public void uploadCsv(string csv)
        {
            //clear sheet
            SpreadsheetsResource.ValuesResource.ClearRequest request = m_Service.Spreadsheets.Values.Clear(new ClearValuesRequest(), m_SpreadsheetID, range);
            ClearValuesResponse response = request.Execute();
            //upload data
            List<ValueRange> data = new List<ValueRange>();
            data.Add(convertCsvStringToRange(csv));

            BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest();
            requestBody.ValueInputOption = "";
            requestBody.Data = data;
        }

        public ValueRange convertCsvStringToRange(string csv)
        {
            ValueRange data = new ValueRange();
            var lines = csv.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(var line in lines)
            {
                var row = new List<object>();
                var rowData = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(var column in rowData)
                {
                    row.Add(column);
                }
                data.Values.Add(row);
            }
            return data;
        }
    }
}
