// $(document).ready(function () {
//     var table = $('#dataTableData').DataTable({
//         // buttons: ['copy', 'csv', 'excel', 'pdf', 'print']
//         searching: false,
//     });

//     table.buttons().container().appendTo('#dataTableData_wrapper .col-md-6:eq(0)');
// });


$(document).ready(function () {
    var table = $('#dataTableData').DataTable({
        // buttons: ['copy', 'csv', 'excel', 'pdf', 'print']
        searching: false,
        responsive: true,
        rowReorder: {
            selector: 'td:nth-child(2)'
        }
    });

    table.buttons().container().appendTo('#dataTableData_wrapper .col-md-6:eq(0)');
});