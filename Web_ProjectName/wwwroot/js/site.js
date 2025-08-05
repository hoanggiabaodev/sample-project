$(document).ready(function () {
    initializeComponents();

    console.log('Site.js loaded successfully');
});

function initializeComponents() {
    if (typeof $.fn.datepicker !== 'undefined') {
        $('.datepicker').datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true,
            todayHighlight: true
        });
    }

    if (typeof $.fn.DataTable !== 'undefined') {
        $('.datatable').DataTable({
            responsive: true,
            language: {
                search: "Tìm kiếm:",
                lengthMenu: "Hiển thị _MENU_ bản ghi",
                info: "Hiển thị _START_ đến _END_ trong tổng số _TOTAL_ bản ghi",
                infoEmpty: "Không có bản ghi nào",
                infoFiltered: "(được lọc từ _MAX_ bản ghi)",
                paginate: {
                    first: "Đầu",
                    previous: "Trước",
                    next: "Tiếp",
                    last: "Cuối"
                }
            }
        });
    }

    if (typeof $.fn.maxlength !== 'undefined') {
        $('.maxlength').maxlength();
    }

    if (typeof $.fn.select2 !== 'undefined') {
        $('.select2').select2({
            placeholder: "Chọn một tùy chọn",
            allowClear: true
        });
    }
} 