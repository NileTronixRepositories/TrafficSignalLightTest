<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignalMap.aspx.cs" Inherits="TrafficSignalLight.SignalMap" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Map</title>
    <script src="https://polyfill.io/v3/polyfill.min.js?features=default"></script>
    <style type="text/css">
        html, body {
            height: 100%;
            margin: 0;
            padding: 0;
        }

        .map-view {
            height: 80vh;
            width: 74vw;
            margin-left: 1vh;
            float: left;
        }

        .point-select {
            height: 100vh;
            width: 20vw;
            background-color: red;
            float: left;
        }

        .nav-side {
            height: 100vh;
            width: 5vw;
            background-color: blue;
            float: left;
        }

        .point-settings {
            height: 19vh;
            width: 75vw;
            background-color: orange;
            margin-top: 1vh;
            float: left;
        }
    </style>
    <link href="Assets/css/bootstrap.min.css" rel="stylesheet" />
    <script src="Assets/js/jquery-2.2.4.min.js"></script>
    <script src="Assets/js/bootstrap.min.js"></script>
    <script type="text/javascript">

        var currentPosition = '';

        $(document).ready(function () {
            window.initMap = initMap;
        });

        function initMap() {
            const map = new google.maps.Map(document.getElementById("map"), {
                zoom: 12,
                center: {
                    lat: 30.0332459,
                    lng: 31.1679859
                },
            });
            const geocoder = new google.maps.Geocoder();
            const infowindow = new google.maps.InfoWindow();
            google.maps.event.addListener(map, 'click', function (event) {
                // do something with event.latLng
                //alert(getCountry(event.latLng));
                var pos = event.latLng;
                geocodeLatLng(geocoder, map, infowindow, pos.toString());
            });
            const trafficLayer = new google.maps.TrafficLayer();

            trafficLayer.setMap(map);
        }

        function geocodeLatLng(geocoder, map, infowindow, input) {
            const latlngStr = input.split(",", 2);
            const latlng = {
                lat: parseFloat(latlngStr[0].replace('(', '')),
                lng: parseFloat(latlngStr[1].replace(')', '')),
            };

            geocoder
                .geocode({ location: latlng })
                .then((response) => {
                    if (response.results[0]) {
                        // map.setZoom(11);

                        const marker = new google.maps.Marker({
                            position: latlng,
                            map: map,
                        });

                        infowindow.setContent(response.results[0].formatted_address);
                        infowindow.open(map, marker);
                    } else {
                        window.alert("No results found");
                    }
                })
                .catch((e) => window.alert("Geocoder failed due to: " + e));
        }


    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="nav-side"></div>
        <div class="point-select"></div>
        <div id="map" class="map-view"></div>
        <div class="point-settings"></div>

        <!-- 
      The `defer` attribute causes the callback to execute after the full HTML
      document has been parsed. For non-blocking uses, avoiding race conditions,
      and consistent behavior across browsers, consider loading using Promises.
      See https://developers.google.com/maps/documentation/javascript/load-maps-js-api
      for more information.
      -->
        <script
            src="https://maps.googleapis.com/maps/api/js?key=AIzaSyBtu5UEeVaDcvv7QpkqKjkiu3HzJBSnmzk&callback=initMap&v=weekly"
            defer></script>
    </form>
</body>
</html>
