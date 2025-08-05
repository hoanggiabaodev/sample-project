# Module Quản lý Tin tức (New)

## Cấu trúc thư mục

```
Views/New/
├── Index.cshtml          # Trang chính quản lý tin tức
├── P_AddForm.cshtml      # Partial view form thêm tin tức
├── P_EditForm.cshtml     # Partial view form chỉnh sửa tin tức
├── P_ViewModal.cshtml    # Partial view modal xem chi tiết
├── ViewDetail.cshtml     # Trang xem chi tiết tin tức
└── README.md            # File hướng dẫn này
```

## Các file JavaScript

```
wwwroot/js/
└── news-management.js    # File JavaScript chính cho quản lý tin tức
```

## Mô tả các file

### 1. Index.cshtml

- **Chức năng**: Trang chính hiển thị danh sách tin tức với DataTable
- **Tính năng**:
  - Tìm kiếm và lọc tin tức
  - Hiển thị danh sách với phân trang
  - Nút thêm tin tức mới
  - Các nút thao tác (xem, sửa, xóa)

### 2. P_AddForm.cshtml

- **Chức năng**: Modal form thêm tin tức mới
- **Tính năng**:
  - Form nhập thông tin tin tức
  - Tích hợp CKEditor cho nội dung
  - Select2 cho danh mục
  - DatePicker cho ngày đăng
  - Upload hình ảnh

### 3. P_EditForm.cshtml

- **Chức năng**: Modal form chỉnh sửa tin tức
- **Tính năng**:
  - Tương tự form thêm nhưng có thêm hidden field cho ID
  - Hiển thị hình ảnh hiện tại
  - Tự động điền dữ liệu khi mở modal

### 4. P_ViewModal.cshtml

- **Chức năng**: Modal xem chi tiết tin tức
- **Tính năng**:
  - Hiển thị đầy đủ thông tin tin tức
  - Layout responsive
  - Chỉ xem, không chỉnh sửa

### 5. news-management.js

- **Chức năng**: File JavaScript chính
- **Tính năng**:
  - Khởi tạo các component (DataTable, Select2, CKEditor, DatePicker)
  - Xử lý các sự kiện (thêm, sửa, xóa, tìm kiếm)
  - Gọi API và xử lý response
  - Hiển thị thông báo (IziToast)

## API Endpoints

### Controller: NewController

1. **GET /New/GetCategories**

   - Lấy danh sách danh mục tin tức
   - Response: `{ result: 1, data: [categories] }`

2. **GET /New/GetListByStatus**

   - Lấy danh sách tin tức theo trạng thái
   - Parameters: `status` (default: 1)
   - Response: `{ result: 1, data: [news], dataCount: number }`

3. **GET /New/GetById/{id}**

   - Lấy chi tiết tin tức theo ID
   - Response: `{ result: 1, data: news }`

4. **POST /New/Save**

   - Lưu hoặc cập nhật tin tức
   - Body: News object
   - Response: `{ result: 1, message: "success" }`

5. **DELETE /New/Delete/{id}**
   - Xóa tin tức theo ID
   - Response: `{ result: 1, message: "success" }`

## Cách sử dụng

### 1. Thêm tin tức mới

```javascript
// Mở modal thêm tin tức
$("#addNewsModal").modal("show");

// Hoặc click nút "Thêm tin tức mới"
```

### 2. Chỉnh sửa tin tức

```javascript
// Gọi function editNews với ID
editNews(newsId);
```

### 3. Xem chi tiết tin tức

```javascript
// Gọi function viewNews với ID
viewNews(newsId);
```

### 4. Xóa tin tức

```javascript
// Gọi function deleteNews với ID
deleteNews(newsId);
```

## Dependencies

### CSS Libraries

- Bootstrap 5
- DataTables Bootstrap 4
- Select2
- Bootstrap DatePicker
- IziToast

### JavaScript Libraries

- jQuery
- DataTables
- Select2
- Bootstrap DatePicker
- CKEditor
- IziToast

## Lưu ý

1. **CKEditor**: Có xử lý suppress security warning
2. **Select2**: Hỗ trợ tiếng Việt
3. **DatePicker**: Format dd/mm/yyyy
4. **DataTable**: Responsive và có phân trang
5. **API**: Có fallback mock data khi API fail

## Troubleshooting

### 1. Categories không hiển thị

- Kiểm tra console log
- Kiểm tra API endpoint `/New/GetCategories`
- Kiểm tra network tab trong browser

### 2. CKEditor không load

- Kiểm tra đường dẫn file CKEditor
- Kiểm tra console error
- Có fallback về textarea

### 3. DataTable không hiển thị dữ liệu

- Kiểm tra API endpoint `/New/GetListByStatus`
- Kiểm tra response format
- Kiểm tra console log

### 4. Modal không mở

- Kiểm tra Bootstrap JS
- Kiểm tra ID của modal
- Kiểm tra console error
