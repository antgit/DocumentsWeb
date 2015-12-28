// Зона
function ZoneWidget(map, zoneId) {
    var own = this;
    this.set('zoneId', zoneId);
    $.getJSON("Router/GetZone", { zoneId: zoneId }, function (data) {
        var pos_X = data.X;
        var pos_Y = data.Y;
        var radius = data.Radius;
        var addr = data.Address;
        var addr_id = data.AddressId;
        var name = data.Name;
        var center = new google.maps.LatLng(pos_X, pos_Y);

        own.set('position', center);
        own.set('radius', radius);
        own.set('address', addr);
        own.set('address_id', addr_id);
        own.set('name', name);

        if (pos_X != 0 && pos_Y != 0) {
            if (objects_map["zone_" + zoneId.toString()] == null) {
                var circle = new google.maps.Circle({
                    map: map,
                    radius: radius,
                    center: center,
                    fillColor: '#AA0000'
                });
                var pinImage = new google.maps.MarkerImage(
                    "../Images/Routes/Pins/pin30_yellow.png",
                    new google.maps.Size(21, 34),
                    new google.maps.Point(0, 0),
                    new google.maps.Point(10, 34)
                );
                var pinShadow = new google.maps.MarkerImage(
                    "../Images/Routes/Pins/pin30_shadow.png",
                    new google.maps.Size(40, 37),
                    new google.maps.Point(0, 0),
                    new google.maps.Point(12, 35)
                );
                var marker = new google.maps.Marker({
                    map: map,
                    icon: pinImage,
                    shadow: pinShadow,
                    position: center,
                    title: name + "\r\n" + addr
                });
                var inf =
                    name + '<br/>' +
                    addr + '<br/><br/>' +
                    'Широта: ' + pos_X.toString() + '<br />' +
                    'Долгота: ' + pos_Y.toString() + '<br />' +
                    'Радиус зоны: ' + radius.toString() + ' м.<br />' +
                    '<a href="javascript:ZoneEdit(' + zoneId + ');" style="float: right;">Настроить зону</a>';
                var zone_info = new google.maps.InfoWindow({
                    content: inf
                });
                google.maps.event.addListener(marker, 'click', function () {
                    zone_info.open(map, marker);
                });
                objects_map["zone_" + zoneId.toString()] = own;
                map.panTo(center);
                map.setZoom(16);

                own.set('marker', marker);
                own.set('circle', circle);
                own.set('zone_info', zone_info);
            }
        }
    });
}
ZoneWidget.prototype = new google.maps.MVCObject();
ZoneWidget.prototype.remove = function () {
    this.get('marker').setMap(null);
    this.get('circle').setMap(null);
    this.get('zone_info').setMap(null);
    objects_map["zone_" + this.get('zoneId').toString()] = null;
}

function ZoneEdit(id) {
    var zone = objects_map["zone_" + id.toString()];
    zone.remove();
    endEditZone(false);
    var editor = new DistanceWidget(map, zone.get('position'), zone.get('address_id'), zone);
    objects_map["current_edit"] = editor;
}

// Показать зону на карте
function show_zone(zone_id) {
    var zoneWidget = new ZoneWidget(map, zone_id);
}

// Скрывает зону с карты
function hide_zone(zone_id) {
    if (objects_map["zone_" + zone_id.toString()] != null) {
        objects_map["zone_" + zone_id.toString()].remove();
        objects_map["zone_" + zone_id.toString()] = null;
    }
}