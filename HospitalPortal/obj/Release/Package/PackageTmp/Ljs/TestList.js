//<link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
//<script src="//code.jquery.com/jquery-1.10.2.js"></script>
//<script src="//code.jquery.com/ui/1.11.4/jquery-ui.js"></script>  

$(document).on('keydown.autocomplete focus.autocomplete', '.txtPd', function () {
    $(this).autocomplete({
        source: function (request, response) {
            debugger
            $.ajax({
                url: '/Rent/RoomType',
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
                                label: item.RoomName,
                                value: item.RoomName,
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