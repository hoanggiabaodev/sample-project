let newsDataTable;
let newsEditor;
let editNewsEditor;

$(document).ready(function () {
    initializeComponents();
    loadCategories();
    initializeDataTable();
    bindEvents();
});

function initializeComponents() {
    initializeSelect2();
    initializeDatepicker();
    initializeCKEditor('newsContent', 'newsEditor');
}

function initializeSelect2() {
    $('#categoryFilter').select2({
        placeholder: "Chọn danh mục",
        allowClear: true,
        language: 'vi'
    });

    $('#addNewsModal, #editNewsModal').on('shown.bs.modal', function () {
        $(this).find('.select2').select2({
            dropdownParent: $(this),
            placeholder: "Chọn một tùy chọn",
            allowClear: true,
            language: 'vi'
        });
    });
}

function initializeDatepicker() {
    $('.datepicker').datepicker({
        format: 'dd/mm/yyyy',
        language: 'vi',
        autoclose: true,
        todayHighlight: true
    });
}

function initializeCKEditor(elementId, editorVariable) {
    try {
        if (CKEDITOR.instances[elementId]) {
            CKEDITOR.instances[elementId].destroy();
        }
        suppressCKEditorWarnings();
        window[editorVariable] = CKEDITOR.replace(elementId, {
            height: 300,
            language: 'vi',
            filebrowserUploadUrl: '/upload-image',
            removePlugins: 'elementspath,resize',
            toolbar: [
                { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', '-', 'RemoveFormat'] },
                { name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'] },
                { name: 'links', items: ['Link', 'Unlink'] },
                { name: 'insert', items: ['Image', 'Table', 'HorizontalRule', 'SpecialChar'] },
                { name: 'styles', items: ['Format', 'Font', 'FontSize'] },
                { name: 'colors', items: ['TextColor', 'BGColor'] },
                { name: 'tools', items: ['Maximize'] }
            ],
            removeButtons: 'Save,NewPage,Preview,Print,Templates,Find,Replace,SelectAll,SpellChecker,Scayt,Subscript,Superscript,CreateDiv,BidiLtr,BidiRtl,Anchor,Flash,PageBreak,Iframe,Styles,ShowBlocks',
            startupMode: 'wysiwyg',
            autoUpdateElement: false
        });
    } catch (error) {
        console.error('Error initializing CKEditor:', error);
        $('#' + elementId).show();
    }
}

function suppressCKEditorWarnings() {
    if (window.console && window.console.warn) {
        const originalWarn = window.console.warn;
        window.console.warn = function (message) {
            if (message && typeof message === 'string' &&
                (message.includes('CKEditor') && message.includes('license') ||
                    message.includes('CKEditor') && message.includes('not secure') ||
                    message.includes('This CKEditor') && message.includes('version is not secure') ||
                    message.includes('4.22.1') && message.includes('not secure') ||
                    message.includes('license key is missing') ||
                    message.includes('LTS version'))) {
                return;
            }
            originalWarn.apply(window.console, arguments);
        };
    }
}

function bindEvents() {
    $('#btnSearch').click(function () {
        newsDataTable.ajax.reload();
        showToast('success', 'Đang tìm kiếm...', 'Vui lòng chờ trong giây lát.');
    });

    $('#btnReset').click(function () {
        resetFilters();
    });

    $('#btnSaveNews').click(saveNews);
    $('#btnUpdateNews').click(updateNews);

    $('#addNewsModal').on('hidden.bs.modal', resetAddForm);
    $('#editNewsModal').on('hidden.bs.modal', resetEditForm);
    $('#addNewsModal').on('shown.bs.modal', function () {
        setTimeout(function () {
            initializeCKEditor('newsContent', 'newsEditor');
        }, 100);
    });
    $('#editNewsModal').on('shown.bs.modal', function () {
        setTimeout(function () {
            if (CKEDITOR.instances['editNewsContent']) {
                CKEDITOR.instances['editNewsContent'].destroy();
            }
            initializeCKEditor('editNewsContent', 'editNewsEditor');
        }, 100);
    });
}

function resetFilters() {
    $('#searchKeyword').val('');
    $('#categoryFilter').val('').trigger('change.select2');
    $('#statusFilter').val('');
    $('#dateFrom').val('');
    $('#dateTo').val('');
    newsDataTable.ajax.reload();
    showToast('info', 'Đã làm mới', 'Đã xóa tất cả bộ lọc.');
}

async function loadCategories() {
    try {
        const response = await NewsApi.getCategories();
        const result = ApiUtils.handleResponse(response, null, 'Không thể tải danh mục tin tức');

        if (result.success) {
            populateCategoryDropdowns(result.data);
        }
    } catch (error) {
        ApiUtils.handleError(error, 'Không thể kết nối đến máy chủ để tải danh mục.');
    }
}

function populateCategoryDropdowns(categories) {
    $('#categoryFilter, #newsCategory, #editNewsCategory').empty();

    $('#categoryFilter').append('<option value="">Tất cả danh mục</option>');
    $('#newsCategory, #editNewsCategory').append('<option value="">Chọn danh mục</option>');

    categories.forEach(function (category) {
        $('#categoryFilter').append(`<option value="${category.id}">${category.name}</option>`);
        $('#newsCategory, #editNewsCategory').append(`<option value="${category.id}">${category.name}</option>`);
    });

    $('#categoryFilter').trigger('change.select2');
}

function initializeDataTable() {
    newsDataTable = $('#newsDataTable').DataTable({
        processing: true,
        serverSide: false,
        ajax: {
            url: '/New/GetListByStatus',
            type: 'GET',
            data: function (d) {
                var statusFilter = $('#statusFilter').val();
                return {
                    page: (d.start / d.length) + 1,
                    record: d.length,
                    keyword: $('#searchKeyword').val(),
                    newsCategoryId: $('#categoryFilter').val(),
                    status: statusFilter === '' ? null : parseInt(statusFilter),
                    dateFrom: $('#dateFrom').val(),
                    dateTo: $('#dateTo').val()
                };
            },
            dataSrc: function (json) {
                if (json.result === 1 && json.data) {
                    return json.data;
                }
                return [];
            }
        },
        columns: [
            { data: 'id', width: '5%' },
            {
                data: 'imageObj',
                width: '10%',
                render: function (data, type, row) {
                    if (data && data.relativeUrl) {
                        return `<img src="${data.relativeUrl}" class="img-thumbnail" style="width: 60px; height: 40px; object-fit: cover;">`;
                    }
                    return '<div class="bg-light text-center" style="width: 60px; height: 40px; line-height: 40px;"><i class="fas fa-image text-muted"></i></div>';
                }
            },
            {
                data: 'name',
                width: '25%',
                render: function (data, type, row) {
                    let hotBadge = row.isHot ? '<span class="badge bg-danger ms-2">Hot</span>' : '';
                    return `<div>
                        <strong>${data}</strong>${hotBadge}
                        <br><small class="text-muted">${row.description || 'Không có mô tả'}</small>
                    </div>`;
                }
            },
            {
                data: 'newsCategoryObj',
                width: '12%',
                render: function (data, type, row) {
                    return data ? `<span class="badge bg-primary">${data.name}</span>` : '<span class="text-muted">-</span>';
                }
            },
            {
                data: 'publishedAt',
                width: '12%',
                render: function (data, type, row) {
                    return data ? formatDate(data) : '-';
                }
            },
            {
                data: 'viewNumber',
                width: '8%',
                render: function (data, type, row) {
                    return `<i class="fas fa-eye"></i> ${data || 0}`;
                }
            },
            {
                data: 'status',
                width: '8%',
                render: function (data, type, row) {
                    switch (data) {
                        case 0:
                            return `<span class="badge bg-danger">Không hoạt động</span>`;
                        case 1:
                            return `<span class="badge bg-success">Hoạt động</span>`;
                        default:
                            return `<span class="badge bg-secondary">Không xác định</span>`;
                    }
                }
            },
            {
                data: 'id',
                width: '20%',
                render: function (data, type, row) {
                    return `<div class="btn-group" role="group">
                        <button type="button" class="btn btn-sm btn-outline-primary" onclick="viewNews(${data})">
                            <i class="fas fa-eye"></i>
                        </button>
                        <button type="button" class="btn btn-sm btn-outline-warning" onclick="editNews(${data})">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button type="button" class="btn btn-sm btn-outline-danger" onclick="deleteNews(${data})">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>`;
                }
            }
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
                last: "Cuối"
            }
        },
        responsive: true,
        pageLength: 10,
        lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "Tất cả"]],
        order: [[0, 'desc']]
    });
}

