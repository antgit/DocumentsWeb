DXDemo = {};
DXDemo.CurrentDemoGroupKey = "";
DXDemo.CurrentDemoKey = "";
DXDemo.CurrentThemeCookieKey = "DXCurrentTheme";
DXDemo.InitDemoView = function(){
    DXDemo.CodeLoaded = TabControl.GetActiveTab().name == "Code";
    DXDemo.DemoViewChanged();
}
DXDemo.ShowThemeSelector = function(){
    ThemeSelectorPopup.Show();
}
DXDemo.ThemeSelectorButton = null;
DXDemo.ThemeSelectorPopupBorder = null;
DXDemo.ThemeSelectorPopupPopUp = function(){
    if(DXDemo.ThemeSelectorButton == null)
        DXDemo.ThemeSelectorButton = document.getElementById("ThemeSelectorButton");
    if(DXDemo.ThemeSelectorPopupBorder == null){
        DXDemo.ThemeSelectorPopupBorder = document.createElement("DIV");
        DXDemo.ThemeSelectorPopupBorder.id = "ThemeSelectorButtonBorder";
        DXDemo.ThemeSelectorButton.parentNode.appendChild(DXDemo.ThemeSelectorPopupBorder);
    }
    var themeSelectorPopupBorderHeight = 7;
    DXDemo.ThemeSelectorPopupBorder.style.height = themeSelectorPopupBorderHeight + "px";
    DXDemo.ThemeSelectorPopupBorder.style.left = (ASPxClientUtils.GetAbsoluteX(DXDemo.ThemeSelectorButton) + 1) + "px";
    DXDemo.ThemeSelectorPopupBorder.style.top = (ASPxClientUtils.GetAbsoluteY(DXDemo.ThemeSelectorButton) + DXDemo.ThemeSelectorButton.offsetHeight - themeSelectorPopupBorderHeight) + "px";
    DXDemo.ThemeSelectorPopupBorder.style.display = "block";
    DXDemo.ThemeSelectorButton.className = "selected";
}
DXDemo.ThemeSelectorPopupCloseUp = function(){
    DXDemo.ThemeSelectorPopupBorder.style.display = "none";
    DXDemo.ThemeSelectorButton.className = "";
}
DXDemo.SetCurrentTheme = function(theme){
    ThemeSelectorPopup.Hide();
    ASPxClientUtils.SetCookie(DXDemo.CurrentThemeCookieKey, theme);
    if(document.forms[0] && (!document.forms[0].onsubmit || document.forms[0].onsubmit.toString().indexOf("Sys.Mvc.AsyncForm") == -1))
        document.forms[0].submit();
    else
        window.location.reload();
}

