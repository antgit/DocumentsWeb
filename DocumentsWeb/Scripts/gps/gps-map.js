// Карта
var map;
var geocoder = new google.maps.Geocoder();
var objects_map = new Object;

// Приделывает информационное окно к маркеру (открывается по клику)
function bindInfoWindow(marker, map, infowindow, html) {
    google.maps.event.addListener(marker, 'click', function () {
        infowindow.setContent(html);
        infowindow.open(map, marker);
    });
}

// Инициализация страницы
$(document).ready(function () {
    var height = splFullScreenMode.GetPaneByName('MapPane').GetClientHeight();
    $('#divActions').height(height - $('#divHeader').height());
    initialize();

    window.setInterval(function () {
        var array = objects_map["routeMembers"];
        if (array != undefined) {
            for (i = 0; i < array.length; i++) {
                array[i].updatePosition();
            }
        }
    }, 10000);
});

// Инициализация карты
function initialize() {
    var myLatlng = new google.maps.LatLng(49.138597, 31.175537);
    var myOptions = {
        zoom: 6,
        center: myLatlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    }
    map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
}