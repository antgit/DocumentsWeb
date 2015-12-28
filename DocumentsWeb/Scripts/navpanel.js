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

    $('.navpanel_views').click(onNavigateClickListener);

    $('.navpanel_docs').click(onNavigateClickListener);

    $('.navpanel_agents').click(onNavigateClickListener);

    $('.navpanel_entities').click(onNavigateClickListener);

    $('.navpanel_user').click(onNavigateClickListener);

});