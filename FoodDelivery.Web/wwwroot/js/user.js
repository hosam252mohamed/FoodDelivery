var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        responsive: true,
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            {
                "data": "city",
                "render": function (data) {
                    if (data == null)
                        return "Unknown"
                    return data
                },
                "width": "15%"
            },
            {
                "data": "emailConfirmed",
                "render": function (data) {
                    if (data)
                        return "Yes"
                    return "No"
                },
                "width": "15%"
            },
            { "data": "role", "width": "15%" },
            {
                data: { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout > today) {
                        return `
    <div class="d-flex gap-2 flex-wrap">
        <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer;">
            <i class="bi bi-lock-fill"></i> Lock
        </a>
        <a href="/admin/user/permission/${data.id}" class="btn btn-danger text-white" style="cursor:pointer;">
            <i class="bi bi-pencil-square"></i> Permission
        </a>
    </div>
                        `
                    }
                    else {
                        return `
    <div class="d-flex gap-2">
        <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer;">
            <i class="bi bi-unlock-fill"></i> UnLock
        </a>
        <a href="/admin/user/permission/${data.id}" class="btn btn-danger text-white" style="cursor:pointer;">
            <i class="bi bi-pencil-square"></i> Permission
        </a>
    </div>
                        `
                    }


                },
                "width": "25%"
            }
        ]
    });
}


function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}