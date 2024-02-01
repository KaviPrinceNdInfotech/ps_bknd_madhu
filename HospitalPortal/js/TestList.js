

$(document).on('keydown.autocomplete focus.autocomplete', '.txtPd', function () {

    $(this).autocomplete({
        source: function (request, response) {
            debugger
            $.ajax({
                url: '/CompletHealthCheckup/GetTest',
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
                                label: item.TestName,
                                value: item.TestName,
                                Id: item.Id,
                                //value:item.Id
                            }
                        }));

                    }
                }
               
            });
        },
        select: function (event, ui) {
            debugger
            var el = $(this);
            var txtProductId = el.parent().parent().find("#Id");
            txtProductId.val(ui.item.Id);
            //Id.val(ui.item.Id);
            //Id.val(Id);
            return false;
        }
    });

});