/// <reference path="Library/jquery-1.7.1.min.js" />


$("btnAdd").click(function () {
    var tr = `<tr>
        <td> <select id="role" name='role'>
        <option value="Doctor">Doctor</option>
        <option value="Hospital">Hospital</option>
        </select>
        </td>
        <td> <input type='Text' class ="form-control" id="Commission" name='Commission'> </td>
        <td>
        <input type='button' class ='btn btn-primary' onclick='saveCommssion(this)' value='Save'>
          <input type='button' class ='btn btn-danger' onclick='cancel(this)' value='Cancel'>
          </td>
        </tr>`
    let table = $("#CommsionTable");
    table.prepend(tr);
});


//Cancel The Row
function cancel(obj) {
    let btn = $(obj);
    btn.parent().parent().remove();
}

//Save the Commission
function saveCommission() {
    let CurrentBtn = $(obj);
    let parentrow


}

