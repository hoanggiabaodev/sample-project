let clickedCategories = [];

function GetMetaUrlFromPath() {
    var path = window.location.pathname;
    var parts = path.split("/");
    if (parts.length > 2 && parts[1] === "tin-tuc") {
        return parts[2];
    }
    return null;
}

function FormatDate(dateString) {
    if (!dateString) return "";
    const date = new Date(dateString);
    return date.toLocaleDateString("vi-VN");
}

function WaitForJQuery() {
    if (typeof $ !== "undefined") {
        $(document).ready(function () {
            LoadCategories(3, "Nhiều hơn");
            LoadFeaturedNews();
            LoadMostViewed();
            const currentMetaUrl = GetMetaUrlFromPath();
            LoadRelatedNews(currentMetaUrl);
            var metaUrl = GetMetaUrlFromPath();
            if (metaUrl) {
                LoadNewsDetail(metaUrl);
            }

            BindEvents();
        });
    } else {
        setTimeout(WaitForJQuery, 100);
    }
}

function LoadCategories(quantity, mess) {
    if (typeof window.$ === "undefined") {
        console.error("jQuery is not loaded yet");
        return;
    }

    window.$.ajax({
        url: "/NewCategory/GetListByStatus",
        type: "GET",
        data: {
            keyword: "",
            status: 1,
        },
        dataType: "json",
        success: (response) => {
            console.log("Categories response:", response);

            window.$("#category-list").fadeOut(300, function () {
                window.$("#category-list").empty();

                if (
                    response &&
                    response.result === 1 &&
                    response.data &&
                    Array.isArray(response.data)
                ) {
                    var limitedCategories;
                    if (quantity) {
                        limitedCategories = response.data.slice(0, quantity);
                    } else {
                        limitedCategories = response.data;
                    }

                    limitedCategories.forEach((category, index) => {
                        const isActive = clickedCategories.some(cat => cat.id == category.id);
                        const activeClass = isActive ? 'active' : '';

                        var categoryHtml =
                            '<a href="/tin-tuc/danh-muc/' +
                            category.id +
                            '" class="category-pill ' + activeClass + '" style="opacity: 0; transform: translateY(20px);">' +
                            category.name +
                            "</a>";
                        window.$("#category-list").append(categoryHtml);
                    });

                    if (quantity) {
                        window.$("#category-list").append(`
                    <button id="btnMore" class="category-action-btn" style="opacity: 0; transform: translateY(20px);">
                        <img src="/images/down.png" alt="${mess}" style="width: 16px; height: 16px;">
                    </button>
                `);
                    } else {
                        window.$("#category-list").append(`
              <button id="btnLess" class="category-action-btn" style="opacity: 0; transform: translateY(20px);">
                  <img src="/images/up.png" alt="${mess}" style="width: 16px; height: 16px;">
              </button>
            `);
                    }
                    window.$("#category-list").append(`
              <button id="btnAllCategory" class="category-action-btn" style="opacity: 0; transform: translateY(20px);">
                  <img src="/images/all.png" alt="Tất cả" style="width: 16px; height: 16px;">
              </button>
            `);
                    window.$("#category-list").append(`
            <button id="btnSearchCategory" class="category-action-btn" style="opacity: 0; transform: translateY(20px);">
                <img src="/images/search-icon.png" alt="Tìm kiếm" style="width: 16px; height: 16px;">
            </button>
          `);

                    window.$("#category-list").fadeIn(300, function () {
                        window
                            .$(
                                "#category-list .category-pill, #category-list .category-action-btn"
                            )
                            .each(function (index) {
                                var $element = window.$(this);
                                setTimeout(function () {
                                    $element.css({
                                        transition: "all 0.3s ease-out",
                                        opacity: "1",
                                        transform: "translateY(0)",
                                    });
                                }, index * 40);
                            });

                        BindEvents();
                    });

                    console.log("Loaded " + limitedCategories.length + " categories");
                } else {
                    console.log("No categories found or invalid response structure");
                    window
                        .$("#category-list")
                        .html('<p class="text-muted">Không có danh mục nào</p>')
                        .fadeIn(300);
                }
            });
        },
        error: (error) => {
            console.log("Error loading categories:", error);
            window.$("#category-list").fadeOut(300, function () {
                window
                    .$("#category-list")
                    .html('<p class="text-danger">Lỗi tải danh mục</p>')
                    .fadeIn(300);
            });
        },
    });
}

