const ApiService = {
  callApi: function (url, method, data = null, {
    contentType = "application/json",
    dataType = "json"
  } = {}) {
    return new Promise((resolve, reject) => {
      $.ajax({
        url: url,
        method: method,
        data: contentType === "application/json" && data !== null
          ? JSON.stringify(data)
          : data,
        contentType: contentType,
        dataType: dataType,
        success: function (response) {
          resolve(response);
        },
        error: function (xhr, status, error) {
          console.error("API Error:", {
            url,
            method,
            status,
            error,
            response: xhr.responseText,
          });
          reject({ xhr, status, error });
        },
      });
    });
  },

  get: function (url, params = {}, options = {}) {
    const queryString =
      Object.keys(params).length > 0
        ? "?" + new URLSearchParams(params).toString()
        : "";
    return this.callApi(url + queryString, "GET", null, options);
  },

  post: function (url, data, options = {}) {
    return this.callApi(url, "POST", data, options);
  },

  put: function (url, data, options = {}) {
    return this.callApi(url, "PUT", data, options);
  },

  delete: function (url, options = {}) {
    return this.callApi(url, "DELETE", null, options);
  },
};

const HomeApi = {
  getList: function (categoryIds = []) {
    const params = {};
    if (categoryIds && categoryIds.length > 0) {
      params.categoryIds = categoryIds.join(',');
    }
    return ApiService.get("/Home/GetList", params);
  },

  mostviewed: function () {
    return ApiService.get("/Home/GetMostViewed");
  },
};


const NewsApi = {
  getList: function (params = {}) {
    return ApiService.get("/New/GetListByStatus", params); // JSON
  },

  getById: function (id) {
    return ApiService.get(`/New/GetById/${id}`);
  },

  save: function (newsData) {
    return ApiService.post("/New/P_Add", newsData);
  },

  update: function (newsData) {
    return ApiService.post("/New/P_Edit", newsData);
  },

  delete: function (id) {
    return ApiService.delete(`/New/Delete/${id}`);
  },

  updateStatus: function (id, status) {
    return ApiService.post(
      `/New/P_EditStatus/${id}?status=${encodeURIComponent(status)}`,
      {}
    );
  },

  getCategories: function () {
    return ApiService.get("/New/GetCategories");
  },
};

const CategoryApi = {
  getList: function (params = {}) {
    return ApiService.get("/NewCategory/GetListByStatus", params);
  },

  getById: function (id) {
    return ApiService.get(`/NewCategory/P_View?id=${id}`);
  },

  save: function (data) {
    return ApiService.post("/NewCategory/P_Add", data);
  },

  update: function (data) {
    return ApiService.post("/NewCategory/P_Edit", data);
  },

  delete: function (id) {
    return ApiService.delete(`/NewCategory/Delete/${id}`);
  },

  getCategories: function () {
    return ApiService.get("/New/GetCategories");
  },
};

const ApiUtils = {
  handleResponse: function (
    response,
    successMessage = "Thành công",
    errorMessage = "Có lỗi xảy ra"
  ) {
    if (response.result === 1) {
      if (successMessage) {
        showToast("success", "Thành công", successMessage);
      }
      return { success: true, data: response.data };
    } else {
      showToast("error", "Lỗi", response.error || errorMessage);
      return { success: false, error: response.error || errorMessage };
    }
  },

  handleError: function (
    error,
    errorMessage = "Không thể kết nối đến máy chủ"
  ) {
    console.error("API Error:", error);
    showToast("error", "Lỗi", errorMessage);
    return { success: false, error: errorMessage };
  },
};

function showToast(type, title, message) {
  if (typeof iziToast !== "undefined") {
    iziToast[type]({
      title: title,
      message: message,
      position: "topRight",
      timeout: 5000,
    });
  } else {
    alert(`${title}: ${message}`);
  }
}