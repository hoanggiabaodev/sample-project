function getMetaUrlFromPath() {
    var path = window.location.pathname;
    var parts = path.split('/');
    if (parts.length > 2 && parts[1] === 'tin-tuc') {
        return parts[2];
    }
    return null;
}

function loadCategories() {
    if (typeof $ === 'undefined') {
        console.error('jQuery is not loaded yet');
        return;
    }

    $.ajax({
        url: '/NewCategory/GetListByStatus',
        type: 'GET',
        data: {
            keyword: '',
            status: 1
        },
        dataType: 'json',
        success: function (response) {
            console.log('Categories response:', response);

            $('#category-list').empty();

            if (response && response.result === 1 && response.data && Array.isArray(response.data)) {
                var limitedCategories = response.data.slice(0, 3);
                var gradientColors = [
                    'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                    'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
                    'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)'
                ];

                limitedCategories.forEach(function (category, index) {
                    var gradient = gradientColors[index] || gradientColors[0];
                    var categoryHtml = '<a href="/tin-tuc/danh-muc/' + category.id + '" ' +
                        'style="display: inline-block; background: ' + gradient + '; ' +
                        'color: white; text-decoration: none; padding: 12px 24px; ' +
                        'border-radius: 25px; font-size: 14px; font-weight: 600; ' +
                        'margin: 0 10px 12px 0; text-transform: uppercase; ' +
                        'letter-spacing: 0.5px; box-shadow: 0 4px 15px rgba(0,0,0,0.15); ' +
                        'transition: all 0.3s ease; border: none;" ' +
                        'onmouseover="this.style.transform=\'translateY(-3px)\'; this.style.boxShadow=\'0 6px 20px rgba(0,0,0,0.2)\';" ' +
                        'onmouseout="this.style.transform=\'translateY(0)\'; this.style.boxShadow=\'0 4px 15px rgba(0,0,0,0.15)\';">' +
                        category.name + '</a>';
                    $('#category-list').append(categoryHtml);
                });

                console.log('Loaded ' + limitedCategories.length + ' categories (limited to 3)');
            } else {
                console.log('No categories found or invalid response structure');
                $('#category-list').html('<p class="text-muted">Không có danh mục nào</p>');
            }
        },
        error: function (error) {
            console.log('Error loading categories:', error);
            $('#category-list').html('<p class="text-danger">Lỗi tải danh mục</p>');
        }
    });
}

function waitForJQuery() {
    if (typeof $ !== 'undefined') {
        $(document).ready(function () {
            loadCategories();
            loadFeaturedNews();
            loadNewsList();
            mostviewed();
            const currentMetaUrl = getMetaUrlFromPath();
            loadRelatedNews(currentMetaUrl);
            var metaUrl = getMetaUrlFromPath();
            if (metaUrl) {
                loadNewsDetail(metaUrl);
            }
        });
    } else {
        setTimeout(waitForJQuery, 100);
    }
}

function formatDate(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN');
}

function loadNewsDetail(metaUrl) {
    if (typeof $ === 'undefined') {
        console.error('jQuery is not loaded yet');
        return;
    }

    $.ajax({
        url: '/New/GetDetail',
        type: 'GET',
        data: { metaUrl: metaUrl }, // truyền metaUrl thay vì id
        dataType: 'json',
        success: function (response) {
            console.log('News detail response:', response);
            $('#news-detail-container').empty();
            if (response && response.result === 1 && response.data) {
                var news = response.data;
                var html = `
                    <div class="news-detail">
                        <h1>${news.name || ''}</h1>
                        <div class="meta">
                            <span>Ngày đăng: ${formatDate(news.publishedAt)}</span>
                            <span style="margin-left: 16px;">Lượt xem: ${news.viewNumber || 0}</span>
                        </div>
                        <div class="description" style="margin: 16px 0; color: #555; font-size: 16px;">${news.description || ''}</div>
                        <div class="content">${news.content || ''}</div>
                    </div>
                `;
                $('#news-detail-container').html(html);
            } else {
                $('#news-detail-container').html('<p class="text-muted">Không tìm thấy tin tức</p>');
            }
        },
        error: function (error) {
            console.log('Error loading news detail:', error);
            $('#news-detail-container').html('<p class="text-danger">Lỗi tải chi tiết tin tức</p>');
        }
    });
}

