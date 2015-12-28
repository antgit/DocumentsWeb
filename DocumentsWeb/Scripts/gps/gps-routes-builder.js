//http: //ru.wikipedia.org/wiki/%D0%9F%D0%B5%D1%80%D0%BF%D0%B5%D0%BD%D0%B4%D0%B8%D0%BA%D1%83%D0%BB%D1%8F%D1%80%D0%BD%D0%BE%D1%81%D1%82%D1%8C

var RouteBuilderPin = new google.maps.MarkerImage(
    '../Images/Routes/route-point_x16.png',
    new google.maps.Size(16, 16),
    new google.maps.Point(0, 0),
    new google.maps.Point(8, 8)
);

// Построитель маршрута
function RouteBuilderWidget() {
    this.set('start', null);
    this.set('end', null);
    this.set('editors', []);
    this.set('active_editor', null);
    this.set('editor_drag', false);
    this.set('line', null);
    this.set('editor_index', 0);
    this.set('editor_hide', false);
    this.set('data', '');
}

// Конструктор
RouteBuilderWidget.prototype = new google.maps.MVCObject();

// Строит маршрут между двумя точками
RouteBuilderWidget.prototype.buildByPoints = function (start, end) {
    this.set('start', start);
    this.set('end', end);
    this.build_();
}

// Создает путевую точку
RouteBuilderWidget.prototype.makeWaypoin_ = function (position) {
    var me = this;
    var idx = this.get('editor_index');

    var marker = new google.maps.Marker({
        map: map,
        icon: RouteBuilderPin,
        draggable: true,
        raiseOnDrag: false,
        position: position,
        zIndex: 1000000
    });
    google.maps.event.addListener(marker, 'dblclick', function (event) {
        me.set('editor_hide', false);
        me.deleteWaypoin_(marker);
        me.build_();
    });
    google.maps.event.addListener(marker, 'dragend', function (event) {
        me.build_();
    });
    google.maps.event.addListener(marker, 'mouseover', function (event) {
        me.set('editor_hide', true);
        me.hideEditor_();
    });
    google.maps.event.addListener(marker, 'mouseout', function (event) {
        me.set('editor_hide', false);
    });
    var editors = this.get('editors');
    if (idx == -1) {
        editors.splice(0, 0, marker);
    } else {
        if (editors.length - 1 == idx) {
            editors.push(marker);
        } else {
            editors.splice(idx + 1, 0, marker);
        }
    }
}

// Удалить путевую точку
RouteBuilderWidget.prototype.deleteWaypoin_ = function (marker) {
    var editors = this.get('editors');

    for (var i = 0; i < editors.length; i++) {
        if (editors[i] == marker) {
            editors.splice(i, 1);
            break;
        }
    }
    google.maps.event.clearListeners(marker, 'dblclick');
    google.maps.event.clearListeners(marker, 'dragend');
    google.maps.event.clearListeners(marker, 'mouseover');
    google.maps.event.clearListeners(marker, 'mouseout');
    marker.setMap(null);
}

// Создает редактор маршрута
RouteBuilderWidget.prototype.showEditor_ = function () {
    var editors = this.get('editors');
    var editor = this.get('active_editor');
    var me = this;

    if (editor == null) {
        if (editors.length < 8) {
            var marker = new google.maps.Marker({
                map: map,
                icon: RouteBuilderPin,
                draggable: true,
                raiseOnDrag: false
            });
            google.maps.event.addListener(marker, 'dragstart', function (event) {
                var nPoint = me.nearPoint_(event.latLng);

                var line = me.get('line');
                var editors = me.get('editors');
                var points = line.getPath().getArray();
                var z = -1;

                for (var i = 0; i < points.length; i++) {
                    for (var j = 0; j < editors.length; j++) {
                        var edt = editors[j].getPosition();
                        if (edt.lat() == points[i].lat() && edt.lng() == points[i].lng()) {
                            z = j;
                            break;
                        }
                    }
                    if (nPoint.lat() == points[i].lat() && nPoint.lng() == points[i].lng()) {
                        break;
                    }
                }
                me.set('editor_index', z);
                me.set('editor_drag', true);
            });
            google.maps.event.addListener(marker, 'dragend', function (event) {
                me.set('editor_drag', false);
                me.makeWaypoin_(event.latLng);
                me.build_();
                me.hideEditor_();
            });
            google.maps.event.addListener(marker, 'mouseout', function (event) {
                if (!me.get('editor_drag')) {
                    me.hideEditor_();
                }
            });
            this.set('active_editor', marker);
        }
    }
}

