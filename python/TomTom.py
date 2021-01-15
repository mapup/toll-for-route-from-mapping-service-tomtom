#Importing modules
import json
import requests
import os
import polyline as poly


'''Fetching Polyline from Tomtom'''

#API key for TomTom
token=os.environ.get("Tomtom_API_Key")

#Source and Destination Coordinates
#Dallas, TX
source_longitude='-96.7970'
source_latitude='32.7767'
#New York, NY
destination_longitude='-74.0060'
destination_latitude='40.7128'

#Query Tomtom with Key and Source-Destination coordinates
url='https://api.tomtom.com/routing/1/calculateRoute/{a},{b}:{c},{d}/json?avoid=unpavedRoads&key={e}'.format(a=source_latitude,b=source_longitude,c=destination_latitude,d=destination_longitude,e=token)

#converting the response to json
response_from_tomtom=requests.get(url).json()

#Tomtom oes not provide polylines directly , it rather embeds Latitude and Longitude of each node in the route as dictionary key-value pair inside a list 
#We thus are extracting the list of such dictionary
coordinates_dict_list=response_from_tomtom['routes'][0]['legs'][0]['points']
#We now must convert this list of dictionary to simple tuple or list iterables for "polyline.encode" to work
coordinates_list=[(i['latitude'],i['longitude']) for i in coordinates_dict_list]
#generating polyline from list of lat-lon pairs
polyline=poly.encode(coordinates_list)



'''Calling Tollguru API'''

#API key for Tollguru
Tolls_Key = os.environ.get("TollGuru_API_Key")

#Tollguru querry url
Tolls_URL = 'https://dev.tollguru.com/v1/calc/route'

#Tollguru resquest parameters
headers = {
            'Content-type': 'application/json',
            'x-api-key': Tolls_Key
          }
params = {
            'source': "tomtom",
            'polyline': polyline ,               
            'vehicleType': '2AxlesAuto',                #'''TODO - Need to users list of acceptable values for vehicle type'''
            'departure_time' : "2021-01-05T09:46:08Z"   #'''TODO - Specify time formats'''
        }

#Requesting Tollguru with parameters
response_tollguru= requests.post(Tolls_URL, json=params, headers=headers).json()

#checking for errors or printing rates
if str(response_tollguru).find('message')==-1:
    print('\n The Rates Are ')
    #extracting rates from Tollguru response is no error
    print(*response_tollguru['route']['costs'].items(),end="\n\n")
else:
    raise Exception(response_tollguru['message'])