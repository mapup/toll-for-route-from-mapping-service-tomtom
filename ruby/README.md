# [](https://developer.tomtom.com/)

### Get API key to access TomTom APIs (if you have an API key skip this)
#### Step 1: Login/Singup
* Create an account to access [TomTom Developer Portal](https://developer.tomtom.com/)
* go to signup/login link https://developer.tomtom.com/user/login
* you will need to agree to TomTom's Terms of Service https://developer.tomtom.com/terms-and-conditions

#### Step 2: Getting you Key
* Login to your TomTom Developer Portal
* You can find you key at https://developer.tomtom.com/user/me/apps
* If you want you can create additional keys as per different
  applications

With this in place, make a GET request: https://api.tomtom.com/routing/1/calculateRoute/#{SOURCE[:latitude]},#{SOURCE[:longitude]}:#{DESTINATION[:latitude]},#{DESTINATION[:longitude]}/json?avoid=unpavedRoads&key=#{KEY}

### Note:
* TomTom accepts source and destination, as `:` seperated `{:longitude,:latitude}`. To convert Source string to lat long pairs we use geocoding API
* TomTom doesn't return us route as a polyline, but as an array of `[ latitude, longitude ]`, we need to convert this to a polyline

```ruby
require 'HTTParty'
require 'json'
require 'fast_polylines'
require 'cgi'

KEY = ENV['TOMTOM_KEY']

def get_coord_hash(loc)
    # Geocoding API requires URL encoded string as parameter
    geocoding_url = "https://api.tomtom.com/search/2/geocode/#{CGI::escape(loc)}.JSON?key=#{KEY}&limit=1"
    coord = JSON.parse(HTTParty.get(geocoding_url).body)
    return (coord['results'].pop)['position']
end

# Source Details in latitude-longitude pair
SOURCE = get_coord_hash("Dallas, TX")

# Destination Details in latitude-longitude pair
DESTINATION = get_coord_hash("New York, NY")

# GET Request to TomTom for Route Coordinates
TOMTOM_URL = "https://api.tomtom.com/routing/1/calculateRoute/#{SOURCE["lat"]},#{SOURCE["lon"]}:#{DESTINATION["lat"]},#{DESTINATION["lon"]}/json?avoid=unpavedRoads&key=#{KEY}"
RESPONSE = HTTParty.get(TOMTOM_URL).body
json_parsed = JSON.parse(RESPONSE)

# Extracting lat-long pairs from JSON and converting to polyline
tomtom_coordinates = json_parsed['routes'].map { |x| x['legs'] }.pop.map{ |y| y['points']}.pop.map {|z| z.values}
google_encoded_polyline = FastPolylines.encode(tomtom_coordinates)
```

Note:

We extracted the polyline for a route from TomTom Maps API

We need to send this route polyline to TollGuru API to receive toll information

## [TollGuru API](https://tollguru.com/developers/docs/)

### Get key to access TollGuru polyline API
* create a dev account to receive a free key from TollGuru https://tollguru.com/developers/get-api-key
* suggest adding `vehicleType` parameter. Tolls for cars are different than trucks and therefore if `vehicleType` is not specified, may not receive accurate tolls. For example, tolls are generally higher for trucks than cars. If `vehicleType` is not specified, by default tolls are returned for 2-axle cars. 
* Similarly, `departure_time` is important for locations where tolls change based on time-of-the-day.

the last line can be changed to following

```ruby
TOLLGURU_URL = 'https://dev.tollguru.com/v1/calc/route'
TOLLGURU_KEY = ENV['TOLLGURU_KEY']
headers = {'content-type' => 'application/json', 'x-api-key' => TOLLGURU_KEY}
body = {'source' => "tomtom", 'polyline' => google_encoded_polyline, 'vehicleType' => "2AxlesAuto", 'departure_time' => "2021-01-05T09:46:08Z"}
tollguru_response = HTTParty.post(TOLLGURU_URL,:body => body.to_json, :headers => headers)
```

The working code can be found in main.rb file.

## License
ISC License (ISC). Copyright 2020 &copy;TollGuru. https://tollguru.com/

Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted, provided that the above copyright notice and this permission notice appear in all copies.

THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
