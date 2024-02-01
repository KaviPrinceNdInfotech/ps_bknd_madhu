/// <reference path="E:\Projects\hospitalproject\HospitalPortal\Scripts/Library/jquery-1.7.1.min.js" />
/// <reference path="E:\Projects\hospitalproject\HospitalPortal\Scripts/Library/jquery-ui-1.8.20.min.js" />


$('#txtVdr').autocomplete({
    source: function (request, response) {
        debugger
        $.ajax({
            url: '/Common/GetAllFranchise',
            data: { term: request.term },
            type: 'POST',
            success: function (data) {
                if (!data.length) {
                    var result = [
                     {
                         label: 'No data found',
                         value: response.term
                     }
                    ];
                    response(result);
                }
                else {
                    response($.map(data, function (item) {
                        return {
                            label: item.UniqueId + '(' + item.CompanyName + ')',
                            //label: item.CompanyName + '(' + item.UniqueId + ')',
                            value: item.CompanyName,
                            id: item.Id,

                        };
                    }));
                }
            }
        });
    },
    select: function (event, ui) {
        $('#Vendor_Id').val(ui.item.id);
        $('#txtVdr').val(ui.item.value);
        return false;
    }
});
