var RateCategoriesSelected = [];
var RateCategoriesdNotSelected = [];
var SelectedCategory;
var ServiceRate;
var TargetsList = [];
var RateDetailsList = [];
var DefaultButtonColor;
//on Page load
$(function () {
    //If a service is selected
    $('#ServiceSelect').on("change", function () {
        if ($('#ServiceSelect').val() != "null") {
            var serviceid = $('#ServiceSelect').val();
            var serviceName = $('#ServiceSelect').find(":selected").text();
            $('#ServiceId').val(serviceid);
            $('#ServiceName').val(serviceName);
            $('#ServiceRateSelect')
                .empty()
                .append('<option value="null">-Select Service Rate-</option>')
                ;
            $('#RatesCategoriesSelected')
                .empty();
            $('#RatesCategoriesNotSelected')
                .empty();
            //Get Service Rates for selected Service
            AjaxCall('GetServiceRates', JSON.stringify(serviceid), 'POST').done(function (response) {
                if (response.length > 0) {
                    $('#stateDropDownList').html('');
                    var options = '';
                    for (var i = 0; i < response.length; i++) {
                        options += '<option value="' + response[i].verNum + '">' + response[i].lexo + '</option>';
                    }
                    $('#ServiceRateSelect').append(options);

                }
            }).fail(function (error) {
                alert(error.StatusText);
            });
            //Get Rate Categories Selected for the Service
            $('#RatesDetailsSection').removeAttr("hidden");
            AjaxCall('GetRateCategoriesSelected', JSON.stringify(serviceid), 'POST').done(function (response) {
                if (response.length > 0) {
                    RateCategoriesSelected = [];
                    for (var i = 0; i < response.length; i++) {
                        RateCategoriesSelected.push({
                            id: response[i].id,
                            lexo: response[i].lexo,
                            grouping: response[i].grouping,
                            conditions: response[i].conditions
                        });
                    }                   
                    AddToSelectedTable(RateCategoriesSelected);
                }
            }).fail(function (error) {
                alert(error.StatusText);
            });
            //Get Rate Categories Not Used in Rate Details for the Service
            AjaxCall('GetRateCategoriesSelectedNotSelected', JSON.stringify(serviceid), 'POST').done(function (response) {
                if (response.length > 0) {
                    RateCategoriesdNotSelected = [];
                    for (var i = 0; i < response.length; i++) {
                        RateCategoriesdNotSelected.push({
                            id: response[i].id,
                            lexo: response[i].lexo,
                            grouping: response[i].grouping,
                            conditions: response[i].conditions
                        });
                    }
                    AddToNotSelectedTable(RateCategoriesdNotSelected);
                }
            }).fail(function (error) {
                alert(error.StatusText);
            });
        }
    });
    //If Service Rates is Selected
    $('#ServiceRateSelect').on("change", function () {
        if ($('#ServiceRateSelect').val() != "null") {
            var vernum = $('#ServiceRateSelect').val();
            //Get Service Rate
            AjaxCall('GetServiceRate', JSON.stringify(vernum), 'POST').done(function (response) {
                if (response.length > 0) {
                    ServiceRate = {
                        VerNum: response[0].verNum,
                        EurKm: response[0].eurKm,
                        EurMinDrive: response[0].eurMinDrive,
                        EurMinimum: response[0].eurMinimum,
                        EurMinWait: response[0].eurMinWait,
                        DefDate: response[0].defDate,
                        AppDate: response[0].appDate,
                        EndDate: response[0].endDate};
                    $('#defDate').val(ServiceRate.DefDate);
                    $('#appDate').val(ServiceRate.AppDate);
                    $('#endDate').val(ServiceRate.EndDate);
                    $('#verNum').val(ServiceRate.VerNum);
                    AddAllToRateDetails();
                }
            }).fail(function (error) {
                alert(error.StatusText);
            });
        }
    });
});
//Ajax Call Function
function AjaxCall(url, data, type) {
    return $.ajax({
        url: url,
        type: type ? type : 'GET',
        data: data,
        contentType: 'application/json'
    });
}
//Adds data to Selected Rate Categories Table. 
function AddToSelectedTable(data) {
    $('#nOfCategories').val(data.length);
    $('#RatesCategoriesSelected').html('');
    var rows = "<tr> <th></th><th>Categorie</th><th>Grouping</th><th></th></tr>";
    for (var i = 0; i < data.length; i++) {
        rows += "<tr>"
        rows += "<td><button type='button' class='tableBtn' onclick='RemoveRateCategories(this.value)' value='" + data[i].id + "'>⇓</button></td>";
        rows += "<td>" + data[i].lexo + "</td>";
        rows += "<td>" + data[i].grouping + "</td>";
        rows += "<td><button type='button' onclick='RatesDetails(this.value)' value='" + data[i].id + "' id='selected" + data[i].id + "'>⇒</button></td>";
        rows += "</tr>"
    }
    $('#RatesCategoriesSelected').append(rows);
    if (ServiceRate!=null)
    HasRateTargetsSelectedButton();
    if (SelectedCategory != null) {
        $('#selected'+SelectedCategory).css('background-color', 'red');
    }
}
//Adds data to Not Selected Rate Categories Table
function AddToNotSelectedTable(data) {
    $('#RatesCategoriesNotSelected').html('');
    var rows = "<tr> <th></th><th>Categorie</th><th>Grouping</th><th></th></tr>";
    for (var i = 0; i < data.length; i++) {
        rows += "<tr>"
        rows += "<td><button class='tableBtn' type='button' onclick='AddToRateCategories(this.value)' value='" + data[i].id + "'>⇑</button></td>";
        rows += "<td>" + data[i].lexo + "</td>";
        rows += "<td>" + data[i].grouping + "</td>";
        rows += "</tr>"

    }
    $('#RatesCategoriesNotSelected').append(rows);
}
//onClick Adds Not Selected Rate Categorie to Selected Array and Table
function AddToRateCategories(val) {
    for (var i = 0; i < RateCategoriesdNotSelected.length; i++) {
        if (RateCategoriesdNotSelected[i].id == val) {
            RateCategoriesSelected.push({
                id: RateCategoriesdNotSelected[i].id,
                lexo: RateCategoriesdNotSelected[i].lexo,
                grouping: RateCategoriesdNotSelected[i].grouping,
                conditions: RateCategoriesdNotSelected[i].conditions
            });
            if (ServiceRate!=null)
            AddToRateDetails(RateCategoriesdNotSelected[i].id, RateCategoriesdNotSelected[i].conditions);
            RateCategoriesdNotSelected.splice(i, 1);
            $('#RatesCategoriesSelected')
                .empty();
            $('#RatesCategoriesNotSelected')
                .empty();
            AddToSelectedTable(RateCategoriesSelected);
            AddToNotSelectedTable(RateCategoriesdNotSelected);
            break;
        }
    }
}
//onClick Removes from Selected Rate Categorie Array and Table. Adds it to Not Selected Array and Table
function RemoveRateCategories(val) {
    if (SelectedCategory == val) {
        SelectedCategory = null;
        $('#Conditions').val('');
        $('#CatId').val('');
        $('#RdId').val('');
        EmptiesDefaults();
    }
    for (var i = 0; i < RateCategoriesSelected.length; i++) {
        if (RateCategoriesSelected[i].id == val) {
            RateCategoriesdNotSelected.push({
                id: RateCategoriesSelected[i].id,
                lexo: RateCategoriesSelected[i].lexo,
                grouping: RateCategoriesSelected[i].grouping,
                conditions: RateCategoriesSelected[i].conditions
            });
            if (ServiceRate!=null)
            RemoveFromRateDetails(RateCategoriesSelected[i].id);
            RateCategoriesSelected.splice(i, 1);
            $('#RatesCategoriesSelected')
                .empty();
            $('#RatesCategoriesNotSelected')
                .empty();
            AddToSelectedTable(RateCategoriesSelected);
            AddToNotSelectedTable(RateCategoriesdNotSelected);
            break;
        }
    }
}
//onClick Display Rate Conditions for Selected Rate Categorie
function RatesDetails(val) {
    if (ServiceRate != null) {
        if (SelectedCategory != null) {
            $('#selected' + SelectedCategory).css('background-color', DefaultButtonColor);
        }
        SelectedCategory = val;
        HasRateTargetsSelectedButton();
        if (DefaultButtonColor==null)
        DefaultButtonColor = $('#selected' + SelectedCategory).css('background-color');
        $('#selected' + SelectedCategory).css('background-color', 'red');
        $('#Conditions').val(RateCategoriesSelected.find(e => e.id == val).conditions);
        $('#CatId').val(val);
        if (ServiceRate != null) {
            var id = GetRatesDetailsIdAndIndex(val).id;
            $('#RdId').val(id);
            EmptiesDefaults();
            DisplayTargets(id);
        }
    }
    else
        alert('Please Select Service Rate');
}
//Gets Service Details Id and Index for specific Category
function GetRatesDetailsIdAndIndex(val) {
    for (var i = 0; i < RateDetailsList.length; i++) {
        if (RateDetailsList[i].categoryid == val) {
            return {
                id: RateDetailsList[i].id,
                index: i
            };
            break;
        }
    }
}
//Adds Categories to Rate Details Array
function AddAllToRateDetails() {
    RateDetailsList = [];
    TargetsList = [];
    for (var i = 0; i < RateCategoriesSelected.length; i++) {
        RateDetailsList.push({
            id: i+1,
            vernum: ServiceRate.VerNum,
            categoryid: RateCategoriesSelected[i].id,
            detailconditions: RateCategoriesSelected[i].conditions
        });
    }
    $('#nOfRateDetails').val(RateDetailsList.length);
}
//Creates new Rates Detail for Created Category
function AddToRateDetails(categoryid, detailconditions) {
    var id;
    if (RateDetailsList.length == 0) {
        id = 1;
    }
    else {
        id = parseInt(RateDetailsList[RateDetailsList.length - 1].id) + 1;
    }
    RateDetailsList.push({
        id: id,
        vernum: ServiceRate.VerNum,
        categoryid: categoryid,
        detailconditions: detailconditions
    });
    $('#nOfRateDetails').val(RateDetailsList.length);
}
//Removes Categories From Rate Details Array
function RemoveFromRateDetails(val) {
    var rd = GetRatesDetailsIdAndIndex(val);
    RemoveFromRateTargets(rd.id);
    RateDetailsList.splice(rd.index, 1);
    $('#nOfRateDetails').val(RateDetailsList.length);
}
//Adds Defaults to Rates Targets Array
function AddToRateTargets(val) {
    TargetsList.push({
        RateDetailId: $('#RdId').val(),
        RateTarget: val,
        RateFigure: $('#' + val + 'Figure').val(),
        RateOperator: $('#' + val + 'Op').val()
    });
    $('#nOfRateTargets').val(TargetsList.length);
}
//Removes Defaults From Rates Targets Array
function RemoveDefaultsFromRateTargets(val) {
    for (var i = 0; i < TargetsList.length; i++) {
        if (TargetsList[i].RateTarget == val) {
            TargetsList.splice(i, 1);
        }
    }
    $('#nOfRateTargets').val(TargetsList.length);
}
// Removes Rate Details From Rates Targets Array
function RemoveFromRateTargets(val) {
    for (var i = 0; i < TargetsList.length; i++) {
        if (TargetsList[i].RateDetailId == val) {
            TargetsList.splice(i, 1);
            i -= 1;
        }
    }
    $('#nOfRateTargets').val(TargetsList.length);
}
//If Defaults Checkbox is clicked
function DefaultsCheckBox(val) {
    if ($('#' + val + 'Checkbox').is(':checked')) {
        if (SelectedCategory != null) {
            $('#Create').validate({
                rules: {
                    Op: "required",
                    Figure: "required"
                },
                messages: {
                    Op: 'Please enter operator',
                    Figure: 'Please enter figure'
                }
            });           
            var valid = true;
            $('#' + val + ' :input').each(function () {
                if (!($('#Create').validate().element(this)))
                    valid = false;
            });
            if (valid == true) {
                AddToRateTargets(val);
            }
            else {
                $('#' + val + 'Checkbox').prop('checked', false);
            }
        }
        else {
            alert('Please Select Category!!!');
            $('#' + val + 'Checkbox').prop('checked', false);
        }
    }
    else {
        RemoveDefaultsFromRateTargets(val);
    }
}
//onClick Simulates trip, calculate price for inputed data
function SimulateTrip() {
    if (ServiceAndServiceRateSelected() == true) {
        $('#Create').validate();
        $("#Kms").rules("add", {
            required: true,
            messages: {
                required: "Required input",
            }
        });
        $("#Drive").rules("add", {
            required: true,
            messages: {
                required: "Required input",
            }
        });
        $("#Wait").rules("add", {
            required: true,
            messages: {
                required: "Required input",
            }
        });
        var valid = true;
        $('#Variables :input').each(function () {
            if (!($('#Create').validate().element(this)))
                valid = false;       
        });
        if (valid==true) {
            var newRate = {
                EurKm: null,
                EurMinDrive: null,
                EurMinimum: null,
                EurMinWait: null
            };
            var price;
            for (var i = 0; i < TargetsList.length; i++) {
                var previousPrice = 0;
                if (newRate[TargetsList[i].RateTarget] != null)
                    previousPrice = newRate[TargetsList[i].RateTarget];
                switch (TargetsList[i].RateOperator) {
                    case "+":
                        newRate[TargetsList[i].RateTarget] = previousPrice + (ServiceRate[TargetsList[i].RateTarget] + TargetsList[i].RateFigure);
                        break;
                    case "*":
                        newRate[TargetsList[i].RateTarget] = previousPrice + (ServiceRate[TargetsList[i].RateTarget] * TargetsList[i].RateFigure);
                        break;
                    case "=":
                        newRate[TargetsList[i].RateTarget] = previousPrice + (ServiceRate[TargetsList[i].RateTarget] = TargetsList[i].RateFigure);
                        break;
                    case "%":
                        newRate[TargetsList[i].RateTarget] = previousPrice + (ServiceRate[TargetsList[i].RateTarget] + ((TargetsList[i].RateFigure / 100) * ServiceRate[TargetsList[i].RateTarget]));
                        break;
                }
            }
            price = (($('#Kms').val() * newRate["EurKm"]) + ($('#Drive').val() * newRate["EurMinDrive"]) + ($('#Wait').val() * newRate["EurMinWait"]));
            if (price < ServiceRate.EurMinimum)
                price = ServiceRate.EurMinimum;
            $('#Total').val(price);
        }
    }
}
// Checks if Service and Service Rate are Selected
function ServiceAndServiceRateSelected() {
    if ($('#ServiceSelect').val() == "null") {
        alert('Please select Service');
        return false;
    }
    else if ($('#ServiceRateSelect').val() == "null") {
        alert('Please select Service Rate');
        return false;
    }
    else {
        return true;
    }
}
// Empties defaults in case Category is Removed or Category is Selected
function EmptiesDefaults() {
    $('#Defaults :input, :checkbox').each(function () {
        if (this.type == 'checkbox') {
            $('#' + this.attributes.id.nodeValue).prop('checked', false);
        }
        else {
            $('#' + this.attributes.id.nodeValue).val('');
        }
    });
}
//Displays Rate Targets for selected Rate Category and Rate Detail
function DisplayTargets(val) {
    for (var i=0; i < TargetsList.length; i++) {
        if (TargetsList[i].RateDetailId == val) {
            $('#' + TargetsList[i].RateTarget + 'Op').val(TargetsList[i].RateOperator);
            $('#' + TargetsList[i].RateTarget + 'Figure').val(TargetsList[i].RateFigure);
            $('#' + TargetsList[i].RateTarget + 'Checkbox').prop('checked', true);
        }
    }
}
//Change color of select button if Rate Categories - Rate Details have Rate Targets
function HasRateTargetsSelectedButton() {
    for (var i = 0; i < RateCategoriesSelected.length; i++) {
        var id = GetRatesDetailsIdAndIndex(RateCategoriesSelected[i].id).id;
        for (var j = 0; j < TargetsList.length; j++) {
            if (TargetsList[j].RateDetailId == id) {
                $('#selected' + RateCategoriesSelected[i].id).css('background-color', 'blue');
            }      
        }
    }
}
//Block special characters for Rate Operator
function blockSpecialChar(event) {
    var e = event.key;
    return (e=='+' || e=='*' || e=='%' || e=='=');
}