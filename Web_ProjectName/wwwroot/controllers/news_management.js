let newsDataTable;
let newsEditor;
let editNewsEditor;

$(document).ready(function () {
  InitializeComponents();
  LoadCategories();
  InitializeDataTable();
  BindEvents();
});

function InitializeComponents() {
  InitializeSelect2();
  InitializeDatepicker();
  InitializeCKEditor("newsContent", "newsEditor");
}

function InitializeSelect2() {
  $("#category_filter").select2({
    placeholder: "Chọn danh mục",
    allowClear: true,
    language: "vi",
  });

  $("#addNewsModal, #editNewsModal").on("shown.bs.modal", function () {
    $(this)
      .find(".select2")
      .select2({
        dropdownParent: $(this),
        placeholder: "Chọn một tùy chọn",
        allowClear: true,
        language: "vi",
      });
  });
}

function InitializeDatepicker() {
  $(".datepicker").datepicker({
    format: "dd/mm/yyyy",
    language: "vi",
    autoclose: true,
    todayHighlight: true,
  });
}

function InitializeCKEditor(elementId, editorVariable) {
  try {
    if (CKEDITOR.instances[elementId]) {
      CKEDITOR.instances[elementId].destroy();
    }
    SuppressCKEditorWarnings();
    window[editorVariable] = CKEDITOR.replace(elementId, {
      height: 300,
      language: "vi",
      filebrowserUploadUrl: "/upload-image",
      removePlugins: "elementspath,resize",
      toolbar: [
        {
          name: "basicstyles",
          items: ["Bold", "Italic", "Underline", "Strike", "-", "RemoveFormat"],
        },
        {
          name: "paragraph",
          items: [
            "NumberedList",
            "BulletedList",
            "-",
            "Outdent",
            "Indent",
            "-",
            "Blockquote",
            "-",
            "JustifyLeft",
            "JustifyCenter",
            "JustifyRight",
            "JustifyBlock",
          ],
        },
        { name: "links", items: ["Link", "Unlink"] },
        {
          name: "insert",
          items: ["Image", "Table", "HorizontalRule", "SpecialChar"],
        },
        { name: "styles", items: ["Format", "Font", "FontSize"] },
        { name: "colors", items: ["TextColor", "BGColor"] },
        { name: "tools", items: ["Maximize"] },
      ],
      removeButtons:
        "Save,NewPage,Preview,Print,Templates,Find,Replace,SelectAll,SpellChecker,Scayt,Subscript,Superscript,CreateDiv,BidiLtr,BidiRtl,Anchor,Flash,PageBreak,Iframe,Styles,ShowBlocks",
      startupMode: "wysiwyg",
      autoUpdateElement: false,
    });
  } catch (error) {
    console.error("Error initializing CKEditor:", error);
    $("#" + elementId).show();
  }
}

function SuppressCKEditorWarnings() {
  if (window.console && window.console.warn) {
    const originalWarn = window.console.warn;
    window.console.warn = function (message) {
      if (
        message &&
        typeof message === "string" &&
        ((message.includes("CKEditor") && message.includes("license")) ||
          (message.includes("CKEditor") && message.includes("not secure")) ||
          (message.includes("This CKEditor") &&
            message.includes("version is not secure")) ||
          (message.includes("4.22.1") && message.includes("not secure")) ||
          message.includes("license key is missing") ||
          message.includes("LTS version"))
      ) {
        return;
      }
      originalWarn.apply(window.console, arguments);
    };
  }
}

function BindEvents() {
  $("#btn_search").click(function () {
    newsDataTable.ajax.reload();
    showToast("success", "Đang tìm kiếm...", "Vui lòng chờ trong giây lát.");
  });

  $("#btn_reset").click(function () {
    ResetFilters();
  });

  $("#btnSaveNews").click(SaveNews);
  $("#btnUpdateNews").click(UpdateNews);

  $(document).on("click", "#btn_apply_bulk_status", ApplyBulkStatus);

  $("#addNewsModal").on("hidden.bs.modal", ResetAddForm);
  $("#editNewsModal").on("hidden.bs.modal", ResetEditForm);
  $("#addNewsModal").on("shown.bs.modal", function () {
    setTimeout(function () {
      InitializeCKEditor("newsContent", "newsEditor");
    }, 100);
  });
  $("#editNewsModal").on("shown.bs.modal", function () {
    setTimeout(function () {
      if (CKEDITOR.instances["editNewsContent"]) {
        CKEDITOR.instances["editNewsContent"].destroy();
      }
      InitializeCKEditor("editNewsContent", "editNewsEditor");
    }, 100);
  });
}

