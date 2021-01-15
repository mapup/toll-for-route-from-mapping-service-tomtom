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
            string TomTom_key="hokjoDegR1FCS7cBQOTPZYekcimP6zus";
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

            //int json_lenght = json.Lenght();
            string polyline ="";
            //json - enumera - lenght - under for - e
            //Console.WriteLine(json_length);


            /*IEnumerable<(double, double)> coordinates = new (double, double) [10000];
            int count = 10000;
            foreach(var value in json){
                coordinates[count]=(value['latitude'],value['longitude']);
                count+=1;
            }*/

            //ArrayList<String> latitude = new ArrayList();
            //ArrayList<String> longitude = new ArrayList();

            foreach(var value in json){

                double lat = value["latitude"];
                double l2 = value["longitude"];
                //latitude.Add(lat);
                //longitude.Add(l2);
                //double[] terms = new double [value.count];
                //Console.WriteLine(polyline)
                //IEnumerable<(double, double)> coordinates = new (double, double) [] { (value["latitude"], value["longitude"])};
	            IEnumerable<(double, double)> coordinates = new (double, double) [] { (lat, l2)};
	            string short_polyline = PolylineAlgorithm.Encode(coordinates);
                polyline += short_polyline;
                //Console.WriteLine(polyline);    
            }
            
            //Console.WriteLine(latitude);
            /*Double[] array = new Double[latitude.size()];
            for (int i = 0; i < latitude.size(); i++) 
            {
            ArrayList<String> row = latitude.get(i);
            array[i] = row.toArray(new Double[row.size()]);
            }
            Double[] array_lon = new Double[longitude.size()];
            for (int i = 0; i < longitude.size(); i++) 
            {
            ArrayList<String> row = longitude.get(i);
            array_lon[i] = row.toArray(new Double[row.size()]);
            }
            Console.WriteLine(array[0]);
            Console.WriteLine(array_lon[0]);*/

            //IEnumerable<(double, double)> coordinates = new (double, double) [] { (value["latitude"], value["longitude"])};
	        //IEnumerable<(double, double)> coordinates = new (double, double) [] { (latitude, longitude)};
	        //string short_polyline = PolylineAlgorithm.Encode(coordinates);
            //polyline += short_polyline;
            //Console.WriteLine(lat);
            //Console.WriteLine(polyline);
            //dynamic data = json.routes[0].legs[0].steps;
            //Console.WriteLine(json);
            //Console.WriteLine(json.routes[0].points);
            //Console.WriteLine(lat, l2);
            


            // foreach(var value in data){
            //    Console.WriteLine(value["longitude"]);
            // }
            // //Console.WriteLine(json.routes.points);
            
            //foreach(var in json.routes.points)
        string api_key2="J9L4QH37NQ7jqRQPND9fJPDHgJd8mptg";
        var client = new RestClient("https://dev.tollguru.com/v1/calc/route");
        var request_tollguru = new RestRequest(Method.POST);
        
        request_tollguru.AddHeader("content-type", "application/json");
        request_tollguru.AddHeader("x-api-key", api_key2);
        request_tollguru.AddParameter("application/json", "{\"source\":\"tomtom\" , \"polyline\":\""+polyline+"\" }", ParameterType.RequestBody);
        IRestResponse response_tollguru = client.Execute(request_tollguru);        
        var content = response_tollguru.Content;
        //string[] dump = content.Split("tag\":");
        Console.WriteLine(content);
        //string[] dump1 = dump[1].Split(",");
        //Cost variable contains the price 
        //string cost = dump1[0];
        //Console.WriteLine(content);

        }
    }
}


