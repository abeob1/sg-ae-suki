Dialog = {
    modalMessage: function(modal, okText, message) {
        $("#dialogMSG").html(message);
        $("#dialogMSG").dialog({
            modal: modal,
            buttons: {
                Ok: function() {
                    $(this).dialog("close");
                }
            }
        });
    },
    showLoader: function() {
        var docH = $(document).height();
        var docW = $(document).width();

        var progressloader_maskHeight = docH;
        var progressloader_maskWidth = docW;

        $('#progressloader_mask').css({ 'width': progressloader_maskWidth, 'height': progressloader_maskHeight });

        $('#progressloader_mask').show();

        var winH = $(window).height();
        var winW = $(window).width();

        $('#progressloader_dialog').css('top', $(document).scrollTop() + winH / 2 - $('#progressloader_dialog').height());
        $('#progressloader_dialog').css('left', winW / 2 - $('#progressloader_dialog').width());

        $('#progressloader_dialog').show();
    },
    hideLoader: function() {
        $('#progressloader_mask').hide();
        $('.progressloader_window').hide();
    }
}