async function viewNews(id) {
    try {
        const response = await NewsApi.getById(id);
        const result = ApiUtils.handleResponse(response, null, 'Không thể tải thông tin tin tức.');

        if (result.success) {
            displayNewsDetails(result.data);
        }
    } catch (error) {
        ApiUtils.handleError(error, 'Không thể kết nối đến máy chủ.');
    }
}

function displayNewsDetails(news) {
    let content = `
        <div class="row">
            <div class="col-md-4">
                ${news.imageObj?.relativeUrl
            ? `<img src="${news.imageObj.relativeUrl}" class="img-fluid rounded" alt="${news.name}">`
            : '<div class="bg-light text-center p-4"><i class="fas fa-image fa-3x text-muted"></i></div>'
        }
            </div>
            <div class="col-md-8">
                <h4>${news.name}</h4>
                <p class="text-muted">${news.description || 'Không có mô tả'}</p>
                <div class="mb-3">
                    <strong>Danh mục:</strong> ${news.newsCategoryObj?.name || 'Không có'}
                </div>
                <div class="mb-3">
                    <strong>Ngày đăng:</strong> ${formatDate(news.publishedAt)}
                </div>
                <div class="mb-3">
                    <strong>Lượt xem:</strong> ${news.viewNumber || 0}
                </div>
                <div class="mb-3">
                    <strong>Trạng thái:</strong> ${getStatusBadge(news.status)}
                </div>
                <div class="mb-3">
                    <strong>Tin nổi bật:</strong> ${news.isHot ? '<span class="badge bg-danger">Hot</span>' : '<span class="badge bg-secondary">Bình thường</span>'}
                </div>
            </div>
        </div>
        <hr>
        <div class="mt-3">
            <h5>Nội dung chi tiết:</h5>
            <div class="border p-3 bg-light">
                ${news.detail || 'Không có nội dung chi tiết'}
            </div>
        </div>
    `;
    $('#viewNewsContent').html(content);
    $('#viewNewsModal').modal('show');
}

