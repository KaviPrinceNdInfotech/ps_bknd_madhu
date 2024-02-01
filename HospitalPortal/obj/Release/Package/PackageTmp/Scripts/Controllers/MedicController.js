/// <reference path="../Library/angular.js" />
var app = angular.module('myApp', []);
app.controller('medicCtrl', function ($scope, $http, medicFac) {

    displayAddUpdate(false);
    $scope.action = "Add";
    //   $scope.medicines = [];
    $scope.term = '';
    $scope.page = 1;
    getMedicineList($scope.term, $scope.page);


    //import

    $scope.uploadFile = function () {
        var files = $("#importFile").get(0).files;
        var formData = new FormData();
        formData.append('importFile', files[0]);

        // Make an AJAX request using $http service
        $http({
            method: 'POST',
            url: '/MedicMaster/ImportFile', // Adjust the URL accordingly
            headers: { 'Content-Type': undefined },
            transformRequest: angular.identity,
            data: formData
        }).then(function (response) {
            // Success callback
            if (response.data.Status === 1) {
                alert(response.data.Message);
            } else {
                alert("Failed to Import");
            }
        }, function (error) {
            // Error callback
            console.error('Error uploading file:', error);
        });
    };
    //
   
        ///


    $scope.displayAddUpdate = function (val) {
        displayAddUpdate(val);
        $scope.action = "Add";
        $scope.medic = {};
    }

    // get medicine list
    $scope.loadMedicineList = function (term, page) {
        getMedicineList(term, page);
    }

    // search medicine

    $scope.searchMedicine = function (t) {
        getMedicineList(t, $scope.page)
    }

    function getMedicineList(term, page) {
        medicFac.getMedicineList(term, page).then(function (r) {
            $scope.medicines = r.medicines;
            $scope.page = r.page;
            $scope.totalPages = [];
            for (var i = 1; i <= r.totalPages; i++) {
                $scope.totalPages.push(i);
            }
        }, function (e) {
            console.log(e.statusText);
        });
    }


    // add medicine
    $scope.addMedicine = function (data) {
        //$scope.medicines.push(data);
        ////$scope.data = {};
        print("Wait...", "alert alert-info")
        if ($scope.action == "Add") {
            medicFac.saveMedicine(data).then(function (r) {
                if (r == "ok") {
                    print("Record has saved", "alert alert-success");
                    // $scope.medicines.push(data);
                    getMedicineList($scope.term, $scope.page);

                    $scope.medic = {};

                }
                else
                    print(r, "alert alert-danger");
            },function (e) {
            print("Server error", "alert alert-danger");
            console.log(e.statusText);
        });
        }
        else {
            medicFac.updateMedicine(data).then(function (r) {
                if (r == "ok") {
                    print("Record has updated", "alert alert-success");
                    // $scope.medicines.push(data);
                    getMedicineList($scope.term, $scope.page);

                    $scope.medic = {};

                }
                else
                    print(r, "alert alert-danger");
            }, function (e) {
                print("Server error", "alert alert-danger");
                console.log(e.statusText);
            });
        }
    }
    




    // delete medicine
    $scope.deleteMedicine = function (id) {
        if (window.confirm("Are you sure to delete?")) {
            medicFac.deleteMedicine(id).then(function (r) {
                getMedicineList($scope.term, $scope.page);
            }, function (e) {
                console.log(e.statusText);
                alert("Server error");
            });
        }
    }

    // edit medicine

    $scope.editMedicine = function (m) {
        $scope.medic = m;
        displayAddUpdate(true);
        $scope.action = "Update";
        //angular.forEach($scope.medicineTypes, function (value, key) {debugger
        //    if ($scope.medic.MedicineType_Id == value.Id) {
        //        $scope.medic.MedicineType_Id = 2;
        //    }
        //});
    }

    // get medicine type
    medicFac.getMedicineType().then(function (r) {
        
        $scope.medicineTypes = r;
    },
    function (e) {
        console.log(e.statusText);
    })

    // common function
    function print(message, alertType) {
        $scope.msg = message;
        $scope.alertType = alertType;
    }

    function displayAddUpdate(val) {
        $scope.showAddUpdate = val;
    }

});

app.factory('medicFac', function ($http, $q) {
    var fac = {};
    var defer = $q.defer();
    // get medicine types
    fac.getMedicineType = function () {

        $http.get('/MedicMaster/GetMedicineType').success(function (response) {
            defer.resolve(response);
        })
            .error(function (err) {
                defer.reject(err)
            });
        return defer.promise;
    }

    // add medicine type

    fac.saveMedicine = function (data) {
        var defer = $q.defer();
        $http({
            url: '/MedicMaster/AddMedicine',
            method: 'post',
            data: data
        }).success(function (r) {
            defer.resolve(r);
        })
        .error(function (e) {
            defer.reject(e);
        });
        return defer.promise;
    }

    fac.getMedicineList = function (term, page) {
        var defer = $q.defer();
        term = term == undefined ? '' : term;
        $http.get('/MedicMaster/GetAllMedicines?term=' + term + "&page=" + page).success(function (r) {
            defer.resolve(r);
        }).error(function (e) {
            defer.reject(e);
        });
        return defer.promise;
    }

    fac.deleteMedicine = function (id) {
        var defer = $q.defer();
        $http.get('/MedicMaster/DeleteMedicine?id=' + id).success(function (r) {
            defer.resolve(r);
        }).error(function (e) {
            defer.reject(e);
        });
        return defer.promise;
    }

    fac.updateMedicine = function (data) {
        var defer = $q.defer();
        $http.post('/MedicMaster/UpdateMedicine', data).success(function (r) {
            defer.resolve(r);
        },
        function (e) {
            defer.reject(e);
        }
        )
        return defer.promise;
    }

    return fac;
});