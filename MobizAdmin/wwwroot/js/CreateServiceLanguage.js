var background = $('#NewBtn').css('background-color');
var existing = true;
$(document).ready(function () {
    if ($('#Lang').children('option').length == 0) {
        $('#NewPanel').attr('hidden', false);
        $('#ExistingPanel').attr('hidden', true);
        $('#NewExistingBtn').attr('hidden', true);
    }
});
function ChangePanel(val) {
    if (val.id == "ExistingBtn") {
        existing = true;
        $('#ExistingPanel').attr('hidden', false);
        $('#NewPanel').attr('hidden', true);
        $('#ExistingBtn').css('background-color', 'red');
        $('#NewBtn').css('background-color', background);
    }
    else if (val.id == "NewBtn") {
        existing = false;
        $('#NewPanel').attr('hidden', false);
        $('#ExistingPanel').attr('hidden', true);
        $('#NewBtn').css('background-color', 'red');
        $('#ExistingBtn').css('background-color', background);
    }
}
function Create() {
    if (existing == true) {
        var serviceLangs = {
            ServiceId: $('#ServiceId').val(),
            Lang: $('#Lang').find(":selected").val(),
            Existing: existing
        }
        $.post('CreateServiceLanguage', { serviceLangs: serviceLangs, ServiceName: $('#ServiceName').val() }).done(function (data) {
            $(location).attr('href', 'ServiceLanguages?ServiceId=' + data.serviceId + '&ServiceName=' + data.serviceName);
        });
    }
    else if (existing == false) {
        if ($('#NewLang').val().length == 2 || $('#NewWord').val() != '') {
            var serviceLangs = {
                ServiceId: $('#ServiceId').val(),
                Lang: $('#NewLang').val(),
                Word: $('#NewWord').val(),
                Existing: existing
            }
            $.post('CreateServiceLanguage', { serviceLangs: serviceLangs, ServiceName: $('#ServiceName').val() }).done(function (data) {
                $(location).attr('href', 'ServiceLanguages?ServiceId=' + data.serviceId +'&ServiceName='+data.serviceName);
            });
        }
        else {
            $.amaran({
                'message': 'Please Check The Input Data',
                'position': 'top right'
            });
        }
    }
}