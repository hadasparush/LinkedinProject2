using System;
using System.IO;
using System.Net;

namespace Linkedin.Models
{
    public class HttpRequestService   
    {

        public string getResponse(string url)
        {
            StreamReader reader = null;
            HttpWebResponse response = null;
            string responseFromServer = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;
                 response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                    Console.WriteLine(String.Format(
                    "Error while getting response Server error (HTTP {0}: {1} ).",
                    response.StatusCode,
                    response.StatusDescription));

                Stream dataStream = response.GetResponseStream();
                 reader = new StreamReader(dataStream);
              
                responseFromServer = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format(
                    "Error while reading response Server error (HTTP {0}: {1} ).",
                    response.StatusCode,
                    response.StatusDescription));
                throw ex;


            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return responseFromServer;
        }

        
      
    }
}