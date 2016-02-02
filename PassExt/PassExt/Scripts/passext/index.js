$(function () {
    jQuery.expr[":"].Contains = jQuery.expr.createPseudo(function (arg) {
        return function (elem) {
            return jQuery(elem).text().toUpperCase().indexOf(arg.toUpperCase()) >= 0;
        };
    });

    slider.Init();
    InitMainButtons();
    reg_form.InitRegForm();
    initSpinners();
    //Погода
    setTimeout(LoadWeather(), 1);
   
});
//Загрузка погоды
function LoadWeather() {
    $.simpleWeather({
        //woeid: '2357536', //2357536
        location: "Нижний Новгорд",
        unit: "c",
        success: function (weather) {
            html = "<table><tr><td colspan=\"3\"><h3>Нижний Новгорд</h3></td></tr>";
            html += "<tr><td>Сегодня</td><td>Завтра</td><td>Послезавтра</td></tr>";
            html += "<tr><td><i class=\"icon-" + weather.code + "\"></i>" + weather.temp + "&deg;" + weather.units.temp + "</td>";
            html += "<td><i class=\"icon-" + weather.forecast[0].code + "\"></i>" + weather.forecast[0].high + "&deg;C</td>";
            html += "<td><i class=\"icon-" + weather.forecast[0].code + "\"></i>" + weather.forecast[0].high + "&deg;C</td></tr>";
            html += "</table>";
            $("#weather_nn").html(html);
        },
        error: function (error) {
            $("#weather_nn").html("<p>" + error + "</p>");
        }
    });

    $.simpleWeather({
        //woeid: '2357536', //2357536
        location: "Муром",
        unit: "c",
        success: function (weather) {
            html = "<table><tr><td colspan=\"3\"><h3>Муром</h3></td></tr>";
            html += "<tr><td>Сегодня</td><td>Завтра</td><td>Послезавтра</td></tr>";
            html += "<tr><td><i class=\"icon-" + weather.code + "\"></i>" + weather.temp + "&deg;" + weather.units.temp + "</td>";
            html += "<td><i class=\"icon-" + weather.forecast[0].code + "\"></i>" + weather.forecast[0].high + "&deg;C</td>";
            html += "<td><i class=\"icon-" + weather.forecast[0].code + "\"></i>" + weather.forecast[0].high + "&deg;C</td></tr>";
            html += "</table>";
            $("#weather_mur").html(html);
        },
        error: function (error) {
            $("#weather_mur").html("<p>" + error + "</p>");
        }
    });

    $.simpleWeather({
        //woeid: '2357536', //2357536
        location: "Кулебаки",
        unit: "c",
        success: function (weather) {
            html = "<table><tr><td colspan=\"3\"><h3>Кулебаки</h3></td></tr>";
            html += "<tr><td>Сегодня</td><td>Завтра</td><td>Послезавтра</td></tr>";
            html += "<tr><td><i class=\"icon-" + weather.code + "\"></i>" + weather.temp + "&deg;" + weather.units.temp + "</td>";
            html += "<td><i class=\"icon-" + weather.forecast[0].code + "\"></i>" + weather.forecast[0].high + "&deg;C</td>";
            html += "<td><i class=\"icon-" + weather.forecast[0].code + "\"></i>" + weather.forecast[0].high + "&deg;C</td></tr>";
            html += "</table>";
            $("#weather_kul").html(html);
        },
        error: function (error) {
            $("#weather_kul").html("<p>" + error + "</p>");
        }
    });
}
//Регистрация
reg_form = {
    steps: [],
    InitRegForm: function () {
        
        reg_form.steps = $("#reg_form").find(".step");
        reg_form.DiactivateStep3();
        reg_form.DiactivateStep4();
        reg_form.DiactivateStep5();
        //Клик на направление
        $("#ways").on("click", ".way", function() {
            $("#ways").find(".selected").removeClass("selected");
            $(this).addClass("selected");
            $("#step1").addClass("done");
            if ($(reg_form.steps[0]).is(".done") && $(reg_form.steps[1]).is(".done")) {
                reg_form.ActivateStep3();
                reg_form.DiactivateStep4();
                reg_form.DiactivateStep5();
            } else
                reg_form.DiactivateStep3();
            RegController.DrawTownPoints($(this).data('townfromid'));
            try {
                var datestr = $("#datepicker").datepicker('getDate');
                RegController.DrawTimes(('0' + datestr.getDate()).slice(-2) + '.' + ('0' + (datestr.getMonth() + 1)).slice(-2) + '.' + datestr.getFullYear(), $(this).data('id'));
            } catch (e) { }

        });
        //Клик на дату
        $("#datepicker").datepicker({
            language: "ru"
        }).on("changeDate", function() {
            $("#step2").addClass("done");
            if ($(reg_form.steps[0]).is(".done") && $(reg_form.steps[1]).is(".done")) {
                reg_form.ActivateStep3();
                reg_form.DiactivateStep4();
                reg_form.DiactivateStep5();
            } else
                reg_form.DiactivateStep3();

            var datestr = $("#datepicker").datepicker('getDate');
            RegController.DrawTimes(('0' + datestr.getDate()).slice(-2) + '.' + ('0' + (datestr.getMonth() + 1)).slice(-2) + '.' + datestr.getFullYear(), $('#ways .way.selected').data('id'));
            //setTimeout(reg_form.DrawTotalOrder(), 1);
        });
        RegController.DrawWays();
        $("#tel").inputmask("+7 (999) 999-99-99");
        $("#accept_button").on("click", function() {
            setTimeout(function() {
                reg_form.FillTotalOrder();
                RegController.InsertOrder(reg_form.totalOrder);
            }, 1);

        });
        $("#point_search").on("change", function () {
            reg_form.FilterPoints($(this).val());
        });
    },
    DiactivateStep3: function() {
        $(reg_form.steps[2]).removeClass("done").addClass("diactivated");
        $("#times").off("click", ".time");
    },
    ActivateStep3: function() {
        $(reg_form.steps[2]).removeClass("diactivated").removeClass("done").find(".selected").removeClass("selected");
        $("#times").on("click", ".time", function() {
            $("#times").find(".selected").removeClass("selected");
            $(this).addClass("selected");
            $(reg_form.steps[2]).addClass("done");
            reg_form.DiactivateStep4();
            reg_form.ActivateStep4();
            reg_form.DiactivateStep5();
            //setTimeout(reg_form.DrawTotalOrder(), 1);
        });
    },
    DiactivateStep4: function() {
        $(reg_form.steps[3]).removeClass("done").addClass("diactivated").find(".selected").removeClass("selected");
        $(reg_form.steps[3]).find('input').prop('disabled', true);
        $("#points").off("click", ".point");
        $("#points").find('.spinner').remove();
    },
    ActivateStep4: function() {
        $(reg_form.steps[3]).removeClass("diactivated").removeClass("done").find(".selected").removeClass("selected");
        $(reg_form.steps[3]).find('input').prop('disabled', false);
        $("#points").on("click", ".point", function () {
            $(this).toggleClass("selected");
            if ($("#points").find(".selected").length !== 0)
                $("#step4").addClass("done");
            else
                $("#step4").removeClass("done");
            if ($(reg_form.steps[3]).is(".done"))
                reg_form.ActivateStep5();
            else
                reg_form.DiactivateStep5();
            if ($(this).is(".selected")) {
                $(this).append('<div class="spinner"><table><tr><td rowspan="2" class="spinlabel">Число пассажиров</td><td rowspan="2" class="value" data-value="1" contenteditable="true">1</td><td class="spinplus">+</td><td rowspan="2">чел</td></tr><tr><td class="spinminus">-</td></tr></table></div>');
            } else {
                $(this).find('.spinner').remove();
            }
            setTimeout(reg_form.DrawTotalOrder(), 1);
        });
    },
    DiactivateStep5: function() {
        $(reg_form.steps[4]).removeClass("done").addClass("diactivated").find("input").prop("disabled", true);
        //$("#tel, #fam").val("");
    },
    ActivateStep5: function() {
        $(reg_form.steps[4]).removeClass("diactivated").removeClass("done").find("input").prop("disabled", false);
    },
    DrawTotalOrder: function () {
        this.FillTotalOrder();
        var townTo = $("#ways .way.selected .town_to").text();
        var townFrom = $("#ways .way.selected .town_from").text();
        var html = townTo + ' <span id="orderDate">' + this.totalOrder.Date + '</span><br/>Отправление из г.' + townFrom + '<br/>';
        html += '<div id="orderPoints">';
        $.each(this.totalOrder.Points, function(key, item) {
            html += '<div class="orderPoint" data-id="'+item.Id+'"><b class="orderPointTime">'+item.Time + '</b> ' + item.Name + ' <b class="orderPointCount">' + item.Count + '</b> чел.' + '</div>';
        });
        html += '</div>';
        $('#total_order').html(html);
    },
    totalOrder: {
        Name: "",
        Fam: "",
        Tel: "",
        WayId: "",
        ScheduleId: "",
        StartTime: "",
        Date: "",
        Points: []
    },
    FillTotalOrder: function () {
        this.totalOrder.Fam = $("#fam").val();
        this.totalOrder.Tel = $("#tel").val();
        this.totalOrder.WayId = $("#ways .way.selected").data("id");
        this.totalOrder.ScheduleId = $("#times .time.selected").data("id");
        this.totalOrder.StartTime = $("#times .time.selected").text();
        var datestr = $("#datepicker").datepicker('getDate');
        this.totalOrder.Date = ('0' + datestr.getDate()).slice(-2) + '.' + ('0' + (datestr.getMonth() + 1)).slice(-2) + '.' + datestr.getFullYear();;
        var pnts = [];
        var pntIDs = "";
        $("#points .point.selected").each(function () {
            pnts.push({
                Id: $(this).data("id"),
                Count: $(this).find('.spinner .value').data('value'),
                Time: "",
                Comment: ""
            });
            pntIDs+=$(this).data("id")+",";
        });
        var pointsWithTime = RegController.GetPointsTime(this.totalOrder.ScheduleId, pntIDs, this.totalOrder.Date);
        $.each(pointsWithTime, function (key, item) {
            $.each(pnts, function(key2, item2) {
                if (item2.Id == item.ID) {
                    item2.Time = item.TakeTime;
                    item2.Name = item.Name;
                }
            });
        });
        this.totalOrder.Points = pnts;
    },
    FilterPoints: function (text)
    {
        $("#points_box .point").addClass("hidden").filter(":Contains('" + text + "')").removeClass("hidden");
    }
}
//Главные кнопки
function InitMainButtons() {
    $("#btn_rasp").on("click", function () {
        $("html, body").animate({ scrollTop: $("#schedule_form").offset().top }, 1000, "easeInOutCubic");
    });
    $("#btn_green").on("click", function () {
        $("html, body").animate({ scrollTop: $("#reg_form").offset().top }, 1000, "easeInOutCubic");
    });
    $("#btn_blue").on("click", function () {
        $("html, body").animate({ scrollTop: $("#lcab_form").offset().top }, 1000, "easeInOutCubic");
    });
}

