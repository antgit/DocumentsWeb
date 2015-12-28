    // Центр зоны
function DistanceWidget(map, pos, addr_id, zone) {
    this.set('map', map);
    this.set('position', pos);
    this.set('zone', zone);

    var marker = new google.maps.Marker({
        draggable: true
    });
    var zone_info = new google.maps.InfoWindow({
        content: ' '
    });
    zone_info.open(map, marker);
    marker.bindTo('map', this);
    marker.bindTo('position', this);
    google.maps.event.addListener(marker, 'click', function () {
        zone_info.open(map, marker);
    });
    var radiusWidget = new RadiusWidget(zone_info, addr_id, zone);
    radiusWidget.bindTo('map', this);
    radiusWidget.bindTo('center', this, 'position');
    this.bindTo('distance', radiusWidget);
    this.bindTo('bounds', radiusWidget);

    this.set('center_marker', marker);
}
DistanceWidget.prototype = new google.maps.MVCObject();
DistanceWidget.prototype.remove = function() {
    this.center_marker.setMap(null);
};

// Окружность вокруг зоны с маркером для изменения радиуса
function RadiusWidget(info, addr_id, zone) {
    var circle = new google.maps.Circle({
        strokeWeight: 2
    });

    this.set('zone', zone);
    this.set('info', info);
    this.set('addr_id', addr_id);
    if (zone == null) {
        this.set('distance', 100);
    }
    else {
        this.set('distance', zone.get('radius'));
    }
    this.set('circle', circle);
    this.bindTo('bounds', circle);
    circle.bindTo('center', this);
    circle.bindTo('map', this);
    circle.bindTo('radius', this);
    this.addSizer_();
}
RadiusWidget.prototype = new google.maps.MVCObject();
RadiusWidget.prototype.distance_changed = function () {
    this.set('radius', this.get('distance'));
};
RadiusWidget.prototype.addSizer_ = function () {
    var sizer = new google.maps.Marker({
        draggable: true,
        icon: '../Images/Routes/resize_x16.png',
        title: 'Радиус зоны: 100 м.'
    });
    sizer.bindTo('map', this);
    sizer.bindTo('position', this, 'sizer_position');
    this.set('sizer_marker', sizer);
    var me = this;
    google.maps.event.addListener(sizer, 'drag', function () {
        me.setDistance();
    });
};
RadiusWidget.prototype.center_changed = function () {
    var bounds = this.get('bounds');
    if (bounds) {
        var lng = bounds.getNorthEast().lng();
        var position = new google.maps.LatLng(this.get('center').lat(), lng);
        this.set('sizer_position', position);
        this.updateInfo_();
    }
};
RadiusWidget.prototype.distanceBetweenPoints_ = function (p1, p2) {
    if (!p1 || !p2) {
        return 0;
    }
    var R = 6371000;
    var dLat = (p2.lat() - p1.lat()) * Math.PI / 180;
    var dLon = (p2.lng() - p1.lng()) * Math.PI / 180;
    var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
    Math.cos(p1.lat() * Math.PI / 180) * Math.cos(p2.lat() * Math.PI / 180) *
    Math.sin(dLon / 2) * Math.sin(dLon / 2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var d = R * c;
    return d;
};
RadiusWidget.prototype.setDistance = function () {
    var pos = this.get('sizer_position');
    var center = this.get('center');
    var distance = this.distanceBetweenPoints_(center, pos);
    this.set('distance', distance);
    this.get('sizer_marker').title = 'Радиус зоны: ' + Math.round(this.get('distance')).toString() + ' м.';
    this.updateInfo_();
};
RadiusWidget.prototype.updateInfo_ = function () {
    var pos = this.get('circle').getCenter();
    var distance = Math.round(this.get('distance'));
    var addr_id = this.get('addr_id');
    this.get('info').setContent(
        (this.get('zone') == null ? $('#name' + addr_id.toString()).val() : this.get('zone').get('name')) + '<br />' +
        (this.get('zone') == null ? $('#addr' + addr_id.toString()).val() : this.get('zone').get('address')) + '<br /><br />' +
        'Широта: ' + pos.lat().toString() + '<br />' +
        'Долгота: ' + pos.lng().toString() + '<br />' +
        'Радиус зоны: ' + distance.toString() + ' м. <br/><br/><div style="float: right;">' +
        '<a href="javascript:setLatLngToAddr(' + addr_id.toString() + ', ' + pos.lat().toString() + ', ' + pos.lng().toString() + ', ' + distance.toString() + ');">Прикрепить координаты</a>&nbsp;&nbsp;' +
        '<a href="javascript:endEditZone(true);">Отмена</a>' +
        '</div>'
    );
}

// Прикрепить координаты к адресу корреспондента
function setLatLngToAddr(addr_id, lat, lng, radius) {
    $.ajax({
        type: 'GET',
        url: 'Router/SetLatLngToAddr',
        async: false,
        data: {
            AddrId: addr_id,
            Lat: lat,
            Lng: lng,
            Radius: radius
        },
        success: function (data) {
            endEditZone(true);
            gvZonesList.PerformCallback();
        }
    });
}

// Завершить изменение зоны
function endEditZone(returnZone) {
    if (objects_map["current_edit"] != null) {
        objects_map["current_edit"].remove();
        if (returnZone) {
            if (objects_map["current_edit"].get('zone') != null) {
                var id = objects_map["current_edit"].get('zone').get('zoneId');
                show_zone(id);
            }
        }
    }
}

// Поиск зоны по адресу
function find_address(addr_id) {
    if (objects_map["current_edit"] != null) {
        objects_map["current_edit"].remove();
        objects_map["current_edit"] = null;
    }
    geocoder.geocode({ 'address': $('#addr' + addr_id).val() }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            map.setCenter(results[0].geometry.location);
            map.setZoom(18);
            var distanceWidget = new DistanceWidget(map, results[0].geometry.location, addr_id);
            objects_map["current_edit"] = distanceWidget;
        } else {
            alert("Ничего не найдено");
        }
    });
}

// Вставить зону в центр карты
function put_zone_to_center(addr_id) {
    if (objects_map["current_edit"] != null) {
        objects_map["current_edit"].remove();
        objects_map["current_edit"] = null;
    }
    var distanceWidget = new DistanceWidget(map, map.getCenter(), addr_id);
    objects_map["current_edit"] = distanceWidget;
}