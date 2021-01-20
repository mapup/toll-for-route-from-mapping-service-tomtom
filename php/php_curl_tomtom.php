<?php
//using tomtommaps API

//Source and Destination Coordinates
//Dallas, TX
$source_longitude='-96.7970';
$source_latitude='32.7767';
//Addison, Texas
$destination_longitude='-96.83256117564599';
$destination_latitude='32.967452488953626';

//tomtom api key..
$key = 'tomtomAPIkey';

$url='https://api.tomtom.com/routing/1/calculateRoute/'.$source_latitude.','.$source_longitude.':'.$destination_latitude.','.$destination_longitude.'/json?avoid=unpavedRoads&key='.$key.'';

//connection..
$tomtom = curl_init();

curl_setopt($tomtom, CURLOPT_SSL_VERIFYHOST, false);
curl_setopt($tomtom, CURLOPT_SSL_VERIFYPEER, false);

curl_setopt($tomtom, CURLOPT_URL, $url);
curl_setopt($tomtom, CURLOPT_RETURNTRANSFER, true);

//getting response from tomtomapis..
$response = curl_exec($tomtom);
$err = curl_error($tomtom);

curl_close($tomtom);

if ($err) {
	  echo "cURL Error #:" . $err;
} else {
	  echo "200 : OK\n";
}
//extracting polyline from the JSON response..
$data_tomtom = json_decode($response, true);
$new_leg_points=$data_tomtom['routes']['0']['legs']['0']['points'];

//polyline..
require_once(__DIR__.'/Polyline.php');
$polyline_tomtom = Polyline::encode($new_leg_points);

//using tollguru API..
$curl = curl_init();

curl_setopt($curl, CURLOPT_SSL_VERIFYHOST, false);
curl_setopt($curl, CURLOPT_SSL_VERIFYPEER, false);

$postdata = array(
	"source" => "gmaps",
	"polyline" => $polyline_tomtom
);

//json encoding source and polyline to send as postfields..
$encode_postData = json_encode($postdata);

curl_setopt_array($curl, array(
CURLOPT_URL => "https://dev.tollguru.com/v1/calc/route",
CURLOPT_RETURNTRANSFER => true,
CURLOPT_ENCODING => "",
CURLOPT_MAXREDIRS => 10,
CURLOPT_TIMEOUT => 30,
CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
CURLOPT_CUSTOMREQUEST => "POST",

// sending tomtom polyline to tollguru
CURLOPT_POSTFIELDS => $encode_postData,
CURLOPT_HTTPHEADER => array(
				      "content-type: application/json",
				      "x-api-key: tollguruapi"),
));

$response = curl_exec($curl);
$err = curl_error($curl);
curl_close($curl);

if ($err) {
	  echo "cURL Error #:" . $err;
} else {
	  echo "200 : OK\n";
}

//response from tollguru..
$data = var_dump(json_decode($response, true));
print_r($data);
?>