async function ApplyBulkStatus() {
  const statusValue = $("#bulk_status_select").val();
  if (!statusValue) {
    showToast("warning", "Cảnh báo", "Vui lòng chọn trạng thái cần cập nhật.");
    return;
  }

  if (!newsDataTable.select) {
    showToast("error", "Lỗi", "Chưa dòng nào được chọn");
    return;
  }

  const api = newsDataTable;
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
    NewsApi.updateStatus(row.id, parseInt(statusValue))
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

function ResetFilters() {
  $("#searchKeyword").val("");
  $("#category_filter").val("").trigger("change.select2");
  $("#status_filter").val("");
  $("#dateFrom").val("");
  $("#dateTo").val("");
  newsDataTable.ajax.reload();
  showToast("info", "Đã làm mới", "Đã xóa tất cả bộ lọc.");
}

async function LoadCategories() {
  try {
    const response = await NewsApi.getCategories();
    const result = ApiUtils.handleResponse(
      response,
      null,
      "Không thể tải danh mục tin tức"
    );

    if (result.success) {
      PopulateCategoryDropdowns(result.data);
    }
  } catch (error) {
    ApiUtils.handleError(
      error,
      "Không thể kết nối đến máy chủ để tải danh mục."
    );
  }
}

function PopulateCategoryDropdowns(categories) {
  $("#category_filter, #newsCategory, #editNewsCategory").empty();

  $("#category_filter").append('<option value="">Tất cả danh mục</option>');
  $("#newsCategory, #editNewsCategory").append(
    '<option value="">Chọn danh mục</option>'
  );

  categories.forEach(function (category) {
    $("#category_filter").append(
      `<option value="${category.id}">${category.name}</option>`
    );
    $("#newsCategory, #editNewsCategory").append(
      `<option value="${category.id}">${category.name}</option>`
    );
  });

  $("#category_filter").trigger("change.select2");
}

function InitializeDataTable() {
  newsDataTable = $("#news_data_table").DataTable({
    processing: true,
    serverSide: false,
    select: {
      style: "multi",
      selector: 'td:not(:nth-child(1))',
    },
    ajax: {
      url: "/New/GetListByStatus",
      type: "GET",
      data: function (d) {
        var status_filter = $("#status_filter").val();
        return {
          page: d.start / d.length + 1,
          record: d.length,
          keyword: $("#searchKeyword").val(),
          newsCategoryId: $("#category_filter").val(),
          status: status_filter === "" ? null : parseInt(status_filter),
          dateFrom: $("#dateFrom").val(),
          dateTo: $("#dateTo").val(),
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
        render: (data) => `
                    <div class="d-flex justify-content-center align-items-center text-center" style="height: 100%;">
                        <button type="button" class="btn btn-sm btn-outline-primary" onclick="ViewNews(${data})" title="Xem chi tiết">
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
        data: "imageObj",
        width: "15%",
        render: function () {
          return `<div class="text-center">
                    <img src="images/demo_images.webp" class="img-thumbnail" style="width: 60px; height: 40px; object-fit: cover;">
                  </div>`;
        },
      },
      {
        data: "name",
        width: "25%",
        render: function (data, type, row) {
          let hotBadge = row.isHot
            ? '<span class="badge bg-danger ms-2"><i class="fas fa-fire"></i> Hot</span>'
            : "";
          return `<div class="text-center">
                        <strong>${data}</strong>${hotBadge}
                    </div>`;
        },
      },
      {
        data: "newsCategoryObj",
        width: "12%",
        render: function (data, type, row) {
          return data
            ? `<div class="text-center"><span class="badge bg-primary">${data.name}</span></div>`
            : '<div class="text-center text-muted">-</div>';
        },
      },
      {
        data: "publishedAt",
        width: "12%",
        render: function (data, type, row) {
          return `<div class="text-center">${
            data ? FormatDate(data) : "-"
          }</div>`;
        },
      },
      {
        data: "viewNumber",
        width: "8%",
        render: function (data, type, row) {
          return `<div class="text-center"><span class="badge bg-info"><i class="fas fa-eye"></i> ${
            data || 0
          }</span></div>`;
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
        data: "id",
        width: "15%",
        render: (data) => `
                    <div class="d-flex justify-content-center align-items-center gap-1">
                        <button type="button" class="btn btn-sm btn-outline-warning" onclick="EditNews(${data})" title="Chỉnh sửa">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button id="btnDeleteNew" type="button" class="btn btn-sm btn-outline-danger" onclick="DeleteNews(${data})" title="Xóa">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>`,
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

async function ViewNews(id) {
  try {
    const response = await NewsApi.getById(id);
    const result = ApiUtils.handleResponse(
      response,
      null,
      "Không thể tải thông tin tin tức."
    );

    if (result.success) {
      DisplayNewsDetails(result.data);
    }
  } catch (error) {
    ApiUtils.handleError(error, "Không thể kết nối đến máy chủ.");
  }
}

function DisplayNewsDetails(news) {
  let content = `
        <div class="row">
            <div class="mt-3 d-flex justify-content-end gap-2">
              <button id="btn-edit" type="button" class="btn btn-primary" onclick="EditNews(${
                news.id
              })">
                  <i class="fas fa-edit"></i> Chỉnh sửa
              </button>
              <button id="btnDeleteNew" type="button" class="btn btn-danger" onclick="DeleteNews(${
                news.id
              })">
                  <i class="fas fa-trash"></i> Xóa
              </button>
            </div>
            <div class="col-md-4">
                <img src="images/demo_images.webp" class="img-fluid rounded" alt="${
                  news.name
                }">
            </div>
            <div class="col-md-8">
                <h4>${news.name}</h4>
                <div class="mb-3">
                    <strong>Danh mục:</strong> ${
                      news.newsCategoryObj?.name || "Không có"
                    }
                </div>
                <div class="mb-3">
                    <strong>Ngày đăng:</strong> ${FormatDate(news.publishedAt)}
                </div>
                <div class="mb-3">
                    <strong>Lượt xem:</strong> ${news.viewNumber || 0}
                </div>
                <div class="mb-3">
                    <strong>Trạng thái:</strong> ${GetStatusBadge(news.status)}
                </div>
                <div class="mb-3">
                    <strong>Tin nổi bật:</strong> ${
                      news.isHot
                        ? '<span class="badge bg-danger">Hot</span>'
                        : '<span class="badge bg-secondary">Bình thường</span>'
                    }
                </div>
            </div>
        </div>
        <hr>
        <div class="mt-3">
            <h5>Nội dung chi tiết:</h5>
            <div class="border p-3 bg-light">
                ${news.detail || "Không có nội dung chi tiết"}
            </div>
        </div>
        <hr>
    `;
  $("#viewNewsContent").html(content);
  $("#viewNewsModal").modal("show");
}

async function EditNews(id) {
  try {
    $("#viewNewsModal").modal("hide");

    const response = await NewsApi.getById(id);
    const result = ApiUtils.handleResponse(
      response,
      null,
      "Không thể tải thông tin tin tức."
    );

    if (result.success) {
      PopulateEditForm(result.data);
      $("#editNewsModal").modal("show");
    }
  } catch (error) {
    ApiUtils.handleError(error, "Không thể kết nối đến máy chủ.");
  }
}

function PopulateEditForm(news) {
  $("#editNewsId").val(news.id);
  $("#editNewsTitle").val(news.name);
  $("#editNewsCategory").val(news.newsCategoryId).trigger("change.select2");
  $("#editNewsDescription").val(news.description);
  $("#editNewsPublishedDate").val(FormatDate(news.publishedAt));
  $("#editNewsIsHot").prop("checked", news.isHot);
  $("#editNewsStatus").val(news.status || 0);

  $("#editNewsMetaKeywords").val(news.metaKeywords || "");
  $("#editNewsMetaDescription").val(
    news.metaDescription || news.description || ""
  );
  $("#editNewsMetaTitle").val(news.metaTitle || news.name || "");
  $("#editNewsMetaImagePreview").val(news.metaImagePreview || "");
  $("#editNewsImageId").val(news.imageId || 0);

  if (news.imageObj?.relativeUrl) {
    $("#currentImagePreview").html(
      `<img src="${news.imageObj.relativeUrl}" class="img-thumbnail" style="max-width: 200px;">`
    );
  } else {
    $("#currentImagePreview").html(
      '<p class="text-muted">Không có hình ảnh</p>'
    );
  }

  let retryCount = 0;
  const maxRetries = 5;

  function setCKEditorContent() {
    try {
      if (editNewsEditor && editNewsEditor.setData) {
        editNewsEditor.setData(news.detail || "");
        console.log("CKEditor data set successfully");
      } else {
        console.log("CKEditor not ready, retrying...", retryCount);
        if (retryCount < maxRetries) {
          retryCount++;
          setTimeout(setCKEditorContent, 300);
        } else {
          console.log("CKEditor not available, setting textarea value");
          $("#editNewsContent").val(news.detail || "");
        }
      }
    } catch (error) {
      console.error("Error setting CKEditor data:", error);
      $("#editNewsContent").val(news.detail || "");
    }
  }

  setTimeout(setCKEditorContent, 100);
}

function DeleteNews(id) {
  Swal.fire({
    icon: 'warning',
    title: 'Bạn chắc không?',
    text: 'Bạn có thực sự muốn xóa mục này? Không thể khôi phục sau khi xóa?',
    showCancelButton: true,
    confirmButtonColor: '#d33',
    cancelButtonColor: '#6c757d',
    confirmButtonText: 'Xóa',
    cancelButtonText: 'Hủy',
    reverseButtons: true,
    customClass: {
      confirmButton: 'btn btn-danger',
      cancelButton: 'btn btn-secondary'
    },
    buttonsStyling: false
  }).then((result) => {
    if (result.isConfirmed) {
      PerformDelete(id);
    }
  });
}

async function PerformDelete(id) {
  try {
    const response = await NewsApi.delete(id);
    const result = ApiUtils.handleResponse(
      response,
      "Đã xóa tin tức thành công.",
      "Không thể xóa tin tức."
    );

    if (result.success) {
      $("#viewNewsModal").modal("hide");
      newsDataTable.ajax.reload();
      
      Swal.fire({
        icon: 'success',
        title: 'Thành công!',
        text: 'Tin tức đã được xóa thành công.',
        timer: 2000,
        showConfirmButton: false
      });
    }
  } catch (error) {
    ApiUtils.handleError(error, "Không thể kết nối đến máy chủ.");
  }
}

$("#category_filter").change((e) =>
  HandleCategoryFilterChange(e)
);

function HandleCategoryFilterChange(event) {
  const $select = $(event.target);
  const selectedText = $select.find("option:selected").text();

  if (!$select.val()) {
    newsDataTable.column(4).search("").draw();
  } else {
    newsDataTable.column(4).search(`^${selectedText}$`, true, false).draw();
  }
}

async function SaveNews() {
  const formData = BuildAddFormData();

  if (!ValidateNewsForm(formData)) {
    return;
  }

  try {
    const response = await NewsApi.save(formData);
    const result = ApiUtils.handleResponse(
      response,
      "Đã lưu tin tức thành công.",
      "Không thể lưu tin tức."
    );

    if (result.success) {
      $("#addNewsModal").modal("hide");
      newsDataTable.ajax.reload();
      ResetAddForm();
    }
  } catch (error) {
    ApiUtils.handleError(error, "Không thể kết nối đến máy chủ.");
  }
}

async function UpdateNews() {
  const formData = BuildEditFormData();

  if (!ValidateNewsForm(formData)) {
    return;
  }

  try {
    const response = await NewsApi.update(formData);
    const result = ApiUtils.handleResponse(
      response,
      "Đã cập nhật tin tức thành công.",
      "Không thể cập nhật tin tức."
    );

    if (result.success) {
      $("#editNewsModal").modal("hide");

      newsDataTable.ajax.reload();

      ResetEditForm();

      try {
        const updatedNewsResponse = await NewsApi.getById(formData.id);
        const updatedNewsResult = ApiUtils.handleResponse(
          updatedNewsResponse,
          null,
          "Không thể tải thông tin tin tức đã cập nhật."
        );

        if (updatedNewsResult.success) {
          DisplayNewsDetails(updatedNewsResult.data);
        }
      } catch (error) {
        console.error("Error loading updated news details:", error);
        showToast(
          "success",
          "Cập nhật thành công",
          "Tin tức đã được cập nhật thành công."
        );
      }
    }
  } catch (error) {
    ApiUtils.handleError(error, "Không thể kết nối đến máy chủ.");
  }
}

function BuildAddFormData() {
  let detail = "";
  try {
    detail =
      newsEditor && newsEditor.getData
        ? newsEditor.getData()
        : $("#newsContent").val();
  } catch (error) {
    console.error("Error getting CKEditor data:", error);
    detail = $("#newsContent").val();
  }

  let publishedDate = $("#newsPublishedDate").val();
  let parsedDate = new Date();

  if (publishedDate) {
    const dateParts = publishedDate.split("/");
    if (dateParts.length === 3) {
      const day = parseInt(dateParts[0]);
      const month = parseInt(dateParts[1]) - 1;
      const year = parseInt(dateParts[2]);
      parsedDate = new Date(year, month, day);
    }
  }

  const formData = {
    name: $("#newsTitle").val(),
    newsCategoryId: parseInt($("#newsCategory").val()),
    description: $("#newsDescription").val(),
    detail: detail,
    publishedAt: parsedDate.toISOString(),
    isHot: $("#newsIsHot").is(":checked"),
    metaUrl: GenerateMetaUrl($("#newsTitle").val()),
    metaKeywords: "",
    metaDescription: $("#newsDescription").val() || "",
    metaTitle: $("#newsTitle").val() || "",
    metaImagePreview: "",
    imageId: 0,
    status: parseInt($("#newsStatus").val()) || 0,
  };

  return formData;
}

function BuildEditFormData() {
  let detail = "";
  try {
    detail =
      editNewsEditor && editNewsEditor.getData
        ? editNewsEditor.getData()
        : $("#editNewsContent").val();
  } catch (error) {
    console.error("Error getting CKEditor data:", error);
    detail = $("#editNewsContent").val();
  }

  let publishedDate = $("#editNewsPublishedDate").val();
  let parsedDate = new Date();

  if (publishedDate) {
    const dateParts = publishedDate.split("/");
    if (dateParts.length === 3) {
      const day = parseInt(dateParts[0]);
      const month = parseInt(dateParts[1]) - 1;
      const year = parseInt(dateParts[2]);
      parsedDate = new Date(year, month, day);
    }
  }

  const editId = parseInt($("#editNewsId").val());
  console.log("Edit ID found:", editId);

  const formData = {
    id: editId,
    name: $("#editNewsTitle").val(),
    newsCategoryId: parseInt($("#editNewsCategory").val()),
    description: $("#editNewsDescription").val(),
    detail: detail,
    publishedAt: parsedDate.toISOString(),
    isHot: $("#editNewsIsHot").is(":checked"),
    metaUrl: GenerateMetaUrl($("#editNewsTitle").val()),
    metaKeywords: $("#editNewsMetaKeywords").val() || "",
    metaDescription:
      $("#editNewsMetaDescription").val() ||
      $("#editNewsDescription").val() ||
      "",
    metaTitle: $("#editNewsMetaTitle").val() || $("#editNewsTitle").val() || "",
    metaImagePreview: $("#editNewsMetaImagePreview").val() || "",
    imageId: parseInt($("#editNewsImageId").val()) || 0,
    status: parseInt($("#editNewsStatus").val()) || 0,
  };

  return formData;
}

function ValidateNewsForm(formData) {
  if (!formData.name || formData.name.trim() === "") {
    showToast("error", "Lỗi", "Vui lòng nhập tiêu đề tin tức.");
    return false;
  }

  if (!formData.newsCategoryId || formData.newsCategoryId === 0) {
    showToast("error", "Lỗi", "Vui lòng chọn danh mục.");
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

function ResetAddForm() {
  $("#newsForm")[0].reset();
  $("#newsCategory").val("").trigger("change.select2");
  $("#newsStatus").val("");

  try {
    if (newsEditor && newsEditor.setData) {
      newsEditor.setData("");
    }
  } catch (error) {
    console.error("Error resetting CKEditor:", error);
    $("#newsContent").val("");
  }
}

function ResetEditForm() {
  $("#editNewsForm")[0].reset();
  $("#editNewsCategory").val("").trigger("change.select2");
  $("#editNewsStatus").val("");

  $("#editNewsMetaKeywords").val("");
  $("#editNewsMetaDescription").val("");
  $("#editNewsMetaTitle").val("");
  $("#editNewsMetaImagePreview").val("");
  $("#editNewsImageId").val("");

  $("#currentImagePreview").empty();

  try {
    if (editNewsEditor && editNewsEditor.setData) {
      editNewsEditor.setData("");
    }
  } catch (error) {
    console.error("Error resetting CKEditor:", error);
    $("#editNewsContent").val("");
  }
}

function FormatDate(dateString) {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toLocaleDateString("vi-VN");
}

function GenerateMetaUrl(title) {
  if (!title) return "";
  return title
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

function GetStatusBadge(status) {
  switch (status) {
    case 0:
      return '<span class="badge bg-danger">Không hoạt động</span>';
    case 1:
      return '<span class="badge bg-success">Hoạt động</span>';
    default:
      return '<span class="badge bg-secondary">Không xác định</span>';
  }
}