// $(document).ready(function () {

//     //Init bootstrap max length
//     $('[maxlength]').maxlength({
//         alwaysShow: !0,
//         warningClass: "badge bg-success",
//         limitReachedClass: "badge bg-danger"
//     });

//     $('[data-toggle="tooltip"]').tooltip();

//     //Submit form contact
//     $('#form_data_contact').on('submit', function (e) {
//         let $formElm = $(this);
//         grecaptcha.ready(function () {
//             grecaptcha.execute(reCATPCHA_Site_Key, { action: 'submit' }).then(function (token) {
//                 // Add your logic to submit to your backend server here.
//                 $formElm.find(".tokenReCAPTCHA").val(token);

//                 let isvalidate = $formElm[0].checkValidity();
//                 if (!isvalidate) { ShowToastNoti('warning', '', _resultActionResource.PleaseWrite); return false; }
//                 e.preventDefault();
//                 e.stopImmediatePropagation();
//                 let formData = new FormData($formElm[0]);
//                 let laddaSubmitForm = Ladda.create($formElm.find('button[type="submit"]')[0]);
//                 laddaSubmitForm.start();
//                 $.ajax({
//                     url: '/Product/Send',
//                     type: 'POST',
//                     data: formData,
//                     contentType: false,
//                     processData: false,
//                     success: function (response) {
//                         laddaSubmitForm.stop();
//                         if (!CheckResponseIsSuccess(response)) return false;
//                         Swal.fire('Đã gửi yêu cầu! Cảm ơn bạn đã quan tâm!', '', 'success');
//                         $formElm[0].reset();
//                     }, error: function (err) {
//                         laddaSubmitForm.stop();
//                         CheckResponseIsSuccess({ result: -1, error: { code: err.status } });
//                     }
//                 });
//             });
//         });
//     });

// });

