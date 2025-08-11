let categoriesDataTable;
let currentDeleteId = null;

$(document).ready(function () {
  initializeComponents();
  initializeDataTable();
  bindEvents();
});

function initializeComponents() {
  initializeSelect2();
  initializeDatepicker();
  loadCategoryFilter();
}

async function loadCategoryFilter() {
  try {
    const response = await CategoryApi.getList();
    if (response && response.result === 1 && Array.isArray(response.data)) {
      var $cat = $("#categoryFilter");
      $cat.empty().append('<option value="">Tất cả danh mục</option>');
      response.data.forEach(function (item) {
        $cat.append('<option value="' + item.id + '">' + item.name + "</option>");
      });
      $cat.trigger("change");
    }
  } catch (e) {
    console.error("Không thể load danh mục:", e);
  }
}

function initializeSelect2() {
  try {
    $("#statusFilter").select2({
      placeholder: "Chọn trạng thái",
      allowClear: true,
      language: "vi",
    });

    $("#categoryFilter").select2({
      placeholder: "Chọn danh mục",
      allowClear: true,
      language: "vi",
    });

    $("#addCategoryModal, #editCategoryModal").on(
      "shown.bs.modal",
      function () {
        $(this)
          .find(".select2")
          .select2({
            dropdownParent: $(this),
            placeholder: "Chọn một tùy chọn",
            allowClear: true,
            language: "vi",
          });
      }
    );
  } catch (error) {
    console.error("Error initializing Select2:", error);
  }
}

function initializeDatepicker() {
  try {
    $(".datepicker").datepicker({
      format: "dd/mm/yyyy",
      language: "vi",
      autoclose: true,
      todayHighlight: true,
    });
  } catch (error) {
    console.error("Error initializing Datepicker:", error);
  }
}

function bindEvents() {
  $("#btnSearch").click(function () {
    categoriesDataTable.ajax.reload();
    showToast("success", "Đang tìm kiếm...", "Vui lòng chờ trong giây lát.");
  });

  $("#btnReset").click(function () {
    resetFilters();
  });

  $("#btnSaveCategory").click(saveCategory);
  $("#btnUpdateCategory").click(updateCategory);
  $("#btnConfirmDelete").click(confirmDelete);

  $("#addCategoryModal").on("hidden.bs.modal", resetAddForm);
  $("#editCategoryModal").on("hidden.bs.modal", resetEditForm);

  $("#statusFilter").change(function () {
    categoriesDataTable.ajax.reload();
  });

  $("#categoryFilter").change(function () {
      var selectedText = $(this).find('option:selected').text();
    if (!$(this).val()) {
      categoriesDataTable.column(2).search('').draw();
    } else {
      categoriesDataTable.column(2).search('^' + selectedText + '$', true, false).draw();
    }
  });
}

function resetFilters() {
  $("#searchKeyword").val("");
  if (typeof $.fn.select2 !== "undefined") {
    $("#statusFilter").val("").trigger("change.select2");
  } else {
    $("#statusFilter").val("");
  }
  categoriesDataTable.ajax.reload();
  showToast("info", "Đã làm mới", "Đã xóa tất cả bộ lọc.");
}

function initializeDataTable() {
  categoriesDataTable = $("#categoriesDataTable").DataTable({
    processing: true,
    serverSide: false,
    ajax: {
      url: "/NewCategory/GetListByStatus",
      type: "GET",
      data: function (d) {
        var statusFilter = $("#statusFilter").val();
        return {
          keyword: $("#searchKeyword").val(),
          status: statusFilter === "" ? null : parseInt(statusFilter),
        };
      },
      dataSrc: function (json) {
        if (json.result === 1 && json.data) {
          return json.data;
        }
        return [];
      },
    },
    columns: [
      {
        data: "id",
        width: "5%",
        render: function (data, type, row) {
          return `<div class="d-flex justify-content-center align-items-center text-center" style="height: 100%;">
          <button type="button" class="btn btn-sm btn-outline-success d-flex justify-content-center align-items-center" onclick="viewCategory(${data})" title="Xem">
            <i class="fas fa-search"></i>
          </button>
        </div>`;
        },
      },
      {
        data: null,
        width: "8%",
        render: function (data, type, row, meta) {
          return `<div class="text-center">${meta.row + 1}</div>`;
        },
      },
      {
        data: "name",
        width: "20%",
        render: function (data, type, row) {
          return `<div class="text-center">${data}</div>`;
        },
      },
      {
        data: null,
        width: "20%",
        render: function (data, type, row) {
          const metaUrl = row.metaUrl || generateMetaUrl(row.name);
          return metaUrl
            ? `<div class="text-center"><code>${metaUrl}</code></div>`
            : '<div class="text-center text-muted">-</div>';
        },
      },
      {
        data: "status",
        width: "12%",
        render: function (data, type, row) {
          switch (data) {
            case 0:
              return `<div class="text-center"><span class="badge bg-danger">Không hoạt động</span></div>`;
            case 1:
              return `<div class="text-center"><span class="badge bg-success">Hoạt động</span></div>`;
            default:
              return `<div class="text-center"><span class="badge bg-secondary">Không xác định</span></div>`;
          }
        },
      },
      {
        data: "createdAt",
        width: "15%",
        render: function (data, type, row) {
          return `<div class="text-center">${
            data ? formatDate(data) : "-"
          }</div>`;
        },
      },
      {
        data: "updatedAt",
        width: "15%",
        render: function (data, type, row) {
          return `<div class="text-center">${
            data ? formatDate(data) : "-"
          }</div>`;
        },
      },
    ],
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
        last: "Cuối",
      },
    },
    responsive: true,
    pageLength: 10,
    lengthMenu: [
      [10, 25, 50, -1],
      [10, 25, 50, "Tất cả"],
    ],
    order: [[0, "desc"]],
  });
}