//Slider
slider = {
    Init: function() {
        //$('#baner').prepend('<img id="slide_img1" src="../../Content/img/bus1.jpg"/><img id="slide_img2" src="../../Content/img/baner.jpg" />');
        this.PlaySlideshow();
    },
    PlaySlideshow: function() {

        var el = $('#baner').find('img'),
            interval = 7000,
            indexImg = 1,
            indexMax = el.length;

        var change = function () {
            el.filter(':nth-child(' + indexImg + ')').animate({
                opacity: 0
            }, 750);
            indexImg++;
            if (indexImg > indexMax) {
                indexImg = 1;
            }
            el.filter(':nth-child(' + indexImg + ')').animate({
                opacity: 1
            }, 750);
        }

        var queue = setInterval(change, interval);

        $('#baner-text').mouseover(function () {
            clearInterval(queue);
        }).mouseout(function () {
            queue = setInterval(change, interval);
        });

        
       
    }
}

//Spinner
function initSpinners() {
    $('#points').on('click', '.spinner', function(e) { e.stopPropagation(); });
    $('#points').on('click', '.spinner  .spinplus', function () {
        var spinvalue = parseInt($(this).parents('.point').find('.spinner .value').text(), 10);
        if (spinvalue < 30) {
            $(this).parents('.point').find('.spinner .value').html(spinvalue + 1).data('value', spinvalue +1);
            setTimeout(reg_form.DrawTotalOrder(), 1);
        } 
    });
    $('#points').on('click', '.spinner .spinminus', function () {
        var spinvalue = parseInt($(this).parents('.point').find('.spinner .value').text(), 10);
        if (spinvalue>1) {
            $(this).parents('.point').find('.spinner .value').html(spinvalue - 1).data('value', spinvalue - 1);
        } else {
            spinvalue = 1;
            $(this).parents('.point').find('.spinner .value').html(spinvalue).data('value', spinvalue);
        }
        setTimeout(reg_form.DrawTotalOrder(), 1);
    });
}

