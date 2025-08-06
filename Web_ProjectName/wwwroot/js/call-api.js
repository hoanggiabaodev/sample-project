const ApiService = {
    callApi: function (url, method, data, contentType = 'application/json') {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: url,
                method: method,
                data: contentType === 'application/json' ? JSON.stringify(data) : data,
                contentType: contentType,
                dataType: 'json',
                success: function (response) {
                    resolve(response);
                },
                error: function (xhr, status, error) {
                    console.error('API Error:', { url, method, status, error, response: xhr.responseText });
                    reject({ xhr, status, error });
                }
            });
        });
    },

    get: function (url, params = {}) {
        const queryString = Object.keys(params).length > 0
            ? '?' + new URLSearchParams(params).toString()
            : '';
        return this.callApi(url + queryString, 'GET');
    },

    post: function (url, data) {
        return this.callApi(url, 'POST', data);
    },

    put: function (url, data) {
        return this.callApi(url, 'PUT', data);
    },

    delete: function (url) {
        return this.callApi(url, 'DELETE');
    }
};

const NewsApi = {
    getList: function (params = {}) {
        return ApiService.get('/New/GetListByStatus', params);
    },

    getById: function (id) {
        return ApiService.get(`/New/GetById/${id}`);
    },

    save: function (newsData) {
        return ApiService.post('/New/Save', newsData);
    },

    update: function (newsData) {
        return ApiService.post('/New/Update', newsData);
    },

    delete: function (id) {
        return ApiService.delete(`/New/Delete/${id}`);
    },

    getCategories: function () {
        return ApiService.get('/New/GetCategories');
    }
};

const CategoryApi = {
    getCategories: function () {
        return ApiService.get('/New/GetCategories');
    }
};

const ApiUtils = {
    handleResponse: function (response, successMessage = 'Thành công', errorMessage = 'Có lỗi xảy ra') {
        if (response.result === 1) {
            if (successMessage) {
                showToast('success', 'Thành công', successMessage);
            }
            return { success: true, data: response.data };
        } else {
            showToast('error', 'Lỗi', response.error || errorMessage);
            return { success: false, error: response.error || errorMessage };
        }
    },

    handleError: function (error, errorMessage = 'Không thể kết nối đến máy chủ') {
        console.error('API Error:', error);
        showToast('error', 'Lỗi', errorMessage);
        return { success: false, error: errorMessage };
    }
};
