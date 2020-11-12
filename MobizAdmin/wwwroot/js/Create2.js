var RatesCategories = [];
var RatesDetails = [];
var SelectedRateDetail;
var DefaultButtonColor;
var ServiceRate;
//Ajax Call Function.
function AjaxCall(url, data, type, async) {
    if (typeof async === "undefined")
        async = true;
    return $.ajax({
        url: url,
        type: type ? type : 'GET',
        async: async,
        data: data,
        contentType: 'application/json',
        dataType: 'json'
    });
}
//onChange #ServiceSelect When a Service is Selected. val = ServiceId
function ServiceSelected(val) {
    if (val != "") {
        var serviceName = $('#ServiceSelect').find(":selected").text();
        $('#ServiceId').val(val);
        $('#ServiceName').val(serviceName);
        ResetInServiceRateSelect(); //Resets Page
        //GetRcRdRt(val); //Get Rc Rd Rt
        GetServiceRates(val);
        GetServiceLangs(val);
    }
}
//Resets Page If a new Service Is Selected
function ResetInServiceRateSelect() {
    SelectedRateDetail = null;
    $('#create').trigger("reset");
    $('#create').trigger("change");
    $('#ServiceRateSelect')
        .empty()
        .append('<option value="">-' + $('#SelectRate').text() + '-</option>')
        ; 
    $('#LanguageSelect')
        .empty()
        .append('<option value="">-Default-</option>')
        ;
    $('#RatesDetails')
        .empty();
    $('#RatesCategories')
        .empty();
    EmptiesRateTargets();
}
//Get Rates Categories, Rates Details and Rate Targets for Selected Service. val = ServiceId, VerNum
function GetRcRdRt(val) {
    RatesCategories = [];
    RatesDetails = [];
    AjaxCall('GetRateDetailsAndCategories', JSON.stringify(val), 'POST').done(function (response) {
        if (response.length > 0) {
            for (var i = 0; i < response.length; i++) {
                if (response[i].ratesDetails.length > 0) {
                    RatesDetails.push(response[i]);
                }
                else {
                    RatesCategories.push(response[i]);
                }
            }
            AddToRatesDetailsTable(RatesDetails);
            AddToRatesCategoriesTable(RatesCategories);
        }
    }).fail(function (error) {
        alert(error.StatusText);
    });
}
//Adds data to Rates Details Table. data = RatesDetails Array
function AddToRatesDetailsTable(data) {
    $('#nOfRateDetails').val(data.length);
    var Categorie = $('#RatesDetails').attr('data-Categorie');
    var Grouping = $('#RatesDetails').attr('data-Grouping');
    $('#RatesDetails').html('');
    var rows = "<tr> <th></th><th>" + Categorie + "</th><th>" + Grouping +"</th><th></th></tr>";
    for (var i = 0; i < data.length; i++) {
        rows += "<tr>";
        if (data[i].ratesDetails[0].locked == false && data[i].ratesDetails[0].nQuotes == 0)
            rows += "<td><button type='button' class='tableBtn' onclick='DeleteRateDetails(this.value)' value='" + data[i].id + "'>⇓</button></td>";
        else
            rows += "<td></td>";
        rows += "<td>" + data[i].lexo + "</td>";
        rows += "<td>" + data[i].grouping + "</td>";
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
    $('#nOfCategories').val(data.length);
    var Categorie = $('#RatesCategories').attr('data-Categorie');
    var Grouping = $('#RatesCategories').attr('data-Grouping');
    $('#RatesCategories').html('');
    var rows = "<tr> <th></th><th>" + Categorie + " &nbsp;</th><th>" + Grouping + "</th><th></th></tr>";
    //var rows = "<tr> <th></th><th>Categorie &nbsp;</th><th>Grouping</th><th></th></tr>";
    for (var i = 0; i < data.length; i++) {
        rows += "<tr>";
        rows += "<td><button class='tableBtn' type='button' onclick='CreateRatesDetails(this.value)' value='" + data[i].id + "'>⇑</button></td>";
        rows += "<td>" + data[i].lexo + "</td>";
        rows += "<td>" + data[i].grouping + "</td>";
        rows += "</tr>";
    }
    $('#RatesCategories').append(rows);
}
// Gets Service Rates and Fills the #ServiceRateSelect. val = ServiceId
function GetServiceRates(val) {
    AjaxCall('GetServiceRates', JSON.stringify(val), 'POST').done(function (response) {
        if (response.length > 0) {
            var options = '';
            for (var i = 0; i < response.length; i++) {
                options += '<option value="' + response[i].verNum + '">' + response[i].lexo + '</option>';
            }
            $('#ServiceRateSelect').append(options);
        }
    }).fail(function (error) {
        alert(error.StatusText);
    });
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
    var cat = RatesDetails.find(e => e.id == val);
    $('#Conditions').val(cat.ratesDetails[0].conditions);
    $('#CatId').val(val);
    $('#RdId').val(cat.ratesDetails[0].id);
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
//onChange if a Service Rate is Selected. val = VerNum
function ServiceRateSelected(val) {
    if (val != "") {
        AjaxCall('GetServiceRateSelected', JSON.stringify(val), 'POST').done(function (response) {
            if (response != null) {
                ServiceRate = response;
                $('#defDate').val(ServiceRate.defDate);
                $('#appDate').val(ServiceRate.appDate);
                $('#endDate').val(ServiceRate.endDate);
                $('#verNum').val(ServiceRate.verNum);
            }
        }).fail(function (error) {
            alert(error.StatusText);
        });
        ResetInServiceRateSelect();
        GetRcRdRt(val);
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
                detailConditions: RatesCategories[index].conditions
            }
            AjaxCall('CreateRateDetails', JSON.stringify(ratesDetails), 'POST').done(function (response) {
                if (response != null) {
                    RatesDetails.push(response);
                    RatesCategories.splice(index, 1);
                    AddToRatesCategoriesTable(RatesCategories);
                    AddToRatesDetailsTable(RatesDetails);
                    alert('New Rate Detail Created');
                }
                else
                    alert("Something went wrong!!!");
            }).fail(function (error) {
                alert(error.StatusText);
            });
        }
    }
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
        AjaxCall('DeleteRateDetails', JSON.stringify(ids), 'POST').done(function (response) {
            if (response != null) {
                RatesCategories.push(response);
                RatesDetails.splice(index, 1);
                AddToRatesCategoriesTable(RatesCategories);
                AddToRatesDetailsTable(RatesDetails);
                alert('Rate Detail Deleted');
            }
            else
                alert("Something went wrong!!!");           
        }).fail(function (error) {
            alert(error.StatusText);
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
//Block special characters for Rate Operator
function blockSpecialChar(event) {
    var e = event.key;
    return (e == '+' || e == '*' || e == '%' || e == '=');
}
//Displays Rate Targets for selected Rate Detail. val = CategoryId
function DisplayTargets(val) {
    var index = GetRateDetailsIndex(val);
    for (var i = 0; i < RatesDetails[index].ratesDetails[0].rateTargets.length; i++) {
        $('#' + RatesDetails[index].ratesDetails[0].rateTargets[i].target + 'Op').val(RatesDetails[index].ratesDetails[0].rateTargets[i].op);
        $('#' + RatesDetails[index].ratesDetails[0].rateTargets[i].target + 'Figure').val(RatesDetails[index].ratesDetails[0].rateTargets[i].figure);
        $('#' + RatesDetails[index].ratesDetails[0].rateTargets[i].target + 'Checkbox').prop('checked', true);
    }
}
// Empties defaults in case Category is Removed or Category is Selected
function EmptiesRateTargets() {
    $('#Defaults :input, :checkbox').each(function () {
        if (this.type == 'checkbox') {
            $('#' + this.attributes.id.nodeValue).prop('checked', false);
        }
        else if (this.type != 'button' && this.type != 'reset') {
            $('#' + this.attributes.id.nodeValue).val('');
        }
    });
}
//If Defaults Checkbox is clicked
function RateTargetCheckBox(val) {
    if ($('#' + val + 'Checkbox').is(':checked')) {
        if (SelectedRateDetail != null) {
              CreateRateTargets(val);
        }
        else {
            alert('Please Select Rate Detail!!!');
            $('#' + val + 'Checkbox').prop('checked', false);
        }
    }
    else {
        RemoveRateTargets(val);
    }
}
//Creates Rate Target for selected Rate Detail. val = Rate Target
function CreateRateTargets(val) {
    var index = GetRateDetailsIndex(SelectedRateDetail);
    var rateTarget={
        RateDetailId: RatesDetails[index].ratesDetails[0].id,
        RateTarget: val,
        RateFigure: $('#' + val + 'Figure').val(),
        RateOperator: $('#' + val + 'Op').val()
    };
    AjaxCall('CreateRateTarget', JSON.stringify(rateTarget), 'POST').done(function (response) {
        if (response != null) {
            RatesDetails.splice(index, 1);
            RatesDetails.push(response);
            AddToRatesDetailsTable(RatesDetails);
            alert('Rate Target Created!!!');
        }
        else
            alert("Something went wrong!!!");
    }).fail(function (error) {
        alert(error.StatusText);
    });
}
//Deletes Rate Target for selected Rate Detail. val = Rate Target
function DeleteRateTargets(val) {
    var index = GetRateDetailsIndex(SelectedRateDetail);
    var id = RatesDetails[index].ratesDetails[0].rateTargets[i].id;
    AjaxCall('CreateRateTarget', JSON.stringify(id), 'POST').done(function (response) {
        if (response != null) {
            RatesDetails.splice(index, 1);
            RatesDetails.push(response);
            AddToRatesDetailsTable(RatesDetails);
            alert('Rate Target Deleted!!!');
        }
        else
            alert("Something went wrong!!!");
    }).fail(function (error) {
        alert(error.StatusText);
    });
}
function SimulateTrip() {
    if (!$('#ServiceSelect').val() == '') {
        if ($('#DateTimePickUp').val() != '' && $('#Pax').val() != '' && $('#Kms').val() != '' && $('#Drive').val() != '' && $('#Wait').val() != '') {
            var key = GetApiKey($('#ServiceSelect').val());
            var trip = {
                "ServiceID": $('#ServiceSelect').val(),
                "ServiceKey": key,
                "DateTimePickupTh": $('#DateTimePickUp').val(),
                "Categories": [],
                "Passengers": parseInt($('#Pax').val()),
                "Kms": parseInt($('#Kms').val()),
                "DriveTime": parseInt($('#Drive').val()),
                "WaitTime": parseInt($('#Wait').val()),
            };
            //Rest Api Request...
            AjaxCall('https://198.38.85.103:44344/api/quotes/calculate', JSON.stringify(trip), 'POST').done(function (response) {
                if (response != null)
                    $('#Total').val(response.price);
            }).fail(function (error) {
                alert(error.StatusText);
            });
        }
        else
            alert("Please fill all the data!");
    }
    else
        alert("Please select service!");
}
//Gets Api Key for Selected Service. val = ServiceId
function GetApiKey(val) {
    var key;
    AjaxCall('GetApiKey', JSON.stringify(val), 'POST', false).done(function (response) {
        key = response;
    }).fail(function (error) {
        alert(error.StatusText);
        return null;
    });
    return key;
}
//Gets Service Langs and fills #LanguageSelect for selected Service. val = ServiceId.
function GetServiceLangs(val) {
    AjaxCall('GetServiceLangs', JSON.stringify(val), 'POST').done(function (response) {
        if (response.length > 0) {
            var options = '';
            for (var i = 0; i < response.length; i++) {
                options += '<option value="' + response[i].id + '">' + response[i].lang + '</option>';
            }
            $('#LanguageSelect').append(options);
        }
    }).fail(function (error) {
        alert(error.StatusText);
    });
}