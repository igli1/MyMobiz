var RatesCategories = [];
var RatesDetails = [];
var SelectedRateDetail;
var DefaultButtonColor;
var ServiceRate;
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
            .append('<option value="">-' + $('#SelectRate').text() + '-</option>')
            ;
        ResetInServiceRateSelect(); //Resets Page
        EmptiesDefaults();
        GetServiceRates(val);
    }
}
//Resets Page If a new Service Rate Is Selected
function ResetInServiceRateSelect() {
    SelectedRateDetail = null;
    $('#RatesDetails')
        .empty();
    $('#RatesCategories')
        .empty();
    EmptiesRateTargets();
    //$('#nOfCategories').val('0');
    $('#locked').prop('checked', false);
    $('#nOfRateDetails').val('0');
    $('#nOfRateTargets').val('0');
    $('#nQuotes').val('');
    $('#CatId').val('');
    $('#RdId').val('');
}
// Gets Service Rates and Fills the #ServiceRateSelect. val = ServiceId
function GetServiceRates(val) {
    FetchCall('GetServiceRates', val)
        .then(data => {
            var options = '';
            for (var i = 0; i < data.length; i++) {
                options += '<option value="' + data[i].verNum + '">' + data[i].lexo + '</option>';
            }
            $('#ServiceRateSelect').append(options);
        });
}
//onChange if a Service Rate is Selected. val = VerNum
function ServiceRateSelected(val) {
    if (val != "") {
        ResetInServiceRateSelect();
        FetchCall('GetServiceRateSelected', val)
            .then(data => {
                if (data != null) {
                    ServiceRate = data;
                    $('#defDate').val(ServiceRate.defDate);
                    $('#appDate').val(ServiceRate.appDate);
                    $('#endDate').val(ServiceRate.endDate);
                    $('#verNum').val(ServiceRate.verNum);
                    $('#EurKmDefault').val(ServiceRate.eurKm);
                    $('#EurMinDriveDefault').val(ServiceRate.eurMinDrive);
                    $('#EurMinWaitDefault').val(ServiceRate.eurMinWait);
                    $('#EurMinimumDefault').val(ServiceRate.eurMinimum);
                    $('#MaxPaxDefault').val(ServiceRate.maxPax);
                    $('#nQuotes').val(ServiceRate.nQuotes);
                    $('#locked').prop('checked', ServiceRate.locked);
                }
            }).then(x => {
                GetRcRdRt(val);
            });   
    }
}
// Lock Inputs if Service Rate is Locked or number of Quotes is > 0.
function LockedOrWithQuotes() {
    AddToRatesDetailsTable(RatesDetails);
    AddToRatesCategoriesTable(RatesCategories);   
    if (ServiceRate.nQuotes > 0) {
        EnableOrDisable(true);
        $('.bottompane').css({
            "border-color": "orange",
            "border-width": "1px",
            "border-style": "solid"
        });
    }
    else if (ServiceRate.locked==true) {
        EnableOrDisable(true);
        $('.bottompane').css({
            "border-color": "red",
            "border-width": "1px",
            "border-style": "solid"
        });
        $('#locked').attr("disabled", false);
    }
    else {
        EnableOrDisable(false);
        $('.bottompane').css({
            "border-color": "grey",
            "border-width": "1px",
            "border-style": "solid"
        });
        $('#locked').attr("disabled", false);
    }
}
function EnableOrDisable(val) {
    console.log(val);
    $('#defDate').attr("readonly", val);
    $('#appDate').attr("readonly", val);
    $('#endDate').attr("readonly", val);
    $('#Conditions').attr("readonly", val);
    $('#Defaults').find('input, :checkbox').each(function () {
        if (this.type == 'checkbox') {
            $('#' + this.attributes.id.nodeValue).attr("disabled", val);
        }
        else {
            $('#' + this.attributes.id.nodeValue).attr("readonly", val);
        }
    });
    /*$('#Defaults :input, :checkbox').each(function () {
        if (this.type == 'checkbox') {
            $('#' + this.attributes.id.nodeValue).attr("disabled", val);
        }
        else {
            $('#' + this.attributes.id.nodeValue).attr("readonly", val);
        }
    });*/
}
//Get Rates Categories, Rates Details and Rate Targets for Selected Service. val = VerNum.
async function GetRcRdRt(val) {
    FetchCall('GetRateDetailsAndCategories', val)
        .then(data => {
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
        });
}
//Adds data to Rates Details Table. data = RatesDetails Array
function AddToRatesDetailsTable(data) {
    $('#nOfRateDetails').val(data.length);
    var Categorie = $('#RatesDetails').attr('data-Categorie');
    var Grouping = $('#RatesDetails').attr('data-Grouping');
    $('#RatesDetails').html('');
    var rows = "<tr> <th></th><th>" + Categorie + "</th><th>" + Grouping + "</th><th></th></tr>";
    for (var i = 0; i < data.length; i++) {
        rows += "<tr>";
        if (ServiceRate.locked == true || ServiceRate.nQuotes > 0)
            rows += "<td></td>";
        else
            rows += "<td><button type='button' class='tableBtn' onclick='DeleteRateDetails(this.value)' value='" + data[i].id + "'>⇓</button></td>";     
        rows += "<td>" + data[i].lexo + "</td>";
        rows += "<td>" + data[i].rateGrouping + "</td>";
        rows += "<td><button type='button' onclick='RateDetailSelected(this.value)' value='" + data[i].id + "' id='selected" + data[i].id + "'>⇒</button></td>";
        rows += "</tr>";
        
    }
    $('#RatesDetails').append(rows);
    if (DefaultButtonColor == null) //Gets Default Button Color
        DefaultButtonColor = $('#selected' + data[0].id).css('background-color');
    HasRateTargets();
    if (SelectedRateDetail != null && data != null) {
        $('#selected' + SelectedRateDetail).css('background-color', 'red');
    }
}
//Adds data to Rate Categories Table. data = RatesCategories Array
function AddToRatesCategoriesTable(data) {
    //$('#nOfCategories').val(data.length);
    var Categorie = $('#RatesCategories').attr('data-Categorie');
    var Grouping = $('#RatesCategories').attr('data-Grouping');
    $('#RatesCategories').html('');
    var rows = "<tr> <th></th><th>" + Categorie + " &nbsp;</th><th>" + Grouping + "</th><th></th></tr>";
    //var rows = "<tr> <th></th><th>Categorie &nbsp;</th><th>Grouping</th><th></th></tr>";
    for (var i = 0; i < data.length; i++) {
        rows += "<tr>";
        if (ServiceRate.locked == true || ServiceRate.nQuotes > 0)
            rows += "<td></td>";
        else
            rows += "<td><button class='tableBtn' type='button' onclick='CreateRatesDetails(this.value)' value='" + data[i].id + "'>⇑</button></td>";  
        rows += "<td>" + data[i].lexo + "</td>";
        rows += "<td>" + data[i].rateGrouping + "</td>";
        rows += "</tr>";
    }
    $('#RatesCategories').append(rows);
}
//Checks if Rate Detail has Rate Targets and Changes the selected button to blue and Counts them
function HasRateTargets() {
    var count = 0;
    for (var i = 0; i < RatesDetails.length; i++) {
        if (RatesDetails[i].ratesDetails[0].rateTargets.length > 0) {
            $('#selected' + RatesDetails[i].id).css('background-color', 'blue');
            count += RatesDetails[i].ratesDetails[0].rateTargets.length;
        }
    }
    $('#nOfRateTargets').val(count);
}
//onClick Deletes Rate Details from database. val= CategoryId
function DeleteRateDetails(val) {
    var index = GetRateDetailsIndex(val)
    var con = confirm("Removing Rate Detail with Category: '" + RatesDetails[index].lexo + "' and RateDetail id: " + RatesDetails[index].ratesDetails[0].id);
    if (con == true) {
        var ids =
        {
            rdId: RatesDetails[index].ratesDetails[0].id,
            rcId: RatesDetails[index].id
        };
        FetchCall('DeleteRateDetails', ids)
            .then(data => {
                if (data != null) {
                    RatesCategories.push(data);
                    RatesDetails.splice(index, 1);
                    AddToRatesCategoriesTable(RatesCategories);
                    AddToRatesDetailsTable(RatesDetails);
                }
            });
    }
}
//onClick Creates a Rates Details. val = CategoryId.
function CreateRatesDetails(val) {
    if (ServiceRate == null) {
        alert('Please Select a Service Rate');
    }
    else if (ServiceRate.locked == true) {
        alert('This Service Rate is Locked');
    }
    else {
        var index = GetRateCategorieIndex(val)
        var con = confirm("Creating New Rate Detail with Category: '" + RatesCategories[index].lexo + "' and VerNum: " + ServiceRate.verNum);
        if (con == true) {
            var ratesDetails = {
                verNum: ServiceRate.verNum,
                categoryId: RatesCategories[index].id,
                lexo: RatesCategories[index].lexo,
                detailConditions: RatesCategories[index].detailConditions
            }
            FetchCall('CreateRateDetails', ratesDetails)
                .then(data => {
                    if (data != null) {
                        RatesDetails.push(data);
                        RatesCategories.splice(index, 1);
                        AddToRatesCategoriesTable(RatesCategories);
                        AddToRatesDetailsTable(RatesDetails);
                    }
                });
        }
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
    if (SelectedRateDetail != null) //Changes Button Color to Default
        $('#selected' + SelectedRateDetail).css('background-color', DefaultButtonColor);

    SelectedRateDetail = val;
    HasRateTargets();
    EmptiesRateTargets();
    DisplayTargets(val);
    $('#selected' + SelectedRateDetail).css('background-color', 'red'); //Changes Selected Rd button color to red
    var index = GetRateDetailsIndex(val);
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
    /*
    $('#Defaults :input, :checkbox').each(function () {
        if (this.type == 'checkbox' && this.attributes.id.nodeValue!='locked') {
            $('#' + this.attributes.id.nodeValue).prop('checked', false);
        }
        else if (this.attributes.name.value != 'Default') {
            $('#' + this.attributes.id.nodeValue).val('');
        }
    });*/
}
// Empties Defaults in case Service is Selected
function EmptiesDefaults() {
    $('#Defaults').find('input').each(function () {
        if (this.attributes.name.value == 'Default') {
            $('#' + this.attributes.id.nodeValue).val('');
        }
    });
    /*$('#Defaults :input').each(function () {
        if (this.attributes.name.value == 'Default') {
            $('#' + this.attributes.id.nodeValue).val('');
        }
    });*/
    $('#defDate').val('');
    $('#appDate').val('');
    $('#endDate').val('');
    $('#verNum').val('');
}
//Displays Rate Targets for selected Rate Detail. val = CategoryId
function DisplayTargets(val) {
    var index = GetRateDetailsIndex(val);
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
    var rateTarget = {
        RateDetailId: RatesDetails[index].ratesDetails[0].id,
        RateTarget: val,
        RateFigure: $('#' + val + 'Figure').val(),
        RateOperator: $('#' + val + 'Op').val()
    };
    FetchCall('CreateRateTarget', rateTarget)
        .then(data => {
            if (data != null) {
                RatesDetails.splice(index, 1);
                RatesDetails.push(data);
                AddToRatesDetailsTable(RatesDetails);
                $('#' + val + 'Checkbox').prop('checked', true);
            }
        });
}
//Deletes Rate Target for selected Rate Detail. val = Rate Target
function DeleteRateTargets(val) {
    var index = GetRateDetailsIndex(SelectedRateDetail);
    var id;
    var rtIndex;
    for (var i = 0; i < RatesDetails[index].ratesDetails[0].rateTargets.length; i++) {
        if (RatesDetails[index].ratesDetails[0].rateTargets[i].rateTarget == val) {
            id = RatesDetails[index].ratesDetails[0].rateTargets[i].id;
            rtIndex = i;
            break;
        }    
    }   
    FetchCall('DeleteRateTargets', id);
    RatesDetails[index].ratesDetails[0].rateTargets.splice(rtIndex, 1);
    $('#' + val + 'Op').val('');
    $('#' + val + 'Figure').val('');
}
function SimulateTrip() {
    if (!$('#ServiceSelect').val() == '') {
        if ($('#DateTimePickUp').val() != '' && $('#Pax').val() != '' && $('#Kms').val() != '' && $('#Drive').val() != '' && $('#Wait').val() != '') {
            var trip = {
                "ServiceID": $('#ServiceSelect').val(),
                "VerNum": ServiceRate.verNum,
                "DateTimePickupTh": $('#DateTimePickUp').val(),
                "Categories": [],
                "Passengers": parseInt($('#Pax').val()),
                "Kms": parseInt($('#Kms').val()),
                "DriveTime": parseInt($('#Drive').val()),
                "WaitTime": parseInt($('#Wait').val()),
            };
            //Rest Api Request...
            FetchCall('https://198.38.85.103:44344/api/quotes/simulate', trip)
                .then(data => {
                      $('#Total').val(data.price);
                });
        }
        else
            alert("Please fill all the data!");
    }
    else
        alert("Please select service!");
}
// onKeyUp Update Defaults. val = default
function UpdateDefaults(val) {
    if (ServiceRate != null) {
        if (val.value.length != 0) {
            var property = val.id.slice(0, val.id.length - 7);
            var DTDefaults = {
                vernum: ServiceRate.verNum,
                property: property,
                value: parseFloat(val.value)
            };
            FetchCall('UpdateDefaults', DTDefaults)
                .then(data => {
                    console.log(data);
                });
        }
    }
    else {
        alert("Please Select Service Rate");
    }
}
// onKeyUp Insert or Update Rate Targets. val = Rate Target
function InsertRateTargets(val) {
    if (ServiceRate != null ) {
        if (SelectedRateDetail != null) {
            var property = val.id.slice(0, val.id.length - val.name.length);
            if ($('#' + property + 'Op').val() != '' && $('#' + property + 'Figure').val() != '') {
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
                    var rateTarget = {
                        id: rtId,
                        RateFigure: $('#' + property + 'Figure').val(),
                        RateOperator: $('#' + property + 'Op').val()
                    };
                    FetchCall('UpdateRateTarget', rateTarget)
                        .then(data => {
                            console.log(data);
                        });
                    RatesDetails[index].ratesDetails[0].rateTargets[rtIndex].rateFigure = $('#' + property + 'Figure').val();
                    RatesDetails[index].ratesDetails[0].rateTargets[i].rateOperator = $('#' + property + 'Op').val();
                }
            }
        }
        alert("Please Select Rate Detail");
    }
    else {
        alert("Please Select Service Rate");
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
        FetchCall('UpdateDateTime', updateDT)
            .then(data => {
                console.log(data);
            });
    }
}
//onChange Update Lock State for Selected Service Rate
function UpdateLocked() {
    if ($('#locked').is(':checked'))
        ServiceRate.locked = true;
    else
        ServiceRate.locked = false;
    console.log(ServiceRate.locked);
    var updateLocked={
        vernum: ServiceRate.verNum,
        locked: ServiceRate.locked
    };
    FetchCall('UpdateLocked', updateLocked)
        .then(data => {
            console.log(data);
        });
    LockedOrWithQuotes();
}