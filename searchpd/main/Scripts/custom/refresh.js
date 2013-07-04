var Refresher;
(function (Refresher) {
    // This code assumes that when the clickHandler runs,
    // other code will have set
    // Refresher.refreshUrl

    var clickHandler = function () {

        $("#progress").show();

        $.ajax({
            url: Searcher.refreshUrl,
            dataType: 'jsonp',
            success: function() {
                $("#progress").hide();
            },
            error: function (xhr, status, error) {
                $("#progress").hide();
                alert(error);
            }
        });

        return true;
    };

    $(function () {
        $("#refresh").click(clickHandler);
        $("#progress").hide();
    });
})(Refresher || (Refresher = {}));
