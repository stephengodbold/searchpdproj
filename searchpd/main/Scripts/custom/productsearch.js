(function() {
    var resultsHandler = function(data) {
        if (data) {
            $("#searchresults").html(data);

            $("#prev").click(prevNextHandler);
            $("#next").click(prevNextHandler);
        } else {
            $("#searchresults").html("No results found");
        }
    };

    var prevNextHandler = function() {
        var url = $(this).attr('href');
        getResults(url);
        return false;
    };

    var getResults = function(url) {
        $.ajax({
            url: url,
            success: resultsHandler,
            error: function(xhr, status, error) {
                alert(error);
            }
        });
    };

    var clickHandler = function () {
        var searchTerm = $("#search").val();
        $("#suggestions").hide();

        var url = "/SearchResults?q=" + escape(searchTerm) + "&skip=0";
        getResults(url);
        return false;
    };

    $(function() {
        $("#search-button").click(clickHandler);
    });
})();
