require 'HTTParty'
require 'json'
require 'fast_polylines'

# Source Details in latitude-longitude pair
SOURCE = { longitude: '-96.7970', latitude: '32.7767'}
# Destination Details in latitude-longitude pair
DESTINATION = {longitude: '-74.0060',latitude: '40.7128' }

# GET Request to TomTom for Route Coordinates
KEY = ENV['TOMTOM_KEY']
TOMTOM_URL = "https://api.tomtom.com/routing/1/calculateRoute/#{SOURCE[:latitude]},#{SOURCE[:longitude]}:#{DESTINATION[:latitude]},#{DESTINATION[:longitude]}/json?avoid=unpavedRoads&key=#{KEY}"
RESPONSE = HTTParty.get(TOMTOM_URL).body
json_parsed = JSON.parse(RESPONSE)

# Extracting lat-long pairs from JSON and converting to polyline
tomtom_coordinates = json_parsed['routes'].map { |x| x['legs'] }.pop.map{ |y| y['points']}.pop.map {|z| z.values}
google_encoded_polyline = FastPolylines.encode(tomtom_coordinates)

# Sending POST request to TollGuru
TOLLGURU_URL = 'https://dev.tollguru.com/v1/calc/route'
TOLLGURU_KEY = ENV['TOLLGURU_KEY']
headers = {'content-type' => 'application/json', 'x-api-key' => TOLLGURU_KEY}
body = {'source' => "tomtom", 'polyline' => google_encoded_polyline, 'vehicleType' => "2AxlesAuto", 'departure_time' => "2021-01-05T09:46:08Z"}
tollguru_response = HTTParty.post(TOLLGURU_URL,:body => body.to_json, :headers => headers)