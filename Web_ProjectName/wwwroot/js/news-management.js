// News Management JavaScript
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
    // Initialize Select2
    $('.select2').select2({
        placeholder: "Chọn một tùy chọn",
        allowClear: true,
        language: 'vi'
    });

    // Initialize DatePicker
    $('.datepicker').datepicker({
        format: 'dd/mm/yyyy',
        language: 'vi',
        autoclose: true,
        todayHighlight: true
    });

    // Initialize CKEditor for add form
    initializeCKEditor('newsContent', 'newsEditor');
}

function initializeCKEditor(elementId, editorVariable) {
    try {
        if (CKEDITOR.instances[elementId]) {
            CKEDITOR.instances[elementId].destroy();
        }

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
                    return; // Suppress CKEditor warnings
                }
                originalWarn.apply(window.console, arguments);
            };
        }

        window[editorVariable] = CKEDITOR.replace(elementId, {
            height: 300,
            language: 'vi',
            filebrowserUploadUrl: '/upload-image',
            // Disable license check
            removePlugins: 'elementspath,resize',
            // Use basic toolbar to avoid license issues
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

function bindEvents() {
    $('#btnSearch').click(function () {
        newsDataTable.ajax.reload();
        showToast('success', 'Đang tìm kiếm...', 'Vui lòng chờ trong giây lát.');
    });

    $('#btnReset').click(function () {
        $('#searchKeyword').val('');
        $('#categoryFilter').val('').trigger('change');
        $('#dateFrom').val('');
        $('#dateTo').val('');
        newsDataTable.ajax.reload();
        showToast('info', 'Đã làm mới', 'Đã xóa tất cả bộ lọc.');
    });

    $('#btnSaveNews').click(function () {
        console.log('Save button clicked');
        saveNews();
    });

    $('#btnUpdateNews').click(function () {
        updateNews();
    });

    $('#addNewsModal').on('hidden.bs.modal', function () {
        resetAddForm();
    });

    $('#editNewsModal').on('hidden.bs.modal', function () {
        resetEditForm();
    });

    $('#addNewsModal').on('shown.bs.modal', function () {
        initializeCKEditor('newsContent', 'newsEditor');
    });

    $('#editNewsModal').on('shown.bs.modal', function () {
        initializeCKEditor('editNewsContent', 'editNewsEditor');
    });
}

function loadCategories() {
    console.log('Loading categories...');
    $.ajax({
        url: '/New/GetCategories',
        type: 'GET',
        success: function (response) {
            console.log('Categories API response:', response);
            if (response.result === 1 && response.data) {
                $('#categoryFilter, #newsCategory, #editNewsCategory').empty();

                $('#categoryFilter').append('<option value="">Tất cả danh mục</option>');
                $('#newsCategory, #editNewsCategory').append('<option value="">Chọn danh mục</option>');

                response.data.forEach(function (category) {
                    console.log('Adding category:', category);
                    $('#categoryFilter').append(`<option value="${category.id}">${category.name}</option>`);
                    $('#newsCategory, #editNewsCategory').append(`<option value="${category.id}">${category.name}</option>`);
                });

                $('#categoryFilter, #newsCategory, #editNewsCategory').trigger('change');

                console.log('Categories loaded successfully:', response.data.length, 'categories');
            } else {
                console.error('Failed to load categories:', response.error);
                showToast('error', 'Lỗi', 'Không thể tải danh mục tin tức: ' + (response.error || 'Unknown error'));
            }
        },
        error: function (xhr, status, error) {
            console.error('Error loading categories:', error);
            console.error('XHR status:', xhr.status);
            console.error('XHR response:', xhr.responseText);
            showToast('error', 'Lỗi', 'Không thể kết nối đến máy chủ để tải danh mục.');
        }
    });
}

function initializeDataTable() {
    newsDataTable = $('#newsDataTable').DataTable({
        processing: true,
        serverSide: false,
        ajax: {
            url: '/New/GetListByStatus',
            type: 'GET',
            data: function (d) {
                return {
                    page: (d.start / d.length) + 1,
                    record: d.length,
                    keyword: $('#searchKeyword').val(),
                    newsCategoryId: $('#categoryFilter').val(),
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
                data: 'id',
                width: '8%',
                render: function (data, type, row) {
                    return `<span class="badge bg-success">Hoạt động</span>`;
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

// News CRUD Functions
function viewNews(id) {
    $.ajax({
        url: `/New/GetById/${id}`,
        type: 'GET',
        success: function (response) {
            if (response.result === 1 && response.data) {
                const news = response.data;
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
                                <strong>Trạng thái:</strong> ${news.isHot ? '<span class="badge bg-danger">Hot</span>' : '<span class="badge bg-secondary">Bình thường</span>'}
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
            } else {
                showToast('error', 'Lỗi', 'Không thể tải thông tin tin tức.');
            }
        },
        error: function () {
            showToast('error', 'Lỗi', 'Không thể kết nối đến máy chủ.');
        }
    });
}

function editNews(id) {
    $.ajax({
        url: `/New/GetById/${id}`,
        type: 'GET',
        success: function (response) {
            if (response.result === 1 && response.data) {
                const news = response.data;
                $('#editNewsId').val(news.id);
                $('#editNewsTitle').val(news.name);
                $('#editNewsCategory').val(news.newsCategoryId).trigger('change');
                $('#editNewsDescription').val(news.description);
                $('#editNewsPublishedDate').val(formatDate(news.publishedAt));
                $('#editNewsIsHot').prop('checked', news.isHot);

                // Populate meta fields
                $('#editNewsMetaKeywords').val(news.metaKeywords || "");
                $('#editNewsMetaDescription').val(news.metaDescription || news.description || "");
                $('#editNewsMetaTitle').val(news.metaTitle || news.name || "");
                $('#editNewsMetaImagePreview').val(news.metaImagePreview || "");
                $('#editNewsImageId').val(news.imageId || 0);

                // Show current image if exists
                if (news.imageObj?.relativeUrl) {
                    $('#currentImagePreview').html(`<img src="${news.imageObj.relativeUrl}" class="img-thumbnail" style="max-width: 200px;">`);
                } else {
                    $('#currentImagePreview').html('<p class="text-muted">Không có hình ảnh</p>');
                }

                try {
                    if (editNewsEditor && editNewsEditor.setData) {
                        editNewsEditor.setData(news.detail || '');
                    }
                } catch (error) {
                    console.error('Error setting CKEditor data:', error);
                    $('#editNewsContent').val(news.detail || '');
                }

                $('#editNewsModal').modal('show');
            } else {
                showToast('error', 'Lỗi', 'Không thể tải thông tin tin tức.');
            }
        },
        error: function () {
            showToast('error', 'Lỗi', 'Không thể kết nối đến máy chủ.');
        }
    });
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
            ['<button><b>Đồng ý</b></button>', function (instance, toast) {
                $.ajax({
                    url: `/New/Delete/${id}`,
                    type: 'DELETE',
                    success: function (response) {
                        if (response.result === 1) {
                            showToast('success', 'Thành công', 'Đã xóa tin tức thành công.');
                            newsDataTable.ajax.reload();
                        } else {
                            showToast('error', 'Lỗi', response.error || 'Không thể xóa tin tức.');
                        }
                    },
                    error: function () {
                        showToast('error', 'Lỗi', 'Không thể kết nối đến máy chủ.');
                    }
                });
                instance.hide({ transitionOut: 'fadeOut' }, toast, 'button');
            }, true],
            ['<button>Hủy</button>', function (instance, toast) {
                instance.hide({ transitionOut: 'fadeOut' }, toast, 'button');
            }]
        ]
    });
}

function saveNews() {
    console.log('saveNews function called');
    let detail = '';
    try {
        detail = newsEditor && newsEditor.getData ? newsEditor.getData() : $('#newsContent').val();
    } catch (error) {
        console.error('Error getting CKEditor data:', error);
        detail = $('#newsContent').val();
    }

    // Parse date from dd/mm/yyyy to ISO format
    let publishedDate = $('#newsPublishedDate').val();
    let parsedDate = new Date();

    if (publishedDate) {
        // Parse dd/mm/yyyy format
        const dateParts = publishedDate.split('/');
        if (dateParts.length === 3) {
            const day = parseInt(dateParts[0]);
            const month = parseInt(dateParts[1]) - 1; // Month is 0-based
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
        status: 1
    };

    // Validate form
    if (!formData.name) {
        showToast('error', 'Lỗi', 'Vui lòng nhập tiêu đề tin tức.');
        return;
    }

    if (!formData.newsCategoryId) {
        showToast('error', 'Lỗi', 'Vui lòng chọn danh mục.');
        return;
    }

    // Save news
    $.ajax({
        url: '/New/Save',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify(formData),
        success: function (response) {
            if (response.result === 1) {
                showToast('success', 'Thành công', 'Đã lưu tin tức thành công.');
                $('#addNewsModal').modal('hide');
                newsDataTable.ajax.reload();
                resetAddForm();
            } else {
                showToast('error', 'Lỗi', response.error || 'Không thể lưu tin tức.');
            }
        },
        error: function (xhr, status, error) {
            console.error('Save error:', xhr.responseText);
            console.error('Status:', xhr.status);
            console.error('StatusText:', xhr.statusText);
            showToast('error', 'Lỗi', 'Không thể kết nối đến máy chủ.');
        }
    });
}

function updateNews() {
    console.log('updateNews function called');
    let detail = '';
    try {
        detail = editNewsEditor && editNewsEditor.getData ? editNewsEditor.getData() : $('#editNewsContent').val();
    } catch (error) {
        console.error('Error getting CKEditor data:', error);
        detail = $('#editNewsContent').val();
    }

    // Parse date from dd/mm/yyyy to ISO format
    let publishedDate = $('#editNewsPublishedDate').val();
    let parsedDate = new Date();

    if (publishedDate) {
        // Parse dd/mm/yyyy format
        const dateParts = publishedDate.split('/');
        if (dateParts.length === 3) {
            const day = parseInt(dateParts[0]);
            const month = parseInt(dateParts[1]) - 1; // Month is 0-based
            const year = parseInt(dateParts[2]);
            parsedDate = new Date(year, month, day);
        }
    }

    const formData = {
        id: parseInt($('#editNewsId').val()),
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
        status: 1
    };

    console.log(5133512165)
    // Validate form
    if (!formData.name) {
        showToast('error', 'Lỗi', 'Vui lòng nhập tiêu đề tin tức.');
        return;
    }

    if (!formData.newsCategoryId) {
        showToast('error', 'Lỗi', 'Vui lòng chọn danh mục.');
        return;
    }

    // Update news using the new Update endpoint
    $.ajax({
        url: '/New/Update',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify(formData),
        success: function (response) {
            if (response.result === 1) {
                showToast('success', 'Thành công', 'Đã cập nhật tin tức thành công.');
                $('#editNewsModal').modal('hide');
                newsDataTable.ajax.reload();
                resetEditForm();
            } else {
                showToast('error', 'Lỗi', response.error || 'Không thể cập nhật tin tức.');
            }
        },
        error: function (xhr, status, error) {
            console.error('Update error:', xhr.responseText);
            console.error('Status:', xhr.status);
            console.error('StatusText:', xhr.statusText);
            showToast('error', 'Lỗi', 'Không thể kết nối đến máy chủ.');
        }
    });
}

function resetAddForm() {
    $('#newsForm')[0].reset();
    $('#newsCategory').val('').trigger('change');
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
    $('#editNewsCategory').val('').trigger('change');

    // Reset hidden fields
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

// Utility Functions
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

function showToast(type, title, message) {
    iziToast[type]({
        title: title,
        message: message,
        position: 'topRight',
        timeout: 5000
    });
} 