var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "responsive": true,
        "ajax": { url: '/Admin/Product/GetAll' },
        "columns": [
            { data: 'name', "width": "25%" },
            { data: 'price', "width": "10%" },
            { data: 'category.name', "width": "20%" },
            {
                data: 'productID',
                "render": function (data) {
                    return `<span id="display-images" onclick=DisplayImages('${data}') style="cursor: pointer; text-decoration: underline;" >View</span>`
                },
                width: "15%"
            },
            {
                data: 'productID',
                "render": function (data) {
                    return `<div class="text-center align-baseline">
                     <a href="/admin/product/upsert/${data}" class="btn btn-primary mr-2"> <i class="bi bi-pencil-square"></i> Edit</a>               
                     <a onClick=Delete('/admin/product/delete/${data}') class="btn btn-danger "> <i class="bi bi-trash-fill"></i> Delete</a>
                    </div>`
                },
                "width": "30%"
            }
        ]
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

function DisplayImages(id) {
    $.ajax({
        url: "/Admin/Product/GetImages/"+id,
        success: function (result) {
            $("#show-images").html(result);
        }
    });
};  
document.addEventListener("click", function (e) {
    if (e.target.getAttribute('id') == 'display-images') {
        globalThis.scrollTo(0,0);
    }
    if (e.target.className == "bi bi-x") {
        let parent = e.target.parentElement.parentElement;
        parent.remove();
    }
});
function close() { 
    let parent = document.getElementById("close").parentElement;
    parent.remove();
}