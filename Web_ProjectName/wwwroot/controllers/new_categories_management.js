let categoriesDataTable;
let categoriesManager;

const CONSTANTS = {
  STATUS: {
    INACTIVE: 0,
    ACTIVE: 1,
  },
  SELECTORS: {
    VIEW_PANEL: "#div_view_panel",
    EDIT_PANEL: "#div_edit_panel",
    CATEGORIES_TABLE: "#categories_dataTable",
    STATUS_FILTER: "#status_filter",
    CATEGORY_FILTER: "#category_filter",
    SEARCH_KEYWORD: "#search_keyword",
    ADD_MODAL: "#addCategoryModal",
    EDIT_MODAL: "#editCategoryModal",
    VIEW_MODAL: "#viewCategoryModal",
    DELETE_MODAL: "#deleteCategoryModal",
  },
  MESSAGES: {
    SUCCESS: "Thành công",
    ERROR: "Lỗi",
    INFO: "Thông tin",
    LOADING: "Đang tìm kiếm...",
    REFRESH: "Đã làm mới",
    RESET: "Đã làm mới",
  },
};

function FormatDate(dateString) {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toLocaleDateString("vi-VN");
}

function GetStatusBadge(status) {
  switch (status) {
    case CONSTANTS.STATUS.INACTIVE:
      return '<span class="badge bg-danger">Không hoạt động</span>';
    case CONSTANTS.STATUS.ACTIVE:
      return '<span class="badge bg-success">Hoạt động</span>';
    default:
      return '<span class="badge bg-secondary">Không xác định</span>';
  }
}

function WaitForJQuery() {
  if (typeof $ !== "undefined") {
    $(document).ready(function () {
      InitializeComponents();
      LoadCategoryFilter();
      InitializeDataTable();
      BindEvents();
    });
  } else {
    setTimeout(WaitForJQuery, 100);
  }
}

function InitializeComponents() {
  InitializeSelect2();
  InitializeDatepicker();
}

function InitializeSelect2() {
  try {
    const select2Config = {
      placeholder: "Chọn một tùy chọn",
      allowClear: true,
      language: "vi",
    };

    $(CONSTANTS.SELECTORS.STATUS_FILTER).select2({
      ...select2Config,
      placeholder: "Chọn trạng thái",
    });

    $(CONSTANTS.SELECTORS.CATEGORY_FILTER).select2({
      ...select2Config,
      placeholder: "Chọn danh mục",
    });

    [CONSTANTS.SELECTORS.ADD_MODAL, CONSTANTS.SELECTORS.EDIT_MODAL].forEach(
      (modalSelector) => {
        $(modalSelector).on("shown.bs.modal", function () {
          $(this)
            .find(".select2")
            .select2({
              ...select2Config,
              dropdownParent: $(this),
            });
        });
      }
    );
  } catch (error) {
    console.error("Error initializing Select2:", error);
  }
}