// Mock data for news
const newsData = [
    {
        id: 1,
        title: "Tin tức công nghệ mới nhất trong năm 2024",
        excerpt: "Những xu hướng công nghệ đang thay đổi thế giới và tác động đến cuộc sống hàng ngày của chúng ta. Từ AI đến blockchain, tất cả đều đang phát triển với tốc độ chóng mặt.",
        image: "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=400&h=200&fit=crop",
        category: "technology",
        categoryName: "Công nghệ",
        date: "2024-01-15",
        views: 1250,
        author: "Nguyễn Văn A"
    },
    {
        id: 2,
        title: "Thị trường chứng khoán Việt Nam tuần qua",
        excerpt: "Phân tích chi tiết về diễn biến thị trường chứng khoán và những cơ hội đầu tư tiềm năng trong thời gian tới. VN-Index có những biến động đáng chú ý.",
        image: "https://images.unsplash.com/photo-1611974789855-9c2a0a7236a3?w=400&h=200&fit=crop",
        category: "economy",
        categoryName: "Kinh tế",
        date: "2024-01-14",
        views: 980,
        author: "Trần Thị B"
    },
    {
        id: 3,
        title: "Bóng đá Việt Nam chuẩn bị cho SEA Games",
        excerpt: "Đội tuyển quốc gia đang tích cực chuẩn bị cho giải đấu quan trọng sắp tới. HLV Park Hang-seo đã có những chiến thuật mới cho đội tuyển.",
        image: "https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=400&h=200&fit=crop",
        category: "sports",
        categoryName: "Thể thao",
        date: "2024-01-13",
        views: 2100,
        author: "Lê Văn C"
    },
    {
        id: 4,
        title: "Du lịch Việt Nam hồi phục mạnh mẽ",
        excerpt: "Ngành du lịch đang có những tín hiệu tích cực với lượng khách quốc tế tăng trưởng mạnh. Các điểm đến nổi tiếng đang thu hút đông đảo du khách.",
        image: "https://images.unsplash.com/photo-1539650116574-1cb2f99b2d8b?w=400&h=200&fit=crop",
        category: "travel",
        categoryName: "Du lịch",
        date: "2024-01-12",
        views: 1500,
        author: "Phạm Thị D"
    },
    {
        id: 5,
        title: "Những bộ phim hay nhất năm 2024",
        excerpt: "Tổng hợp những tác phẩm điện ảnh đáng chú ý và được đánh giá cao trong năm. Hollywood và điện ảnh Việt Nam đều có những dự án đáng mong đợi.",
        image: "https://images.unsplash.com/photo-1489599904472-af35ff2c7c3f?w=400&h=200&fit=crop",
        category: "entertainment",
        categoryName: "Giải trí",
        date: "2024-01-11",
        views: 1800,
        author: "Hoàng Văn E"
    },
    {
        id: 6,
        title: "Bí quyết sống khỏe trong thời đại số",
        excerpt: "Những lời khuyên hữu ích để duy trì sức khỏe tốt trong cuộc sống hiện đại. Cách cân bằng giữa công việc và sức khỏe tinh thần.",
        image: "https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?w=400&h=200&fit=crop",
        category: "health",
        categoryName: "Sức khỏe",
        date: "2024-01-10",
        views: 1350,
        author: "Nguyễn Thị F"
    },
    {
        id: 7,
        title: "Startup Việt Nam thu hút đầu tư lớn",
        excerpt: "Các startup công nghệ Việt Nam đang nhận được sự quan tâm lớn từ các nhà đầu tư quốc tế. Fintech và e-commerce dẫn đầu xu hướng.",
        image: "https://images.unsplash.com/photo-1559136555-9303baea8ebd?w=400&h=200&fit=crop",
        category: "technology",
        categoryName: "Công nghệ",
        date: "2024-01-09",
        views: 1680,
        author: "Đỗ Văn G"
    },
    {
        id: 8,
        title: "Lễ hội âm nhạc quốc tế tại Việt Nam",
        excerpt: "Sự kiện âm nhạc lớn nhất năm sẽ diễn ra tại TP.HCM với sự tham gia của nhiều nghệ sĩ quốc tế nổi tiếng.",
        image: "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=400&h=200&fit=crop",
        category: "entertainment",
        categoryName: "Giải trí",
        date: "2024-01-08",
        views: 2250,
        author: "Vũ Thị H"
    }
];

const featuredNewsData = [
    {
        id: 1,
        title: "Breakthrough in AI Technology Reshapes Industry",
        image: "https://images.unsplash.com/photo-1677442136019-21780ecad995?w=80&h=80&fit=crop",
        views: 3200
    },
    {
        id: 2,
        title: "Economic Growth Projections for 2024",
        image: "https://images.unsplash.com/photo-1611974789855-9c2a0a7236a3?w=80&h=80&fit=crop",
        views: 2800
    },
    {
        id: 3,
        title: "Sports Championship Finals This Weekend",
        image: "https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=80&h=80&fit=crop",
        views: 2400
    }
];

const mostViewedData = [
    { id: 1, title: "Tin tức nổi bật nhất tuần", views: 5200, rank: 1 },
    { id: 2, title: "Thông tin kinh tế mới nhất", views: 4800, rank: 2 },
    { id: 3, title: "Tin thể thao hấp dẫn", views: 4200, rank: 3 },
    { id: 4, title: "Công nghệ AI mới nhất", views: 3900, rank: 4 },
    { id: 5, title: "Du lịch hè 2024", views: 3600, rank: 5 }
];

// Global variables
let currentCategory = 'all';
let currentPage = 1;
let filteredNews = [...newsData];
const itemsPerPage = 4;

// Initialize the page
document.addEventListener('DOMContentLoaded', function() {
    initializeCategories();
    renderNews();
    renderFeaturedNews();
    renderMostViewed();
    renderPagination();
});

