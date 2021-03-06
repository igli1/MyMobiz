﻿var RatesCategories = [];
var RatesDetails = [];
var SelectedRateDetail;
var ServiceRate = null;
var TargetProperty = null;
//Adds Operators to Op DropDown
$(document).ready(async function () {
    $('#Defaults').find('select').each(function () {
        var options;
        options += '<option value=""></option>';
        options += '<option value="*"> * </option>';
        options += '<option value="="> = </option>';
        options += '<option value="+"> + </option>';
        options += '<option value="-"> - </option>';
        options += '<option value="%"> % </option>';
        $('#' + this.attributes.id.nodeValue).append(options);
    });
});
async function FetchCall(url, data) {
    const response = await fetch(url, {
        method: 'POST', // *GET, POST, PUT, DELETE, etc.
        mode: 'cors', // no-cors, *cors, same-origin
        cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
        credentials: 'same-origin', // include, *same-origin, omit
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        },
        redirect: 'follow', // manual, *follow, error
        referrerPolicy: 'no-referrer', // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
        body: JSON.stringify(data) // body data type must match "Content-Type" header
    });
    return response.json(); // parses JSON response into native JavaScript objects
}
//onChange #ServiceSelect When a Service is Selected. val = ServiceId
async function ServiceSelected(val) {
    if (val != "") {
        var serviceName = $('#ServiceSelect').find(":selected").text();
        $('#ServiceId').val(val);
        $('#ServiceName').val(serviceName);
        $('#ServiceRateSelect')
            .empty()
            .append('<option value="">-Select Service Rate -</option>')
            ;
        $('#ServiceLanguageSelect').empty();

        await ResetInServiceRateSelect(); //Resets Page
        await EmptiesDefaults();
        await GetServiceRates(val);
        await GetServiceLanguages(val);
        ServiceRate = null;
    }
}
//Gets Service Languages for Selected Service. val = ServiceId.
async function GetServiceLanguages(val) {
    await $.post('GetServiceLanguages', { serviceId: val }).done(function (data) {
        var options = '';
        for (var i = 0; i < data.length; i++) {
            options += '<option value=' + data[i].id + '>' + data[i].word + '</option>';
        }
        $('#ServiceLanguageSelect').append(options);
    });
}
//Resets Page If a new Service Rate Is Selected
async function ResetInServiceRateSelect() {
    SelectedRateDetail = null;
    $('#RatesDetails')
        .empty();
    $('#RatesCategories')
        .empty();
    EmptiesRateTargets();
    $('#nQuotes').val('');
    $('#Conditions').val('');
    $('#defDate').val('').css("color", "black");
    $('#appDate').val('').css("color", "black");
    $('#endDate').val('').css("color", "black");
}
// Gets Service Rates and Fills the #ServiceRateSelect. val = ServiceId
async function GetServiceRates(val) {
    await $.post('GetServiceRates', { serviceId: val }).done(function (data) {
        var options = '';
        for (var i = 0; i < data.length; i++) {
            options += '<option value=' + data[i].verNum + '>' + data[i].verNum + ' ' + data[i].lexo + '</option>';
        }
        $('#ServiceRateSelect').append(options);
    });
}
//onChange if a Service Rate is Selected. val = VerNum
async function ServiceRateSelected(val) {
    if (val != "") {
        await ResetInServiceRateSelect();
        await $.post('GetServiceRateSelected', { vernum: val }).done(function (data) {
            if (data != null) {
                ServiceRate = data;
                if (ServiceRate.defDate != null) {
                    var date = ServiceRate.defDate.split("T")[0];
                    $('#defDate').val(date).css("color", CheckDate(date));
                }
                else
                    $('#defDate').css("color", "black");
                if (ServiceRate.appDate != null) {
                    var date = ServiceRate.appDate.split("T")[0];
                    $('#appDate').val(date).css("color", CheckDate(date));
                }
                else
                    $('#appDate').css("color", "black");
                if (ServiceRate.endDate != null) {
                    var date = ServiceRate.endDate.split("T")[0];
                    $('#endDate').val(date).css("color", CheckDate(date));
                }
                else
                    $('#endDate').css("color", "black");
                $('#verNum').val(ServiceRate.verNum);
                $('#serviceRateLexo').val(ServiceRate.lexo);
                $('#EurKmDefault').val(ServiceRate.eurKm);
                $('#EurMinDriveDefault').val(ServiceRate.eurMinDrive);
                $('#EurMinWaitDefault').val(ServiceRate.eurMinWait);
                $('#EurMinimumDefault').val(ServiceRate.eurMinimum);
                $('#MaxPaxDefault').val(ServiceRate.maxPax);
                $('#nQuotes').val(ServiceRate.nQuotes);
                $('#locked').prop('checked', ServiceRate.locked);
                AddDateTimePickUpAndOrder();
            }
        }).then(x => {
            GetRcRdRt(val);
        });
    }
}
async function AddDateTimePickUpAndOrder() {
    var date = new Date().toISOString('yyyy-MM-ddThh:mm:ss.SSS');
    var hour;
    var onehour;
    if (new Date().getHours() < 9) {
        hour = '0' + new Date().getHours();

        onehour = '0' + (new Date().getHours() + 1);
    }
    else if (new Date().getHours() == 9) {
        hour = '0' + new Date().getHours();
        onehour = (new Date().getHours() + 1);
    }
    else {
        hour = new Date().getHours();
        onehour = (new Date().getHours() + 1);
    }
    var minsSecs = date.split(":")[1] + ":" + date.split(":")[2].split('.')[0];
    date = (date.split("T")[0] + "T" + (hour) + ":" + minsSecs);
    $('#DateTimeOrder').val(date);
    date = (date.split("T")[0] + "T" + (onehour) + ":" + minsSecs);
    $('#DateTimePickUp').val(date);
}
// Checks if date is past, future or today in order to change coors. val = date
async function CheckDate(val) {
    var isoDate = new Date(Date.now()).toISOString().split("T")[0];
    if (val < isoDate)
        return "red";
    else if (val > isoDate)
        return "gray";
    else
        return "orange";
}
// Lock Inputs if Service Rate is Locked or number of Quotes is > 0.
async function LockedOrWithQuotes() {
    await AddToRatesDetailsTable(RatesDetails);
    await AddToRatesCategoriesTable(RatesCategories);
    if (ServiceRate.nQuotes > 0) {
        await EnableOrDisable(true);
        $('.bottompane').css("border-color", "red");
    }
    else if (ServiceRate.locked == true) {
        await EnableOrDisable(true);
        $('.bottompane').css("border-color", "orange");
        $('#locked').attr("disabled", false);
    }
    else {
        await EnableOrDisable(false);
        $('.bottompane').css("border-color", "gray");
        $('#locked').attr("disabled", false);
    }
}
// Enables or Disables Screen in case Service Rate is Locked or Has Quotes. val = boolean
async function EnableOrDisable(val) {
    $.amaran({
        'message': 'Editing: '+!val,
        'position': 'top right'
    });
    $('#defDate').attr("readonly", val);
    $('#appDate').attr("readonly", val);
    $('#endDate').attr("readonly", val);
    if (ServiceRate.locked == true && val == true)
        $('#locked').attr("disabled", val);
    else
        $('#locked').attr("disabled", false);
    $('#Defaults').find('input, :checkbox').each(function () {
        if (this.type == 'checkbox') {
            $('#' + this.attributes.id.nodeValue).attr("disabled", val);
        }
        else {
            $('#' + this.attributes.id.nodeValue).attr("readonly", val);
        }
    });
}
//Get Rates Categories, Rates Details and Rate Targets for Selected Service. val = VerNum.
async function GetRcRdRt(val) {
    RatesDetails = [];
    RatesCategories = [];
    var Lang = $('#ServiceLanguageSelect').find(":selected").val();
    await $.post('GetRateDetailsAndCategories', { vernum: val, langId: Lang }).done(function (data) {
        for (var i = 0; i < data.length; i++) {
            if (data[i].ratesDetails.length > 0) {
                RatesDetails.push(data[i]);
            }
            else {
                RatesCategories.push(data[i]);
            }
        }
    }).then(x => {
        LockedOrWithQuotes();
        Variables();
    });
}
//Adds data to Rates Details Table. data = RatesDetails Array
async function AddToRatesDetailsTable(data) {
    $('#nOfRateDetails').val(data.length);
    var Categorie = $('#RatesDetails').attr('data-Categorie');
    var Grouping = $('#RatesDetails').attr('data-Grouping');
    $('#RatesDetails').html('');
    var rows = "<tr> <th></th><th>" + Categorie + "</th><th>" + Grouping + "</th><th></th><th></th></tr>";
    for (var i = 0; i < data.length; i++) {
        rows += "<tr class='tr_hover' style='height: 60px;' id='tr" + data[i].id+"'>";
        if (ServiceRate.locked == true || ServiceRate.nQuotes > 0)
            rows += "<td style='padding-left: 10px;'></td>";
        else
            rows += "<td style='padding-left: 10px;'><label class='arrow ArrowDown' onclick='DeleteRateDetails(this)' id='" + data[i].id + "'></label></td>";

        if ($('#ServiceLanguageSelect').find(":selected").val() != -1)
            rows += "<td style='padding-left: 30px;' ><input  class='InputNoBorder' onChange='UpdateLexo(this)' value='" + data[i].word + "' id='" + data[i].id + "'/></td>";
        else
            rows += "<td style='padding-left: 30px;'>" + data[i].word + "</td>";
        rows += "<td><label>" + data[i].rateGrouping + "</label></td>";
        rows += "<td><label class='arrow' onclick='RateDetailSelected(this)' id='selected" + data[i].id + "' name='Arrow'></label></td>";
        rows += "<td><img id='rt" + data[i].id + "' name='Img' height='35px' width='35px' src='../Images/rate-icon.jpg' onclick='RateDetailSelected(this)'/></td>";
        rows += "</tr>";
    }
    $('#RatesDetails').append(rows);
    HasRateTargets();
    if (SelectedRateDetail != null && data != null) {
        $("tr[id ='tr" + SelectedRateDetail + "']").addClass('rdSelected');
        $("#" + SelectedRateDetail).addClass('rdBtnSelected');
    }
}
//Adds data to Rate Categories Table. data = RatesCategories Array
async function AddToRatesCategoriesTable(data) {
    var Categorie = $('#RatesCategories').attr('data-Categorie');
    var Grouping = $('#RatesCategories').attr('data-Grouping');
    $('#RatesCategories').html('');
    var rows = "<tr> <th></th><th>" + Categorie + " &nbsp;</th><th>" + Grouping + "</th><th></th></tr>";
    for (var i = 0; i < data.length; i++) {
        rows += "<tr class='tr_hover' style='height: 40px;'>";
        if (ServiceRate.locked == true || ServiceRate.nQuotes > 0)
            rows += "<td style='padding-left: 10px;'></td>";
        else
            rows += "<td style='padding-left: 10px;'><label class='arrow ArrowUp' width='10px'onclick='CreateRatesDetails(this)' id='" + data[i].id + "'></label></td>";
        if ($('#ServiceLanguageSelect').find(":selected").val() != -1)
            rows += "<td style='padding-left: 30px;'><input class='InputNoBorder' onChange = 'UpdateLexo(this)' value = '" + data[i].word + "' id = '" + data[i].id + "' /></td > ";
        else
            rows += "<td style='padding-left: 30px;'>" + data[i].word + "</td>";
        rows += "<td>" + data[i].rateGrouping + "</td>";
        rows += "</tr>";
    }
    $('#RatesCategories').append(rows);
}
//Checks if Rate Detail has Rate Targets and Changes the selected button to blue and Counts them
async function HasRateTargets() {
    for (var i = 0; i < RatesDetails.length; i++) {
        if (RatesDetails[i].ratesDetails[0].rateTargets.length > 0) {
            //$('#rt' + RatesDetails[i].id).attr('src', '../Images/rate-icon.jpg');
            $('#rt' + RatesDetails[i].id).show();
        }
        else
            $('#rt' + RatesDetails[i].id).hide();
    }
}
//onClick Deletes Rate Details from database. val= CategoryId
async function DeleteRateDetails(val) {
    if (SelectedRateDetail == val.id)
        Deselect();
    var index = await GetRateDetailsIndex(val.id);
    await DeleteVariables(index);
    var Lang = $('#ServiceLanguageSelect').find(":selected").val();
    await $.post('DeleteRateDetails', { rdId: RatesDetails[index].ratesDetails[0].id, rcId: RatesDetails[index].id, langId: Lang, verNum: ServiceRate.verNum }).done(function (data) {
            if (data != null) {
                RatesCategories.push(data);
                RatesDetails.splice(index, 1);
                AddToRatesCategoriesTable(RatesCategories);
                AddToRatesDetailsTable(RatesDetails);
        }
    }).then(function (){
        $.amaran({
            'message': "Rate Detail Deleted",
            'position': 'top right'
        });});
}
async function Deselect() {
    SelectedRateDetail = null;
    EmptiesRateTargets();
}
//onClick Creates a Rates Details. val = CategoryId.
async function CreateRatesDetails(val) {
    if (ServiceRate == null) {
        $.amaran({
            'message': 'Please Select a Service Rate',
            'position': 'top right'
        });
    }
    else if (ServiceRate.locked == true) {
        $.amaran({
            'message': 'This Service Rate is Locked',
            'position': 'top right'
        });
    }
    else {
        var index = await GetRateCategorieIndex(val.id);
        var rdId; 
        var Lang = $('#ServiceLanguageSelect').find(":selected").val();
        var ratesDetails = {
            verNum: ServiceRate.verNum,
            categoryId: RatesCategories[index].id,
            lexo: RatesCategories[index].lexo,
            detailConditions: RatesCategories[index].detailConditions
        };
        await $.post('CreateRateDetails', { ratesDetails: ratesDetails, langId: Lang} ).done(function (data) {
            if (data != null) {
                RatesDetails.push(data);
                RatesCategories.splice(index, 1);
            }
        }).then(x => {
            AddToRatesCategoriesTable(RatesCategories);
            AddToRatesDetailsTable(RatesDetails);
            rdId = x.id;
           
        }).then(function () {
            $.amaran({
                'message': "Rate Detail Created",
                'position': 'top right'
            });
        });
        await AddVariables(await GetRateDetailsIndex(rdId));
    }
}
//Gets Rates Details Index. val = CategoryId
async function GetRateDetailsIndex(val) {
    for (var i = 0; i < RatesDetails.length; i++) {
        if (RatesDetails[i].id == val) {
            return i;
        }
    }
}
//Gets Rates Category Index. val = CategoryId
async function GetRateCategorieIndex(val) {
    for (var i = 0; i < RatesCategories.length; i++) {
        if (RatesCategories[i].id == val) {
            return i;
        }
    }
}
//onClick Selects Rate Detail. val = RateCategory Id
async function RateDetailSelected(val) {
    var rdId;
    if (val.attributes.name.value =='Arrow')
        rdId = val.id.split('selected')[1];
    else if (val.attributes.name.value == 'Img')
        rdId = val.id.split('rt')[1];
    if (SelectedRateDetail != null) { //Changes Button Color to Default
        $("tr[id ='tr" + SelectedRateDetail + "']").removeClass('rdSelected');
    }
    SelectedRateDetail = rdId;
    $("tr[id ='tr" + SelectedRateDetail + "']").addClass('rdSelected');
    await HasRateTargets();
    await EmptiesRateTargets();
    var index = await GetRateDetailsIndex(SelectedRateDetail);
    await DisplayTargets(index);
    $('#Conditions').val(RatesDetails[index].ratesDetails[0].detailConditions);
    if (ServiceRate.locked == false) {
        if (RatesDetails[index].rateGrouping == 'Discount' || RatesDetails[index].rateGrouping == 'Extra')
            $('#Conditions').attr("readonly", false);
        else
            $('#Conditions').attr("readonly", true);
    }
    
}
// Empties Rate Targets in case Rate Detail is Removed or Other is Selected
async function EmptiesRateTargets() {
    $('#Defaults').find('input, :checkbox, select').each(function () {
        if (this.type == 'checkbox') {
            $('#' + this.attributes.id.nodeValue).prop('checked', false);
        }
        else if (this.attributes.name.value != 'Default') {
            $('#' + this.attributes.id.nodeValue).val('');
        }
    });
}
// Empties Defaults in case Service is Selected
async function EmptiesDefaults() {
    await $('#Defaults').find('input').each(function () {
        if (this.attributes.name.value == 'Default') {
            $('#' + this.attributes.id.nodeValue).val('');
        }
    });
    $('#defDate').val('');
    $('#appDate').val('');
    $('#endDate').val('');
    $('#verNum').val('');
    $('#serviceRateLexo').val('');
}
//Displays Rate Targets for selected Rate Detail. index = Ratedetail index
async function DisplayTargets(index) {
    for (var i = 0; i < await RatesDetails[index].ratesDetails[0].rateTargets.length; i++) {
        $('#' + RatesDetails[index].ratesDetails[0].rateTargets[i].rateTarget + 'Op').val(RatesDetails[index].ratesDetails[0].rateTargets[i].rateOperator);
        $('#' + RatesDetails[index].ratesDetails[0].rateTargets[i].rateTarget + 'Figure').val(RatesDetails[index].ratesDetails[0].rateTargets[i].rateFigure);
        $('#' + RatesDetails[index].ratesDetails[0].rateTargets[i].rateTarget + 'Checkbox').prop('checked', true);
    }
}
//If Defaults Checkbox is clicked. val = Rate Target
async function RateTargetCheckBox(val) {
    if (ServiceRate.locked == false && ServiceRate.nQuotes == 0) {
        if (!$('#' + val + 'Checkbox').is(':checked')) {
            await DeleteRateTargets(val);
        }
        else {

            $('#' + val + 'Checkbox').prop('checked', false);
        }
    }
}
//Creates Rate Target for selected Rate Detail. val = Rate Target, index Rate Detail index
async function CreateRateTargets(val, index) {
    var rt = {
        RateDetailId: RatesDetails[index].ratesDetails[0].id,
        RateTarget: val,
        RateFigure: $('#' + val + 'Figure').val(),
        RateOperator: $('#' + val + 'Op').val()
    };
    await $.post('CreateRateTarget', rt).done(function (data) {
        if (data != null) {
            RatesDetails[index].ratesDetails[0].rateTargets.push(data);
            $('#' + val + 'Checkbox').prop('checked', true);
        }
    }).then(function () {
        $.amaran({
            'message': val + ' Rate Target Created',
            'position': 'top right'
        });
        HasRateTargets();
    });
}
//Gets the Rate Target Index. rdIndex = RateDetail Index, val = Rate Target
async function RateTargetIndex(rdIndex, val) {
    for (var i = 0; i < RatesDetails[rdIndex].ratesDetails[0].rateTargets.length; i++) {
        if (RatesDetails[rdIndex].ratesDetails[0].rateTargets[i].rateTarget == val) {
            return i;
        }
    }
}
//Deletes Rate Target for selected Rate Detail. val = Rate Target
async function DeleteRateTargets(val) {
    var index = await GetRateDetailsIndex(SelectedRateDetail);
    var rtIndex = await RateTargetIndex(index, val);
    var id = RatesDetails[index].ratesDetails[0].rateTargets[rtIndex].id;
    await $.post('DeleteRateTargets', { id: id }).done(function (data) {
        RatesDetails[index].ratesDetails[0].rateTargets.splice(rtIndex, 1);
        $('#' + val + 'Op').val('');
        $('#' + val + 'Figure').val('');
        HasRateTargets();
    }).then(x=> {
        $.amaran({
            'message': 'Rate Target Deleted',
            'position': 'top right'
        });
    });
}
async function SimulateTrip() {
    if (!$('#ServiceSelect').val() == '') {
        if (ServiceRate.endDate >= new Date().toISOString('yyyy-MM-ddT')) {
            if ($('#DateTimePickUp').val() != '' && $('#Pax').val() != '' && $('#Kms').val() != '' && $('#Drive').val() != '' && $('#Wait').val() != '') {
                await UpdateCacheForSingeRate();
                var categories = [];
                await $('#OptionDiv').find('input, :checkbox').each(function () {
                    if (this.type == 'checkbox') {
                        var option = false;
                        if ($('#' + this.attributes.id.nodeValue).is(':checked'))
                            option = true;
                        categories.push({ Category: this.attributes.id.nodeValue, Option: option });
                    }
                });
                await $('#VehicleTypeDiv').find('select').each(function () {
                    if (this.type == 'select-one')
                        categories.push({ Category: $('#' + this.attributes.id.nodeValue).find(":selected").val() });

                });
                await $('#InputDiv').find('input').each(function () {
                    if (this.type == 'input') {
                        if ($('#' + this.attributes.id.nodeValue).val()!="")
                            categories.push({ Category: this.attributes.id.nodeValue, Input: $('#' + this.attributes.id.nodeValue).val() });
                    }
                });
                var dtCalculateQuote = {
                    ServiceID: $('#ServiceSelect').val(),
                    VerNum: parseInt(ServiceRate.verNum),
                    DateTimePickupTh: $('#DateTimePickUp').val(),
                    Categories: categories,
                    Passengers: parseInt($('#Pax').val()),
                    Kms: parseInt($('#Kms').val()),
                    DriveTime: parseInt($('#Drive').val()),
                    WaitTime: parseInt($('#Wait').val()),
                };
                //Rest Api Request...
                await FetchCall('https://www.igli-developing.com:44344/api/quotes/simulate', dtCalculateQuote)
                    .then(data => {
                        $('#Price').val(data.price);
                    });
            }
            else {
                $.amaran({
                    'message': 'Please fill all the data!',
                    'position': 'top right'
                });
            }
        }
        else {
            $.amaran({
                'message': 'Service Rate endDate has passed',
                'position': 'top right'
            });
        }
    }
    else {
        $.amaran({
            'message': "Please select service!",
            'position': 'top right'
        });
    }
}
// Update Defaults. val = default
async function UpdateDefaults(val) {
    if (ServiceRate != null) {
        if (val.value.length != 0) {
            var property = val.id.slice(0, val.id.length - 7);
            await $.post('UpdateDefaults', {vernum: ServiceRate.verNum, property: property, value: parseFloat(val.value) }).done(function (data) {
                $.amaran({
                    'message': data,
                    'position': 'top right'
                });
            });
        }
    }
    else {
        $.amaran({
            'message': "Please Select Service Rate",
            'position': 'top right'
        });
    }
}
// onKeyUp Insert or Update Rate Targets. val = Rate Target
async function InsertRateTargets(val) {
    if (ServiceRate != null) {
        if (SelectedRateDetail != null) {
            var property = val.id.slice(0, val.id.length - val.name.length);
            if ($('#' + property + 'Op').val() != '' && $('#' + property + 'Figure').val() != '') {
                if (TargetProperty == property) {
                    $.delay(1000);
                }
                var index = await GetRateDetailsIndex(SelectedRateDetail); //Rate Detail Index
                var exists = false;
                var rtId; // Rate Target Id
                var rtIndex; // Rate Target Index

                for (var i = 0; i < await RatesDetails[index].ratesDetails[0].rateTargets.length; i++) {
                    if (RatesDetails[index].ratesDetails[0].rateTargets[i].rateTarget == property) {
                        exists = true;
                        rtId = RatesDetails[index].ratesDetails[0].rateTargets[i].id;
                        rtIndex = i;
                        break;
                    }
                }
                if (exists == false) { //Create Rate Target
                    await CreateRateTargets(property, index);     
                }
                else { //Update Rate Target
                    var rt = {
                        id: rtId,
                        rateTarget: property,
                        RateFigure: $('#' + property + 'Figure').val(),
                        RateOperator: $('#' + property + 'Op').val()
                    };
                    await $.post('UpdateRateTarget', rt).done(function (data) {
                        RatesDetails[index].ratesDetails[0].rateTargets.slice(rtIndex, 1);
                        RatesDetails[index].ratesDetails[0].rateTargets.push(data);
                    }).then(x => {
                        $.amaran({
                            'message': property + ' Updated',
                            'position': 'top right'
                        });
                    });
                }
            }
        }
        else {
            $.amaran({
                'message': "Please Select Rate Detail",
                'position': 'top right'
            });
        }
    }
    else {
        $.amaran({
            'message': "Please Select Service Rate",
            'position': 'top right'
        });
    }
}
// onChange Update Date Time for Selected Service. val = dateTime
async function UpdateDateTime(val) {
    if (ServiceRate != null) {
        var updateDT = {
            value: val.value,
            property: val.id,
            vernum: ServiceRate.verNum
        };
        await $.post('UpdateDateTime', { value: val.value, property: val.id, vernum: ServiceRate.verNum }).done(function (data) {
            $.amaran({
                'message': data,
                'position': 'top right'
            });
        }).then(x => {
            $('#' + val.id).css("color",  CheckDate(val.value));
        });
    }
    else {
        $.amaran({
            'message': 'Please Select Service Rate',
            'position': 'top right'
        });
    }
}
//onChange Update Lock State for Selected Service Rate
async function UpdateLocked() {
    if (ServiceRate != null) {
        if ($('#locked').is(':checked'))
            ServiceRate.locked = true;
        else
            ServiceRate.locked = false;
        var updateLocked = {
            vernum: ServiceRate.verNum,
            locked: ServiceRate.locked
        };
        await $.post('UpdateLocked', { vernum: ServiceRate.verNum, locked: ServiceRate.locked }).done(function (data) {
            $.amaran({
                'message': data,
                'position': 'top right'
            });
        }).then(x => {
            LockedOrWithQuotes();
        }); 
    }
    else {
        $.amaran({
            'message': 'Please Select Service Rate',
            'position': 'top right'
        });
    }
}
async function CreateServiceModal() {
    await $.get('CreateServiceModal').done(function (data) {
        // append HTML to document, find modal and show it
        $('#modal-placeholder').html(data);
        $('#modal-placeholder').find('.modal').modal('show');
    });
}
async function CreateService() {
    var dataToSend = $('#ServiceModal').serialize();
    var actionUrl = $('#ServiceModal').attr('action');
    await $.post(actionUrl, dataToSend).done(function (data) {
        $('#modal-placeholder').find('.modal').modal('hide');
        $('#ServiceSelect')
            .append('<option value="' + data.id + '">' + data.serviceName + '</option>');
        $.amaran({
            'message': "Service Created",
            'position': 'top right'
        });
    });
}
async function CreateServiceRateModal() {
    if ($('#ServiceSelect').val() != "") {
        var Id = $('#ServiceId').val();
        await $.post('CreateServiceRateModal', { serviceId: Id }).done(function (data) {
            $('#modal-placeholder').html(data);
            $('#modal-placeholder').find('.modal').modal('show');
        });
    }
    else {
        $.amaran({
            'message': "Please Select Service",
            'position': 'top right'
        });
    }
}
async function CreateServiceRate() {
    var dataToSend = $('#ServiceRateModal').serialize();
    await $.post('CreateServiceRate', dataToSend).done(function (data) {
        $('#modal-placeholder').find('.modal').modal('hide');
        $('#ServiceRateSelect')
            .append('<option value="' + data.verNum + '">' + data.verNum + ' ' + data.lexo + '</option>');
        $.amaran({
            'message': "Service Rate Created",
            'position': 'top right'
        });
    });
}
async function CreateRateCategorieModal() {
    if ($('#ServiceSelect').val() != "") {
        var Id = $('#ServiceId').val();
        await $.post('CreateRateCategorieModal', { serviceId: Id }).done(function (data) {
            $('#modal-placeholder').html(data);
            $('#modal-placeholder').find('.modal').modal('show');
        });
    }
    else {
        $.amaran({
            'message': "Please Select Service",
            'position': 'top right'
        });
    }
}
async function CreateRateCategorie() {
    var dataToSend = $('#RateCategorieModal').serialize();
    await $.post('CreateRateCategorie', dataToSend).done(function (data) {
        $('#modal-placeholder').find('.modal').modal('hide');
        if (ServiceRate != null) {
            var rows = "<tr class='tr_hover' style='height: 40px;'>";
            if (ServiceRate.locked == true || ServiceRate.nQuotes > 0)
                rows += "<td style='padding-left: 10px;'></td>";
            else
                rows += "<td style='padding-left: 10px;'><label class='arrow ArrowUp' width='10px'onclick='CreateRatesDetails(this)' id='" + data.id + "'></label></td>";
            if ($('#ServiceLanguageSelect').find(":selected").val() != -1)
                rows += "<td style='padding-left: 30px;'><input class='InputNoBorder' onChange = 'UpdateLexo(this)' value = '" + data.lexo + "' id = '" + data.id + "' /></td > ";
            else
                rows += "<td style='padding-left: 30px;'>" + data.lexo + "</td>";
            rows += "<td>" + data.rateGrouping + "</td>";
            rows += "</tr>";
            $('#RatesCategories').append(rows);
            RatesCategories.push(data);
            $.amaran({
                'message': "Rate Categorie Created",
                'position': 'top right'
            });
        }
    });
}
async function UpdateDetailConditions() {
    if (ServiceRate != null) {
        if (SelectedRateDetail != null) {
            var index = await GetRateDetailsIndex(SelectedRateDetail); //Rate Detail Index
            await $.post('UpdateDetailConditions', { condition: $('#Conditions').val(), id: RatesDetails[index].ratesDetails[0].id}).done(function (data) {
                RatesDetails[index].ratesDetails[0].detailConditions = $('#Conditions').val();
            }).then(x => {
                $.amaran({
                    'message': x,
                    'position': 'top right'
                });
            });
        }
        else {
            $.amaran({
                'message': "Please Select Rate Detail",
                'position': 'top right'
            });
        }
    }
    else {
        $.amaran({
            'message': "Please Select Service Rate",
            'position': 'top right'
        });
    }
}
async function ServiceLanguageSelected(val) {
    if (val != '' && ServiceRate != null) {
        $('#RatesDetails')
            .empty();
        $('#RatesCategories')
            .empty();
        await GetRcRdRt(ServiceRate.verNum);
    }
}
async function UpdateLexo(val) {
    var Lang = $('#ServiceLanguageSelect').find(":selected").val();
    await $.post('InsertWord', { id: val.id, value: val.value, langId: Lang }).done(function (data) {
        $.amaran({
            'message': data,
            'position': 'top right'
        });
    });
}
async function DuplicateVerNumModal() {
    if (ServiceRate != null) {
        var serviceid = $('#ServiceSelect').find(":selected").val();
        await $.post('DuplicateVerNumModal', { vernum: ServiceRate.verNum, serviceid: serviceid }).done(function (data) {
            $('#modal-placeholder').html(data);
            $('#modal-placeholder').find('.modal').modal('show');
        });
    }
    else {
        $.amaran({
            'message': "Please Select Service Rate",
            'position': 'top right'
        });
    }
}
async function DuplicateVerNum() {
    var dataToSend = $('#DuplicateVerNumModal').serialize();
    await $.post('DuplicateVerNum', dataToSend).done(function (data) {
        $('#modal-placeholder').find('.modal').modal('hide');
        if (data != null) {
            if (data.existing == true) {
                $('#ServiceRateSelect')
                    .append('<option value="' + data.verNum + '">' + data.verNum + ' ' + data.lexo + '</option>');
                ('#ServiceRateSelect').val(data.verNum).change();
            }
            else {
                $('#ServiceSelect').val(data.serviceId).change();
                setTimeout(() => { $('#ServiceRateSelect').val(data.verNum).change(); }, 1000);
            }
            $.amaran({
                'message': 'Service Rate Duplicated',
                'position': 'top right'
            });
        }
        else {
            $.amaran({
                'message': 'Duplicate: Error',
                'position': 'top right'
            });
        }

    });
}
async function Variables() {
    $('#OptionDiv')
        .empty();
    $('#VehicleTypeDiv')
        .empty();
    $('#InputDiv')
        .empty();
    var option = "";
    var vehicle = "";
    var input = "";
    for (var i = 0; i < RatesDetails.length; i++) {
        if (RatesDetails[i].rateGrouping == "Option") {
            option += await InsertOption(i);
        }
        else if (RatesDetails[i].rateGrouping == "VehicleType") {
            vehicle += await InsertVehicles(vehicle, i);
        }
        else if (RatesDetails[i].rateGrouping == "Input") {
            input += await InsertInput(i);
        }
    }
    if (option != null) 
        $('#OptionDiv').append(option);
    
    if (vehicle != null)
        $('#VehicleTypeDiv').append(vehicle);

    if (input != null)
        $('#InputDiv').append(input);
}
async function InsertOption(i) {
    var option = "";
    option += "<span style='display: inline-block;' id='" + RatesDetails[i].lexo + "Option'>";
    option += "<label>" + RatesDetails[i].word + " &nbsp;</label>";
    option += "<input type='checkbox' id='" + RatesDetails[i].lexo + "'/>";
    option += "</span>";
    return option;
}
async function InsertVehicles(val, i) {
    var vehicle = "";
    if (val == "") {
        vehicle += "<label style='text-align: center;' id='" + RatesDetails[i].rateGrouping + "Label'>" + RatesDetails[i].rateGrouping + "</label>";
        vehicle += "<select id='" + RatesDetails[i].rateGrouping + "Select'>";
    }
    vehicle += "<option value='" + RatesDetails[i].lexo + "' >" + RatesDetails[i].word + "</option >";
    return vehicle;
}
async function InsertInput(i) {
    var input = "";
    input += "<div id='" + RatesDetails[i].lexo + "Div' >";
    input += "<label>" + RatesDetails[i].word + "</label>";
    input += "<input id='" + RatesDetails[i].lexo + "Input'/></div>";
    return input;
}
async function AddVariables(index) {
    if (RatesDetails[index].rateGrouping == "Option") {
        var option = await InsertOption(index);
        $('#OptionDiv').append(option);
    }
    else if (RatesDetails[index].rateGrouping == "VehicleType") {
        if ($("#VehicleTypeSelect").length == 0) {
            var vehicle = await InsertVehicles("", index);
            $('#VehicleTypeDiv').append(vehicle);
        }
        else {
            var vehicle = await InsertVehicles("noNull", index);
            $('#VehicleTypeSelect').append(vehicle);
        }
    }
    else if (RatesDetails[index].rateGrouping == "Input") {
        var input = await InsertInput(index);
        $('#InputDiv').append(input);
    }
}
//Deletes variables in case rate detail is deleted. index = ratedetail index
async function DeleteVariables(index) {
    if (RatesDetails[index].rateGrouping == "Option") {
        $('#' + RatesDetails[index].lexo + 'Option').remove();
    }
    else if (RatesDetails[index].rateGrouping == "VehicleType") {
        $('#VehicleTypeSelect option[value=' + RatesDetails[index].lexo + ']').remove();
        if ($('#VehicleTypeSelect option').length == 0) {
            $('#VehicleTypeSelect').remove();
            $('#VehicleTypeLabel').remove();
        }
    }
    else if (RatesDetails[index].rateGrouping == "Input")
        $('#' + RatesDetails[index].lexo + 'Div').remove();
}
async function UpdateCacheForSingeRate() {
    var rates = {
        ServiceId: $('#ServiceSelect').val(),
        VerNum: $('#ServiceRateSelect').val()
    };
    await FetchCall('https://www.igli-developing.com:44344/api/quotes/updaterate', rates).then
        (data => {
            $.amaran({
                'message': data,
                'position': 'top right'
            });
        }).catch((error) => {
            $.amaran({
                'message': error,
                'position': 'top right'
            });
        });
}
// onChaange Update Service Rate Lexo. val = Lexo.
async function UpdateServiceRateLexo(val) {
    if (ServiceRate != null) {
        if (val.value.length != 0) {
            await $.post('UpdateServiceRateLexo', { VerNum: ServiceRate.verNum, Lexo: val.value }).done(function (data, status) {
                if (data != null) {
                    ServiceRate.lexo == data;
                    $('select[id="ServiceRateSelect"]').find('option[value=' + ServiceRate.verNum + ']').text(ServiceRate.verNum + ' ' +data);
                }
            }).then(x => {
                $.amaran({
                    'message': 'Service Rate Name Updated',
                    'position': 'top right'
                });
            });
        }
    }
    else {
        $.amaran({
            'message': "Please Select Service Rate",
            'position': 'top right'
        });
    }
}
async function UpdateCache() {
    await $.post('https://www.igli-developing.com:44344/api/quotes/update').done(function (data, status) {
        if (status == "success") {
            $.amaran({
                'message': "Cache Updated",
                'position': 'top right'
            });
        }
        else {
            $.amaran({
                'message': 'Cannot Update Cache',
                'position': 'top right'
            });
        }
    });
}