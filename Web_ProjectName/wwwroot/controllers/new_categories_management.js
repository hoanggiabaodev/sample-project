const CONSTANTS = {
  STATUS: {
    INACTIVE: 0,
    ACTIVE: 1,
  },
  SELECTORS: {
    VIEW_PANEL: "#div_view_panel",
    EDIT_PANEL: "#div_edit_panel",
    CATEGORIES_TABLE: "#categoriesDataTable",
    STATUS_FILTER: "#statusFilter",
    CATEGORY_FILTER: "#categoryFilter",
    SEARCH_KEYWORD: "#searchKeyword",
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

class CategoriesManager {
  constructor() {
    this.dataTable = null;
    this.currentDeleteId = null;
    this.currentEditingId = null;
    this.init();
  }

  init() {
    this.initializeComponents();
    this.initializeDataTable();
    this.bindEvents();
  }

  initializeComponents() {
    this.initializeSelect2();
    this.initializeDatepicker();
    this.loadCategoryFilter();
  }

  async loadCategoryFilter() {
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

  initializeSelect2() {
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

  initializeDatepicker() {
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

  bindEvents() {
    $("#btnSearch").click(() => {
      this.dataTable.ajax.reload();
      this.showToast(
        "success",
        CONSTANTS.MESSAGES.SUCCESS,
        CONSTANTS.MESSAGES.LOADING
      );
    });

    $("#btnReset").click(() => this.resetFilters());

    $("#btnSaveCategory").click(() => this.saveCategory());
    $("#btnUpdateCategory").click(() => this.updateCategory());
    $("#btnConfirmDelete").click(() => this.confirmDelete());

    $(CONSTANTS.SELECTORS.ADD_MODAL).on("hidden.bs.modal", () =>
      this.resetAddForm()
    );
    $(CONSTANTS.SELECTORS.EDIT_MODAL).on("hidden.bs.modal", () =>
      this.resetEditForm()
    );

    $(CONSTANTS.SELECTORS.STATUS_FILTER).change(() =>
      this.dataTable.ajax.reload()
    );
    $(CONSTANTS.SELECTORS.CATEGORY_FILTER).change((e) =>
      this.handleCategoryFilterChange(e)
    );

    $("#addCategoryForm").on("submit", (e) => {
      e.preventDefault();
      this.saveCategory();
    });

    $("#editCategoryForm").on("submit", (e) => {
      e.preventDefault();
      this.updateCategory();
    });

    $("#categoryName, #editCategoryName").on("input", (e) => {
      this.handleNameInput(e);
    });

    $(CONSTANTS.SELECTORS.EDIT_MODAL + " .btn-secondary")
      .off("click.editCancel")
      .on("click.editCancel", () => this.handleEditCancel());
  }

  handleCategoryFilterChange(event) {
    const $select = $(event.target);
    const selectedText = $select.find("option:selected").text();

    if (!$select.val()) {
      this.dataTable.column(2).search("").draw();
    } else {
      this.dataTable.column(2).search(`^${selectedText}$`, true, false).draw();
    }
  }

  handleNameInput(event) {
    const name = $(event.target).val();
    const metaUrlField =
      $(event.target).attr("id") === "categoryName"
        ? "#categoryMetaUrl"
        : "#editCategoryMetaUrl";

    if (name && !$(metaUrlField).val()) {
      $(metaUrlField).val(this.generateMetaUrl(name));
    }
  }

  handleEditCancel() {
    $(CONSTANTS.SELECTORS.EDIT_MODAL).modal("hide");
    if (this.currentEditingId) {
      setTimeout(() => this.viewCategory(this.currentEditingId), 400);
    }
  }

  resetFilters() {
    $(CONSTANTS.SELECTORS.SEARCH_KEYWORD).val("");

    if (typeof $.fn.select2 !== "undefined") {
      $(CONSTANTS.SELECTORS.STATUS_FILTER).val("").trigger("change.select2");
    } else {
      $(CONSTANTS.SELECTORS.STATUS_FILTER).val("");
    }

    this.dataTable.ajax.reload();
    this.showToast("info", CONSTANTS.MESSAGES.INFO, CONSTANTS.MESSAGES.RESET);
  }

  initializeDataTable() {
    this.dataTable = $(CONSTANTS.SELECTORS.CATEGORIES_TABLE).DataTable({
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
                      onclick="categoriesManager.viewCategory(${data})" title="Xem">
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
            const metaUrl = row.metaUrl || this.generateMetaUrl(row.name);
            return metaUrl
              ? `<div class="text-center"><code>${metaUrl}</code></div>`
              : '<div class="text-center text-muted">-</div>';
          },
        },
        {
          data: "status",
          width: "12%",
          render: (data) =>
            `<div class="text-center">${this.getStatusBadge(data)}</div>`,
        },
        {
          data: "createdAt",
          width: "15%",
          render: (data) =>
            `<div class="text-center">${
              data ? this.formatDate(data) : "-"
            }</div>`,
        },
        {
          data: "updatedAt",
          width: "15%",
          render: (data) =>
            `<div class="text-center">${
              data ? this.formatDate(data) : "-"
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

  async viewCategory(id) {
    try {
      const response = await CategoryApi.getById(id);
      const result = ApiUtils.handleResponse(
        response,
        null,
        "Không thể tải thông tin danh mục."
      );

      if (result.success) {
        this.displayCategoryDetails(result.data);
      }
    } catch (error) {
      ApiUtils.handleError(error, "Không thể kết nối đến máy chủ.");
    }
  }

  displayCategoryDetails(category) {
    const content = `
      <div id="categoryDetailContainer">
        <div class="card mb-4">
          <div class="card-header d-flex align-items-center bg-inverse bg-opacity-10 fw-400">
            Thông tin
            <div class="ms-auto">
              <a href="javascript:void(0)" class="text-decoration-none text-danger fw-bold ms-2" 
                 onclick="categoriesManager.deleteCategory(${category.id})">
                Xóa <i class="fas fa-fw me-1 fa-trash"></i>
              </a>
              <a href="javascript:void(0)" class="text-decoration-none text-success fw-bold ms-2" 
                 onclick="categoriesManager.editCategory(${category.id})">
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
                  <td>${
                    category.metaUrl || this.generateMetaUrl(category.name)
                  }</td>
                </tr>
                <tr>
                  <td class="fw-bold">Trạng thái</td>
                  <td>${this.getStatusBadge(category.status)}</td>
                </tr>
                <tr>
                  <td class="fw-bold">Ngày tạo</td>
                  <td>${this.formatDate(category.createdAt)}</td>
                </tr>
                <tr>
                  <td class="fw-bold">Cập nhật lần cuối</td>
                  <td>${this.formatDate(category.updatedAt)}</td>
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

  async editCategory(id) {
    this.currentEditingId = id;
    try {
      $(CONSTANTS.SELECTORS.VIEW_MODAL).modal("hide");
      const response = await CategoryApi.getById(id);
      const result = ApiUtils.handleResponse(
        response,
        null,
        "Không thể tải thông tin danh mục."
      );

      if (result.success) {
        this.populateEditForm(result.data);
        $(CONSTANTS.SELECTORS.EDIT_MODAL).modal("show");
      }
    } catch (error) {
      ApiUtils.handleError(error, "Không thể kết nối đến máy chủ.");
    }
  }

  populateEditForm(category) {
    $("#editCategoryId").val(category.id);
    $("#editCategoryName").val(category.name);
    $("#editCategoryStatus").val(category.status || CONSTANTS.STATUS.INACTIVE);
    $("#editCategoryMetaUrl").val(category.metaUrl || "");
  }

  deleteCategory(id) {
    this.currentDeleteId = id;
    $(CONSTANTS.SELECTORS.DELETE_MODAL).modal("show");
  }

  confirmDelete() {
    if (!this.currentDeleteId) return;

    $.ajax({
      url: `/NewCategory/delete?id=${this.currentDeleteId}`,
      type: "DELETE",
      success: (response) => {
        if (response.result === 1) {
          this.showToast(
            "success",
            CONSTANTS.MESSAGES.SUCCESS,
            "Đã xóa danh mục thành công"
          );
          $(CONSTANTS.SELECTORS.DELETE_MODAL).modal("hide");
          $(CONSTANTS.SELECTORS.VIEW_MODAL).modal("hide");
          this.dataTable.ajax.reload();
        } else {
          this.showToast(
            "error",
            CONSTANTS.MESSAGES.ERROR,
            response.error?.message || "Không thể xóa danh mục"
          );
        }
      },
      error: () => {
        this.showToast(
          "error",
          CONSTANTS.MESSAGES.ERROR,
          "Không thể kết nối đến máy chủ"
        );
      },
    });
  }

  async saveCategory() {
    const formData = this.buildFormData(
      "#categoryName",
      "#categoryStatus",
      "#categoryMetaUrl"
    );

    if (!this.validateForm(formData)) return;

    try {
      const response = await CategoryApi.save(formData);
      const result = ApiUtils.handleResponse(
        response,
        "Đã lưu danh mục thành công.",
        "Không thể lưu danh mục."
      );

      if (result.success) {
        $(CONSTANTS.SELECTORS.ADD_MODAL).modal("hide");
        this.dataTable.ajax.reload();
        this.resetAddForm();
      }
    } catch (error) {
      ApiUtils.handleError(error, "Không thể kết nối đến máy chủ.");
    }
  }

  async updateCategory() {
    const formData = this.buildFormData(
      "#editCategoryName",
      "#editCategoryStatus",
      "#editCategoryMetaUrl",
      "#editCategoryId"
    );

    if (!this.validateForm(formData)) return;

    try {
      const response = await CategoryApi.update(formData);
      const result = ApiUtils.handleResponse(
        response,
        "Đã cập nhật danh mục thành công.",
        "Không thể cập nhật danh mục."
      );

      if (result.success) {
        $(CONSTANTS.SELECTORS.EDIT_MODAL).modal("hide");
        this.dataTable.ajax.reload();
        this.resetEditForm();

        if (this.currentEditingId) {
          setTimeout(() => this.viewCategory(this.currentEditingId), 400);
        }
      }
    } catch (error) {
      ApiUtils.handleError(error, "Không thể kết nối đến máy chủ.");
    }
  }

  buildFormData(
    nameSelector,
    statusSelector,
    metaUrlSelector,
    idSelector = null
  ) {
    const formData = {
      name: $(nameSelector).val(),
      status: parseInt($(statusSelector).val()) || CONSTANTS.STATUS.INACTIVE,
      metaUrl:
        $(metaUrlSelector).val() || this.generateMetaUrl($(nameSelector).val()),
    };

    if (idSelector) {
      formData.id = parseInt($(idSelector).val());
    }

    return formData;
  }

  validateForm(formData) {
    if (!formData.name?.trim()) {
      this.showToast(
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
      this.showToast(
        "error",
        CONSTANTS.MESSAGES.ERROR,
        "Vui lòng chọn trạng thái."
      );
      return false;
    }

    return true;
  }

  resetAddForm() {
    $("#addCategoryForm")[0].reset();
    this.resetSelect2("#categoryStatus");
  }

  resetEditForm() {
    $("#editCategoryForm")[0].reset();
    this.resetSelect2("#editCategoryStatus");
    $("#editCategoryMetaUrl").val("");
  }

  resetSelect2(selector) {
    if (typeof $.fn.select2 !== "undefined") {
      $(selector).val("").trigger("change.select2");
    } else {
      $(selector).val("");
    }
  }

  formatDate(dateString) {
    if (!dateString) return "";
    const date = new Date(dateString);
    return date.toLocaleDateString("vi-VN");
  }

  generateMetaUrl(name) {
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

  getStatusBadge(status) {
    switch (status) {
      case CONSTANTS.STATUS.INACTIVE:
        return '<span class="badge bg-danger">Không hoạt động</span>';
      case CONSTANTS.STATUS.ACTIVE:
        return '<span class="badge bg-success">Hoạt động</span>';
      default:
        return '<span class="badge bg-secondary">Không xác định</span>';
    }
  }

  showToast(type, title, message) {
    if (typeof iziToast !== "undefined") {
      iziToast[type]({
        title,
        message,
        position: "topRight",
        timeout: 5000,
      });
    } else {
      alert(`${title}: ${message}`);
    }
  }

  refreshTable() {
    this.dataTable.ajax.reload();
    this.showToast(
      "info",
      CONSTANTS.MESSAGES.INFO,
      "Dữ liệu đã được cập nhật."
    );
  }
}

function showEditForm() {
  $(CONSTANTS.SELECTORS.VIEW_PANEL).hide();
  $(CONSTANTS.SELECTORS.EDIT_PANEL).show();
}

function hideEditForm() {
  $(CONSTANTS.SELECTORS.EDIT_PANEL).hide();
  $(CONSTANTS.SELECTORS.VIEW_PANEL).show();
}

let categoriesManager;

$(document).ready(() => {
  categoriesManager = new CategoriesManager();
});

$(document).on("submit", "#div_edit_panel #editCategoryForm", (e) => {
  e.preventDefault();
  hideEditForm();
});

$(document).on("click", "#div_edit_panel .btn-secondary", () => {
  hideEditForm();
});

$(document).on("click", "#btnApplyBulkStatus", applyBulkStatus);

async function applyBulkStatus() {
  const statusValue = $("#bulkStatusSelect").val();
  if (!statusValue) {
    showToast("warning", "Cảnh báo", "Vui lòng chọn trạng thái cần cập nhật.");
    return;
  }

  console.log(categoriesManager.dataTable);

  if (!categoriesManager.dataTable.select) {
    console.warn("DataTables Select plugin not available; cannot read selected rows.");
    showToast("error", "Lỗi", "Plugin chọn nhiều dòng chưa được tải.");
    return;
  }

  const api = categoriesManager.dataTable;
  const selected = api.rows({ selected: true });
  const selectedData = selected.data().toArray();

  if (selectedData.length === 0) {
    showToast("warning", "Cảnh báo", "Vui lòng chọn ít nhất một bản ghi.");
    return;
  }

  const confirmText = statusValue === "1" ? "Mở khóa" : "Khóa";
  const result = await Swal.fire({
    icon: "question",
    title: `Xác nhận ${confirmText}`,
    text: `Bạn có chắc chắn muốn ${confirmText.toLowerCase()} ${selectedData.length} tin tức đã chọn?`,
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
    showToast(
      "success",
      "Thành công",
      `Đã cập nhật ${successCount} bản ghi${errorCount ? `, lỗi: ${errorCount}` : ""}.`
    );
    api.rows({ selected: true }).deselect();
    api.ajax.reload(null, false);
  } else {
    showToast("error", "Thất bại", "Không thể cập nhật bản ghi nào.");
  }
}

window.viewCategory = (id) => categoriesManager.viewCategory(id);
window.editCategory = (id) => categoriesManager.editCategory(id);
window.deleteCategory = (id) => categoriesManager.deleteCategory(id);
window.confirmDelete = () => categoriesManager.confirmDelete();
window.refreshTable = () => categoriesManager.refreshTable();
window.saveCategory = () => categoriesManager.saveCategory();
window.updateCategory = () => categoriesManager.updateCategory();