// Category filtering
function initializeCategories() {
    const categoryButtons = document.querySelectorAll('.category-btn');
    
    categoryButtons.forEach(button => {
        button.addEventListener('click', function() {
            // Remove active class from all buttons
            categoryButtons.forEach(btn => btn.classList.remove('active'));
            
            // Add active class to clicked button
            this.classList.add('active');
            
            // Update current category
            currentCategory = this.dataset.category;
            
            // Filter news
            filterNews();
            
            // Reset to first page
            currentPage = 1;
            
            // Re-render news and pagination
            renderNews();
            renderPagination();
        });
    });
}

// Filter news based on category
function filterNews() {
    if (currentCategory === 'all') {
        filteredNews = [...newsData];
    } else {
        filteredNews = newsData.filter(news => news.category === currentCategory);
    }
}

// Render news grid
function renderNews() {
    const newsGrid = document.getElementById('newsGrid');
    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    const currentNews = filteredNews.slice(startIndex, endIndex);
    
    newsGrid.innerHTML = '';
    
    if (currentNews.length === 0) {
        newsGrid.innerHTML = `
            <div class="no-news">
                <h4>Chưa có tin tức nào</h4>
                <p>Vui lòng quay lại sau.</p>
            </div>
        `;
        return;
    }
    
    currentNews.forEach(news => {
        const newsCard = createNewsCard(news);
        newsGrid.appendChild(newsCard);
    });
    
    // Add fade-in animation
    newsGrid.classList.add('fade-in');
    setTimeout(() => newsGrid.classList.remove('fade-in'), 500);
}

// Create news card element
function createNewsCard(news) {
    const card = document.createElement('div');
    card.className = 'news-card';
    
    card.innerHTML = `
        <div class="news-image">
            <img src="${news.image}" alt="${news.title}" loading="lazy">
            <div class="news-category">${news.categoryName}</div>
            <div class="news-views">
                <i class="fas fa-eye"></i>
                ${formatNumber(news.views)}
            </div>
        </div>
        <div class="news-content">
            <h3 class="news-title">${news.title}</h3>
            <p class="news-excerpt">${news.excerpt}</p>
            <div class="news-meta">
                <div class="news-info">
                    <span>
                        <i class="fas fa-user"></i>
                        ${news.author}
                    </span>
                    <span>
                        <i class="fas fa-calendar"></i>
                        ${formatDate(news.date)}
                    </span>
                </div>
                <div class="news-actions">
                    <button class="action-btn" onclick="likeNews(${news.id})">
                        <i class="fas fa-heart"></i>
                    </button>
                    <button class="action-btn" onclick="shareNews(${news.id})">
                        <i class="fas fa-share"></i>
                    </button>
                </div>
            </div>
        </div>
    `;
    
    return card;
}

// Render featured news
function renderFeaturedNews() {
    const featuredContainer = document.getElementById('featuredNews');
    
    featuredContainer.innerHTML = '';
    
    featuredNewsData.forEach(news => {
        const featuredItem = document.createElement('div');
        featuredItem.className = 'featured-item';
        
        featuredItem.innerHTML = `
            <div class="featured-image">
                <img src="${news.image}" alt="${news.title}" loading="lazy">
            </div>
            <div class="featured-content">
                <h4>${news.title}</h4>
                <div class="featured-views">
                    <i class="fas fa-eye"></i>
                    ${formatNumber(news.views)} lượt xem
                </div>
            </div>
        `;
        
        featuredContainer.appendChild(featuredItem);
    });
}

// Render most viewed
function renderMostViewed() {
    const mostViewedContainer = document.getElementById('mostViewed');
    
    mostViewedContainer.innerHTML = '';
    
    mostViewedData.forEach(item => {
        const mostViewedItem = document.createElement('div');
        mostViewedItem.className = 'most-viewed-item';
        
        const rankClass = item.rank === 1 ? 'rank-1' : 
                         item.rank === 2 ? 'rank-2' : 
                         item.rank === 3 ? 'rank-3' : 'rank-other';
        
        mostViewedItem.innerHTML = `
            <div class="rank-badge ${rankClass}">${item.rank}</div>
            <div class="most-viewed-content">
                <h4>${item.title}</h4>
                <div class="most-viewed-views">
                    <i class="fas fa-eye"></i>
                    ${formatNumber(item.views)} lượt xem
                </div>
            </div>
        `;
        
        mostViewedContainer.appendChild(mostViewedItem);
    });
}

