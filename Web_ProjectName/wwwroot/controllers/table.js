const FIRST_OPTION = '<option value="0">-- Chọn --</option>';

let $selectYearElm = $("#select_search_year");
let $selectVarietyElm = $("#select_search_variety");

let _year = 0;
let _variety = "";

$(document).ready(function () {
    $("#select_search_year").select2({
        placeholder: "Chọn năm",
        allowClear: false,
        language: "vi",
    });

    $("#select_search_variety").select2({
        placeholder: "Chọn giống",
        allowClear: false,
        language: "vi",
    });

    $("#select_search_year").on('change', HandleYearFilterChange);

    $("#select_search_variety").on('change', HandleVarietyFilterChange);

    LoadSelectYears();
    LoadSelectVarieties();
    Search();
});

function CalcHeight() {
    const windowHeight = $(window).height();
    const headerHeight = $('#header').height();
    const windowWidth = $(window).width();

    let offset;

    if (windowWidth <= 768) {
        offset = 200;
    } else if (windowWidth <= 1280) {
        offset = 260;
    } else if (windowWidth <= 1500) {
        offset = 280;
    } else {
        offset = 350;
    }

    return windowHeight - headerHeight - offset;
}

function Search() {
    LoadDataTable();
}

function HandleYearFilterChange(event) {
    const $select = $(event.target);
    const value = $select.val();
    const selectedText = $select.find("option:selected").text();

    if (!dataTableLo) return;

    if (!value || value === "0") {
        dataTableLo.column(3).search("").draw();
    } else {
        dataTableLo.column(3).search(`^${selectedText}$`, true, false).draw();
    }
}

function HandleVarietyFilterChange(event) {
    const $select = $(event.target);
    const value = $select.val();
    const selectedText = $select.find("option:selected").text();

    if (!dataTableLo) return;

    if (!value || value === "0") {
        dataTableLo.column(4).search("").draw();
    } else {
        dataTableLo.column(4).search(`^${selectedText}$`, true, false).draw();
    }
}

let dataTableLo;
let $tableMainLo = $("#table_lo");

const columnTableLo = function () {
    return [
        {
            data: null,
            visible: true,
            orderable: false,
            render: (data, type, row, meta) => {
                return meta.row + meta.settings._iDisplayStart + 1;
            },
            className: "text-center"
        },
        {
            data: "plotId",
            className: "text-center fw-500"
        },
        {
            data: "idPrivate",
            className: "text-center fw-500"
        },
        {
            data: "yearOfPlanting",
            className: "text-center fw-500"
        },
        {
            data: "typeOfTreeName",
            className: "text-center fw-500"
        },
        {
            data: "area",
            className: "text-center fw-500 bg-success-subtle",
            render: function (data, type, row) {
                if (data) {
                    const hectares = parseFloat(data) / 10000;
                    return hectares.toFixed(2);
                }
                return "0.00";
            }
        },
        {
            data: "treeQuantityObj.treeQuantity",
            className: "text-center fw-500",
            render: function (data, type, row) {
                return row.treeQuantityObj?.treeQuantity || "0";
            }
        },
        {
            data: "treeQuantityObj.treeQuantityActual",
            className: "text-center fw-500",
            render: function (data, type, row) {
                return row.treeQuantityObj?.treeQuantityActual || "0";
            }
        },
        {
            data: "treeQuantityObj.treeQuantityRepair",
            className: "text-center fw-500",
            render: function (data, type, row) {
                return row.treeQuantityObj?.treeQuantityRepair || "0";
            }
        },
        {
            data: "treeQuantityObj.treeQuantityActualRepair",
            className: "text-center fw-500",
            render: function (data, type, row) {
                return row.treeQuantityObj?.treeQuantityActualRepair || "0";
            }
        },
        {
            data: "treeQuantityObj.treeQuantityExpectedIncrease",
            className: "text-center fw-500",
            render: function (data, type, row) {
                return row.treeQuantityObj?.treeQuantityExpectedIncrease || "0";
            }
        },
        {
            data: "treeQuantityObj.treeQuantityActualIncrease",
            className: "text-center fw-500 bg-warning-subtle",
            render: function (data, type, row) {
                return row.treeQuantityObj?.treeQuantityActualIncrease || "0";
            }
        },
        {
            data: "farmName",
            className: "text-center fw-500",
            render: function (data, type, row) {
                return data || "";
            }
        }
    ];
}

