function Plus(id) {
    $.ajax({
        type: "POST",
        url: '/Customer/Cart/Plus',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                $('#cart-container').load(document.URL + ' #cart');
            }
        }
    });
}

function Minus(id) {
    $.ajax({
        type: "POST",
        url: '/Customer/Cart/Minus',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                $('#cart-container').load(document.URL + ' #cart');
            }
        }
    });
}
function Delete(id) {
    url = '/Customer/Cart/Delete/' + id
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
                    toastr.success(data.message);
                    $('#cart-container').load(document.URL + ' #cart');
                    $('#shopping-link').load(document.URL + ' #shopping-cart')
                },
            })
        }
    })
}