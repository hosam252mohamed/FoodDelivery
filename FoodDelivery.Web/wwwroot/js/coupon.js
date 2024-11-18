var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        responsive: true,
        "ajax": {url: '/Admin/Coupun/GetAll' },
        "columns": [
            { "data": 'name', "width": "15%" },
            { "data": 'type', "width": "10%" },
            { "data": 'discount', "width": "10%" },
            { "data": 'minAmount', "width": "15%" },
            { "data": 'validTo',  width: "20%" },
            {
                data: { coupunID: 'coupunID', isActive: 'isActive' },
                "render": function (data) {
                    if (data.isActive) {
                        return `
                            <div class="text-center">
                                <a onclick=Activate('${data.coupunID}') class="btn btn-danger text-white" style="cursor:pointer">
                                    <i class="bi bi-unlock-fill"></i> UnActive </a>
                                <a href="/admin/coupun/upsert/${data.coupunID}" class="btn btn-primary text-white" style="cursor:pointer">
                                    <i class="bi bi-pencil-square"></i> Edit </a>
                                <a onclick=Delete('/admin/coupun/delete/${data.coupunID}') class="btn btn-danger text-white" style="cursor:pointer">
                                    <i class="bi bi-trash3-fill"></i></i> Delete </a>
                            </div>
                        `
                    }
                    else {
                        return `
                            <div class="text-center">
                                <a onclick=Activate('${data.coupunID}') class="btn btn-success text-white" style="cursor:pointer">
                                        <i class="bi bi-unlock-fill"></i> Active </a>
                                <a href='/admin/coupun/upsert/${data.coupunID}' class="btn btn-primary mx-2" style="cursor:pointer">
                                    <i class="bi bi-pencil-square"></i> Edit </a>
                                <a onclick=Delete('/admin/coupun/delete/${data.coupunID}') class="btn btn-danger text-white" style="cursor:pointer">
                                    <i class="bi bi-trash3-fill"></i></i> Delete </a>
                            </div>
                        `
                    }
                            
                },
                "width": "30%"
            }
        ]
    });
}
function Activate(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/Coupun/Activate',
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
function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!',
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                },
                error: function (data) {
                    toastr.error(data.message);
                }
            })
        }
    })
}