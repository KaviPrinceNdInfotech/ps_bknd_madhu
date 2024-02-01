/// <reference path="../jquery-ui.js" />
/// <reference path="jquery-1.7.1.min.js" />

    $("#VehicleNumber").autocomplete({
        source: function (request, response) {
            debugger
            $.ajax({
                url: '/Vehicle/GetVehicleNumberList',
                data: { term: request.term },
                type: 'POST',
                success: function (data) {
                    if (!data.length) {
                        debugger
                        var result = [
                         {
                             label: 'No Vehicle found',
                             value: response.term
                         }
                        ];
                        response(result);
                    }
                    else {

                        response($.map(data, function (item) {
                            debugger
                            id = item.Id
                            console.log(id);
                            return {
                                label: item.VehicleNumber,
                                value: item.VehicleNumber,
                                id: item.Id,
                            };
                        }));
                    }
                }
            });
        },
    });
