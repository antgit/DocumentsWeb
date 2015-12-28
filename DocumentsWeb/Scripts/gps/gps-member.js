// Объект слежения с направлением движения
function Member(opt_options) {
    this.setValues(opt_options);

    var content = this.content_ = document.createElement('img');
    content.id = 'mb_' + (new Date().getTime());
    content.src = '../Images/Routes/route-member_x24.png';
    content.style.cssText = 'top: -12px; left: -12px;';
    /*content.style.cssText = 'position: relative; ' +
                          'background-color: #FFF;border: 1px solid #000;' +
                          'white-space:nowrap;padding: 2px;font-family: Arial;' +
                          'font-size: 10px;';*/
    var div = this.div_ = document.createElement('div');
    div.appendChild(content);
    div.style.cssText = 'position: absolute; display: none';
};

Member.prototype = new google.maps.OverlayView;

// Вызывается при создании
Member.prototype.onAdd = function () {
    var pane = this.getPanes().overlayImage;
    pane.appendChild(this.div_);

    var me = this;
    this.listeners_ = [
          google.maps.event.addListener(this, 'position_changed',
               function () { me.draw(); }),
          google.maps.event.addListener(this, 'text_changed',
               function () { me.draw(); }),
          google.maps.event.addListener(this, 'zindex_changed',
               function () { me.draw(); }),
          google.maps.event.addListener(this, 'direction_changed',
               function () { me.draw(); })
     ];
};

// Вызывается при удалении
Member.prototype.onRemove = function () {
    this.div_.parentNode.removeChild(this.div_);

    // Метка удаляется с карты, останавливаются обработчики событий
    for (var i = 0, I = this.listeners_.length; i < I; ++i) {
        google.maps.event.removeListener(this.listeners_[i]);
    }
};

// Прорисовка
Member.prototype.draw = function () {
    var projection = this.getProjection();
    var position = projection.fromLatLngToDivPixel(this.get('position'));
    var div = this.div_;
 
    div.style.left = (position.x - 12) + 'px';
    div.style.top = (position.y - 12) + 'px';
    div.style.display = 'block';
    div.style.zIndex = this.get('zIndex');
    $('#' + this.content_.id).rotate(this.get('direction'));
};