var RatesCategories = [];
var RatesDetails = [];
var SelectedRateDetail;
var DefaultButtonColor;
var ServiceRate = null;
var TargetProperty = null;
async function FetchCall(url, data) {
    const response = await fetch(url, {
        method: 'POST', // *GET, POST, PUT, DELETE, etc.
        mode: 'cors', // no-cors, *cors, same-origin
        cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
        credentials: 'same-origin', // include, *same-origin, omit
        headers: {
            'Content-Type': 'application/json'
        },
        redirect: 'follow', // manual, *follow, error
        referrerPolicy: 'no-referrer', // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
        body: JSON.stringify(data) // body data type must match "Content-Type" header
    });
    return response.json(); // parses JSON response into native JavaScript objects
}
//onChange #ServiceSelect When a Service is Selected. val = ServiceId
function ServiceSelected(val) {
    if (val != "") {
        var serviceName = $('#ServiceSelect').find(":selected").text();
        $('#ServiceId').val(val);
        $('#ServiceName').val(serviceName);
        $('#ServiceRateSelect')
            .empty()
            .append('<option value="">-Select Service Rate -</option>')
            ;
        $('#ServiceLanguageSelect').empty();

        ResetInServiceRateSelect(); //Resets Page
        EmptiesDefaults();
        ServiceRate = null;
        GetServiceRates(val);
        GetServiceLanguages(val);
    }
}
//Gets Service Languages for Selected Service. val = ServiceId.
function GetServiceLanguages(val) {
    $.post('GetServiceLanguages', { serviceId: val }).done(function (data) {
        var options = '';
        for (var i = 0; i < data.length; i++) {
            options += '<option value=' + data[i].id + '>' + data[i].word + '</option>';
        }
        $('#ServiceLanguageSelect').append(options);
    });
}
//Resets Page If a new Service Rate Is Selected
function ResetInServiceRateSelect() {
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
function GetServiceRates(val) {
    $.post('GetServiceRates', { serviceId: val }).done(function (data) {
        var options = '';
        for (var i = 0; i < data.length; i++) {
            options += '<option value=' + data[i].verNum + '>' + data[i].verNum + ' ' + data[i].lexo + '</option>';
        }
        $('#ServiceRateSelect').append(options);
    });
}
//onChange if a Service Rate is Selected. val = VerNum
function ServiceRateSelected(val) {
    if (val != "") {
        ResetInServiceRateSelect();
        $.post('GetServiceRateSelected', { vernum: val }).done(function (data) {
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
                $('#EurKmDefault').val(ServiceRate.eurKm);
                $('#EurMinDriveDefault').val(ServiceRate.eurMinDrive);
                $('#EurMinWaitDefault').val(ServiceRate.eurMinWait);
                $('#EurMinimumDefault').val(ServiceRate.eurMinimum);
                $('#MaxPaxDefault').val(ServiceRate.maxPax);
                $('#nQuotes').val(ServiceRate.nQuotes);
                $('#locked').prop('checked', ServiceRate.locked);
                var date = new Date().toISOString('yyyy-MM-ddThh:mm:ss.SSS');
                var hour;
                var onehour;
                if (new Date().getHours() < 9) {
                    hour = '0' + new Date().getHours();
                    
                    onehour = '0' + (new Date().getHours() + 1);
                }
                else if (new Date().getHours() ==9) {
                    hour = '0' + new Date().getHours();
                    onehour = (new Date().getHours() + 1);
                }
                else {
                    hour = new Date().getHours();
                    onehour = (new Date().getHours() + 1);
                }
                console.log()
                var minsSecs = date.split(":")[1] +":"+ date.split(":")[2].split('Z')[0];
                date = (date.split("T")[0] + "T" + (hour) + ":" + minsSecs);
                $('#DateTimeOrder').val(date);
                date = (date.split("T")[0] + "T" + (onehour) + ":" + minsSecs);
                $('#DateTimePickUp').val(date);
            }
        }).then(x => {
            GetRcRdRt(val);
        });
    }
}
// Checks if date is past, future or today in order to change coors. val = date
function CheckDate(val) {
    var isoDate = new Date(Date.now()).toISOString().split("T")[0];;
    if (val < isoDate)
        return "red";
    else if (val > isoDate)
        return "gray";
    else
        return "orange";
}
// Lock Inputs if Service Rate is Locked or number of Quotes is > 0.
function LockedOrWithQuotes() {
    AddToRatesDetailsTable(RatesDetails);
    AddToRatesCategoriesTable(RatesCategories);
    if (ServiceRate.nQuotes > 0) {
        EnableOrDisable(true);
        $('.bottompane').css("border-color", "red");
    }
    else if (ServiceRate.locked == true) {
        EnableOrDisable(true);
        $('.bottompane').css("border-color", "orange");
        $('#locked').attr("disabled", false);
    }
    else {
        EnableOrDisable(false);
        $('.bottompane').css("border-color", "gray");
        $('#locked').attr("disabled", false);
    }
}
// Enables or Disables Screen in case Service Rate is Locked or Has Quotes. val = boolean
function EnableOrDisable(val) {
    $.amaran({
        'message': 'Editing: '+!val,
        'position': 'top right'
    });
    $('#defDate').attr("readonly", val);
    $('#appDate').attr("readonly", val);
    $('#endDate').attr("readonly", val);
    $('#Conditions').attr("readonly", val);
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
    $.post('GetRateDetailsAndCategories', { vernum: val, langId: Lang }).done(function (data) {
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
function AddToRatesDetailsTable(data) {
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
            rows += "<td style='padding-left: 10px;'><label class='arrow down' onclick='DeleteRateDetails(this)' id='" + data[i].id + "'></label></td>";
        if ($('#ServiceLanguageSelect').find(":selected").val()!=-1 )
            rows += "<td style='padding-left: 30px;' ><input  class='InputNoBorder' onChange='UpdateLexo(this)' value='" + data[i].word + "' id='" + data[i].id + "'/></td>";
        else
            rows += "<td style='padding-left: 30px;'>"+ data[i].word +"</td>"
        rows += "<td><label>" + data[i].rateGrouping + "</label></td>";
        rows += "<td><label class='arrow right' onclick='RateDetailSelected(this)' id='selected" + data[i].id + "' name='Arrow'></label></td>";
        rows += "<td><img id='rt" + data[i].id + "' name='Img' height='35px' width='35px' src='../Images/rate-icon.jpg' onclick='RateDetailSelected(this)'/></td>"
        rows += "</tr>";
    }
    $('#RatesDetails').append(rows);
    HasRateTargets();
    if (SelectedRateDetail != null && data != null) {
        $("tr[id ='tr" + SelectedRateDetail + "']").addClass('rdSelected');
    }
}
//Adds data to Rate Categories Table. data = RatesCategories Array
function AddToRatesCategoriesTable(data) {
    var Categorie = $('#RatesCategories').attr('data-Categorie');
    var Grouping = $('#RatesCategories').attr('data-Grouping');
    $('#RatesCategories').html('');
    var rows = "<tr> <th></th><th>" + Categorie + " &nbsp;</th><th>" + Grouping + "</th><th></th></tr>";
    for (var i = 0; i < data.length; i++) {
        rows += "<tr class='tr_hover' style='height: 40px;'>";
        if (ServiceRate.locked == true || ServiceRate.nQuotes > 0)
            rows += "<td style='padding-left: 10px;'></td>";
        else
            rows += "<td style='padding-left: 10px;'><label class='arrow up' width='10px'onclick='CreateRatesDetails(this)' id='" + data[i].id + "'></label></td>";
        if ($('#ServiceLanguageSelect').find(":selected").val() != -1)
            rows += "<td style='padding-left: 30px;'><input class='InputNoBorder' onChange = 'UpdateLexo(this)' value = '" + data[i].word + "' id = '" + data[i].id + "' /></td > ";
        else
            rows += "<td style='padding-left: 30px;'>" + data[i].word + "</td>"
        rows += "<td>" + data[i].rateGrouping + "</td>";
        rows += "</tr>";
    }
    $('#RatesCategories').append(rows);
}
//Checks if Rate Detail has Rate Targets and Changes the selected button to blue and Counts them
function HasRateTargets() {
    for (var i = 0; i < RatesDetails.length; i++) {
        if (RatesDetails[i].ratesDetails[0].rateTargets.length > 0) {
            //$('#rt' + RatesDetails[i].id).attr('src', '../Images/rate-icon.jpg');
            $('#rt' + RatesDetails[i].id).show();
        }
        else
            $('#rt' + RatesDetails[i].id).hide();
    };
}
//onClick Deletes Rate Details from database. val= CategoryId
function DeleteRateDetails(val) {
    var index = GetRateDetailsIndex(val.id);
    var Lang = $('#ServiceLanguageSelect').find(":selected").val();
    $.post('DeleteRateDetails', { rdId: RatesDetails[index].ratesDetails[0].id, rcId: RatesDetails[index].id, langId: Lang}).done(function (data) {
            if (data != null) {
                RatesCategories.push(data);
                RatesDetails.splice(index, 1);
                AddToRatesCategoriesTable(RatesCategories);
                AddToRatesDetailsTable(RatesDetails);
        }
    }).then(function (){
        $.amaran({
            'message': "Removing Rate Detail with Category: '" + RatesDetails[index].lexo + "' and RateDetail id: " + RatesDetails[index].ratesDetails[0].id,
            'position': 'top right'
        });});
    DeleteVariables(index);
}
//onClick Creates a Rates Details. val = CategoryId.
function CreateRatesDetails(val) {
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
        var index = GetRateCategorieIndex(val.id);
        var Lang = $('#ServiceLanguageSelect').find(":selected").val();
        var ratesDetails = {
            verNum: ServiceRate.verNum,
            categoryId: RatesCategories[index].id,
            lexo: RatesCategories[index].lexo,
            detailConditions: RatesCategories[index].detailConditions
        };
        $.post('CreateRateDetails', { ratesDetails: ratesDetails, langId: Lang} ).done(function (data) {
            if (data != null) {
                RatesDetails.push(data);
                RatesCategories.splice(index, 1);
            }
        }).then(x => {
            AddToRatesCategoriesTable(RatesCategories);
            AddToRatesDetailsTable(RatesDetails);
            AddVariables(GetRateDetailsIndex(x.id));
        }).then(function () {
            $.amaran({
                'message': "Creating New Rate Detail with Category: '" + RatesCategories[index].word + "' and VerNum: " + ServiceRate.verNum,
                'position': 'top right'
            });
        });
    }
}
//Gets Rates Details Index. val = CategoryId
function GetRateDetailsIndex(val) {
    for (var i = 0; i < RatesDetails.length; i++) {
        if (RatesDetails[i].id == val) {
            return i;
        }
    }
}
//Gets Rates Category Index. val = CategoryId
function GetRateCategorieIndex(val) {
    for (var i = 0; i < RatesCategories.length; i++) {
        if (RatesCategories[i].id == val) {
            return i;
        }
    }
}
//onClick Selects Rate Detail. val = RateCategory Id
function RateDetailSelected(val) {
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
    HasRateTargets();
    EmptiesRateTargets();
    var index = GetRateDetailsIndex(SelectedRateDetail);
    DisplayTargets(index);
    $('#Conditions').val(RatesDetails[index].ratesDetails[0].detailConditions);
}
//Block special characters for Rate Operator
function blockSpecialChar(event) {
    var e = event.key;
    return (e == '+' || e == '*' || e == '%' || e == '=' || e == '-');
}
// Empties Rate Targets in case Rate Detail is Removed or Other is Selected
function EmptiesRateTargets() {
    $('#Defaults').find('input, :checkbox').each(function () {
        if (this.type == 'checkbox') {
            $('#' + this.attributes.id.nodeValue).prop('checked', false);
        }
        else if (this.attributes.name.value != 'Default') {
            $('#' + this.attributes.id.nodeValue).val('');
        }
    });
}
// Empties Defaults in case Service is Selected
function EmptiesDefaults() {
    $('#Defaults').find('input').each(function () {
        if (this.attributes.name.value == 'Default') {
            $('#' + this.attributes.id.nodeValue).val('');
        }
    });
    $('#defDate').val('');
    $('#appDate').val('');
    $('#endDate').val('');
    $('#verNum').val('');
}
//Displays Rate Targets for selected Rate Detail. index = Ratedetail index
function DisplayTargets(index) {
    for (var i = 0; i < RatesDetails[index].ratesDetails[0].rateTargets.length; i++) {
        $('#' + RatesDetails[index].ratesDetails[0].rateTargets[i].rateTarget + 'Op').val(RatesDetails[index].ratesDetails[0].rateTargets[i].rateOperator);
        $('#' + RatesDetails[index].ratesDetails[0].rateTargets[i].rateTarget + 'Figure').val(RatesDetails[index].ratesDetails[0].rateTargets[i].rateFigure);
        $('#' + RatesDetails[index].ratesDetails[0].rateTargets[i].rateTarget + 'Checkbox').prop('checked', true);
    }
}
//If Defaults Checkbox is clicked. val = Rate Target
function RateTargetCheckBox(val) {
    if (ServiceRate.locked == false && ServiceRate.nQuotes == 0) {
        if (!$('#' + val + 'Checkbox').is(':checked')) {
            DeleteRateTargets(val);
        }
        else {

            $('#' + val + 'Checkbox').prop('checked', false);
        }
    }
}
//Creates Rate Target for selected Rate Detail. val = Rate Target, index Rate Detail index
function CreateRateTargets(val, index) {
    var rt = {
        RateDetailId: RatesDetails[index].ratesDetails[0].id,
        RateTarget: val,
        RateFigure: $('#' + val + 'Figure').val(),
        RateOperator: $('#' + val + 'Op').val()
    };
    $.post('CreateRateTarget', rt).done(function (data) {
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
function RateTargetIndex(rdIndex, val) {
    for (var i = 0; i < RatesDetails[rdIndex].ratesDetails[0].rateTargets.length; i++) {
        if (RatesDetails[rdIndex].ratesDetails[0].rateTargets[i].rateTarget == val) {
            return i;
        }
    }
}
//Deletes Rate Target for selected Rate Detail. val = Rate Target
function DeleteRateTargets(val) {
    var index = GetRateDetailsIndex(SelectedRateDetail);
    var rtIndex = RateTargetIndex(index, val);
    var id = RatesDetails[index].ratesDetails[0].rateTargets[rtIndex].id;
    $.post('DeleteRateTargets', { id: id }).done(function (data) {
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
function SimulateTrip() {
    if (!$('#ServiceSelect').val() == '') {
        if (ServiceRate.defDate > new Date()) {
            if ($('#DateTimePickUp').val() != '' && $('#Pax').val() != '' && $('#Kms').val() != '' && $('#Drive').val() != '' && $('#Wait').val() != '') {
                var categories = [];
                $('#Option').find('input, :checkbox').each(function () {
                    if (this.type == 'checkbox') {
                        var option = false;
                        if ($('#' + this.attributes.id.nodeValue).is(':checked'))
                            option = true;
                        categories.push({ Category: this.attributes.id.nodeValue, Option: option });
                    }
                });
                $('#VehicleType').find('select').each(function () {
                    if (this.type == 'select-one')
                        categories.push({ Category: $('#' + this.attributes.id.nodeValue).find(":selected").val() });

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
                FetchCall('https://www.igli-developing.com:44344/api/quotes/simulate', dtCalculateQuote)
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
                'message': 'Service Rate defDate has passed',
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
// onKeyUp Update Defaults. val = default
function UpdateDefaults(val) {
    if (ServiceRate != null) {
        if (val.value.length != 0) {
            var property = val.id.slice(0, val.id.length - 7);
            $.post('UpdateDefaults', {vernum: ServiceRate.verNum, property: property, value: parseFloat(val.value) }).done(function (data) {
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
function InsertRateTargets(val) {
    if (ServiceRate != null) {
        if (SelectedRateDetail != null) {
            var property = val.id.slice(0, val.id.length - val.name.length);
            if ($('#' + property + 'Op').val() != '' && $('#' + property + 'Figure').val() != '') {
                if (TargetProperty == property) {
                    $.delay(1000);
                }
                var index = GetRateDetailsIndex(SelectedRateDetail); //Rate Detail Index
                var exists = false;
                var rtId; // Rate Target Id
                var rtIndex // Rate Target Index

                for (var i = 0; i < RatesDetails[index].ratesDetails[0].rateTargets.length; i++) {
                    if (RatesDetails[index].ratesDetails[0].rateTargets[i].rateTarget == property) {
                        exists = true;
                        rtId = RatesDetails[index].ratesDetails[0].rateTargets[i].id;
                        rtIndex = i;
                        break;
                    }
                }
                if (exists == false) { //Create Rate Target
                    CreateRateTargets(property, index);     
                }
                else { //Update Rate Target
                    var rt = {
                        id: rtId,
                        rateTarget: property,
                        RateFigure: $('#' + property + 'Figure').val(),
                        RateOperator: $('#' + property + 'Op').val()
                    };
                    $.post('UpdateRateTarget', rt).done(function (data) {
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
function UpdateDateTime(val) {
    if (ServiceRate != null) {
        var updateDT = {
            value: val.value,
            property: val.id,
            vernum: ServiceRate.verNum
        }
        $.post('UpdateDateTime', { value: val.value, property: val.id, vernum: ServiceRate.verNum }).done(function (data) {
            $.amaran({
                'message': data,
                'position': 'top right'
            });
        }).then(x => {
            $('#' + val.id).css("color", CheckDate(val.value));
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
function UpdateLocked() {
    if (ServiceRate != null) {
        if ($('#locked').is(':checked'))
            ServiceRate.locked = true;
        else
            ServiceRate.locked = false;
        var updateLocked = {
            vernum: ServiceRate.verNum,
            locked: ServiceRate.locked
        };
        $.post('UpdateLocked', { vernum: ServiceRate.verNum, locked: ServiceRate.locked }).done(function (data) {
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
function CreateServiceModal() {
    $.get('CreateServiceModal').done(function (data) {
        // append HTML to document, find modal and show it
        $('#modal-placeholder').html(data);
        $('#modal-placeholder').find('.modal').modal('show');
    });
}
function CreateService() {
    var dataToSend = $('#ServiceModal').serialize();
    var actionUrl = $('#ServiceModal').attr('action');
    $.post(actionUrl, dataToSend).done(function (data) {
        $('#modal-placeholder').find('.modal').modal('hide');
        $('#ServiceSelect')
            .append('<option value="' + data.id + '">' + data.serviceName + '</option>');
        $.amaran({
            'message': "Service Created",
            'position': 'top right'
        });
    });
}
function CreateServiceRateModal() {
    if ($('#ServiceSelect').val() != "") {
        var Id = $('#ServiceId').val();
        $.post('CreateServiceRateModal', { serviceId: Id }).done(function (data) {
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
function CreateServiceRate() {
    var dataToSend = $('#ServiceRateModal').serialize();
    $.post('CreateServiceRate', dataToSend).done(function (data) {
        $('#modal-placeholder').find('.modal').modal('hide');
        $('#ServiceRateSelect')
            .append('<option value="' + data.verNum + '">' + data.verNum + ' ' + data.lexo + '</option>');
        $.amaran({
            'message': "Service Rate Created",
            'position': 'top right'
        });
    });
}
function CreateRateCategorieModal() {
    if ($('#ServiceSelect').val() != "") {
        var Id = $('#ServiceId').val();
        $.post('CreateRateCategorieModal', { serviceId: Id }).done(function (data) {
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
function CreateRateCategorie() {
    var dataToSend = $('#RateCategorieModal').serialize();
    $.post('CreateRateCategorie', dataToSend).done(function (data) {
        $('#modal-placeholder').find('.modal').modal('hide');
        if (ServiceRate != null) {
            var rows = "<tr class='tr_hover'>";
            if (ServiceRate.locked == true || ServiceRate.nQuotes > 0)
                rows += "<td></td>";
            else
                rows += "<td><button class='tableBtn' type='button' onclick='CreateRatesDetails(this.value)' value='" + data.id + "'>⇑</button></td>";
            rows += "<td><input class='InputNoBorder' onChange = 'UpdateLexo(this)' value = '" + data.lexo + "' id = '" + data.id + "' /></td > ";
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
function DuplicateVernum() {
    if (ServiceRate != null) {
        var servicerate = {
            serviceID: $('#ServiceId').val(),
            lexo : ServiceRate.lexo,
            locked : false,
            appDate : ServiceRate.appDate,
            defDate : ServiceRate.defDate,
            endDate : ServiceRate.endDate,
            eurKm : ServiceRate.eurKm,
            eurMinDrive : ServiceRate.eurMinDrive,
            eurMinimum : ServiceRate.eurMinimum,
            eurMinWait : ServiceRate.eurMinWait,
            maxPax : ServiceRate.maxPax,
            ratedetails: newRateDetails()
        }
        $.post('DuplicateVerNum', servicerate).done(function (data) {
            $('#ServiceRateSelect')
                .append('<option value="' + data.verNum + '">' + data.verNum + ' ' + data.lexo + '</option>');
            $.amaran({
                'message': 'Service Rate Duplicated',
                'position': 'top right'
            });
        }).then(x => {
            $('#ServiceRateSelect').val(x.verNum).change();
        });   
    }
    else {
        $.amaran({
            'message': "Please Select Service Rate",
            'position': 'top right'
        });
    }
}
function newRateDetails() {
    var rd = [];
    for (var i = 0; i < RatesDetails.length; i++) {
        rd.push(RatesDetails[i].ratesDetails[0]);
        rd[i]['categoryId'] = RatesDetails[i].id;
    }
    for (var i = 0; i < rd.length; i++) {
        delete rd[i].id;
        delete rd[i].vernum;
        for (var j = 0; j < rd[i].rateTargets.length; j++) {
            delete rd[i].rateTargets[j].id;
        }
    }
    return rd;
}
function UpdateDetailConditions() {
    if (ServiceRate != null) {
        if (SelectedRateDetail != null) {
            var index = GetRateDetailsIndex(SelectedRateDetail); //Rate Detail Index
            $.post('UpdateDetailConditions', { condition: $('#Conditions').val(), id: RatesDetails[index].ratesDetails[0].id}).done(function (data) {
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
function ServiceLanguageSelected(val) {
    if (val != '' && ServiceRate != null) {
        $('#RatesDetails')
            .empty();
        $('#RatesCategories')
            .empty();
        GetRcRdRt(ServiceRate.verNum);
    }
}
function UpdateLexo(val) {
    var Lang = $('#ServiceLanguageSelect').find(":selected").val();
    $.post('InsertWord', { id: val.id, value: val.value, langId: Lang }).done(function (data) {
        $.amaran({
            'message': data,
            'position': 'top right'
        });
    });
}
function DuplicateVerNumOtherServiceModal() {
    if (ServiceRate != null) {
        $.post('DuplicateVerNumModal', { vernum: ServiceRate.verNum }).done(function (data) {
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
function DuplicateVerNumOtherService() {
    var dataToSend = $('#DuplicateVerNumOtherServiceModal').serialize();
    $.post('DuplicateVerNumOtherService', dataToSend).done(function (data) {
        $('#modal-placeholder').find('.modal').modal('hide');
        if (data != null) {
            $('#ServiceSelect').val(data.serviceId).change();
            setTimeout(() => { $('#ServiceRateSelect').val(data.verNum).change(); }, 1000);
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
    $('#Option')
        .empty();
    $('#VehicleType')
        .empty();
    var option = "";
    var vehicle="";
    for (var i = 0; i < RatesDetails.length; i++) {
        if (RatesDetails[i].rateGrouping == "Option") {
            option += "<span style='display: inline-block;' id='" + RatesDetails[i].lexo + "Option'>"
            option += "<label>" + RatesDetails[i].word +" &nbsp;</label>";
            option += "<input type='checkbox' id='" + RatesDetails[i].lexo + "'/>"
            option +="</span>"
        }
        else if (RatesDetails[i].rateGrouping == "VehicleType") {
            if (vehicle == "") {
                vehicle += "<label style='text-align: center;' id='" + RatesDetails[i].rateGrouping + "Label'>" + RatesDetails[i].rateGrouping + "</label>";
                vehicle += "<select id='" + RatesDetails[i].rateGrouping + "Select'>";
            }
            vehicle += "<option value='" + RatesDetails[i].lexo + "' >" + RatesDetails[i].word +"</option >";
        }
    }
    if (option != "")
        $('#Option').append(option);
    if (vehicle != "") {
        vehicle += "</select >";
        $('#VehicleType').append(vehicle);
    }
}
//Deletes variables in case rate detail is deleted. index = ratedetail index
function DeleteVariables(index) {
    if (RatesDetails[index].rateGrouping == "Option") {
        $('#' + RatesDetails[index].lexo + 'Option').remove();
    }
    else if (RatesDetails[index].rateGrouping == "VehicleType") {
        $('#VehicleTypeSelect option[value=' + RatesDetails[index].lexo + ']').remove();
        if ($('#VehicleTypeSelect option').length==0) {
            $('#VehicleTypeSelect').remove();
            $('#VehicleTypeLabel').remove();
        }
    }
}
function AddVariables(index) {
    if (RatesDetails[index].rateGrouping == "Option") {
        var option = "";
        option += "<span style='display: inline-block;' id='" + RatesDetails[index].lexo + "Option'>"
        option += "<label>" + RatesDetails[index].word + " &nbsp;</label>";
        option += "<input type='checkbox' id='" + RatesDetails[index].lexo + "'/>"
        option += "</span>"
        $('#Option').append(option);
    }
    else if (RatesDetails[index].rateGrouping == "VehicleType") {
        var vehicle=""
        if ($("#VehicleTypeSelect").length == 0) {
            vehicle += "<label style='text-align: center;' id='" + RatesDetails[index].rateGrouping + "Label'>" + RatesDetails[index].rateGrouping + "</label>";
            vehicle += "<select id='" + RatesDetails[index].rateGrouping + "Select'>";
            vehicle += "<option value='" + RatesDetails[index].lexo + "' >" + RatesDetails[index].word + "</option >";
            $('#VehicleType').append(vehicle);
        }
        else {
            vehicle += "<option value='" + RatesDetails[index].lexo + "' >" + RatesDetails[index].word + "</option >";
            $('#VehicleTypeSelect').append(vehicle);
        }
    }
}
function UpdateCache() {
    $.post('https://www.igli-developing.com:44344/api/quotes/update', { key: 'Hell Yeah' }).done(function (data) {
    })
}