async function viewCategory(id) {
  try {
    const response = await CategoryApi.getById(id);
    const result = ApiUtils.handleResponse(
      response,
      null,
      "Không thể tải thông tin danh mục."
    );

    if (result.success) {
      displayCategoryDetails(result.data);
    }
  } catch (error) {
    ApiUtils.handleError(error, "Không thể kết nối đến máy chủ.");
  }
}

function displayCategoryDetails(category) {
  let content = `
        <div class="row">
            <div class="col-md-8">
                <h4>${category.name}</h4>
                <div class="mb-3">
                    <strong>URL Meta:</strong> <code>${
                      category.metaUrl || generateMetaUrl(category.name)
                    }</code>
                </div>
                <div class="mb-3">
                    <strong>Trạng thái:</strong> ${getStatusBadge(
                      category.status
                    )}
                </div>
                <div class="mb-3">
                    <strong>Ngày tạo:</strong> ${formatDate(category.createdAt)}
                </div>
                <div class="mb-3">
                    <strong>Cập nhật lần cuối:</strong> ${formatDate(
                      category.updatedAt
                    )}
                </div>
            </div>
            <div class="col-md-4">
                <div class="card">
                    <div class="card-header">
                        <h6 class="mb-0">Thống kê</h6>
                    </div>
                    <div class="card-body">
                        <div class="mb-2">
                            <span>Số tin tức:</span>
                            <span class="badge bg-primary float-end">0</span>
                        </div>
                        <div class="mb-2">
                            <span>Tổng lượt xem:</span>
                            <span class="badge bg-info float-end">0</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;
  $("#viewCategoryContent").html(content);
  $("#viewCategoryModal").modal("show");
}

async function editCategory(id) {
  try {
    const response = await CategoryApi.getById(id);
    const result = ApiUtils.handleResponse(
      response,
      null,
      "Không thể tải thông tin danh mục."
    );

    if (result.success) {
      populateEditForm(result.data);
      $("#editCategoryModal").modal("show");
    }
  } catch (error) {
    ApiUtils.handleError(error, "Không thể kết nối đến máy chủ.");
  }
}

function populateEditForm(category) {
  $("#editCategoryId").val(category.id);
  $("#editCategoryName").val(category.name);
  $("#editCategoryStatus").val(category.status || 0);
  $("#editCategoryMetaUrl").val(category.metaUrl || "");
}

function deleteCategory(id) {
  currentDeleteId = id;
  $("#deleteCategoryModal").modal("show");
}

function confirmDelete() {
  if (!currentDeleteId) return;

  $.ajax({
    url: `/NewCategory/delete?id=${currentDeleteId}`,
    type: "DELETE",
    success: function (response) {
      if (response.result === 1) {
        showToast("success", "Thành công", "Đã xóa danh mục thành công");
        $("#deleteCategoryModal").modal("hide");
        categoriesDataTable.ajax.reload();
      } else {
        showToast(
          "error",
          "Lỗi",
          response.error?.message || "Không thể xóa danh mục"
        );
      }
    },
    error: function () {
      showToast("error", "Lỗi", "Không thể kết nối đến máy chủ");
    },
  });
}

async function saveCategory() {
  const formData = buildAddFormData();

  if (!validateCategoryForm(formData)) {
    return;
  }

  try {
    const response = await CategoryApi.save(formData);
    const result = ApiUtils.handleResponse(
      response,
      "Đã lưu danh mục thành công.",
      "Không thể lưu danh mục."
    );

    if (result.success) {
      $("#addCategoryModal").modal("hide");
      categoriesDataTable.ajax.reload();
      resetAddForm();
    }
  } catch (error) {
    ApiUtils.handleError(error, "Không thể kết nối đến máy chủ.");
  }
}

async function updateCategory() {
  const formData = buildEditFormData();

  if (!validateCategoryForm(formData)) {
    return;
  }

  try {
    const response = await CategoryApi.update(formData);
    const result = ApiUtils.handleResponse(
      response,
      "Đã cập nhật danh mục thành công.",
      "Không thể cập nhật danh mục."
    );

    if (result.success) {
      $("#editCategoryModal").modal("hide");
      categoriesDataTable.ajax.reload();
      resetEditForm();
    }
  } catch (error) {
    ApiUtils.handleError(error, "Không thể kết nối đến máy chủ.");
  }
}

function buildAddFormData() {
  const formData = {
    name: $("#categoryName").val(),
    status: parseInt($("#categoryStatus").val()) || 0,
    metaUrl:
      $("#categoryMetaUrl").val() || generateMetaUrl($("#categoryName").val()),
  };

  return formData;
}

function buildEditFormData() {
  const formData = {
    id: parseInt($("#editCategoryId").val()),
    name: $("#editCategoryName").val(),
    status: parseInt($("#editCategoryStatus").val()) || 0,
    metaUrl:
      $("#editCategoryMetaUrl").val() ||
      generateMetaUrl($("#editCategoryName").val()),
  };

  return formData;
}

function validateCategoryForm(formData) {
  if (!formData.name || formData.name.trim() === "") {
    showToast("error", "Lỗi", "Vui lòng nhập tên danh mục.");
    return false;
  }

  if (
    formData.status === undefined ||
    formData.status === null ||
    formData.status === ""
  ) {
    showToast("error", "Lỗi", "Vui lòng chọn trạng thái.");
    return false;
  }

  return true;
}

function resetAddForm() {
  $("#addCategoryForm")[0].reset();
  if (typeof $.fn.select2 !== "undefined") {
    $("#categoryStatus").val("").trigger("change.select2");
  } else {
    $("#categoryStatus").val("");
  }
}

function resetEditForm() {
  $("#editCategoryForm")[0].reset();
  if (typeof $.fn.select2 !== "undefined") {
    $("#editCategoryStatus").val("").trigger("change.select2");
  } else {
    $("#editCategoryStatus").val("");
  }
  $("#editCategoryMetaUrl").val("");
}

function formatDate(dateString) {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toLocaleDateString("vi-VN");
}

function generateMetaUrl(name) {
  if (!name) return "";
  return name
    .toLowerCase()
    .replace(/[àáạảãâầấậẩẫăằắặẳẵ]/g, "a")
    .replace(/[èéẹẻẽêềếệểễ]/g, "e")
    .replace(/[ìíịỉĩ]/g, "i")
    .replace(/[òóọỏõôồốộổỗơờớợởỡ]/g, "o")
    .replace(/[ùúụủũưừứựửữ]/g, "u")
    .replace(/[ỳýỵỷỹ]/g, "y")
    .replace(/đ/g, "d")
    .replace(/[^a-z0-9\s-]/g, "")
    .replace(/\s+/g, "-")
    .replace(/-+/g, "-")
    .trim("-");
}

function getStatusBadge(status) {
  switch (status) {
    case 0:
      return '<span class="badge bg-danger">Không hoạt động</span>';
    case 1:
      return '<span class="badge bg-success">Hoạt động</span>';
    default:
      return '<span class="badge bg-secondary">Không xác định</span>';
  }
}

function showToast(type, title, message) {
  if (typeof iziToast !== "undefined") {
    iziToast[type]({
      title: title,
      message: message,
      position: "topRight",
      timeout: 5000,
    });
  } else {
    alert(`${title}: ${message}`);
  }
}

$(document).ready(function () {
  $("#categoryName, #editCategoryName").on("input", function () {
    const name = $(this).val();
    const metaUrlField =
      $(this).attr("id") === "categoryName"
        ? "#categoryMetaUrl"
        : "#editCategoryMetaUrl";

    if (name && !$(metaUrlField).val()) {
      const metaUrl = generateMetaUrl(name);
      $(metaUrlField).val(metaUrl);
    }
  });
});

$(document).ready(function () {
  $("#addCategoryForm").on("submit", function (e) {
    e.preventDefault();
    saveCategory();
  });

  $("#editCategoryForm").on("submit", function (e) {
    e.preventDefault();
    updateCategory();
  });
});

function refreshTable() {
  categoriesDataTable.ajax.reload();
  showToast("info", "Đã làm mới", "Dữ liệu đã được cập nhật.");
}

window.viewCategory = viewCategory;
window.editCategory = editCategory;
window.deleteCategory = deleteCategory;
window.confirmDelete = confirmDelete;
window.refreshTable = refreshTable;
window.saveCategory = saveCategory;
window.updateCategory = updateCategory;
