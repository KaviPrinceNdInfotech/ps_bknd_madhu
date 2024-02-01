/// <reference path="D:\ravindra\AdPont\AdventurePoint\AdventurePoint\Scripts/jquery-1.10.2.js" />
/// <reference path="D:\ravindra\AdPont\AdventurePoint\AdventurePoint\Scripts/jquery-1.10.2.intellisense.js" />



$("#addMore").click(function () {
    debugger
    let table = $("#ServiceAmenity");
    let lastRow = table.find("tr").last();
    let firstInput = lastRow.find(":input").first();
    let nameOfFirstInput = firstInput.attr("name");
    let currentIndex = parseInt(nameOfFirstInput.replace(/[^\d.]/g, ''));
    let nextIndex = currentIndex + 1;
    let nextRow = lastRow.clone();
    nextRow.find(":input").each(function () {
        var currentInput = $(this);
        currentInput.val('');
        var name = currentInput.attr("name");
        var newName = name.replace(currentIndex, nextIndex);
        currentInput.attr("name", newName);
    });
    nextRow.find("td").last().html('<span class="btn btn-danger" id="btnDeleteAttr"><i class="fa fa-trash-o"></i></span>');
    table.append(nextRow);
});

// deleting attribue
$("#ServiceAmenity").on("click", "#btnDeleteAttr", function () {
    debugger
    var btn = $(this);
    var currentRow = btn.parent().parent();
    // finding all rows after this row ad increase their index by 1
    currentRow.nextAll().find(":input").each(function () {
        var row = $(this);
        var name = row.attr('name');
        var index = parseInt(name.replace(/[^\d.]/g, ''));
        var nextIndex = index - 1;
        var newName = name.replace(index, nextIndex);
        row.attr('name', newName);
        console.log(row.attr("name"));
    });
    currentRow.remove();
});



