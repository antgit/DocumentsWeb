$(document).ready(function () {

    // —лушатель клика по пункту навигационной панели
    var onNavigateClickListener = function () {
        var index = $(this);
        var detailDiv = $("div.navpanel_details[data-content=" + index.attr("data-content") + "]");
        if (detailDiv.css("display") == 'none' && $("div.navpanel_details:visible").size() > 0) {


            $("div.navpanel_details:visible").slideToggle("normal", function () {
                detailDiv.slideToggle("normal");
            });
        }
        else {
            detailDiv.slideToggle("normal");
        }
    }

    // 

    $("div.menu_item").mouseenter(function () {
        var index = $(this);
        index.css("background-color", "#D8DFEA");
    });

    $("div.menu_item").mouseleave(function () {
        var index = $(this);
        index.css("background-color", "transparent");
    });
});