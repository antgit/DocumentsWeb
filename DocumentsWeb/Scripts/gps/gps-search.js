function geo_search(text) {
    geocoder.geocode({ 'address': text }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            lbSearchResult.ClearItems();
            for (var i = 0; i < results.length; i++) {
                lbSearchResult.AddItem(results[i].formatted_address, results[i]);
            }
            $('#searchResult').show();
        } else {
            lbSearchResult.ClearItems();
            $('#searchResult').hide();
            alert("Ничего не найдено");
        }
    });
}

function moveToSearch(obj) {
    clearSearchMarker();
    var marker = new google.maps.Marker({
        map: map,
        position: obj.geometry.location,
        title: obj.formatted_address
    });
    map.setCenter(obj.geometry.location);
    map.setZoom(15);
    objects_map["search_marker"] = marker;
}

function clearSearchMarker() {
    if (objects_map["search_marker"] != undefined) {
        objects_map["search_marker"].setMap(null);
        objects_map["search_marker"] = undefined;
    }
}