function LoadNewsDetail(metaUrl) {
    if (typeof $ === "undefined") {
        console.error("jQuery is not loaded yet");
        return;
    }

    $.ajax({
        url: "/New/GetDetail",
        type: "GET",
        data: { metaUrl: metaUrl },
        dataType: "json",
        success: function (response) {
            console.log("News detail response:", response);
            $("#news-detail-container").empty();
            if (response && response.result === 1 && response.data) {
                var news = response.data;
                var html = `
                    <div class="news-detail">
                        <h1>${news.name || ""}</h1>
                        <div class="meta">
                            <span>Ngày đăng: ${FormatDate(
                    news.publishedAt
                )}</span>
                            <span style="margin-left: 16px;">Lượt xem: ${news.viewNumber || 0
                    }</span>
                        </div>
                        <div class="description" style="margin: 16px 0; color: #555; font-size: 16px;">${news.description || ""
                    }</div>
                        <div class="content">${news.content || ""}</div>
                    </div>
                `;
                $("#news-detail-container").html(html);
            } else {
                $("#news-detail-container").html(
                    '<p class="text-muted">Không tìm thấy tin tức</p>'
                );
            }
        },
        error: function (error) {
            console.log("Error loading news detail:", error);
            $("#news-detail-container").html(
                '<p class="text-danger">Lỗi tải chi tiết tin tức</p>'
            );
        },
    });
}

function LoadFeaturedNews() {
    if (typeof $ === "undefined") {
        console.error("jQuery is not loaded yet");
        return;
    }

    $.ajax({
        url: "/New/GetFeaturedNews",
        type: "GET",
        dataType: "json",
        success: function (response) {
            console.log("Featured news response:", response);

            $("#featured-news-container").empty();

            if (response && response.result === 1 && Array.isArray(response.data)) {
                var limitedNews = response.data.slice(0, 3);
                var badgeColors = ["#0d6efd", "#198754", "#ffc107"];

                limitedNews.forEach(function (news, index) {
                    var badgeColor = badgeColors[index] || "#6c757d";
                    var imageUrl = "/images/demo_images.webp";

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
                                    ${news.newsCategoryObj?.name || "Tin tức"}
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

                    $("#featured-news-container").append(newsHtml);
                });

                console.log(`Loaded ${limitedNews.length} featured news items`);
            } else {
                $("#featured-news-container").html(
                    '<div class="text-center text-muted" style="font-size: 14px;">Không có tin tức nổi bật nào</div>'
                );
            }
        },
        error: function (error) {
            console.error("Error loading featured news:", error);
            $("#featured-news-container").html(
                '<div class="text-center text-danger" style="font-size: 14px;">Lỗi tải tin tức nổi bật</div>'
            );
        },
    });
}

async function LoadNewsList(categoryIds = []) {
    try {
        $("#news-list").html('<div class="text-center"><div class="spinner-border text-primary" role="status"></div><p class="mt-2">Đang tải tin tức...</p></div>');

        const response = await HomeApi.getList(categoryIds);

        console.log("News list response:", response);

        const data = response && response.res && Array.isArray(response.res.data)
            ? response.res.data
            : response && response.data && Array.isArray(response.data)
                ? response.data
                : [];

        if (data.length > 0) {
            RenderNewsList(data);
        } else {
            $("#news-list").html('<p class="text-muted text-center">Không tìm thấy tin tức nào</p>');
        }

    } catch (error) {
        console.error("Error loading news list:", error);
        $("#news-list").html('<p class="text-danger text-center">Lỗi tải tin tức</p>');
    }
}

