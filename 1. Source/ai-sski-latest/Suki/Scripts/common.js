function MouseEvents(objRef, evt) {
    if (evt.type == "mouseover") {
        objRef.style.backgroundColor = "orange";
    }
    else {
        if (evt.type == "mouseout") {
            if (objRef.rowIndex % 2 == 0) {
                objRef.style.backgroundColor = "#EEF1F7";
            }
            else {
                objRef.style.backgroundColor = "#D9E0ED";
            }
        }
    }
}
function isNumberKey(sender, evt) {
    var txt = sender.value;
    var dotcontainer = txt.split('.');
    var charCode = (evt.which) ? evt.which : event.keyCode;
    if (!(dotcontainer.length == 1 && charCode == 46) && charCode > 31 && (charCode < 48 || charCode > 57))
        return false;

    return true;
}
function mathRoundForTaxes(source) {
    var txtBox = document.getElementById(source);
    var txt = txtBox.value;
    if (!isNaN(txt) && isFinite(txt) && txt.length != 0) {
        var rounded = Math.round(txt * 100) / 100;
        txtBox.value = rounded.toFixed(2);
    }
}