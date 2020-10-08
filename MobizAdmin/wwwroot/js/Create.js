var RateCategories = [];
var RateCategoriesRemoved = [];
var ButtonColor;
$(function () {
    $('#ServiceSelect').on("change", function () {
        if ($('#ServiceSelect').val() != "null") {

            var serviceid = $('#ServiceSelect').val();
            var serviceName = $('#ServiceSelect').find(":selected").text();
            $('#ServiceId').text(serviceid);
            $('#ServiceName').text(serviceName);
            $('#ServiceRateSelect')
                .empty()
                .append('<option value="null">-Select Service Rate-</option>')
                ;
            $('#RatesDetailsTable')
                .empty();
            $('#RatesDetailsTableRemoved')
                .empty();
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
            //Get Rate Categories
            $('#RatesDetailsSection').removeAttr("hidden");
            AjaxCall('GetRateCategories', JSON.stringify(serviceid), 'POST').done(function (response) {
                if (response.length > 0) {
                    RateCategories = response;
                    AddToTable(RateCategories);
                }
            }).fail(function (error) {
                alert(error.StatusText);
            });
        }
    });
    $('#ServiceRateSelect').on("change", function () {
        //Get Service Rates
        if ($('#ServiceRateSelect').val() != "null") {
            var vernum = $('#ServiceRateSelect').val();
            AjaxCall('GetServiceRate', JSON.stringify(vernum), 'POST').done(function (response) {
                if (response.length > 0) {
                    $('#defDate').text(response[0].defDate);
                    $('#appDate').text(response[0].appDate);
                    $('#endDate').text(response[0].endDate);
                    $('#verNum').text(vernum);
                }
            }).fail(function (error) {
                alert(error.StatusText);
            });
        }
    });
});

function AjaxCall(url, data, type) {
    return $.ajax({
        url: url,
        type: type ? type : 'GET',
        data: data,
        contentType: 'application/json'
    });
}
function AddToTable(data) {
    $('#RatesDetailsTable').html('');
    var rows = "<tr> <th></th><th>Grouping</th><th>Categorie</th><th></th></tr>";
    for (var i = 0; i < data.length; i++) {
        rows += "<tr>"
        rows += "<td><button type='button' onclick='RemoveRateCategories(this.value)' value='" + data[i].id + "'>-</button></td>";
        rows += "<td>" + data[i].lexo + "</td>";
        rows += "<td>" + data[i].grouping + "</td>";
        rows += "<td><button type='button' onclick='RatesDetails(this.value)' value='" + data[i].id + "' id='edit" + data[i].id + "'>=></button></td>";
        rows += "</tr>"
    }
    $('#RatesDetailsTable').append(rows);
}
function AddtoRemovedTable(data) {
    $('#RatesDetailsTableRemoved').html('');
    var rows = "";
    for (var i = 0; i < data.length; i++) {
        rows += "<tr>"
        rows += "<td><button type='button' onclick='AddToRateCategories(this.value)' value='" + data[i].id + "'>+</button></td>";
        rows += "<td>" + data[i].lexo + "</td>";
        rows += "<td>" + data[i].grouping + "</td>";
        rows += "</tr>"
    }
    $('#RatesDetailsTableRemoved').append(rows);
}
function AddToRateCategories(val) {
    for (var i = 0; i < RateCategoriesRemoved.length; i++) {
        if (RateCategoriesRemoved[i].id == val) {
            RateCategories.push(RateCategoriesRemoved[i]);
            RateCategoriesRemoved.splice(i, 1);
            $('#RatesDetailsTable')
                .empty();
            $('#RatesDetailsTableRemoved')
                .empty();
            AddToTable(RateCategories);
            AddtoRemovedTable(RateCategoriesRemoved);
            break;
        }
    }
}
function RemoveRateCategories(val) {
    for (var i = 0; i < RateCategories.length; i++) {
        if (RateCategories[i].id == val) {
            RateCategoriesRemoved.push(RateCategories[i]);
            RateCategories.splice(i, 1);
            $('#RatesDetailsTable')
                .empty();
            $('#RatesDetailsTableRemoved')
                .empty();
            AddToTable(RateCategories);
            AddtoRemovedTable(RateCategoriesRemoved);
            break;
        }
    }
}
function RatesDetails(val) {
    if (ButtonColor != null) {
        $(ButtonColor).css('background-color', 'white');
    } 
    $('#edit' + val).css('background-color', 'red');
    ButtonColor = '#edit' + val;
    $('#Conditions').text(RateCategories.find(e => e.id == val).conditions);
}