function RenderNewsList(newsList) {
    $("#news-list").empty();

    newsList.forEach(function (news) {
        var imageUrl = "/images/demo_images.webp";
        var categoryName = news.newsCategoryObj && news.newsCategoryObj.name
            ? news.newsCategoryObj.name
            : "Tin tức";
        var hotBadge = news.isHot
            ? '<span class="badge bg-danger ms-2" style="font-size: 10px;">Hot</span>'
            : "";
        var publishedDate = news.publishedAt
            ? FormatDate(news.publishedAt)
            : "";
        var viewCount = news.viewNumber || 0;

        var newsHtml = `
      <div class="single-what-news mb-4" style="border: 1px solid #e9ecef; border-radius: 12px; overflow: hidden; background: #fff; box-shadow: 0 2px 8px rgba(0,0,0,0.08); transition: all 0.3s ease;" 
           onmouseover="this.style.transform='translateY(-2px)'; this.style.boxShadow='0 4px 15px rgba(0,0,0,0.12)';" 
           onmouseout="this.style.transform='translateY(0)'; this.style.boxShadow='0 2px 8px rgba(0,0,0,0.08)';">
        <div class="what-img" style="position: relative; overflow: hidden; height: 250px;">
          <img src="${imageUrl}" alt="${news.name || "Tin tức"}" 
               style="width: 100%; height: 100%; object-fit: cover; transition: transform 0.3s ease;" 
               onmouseover="this.style.transform='scale(1.05)';" 
               onmouseout="this.style.transform='scale(1)'">
          <div style="position: absolute; bottom: 15px; left: 15px; background: linear-gradient(135deg, #007bff, #0056b3); color: white; padding: 8px 16px; border-radius: 25px; font-size: 11px; font-weight: 700; text-transform: uppercase; letter-spacing: 0.8px; box-shadow: 0 4px 12px rgba(0,123,255,0.3); backdrop-filter: blur(10px);">
            ${categoryName}
          </div>
        </div>
        <div class="what-cap" style="padding: 25px 20px 20px 20px;">
          <h4 style="margin: 0 0 12px 0; font-size: 18px; font-weight: 700; line-height: 1.4;">
            <a href="/tin-tuc/${news.metaUrl || news.id}" 
               style="text-decoration: none; color: #2c3e50; transition: color 0.2s ease;" 
               onmouseover="this.style.color='#007bff';" 
               onmouseout="this.style.color='#2c3e50';">
              ${news.name || "Tin tức"}
            </a>
            ${hotBadge}
          </h4>
          <p style="color: #6c757d; font-size: 14px; line-height: 1.6; margin: 0 0 15px 0; display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden;">
            ${news.description || news.summary || ""}
          </p>
          <div class="whats-date" style="display: flex; justify-content: space-between; align-items: center; padding-top: 15px; border-top: 1px solid #f1f3f4; font-size: 13px; color: #6c757d;">
            <span style="display: flex; align-items: center;">
              <i class="fas fa-calendar" style="color: #007bff; margin-right: 8px;"></i>
              ${publishedDate}
            </span>
            <span style="display: flex; align-items: center;">
              <i class="fas fa-eye" style="color: #007bff; margin-right: 8px;"></i>
              ${viewCount} lượt xem
            </span>
          </div>
        </div>
      </div>
    `;

        $("#news-list").append(newsHtml);
    });

    console.log("Loaded " + newsList.length + " news items");
}

