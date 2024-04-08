$(document).ready(function () {
    loadCompanyTable();
});

var companyTable;

function loadCompanyTable() {

    companyTable = $('#CompanyData').DataTable({
        ajax: '/Company/getall',
        columns: [
            { data: 'name', "width": "25%" },
            { data: 'streetAddress', "width": "20%" },
            { data: 'city', "width": "15%" },
            { data: 'postalCode', "width": "15%" },
            { data: 'phone', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="btn-group" role="group">
                                <a href="/company/upsert?id=${data}"  class="btn "><i class="bi bi-pencil-square"></i> </a >
                            </div>
                            <div class="btn-group" role="group">
                                <a onclick=DeleteConfirmation('/company/deletepost?id=${data}') class="btn btn-danger"><i class="bi bi-trash"></i> </a >
                            </div>`
                },
                "width": "10%"
            },
        ]
    });

}


function DeleteConfirmation(DeleteUrl) {

    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: DeleteUrl,
                type: 'DELETE',
                success: function (data) {
                    if (data.success == true) {
                        Swal.fire({

                            title: "Deleted!",
                            text: data.message,
                            icon: "success",
                        });

                        companyTable.ajax.reload();
                    }

                    else {
                        Swal.fire({
                            title: "Failed!",
                            text: data.message,
                            icon: "error"
                        });


                    }

                },

            })
        }
    });

}


//let DeleteUrl = "/product/DeletePost?id=".concat(ID);