require 'HTTParty'
require 'json'

# Source Details in latitude-longitude pair
SOURCE = {longitude: -99.13205, latitude: 19.41510}
# Destination Details in latitude-longitude pair
DESTINATION = {longitude: -98.22568, latitude: 19.08060 }

# GET Request to Mapbox for Polyline
TOKEN = ENV['MAPBOX_KEY']
MAPBOX_URL = "https://api.mapbox.com/directions/v5/mapbox/driving/#{SOURCE[:longitude]},#{SOURCE[:latitude]};#{DESTINATION[:longitude]},#{DESTINATION[:latitude]}?geometries=polyline&access_token=#{TOKEN}&overview=full"
RESPONSE = HTTParty.get(MAPBOX_URL).body
json_parsed = JSON.parse(RESPONSE)

# Extracting mapbox polyline from JSON
mapbox_polyline = json_parsed['routes'].map { |x| x['geometry'] }.pop

# Sending POST request to TollGuru
TOLLGURU_URL = 'https://dev.tollguru.com/v1/calc/route'
TOLLGURU_KEY = ENV['TOLLGURU_KEY']
headers = {'content-type' => 'application/json', 'x-api-key' => TOLLGURU_KEY}
body = {'source' => "mapbox", 'polyline' => mapbox_polyline, 'vehicleType' => "2AxlesAuto", 'departure_time' => "2021-01-05T09:46:08Z"}
tollguru_response = HTTParty.post(TOLLGURU_URL,:body => body.to_json, :headers => headers)