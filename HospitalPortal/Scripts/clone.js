/// <reference path="Library/jquery-1.7.1.min.js" />

function make_clone() {
    var last_row = $('tr[id^="row"]:last');
    var suffix = parseInt(last_row.prop("id").match(/\d+/g), 10);
    var new_row = last_row.clone(true).prop("id", "row" + (suffix + 1));
    new_row.find('input[type^="text"]').each(function () {
        $(this).val('');
        var old_name = $(this).attr("name");
        var new_name = old_name.replace('[' + suffix + ']', '[' + (suffix + 1) + ']');
        $(this).attr('name', new_name);

    });

    new_row.find('input[type^="hidden"]').each(function () {
        $(this).val('');
        var old_name = $(this).attr("name");
        var new_name = old_name.replace('[' + suffix + ']', '[' + (suffix + 1) + ']');
        $(this).attr('name', new_name);

    });
    new_row.find('select').each(function () {

        var old_name = $(this).attr("name");
        var new_name = old_name.replace('[' + suffix + ']', '[' + (suffix + 1) + ']');
        $(this).attr('name', new_name);

    });
    $("#calculator").append(new_row);

}

$(".remove").click(function (e) {
    e.preventDefault();
    debugger
});

function remove(obj)
{
    $(obj).parent().parent().remove();

}