async function editNews(id) {
    try {
        const response = await NewsApi.getById(id);
        const result = ApiUtils.handleResponse(response, null, 'Không thể tải thông tin tin tức.');

        if (result.success) {
            populateEditForm(result.data);
            $('#editNewsModal').modal('show');
        }
    } catch (error) {
        ApiUtils.handleError(error, 'Không thể kết nối đến máy chủ.');
    }
}

function populateEditForm(news) {
    $('#editNewsId').val(news.id);
    $('#editNewsTitle').val(news.name);
    $('#editNewsCategory').val(news.newsCategoryId).trigger('change.select2');
    $('#editNewsDescription').val(news.description);
    $('#editNewsPublishedDate').val(formatDate(news.publishedAt));
    $('#editNewsIsHot').prop('checked', news.isHot);
    $('#editNewsStatus').val(news.status || 0);

    $('#editNewsMetaKeywords').val(news.metaKeywords || "");
    $('#editNewsMetaDescription').val(news.metaDescription || news.description || "");
    $('#editNewsMetaTitle').val(news.metaTitle || news.name || "");
    $('#editNewsMetaImagePreview').val(news.metaImagePreview || "");
    $('#editNewsImageId').val(news.imageId || 0);

    if (news.imageObj?.relativeUrl) {
        $('#currentImagePreview').html(`<img src="${news.imageObj.relativeUrl}" class="img-thumbnail" style="max-width: 200px;">`);
    } else {
        $('#currentImagePreview').html('<p class="text-muted">Không có hình ảnh</p>');
    }

    // Set CKEditor content with retry mechanism
    let retryCount = 0;
    const maxRetries = 5;

    function setCKEditorContent() {
        try {
            if (editNewsEditor && editNewsEditor.setData) {
                editNewsEditor.setData(news.detail || '');
                console.log('CKEditor data set successfully');
            } else {
                console.log('CKEditor not ready, retrying...', retryCount);
                if (retryCount < maxRetries) {
                    retryCount++;
                    setTimeout(setCKEditorContent, 300);
                } else {
                    console.log('CKEditor not available, setting textarea value');
                    $('#editNewsContent').val(news.detail || '');
                }
            }
        } catch (error) {
            console.error('Error setting CKEditor data:', error);
            $('#editNewsContent').val(news.detail || '');
        }
    }

    // Start the retry mechanism
    setTimeout(setCKEditorContent, 100);
}

