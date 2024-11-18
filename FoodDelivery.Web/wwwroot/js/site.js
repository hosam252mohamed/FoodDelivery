function Subscribe() {
    let email = document.querySelector("#email-sub").value;
    if (email != "") {
        $.ajax({
            type: "POST",
            url: '/Main/Home/EmailSubscribe',
            data: JSON.stringify(email),
            contentType: "application/json",
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                }
                else {
                    toastr.error(data.message);
                }
            }
        });
    }
}