const dataParamsTableLo = function () {
    return {
        type: 'GET',
        url: "/Table/GetList",
        data: function (d) {
            d.year = $selectYearElm.val();
            d.variety = $selectVarietyElm.val();
        },
        dataType: 'json',
        dataSrc: function (response) {
            if (CheckResponseIsSuccess(response) && response.data != null)
                return response.data;
            return [];
        },
        error: function (err) {
            CheckResponseIsSuccess({ result: -1, error: { code: err.status } });
            return [];
        }
    };
}

const dataSelectYears = function () {
    return {
        type: 'GET',
        url: "/Table/GetYears",
        dataType: 'json',
        dataSrc: function (response) {
            if (CheckResponseIsSuccess(response) && response.data != null)
                return response.data;
            return [];
        },
        error: function (err) {
            CheckResponseIsSuccess({ result: -1, error: { code: err.status } });
            return [];
        },
    };
}

const dataSelectVarieties = function () {
    return {
        type: 'GET',
        url: "/Table/GetVarieties",
        dataType: 'json',
        dataSrc: function (response) {
            if (CheckResponseIsSuccess(response) && response.data != null)
                return response.data;
            return [];
        },
        error: function (err) {
            CheckResponseIsSuccess({ result: -1, error: { code: err.status } });
            return [];
        }
    };
}

const MyAppLang = {
    datatable: {
        "sEmptyTable": "Không có dữ liệu",
        "sInfo": "Hiển thị _START_ đến _END_ của _TOTAL_ bản ghi",
        "sInfoEmpty": "Không có dữ liệu để hiển thị",
        "sInfoFiltered": "(lọc từ tổng số _MAX_ bản ghi)",
        "sLengthMenu": "Hiển thị _MENU_ bản ghi",
        "sLoadingRecords": "Đang tải...",
        "sProcessing": "Đang xử lý...",
        "sSearch": "Tìm kiếm:",
        "sZeroRecords": "Không tìm thấy kết quả nào",
        "oPaginate": {
            "sFirst": "Đầu",
            "sLast": "Cuối",
            "sNext": "Sau",
            "sPrevious": "Trước"
        }
    }
};

function LoadDataTable() {
    if (dataTableLo) {
        dataTableLo.ajax.reload(null, false);
        return;
    }

    let height = CalcHeight();
    let options = {
        dom: '<"top"<"action-filters d-flex align-items-center justify-content-between flex-wrap mb-1"lf<"actions ms-1 d-none"B>>><"clear">tr<"bottom"<"actions"><"row"<"col-md-6"i><"col-md-6"p>>>',
        scrollY: height,
        scrollX: true,
        order: [[0, 'asc']],
        fixedColumns: { left: 3 },
        paging: true,
        pageLength: 25,
        processing: true,
        stateSave: false,
        language: MyAppLang.datatable,
        ajax: dataParamsTableLo(),
        columns: columnTableLo(),
        select: {
            style: 'multi',
            selector: 'td:not(:first-child)'
        },
        initComplete: function () {
            dataTableLo.columns.adjust();
            setTimeout(function myfunction() {
                $(window).trigger('resize');
            }, 500);
        }
    };
    if ($(window).width() < 767) { options.fixedColumns = { left: 2 }; }
    dataTableLo = $tableMainLo.DataTable(options);
    setTimeout(function myfunction() {
        $(window).trigger('resize');
    }, 500);
}

function getSelectedRowIds() {
    if (!dataTableLo) return [];
    try {
        const rows = dataTableLo.rows({ selected: true }).data().toArray();
        const ids = rows.map(r => r?.id ?? r?.Id).filter(Boolean);
        return Array.from(new Set(ids));
    } catch (e) {
        return [];
    }
}

function LoadSelectYears() {
    $selectYearElm.empty();
    $selectYearElm.append(FIRST_OPTION);

    $.ajax(dataSelectYears()).done(function (response) {
        if (response && response.result === 1 && Array.isArray(response.data)) {
            response.data.forEach(function (item) {
                $selectYearElm.append('<option value="' + item + '">' + item + '</option>');
            });
            $selectYearElm.trigger("change.select2");
        }
    });
}

