// Трек
function TrackWidget(map, trackId) {
    this.set('trackId', trackId);
}

// Конструктор
TrackWidget.prototype = new google.maps.MVCObject();

// Строит трек
TrackWidget.prototype.draw = function () {
    var me = this;
    $.getJSON('Router/GetTrack', { trackId: this.get('trackId'), tmps: new Date().getTime() }, function (data) {
        var polylineCoords = [];
        var directionMarkers = [];
        var llbounds = new google.maps.LatLngBounds();

        var infowindow = new google.maps.InfoWindow({
            content: ''
        });

        var pinShadow = new google.maps.MarkerImage(
            "http://chart.apis.google.com/chart?chst=d_map_pin_shadow",
            new google.maps.Size(40, 37),
            new google.maps.Point(0, 0),
            new google.maps.Point(12, 35)
        );

        for (i = 0; i < data.length; i++) {
            var point = new google.maps.LatLng(data[i].X, data[i].Y);
            polylineCoords.push(point);
            llbounds.extend(point);
            var iconURL = '../Images/Routes/Pins/pin30_red_dot.png';

            if (data[i].Direction > -1) {
                if (data[i].Speed > 15) {
                    iconURL = '../Images/Routes/Pins/pin30_green_h' + data[i].Direction.toString() + '.png';
                } else if (data[i].Speed <= 15 && data[i].Speed > 5) {
                    iconURL = '../Images/Routes/Pins/pin30_yellow_h' + data[i].Direction.toString() + '.png';
                } else if (data[i].Speed <= 5 && data[i].Speed > 0) {
                    iconURL = '../Images/Routes/Pins/pin30_gray_h' + data[i].Direction.toString() + '.png';
                }
            }

            var marker = new google.maps.Marker({
                map: map,
                icon: iconURL,
                shadow: pinShadow,
                position: point
            });
            directionMarkers.push(marker);
            bindInfoWindow(marker, map, infowindow, data[i].RouteMemberName + '<br />' + data[i].stringDate + '<!--<br /><img src="../Images/Routes/Batt/Batt050.png"/>--><br /><br />Скорость: ' + Math.round(data[i].Speed, 2) + ' км/ч');
        }

        // Настройки для линии
        var polyline = new google.maps.Polyline({
            map: map,
            path: polylineCoords,
            strokeColor: '#FF0000',
            strokeOpacity: 1.0,
            strokeWeight: 2
        });

        map.fitBounds(llbounds);

        // Добавляем на карту
        me.set('trackLine', polyline);
        me.set('markersList', directionMarkers);
    });
}

// Удаляет трек
TrackWidget.prototype.remove = function () {
    this.get('trackLine').setMap(null);
    var directions = this.get('markersList');
    for (var i = 0; i < directions.length; i++) {
        directions[i].setMap(null);
    }
}

// Строит трек
function buildTrack(trackId) {
    if (objects_map['track_' + trackId] == undefined) {
        var track = new TrackWidget(map, trackId);
        track.draw();
        objects_map['track_' + trackId] = track;
    }
}

// Удаляет трек
function removeTrack(trackId) {
    if (objects_map['track_' + trackId] != undefined) {
        objects_map['track_' + trackId].remove();
        objects_map['track_' + trackId] = undefined;
    }
}