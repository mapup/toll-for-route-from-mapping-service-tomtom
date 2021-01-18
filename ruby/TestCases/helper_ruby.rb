require 'HTTParty'
require 'json'
require 'fast_polylines'
require 'cgi'

$key = ENV['TOMTOM_KEY']

def get_toll_rate(source,destination)

    def get_coord_hash(loc)
        geocoding_url = "https://api.tomtom.com/search/2/geocode/#{CGI::escape(loc)}.JSON?key=#{$key}&limit=1"
        coord = JSON.parse(HTTParty.get(geocoding_url).body)
        return (coord['results'].pop)['position']
    end

    # Source Details in latitude-longitude pair using geocoding API
    source = get_coord_hash(source)

    # Destination Details in latitude-longitude pair using geocoding API
    destination = get_coord_hash(destination)

    # GET Request to TomTom for Route Coordinates
    tomtom_url = "https://api.tomtom.com/routing/1/calculateRoute/#{source["lat"]},#{source["lon"]}:#{destination["lat"]},#{destination["lon"]}/json?avoid=unpavedRoads&key=#{$key}"
    response = HTTParty.get(tomtom_url).body
    json_parsed = JSON.parse(response)

    # Extracting lat-long pairs from JSON and converting to polyline
    tomtom_coordinates = json_parsed['routes'].map { |x| x['legs'] }.pop.map{ |y| y['points']}.pop.map {|z| z.values}
    google_encoded_polyline = FastPolylines.encode(tomtom_coordinates)

    # Sending POST request to TollGuru
    tollguru_url = 'https://dev.tollguru.com/v1/calc/route'
    tollguru_key = ENV['TOLLGURU_KEY']
    headers = {'content-type' => 'application/json', 'x-api-key' => tollguru_key}
    body = {'source' => "tomtom", 'polyline' => google_encoded_polyline, 'vehicleType' => "2AxlesAuto", 'departure_time' => "2021-01-05T09:46:08Z"}
    tollguru_response = HTTParty.post(tollguru_url,:body => body.to_json, :headers => headers)
    begin
        toll_body = JSON.parse(tollguru_response.body)    
        if toll_body["route"]["hasTolls"] == true
            return google_encoded_polyline,toll_body["route"]["costs"]["tag"], toll_body["route"]["costs"]["cash"] 
        else
            raise "No tolls encountered in this route"
        end
    rescue Exception => e
        puts e.message 
    end
end