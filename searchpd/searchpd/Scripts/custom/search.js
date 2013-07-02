(function() {
    var keyupHandler = function () {
        var subString = $("#search").val();

        if (subString.length < 3) {
            $("#suggestions").hide();
            return true;
        }

        // On keypress in search box, send request to server with entered sub string.
        // If server sends data, this goes in the suggestion box. If not, ensure suggestions are invisible.
        $.get("/api/search/" + subString, function (data, status) {
            if (data) {
                $("#suggestions").show().html(data);
            } else {
                $("#suggestions").hide();
            }
        });

        return true;
    };

    $(function () {
        $("#search").keyup(keyupHandler);
    });
})();
