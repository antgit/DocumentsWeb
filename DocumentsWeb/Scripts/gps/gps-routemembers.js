// Объект слежения
function RouteMemberWidget(routeMemberId) {
    this.set("routeMemberId", routeMemberId);
    var array = objects_map["routeMembers"];
    if (array == undefined) {
        array = new Array();
        objects_map["routeMembers"] = array;
    }
    array.push(this);
    this.loadPosition_();
}

RouteMemberWidget.prototype = new google.maps.MVCObject();

// Обновляет позицию маркера
RouteMemberWidget.prototype.updatePosition = function () {
    this.loadPosition_();
}

// Возвращает идентификатор
RouteMemberWidget.prototype.getId = function () {
    return this.get("routeMemberId");
}

// Удаляет маркер с карты
RouteMemberWidget.prototype.remove = function () {
    var market = this.get("marker");
    var label = this.get("label");
    if (market != undefined) {
        market.setMap(null);
        var array = objects_map["routeMembers"];
        if (array != undefined) {
            for (i = 0; i < array.length; i++) {
                if (array[i].get("routeMemberId") == this.get("routeMemberId")) {
                    array.splice(i, 1);
                    break;
                }
            }
        }
    }
    if (label != undefined) {
        label.setMap(null);
    }
}

// Загружает данные о положении объекта
RouteMemberWidget.prototype.loadPosition_ = function () {
    var id = this.get("routeMemberId");
    var me = this;
    $.ajax({
        type: 'GET',
        url: 'Router/GetRouteMemberPosition',
        async: false,
        cache: false,
        data: {
            Id: id
        },
        success: function (data) {
            me.set("x", data.X);
            me.set("y", data.Y);
            me.set("name", data.Name);
            me.set("speed", data.Speed);
            me.set("timestamp", data.Date_Time);
            me.set("direction", data.Direction);
            me.onDataLoad_();
        }
    });
}

// Срабатывает после загрузки данных с сервера
RouteMemberWidget.prototype.onDataLoad_ = function () {
    var market = this.get("marker");
    var label = this.get("label");
    var text = '<b>' + this.get("name") + '</b><br />' + this.get('timestamp') + '<br />V: ' + Math.round(this.get('speed'), 2) + 'км/час';
    var point = new google.maps.LatLng(this.get("x"), this.get("y"));
    if (market == undefined) {
        marker = new Member({
            map: map
        });
        marker.set('position', point);
        marker.set('direction', this.get('direction'));
        label = new Label({
            map: map
        });
        label.set('zIndex', 5);
        label.set('text', text);
        label.bindTo('position', marker, 'position');
        this.set("label", label);
        this.set("marker", marker);
        /*var array = objects_map["routeMembers"];
        if (array == undefined) {
            array = new Array();
            objects_map["routeMembers"] = array;
        }
        array.push(this);*/
        map.panTo(point);
        map.setZoom(11);
    } else {
        label.set('text', text);
        marker.set('position', point);
        marker.set('direction', this.get('direction'));
    }
}

function addRouteMember(id) {
    var rm = new RouteMemberWidget(id);
}

function removeRouteMember(id) {
    var array = objects_map["routeMembers"];
    if (array != undefined) {
        for (i = 0; i < array.length; i++) {
            if (array[i].getId() == id) {
                array[i].remove();
                break;
            }
        }
    }
}