(function () {
    var nbrOutstandingRequests = 0;

    var sendRefreshRequest = function(url, progressId, progressMessage) {
        $("#refreshbutton").attr('disabled', 'disabled');
        $("#" + progressId).html(progressMessage);
        nbrOutstandingRequests++;

        $.ajax({
            type: "POST",
            url: url,
            data: null,
            success: function(data) {
                $("#" + progressId).html(data);
                nbrOutstandingRequests--;
                if (nbrOutstandingRequests == 0) {
                    $("#refreshbutton").removeAttr('disabled');
                }
            },
            dataType: 'text'
        });
    };

    var clickHandler = function () {
        sendRefreshRequest("/Refresh/SearchResults", "progress-searchresults", "refreshing search results...");
        sendRefreshRequest("/Refresh/Suggestions", "progress-suggestions", "refreshing suggestions...");

        return false;
    };

    $(function () {
        $("#refreshbutton").click(clickHandler);
    });
})();