function loadFeaturedNews() {
    if (typeof $ === 'undefined') {
        console.error('jQuery is not loaded yet');
        return;
    }

    $.ajax({
        url: '/New/GetFeaturedNews',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            console.log('Featured news response:', response);

            $('#featured-news-container').empty();

            if (response && response.result === 1 && Array.isArray(response.data)) {
                var limitedNews = response.data.slice(0, 3);
                var badgeColors = ['#0d6efd', '#198754', '#ffc107'];

                limitedNews.forEach(function (news, index) {
                    var badgeColor = badgeColors[index] || '#6c757d';
                    var imageUrl = '/images/demo_images.webp';

                    var newsHtml = `
                        <div style="display: flex; align-items: center; background: #fff; border-radius: 8px; 
                                    box-shadow: 0 2px 8px rgba(0,0,0,0.05); padding: 8px; margin-bottom: 10px;
                                    transition: transform 0.2s ease, box-shadow 0.2s ease;"
                             onmouseover="this.style.transform='translateY(-2px)'; this.style.boxShadow='0 4px 12px rgba(0,0,0,0.08)';"
                             onmouseout="this.style.transform='none'; this.style.boxShadow='0 2px 8px rgba(0,0,0,0.05)';">
                            
                            <div style="flex-shrink: 0;">
                                <img src="${imageUrl}" alt="${news.name}" 
                                     style="width: 70px; height: 50px; object-fit: cover; border-radius: 6px;">
                            </div>

                            <div style="flex-grow: 1; padding-left: 10px;">
                                <span style="display: inline-block; font-size: 11px; font-weight: 500; 
                                             text-transform: uppercase; letter-spacing: 0.5px; color: white; 
                                             background: ${badgeColor}; border-radius: 4px; padding: 2px 6px;">
                                    ${news.newsCategoryObj?.name || 'Tin tức'}
                                </span>
                                <h6 style="font-size: 13px; font-weight: 600; line-height: 1.3; margin: 4px 0 0;">
                                    <a href="/tin-tuc/${news.metaUrl}" 
                                       style="text-decoration: none; color: #333; transition: color 0.2s;"
                                       onmouseover="this.style.color='#0d6efd'" 
                                       onmouseout="this.style.color='#333'">
                                        ${news.name}
                                    </a>
                                </h6>
                            </div>
                        </div>
                    `;

                    $('#featured-news-container').append(newsHtml);
                });

                console.log(`Loaded ${limitedNews.length} featured news items`);
            } else {
                $('#featured-news-container').html('<div class="text-center text-muted" style="font-size: 14px;">Không có tin tức nổi bật nào</div>');
            }
        },
        error: function (error) {
            console.error('Error loading featured news:', error);
            $('#featured-news-container').html('<div class="text-center text-danger" style="font-size: 14px;">Lỗi tải tin tức nổi bật</div>');
        }
    });
}


function loadNewsList() {
    if (typeof $ === 'undefined') {
        console.error('jQuery is not loaded yet');
        return;
    }

    $.ajax({
        url: '/New/GetListByStatus',
        type: 'GET',
        data: {
            page: 1,
            record: 50,  // Lấy nhiều bản ghi hơn
            keyword: '',
            newsCategoryId: null,
            status: 1,  // Chỉ lấy tin tức active
            dateFrom: '',
            dateTo: ''
        },
        dataType: 'json',
        success: function (response) {
            console.log('News list response:', response);

            $('#news-list').empty();

            if (response && response.result === 1 && response.data && Array.isArray(response.data)) {
                response.data.forEach(function (news) {
                    var imageUrl = '/images/demo_images.webp';
                    var categoryName = (news.newsCategoryObj && news.newsCategoryObj.name) ? news.newsCategoryObj.name : 'Tin tức';
                    var hotBadge = news.isHot ? '<span class="badge bg-danger ms-2" style="font-size: 10px;">Hot</span>' : '';
                    var publishedDate = news.publishedAt ? formatDate(news.publishedAt) : '';
                    var viewCount = news.viewNumber || 0;

                    var newsHtml = '<div class="single-what-news mb-4" style="border: 1px solid #e9ecef; border-radius: 12px; overflow: hidden; background: #fff; box-shadow: 0 2px 8px rgba(0,0,0,0.08); transition: all 0.3s ease;" onmouseover="this.style.transform=\'translateY(-2px)\'; this.style.boxShadow=\'0 4px 15px rgba(0,0,0,0.12)\';" onmouseout="this.style.transform=\'translateY(0)\'; this.style.boxShadow=\'0 2px 8px rgba(0,0,0,0.08)\';">' +
                        '<div class="what-img" style="position: relative; overflow: hidden; height: 250px;">' +
                        '<img src="' + imageUrl + '" alt="' + (news.name || 'Tin tức') + '" style="width: 100%; height: 100%; object-fit: cover; transition: transform 0.3s ease;" onmouseover="this.style.transform=\'scale(1.05)\';" onmouseout="this.style.transform=\'scale(1)\';">' +
                        '<div style="position: absolute; bottom: 15px; left: 15px; background: linear-gradient(135deg, #007bff, #0056b3); color: white; padding: 8px 16px; border-radius: 25px; font-size: 11px; font-weight: 700; text-transform: uppercase; letter-spacing: 0.8px; box-shadow: 0 4px 12px rgba(0,123,255,0.3); backdrop-filter: blur(10px);">' + categoryName + '</div>' +
                        '</div>' +
                        '<div class="what-cap" style="padding: 25px 20px 20px 20px;">' +
                        '<h4 style="margin: 0 0 12px 0; font-size: 18px; font-weight: 700; line-height: 1.4;"><a href="/tin-tuc/' + (news.metaUrl || news.id) + '" style="text-decoration: none; color: #2c3e50; transition: color 0.2s ease;" onmouseover="this.style.color=\'#007bff\';" onmouseout="this.style.color=\'#2c3e50\';">' + (news.name || 'Tin tức') + '</a>' + hotBadge + '</h4>' +
                        '<p style="color: #6c757d; font-size: 14px; line-height: 1.6; margin: 0 0 15px 0; display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden;">' + (news.description || news.summary || '') + '</p>' +
                        '<div class="whats-date" style="display: flex; justify-content: space-between; align-items: center; padding-top: 15px; border-top: 1px solid #f1f3f4; font-size: 13px; color: #6c757d;">' +
                        '<span style="display: flex; align-items: center;"><i class="fas fa-calendar" style="color: #007bff; margin-right: 8px;"></i>' + publishedDate + '</span>' +
                        '<span style="display: flex; align-items: center;"><i class="fas fa-eye" style="color: #007bff; margin-right: 8px;"></i>' + viewCount + ' lượt xem</span>' +
                        '</div>' +
                        '</div>' +
                        '</div>';
                    $('#news-list').append(newsHtml);
                });
                console.log('Loaded ' + response.data.length + ' news items (full data)');
            } else {
                console.log('Không có tin tức nào hoặc cấu trúc phản hồi không hợp lệ');
                $('#news-list').html('<p class="text-muted">Không có tin tức nào</p>');
            }
        },
        error: function (xhr, status, error) {
            console.log('Error loading news list:', error);
            console.log('XHR status:', status);
            console.log('XHR response:', xhr.responseText);
            $('#news-list').html('<p class="text-danger">Lỗi tải tin tức</p>');
        }
    });
}

