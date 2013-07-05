var Refresher;
(function (Refresher) {
    var clickHandler = function () {

        $("#progress-searchresults").show().html("refreshing search results...");
        
        $.ajax({
            type: "POST",
            url: "/SearchResults/Refresh",
            data: null,
            success: function(data) {
                $("#progress-searchresults").html(data);
            },
            dataType: 'text'
        });

        return true;
    };

    $(function () {
        $("#refreshbutton").click(clickHandler);
    });
})(Refresher || (Refresher = {}));
