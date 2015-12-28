//Стандартный подход загрузки маршрута (плохой подход) 
//http://stackoverflow.com/questions/9341700/google-maps-directionsdisplay-setdirections-reverse-engineering

// Маршрут
function RouteWidget(map, routeId) {
    this.set('routeId', routeId);
}

// Конструтор
RouteWidget.prototype = new google.maps.MVCObject();

// Загрузка точек маршрута
RouteWidget.prototype.loadRoute = function () {
    var routeBuilders = this.get('routeBuilders');
    if (routeBuilders != undefined) {
        for (var i = 0; i < routeBuilders.length; i++) {
            routeBuilders[i].remove();
        }
    }
    var me = this;
    $.getJSON('Router/GetRoutePoints', { RouteId: this.get('routeId'), tmps: new Date().getTime() }, function (data) {
        me.set('zones', data.Zones);
        me.draw();
        if (data.Route != '') {
            var routeBuilders = [];
            var steps = JSON.parse(data.Route);
            for (var i = 0; i < steps.length; i++) {
                var rb = new RouteBuilderWidget();
                rb.setRoute(steps[i]);
                routeBuilders.push(rb);
            }
            me.set('routeBuilders', routeBuilders);
        }
    });
}

// Прорисовка маршрута
RouteWidget.prototype.draw = function () {
    var llbounds = new google.maps.LatLngBounds();
    var data = this.get('zones');
    var markers = [];
    if (data != undefined) {
        var pinColor = "FE7569";
        var pinShadow = new google.maps.MarkerImage(
            "http://chart.apis.google.com/chart?chst=d_map_pin_shadow",
            new google.maps.Size(40, 37),
            new google.maps.Point(0, 0),
            new google.maps.Point(12, 35)
        );
        for (var i = 0; i < data.length; i++) {
            var point = new google.maps.LatLng(data[i].X, data[i].Y);
            llbounds.extend(point);
            var pinImage = new google.maps.MarkerImage(
                "http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=" + data[i].Number + "|" + pinColor,
                new google.maps.Size(21, 34),
                new google.maps.Point(0, 0),
                new google.maps.Point(10, 34)
            );
            var marker = new google.maps.Marker({
                map: map,
                icon: pinImage,
                shadow: pinShadow,
                position: point,
                title: data[i].Name
            });
            markers.push(marker);
        }
        map.fitBounds(llbounds);
        this.set('markers', markers);
    }
}

// Прокладка маршрута
RouteWidget.prototype.paveWay = function () {
    var routeBuilders = this.get('routeBuilders');
    if (routeBuilders != undefined) {
        for (var i = 0; i < routeBuilders.length; i++) {
            routeBuilders[i].remove();
        }
    }
    var routeBuilders = [];
    var data = this.get('zones');
    if (data != undefined) {
        for (var i = 0; i < data.length; i++) {
            var point = new google.maps.LatLng(data[i].X, data[i].Y);
            if (i > 0) {
                var pre_point = new google.maps.LatLng(data[i - 1].X, data[i - 1].Y);
                // Построение маршрута
                var rb = new RouteBuilderWidget();
                rb.buildByPoints(pre_point, point);
                routeBuilders.push(rb);
            }
        }
        this.set('routeBuilders', routeBuilders);
    }
}

// Сохраняет маршрут
RouteWidget.prototype.save = function () {
    var steps = this.get('routeBuilders');
    var routeId = this.get('routeId');
    if (steps != undefined) {
        var data = [];
        for (var i = 0; i < steps.length; i++) {
            data.push(steps[i].getRoute());
        }
        var steps_string = encodeURIComponent(JSON.stringify(data));
        $.ajax({
            type: 'POST',
            url: 'Router/SaveRoute',
            async: false,
            data: {
                RouteId: routeId,
                Steps: steps_string
            },
            success: function (data) {
                alert('Маршрут успешно сохранен');
            }
        });
    }
}

// Удаление маршрута
RouteWidget.prototype.remove = function () {
    var markers = this.get('markers');
    var routeBuilders = this.get('routeBuilders');
    if (markers != undefined) {
        for (var i = 0; i < markers.length; i++) {
            markers[i].setMap(null);
        }
    }
    if (routeBuilders != undefined) {
        for (var i = 0; i < routeBuilders.length; i++) {
            routeBuilders[i].remove();
        }
    }
}

// Показать маршрут
function showRoute(id) {
    removeRoute();
    var route = new RouteWidget(map, id);
    route.loadRoute();
    objects_map['current_route'] = route;
}

// Скрывает текущий маршрут
function removeRoute() {
    if (objects_map['current_route'] != undefined) {
        objects_map['current_route'].remove();
        objects_map['current_route'] = undefined;
    }
}

// Построить маршрут
function buildRoute() {
    if (objects_map['current_route'] != undefined) {
        objects_map['current_route'].paveWay();
    }
}

// Сохранить маршрут
function saveRoute() {
    if (objects_map['current_route'] != undefined) {
        objects_map['current_route'].save();
    }
}


//var directionsService = new google.maps.DirectionsService();

/*var request = {
origin: pre_point,
destination: point,
travelMode: google.maps.TravelMode.DRIVING
};*/

