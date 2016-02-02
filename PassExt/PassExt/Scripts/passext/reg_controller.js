var RegController = {
    uri: 'api/reg/',

    DrawWays: function() {
        var html = "";
        $.getJSON(this.uri + 'GetWays')
            .done(function(data) {
                $.each(data, function(key, item) {
                    html += '<div class="way" data-id="' + item.ID + '" data-townfromid="' + item.TownFrom_id + '"><div class="town_from">' + item.TownFrom_Name + '</div><div class="town_to">' + item.TownTo_Name + '</div></div>';
                });
                $('#ways').html(html);
            });
    },

    DrawTimes: function(forDate, wayId) {
        var html = "";
        $.getJSON(this.uri + 'GetTimes/' + forDate + '/' + wayId)
            .done(function(data) {
                $.each(data, function(key, item) {
                    html += '<div class="time" data-id="' + item.ID + '">' + item.Start_time + '</div>';
                });
                $('#times .mCSB_container').html(html);
                setTimeout(function () { $('#times').mCustomScrollbar("update"); }, 1000);
            });
    },

    DrawTownPoints: function(townId) {
        var html = "";
        var result = [];
        $.getJSON(this.uri + 'gettownpoints/' + townId)
            .done(function(data) {
                $.each(data, function(key, item) {
                    html += '<div class="point" data-id="' + item.ID + '"><span class="point_name">' + item.Name + '</span></div>';
                });
                $('#points_box').html(html);
                setTimeout(function () { $('#points').mCustomScrollbar("update"); }, 1000);
                result = data;
            });
        return result;
    },

    InsertOrder: function(order) {
        $.ajax({
            url: this.uri + 'insertorder/' + order,
            type: 'POST',
            contentType: "application/json",
            data: JSON.stringify(order),
            success: function(result) {
                pageOverlay.show(result.Message, result.Status);
            }
        });
    },

    GetPointsTime: function(scheduleId, pointIDs, dateText) {
        var result = [];
        $.ajax(
        {
            url: this.uri + "getpointstime",
            type: "POST",
            contentType: "application/json",
            async: false,
            data: JSON.stringify({ scheduleId: scheduleId, pointIDs: pointIDs, dateText: dateText }),
            success: function(res) {
                result = res;
            }
        });
        return result;
    }
}