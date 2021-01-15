require 'HTTParty'
require 'json'
require 'fast_polylines'
require 'cgi'

KEY = ENV['TOMTOM_KEY']

def get_coord_hash(loc)
    geocoding_url = "https://api.tomtom.com/search/2/geocode/#{CGI::escape(loc)}.JSON?key=#{KEY}&limit=1"
    coord = JSON.parse(HTTParty.get(geocoding_url).body)
    return (coord['results'].pop)['position']
end

# Source Details in latitude-longitude pair using geocoding API
SOURCE = get_coord_hash("Dallas, TX")

# Destination Details in latitude-longitude pair using geocoding API
DESTINATION = get_coord_hash("New York, NY")

# GET Request to TomTom for Route Coordinates
TOMTOM_URL = "https://api.tomtom.com/routing/1/calculateRoute/#{SOURCE["lat"]},#{SOURCE["lon"]}:#{DESTINATION["lat"]},#{DESTINATION["lon"]}/json?avoid=unpavedRoads&key=#{KEY}"
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