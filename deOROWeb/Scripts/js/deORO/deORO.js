function loadGridData(url, id, metricid, customerid, locationid, fromdate, todate) {

    var postData = JSON.stringify({
        'metricid': metricid,
        'customerid': customerid,
        'locationid': locationid,
        'fromdate': fromdate,
        'todate': todate
    });

    $.ajax({
        type: "POST",
        url: url,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: postData,
        async: true,
        cache:false,
        success: function (result) {
            if (result != null) {
                var table = $('#' + id + '-grid').dataTable({
                    "aaData": result.rows,
                    "aoColumns": result.columns,
                    "bDestroy": true,
                    "bAutoWidth": false
                });
            }
            else {
                $('#' + id + '-not-configured').show();
            }

            $('#' + id + '-overlay').hide();
            $('#' + id + '-loading-img').hide();
        },
        error: function (error) {
            $('#' + id + '-not-configured').show();
            $('#' + id + '-overlay').hide();
            $('#' + id + '-loading-img').hide();
        }
    });

};


