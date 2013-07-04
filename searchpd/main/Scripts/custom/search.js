var Searcher;
(function (Searcher) {
    // This code assumes that when the keyupHandler runs,
    // other code will have set
    // Searcher.searchUrl

    var suggestionHandler = function(data) {
        if (data) {
            $("#suggestions").show().html(data);
        } else {
            $("#suggestions").hide();
        }
    };

    var keyupHandler = function () {
        var subString = $("#search").val();

        if (subString.length < 3) {
            $("#suggestions").hide();
            return true;
        }

        // On keypress in search box, send request to server with entered sub string.
        // If server sends data, this goes in the suggestion box. If not, ensure suggestions are invisible.
        // Use a query string rather than adding the sub string to the uri as a rest item, because if you do
        // the latter the MVC framework will throw an exception when you send unsafe characters such as %.
        //$.get(Searcher.searchUrl + "?q=" + escape(subString), function (data, status) {
        //    if (data) {
        //        $("#suggestions").show().html(data);
        //    } else {
        //        $("#suggestions").hide();
        //    }
        //}, 'jsonp');



        $.ajax({
            url: Searcher.searchUrl + "?q=" + escape(subString),
            dataType: 'jsonp',
            success: suggestionHandler,
            error: function (xhr, status, error) {
                alert(error);
            }
        });





        return true;
    };

    $(function () {
        $("#search").keyup(keyupHandler);
    });
})(Searcher || (Searcher = {}));