function mostviewed() {
    if (typeof $ === 'undefined') {
        console.error('jQuery is not loaded yet');
        return;
    }

    $.ajax({
        url: '/Home/GetMostViewed',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            console.log('Most viewed response:', response);

            if (response && response.result === 1 && Array.isArray(response.data)) {
                const container = $('.most-recent-area');
                container.find('.most-recent').remove(); // Xóa cũ

                const top3 = response.data.slice(0, 3);
                const imageUrl = '/images/demo_images.webp'; // dùng chung ảnh hoặc lấy từ news.image nếu có

                top3.forEach((news, index) => {
                    const rank = String(index + 1).padStart(2, '0');
                    const date = formatDate(news.publishedAt);
                    const html = `
                        <div class="most-recent mb-40">
                            <div class="most-recent-img">
                                <img src="${imageUrl}" alt="${news.name}">
                                <div class="most-recent-cap">
                                    <span class="bgbeg">${rank}</span>
                                    <h4><a href="/tin-tuc/${news.metaUrl}">${news.name}</a></h4>
                                    <p>${date}</p>
                                </div>
                            </div>
                        </div>`;
                    container.append(html);
                });
            } else {
                console.log('Không có dữ liệu tin tức xem nhiều');
            }
        },
        error: function (error) {
            console.log('Error loading most viewed news:', error);
        }
    });
}

function loadRelatedNews(metaUrl) {
    $.ajax({
        url: '/Home/GetRelatedNews',
        type: 'GET',
        data: { metaUrl: metaUrl },
        success: function (res) {
            if (res.result === 1) {
                let html = '';
                res.data.forEach(item => {
                    html += `
                        <div class="col-md-6 col-lg-4">
                            <div class="card h-100 related-card" style="border-radius: 16px; overflow: hidden; border: none; box-shadow: 0 2px 12px rgba(0,0,0,0.07); transition: box-shadow 0.3s, transform 0.3s;">
                                <a href="/tin-tuc/${item.metaUrl}" class="related-img-link" style="display:block; overflow:hidden;">
                                    <img src="/images/demo_images.webp" class="card-img-top related-img" alt="${item.name}" style="height:170px; object-fit:cover; border-radius: 16px 16px 0 0; transition: transform 0.3s;">
                                </a>
                                <div class="card-body" style="padding: 16px 14px 12px 14px;">
                                    <h5 class="card-title mb-0" style="font-size:1.08rem; font-weight:600; line-height:1.4;">
                                        <a href="/tin-tuc/${item.metaUrl}" class="related-title" style="text-decoration:none; color:#222; transition:color 0.2s;">${item.name}</a>
                                    </h5>
                                </div>
                            </div>
                        </div>`;
                });
                // Thêm CSS custom chỉ 1 lần
                if (!document.getElementById('related-news-custom-style')) {
                    const style = document.createElement('style');
                    style.id = 'related-news-custom-style';
                    style.innerHTML = `
                        .related-card:hover {
                            box-shadow: 0 8px 32px rgba(0,0,0,0.13);
                            transform: translateY(-4px) scale(1.025);
                        }
                        .related-img-link:hover .related-img {
                            transform: scale(1.07);
                        }
                        .related-title:hover {
                            color: #007bff !important;
                        }
                        .related-card {
                            background: #fff;
                        }
                    `;
                    document.head.appendChild(style);
                }
                $('#related-news-container').html(html);
            } else {
                console.log('No related news found or API result != 1');
            }
        },
        error: function (err) {
            console.log('Error when calling /Home/GetRelatedNews:', err);
        }
    });
}

waitForJQuery();