function InitializeDatepicker() {
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

function InitializeDataTable() {
  categoriesDataTable = $(CONSTANTS.SELECTORS.CATEGORIES_TABLE).DataTable({
    processing: true,
    serverSide: false,
    select: {
      style: "multi",
      selector: 'td:not(:nth-child(1))',
    },
    ajax: {
      url: "/NewCategory/GetListByStatus",
      type: "GET",
      data: (d) => ({
        keyword: $(CONSTANTS.SELECTORS.SEARCH_KEYWORD).val(),
        status: $(CONSTANTS.SELECTORS.STATUS_FILTER).val() || null,
      }),
      dataSrc: (json) => (json.result === 1 && json.data ? json.data : []),
    },
    columns: [
      {
        data: "id",
        width: "5%",
        render: (data) => `
          <div class="d-flex justify-content-center align-items-center text-center" style="height: 100%;">
            <button type="button" class="btn btn-sm btn-outline-success d-flex justify-content-center align-items-center" 
                    onclick="ViewCategory(${data})" title="Xem">
              <i class="fas fa-search"></i>
            </button>
          </div>`,
      },
      {
        data: null,
        width: "8%",
        render: (data, type, row, meta) =>
          `<div class="text-center">${meta.row + 1}</div>`,
      },
      {
        data: "name",
        width: "20%",
        render: (data) => `<div class="text-center">${data}</div>`,
      },
      {
        data: null,
        width: "20%",
        render: (data, type, row) => {
          const metaUrl = row.metaUrl || GenerateMetaUrl(row.name);
          return metaUrl
            ? `<div class="text-center"><code>${metaUrl}</code></div>`
            : '<div class="text-center text-muted">-</div>';
        },
      },
      {
        data: "status",
        width: "12%",
        render: (data) =>
          `<div class="text-center">${GetStatusBadge(data)}</div>`,
      },
      {
        data: "createdAt",
        width: "15%",
        render: (data) =>
          `<div class="text-center">${data ? FormatDate(data) : "-"
          }</div>`,
      },
      {
        data: "updatedAt",
        width: "15%",
        render: (data) =>
          `<div class="text-center">${data ? FormatDate(data) : "-"
          }</div>`,
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

async function LoadCategoryFilter() {
  try {
    const response = await CategoryApi.getList();
    if (response?.result === 1 && Array.isArray(response.data)) {
      const $categoryFilter = $(CONSTANTS.SELECTORS.CATEGORY_FILTER);
      $categoryFilter
        .empty()
        .append('<option value="">Tất cả danh mục</option>');

      response.data.forEach((item) => {
        $categoryFilter.append(
          `<option value="${item.id}">${item.name}</option>`
        );
      });

      $categoryFilter.trigger("change");
    }
  } catch (error) {
    console.error("Không thể load danh mục:", error);
  }
}

async function ViewCategory(id) {
  try {
    const response = await CategoryApi.getById(id);
    const result = ApiUtils.handleResponse(
      response,
      null,
      "Không thể tải thông tin danh mục."
    );

    if (result.success) {
      DisplayCategoryDetails(result.data);
    }
  } catch (error) {
    ApiUtils.handleError(error, "Không thể kết nối đến máy chủ.");
  }
}

function DisplayCategoryDetails(category) {
  const content = `
    <div id="categoryDetailContainer">
      <div class="card mb-4">
        <div class="card-header d-flex align-items-center bg-inverse bg-opacity-10 fw-400">
          Thông tin
          <div class="ms-auto">
            <a href="javascript:void(0)" class="text-decoration-none text-danger fw-bold ms-2" 
               onclick="DeleteCategory(${category.id})">
              Xóa <i class="fas fa-fw me-1 fa-trash"></i>
            </a>
            <a href="javascript:void(0)" class="text-decoration-none text-success fw-bold ms-2" 
               onclick="EditCategory(${category.id})">
              Sửa <i class="fas fa-fw me-1 fa-edit"></i>
            </a>
          </div>
        </div>
        <div class="card-body">
          <table class="table table-borderless table-striped table-sm m-0">
            <tbody>
              <tr>
                <td class="w-150px fw-bold">Tên danh mục</td>
                <td>${category.name}</td>
              </tr>
              <tr>
                <td class="fw-bold">URL Meta</td>
                <td>${category.metaUrl || GenerateMetaUrl(category.name)
    }</td>
              </tr>
              <tr>
                <td class="fw-bold">Trạng thái</td>
                <td>${GetStatusBadge(category.status)}</td>
              </tr>
              <tr>
                <td class="fw-bold">Ngày tạo</td>
                <td>${FormatDate(category.createdAt)}</td>
              </tr>
              <tr>
                <td class="fw-bold">Cập nhật lần cuối</td>
                <td>${FormatDate(category.updatedAt)}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="card">
        <div class="card-header d-flex align-items-center bg-inverse bg-opacity-10 fw-400">
          Thống kê
        </div>
        <div class="card-body">
          <div class="mb-2">
            <span>Số tin tức:</span>
            <span class="badge bg-primary float-end" id="stat_newsCount">0</span>
          </div>
          <div class="mb-2">
            <span>Tổng lượt xem:</span>
            <span class="badge bg-info float-end" id="stat_viewCount">0</span>
          </div>
        </div>
      </div>
    </div>
  `;

  $("#viewCategoryContent").html(content);
  $(CONSTANTS.SELECTORS.VIEW_MODAL).modal("show");
}

async function EditCategory(id) {
  try {
    $(CONSTANTS.SELECTORS.VIEW_MODAL).modal("hide");
    const response = await CategoryApi.getById(id);
    const result = ApiUtils.handleResponse(
      response,
      null,
      "Không thể tải thông tin danh mục."
    );

    if (result.success) {
      PopulateEditForm(result.data);
      $(CONSTANTS.SELECTORS.EDIT_MODAL).modal("show");
    }
  } catch (error) {
    ApiUtils.handleError(error, "Không thể kết nối đến máy chủ.");
  }
}

function PopulateEditForm(category) {
  $("#edit_category_id").val(category.id);
  $("#edit_category_name").val(category.name);
  $("#edit_category_status").val(category.status || CONSTANTS.STATUS.INACTIVE);
  $("#edit_category_metaUrl").val(category.metaUrl || "");
}

function DeleteCategory(id) {
  $(CONSTANTS.SELECTORS.DELETE_MODAL).modal("show");
  $("#delete_category_id").val(id);
}

async function ConfirmDelete() {
  const id = $("#delete_category_id").val();
  if (!id) return;

  try {
    const response = await CategoryApi.delete(id);
    const result = ApiUtils.handleResponse(
      response,
      "Đã xóa danh mục thành công",
      "Không thể xóa danh mục"
    );

    if (result.success) {
      $(CONSTANTS.SELECTORS.DELETE_MODAL).modal("hide");
      $(CONSTANTS.SELECTORS.VIEW_MODAL).modal("hide");
      categoriesDataTable.ajax.reload();
    }
  } catch (error) {
    ApiUtils.handleError(error, "Không thể kết nối đến máy chủ");
  }
}

async function SaveCategory() {
  const formData = BuildFormData(
    "#category_name",
    "#category_status",
    "#category_metaUrl"
  );

  if (!ValidateForm(formData)) return;

  try {
    const response = await CategoryApi.save(formData);
    const result = ApiUtils.handleResponse(
      response,
      "Đã lưu danh mục thành công.",
      "Không thể lưu danh mục."
    );

    if (result.success) {
      $(CONSTANTS.SELECTORS.ADD_MODAL).modal("hide");
      categoriesDataTable.ajax.reload();
      ResetAddForm();
    }
  } catch (error) {
    ApiUtils.handleError(error, "Không thể kết nối đến máy chủ.");
  }
}

async function UpdateCategory() {
  const formData = BuildFormData(
    "#edit_category_name",
    "#edit_category_status",
    "#edit_category_metaUrl",
    "#edit_category_id"
  );

  if (!ValidateForm(formData)) return;

  try {
    const response = await CategoryApi.update(formData);
    const result = ApiUtils.handleResponse(
      response,
      "Đã cập nhật danh mục thành công.",
      "Không thể cập nhật danh mục."
    );

    if (result.success) {
      $(CONSTANTS.SELECTORS.EDIT_MODAL).modal("hide");
      categoriesDataTable.ajax.reload();
      ResetEditForm();
    }
  } catch (error) {
    ApiUtils.handleError(error, "Không thể kết nối đến máy chủ.");
  }
}

function BuildFormData(
  nameSelector,
  statusSelector,
  metaUrlSelector,
  idSelector = null
) {
  const formData = {
    name: $(nameSelector).val(),
    status: parseInt($(statusSelector).val()) || CONSTANTS.STATUS.INACTIVE,
    metaUrl:
      $(metaUrlSelector).val() || GenerateMetaUrl($(nameSelector).val()),
  };

  if (idSelector) {
    formData.id = parseInt($(idSelector).val());
  }

  return formData;
}

function ValidateForm(formData) {
  if (!formData.name?.trim()) {
    ShowToast(
      "error",
      CONSTANTS.MESSAGES.ERROR,
      "Vui lòng nhập tên danh mục."
    );
    return false;
  }

  if (
    formData.status === undefined ||
    formData.status === null ||
    formData.status === ""
  ) {
    ShowToast(
      "error",
      CONSTANTS.MESSAGES.ERROR,
      "Vui lòng chọn trạng thái."
    );
    return false;
  }

  return true;
}

function ResetAddForm() {
  $("#addCategoryForm")[0].reset();
  ResetSelect2("#category_status");
}

function ResetEditForm() {
  $("#editCategoryForm")[0].reset();
  ResetSelect2("#edit_category_status");
  $("#edit_category_metaUrl").val("");
}

function ResetSelect2(selector) {
  if (typeof $.fn.select2 !== "undefined") {
    $(selector).val("").trigger("change.select2");
  } else {
    $(selector).val("");
  }
}

function ResetFilters() {
  $(CONSTANTS.SELECTORS.SEARCH_KEYWORD).val("");

  if (typeof $.fn.select2 !== "undefined") {
    $(CONSTANTS.SELECTORS.STATUS_FILTER).val("").trigger("change.select2");
    $(CONSTANTS.SELECTORS.CATEGORY_FILTER).val("").trigger("change.select2");
  } else {
    $(CONSTANTS.SELECTORS.STATUS_FILTER).val("");
    $(CONSTANTS.SELECTORS.CATEGORY_FILTER).val("");
  }

  categoriesDataTable.ajax.reload();
  ShowToast("info", CONSTANTS.MESSAGES.INFO, CONSTANTS.MESSAGES.RESET);
}

function HandleCategoryFilterChange(event) {
  const $select = $(event.target);
  const selectedText = $select.find("option:selected").text();

  if (!$select.val()) {
    categoriesDataTable.column(2).search("").draw();
  } else {
    categoriesDataTable.column(2).search(`^${selectedText}$`, true, false).draw();
  }
}

function HandleStatusFilterChange() {
  categoriesDataTable.ajax.reload();
}

async function ApplyBulkStatus() {
  const statusValue = $("#bulk_status_select").val();
  if (!statusValue) {
    ShowToast("warning", "Cảnh báo", "Vui lòng chọn trạng thái cần cập nhật.");
    return;
  }

  if (!categoriesDataTable.select) {
    ShowToast("error", "Lỗi", "Plugin chọn nhiều dòng chưa được tải.");
    return;
  }

  const api = categoriesDataTable;
  const selected = api.rows({ selected: true });
  const selectedData = selected.data().toArray();

  if (selectedData.length === 0) {
    ShowToast("warning", "Cảnh báo", "Vui lòng chọn ít nhất một bản ghi.");
    return;
  }

  const confirmText = statusValue === "1" ? "Mở khóa" : "Khóa";
  const result = await Swal.fire({
    icon: "question",
    title: `Xác nhận ${confirmText}`,
    text: `Bạn có chắc chắn muốn ${confirmText.toLowerCase()} ${selectedData.length} danh mục đã chọn?`,
    showCancelButton: true,
    confirmButtonText: "Cập nhật",
    cancelButtonText: "Hủy",
  });

  if (!result.isConfirmed) return;

  const requests = selectedData.map(row =>
    CategoryApi.updateStatus(row.id, parseInt(statusValue))
      .then(res => ({ id: row.id, success: res?.result === 1 }))
      .catch(() => ({ id: row.id, success: false }))
  );

  const results = await Promise.all(requests);
  const successCount = results.filter(r => r.success).length;
  const errorCount = results.length - successCount;

  if (successCount > 0) {
    ShowToast(
      "success",
      CONSTANTS.MESSAGES.SUCCESS,
      `Đã cập nhật ${successCount} bản ghi${errorCount ? `, lỗi: ${errorCount}` : ""}.`
    );
    api.rows({ selected: true }).deselect();
    api.ajax.reload(null, false);
  } else {
    ShowToast("error", "Thất bại", "Không thể cập nhật bản ghi nào.");
  }
}

function HandleBtnSearchClick() {
  categoriesDataTable.ajax.reload();
  ShowToast(
    "success",
    CONSTANTS.MESSAGES.SUCCESS,
    CONSTANTS.MESSAGES.LOADING
  );
}

function HandleBtnResetClick() {
  ResetFilters();
}

function HandleSaveCategoryClick() {
  SaveCategory();
}

function HandleUpdateCategoryClick() {
  UpdateCategory();
}

function HandleConfirmDeleteClick() {
  ConfirmDelete();
}

function HandleAddModalHidden() {
  ResetAddForm();
}

function HandleEditModalHidden() {
  ResetEditForm();
}

function HandleNameInput(event) {
  const name = $(event.target).val();
  const metaUrlField =
    $(event.target).attr("id") === "category_name"
      ? "#category_metaUrl"
      : "#edit_category_metaUrl";

  if (name && !$(metaUrlField).val()) {
    $(metaUrlField).val(GenerateMetaUrl(name));
  }
}

function HandleAddFormSubmit(event) {
  event.preventDefault();
  SaveCategory();
}

function HandleEditFormSubmit(event) {
  event.preventDefault();
  UpdateCategory();
}

function BindEvents() {
  $("#btn_search").off("click").on("click", HandleBtnSearchClick);
  $("#btn_reset").off("click").on("click", HandleBtnResetClick);
  $("#btn_save_category").off("click").on("click", HandleSaveCategoryClick);
  $("#btn_update_category").off("click").on("click", HandleUpdateCategoryClick);
  $("#btn_confirm_delete").off("click").on("click", HandleConfirmDeleteClick);

  $(CONSTANTS.SELECTORS.STATUS_FILTER).off("change").on("change", HandleStatusFilterChange);
  $(CONSTANTS.SELECTORS.CATEGORY_FILTER).off("change").on("change", HandleCategoryFilterChange);

  $(CONSTANTS.SELECTORS.ADD_MODAL).off("hidden.bs.modal").on("hidden.bs.modal", HandleAddModalHidden);
  $(CONSTANTS.SELECTORS.EDIT_MODAL).off("hidden.bs.modal").on("hidden.bs.modal", HandleEditModalHidden);

  $("#addCategoryForm").off("submit").on("submit", HandleAddFormSubmit);
  $("#editCategoryForm").off("submit").on("submit", HandleEditFormSubmit);

  $("#category_name, #edit_category_name").off("input").on("input", HandleNameInput);
}

WaitForJQuery();
