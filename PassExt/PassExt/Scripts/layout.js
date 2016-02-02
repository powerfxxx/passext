jQuery(document).ready(function () {
    jQuery('#scrollup img').mouseover(function () {
        jQuery(this).animate({ opacity: 0.75 }, 100);
    }).mouseout(function () {
        jQuery(this).animate({ opacity: 1 }, 100);
    }).click(function () {
        window.scroll(0, 0);
        return false;
    });

    jQuery(window).scroll(function () {
        if (jQuery(document).scrollTop() > 300) {
            jQuery('#scrollup').fadeIn('fast');
        } else {
            jQuery('#scrollup').fadeOut('fast');
        }
    });
});

//Перекртытие страницы
var pageOverlay = {
    
    show: function (message, state) {
        $("#pageOverlay-message").removeAttr("class").addClass(state).html(message);
        $("#pageOverlay").fadeIn(500).delay(3000).fadeOut(500);
    },

    hide: function() {
        $("#pageOverlay").fadeOut(500);
    }
}