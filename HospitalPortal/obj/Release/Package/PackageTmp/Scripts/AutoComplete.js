$("#PatientName").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/CompletHealthCheckup/GetPatientName',
                data: { term: request.term },
                type: 'GET',
                success: function (data) {
                    if (!data.length) {
                        debugger
                        var result = [
                         {
                             label: 'No Patient found',
                             value: response.term
                         }
                        ];
                        response(result);
                    }
                    else {
                        response($.map(data, function (item) {
                            return {
                                label: item.PatientName,
                                value: item.PatientName,
                                id: item.Id,
                            };
                        }));
                    }
                }
            });
        },
    });



//function clearLocation() {
//    $( '#txtId').val( "0");
//    $( '#txtArea').val("" ).attr("placeholder" , "Enter Location" );
//    $( "#city").css("border" , "1px solid #fff" );
//}
