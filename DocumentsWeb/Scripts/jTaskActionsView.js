function OnViewTaskControlMenuItemClick(s, e) {
        switch (e.item.name) {
            case "mnuClose":
                window.open('', '_self', '');
                window.close();
                break;
        }
    }