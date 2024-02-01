/// <reference path="Library/jquery-1.7.1.min.js" />

$("#btnCheck").click(function () {
    debugger
    var term = $("#tp").val();
    if (term == "Daily") {
        $.ajax({
            url: "/ViewReport/DailyChe?term=" + term,
            type: "get",
            success: function (response) {
                console.log(response);
                $("#prevAtt").html(response);
            },
            error: function (error) {
                console.log(error.responseText);
                msg.text("");
            }
        });
    }
    if (term == "Monthly") {
        $.ajax({
            url: "/ViewReport/MonthlyChe?term=" + term,
            type: "get",
            success: function (response) {
                console.log(response);
                $("#prevAtt").html(response);
            },
            error: function (error) {
                console.log(error.responseText);
                msg.text("");
            }
        });
    }

    if (term == "Yearly") {
        $.ajax({
            url: "/ViewReport/yearlyChe?term=" + term,
            type: "get",
            success: function (response) {
                console.log(response);
                $("#prevAtt").html(response);
            },
            error: function (error) {
                console.log(error.responseText);
                msg.text("");
            }
        });
    }

    if (term == "Weekly") {
        $.ajax({
            url: "/ViewReport/WeeklyChe?term=" + term,
            type: "get",
            success: function (response) {
                console.log(response);
                $("#prevAtt").html(response);
            },
            error: function (error) {
                console.log(error.responseText);
                msg.text("");
            }
        });
    }
});


$("#test").click(function () {
    debugger
    var date = $("#OrderDate").val();
    $.ajax({
        url: "/ViewReport/DailyChe?date=" + date + "",
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