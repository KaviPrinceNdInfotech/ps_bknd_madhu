/// <reference path="Library/jquery-1.7.1.min.js" />
 


//Doctor Commission
$("#test").click(function () {
    var date = $("#StartDate").val();
    var EndDate = $("#EndDate").val();
    if (date == null && EndDate == null) {
        alert("Kindly Select Date");
    }
    $.ajax({
        url: "/Commission/Doctor?term=Doctor&StartDate=" + date + "&EndDate=" + EndDate + "",
        type: "get",
        success: function (response) {
            console.log(response);
            $("#prevAtt").html(response);
        },
        error: function (error) {
            console.log(error.responseText);
        }
    });
});

//Lab Commission
$("#test").click(function () {
    debugger
    var date = $("#StartDate").val();
    var EndDate = $("#EndDate").val();
    if (date == null && EndDate == null) {
        alert("Kindly Select Date");
    }
    $.ajax({
        url: "/Commission/Lab?term=Lab&StartDate=" + date + "&EndDate=" + EndDate + "",
        type: "get",
        success: function (response) {
            console.log(response);
            $("#prevAtt").html(response);
        },
        error: function (error) {
            console.log(error.responseText);

        }
    });
});