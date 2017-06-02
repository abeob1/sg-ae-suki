Main = {
    myUpdatePanelId: '',
    widthDialog: 0,
    heightDialog: 0,
    init: function () {
        if ($('#isMainPage').val() == '1') {
            // layout
            $("#splitterContainer").splitter({
                minAsize: 250,
                maxAsize: 300,
                splitVertical: true,
                A: $('#leftPane'),
                B: $('#rightPane'),
                closeableto: 0
            });
            $("#splitterContainer").height($('body').height() - 93);
            // Accordion
            $("#accordion").accordion({ autoHeight: false, header: "h3" });

            // Tabs
            $('#tabs').tabs();

            //initialize dialog
            $('#dialogFrame').dialog({
                autoOpen: false,
                modal: true,
                width: this.widthDialog,
                height: this.heightDialog
            });
            //

            $(".txtDate").datepicker();
        }
        ////
        if (window.parent != null || window.parent != undefined) {
            if ($('body').height() > 0) {
                window.parent.Main.resizeIframe($('body').height());
            }
        }

    },
    rebind_init: function () {
        if ($('#isMainPage').val() == '1') {
            // Accordion
            $("#accordion").accordion({ header: "h3" });

            // Tabs
            $('#tabs').tabs();

            //rebinf dialog
            $('#dialogFrame').dialog({
                autoOpen: false,
                modal: true,
                width: this.widthDialog,
                height: this.heightDialog
            });
            $(".txtDate").datepicker();
            //
        }
        ////
        if (window.parent != null || window.parent != undefined) {
            if ($('body').height() > 0) {
                window.parent.Main.resizeIframe($('body').height());
            }
        }
    },
    resizeIframe: function (newHeight) {
        //$('#dialogFrame > #iframeItem').height(parseInt(newHeight, 10) + 20);
    },
    openCustomDialog: function (url, _width, _height, param) {
        $('#dialogFrame').dialog({
            autoOpen: false,
            modal: true,
            width: _width,
            height: _height
        });
        this.widthDialog = _width;
        this.heightDialog = _height;
        $('#dialogFrame').dialog('open');
        $('#dialogFrame > #iframeItem').empty();
        $('#dialogFrame > #iframeItem').attr('src', url + (param == '' || param == undefined ? '' : ("?" + param)));
    },
    openDialog: function (url, param) {
        var n = url.indexOf("Promo");
        if (n > 0)
            this.openCustomDialog(url, 1200, 300, param);
        else
            this.openCustomDialog(url, 600, 300, param);
    },
    okDialogClick: function (action) {
        window.parent.__doPostBack(window.parent.Main.myUpdatePanelId, action);
        window.parent.$('#dialogFrame').dialog('close');
    },
    cancelDialogClick: function () {
        window.parent.$('#dialogFrame').dialog('close');
    },
    cancelNewDialogClick: function (newurl) {
        window.parent.$('#dialogFrame').dialog('close');
        redirectPage(newurl);
    },
    redirectWithTimeout: function (url, timeout) {
        setTimeout('Main.redirectPage("' + url + '")', timeout);
    },
    updateMasterMessage: function () {
        $('#customeMessage').text('');
    },
    clearMasterMessage: function () {
        setTimeout('Main.updateMasterMessage()', 10000);
    },
    setMasterMessage: function (msg) {
        var msgdecoded = decodeURIComponent((msg + '').replace(/\+/g, '%20'));
        $('#customeMessage').text(msgdecoded);
        Main.clearMasterMessage();
    },
    integer_textbox_keypress: function (e) {
        var keynum;
        if (window.event) // IE
        {
            keynum = e.keyCode
        }
        else if (e.which) // Netscape/Firefox/Opera
        {
            keynum = e.which
        }
        if (keynum != 8 && keynum != 0 && (keynum < 48 || keynum > 57)) {
            return false;
        }
    }


}