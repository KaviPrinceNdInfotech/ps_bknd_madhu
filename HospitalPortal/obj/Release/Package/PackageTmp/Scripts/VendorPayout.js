/// <reference path="Library/jquery-1.7.1.min.js" />

    $("#btnCheck").click(function () {
        debugger
        var term = $("#tp").val();
        if (term == "Doctor") {
            $.ajax({
                url: "/VendorPayout/Doctor?term=" + term,
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
        if (term == "Driver") {
            $.ajax({
                url: "/VendorPayout/Driver?term=" + term,
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

        if (term == "Vehicle") {
            $.ajax({
                url: "/VendorPayout/Vehicle?term=" + term,
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

        if (term == "HealthCheckUp") {
            $.ajax({
                url: "/VendorPayout/HealthCheckUp?term=" + term,
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


        if (term == "Lab") {
            $.ajax({
                url: "/VendorPayout/Lab?term=" + term,
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


        if (term == "Nurse") {
            $.ajax({
                url: "/VendorPayout/Nurse?term=" + term,
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

        if (term == "Chemist") {
            $.ajax({
                url: "/VendorPayout/Chemist?term=" + term,
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