// Render pagination
function renderPagination() {
    const totalPages = Math.ceil(filteredNews.length / itemsPerPage);
    const paginationNumbers = document.getElementById('paginationNumbers');
    const prevBtn = document.getElementById('prevBtn');
    const nextBtn = document.getElementById('nextBtn');
    
    // Clear existing pagination
    paginationNumbers.innerHTML = '';
    
    // Create page buttons
    for (let i = 1; i <= totalPages; i++) {
        const pageBtn = document.createElement('button');
        pageBtn.className = `page-btn ${i === currentPage ? 'active' : ''}`;
        pageBtn.textContent = i;
        pageBtn.addEventListener('click', () => {
            currentPage = i;
            renderNews();
            renderPagination();
            scrollToTop();
        });
        paginationNumbers.appendChild(pageBtn);
    }
    
    // Update prev/next buttons
    prevBtn.disabled = currentPage === 1;
    nextBtn.disabled = currentPage === totalPages || totalPages === 0;
    
    prevBtn.onclick = () => {
        if (currentPage > 1) {
            currentPage--;
            renderNews();
            renderPagination();
            scrollToTop();
        }
    };
    
    nextBtn.onclick = () => {
        if (currentPage < totalPages) {
            currentPage++;
            renderNews();
            renderPagination();
            scrollToTop();
        }
    };
}

// Utility functions
function formatNumber(num) {
    return num.toLocaleString('vi-VN');
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN', { 
        year: 'numeric', 
        month: 'long', 
        day: 'numeric' 
    });
}

function scrollToTop() {
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
}

// Action functions
function likeNews(newsId) {
    console.log('Liked news:', newsId);
    // Add your like functionality here
    alert('Đã thích tin tức này!');
}

function shareNews(newsId) {
    console.log('Shared news:', newsId);
    // Add your share functionality here
    if (navigator.share) {
        navigator.share({
            title: 'Chia sẻ tin tức',
            text: 'Xem tin tức thú vị này!',
            url: window.location.href
        });
    } else {
        // Fallback for browsers that don't support Web Share API
        const url = window.location.href;
        navigator.clipboard.writeText(url).then(() => {
            alert('Đã sao chép link tin tức!');
        });
    }
}

// Add smooth scrolling for better UX
function smoothScrollTo(element) {
    element.scrollIntoView({
        behavior: 'smooth',
        block: 'start'
    });
}

// Add intersection observer for animations
const observerOptions = {
    threshold: 0.1,
    rootMargin: '0px 0px -50px 0px'
};

const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            entry.target.classList.add('fade-in');
        }
    });
}, observerOptions);

// Observe elements for animation when they come into view
document.addEventListener('DOMContentLoaded', () => {
    setTimeout(() => {
        const animatedElements = document.querySelectorAll('.news-card, .sidebar-card');
        animatedElements.forEach(el => observer.observe(el));
    }, 100);
});

// Add loading state
function showLoading(container) {
    container.innerHTML = `
        <div class="loading">
            <div class="spinner"></div>
        </div>
    `;
}

// Handle window resize
window.addEventListener('resize', () => {
    // Re-render if needed for responsive adjustments
    renderPagination();
});

// Add keyboard navigation
document.addEventListener('keydown', (e) => {
    if (e.key === 'ArrowLeft' && currentPage > 1) {
        currentPage--;
        renderNews();
        renderPagination();
        scrollToTop();
    } else if (e.key === 'ArrowRight' && currentPage < Math.ceil(filteredNews.length / itemsPerPage)) {
        currentPage++;
        renderNews();
        renderPagination();
        scrollToTop();
    }
});
