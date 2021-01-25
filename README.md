# toll-tomtom
Click on the folders above to see examples to extend mapping capabilities of TomTom by adding toll information from [**TollGuru**](https://tollguru.com/) to the route information from TomTom.

The toll information has following [key features](https://tollguru.com/developers/features):
### Support for [geographies](https://github.com/mapup/toll-tomtom/wiki/Countries-supported-by-TollGuru) 
* North America - United States, Canada, Mexico
* Europe - UK, France, Spain, Portugal, Ireland, Netherlands, Denmark, Norway, Sweden, Italy, Germany
* Australia - Australia
* Asia - India
* Latin America - Peru, Colombia, Argentina, Chile

### Based on vehicles in use in each country, [vehicle type support](https://github.com/mapup/toll-tomtom/wiki/Supported-vehicle-type-list-for-TollGuru-for-respective-continents)
* Car, SUV or Pickup truck. You can specify number of axles including axles in trailers
* Carpool
* Taxi
* Rideshare
* Motorcycle
* Truck
* Bus
* Recreational vehicle (RV), motorhome, caravan, van

### Rates for all the available payment modes in local currencies
* Tag transponder (including primary and secondary transponders)
* cash
* licence plate
* credit card
* prepaid

### Time based tolls
You can specify "departure_time" as DateTime (string) or Timestamp (number) to provide you with most accurate toll rates based on time of day/week/month/year

### All types of toll systems
Support for Barrier, Ticket System and Distance based tolling

### Support for [other mapping services](https://github.com/mapup)
[See the Mapping services list](https://github.com/mapup/toll-tomtom/wiki/Mapping-platforms-supported-by-TollGuru) for all mapping platforms supported. You can edit the **source** argument to send polyline from another mapping service.

### [Support for trucks based on height, weight, harardous goods, etc.](https://github.com/mapup/toll-tomtom/wiki/Supported-trucking-parameter-in-TollGuru)
You can receive tolls based on vehicle height, weight etc., while calculating toll: "truckType","shippedHazardousGoods","tunnelCategory","truckRestrictionPenalty" and [more](https://github.com/mapup/toll-tomtom/wiki/Supported-trucking-parameter-in-TollGuru).





