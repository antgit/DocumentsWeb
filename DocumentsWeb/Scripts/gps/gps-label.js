// HTML метка
function Label(opt_options) {
    this.setValues(opt_options);

    var content = this.content_ = document.createElement('div');
    content.id = 'lb_' + (new Date().getTime());
    content.style.cssText = 'position: relative; ' +
                          'background-color: #FFF;border: 1px solid #000;' +
                          'white-space:nowrap;padding: 2px;font-family: Arial;' +
                          'font-size: 10px;';
    var div = this.div_ = document.createElement('div');
    div.appendChild(content);
    div.style.cssText = 'position: absolute; display: none';
};

Label.prototype = new google.maps.OverlayView;

// Вызывается при создании
Label.prototype.onAdd = function () {
    var pane = this.getPanes().overlayImage;
    pane.appendChild(this.div_);

    var me = this;
    this.listeners_ = [
          google.maps.event.addListener(this, 'position_changed',
               function () { me.draw(); }),
          google.maps.event.addListener(this, 'text_changed',
               function () { me.draw(); }),
          google.maps.event.addListener(this, 'zindex_changed',
               function () { me.draw(); })
     ];
};

// Вызывается при удалении
Label.prototype.onRemove = function () {
    this.div_.parentNode.removeChild(this.div_);

    // Метка удаляется с карты, останавливаются обработчики событий
    for (var i = 0, I = this.listeners_.length; i < I; ++i) {
        google.maps.event.removeListener(this.listeners_[i]);
    }
};

// Прорисовка
Label.prototype.draw = function () {
    var projection = this.getProjection();
    var position = projection.fromLatLngToDivPixel(this.get('position'));
    var div = this.div_;

    $('#' + this.content_.id).html(this.get('text').toString());

    var width = $('#' + this.content_.id).width();
    // TODO: Костыль с высотой дива
    var height = $('#' + this.content_.id).height() == 0 ? 38 : $('#' + this.content_.id).height();

    div.style.left = (position.x + 5) + 'px';
    div.style.top = (position.y - height - 25) + 'px';
    div.style.display = 'block';
    div.style.zIndex = this.get('zIndex');
};