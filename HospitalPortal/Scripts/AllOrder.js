/// <reference path="E:\Rooms\hospitalproject\hospitalproject\HospitalPortal\js/jquery-1.11.1.min.js" />

function quatation(obj) {

    let currentElem = $(obj);
    let oid = currentElem.attr("Order_Id");
    let uid = currentElem.attr("User_Id");
    let pid = currentElem.attr("Product_Id");


    $(function GetQuotation() {
        debugger;
        var detail = {};
        $.ajax({
            type: "GET",
            url: '/Admin/GetAdminQuotation?orderId=' + oid,
            data: JSON.stringify(detail),
            contentType: "application/json; charset=utf-8",
            //data: { "Id": Id },
            //data: { "ProdOrder_Id": ProdOrder_Id },
            //data: { "With_Gst": With_Gst },
            //data: { "Without_Gst": Without_Gst },
            //data: { "Description": Description },
            //data: { "ModeOfPayment": ModeOfPayment },
            datatype: "json",
            success: function (response) {
                //var items = "";

                //$.each(response, function (i, val) {
                //    items += "<option value='" + val.ModeOfPayment + "'>" + val.ModeOfPayment + "</option>";
                //});

                // var array = eval(response.d);
                //$("#stateID").append("<option value='" + response.ModeOfPayment + "'>" + response.ModeOfPayment + "</option>");
                $('[id*=withgst]').val(response.With_Gst);
                $('[id*=withoutgst]').val(response.Without_Gst);
                $('[id*=description]').val(response.Description);
                $('[id*=mofp]').val(response.ModeOfPayment);

            },
            failure: function (response) {
                alert(response.d);
            },
            error: function (response) {
                alert(response.d);
            }
        });
    });

    $("#closbtn").click(function () {
        $('#myModal').modal('hide');
    });

}