function deleteNews(id) {
    iziToast.question({
        timeout: 20000,
        close: false,
        overlay: true,
        displayMode: 'once',
        id: 'question',
        zindex: 999,
        title: 'Xác nhận',
        message: 'Bạn có chắc chắn muốn xóa tin tức này?',
        position: 'center',
        buttons: [
            ['<button><b>Đồng ý</b></button>', async function (instance, toast) {
                try {
                    const response = await NewsApi.delete(id);
                    const result = ApiUtils.handleResponse(response, 'Đã xóa tin tức thành công.', 'Không thể xóa tin tức.');

                    if (result.success) {
                        newsDataTable.ajax.reload();
                    }
                } catch (error) {
                    ApiUtils.handleError(error, 'Không thể kết nối đến máy chủ.');
                }
                instance.hide({ transitionOut: 'fadeOut' }, toast, 'button');
            }, true],
            ['<button>Hủy</button>', function (instance, toast) {
                instance.hide({ transitionOut: 'fadeOut' }, toast, 'button');
            }]
        ]
    });
}

async function saveNews() {
    const formData = buildAddFormData();

    if (!validateNewsForm(formData)) {
        return;
    }

    try {
        const response = await NewsApi.save(formData);
        const result = ApiUtils.handleResponse(response, 'Đã lưu tin tức thành công.', 'Không thể lưu tin tức.');

        if (result.success) {
            $('#addNewsModal').modal('hide');
            newsDataTable.ajax.reload();
            resetAddForm();
        }
    } catch (error) {
        ApiUtils.handleError(error, 'Không thể kết nối đến máy chủ.');
    }
}

async function updateNews() {
    const formData = buildEditFormData();

    if (!validateNewsForm(formData)) {
        return;
    }

    try {
        const response = await NewsApi.update(formData);
        const result = ApiUtils.handleResponse(response, 'Đã cập nhật tin tức thành công.', 'Không thể cập nhật tin tức.');

        if (result.success) {
            $('#editNewsModal').modal('hide');
            newsDataTable.ajax.reload();
            resetEditForm();
        }
    } catch (error) {
        ApiUtils.handleError(error, 'Không thể kết nối đến máy chủ.');
    }
}

function buildAddFormData() {
    let detail = '';
    try {
        detail = newsEditor && newsEditor.getData ? newsEditor.getData() : $('#newsContent').val();
    } catch (error) {
        console.error('Error getting CKEditor data:', error);
        detail = $('#newsContent').val();
    }

    let publishedDate = $('#newsPublishedDate').val();
    let parsedDate = new Date();

    if (publishedDate) {
        const dateParts = publishedDate.split('/');
        if (dateParts.length === 3) {
            const day = parseInt(dateParts[0]);
            const month = parseInt(dateParts[1]) - 1;
            const year = parseInt(dateParts[2]);
            parsedDate = new Date(year, month, day);
        }
    }

    const formData = {
        name: $('#newsTitle').val(),
        newsCategoryId: parseInt($('#newsCategory').val()),
        description: $('#newsDescription').val(),
        detail: detail,
        publishedAt: parsedDate.toISOString(),
        isHot: $('#newsIsHot').is(':checked'),
        metaUrl: generateMetaUrl($('#newsTitle').val()),
        metaKeywords: "",
        metaDescription: $('#newsDescription').val() || "",
        metaTitle: $('#newsTitle').val() || "",
        metaImagePreview: "",
        imageId: 0,
        status: parseInt($('#newsStatus').val()) || 0
    };

    return formData;
}

