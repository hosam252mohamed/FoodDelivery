$("#buttonSub").on("click", function (e) {
    if ($("#textArea").val().trim().length == 0) {
        e.preventDefault();
        var spanEl = $(".textarea-cont .error")[0];
        if (spanEl) {
            $(".textarea-cont")[0].removeChild(spanEl);
        }
        var spanEl = document.createElement("span");
        spanEl.className = "error";
        spanEl.innerHTML = "This field cannot be empty"
        $(".textarea-cont").append(spanEl);
    }
});

$(".comment").on("click", function (e) {
    var revId = this.querySelector("#rev-id").value;
    if (e.target.classList.contains("deleteBtn")) {
        var url = "/Main/Home/Delete/" + revId;
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
                    type: 'Delete',
                    success: function (data) {
                        toastr.success(data.message);
                        location.reload();
                    },
                    error: function (data) {
                        toastr.error(data.message);
                    }
                })
            }
        })
    }
});

$(".comment").each(function () {
    var reviewText = this.querySelector("#review-text");
    var reviewValue = reviewText.innerHTML; // Store the original value outside the click handler

    $(this).on("click", function (e) {
        if (e.target.classList.contains("edit-btn")) {
            this.querySelector(".dropdown").style.display = "none";
            var revId = this.querySelector("#rev-id").value;
            reviewText.innerHTML = `
                <form action='/Main/Home/Edit' method='post'>
                    <input hidden type='text' name='id' value='${revId}'/>
                    <div class="d-flex flex-start w-100">
                        <div data-mdb-input-init class="form-outline w-100 textarea-cont">
                            <textarea name="Text" class="form-control" id="textArea" rows="3"
                                      style="background: #fff;">${reviewValue}</textarea>
                        </div>
                    </div>
                    <div class="d-flex justify-content-end gap-1 pt-1 mt-2">
                        <button type="submit" class="btn btn-primary btn-sm" id="buttonSub">Edit Review</button>
                        <button type="button" class="btn btn-primary btn-sm" id="btn-cancel">Cancel</button>
                    </div>
                </form>
            `;
        }
        if (e.target.getAttribute("id") == "btn-cancel") {
            reviewText.innerHTML = reviewValue; // Revert to original value
            this.querySelector(".dropdown").style.display = "initial";
        }
    });
});
