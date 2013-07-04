(function() {
    var resultsHandler = function(data) {
        if (data) {
            $("#searchresults").html(data);
        } else {
            $("#searchresults").html("No results found");
        }
    };

    var clickHandler = function () {
        var searchTerm = $("#search").val();
        $("#suggestions").hide();

        $.ajax({
            url: "/SearchResults?q=" + escape(searchTerm),
            success: resultsHandler,
            error: function(xhr, status, error) {
                alert(error);
            }
        });

        return true;
    };

    $(function() {
        $("#search-button").click(clickHandler);
    });
})();