function buildEditFormData() {
    let detail = '';
    try {
        detail = editNewsEditor && editNewsEditor.getData ? editNewsEditor.getData() : $('#editNewsContent').val();
    } catch (error) {
        console.error('Error getting CKEditor data:', error);
        detail = $('#editNewsContent').val();
    }

    let publishedDate = $('#editNewsPublishedDate').val();
    let parsedDate = new Date();

    if (publishedDate) {
        const dateParts = publishedDate.split('/');
        if (dateParts.length === 3) {
            const day = parseInt(dateParts[0]);
            const month = parseInt(dateParts[1]) - 1;
            const year = parseInt(dateParts[2]);
            parsedDate = new Date(year, month, day);
        }
    }

    const editId = parseInt($('#editNewsId').val());
    console.log('Edit ID found:', editId);

    const formData = {
        id: editId,
        name: $('#editNewsTitle').val(),
        newsCategoryId: parseInt($('#editNewsCategory').val()),
        description: $('#editNewsDescription').val(),
        detail: detail,
        publishedAt: parsedDate.toISOString(),
        isHot: $('#editNewsIsHot').is(':checked'),
        metaUrl: generateMetaUrl($('#editNewsTitle').val()),
        metaKeywords: $('#editNewsMetaKeywords').val() || "",
        metaDescription: $('#editNewsMetaDescription').val() || $('#editNewsDescription').val() || "",
        metaTitle: $('#editNewsMetaTitle').val() || $('#editNewsTitle').val() || "",
        metaImagePreview: $('#editNewsMetaImagePreview').val() || "",
        imageId: parseInt($('#editNewsImageId').val()) || 0,
        status: parseInt($('#editNewsStatus').val()) || 0
    };

    return formData;
}

function validateNewsForm(formData) {
    if (!formData.name || formData.name.trim() === '') {
        showToast('error', 'Lỗi', 'Vui lòng nhập tiêu đề tin tức.');
        return false;
    }

    if (!formData.newsCategoryId || formData.newsCategoryId === 0) {
        showToast('error', 'Lỗi', 'Vui lòng chọn danh mục.');
        return false;
    }

    if (formData.status === undefined || formData.status === null || formData.status === '') {
        showToast('error', 'Lỗi', 'Vui lòng chọn trạng thái.');
        return false;
    }

    return true;
}

function resetAddForm() {
    $('#newsForm')[0].reset();
    $('#newsCategory').val('').trigger('change.select2');
    $('#newsStatus').val('');

    try {
        if (newsEditor && newsEditor.setData) {
            newsEditor.setData('');
        }
    } catch (error) {
        console.error('Error resetting CKEditor:', error);
        $('#newsContent').val('');
    }
}

function resetEditForm() {
    $('#editNewsForm')[0].reset();
    $('#editNewsCategory').val('').trigger('change.select2');
    $('#editNewsStatus').val('');

    $('#editNewsMetaKeywords').val('');
    $('#editNewsMetaDescription').val('');
    $('#editNewsMetaTitle').val('');
    $('#editNewsMetaImagePreview').val('');
    $('#editNewsImageId').val('');

    $('#currentImagePreview').empty();

    try {
        if (editNewsEditor && editNewsEditor.setData) {
            editNewsEditor.setData('');
        }
    } catch (error) {
        console.error('Error resetting CKEditor:', error);
        $('#editNewsContent').val('');
    }
}

function formatDate(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN');
}

function generateMetaUrl(title) {
    if (!title) return '';
    return title
        .toLowerCase()
        .replace(/[àáạảãâầấậẩẫăằắặẳẵ]/g, 'a')
        .replace(/[èéẹẻẽêềếệểễ]/g, 'e')
        .replace(/[ìíịỉĩ]/g, 'i')
        .replace(/[òóọỏõôồốộổỗơờớợởỡ]/g, 'o')
        .replace(/[ùúụủũưừứựửữ]/g, 'u')
        .replace(/[ỳýỵỷỹ]/g, 'y')
        .replace(/đ/g, 'd')
        .replace(/[^a-z0-9\s-]/g, '')
        .replace(/\s+/g, '-')
        .replace(/-+/g, '-')
        .trim('-');
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
    iziToast[type]({
        title: title,
        message: message,
        position: 'topRight',
        timeout: 5000
    });
} 