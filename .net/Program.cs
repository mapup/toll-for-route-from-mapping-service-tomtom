using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Cloudikka.PolylineAlgorithm;
using System.Net;
using System.IO;
using RestSharp;
using System.Collections.Generic;
using System.Collections;


namespace sample
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //TomTom_Key
            string TomTom_key="TomTom_Key";
            //Location co-ordinates
            string source_longitude="-96.915920";
            string source_latitude="32.981910";
            string destination_longitude="-96.551600";
            string destination_latitude="32.854900";
            //concat url with location&TomTom_Key
            string url="https://api.tomtom.com/routing/1/calculateRoute/"+source_latitude+","+source_longitude+":"+destination_latitude+","+destination_longitude+"/json?avoid=unpavedRoads&key="+TomTom_key;
            //Console.WriteLine(url);
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            //Console.WriteLine(response);
            string responseFromServer;
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                responseFromServer = reader.ReadToEnd();  
            }
            response.Close();
            
            //Convert response into Json
            dynamic json  = JsonConvert.DeserializeObject(responseFromServer);
            //Get the latitude&longitude
            json = json.routes[0].legs[0].points;

            string polyline ="";
            foreach(var value in json){

                double lat = value["latitude"];
                double l2 = value["longitude"];
	        IEnumerable<(double, double)> coordinates = new (double, double) [] { (lat, l2)};
	        string short_polyline = PolylineAlgorithm.Encode(coordinates);
                polyline += short_polyline;
                //Console.WriteLine(polyline);    
            }
		
        string Toll_key="Toll_key";
        var client = new RestClient("https://dev.tollguru.com/v1/calc/route");
        var request_tollguru = new RestRequest(Method.POST);
        
        request_tollguru.AddHeader("content-type", "application/json");
        request_tollguru.AddHeader("x-api-key", Toll_key);
        request_tollguru.AddParameter("application/json", "{\"source\":\"tomtom\" , \"polyline\":\""+polyline+"\" }", ParameterType.RequestBody);
        IRestResponse response_tollguru = client.Execute(request_tollguru);        
        var content = response_tollguru.Content;
        string[] dump = content.Split("tag\":");
        //Console.WriteLine(content);
        string[] dump1 = dump[1].Split(",");
        //Cost variable contains the price 
        string cost = dump1[0];
        Console.WriteLine(cost);

        }
    }
}


