const axios = require("axios");
const polyline = require("@mapbox/polyline");

// REST API key from tomtom
const TOMTOM_API_KEY = process.env.TOMTOM_API_KEY;

// API Key from TollGuru
const TOLLGURU_API_KEY = process.env.TOLLGURU_API_KEY;
const TOLLGURU_API_URL = "https://apis.tollguru.com/toll/v2";
const POLYLINE_ENDPOINT = "complete-polyline-from-mapping-service";

// Philadelphia, PA
const source = {
  longitude: "-75.1652",
  latitude: "39.9526",
};

// New York, NY
const destination = {
  longitude: "-74.0060",
  latitude: "40.7128",
};

// Explore https://tollguru.com/toll-api-docs to get best of all the parameter that TollGuru has to offer
const request_parameters = {
  vehicle: {
    type: "2AxlesAuto",
  },
  // Visit https://en.wikipedia.org/wiki/Unix_time to know the time format
  departure_time: "2021-01-05T09:46:08Z",
};

const url = `https://api.tomtom.com/routing/1/calculateRoute/${source.latitude},${source.longitude}:${destination.latitude},${destination.longitude}/json?avoid=unpavedRoads&key=${TOMTOM_API_KEY}`;

const flatten = (arr) => arr.reduce((acc, val) => acc.concat(val), []);

// JSON path "$..points"
const getPoints = (body) => {
  const legs = body.routes.map((route) => route.legs);
  const points = flatten(legs).map((leg) => leg.points);
  return flatten(points).map(({ latitude, longitude }) => [latitude, longitude]);
};

const getPolyline = (body) => polyline.encode(getPoints(body));

const handleRoute = async () => {
  try {
    const response = await axios.get(url);
    const body = response.data;
    const _polyline = getPolyline(body);
    console.log(_polyline);

    const requestBody = {
      source: "tomtom",
      polyline: _polyline,
      ...request_parameters,
    };

    const tollguruResponse = await axios.post(
      `${TOLLGURU_API_URL}/${POLYLINE_ENDPOINT}`,
      requestBody,
      {
        headers: {
          "content-type": "application/json",
          "x-api-key": TOLLGURU_API_KEY,
        },
      }
    );

    console.log(tollguruResponse.data);
  } catch (error) {
    console.error(error);
  }
};

handleRoute();
