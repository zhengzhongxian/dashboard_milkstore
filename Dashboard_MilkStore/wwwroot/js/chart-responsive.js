// Xử lý responsive cho biểu đồ
(function () {
  // Biến lưu trữ các biểu đồ
  let charts = [];

  // Hàm khởi tạo
  function init() {
    // Lắng nghe sự kiện resize của cửa sổ
    window.addEventListener("resize", debounce(handleResize, 250));

    // Lắng nghe sự kiện khi biểu đồ được tạo
    document.addEventListener("chartCreated", function (e) {
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
    charts.forEach((chart) => {
      if (chart && typeof chart.update === "function") {
        chart.update();
      }
    });
  }

  // Hàm cập nhật kích thước font chữ dựa trên kích thước màn hình
  function updateFontSizes() {
    const width = window.innerWidth;
    const height = window.innerHeight;
    const isLandscape = width > height;
    let titleSize, labelSize, tickSize, aspectRatio;

    // Xác định kích thước dựa trên chiều rộng màn hình và hướng
    if (width < 320) {
      // iPhone 4/4S và các thiết bị cực nhỏ
      titleSize = "0.7rem";
      labelSize = "0.6rem";
      tickSize = 6;
      aspectRatio = isLandscape ? 2 : 1.2;
    } else if (width < 375) {
      // iPhone 5/5S/5C/SE (1st gen)
      titleSize = "0.75rem";
      labelSize = "0.65rem";
      tickSize = 7;
      aspectRatio = isLandscape ? 2 : 1.3;
    } else if (width < 390) {
      // iPhone 6/6S/7/8/SE (2nd gen)
      titleSize = "0.8rem";
      labelSize = "0.7rem";
      tickSize = 8;
      aspectRatio = isLandscape ? 2 : 1.4;
    } else if (width < 414) {
      // iPhone X/XS/11 Pro/12 Mini/13 Mini
      titleSize = "0.85rem";
      labelSize = "0.75rem";
      tickSize = 9;
      aspectRatio = isLandscape ? 2 : 1.5;
    } else if (width < 428) {
      // iPhone XR/11/12/12 Pro/13/13 Pro/14/14 Pro
      titleSize = "0.9rem";
      labelSize = "0.8rem";
      tickSize = 10;
      aspectRatio = isLandscape ? 2 : 1.6;
    } else if (width < 576) {
      // iPhone 12 Pro Max/13 Pro Max/14 Plus/14 Pro Max/15/15 Pro/15 Pro Max/16/16 Pro/16 Pro Max
      titleSize = "0.95rem";
      labelSize = "0.85rem";
      tickSize = 11;
      aspectRatio = isLandscape ? 2 : 1.7;
    } else if (width < 768) {
      titleSize = "1rem";
      labelSize = "0.9rem";
      tickSize = 12;
      aspectRatio = 1.8;
    } else {
      titleSize = "1.1rem";
      labelSize = "1rem";
      tickSize = 14;
      aspectRatio = 2;
    }

    // Cập nhật kích thước font chữ cho tiêu đề biểu đồ
    document.querySelectorAll(".chart-title").forEach((title) => {
      title.style.fontSize = titleSize;
    });

    // Cập nhật kích thước font chữ cho các nhãn trong biểu đồ
    charts.forEach((chart) => {
      if (chart && chart.options) {
        // Cập nhật tỷ lệ khung hình
        if (chart.options.maintainAspectRatio) {
          chart.options.aspectRatio = aspectRatio;
        }

        // Cập nhật kích thước font chữ cho legend
        if (
          chart.options.plugins &&
          chart.options.plugins.legend &&
          chart.options.plugins.legend.labels
        ) {
          chart.options.plugins.legend.labels.font = {
            ...chart.options.plugins.legend.labels.font,
            size: tickSize + 2,
          };

          // Ẩn legend trên màn hình nhỏ
          if (width < 375) {
            chart.options.plugins.legend.display = false;
          } else {
            chart.options.plugins.legend.display = true;
          }
        }

        // Cập nhật kích thước font chữ cho tooltip
        if (chart.options.plugins && chart.options.plugins.tooltip) {
          if (chart.options.plugins.tooltip.titleFont) {
            chart.options.plugins.tooltip.titleFont.size = tickSize + 2;
          }
          if (chart.options.plugins.tooltip.bodyFont) {
            chart.options.plugins.tooltip.bodyFont.size = tickSize;
          }

          // Điều chỉnh padding cho tooltip trên màn hình nhỏ
          if (width < 375) {
            chart.options.plugins.tooltip.padding = 6;
          } else {
            chart.options.plugins.tooltip.padding = 12;
          }
        }

        if (chart.options.scales) {
          // Cập nhật kích thước font chữ cho trục x
          if (chart.options.scales.x && chart.options.scales.x.ticks) {
            chart.options.scales.x.ticks.font = {
              ...chart.options.scales.x.ticks.font,
              size: tickSize,
            };

            // Điều chỉnh padding cho trục x trên màn hình nhỏ
            if (width < 375) {
              chart.options.scales.x.ticks.padding = 6;
            } else {
              chart.options.scales.x.ticks.padding = 12;
            }

            // Giới hạn số lượng nhãn hiển thị trên màn hình nhỏ
            if (width < 375) {
              chart.options.scales.x.ticks.maxTicksLimit = 6;
            } else if (width < 576) {
              chart.options.scales.x.ticks.maxTicksLimit = 8;
            } else {
              chart.options.scales.x.ticks.maxTicksLimit = 12;
            }
          }

          // Cập nhật kích thước font chữ cho trục y
          if (chart.options.scales.y && chart.options.scales.y.ticks) {
            chart.options.scales.y.ticks.font = {
              ...chart.options.scales.y.ticks.font,
              size: tickSize,
            };

            // Điều chỉnh padding cho trục y trên màn hình nhỏ
            if (width < 375) {
              chart.options.scales.y.ticks.padding = 6;
            } else {
              chart.options.scales.y.ticks.padding = 12;
            }

            // Giới hạn số lượng nhãn hiển thị trên màn hình nhỏ
            if (width < 375) {
              chart.options.scales.y.ticks.maxTicksLimit = 5;
            } else if (width < 576) {
              chart.options.scales.y.ticks.maxTicksLimit = 6;
            } else {
              chart.options.scales.y.ticks.maxTicksLimit = 8;
            }
          }
        }
      }
    });
  }

  // Hàm debounce để tránh gọi hàm quá nhiều lần
  function debounce(func, wait) {
    let timeout;
    return function () {
      const context = this;
      const args = arguments;
      clearTimeout(timeout);
      timeout = setTimeout(function () {
        func.apply(context, args);
      }, wait);
    };
  }

  // Khởi tạo khi trang đã tải xong
  document.addEventListener("DOMContentLoaded", init);
})();
