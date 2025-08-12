# Quản lý Tin tức với Dashmin Template

## Tổng quan

Đã tích hợp template Dashmin để hiển thị danh sách tin tức với giao diện admin hiện đại và chuyên nghiệp.

## Các thay đổi đã thực hiện

### 1. Layout mới

- Tạo file `_LayoutDashmin.cshtml` dựa trên template Dashmin
- Sử dụng sidebar navigation với menu quản lý
- Header với search bar và user dropdown
- Footer với thông tin copyright

### 2. Cập nhật News Index

- Sử dụng layout Dashmin: `Layout = "_LayoutDashmin"`
- Thiết kế card-based layout với `bg-light rounded h-100 p-4`
- Table với styling `table-striped table-hover` và `thead-dark`
- Sample data để demo giao diện

### 3. Modal Forms

- **Add Form**: Modal xl với header màu primary
- **Edit Form**: Modal xl với header màu warning
- **View Modal**: Modal xl với header màu info
- Tất cả đều có icons và styling nhất quán

### 4. Styling Features

- **Badges**: Sử dụng Bootstrap badges cho status và categories
- **Buttons**: Icon buttons với tooltips
- **Responsive**: Table responsive và mobile-friendly
- **Icons**: Font Awesome icons cho tất cả actions

## Cấu trúc Template

```
Views/Shared/
├── _LayoutDashmin.cshtml          # Layout chính cho admin
└── _Layout.cshtml                 # Layout gốc (giữ nguyên)

Views/New/
├── Index.cshtml                   # Trang danh sách tin tức
├── P_AddForm.cshtml              # Modal thêm tin tức
├── P_EditForm.cshtml             # Modal sửa tin tức
├── P_ViewModal.cshtml            # Modal xem chi tiết
└── README.md                     # Tài liệu này
```

## Assets được sử dụng

### CSS Files

- `~/template/dashmin-1.0.0/css/bootstrap.min.css`
- `~/template/dashmin-1.0.0/css/style.css`

### JavaScript Files

- `~/template/dashmin-1.0.0/js/main.js`
- `~/template/dashmin-1.0.0/lib/chart/chart.min.js`

### Libraries

- Font Awesome 5.10.0
- Bootstrap Icons 1.4.1
- Google Fonts (Heebo)

## Tính năng chính

### 1. Navigation

- Sidebar với menu quản lý
- Active state cho trang hiện tại
- Collapsible sidebar

### 2. Data Display

- Responsive table với sorting
- Image thumbnails
- Status badges
- Action buttons với tooltips

### 3. Search & Filter

- Keyword search
- Category filter với Select2
- Date range picker
- Reset functionality

### 4. Modal Forms

- Large modals (xl) cho better UX
- Color-coded headers
- Form validation
- File upload support

## Responsive Design

- **Desktop**: Full sidebar và content area
- **Tablet**: Collapsible sidebar
- **Mobile**: Stacked layout với mobile-friendly buttons

## Browser Support

- Chrome/Edge (recommended)
- Firefox
- Safari
- Mobile browsers

## Performance

- Lazy loading cho images
- Minified CSS/JS files
- Optimized Bootstrap components
- Efficient DOM manipulation

## Customization

### Colors

- Primary: Bootstrap primary blue
- Success: Green for published status
- Warning: Orange for pending status
- Danger: Red for hot news
- Info: Blue for view counts

### Icons

- Plus: Add new news
- Edit: Edit existing news
- Eye: View details
- Trash: Delete news
- Fire: Hot news indicator
- Search: Search functionality

## Next Steps

1. **API Integration**: Connect với backend APIs
2. **Real Data**: Replace sample data với real data
3. **Pagination**: Implement server-side pagination
4. **Export**: Add export functionality (PDF, Excel)
5. **Bulk Actions**: Multi-select và bulk operations
6. **Advanced Filters**: More filter options
7. **Real-time Updates**: WebSocket cho live updates

## Troubleshooting

### Common Issues

1. **Layout not loading**: Check file paths và CSS/JS references
2. **Icons not showing**: Verify Font Awesome CDN
3. **Modal not working**: Check Bootstrap JS inclusion
4. **Responsive issues**: Test trên different screen sizes

### Debug Tips

- Check browser console cho JavaScript errors
- Verify all asset files are accessible
- Test với different browsers
- Validate HTML structure

## Credits

- **Template**: Dashmin Bootstrap Admin Template
- **Icons**: Font Awesome
- **Framework**: Bootstrap 5
- **Editor**: CKEditor
- **Tables**: DataTables
- **Date Picker**: Bootstrap Datepicker
