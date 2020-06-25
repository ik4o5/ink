// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// <imports>
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
// </imports>

namespace RecognizeInk
{
    class Program
    {
        // <vars>
        // Add your Ink Recognizer subscription key to your environment variables.
        static readonly string subscriptionKey = Environment.GetEnvironmentVariable("f18cbd4adce041a7aa1740813ed0d68a");
        
        // Add your Ink Recognizer endpoint to your environment variables.
        // For example: <your-custom-subdomain>.cognitiveservices.azure.com
        static readonly string endpoint = Environment.GetEnvironmentVariable("https://inkuruk.cognitiveservices.azure.com/");
        static readonly string inkRecognitionUrl = "/inkrecognizer/v1.0-preview/recognize";

        // Replace the dataPath string with a path to the JSON formatted ink stroke data.
        // Optionally, use the example-ink-strokes.json file of this sample. Add to your bin\Debug\netcoreapp3.0 project folder.
        static readonly string dataPath = @"PATH_TO_INK_STROKE_DATA";
        // </vars>
        // <request>
        static async Task<string> Request(string apiAddress, string endpoint, string subscriptionKey, string requestData)
        {

            using (HttpClient client = new HttpClient { BaseAddress = new Uri(apiAddress) })
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                var content = new StringContent(requestData, Encoding.UTF8, "application/json");
                var res = await client.PutAsync(endpoint, content);
                if (res.IsSuccessStatusCode)
                {
                    return await res.Content.ReadAsStringAsync();
                }
                else
                {
                    return $"ErrorCode: {res.StatusCode}";
                }
            }
        }
        // </request>

        // <recognize>
        static void recognizeInk(string requestData)
        {

            //construct the request
            var result = Request(
                endpoint,
                inkRecognitionUrl,
                subscriptionKey,
                requestData).Result;

            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
            System.Console.WriteLine(jsonObj);
        }
        // </recognize>
        // <loadJson>
        public static JObject LoadJson(string fileLocation)
        {
            var jsonObj = new JObject();

            using (StreamReader file = File.OpenText(fileLocation))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                jsonObj = (JObject)JToken.ReadFrom(reader);
            }
            return jsonObj;
        }
        // </loadJson> 
        // <main>   
        static void Main(string[] args)
        {

            var requestData = LoadJson(dataPath);
            string requestString = requestData.ToString(Newtonsoft.Json.Formatting.None);
            recognizeInk(requestString);
            System.Console.WriteLine("\nPress any key to exit ");
            System.Console.ReadKey();
        }
        // </main>
    }
}
