/// <reference path="../Library/angular.js" />
var app = angular.module('myApp', []);
app.controller('changePasswordController', function ($scope, changePasswordFactory) {
    $scope.changePassword = function (user) {
        printMessage("Wait....", 'grey');
        changePasswordFactory.changePassword(user).then(function (response) {
            if (response.data == "ok") {
                printMessage("Password has been changed.", "green");
                user = null;
            }
            else
                printMessage(response.data, "red");
        });
    }

    function printMessage(msg, color) {
       
        $scope.message = msg;
        $scope.color = color;
    }
});

app.factory('changePasswordFactory', function ($http) {
    var fac = {};
    fac.changePassword = function (user) {
        return $http({
            url: '/Admin/ChangePassword',
            method: 'post',
            data: user,
            dataType: 'json',
            contentType: 'application/json;charset=utf-8'
        });
    }
    return fac;
});