function LoadSelectVarieties() {
    $selectVarietyElm.empty();
    $selectVarietyElm.append(FIRST_OPTION);
    $.ajax(dataSelectVarieties()).done(function (response) {
        response.data.forEach(function (item) {
            $selectVarietyElm.append('<option value="' + item + '">' + item + '</option>');
        });
        $selectVarietyElm.trigger("change.select2");
    });
}

function ImportExcelFromFile() {
    const fileInput = $('#import_excel_file')[0];
    if (fileInput.files.length === 0) {
        iziToast.warning({ title: 'Cảnh báo', message: 'Vui lòng chọn file Excel!' });
        return;
    }

    const formData = new FormData();
    formData.append("file", fileInput.files[0]);

    // Lấy Anti-Forgery Token
    const token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: "/Table/ImportExcel",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        headers: {
            'RequestVerificationToken': token
        },
        success: function (res) {
            console.log("Dữ liệu Excel từ server:", res);

            if (res.result === 1) {
                iziToast.success({ title: 'Thành công', message: res.error?.message || 'Import dữ liệu thành công!' });

                if (res.data && res.data.length > 0) {
                    // console.log("=== DỮ LIỆU EXCEL ===");

                    const seen = new Set();
                    const duplicates = [];

                    res.data.forEach((row, index) => {
                        if (seen.has(row.idPrivate)) {
                            duplicates.push({ index: index + 1, id: row.idPrivate });
                        } else {
                            seen.add(row.idPrivate);
                        }
                        // console.log(`Dòng ${index + 1}:`, row);
                    });

                    // if (duplicates.length > 0) {
                    //     console.warn("⚠️ Các idPrivate bị trùng:", duplicates);
                    //     iziToast.warning({
                    //         title: 'Cảnh báo',
                    //         message: `Có ${duplicates.length} dòng bị trùng idPrivate!`
                    //     });
                    // }
                    // else {
                    //     iziToast.success({ title: 'Thành công', message: 'Không có dữ liệu trùng!' });
                    // }
                }

                dataTableLo.ajax.reload();
            } else {
                iziToast.error({ title: 'Lỗi', message: res.error?.message || 'Import thất bại!' });
            }
        },
        error: function (xhr) {
            // console.error("Lỗi AJAX:", xhr);
            iziToast.error({ title: 'Lỗi hệ thống', message: xhr.responseText });
        }
    });
}

function ExportExcel() {
    const year = $selectYearElm.val();
    const variety = $selectVarietyElm.val();
    const ids = getSelectedRowIds();

    let timerInterval;
    Swal.fire({
        title: "Đang xử lý...",
        html: "Vui lòng chờ <b></b> ms.",
        timer: 3000,
        timerProgressBar: true,
        didOpen: () => {
            Swal.showLoading();
            const timer = Swal.getHtmlContainer().querySelector("b");
            timerInterval = setInterval(() => {
                if (timer) timer.textContent = `${Swal.getTimerLeft()}`;
            }, 100);
        },
        willClose: () => {
            clearInterval(timerInterval);
        }
    });

    $.ajax({
        type: 'POST',
        url: '/Table/ExportExcel',
        data: { year: year, variety: variety, ids: ids },
        success: function (response) {
            if (!CheckResponseIsSuccess(response)) return;

            const linkDownload = location.origin + (response.data || '');
            if (typeof DownloadFile === 'function') {
                DownloadFile(linkDownload);
            } else {
                const a = document.createElement('a');
                a.href = linkDownload;
                a.download = '';
                document.body.appendChild(a);
                a.click();
                document.body.removeChild(a);
            }
            Swal.fire('Đã tải!', 'Tệp Excel đã được tải xuống thành công.', 'success');
        },
        error: function (err) {
            Swal.close();
            CheckResponseIsSuccess({ result: -1, error: { code: err.status } });
            Swal.fire({
                title: 'Lỗi',
                text: 'Có lỗi xảy ra khi tải tệp. Vui lòng thử lại sau.',
                icon: 'error',
                confirmButtonText: 'Đóng'
            });
        }
    });
}