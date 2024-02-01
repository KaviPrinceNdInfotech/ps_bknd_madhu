/// <reference path="jquery-2.1.4.min.js" />

$(document).on("keyup", ".quantity", function () {
    debugger
    var txtQuantity = $(this);
    var quantity = 1;
    if (txtQuantity.val())
        quantity = parseFloat(txtQuantity.val());
    var stock = parseInt(txtQuantity.parent().parent().find("#stock").text());
    if (quantity > stock)
    {
        alert("Only " + stock + " items remains in Store");
        txtQuantity.val(1);
        return;
    }
    var txtFinalPrice = txtQuantity.parent().parent().find("#price");
    var finalPrice = txtFinalPrice.val() ? parseFloat(txtFinalPrice.val()) : 0;
    var total = finalPrice * quantity;
    var txtTotal = txtQuantity.parent().parent().find("#total");
    txtTotal.val(total);
    calculateGrandTotal();
})




$("#AdditionalDiscount").keyup(function () {
    var discount = $(this).val();
    if (!discount)
        discount=0;
    if(parseInt(discount)>10)
    {
        alert("maximum 10% allowed");
        discount = 0;
        $("#AdditionalDiscount").val(discount);
    }
    var gt = 0;
    $(".total").each(function () {
        var t = $(this).val() ? parseInt($(this).val()) : 0;
        gt += t;
    });
    
    var discountInPercent = parseInt(discount);
    var discountValue = gt * discountInPercent / 100;
    var newGt = gt - discountValue;
    var txtGrandTotal = $("#GrandTotal");
    txtGrandTotal.val(newGt.toFixed(0));
});

//calculate additional charge
$("#adCharge").change(function () {
    debugger
    var charge = $(this).val();
    if (!charge)
        charge = 0;
    //var gt = 0;
    //$(".total").each(function () {
    //    var t = $(this).val() ? parseInt($(this).val()) : 0;
    //    gt += t;
    //});

    var newGrandT = parseFloat($("#GrandTotal").val());
    newGrandT += parseFloat(charge);
    var txtGrandTotal = $("#GrandTotal");
    txtGrandTotal.val(newGrandT.toFixed(0));
});


function calculateGrandTotal()
{
    var txtGrandTotal = $("#GrandTotal");
    var gt = 0;
    $(".total").each(function () {
        var t = $(this).val() ? parseInt($(this).val()) : 0;
        gt += t;
    });
    txtGrandTotal.val(gt.toFixed(0));
}