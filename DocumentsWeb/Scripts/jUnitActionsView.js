﻿    function OnViewUnitControlMenuItemClick(s, e) {
        switch (e.item.name) {
            case "mnuClose":
                window.open('', '_self', '');
                window.close();
                break;
            case "mnuSave":
                document.getElementById('editForm').submit();
                break;
        }
    }