// Создает редактор маршрута
RouteBuilderWidget.prototype.hideEditor_ = function () {
    var editor = this.get('active_editor');

    if (editor != null) {
        google.maps.event.clearListeners(editor, 'dragend');
        google.maps.event.clearListeners(editor, 'dragstart');
        google.maps.event.clearListeners(editor, 'mouseout');
        editor.setMap(null);
        this.set('active_editor', null);
    }
}

// Строит маршрут между двумя точками
RouteBuilderWidget.prototype.build_ = function () {
    var start = this.get('start');
    var end = this.get('end');
    var editors = this.get('editors');
    var waypoints = '';
    var me = this;

    this.clear_();

    for (var i = 0; i < editors.length; i++) {
        if (waypoints != '') {
            waypoints += '|';
        }
        var pos = editors[i].getPosition();
        waypoints += pos.lat().toString() + ',' + pos.lng().toString();
    }

    $.getJSON('Router/ReqGoogleRoute',
        {
            start: start.lat().toString() + ',' + start.lng().toString(),
            end: end.lat().toString() + ',' + end.lng().toString(),
            waypoints: waypoints
        },
        function (data) {
            if (data.status == 'OK') {
                me.set('data', data);
                me.draw_();
            }
        }
    );
}

// Прорисовка маршрута
RouteBuilderWidget.prototype.draw_ = function () {
    var me = this;
    var data = this.get('data');
    var editors = this.get('editors');

    var line = new google.maps.Polyline({
        path: google.maps.geometry.encoding.decodePath(data.routes[0].overview_polyline.points),
        geodesic: false,
        strokeColor: '#FE7569',
        strokeOpacity: 0.7,
        strokeWeight: 5
    });
    google.maps.event.addListener(line, 'mouseover', function (event) {
        var editors = me.get('editors');
        if (!me.get('editor_hide') && editors.length < 8) {
            me.showEditor_();
            var editor = me.get('active_editor');
            editor.setPosition(event.latLng);
        }
    });
    google.maps.event.addListener(line, 'mousemove', function (event) {
        var editors = me.get('editors');
        if (me.get('active_editor') != null) {
            me.get('active_editor').setPosition(event.latLng);
        } else {
            if (!me.get('editor_hide') && editors.length < 8) {
                me.showEditor_();
                me.get('active_editor').setPosition(event.latLng);
            }
        }
    });
    line.setMap(map);
    me.set('line', line);
    me.set('data', data);

    for (var k = 0; k < editors.length; k++) {
        var marker = editors[k];
        marker.setPosition(me.nearPoint_(marker.getPosition()));
    }
}

// Находит ближайшую точку на марштуре
RouteBuilderWidget.prototype.nearPoint_ = function (pos) {
    var line = this.get('line');
    var editors = this.get('editors');

    var points = line.getPath().getArray();
    var p = null;
    var len_ = 1000;

    var xp = pos.lat();
    var yp = pos.lng();

    for (var i = 0; i < points.length; i++) {
        var point = points[i];
        var x = point.lat();
        var y = point.lng();

        var len = Math.sqrt((xp - x) * (xp - x) + (yp - y) * (yp - y));
        if (len < len_) {
            p = point;
            len_ = len;
        }
    }

    return p;
}

// Очистка маршрута
RouteBuilderWidget.prototype.clear_ = function () {
    var line = this.get('line');

    if (line != null) {
        google.maps.event.clearListeners(line, 'mouseover');
        google.maps.event.clearListeners(line, 'mousemove');
        line.setMap(null);
    }
}

// Удаляет маршрут
RouteBuilderWidget.prototype.remove = function () {
    this.clear_();
    var editors = this.get('editors');
    for (var i = editors.length - 1; i >= 0; i--) {
        this.deleteWaypoin_(editors[i]);
    }
    this.hideEditor_();
}

// Возвращает маршрут
RouteBuilderWidget.prototype.getRoute = function () {
    var data = [];
    var e = [];
    var editors = this.get('editors');
    for (var i = 0; i < editors.length; i++) {
        e.push(editors[i].getPosition());
    }
    data.push(e);
    data.push(this.get('data'));
    return data;
}

// Задает маршрут
RouteBuilderWidget.prototype.setRoute = function (route) {
    for (var i = 0; i < route[0].length; i++) {
        this.makeWaypoin_(new google.maps.LatLng(route[0][i].Xa, route[0][i].Ya));
    }
    this.set('data', route[1]);
    this.draw_();
    var line = this.get('line');
    this.set('start', line.getPath().getArray()[0]);
    this.set('end', line.getPath().getArray()[line.getPath().getArray().length - 1]);
}