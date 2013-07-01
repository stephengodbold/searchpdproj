(function() {
    var blurHandler = function() {
        var category = $("#search").val();

        // On blur, send request to server with entered category.
        // If server sends data, this goes in the suggestion box. If not, ensure suggestions are invisible.
        $.get("/api/search/" + category, function (data, status) {
            if (data) {
                $("#suggestions").show().html(data);
            } else {
                $("#suggestions").hide();
            }
        });
    };

    $(function () {
        $("#search").blur(blurHandler);
    });
})();
