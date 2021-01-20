#Importing modules
import json
import requests
import os
import polyline as poly

#API key for TomTom
token=os.environ.get("Tomtom_API_Key")
#API key for Tollguru
Tolls_Key = os.environ.get("TollGuru_API_Key")

'''Fetching the geocodes from Tomtom'''
def get_geocode_from_tomtom(address):
    url = f"https://api.tomtom.com/search/2/geocode/{address}.JSON?key={token}&limit=1"
    latitude,longitude=requests.get(url).json()['results'][0]['position'].values()
    return (latitude,longitude)

'''Extracting Polyline from TOMTOM'''
def get_polyline_from_tomtom(source_latitude,source_longitude,destination_latitude,destination_longitude):
    #Query Tomtom with Key and Source-Destination coordinates
    url='https://api.tomtom.com/routing/1/calculateRoute/{a},{b}:{c},{d}/json?avoid=unpavedRoads&key={e}'.format(a=source_latitude,b=source_longitude,c=destination_latitude,d=destination_longitude,e=token)
    #converting the response to json
    response_from_tomtom=requests.get(url).json()
    #Tomtom does not provide polylines directly , it rather embeds Latitude and Longitude of each node in the route as dictionary key-value pair inside a list 
    #We thus are extracting the list of such dictionary
    coordinates_dict_list=response_from_tomtom['routes'][0]['legs'][0]['points']
    #We now must convert this list of dictionary to simple tuple or list iterables for "polyline.encode" to work
    coordinates_list=[(i['latitude'],i['longitude']) for i in coordinates_dict_list]
    #generating polyline from list of lat-lon pairs
    polyline_from_tomtom=poly.encode(coordinates_list)
    return(polyline_from_tomtom)


'''Calling Tollguru API'''
def get_rates_from_tollguru(polyline):
    #Tollguru querry url
    Tolls_URL = 'https://dev.tollguru.com/v1/calc/route'
   #Tollguru resquest parameters
    headers = {
                'Content-type': 'application/json',
                'x-api-key': Tolls_Key
                }
    params = {
                #Explore https://tollguru.com/developers/docs/ to get best of all the parameter that tollguru has to offer 
                'source': "tomtom",
                'polyline': polyline ,                      # this is the encoded polyline that we made     
                'vehicleType': '2AxlesAuto',                #'''Visit https://tollguru.com/developers/docs/#vehicle-types to know more options'''
                'departure_time' : "2021-01-05T09:46:08Z"   #'''Visit https://en.wikipedia.org/wiki/Unix_time to know the time format'''
                }
    #Requesting Tollguru with parameters
    response_tollguru= requests.post(Tolls_URL, json=params, headers=headers).json()
    #checking for errors or printing rates
    if str(response_tollguru).find('message')==-1:
        return(response_tollguru['route']['costs'])
    else:
        raise Exception(response_tollguru['message'])
                
'''Program Starts'''
#Step 1 :Getting Geocodes from Arcgis for Source and Destination
source_latitude,source_longitude=get_geocode_from_tomtom("Dallas, TX")               
destination_latitude,destination_longitude=get_geocode_from_tomtom("Newyork, NY")

#Step 2 : Get Polyline from Arcgis
polyline_from_tomtom=get_polyline_from_tomtom(source_latitude,source_longitude,destination_latitude,destination_longitude)

#Step 3 : Get rates from Tollguru
rates_from_tollguru=get_rates_from_tollguru(polyline_from_tomtom)

#Print the rates of all the available modes of payment
if rates_from_tollguru=={}:
    print("The route doesn't have tolls")
else:
    print(f"The rates are \n {rates_from_tollguru}")
'''Program Ends'''