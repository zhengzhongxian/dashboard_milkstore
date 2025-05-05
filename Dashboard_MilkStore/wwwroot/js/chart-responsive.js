// Xử lý responsive cho biểu đồ
(function() {
    // Biến lưu trữ các biểu đồ
    let charts = [];
    
    // Hàm khởi tạo
    function init() {
        // Lắng nghe sự kiện resize của cửa sổ
        window.addEventListener('resize', debounce(handleResize, 250));
        
        // Lắng nghe sự kiện khi biểu đồ được tạo
        document.addEventListener('chartCreated', function(e) {
            if (e.detail && e.detail.chart) {
                charts.push(e.detail.chart);
            }
        });
    }
    
    // Hàm xử lý khi cửa sổ thay đổi kích thước
    function handleResize() {
        // Cập nhật kích thước font chữ dựa trên kích thước màn hình
        updateFontSizes();
        
        // Vẽ lại tất cả các biểu đồ
        charts.forEach(chart => {
            if (chart && typeof chart.update === 'function') {
                chart.update();
            }
        });
    }
    
    // Hàm cập nhật kích thước font chữ dựa trên kích thước màn hình
    function updateFontSizes() {
        const width = window.innerWidth;
        let titleSize, labelSize, tickSize;
        
        if (width < 400) {
            titleSize = '0.8rem';
            labelSize = '0.7rem';
            tickSize = 8;
        } else if (width < 576) {
            titleSize = '0.9rem';
            labelSize = '0.8rem';
            tickSize = 10;
        } else if (width < 768) {
            titleSize = '1rem';
            labelSize = '0.9rem';
            tickSize = 12;
        } else {
            titleSize = '1.1rem';
            labelSize = '1rem';
            tickSize = 14;
        }
        
        // Cập nhật kích thước font chữ cho tiêu đề biểu đồ
        document.querySelectorAll('.chart-title').forEach(title => {
            title.style.fontSize = titleSize;
        });
        
        // Cập nhật kích thước font chữ cho các nhãn trong biểu đồ
        charts.forEach(chart => {
            if (chart && chart.options && chart.options.scales) {
                // Cập nhật kích thước font chữ cho trục x
                if (chart.options.scales.x && chart.options.scales.x.ticks) {
                    chart.options.scales.x.ticks.font = {
                        ...chart.options.scales.x.ticks.font,
                        size: tickSize
                    };
                }
                
                // Cập nhật kích thước font chữ cho trục y
                if (chart.options.scales.y && chart.options.scales.y.ticks) {
                    chart.options.scales.y.ticks.font = {
                        ...chart.options.scales.y.ticks.font,
                        size: tickSize
                    };
                }
            }
        });
    }
    
    // Hàm debounce để tránh gọi hàm quá nhiều lần
    function debounce(func, wait) {
        let timeout;
        return function() {
            const context = this;
            const args = arguments;
            clearTimeout(timeout);
            timeout = setTimeout(function() {
                func.apply(context, args);
            }, wait);
        };
    }
    
    // Khởi tạo khi trang đã tải xong
    document.addEventListener('DOMContentLoaded', init);
})();