function LoadMostViewed() {
    if (typeof $ === "undefined") {
        console.error("jQuery is not loaded yet");
        return;
    }

    $.ajax({
        url: "/Home/GetMostViewed",
        type: "GET",
        dataType: "json",
        success: function (response) {
            console.log("Most viewed response:", response);

            if (response && response.result === 1 && Array.isArray(response.data)) {
                const container = $(".most-recent-area");
                container.find(".most-recent").remove();

                const top3 = response.data.slice(0, 3);
                const imageUrl = "/images/demo_images.webp";

                top3.forEach((news, index) => {
                    const rank = String(index + 1).padStart(2, "0");
                    const date = FormatDate(news.publishedAt);
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
                console.log("Không có dữ liệu tin tức xem nhiều");
            }
        },
        error: function (error) {
            console.log("Error loading most viewed news:", error);
        },
    });
}

function LoadRelatedNews(metaUrl) {
    $.ajax({
        url: "/Home/GetRelatedNews",
        type: "GET",
        data: { metaUrl: metaUrl },
        success: function (res) {
            if (res.result === 1) {
                let html = "";
                res.data.forEach((item) => {
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

                if (!document.getElementById("related-news-custom-style")) {
                    const style = document.createElement("style");
                    style.id = "related-news-custom-style";
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
                $("#related-news-container").html(html);
            } else {
                console.log("No related news found or API result != 1");
            }
        },
        error: function (err) {
            console.log("Error when calling /Home/GetRelatedNews:", err);
        },
    });
}

function HandleBtnMoreClick() {
    var $btn = $("#btnMore");
    var $img = $btn.find("img");
    var originalSrc = $img.attr("src");
    $btn.prop("disabled", true);

    $img.css({
        opacity: "0.6",
        transform: "translateY(0) rotate(360deg)",
        transition: "all 0.3s ease",
    });

    LoadCategories(null, "Ít hơn");

    setTimeout(function () {
        $btn.prop("disabled", false);
        $img.css({
            opacity: "1",
            transform: "translateY(0) rotate(0deg)",
            transition: "all 0.3s ease",
        });
    }, 500);
}

function HandleBtnLessClick() {
    var $btn = $("#btnLess");
    var $img = $btn.find("img");
    $btn.prop("disabled", true);

    $img.css({
        opacity: "0.6",
        transform: "translateY(0) rotate(360deg)",
        transition: "all 0.3s ease",
    });

    LoadCategories(3, "Nhiều hơn");

    setTimeout(function () {
        $btn.prop("disabled", false);
        $img.css({
            opacity: "1",
            transform: "translateY(0) rotate(0deg)",
            transition: "all 0.3s ease",
        });
    }, 500);
}

function HandleBtnSearchCategoryClick() {
    if (clickedCategories.length > 0) {
        const categoryIds = clickedCategories.map(cat => cat.id);
        console.log("Category IDs to search:", categoryIds);
        LoadNewsList(categoryIds);
    } else {
        console.log("Không có danh mục nào được chọn");
        alert("Vui lòng chọn ít nhất một danh mục để tìm kiếm");
    }
}

function HandleBtnAllCategoryClick() {
    const $allPills = $("#category-list .category-pill");
    const totalCategories = $allPills.length;
    const selectedCategories = clickedCategories.length;

    if (selectedCategories === totalCategories && totalCategories > 0) {
        clickedCategories = [];
        $allPills.removeClass("active");

        $("#btnAllCategory img").attr("src", "/images/all.png");

        iziToast.info({
            title: 'Đã bỏ chọn',
            message: 'Đã bỏ chọn tất cả danh mục',
            position: 'topRight'
        });
    } else {
        clickedCategories = [];

        $allPills.each(function () {
            const $pill = $(this);
            const categoryId = $pill.attr("href").split("/").pop();
            const categoryName = $pill.text().trim();

            clickedCategories.push({ id: categoryId, name: categoryName });
            $pill.addClass("active");
        });

        $("#btnAllCategory img").attr("src", "/images/cross.png");

        if (clickedCategories.length > 0) {
            iziToast.success({
                title: 'Thành công',
                message: `Đã chọn ${clickedCategories.length} danh mục`,
                position: 'topRight'
            });
        }
    }
}

function HandleCategoryPillClick(e) {
    e.preventDefault();

    const $pill = $(this);
    const categoryId = $pill.attr("href").split("/").pop();
    const categoryName = $pill.text().trim();

    const existingIndex = clickedCategories.findIndex(cat => cat.id == categoryId);

    if (existingIndex === -1) {
        clickedCategories.push({ id: categoryId, name: categoryName });
        $pill.addClass("active");
        console.log(`Đã thêm danh mục: ${categoryName} (ID: ${categoryId})`);
    } else {
        clickedCategories.splice(existingIndex, 1);
        $pill.removeClass("active");
        console.log(`Đã xóa danh mục: ${categoryName} (ID: ${categoryId})`);
    }

    console.log("Danh mục đã click:", clickedCategories);
}

function HandleCategorySelectChange() {
    var val = $(this).val();
    LoadNewsList(val ? parseInt(val) : null);
}

function BindEvents() {
    $("#btnMore").off("click").on("click", HandleBtnMoreClick);
    $("#btnLess").off("click").on("click", HandleBtnLessClick);
    $("#btnSearchCategory").off("click").on("click", HandleBtnSearchCategoryClick);
    $("#btnAllCategory").off("click").on("click", HandleBtnAllCategoryClick);
    $("#categorySelect").off("change").on("change", HandleCategorySelectChange);

    $(document).off("click", ".category-pill").on("click", ".category-pill", HandleCategoryPillClick);
}

WaitForJQuery();