/// <reference path="D:\ravindra\AdPont\AdventurePoint\AdventurePoint\Scripts/jquery-1.10.2.js" />
/// <reference path="D:\ravindra\AdPont\AdventurePoint\AdventurePoint\Scripts/jquery-1.10.2.intellisense.js" />



$("body").on("click", "#tblRoomType .Delete", function () {
    debugger
    if (confirm("Do you want to delete this row?")) {
        var row = $(this).closest("tr");
        var ListId = row.find("span").html();
        $.ajax({
            type: "POST",
            url: "/MasterEntry/DeleteRoomType",
            data: '{Id: ' + ListId + '}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if ($("#tblRoomType tr").length > 2) {
                    row.remove();
                } else {
                    row.find(".Delete").hide();
                    row.find("span").html('&nbsp;');
                }
            }
        });
    }
});