/*directionsService.route(request, function (result, status) {
                    if (status == google.maps.DirectionsStatus.OK) {
                        var rendererOptions = {
                            draggable: true,
                            preserveViewport: true,
                            markerOptions: {
                                visible: false
                            }
                        };
                        var directionsDisplay = new google.maps.DirectionsRenderer(rendererOptions);
                        directionsDisplay.setMap(map);
                        //alert(result.routes[0].bounds.ca.f);
                        //var r = JSON.restory(JSON.dump(result));
                        //var r = JSON.stringify(result);
                        //r = r.replace('"ea"', '"southwest"').replace('"ca"', '"northeast"').replace('"b"', '"lat"').replace('"f"', '"lng"').replace('"Ya"', '"lan"').replace('"Za"', '"lng"');
                        //var obj = JSON.parse(r);
                        //directionsDisplay.setDirections(new google.maps.DirectionsResult(obj));
                        directionsDisplay.setDirections(result);
                        directionsDisplays.push(directionsDisplay);
                        directionsSteps.push(result);
                    }
                });*/

/*
// Расчет маршрута
RouteWidget.prototype.calcRoute = function () {
    $.getJSON("Router/GetRoutePoints", { RouteId: this.get('routeId') }, function (data) {
        var llbounds = new google.maps.LatLngBounds();
        for (i = 0; i < data.length; i++) {
            var point = new google.maps.LatLng(data[i].X, data[i].Y);
            llbounds.extend(point);
            var marker = new google.maps.Marker({
                map: map,
                icon: 'http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=' + data[i].Number + '|FE7569',
                shadow: 'http://chart.apis.google.com/chart?chst=d_map_pin_shadow',
                position: point,
                title: data[i].Name
            });

            if (i > 0) {
                var pre_point = new google.maps.LatLng(data[i - 1].X, data[i - 1].Y);
                var request = {
                    origin: pre_point,
                    destination: point,
                    travelMode: google.maps.TravelMode.DRIVING
                };

                directionsService.route(request, function (result, status) {
                    if (status == google.maps.DirectionsStatus.OK) {
                        var rendererOptions = {
                            draggable: true,
                            markerOptions: {
                                visible: false
                            }
                        };
                        var directionsDisplay = new google.maps.DirectionsRenderer(rendererOptions);
                        directionsDisplay.setMap(map);
                        directionsDisplay.setDirections(result);
                    }
                });
            }
        }
        map.fitBounds(llbounds);
    });
}
*/


/*
// DirectionsStep -> { distance: <Distance>,
//                     duration: <Duration>,
//                     end_location: <LatLng>,
//                     instructions: <string>,
//                     path: Array <LatLng>,
//                     start_location: <LatLng>,
//                     travel_mode: <TravelMode> }
google.maps.DirectionsStep = function( step ){
    return {
        distance: step.distance,
        duration: step.duration,
        end_location: new google.maps.LatLng( step.end_location.lat, step.end_location.lng ),
        instructions: step.html_instructions,
        path: google.maps.geometry.encoding.decodePath( step.polyline.points ),
        start_location: new google.maps.LatLng( step.start_location.lat, step.start_location.lng ),
        travel_mode: eval('google.maps.TravelMode'+step.travel_mode)
    };
}

// DirectionsLeg -> { distance: <Distance>,
//                    duration: <Duration>,
//                    end_address: <string>,
//                    end_location: <LatLng>
//                    start_address: string,
//                    start_location: <LatLng>,
//                    steps: Array <DirectionsStep>,
//                    via_waypoints: Array <LatLng> }
google.maps.DirectionsLeg = function( leg ){
    var steps = [];
    for (var i=0; i<leg.steps.length; i++)
        steps.push( new google.maps.DirectionsStep( leg.steps[i] ) );
    return {
        distance: leg.distance,
        duration: leg.duration,
        end_address: leg.end_address,
        end_location: new google.maps.LatLng( leg.end_location.lat, leg.end_location.lng ),
        start_address: leg.start_address,
        start_location: new google.maps.LatLng( leg.start_location.lat, leg.start_location.lng ),
        steps: steps,
        via_waypoints: [],  //ToDo: try with waypoints!
    };
}

// DirectionsRoute -> { bounds: <LatLngBounds>,
//                      copyrights: <string>,
//                      legs: Array <DirectionsLeg>,
//                      overview_path: Array <LatLng>,
//                      warnings: Array <string>,
//                      waypoint_order: Array <number> }
google.maps.DirectionsRoute = function( route ){
    var legs = [];
    for (var i=0; i<route.legs.length; i++)
        legs.push( new google.maps.DirectionsLeg( route.legs[i] ) );
    return {
        bounds: new google.maps.LatLngBounds(
            new google.maps.LatLng( route.bounds.southwest.lat, route.bounds.southwest.lng ),
            new google.maps.LatLng( route.bounds.northeast.lat, route.bounds.northeast.lng )
        ),
        copyrights: route.copyrights,
        legs: legs,
        overview_path: google.maps.geometry.encoding.decodePath( route.overview_polyline.points ),
        warnings: route.warnings,
        waypoint_order: route.waypoint_order
    };
}

// DirectionsResult -> Array <DirectionsRoute>
google.maps.DirectionsResult = function( directionsApiResponse ) {
    var routes = [];
    for (var i=0; i<directionsApiResponse.routes.length; i++)
        routes.push( new google.maps.DirectionsRoute( directionsApiResponse.routes[i] ) );
    return { routes:routes };
}*/