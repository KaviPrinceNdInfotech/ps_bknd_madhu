/// <reference path="Library/jquery-1.7.1.min.js" />


//Event for adding new skill

$("#btnAdd").click(function () {
    var tr = `<tr>
          <td> <input type='text' class ='form-control' id="SkillName" name='SkillName'> </td>
          <td>
          <input type='button' class ='btn btn-primary' onclick='saveSkill(this)' value='Save'>
          <input type='button' class ='btn btn-danger' onclick='cancel(this)' value='Cancel'>
          </td>
        </tr>
        `;
    let table = $("#skillTable");
    table.prepend(tr);
});


// cancel the add row
function cancel(obj) {
    let btn = $(obj);
    btn.parent().parent().remove();
}

// Saving the skill in DB

function saveSkill(obj)
{
    let currentBtn = $(obj);
    let parentRow=currentBtn.parent().parent();
    let skill = currentBtn.parent().parent().find("#SkillName").val();
    let docId = $("#docId").val();
    let data = JSON.stringify({ Doctor_Id: docId, SkillName: skill });
    let table = $("#skillTable tbody");

    $.ajax({
        url: '/DoctorRegistration/AddSkill',
        type: 'post',
        contentType: 'application/json;charset=utf-8',
        data: data,
        success: function (r) {
            if(r!="error")
            {
                //removig the text box containing row
                currentBtn.parent().parent().remove();
                //add new row with data
                let newTr = ` <tr>
                      <td>${r.SkillName}</td>
                      <td>
                          <button url="/DoctorRegistration/RemoveSkill?id=${r.Id}" type="button" class="btn btn-lg btn-danger btnRemove" >
                              <i class="fa fa-trash-o"></i>
                          </button>
                      </td>
                  </tr>`;
                table.prepend(newTr);
            }
            else
            {
                alert("Error");
            }
        },
        error: function (err) {
            console.log(err.responseText);
            alert("Error");
        }
    });
}

// event for removing each element from the list
$(document).on("click", ".btnRemove", function () {
    if (!window.confirm("are you sure to delete ????"))
        return;

    var btn = $(this);
    var url = btn.attr("url");
    $.get(url).then(function (res) {
        if (res == "ok")
            btn.parent().parent().remove();
        else
            console.log(res);
    }).error(function (err) {
        console.log(err.responseText);
    });
});