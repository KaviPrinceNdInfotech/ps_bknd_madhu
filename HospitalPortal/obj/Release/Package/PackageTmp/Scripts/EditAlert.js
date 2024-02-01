
/// <reference path="Library/jquery-1.7.1.min.js" />


$(window).on('load', function () {
    //alert("If Already Updated. Kindly Ignore.")
    var txt;
    var r = confirm("Wait for Approval, If Already Updated. Kindly Ignore.");
    if (r == true) {
        location.href = "/Admin/Logout";
    } else {

    }
});