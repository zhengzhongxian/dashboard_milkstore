// Fullscreen Fireworks Effect
class FullscreenFireworks {
  constructor(selector) {
    this.buttons = document.querySelectorAll(selector);
    this.canvas = null;
    this.ctx = null;
    this.fireworks = [];
    this.particles = [];
    this.isActive = false;
    this.animationId = null;
    this.init();
  }

  init() {
    // Khởi tạo biến để throttling animation
    this.lastFrameTime = 0;

    // Create overlay for darkening the screen
    this.overlay = document.createElement("div");
    this.overlay.className = "fireworks-overlay";
    this.overlay.style.position = "fixed";
    this.overlay.style.top = "0";
    this.overlay.style.left = "0";
    this.overlay.style.width = "100%";
    this.overlay.style.height = "100%";
    this.overlay.style.backgroundColor = "rgba(0, 0, 0, 0.15)";
    this.overlay.style.pointerEvents = "none";
    this.overlay.style.zIndex = "9998";
    this.overlay.style.display = "none";
    this.overlay.style.transition = "opacity 0.3s ease";
    this.overlay.style.opacity = "0";

    // Add overlay to body
    document.body.appendChild(this.overlay);

    // Create canvas for fullscreen fireworks
    this.canvas = document.createElement("canvas");
    this.canvas.className = "fireworks-canvas";
    this.canvas.style.position = "fixed";
    this.canvas.style.top = "0";
    this.canvas.style.left = "0";
    this.canvas.style.width = "100%";
    this.canvas.style.height = "100%";
    this.canvas.style.pointerEvents = "none";
    this.canvas.style.zIndex = "9999";
    this.canvas.style.display = "none";

    // Add canvas to body
    document.body.appendChild(this.canvas);

    // Get context
    this.ctx = this.canvas.getContext("2d");

    // Find close button
    this.closeButton = document.querySelector(".fireworks-close-btn");

    // Add click event to fireworks buttons
    this.buttons.forEach((button) => {
      button.addEventListener("click", () => {
        this.startFireworks();

        // Add ripple effect to button
        this.createRipple(button);

        // Show close button
        if (this.closeButton) {
          this.closeButton.style.opacity = "1";
          this.closeButton.style.visibility = "visible";
        }
      });
    });

    // Add click event to close button
    if (this.closeButton) {
      this.closeButton.addEventListener("click", () => {
        this.stopFireworks();

        // Hide close button
        this.closeButton.style.opacity = "0";
        this.closeButton.style.visibility = "hidden";
      });
    }

    // Handle resize
    window.addEventListener("resize", () => {
      this.resizeCanvas();
    });

    // Initial resize
    this.resizeCanvas();
  }

  resizeCanvas() {
    this.canvas.width = window.innerWidth;
    this.canvas.height = window.innerHeight;
  }

  startFireworks() {
    // Nếu đang có hiệu ứng chạy, không làm gì cả
    if (this.isActive) {
      return;
    }

    // Show overlay with fade in effect
    this.overlay.style.display = "block";
    setTimeout(() => {
      this.overlay.style.opacity = "1";
    }, 10);

    // Show canvas
    this.canvas.style.display = "block";

    // Clear any existing animation
    if (this.animationId) {
      cancelAnimationFrame(this.animationId);
    }

    // Reset arrays
    this.fireworks = [];
    this.particles = [];

    // Start animation
    this.isActive = true;
    this.animate();

    // Bắn pháo hoa ban đầu với tốc độ đều đặn
    for (let i = 0; i < 8; i++) {
      // Giảm số lượng pháo hoa ban đầu để đều hơn
      setTimeout(() => {
        if (this.isActive) {
          // Kiểm tra xem hiệu ứng có còn hoạt động không
          this.launchFirework();

          // Thêm cơ hội bắn pháo hoa kép với tần suất vừa phải
          if (Math.random() > 0.4) {
            // Giảm tần suất xuống 60% để đều hơn
            setTimeout(() => {
              if (this.isActive) {
                this.launchFirework();

                // Thêm cơ hội bắn pháo hoa ba với tần suất thấp
                if (Math.random() > 0.6) {
                  // Giảm tần suất xuống 40% để đều hơn
                  setTimeout(() => {
                    if (this.isActive) {
                      this.launchFirework();
                    }
                  }, 60); // Tăng thời gian chờ để đều hơn
                }
              }
            }, 60); // Tăng thời gian chờ để đều hơn
          }
        }
      }, i * 100); // Tăng thời gian chờ giữa các pháo hoa để đều hơn
    }

    // Tiếp tục bắn pháo hoa với tần suất cao để nổ diện rộng hơn
    this.launchInterval = setInterval(() => {
      if (this.isActive && this.fireworks.length < 8) {
        // Tăng số lượng pháo hoa đồng thời lên 8 để nổ diện rộng hơn
        this.launchFirework();

        // Thêm cơ hội bắn pháo hoa kép với tần suất vừa phải
        if (Math.random() > 0.5) {
          // Giảm tần suất xuống 50% để đều hơn
          setTimeout(() => {
            if (this.isActive) {
              this.launchFirework();

              // Thêm cơ hội bắn pháo hoa ba với tần suất thấp
              if (Math.random() > 0.7) {
                // Giảm tần suất xuống 30% để đều hơn
                setTimeout(() => {
                  if (this.isActive) {
                    this.launchFirework();
                  }
                }, 80); // Tăng thời gian chờ để đều hơn
              }
            }
          }, 80); // Tăng thời gian chờ để đều hơn
        }
      }
    }, 300); // Tăng thời gian chờ giữa các lần bắn để đều hơn

    // Kéo dài thời gian hiệu ứng
    this.autoStopTimeout = setTimeout(() => {
      this.stopFireworks(true); // true = tự động dừng
    }, 12000); // Tăng thời gian lên 12 giây để thưởng thức hiệu ứng siêu sặc sỡ diện rộng
  }

  stopFireworks(isAutoStop = false) {
    // Hủy timeout tự động dừng nếu có
    if (this.autoStopTimeout) {
      clearTimeout(this.autoStopTimeout);
      this.autoStopTimeout = null;
    }

    // Nếu đã dừng rồi, không làm gì cả
    if (!this.isActive) {
      return;
    }

    // Dừng tất cả hoạt động
    this.isActive = false;
    clearInterval(this.launchInterval);

    // Hủy animation frame hiện tại nếu có
    if (this.animationId) {
      cancelAnimationFrame(this.animationId);
      this.animationId = null;
    }

    // Xóa tất cả pháo hoa và hạt
    this.fireworks = [];
    this.particles = [];

    // Ẩn canvas ngay lập tức
    this.canvas.style.display = "none";

    // Fade out và ẩn overlay
    this.overlay.style.opacity = "0";
    setTimeout(() => {
      this.overlay.style.display = "none";
    }, 300); // Chỉ đợi hiệu ứng fade out hoàn thành

    // Ẩn nút đóng - luôn ẩn khi dừng hiệu ứng
    if (this.closeButton) {
      this.closeButton.style.opacity = "0";
      this.closeButton.style.visibility = "hidden";
    }

    // Log thông tin để debug
    console.log("Fireworks stopped, auto stop:", isAutoStop);
  }

  launchFirework() {
    // Tạo vị trí bắt đầu ngẫu nhiên ở dưới cùng của màn hình
    const startX = Math.random() * this.canvas.width;
    const startY = this.canvas.height;

    // Tạo vị trí đích ở nửa trên của màn hình
    const endX = startX + (Math.random() * 400 - 200); // Phạm vi rộng hơn
    const endY = Math.random() * (this.canvas.height * 0.6); // Cao hơn một chút

    // Tạo màu sắc rực rỡ
    const color = this.getRandomColor();

    // Tạo hiệu ứng pháo hoa
    this.fireworks.push({
      x: startX,
      y: startY,
      targetX: endX,
      targetY: endY,
      speed: 12, // Tốc độ cố định để bay đều hơn
      angle: Math.atan2(endY - startY, endX - startX),
      color: color,
      size: 2 + Math.random() * 2, // Kích thước đa dạng hơn
      trail: [],
      // Thêm hiệu ứng lấp lánh cho đuôi pháo hoa
      glitter: Math.random() > 0.5, // 50% cơ hội có lấp lánh
      // Thêm hiệu ứng âm thanh (chỉ là thuộc tính, không thực sự phát âm thanh)
      soundEffect: Math.floor(Math.random() * 3),
    });

    // Thêm một số hạt nhỏ bay theo pháo hoa - giảm số lượng để tăng hiệu suất
    if (Math.random() > 0.8) {
      // Giảm xuống 20% cơ hội có hạt phụ
      const sparkCount = Math.floor(Math.random() * 5) + 3; // Giảm số lượng hạt
      for (let i = 0; i < sparkCount; i++) {
        const sparkSpeed = 1 + Math.random() * 2;
        const sparkAngle = Math.random() * Math.PI * 2;

        this.particles.push({
          x: startX,
          y: startY,
          vx: Math.cos(sparkAngle) * sparkSpeed,
          vy: Math.sin(sparkAngle) * sparkSpeed - 2, // Bay lên trên
          color: color,
          size: Math.random() * 2 + 0.5,
          alpha: 1,
          decay: Math.random() * 0.04 + 0.03, // Tăng tốc độ biến mất
          gravity: 0.1,
        });
      }
    }
  }

  explodeFirework(firework) {
    // Tăng số lượng hạt để nổ diện rộng hơn
    const particleCount = 100 + Math.floor(Math.random() * 50);

    // Quyết định loại pháo hoa - tăng tỷ lệ pháo hoa nhiều màu
    const fireworkType = Math.floor(Math.random() * 6);

    // Tạo hiệu ứng lấp lánh tại điểm nổ (giảm số lượng hạt)
    this.createSparkleEffect(firework);

    switch (fireworkType) {
      case 0: // Pháo hoa đơn màu
        this.createStandardExplosion(firework, particleCount);
        break;
      case 1: // Pháo hoa nhiều màu
      case 5: // Tăng tỷ lệ pháo hoa nhiều màu (2/6)
        this.createMultiColorExplosion(firework, particleCount);
        break;
      case 2: // Pháo hoa hình tia
        this.createRayExplosion(firework, particleCount);
        break;
      case 3: // Pháo hoa hình tròn
        this.createCircleExplosion(firework, particleCount);
        break;
      case 4: // Pháo hoa hình trái tim
        this.createHeartExplosion(firework, particleCount);
        break;
      default:
        this.createStandardExplosion(firework, particleCount);
    }

    // Tăng xác suất pháo hoa phụ lên 60% để nổ diện rộng hơn
    if (Math.random() < 0.6) {
      setTimeout(() => {
        // Tạo pháo hoa phụ xa hơn để nổ diện rộng hơn
        const offsetX = (Math.random() - 0.5) * 200;
        const offsetY = (Math.random() - 0.5) * 200;

        // Chọn loại pháo hoa phụ ngẫu nhiên
        const secondaryType = Math.floor(Math.random() * 5);
        // Tăng số lượng hạt cho pháo hoa phụ
        const secondaryCount = 80 + Math.floor(Math.random() * 40);

        switch (secondaryType) {
          case 0:
            this.createStandardExplosion(
              {
                x: firework.x + offsetX,
                y: firework.y + offsetY,
                color: this.getRandomColor(),
              },
              secondaryCount
            );
            break;
          case 1:
            this.createMultiColorExplosion(
              {
                x: firework.x + offsetX,
                y: firework.y + offsetY,
                color: this.getRandomColor(),
              },
              secondaryCount
            );
            break;
          case 2:
            this.createRayExplosion(
              {
                x: firework.x + offsetX,
                y: firework.y + offsetY,
                color: this.getRandomColor(),
              },
              secondaryCount
            );
            break;
          case 3:
            this.createCircleExplosion(
              {
                x: firework.x + offsetX,
                y: firework.y + offsetY,
                color: this.getRandomColor(),
              },
              secondaryCount
            );
            break;
          case 4:
            this.createHeartExplosion(
              {
                x: firework.x + offsetX,
                y: firework.y + offsetY,
                color: this.getRandomColor(),
              },
              secondaryCount
            );
            break;
        }
      }, 30 + Math.random() * 70); // Nổ cực nhanh

      // Thêm pháo hoa phụ thứ ba để nổ diện rộng hơn nữa
      if (Math.random() < 0.4) {
        setTimeout(() => {
          // Tạo pháo hoa phụ thứ ba xa hơn nữa
          const offsetX = (Math.random() - 0.5) * 300;
          const offsetY = (Math.random() - 0.5) * 300;

          // Chọn loại pháo hoa phụ ngẫu nhiên
          const tertiaryType = Math.floor(Math.random() * 5);
          // Số lượng hạt cho pháo hoa phụ thứ ba
          const tertiaryCount = 60 + Math.floor(Math.random() * 30);

          switch (tertiaryType) {
            case 0:
              this.createStandardExplosion(
                {
                  x: firework.x + offsetX,
                  y: firework.y + offsetY,
                  color: this.getRandomColor(),
                },
                tertiaryCount
              );
              break;
            case 1:
              this.createMultiColorExplosion(
                {
                  x: firework.x + offsetX,
                  y: firework.y + offsetY,
                  color: this.getRandomColor(),
                },
                tertiaryCount
              );
              break;
            case 2:
              this.createRayExplosion(
                {
                  x: firework.x + offsetX,
                  y: firework.y + offsetY,
                  color: this.getRandomColor(),
                },
                tertiaryCount
              );
              break;
            case 3:
              this.createCircleExplosion(
                {
                  x: firework.x + offsetX,
                  y: firework.y + offsetY,
                  color: this.getRandomColor(),
                },
                tertiaryCount
              );
              break;
            case 4:
              this.createHeartExplosion(
                {
                  x: firework.x + offsetX,
                  y: firework.y + offsetY,
                  color: this.getRandomColor(),
                },
                tertiaryCount
              );
              break;
          }
        }, 100 + Math.random() * 150); // Nổ sau pháo hoa phụ thứ hai
      }
    }
  }

  // Hiệu ứng lấp lánh tại điểm nổ - tăng tốc độ
  createSparkleEffect(firework) {
    // Tăng số lượng hạt lấp lánh để hiệu ứng đẹp hơn
    const sparkleCount = 25 + Math.floor(Math.random() * 15);

    for (let i = 0; i < sparkleCount; i++) {
      const size = Math.random() * 2.5 + 2;
      this.particles.push({
        x: firework.x + (Math.random() - 0.5) * 25,
        y: firework.y + (Math.random() - 0.5) * 25,
        vx: (Math.random() - 0.5) * 4, // Tăng tốc độ bay
        vy: (Math.random() - 0.5) * 4, // Tăng tốc độ bay
        color: "#FFFFFF", // Màu trắng cho hiệu ứng lấp lánh
        size: size,
        alpha: 1,
        decay: Math.random() * 0.04 + 0.04, // Tăng tốc độ biến mất
        gravity: 0.02, // Tăng trọng lực để rơi nhanh hơn
        // Đánh dấu là hạt lấp lánh đặc biệt
        isSparkle: true,
      });
    }
  }

  // Pháo hoa chuẩn - nâng cấp để sặc sỡ hơn, nổ nhanh hơn và diện rộng hơn
  createStandardExplosion(firework, particleCount) {
    const color = firework.color;
    // Thêm màu phụ để tạo hiệu ứng đa sắc
    const secondaryColor = this.getRandomColor();

    for (let i = 0; i < particleCount; i++) {
      // Tăng tốc độ và phạm vi nổ SIÊU MẠNH để nổ diện rộng hơn
      const speed = 8 + Math.random() * 15;
      const angle = Math.random() * Math.PI * 2;

      // Tạo hiệu ứng đa dạng về kích thước
      let particleSize;
      let particleDecay;
      let particleColor;

      // Phân loại hạt để tạo hiệu ứng đa dạng
      if (Math.random() < 0.2) {
        // Hạt lớn, tồn tại ngắn hơn
        particleSize = Math.random() * 5 + 2;
        particleDecay = Math.random() * 0.015 + 0.015; // Tăng tốc độ biến mất
        particleColor = Math.random() < 0.5 ? color : secondaryColor;
      } else if (Math.random() < 0.4) {
        // Hạt trung bình
        particleSize = Math.random() * 3 + 1.5;
        particleDecay = Math.random() * 0.02 + 0.02; // Tăng tốc độ biến mất
        particleColor = Math.random() < 0.7 ? color : secondaryColor;
      } else {
        // Hạt nhỏ, tạo hiệu ứng bụi
        particleSize = Math.random() * 2 + 0.8;
        particleDecay = Math.random() * 0.025 + 0.025; // Tăng tốc độ biến mất
        particleColor = Math.random() < 0.9 ? color : "#FFFFFF"; // Thêm một số hạt trắng
      }

      this.particles.push({
        x: firework.x,
        y: firework.y,
        vx: Math.cos(angle) * speed,
        vy: Math.sin(angle) * speed,
        color: particleColor,
        size: particleSize,
        alpha: 1,
        decay: particleDecay,
        gravity: 0.06 + Math.random() * 0.03, // Tăng trọng lực để rơi nhanh hơn
        // Thêm hiệu ứng lấp lánh cho một số hạt
        sparkle: Math.random() < 0.3,
      });
    }
  }

  // Pháo hoa nhiều màu - nâng cấp siêu sặc sỡ
  createMultiColorExplosion(firework, particleCount) {
    // Tạo một bảng màu đặc biệt cho vụ nổ này
    const specialColors = [];
    for (let i = 0; i < 5; i++) {
      specialColors.push(this.getRandomColor());
    }

    for (let i = 0; i < particleCount; i++) {
      // Tăng tốc độ và phạm vi nổ SIÊU MẠNH để nổ diện rộng hơn
      const speed = 10 + Math.random() * 18;
      const angle = Math.random() * Math.PI * 2;

      // Tạo hiệu ứng đa dạng về kích thước và tốc độ
      let particleSize;
      let particleDecay;
      let particleGravity;

      // Phân loại hạt để tạo hiệu ứng đa dạng
      if (Math.random() < 0.15) {
        // Hạt lớn, tồn tại ngắn hơn
        particleSize = Math.random() * 5 + 2;
        particleDecay = Math.random() * 0.015 + 0.012; // Tăng tốc độ biến mất
        particleGravity = 0.05 + Math.random() * 0.03; // Tăng trọng lực
      } else if (Math.random() < 0.4) {
        // Hạt trung bình
        particleSize = Math.random() * 3 + 1.5;
        particleDecay = Math.random() * 0.02 + 0.015; // Tăng tốc độ biến mất
        particleGravity = 0.06 + Math.random() * 0.03; // Tăng trọng lực
      } else {
        // Hạt nhỏ, tạo hiệu ứng bụi
        particleSize = Math.random() * 2 + 0.8;
        particleDecay = Math.random() * 0.025 + 0.02; // Tăng tốc độ biến mất
        particleGravity = 0.07 + Math.random() * 0.03; // Tăng trọng lực
      }

      // Chọn màu từ bảng màu đặc biệt hoặc lấy màu hoàn toàn ngẫu nhiên
      let particleColor;
      if (Math.random() < 0.7) {
        // 70% lấy từ bảng màu đặc biệt để tạo sự đồng nhất
        particleColor =
          specialColors[Math.floor(Math.random() * specialColors.length)];
      } else {
        // 30% lấy màu hoàn toàn ngẫu nhiên để tạo điểm nhấn
        particleColor = this.getRandomColor();
      }

      // Thêm hiệu ứng lấp lánh cho một số hạt
      const hasSparkle = Math.random() < 0.3;

      this.particles.push({
        x: firework.x,
        y: firework.y,
        vx: Math.cos(angle) * speed,
        vy: Math.sin(angle) * speed,
        color: particleColor,
        size: particleSize,
        alpha: 1,
        decay: particleDecay,
        gravity: particleGravity,
        sparkle: hasSparkle,
      });
    }
  }

  // Pháo hoa hình tia
  createRayExplosion(firework, particleCount) {
    const color = firework.color;
    const rayCount = 12; // Số tia

    for (let i = 0; i < rayCount; i++) {
      const angle = (i / rayCount) * Math.PI * 2;
      const particlesPerRay = Math.floor(particleCount / rayCount);

      for (let j = 0; j < particlesPerRay; j++) {
        const speed = 4 + Math.random() * 8 + j * 0.2; // Tăng tốc độ nổ và tăng nhanh hơn theo khoảng cách

        this.particles.push({
          x: firework.x,
          y: firework.y,
          vx: Math.cos(angle) * speed,
          vy: Math.sin(angle) * speed,
          color: color,
          size: Math.random() * 3 + 1,
          alpha: 1,
          decay: Math.random() * 0.01 + 0.01,
          gravity: 0.03,
        });
      }
    }
  }

  // Pháo hoa hình tròn
  createCircleExplosion(firework, particleCount) {
    const colors = [this.getRandomColor(), this.getRandomColor()]; // 2 màu xen kẽ
    const rings = 3; // Số vòng

    for (let ring = 0; ring < rings; ring++) {
      const radius = 2 + ring * 2; // Bán kính tăng dần
      const particlesInRing = Math.floor(particleCount / rings);

      for (let i = 0; i < particlesInRing; i++) {
        const angle = (i / particlesInRing) * Math.PI * 2;
        const speed = radius * 2; // Tăng gấp đôi tốc độ nổ

        this.particles.push({
          x: firework.x,
          y: firework.y,
          vx: Math.cos(angle) * speed,
          vy: Math.sin(angle) * speed,
          color: colors[i % 2], // Xen kẽ 2 màu
          size: Math.random() * 3 + 1,
          alpha: 1,
          decay: Math.random() * 0.01 + 0.01,
          gravity: 0.02,
        });
      }
    }
  }

  // Pháo hoa hình trái tim
  createHeartExplosion(firework, particleCount) {
    const color = "#FF0040"; // Màu đỏ cho trái tim

    for (let i = 0; i < particleCount; i++) {
      const angle = Math.random() * Math.PI * 2;
      const heartX = 16 * Math.pow(Math.sin(angle), 3); // Phương trình trái tim
      const heartY = -(
        13 * Math.cos(angle) -
        5 * Math.cos(2 * angle) -
        2 * Math.cos(3 * angle) -
        Math.cos(4 * angle)
      );

      const speed = 1.5; // Tăng tốc độ nổ cho trái tim

      this.particles.push({
        x: firework.x,
        y: firework.y,
        vx: heartX * speed * 0.2,
        vy: heartY * speed * 0.2,
        color: color,
        size: Math.random() * 3 + 1,
        alpha: 1,
        decay: Math.random() * 0.008 + 0.008, // Chậm hơn để thấy rõ hình dạng
        gravity: 0.01,
      });
    }
  }

  animate(timestamp = 0) {
    // Cập nhật thời gian frame cuối để theo dõi
    this.lastFrameTime = timestamp;

    // Tối ưu hóa hiệu suất bằng cách chỉ xóa canvas khi cần thiết
    // Sử dụng clearRect thay vì fillRect để tăng hiệu suất
    this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);

    // Giảm độ mờ của nền để tăng hiệu suất và làm cho hiệu ứng biến mất nhanh hơn
    this.ctx.fillStyle = "rgba(0, 0, 0, 0.01)"; // Giảm độ mờ để hiệu ứng biến mất nhanh hơn
    this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

    // Update and draw fireworks
    for (let i = this.fireworks.length - 1; i >= 0; i--) {
      const f = this.fireworks[i];

      // Move firework
      f.x += Math.cos(f.angle) * f.speed;
      f.y += Math.sin(f.angle) * f.speed;

      // Add to trail
      f.trail.push({ x: f.x, y: f.y, alpha: 1 });

      // Giảm độ dài đuôi để bay đều hơn
      if (f.trail.length > 10) {
        f.trail.shift();
      }

      // Vẽ đuôi pháo hoa với hiệu ứng sáng mạnh hơn
      this.ctx.shadowBlur = 8;
      this.ctx.shadowColor = f.color;

      this.ctx.beginPath();
      for (let j = 0; j < f.trail.length; j++) {
        const point = f.trail[j];
        const alpha = point.alpha * (j / f.trail.length);

        this.ctx.strokeStyle = this.colorWithAlpha(f.color, alpha);
        this.ctx.lineWidth = f.size * (j / f.trail.length);

        if (j === 0) {
          this.ctx.moveTo(point.x, point.y);
        } else {
          this.ctx.lineTo(point.x, point.y);
        }

        point.alpha -= 0.1; // Tăng tốc độ mờ đi để đuôi ngắn hơn, bay đều hơn
      }
      this.ctx.stroke();

      // Vẽ hiệu ứng lấp lánh cho đuôi pháo hoa (nếu có) - giảm tần suất để tăng hiệu suất
      if (f.glitter && Math.random() > 0.85) {
        // Giảm tần suất xuống 15%
        const trailPoint = f.trail[Math.floor(Math.random() * f.trail.length)];
        if (trailPoint) {
          this.ctx.fillStyle = "#FFFFFF";
          this.ctx.beginPath();
          this.ctx.arc(
            trailPoint.x,
            trailPoint.y,
            f.size * 0.5,
            0,
            Math.PI * 2
          );
          this.ctx.closePath();
          this.ctx.fill();
        }
      }

      // Reset shadow
      this.ctx.shadowBlur = 0;

      // Check if firework reached target - giảm khoảng cách để nổ sớm hơn
      const distance = Math.sqrt(
        Math.pow(f.x - f.targetX, 2) + Math.pow(f.y - f.targetY, 2)
      );

      if (distance < 15 || f.y < f.targetY) {
        // Explode - tăng khoảng cách để nổ sớm hơn và đều hơn
        this.explodeFirework(f);
        this.fireworks.splice(i, 1);
      }
    }

    // Update and draw particles
    for (let i = this.particles.length - 1; i >= 0; i--) {
      const p = this.particles[i];

      // Apply gravity
      p.vy += p.gravity;

      // Move particle
      p.x += p.vx;
      p.y += p.vy;

      // Reduce alpha
      p.alpha -= p.decay;

      // Draw particle
      if (p.alpha > 0) {
        this.ctx.globalAlpha = p.alpha;

        // Tối ưu hiệu ứng đổ bóng để mượt hơn
        // Áp dụng hiệu ứng đổ bóng cho tất cả các hạt với mức độ khác nhau
        if (p.isSparkle) {
          // Hạt lấp lánh đặc biệt có đổ bóng mạnh
          this.ctx.shadowBlur = 20;
          this.ctx.shadowColor = "#FFFFFF";
        } else if (Math.random() < 0.5) {
          // 50% hạt có đổ bóng mạnh
          this.ctx.shadowBlur = 12;
          this.ctx.shadowColor = p.color;
        } else {
          // 50% hạt còn lại có đổ bóng nhẹ
          this.ctx.shadowBlur = 6;
          this.ctx.shadowColor = p.color;
        }

        // Vẽ hạt pháo hoa với hiệu ứng đặc biệt
        this.ctx.fillStyle = p.color;
        this.ctx.beginPath();
        this.ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
        this.ctx.closePath();
        this.ctx.fill();

        // Tối ưu hiệu ứng lấp lánh để mượt hơn
        if (p.sparkle || p.isSparkle || Math.random() < 0.1) {
          // Giảm xác suất ngẫu nhiên xuống 10% để tối ưu hiệu suất

          // Vẽ hiệu ứng lấp lánh với màu trắng
          this.ctx.fillStyle = "#FFFFFF";
          this.ctx.beginPath();
          this.ctx.arc(p.x, p.y, p.size * 0.5, 0, Math.PI * 2);
          this.ctx.closePath();
          this.ctx.fill();

          // Thêm hiệu ứng ánh sáng cho hạt lấp lánh đặc biệt
          if (p.isSparkle) {
            this.ctx.shadowBlur = 15;
            this.ctx.shadowColor = "#FFFFFF";
            this.ctx.beginPath();
            this.ctx.arc(p.x, p.y, p.size * 0.3, 0, Math.PI * 2);
            this.ctx.closePath();
            this.ctx.fill();
          }
        }

        // Reset shadow
        this.ctx.shadowBlur = 0;
        this.ctx.globalAlpha = 1;
      } else {
        this.particles.splice(i, 1);
      }
    }

    // Continue animation
    if (
      this.isActive ||
      this.fireworks.length > 0 ||
      this.particles.length > 0
    ) {
      this.animationId = requestAnimationFrame((timestamp) =>
        this.animate(timestamp)
      );
    }
  }

  getRandomColor() {
    // Bảng màu siêu sặc sỡ giống như trong ảnh pháo hoa thực tế
    const colors = [
      // Màu chính từ ảnh pháo hoa
      "#FF3EA5", // Hồng đậm
      "#FF9AE3", // Hồng nhạt
      "#FF73DF", // Hồng tươi
      "#00FFFF", // Cyan sáng
      "#00E1FF", // Cyan nhạt
      "#73FDFF", // Xanh biển nhạt
      "#FFF700", // Vàng rực
      "#FFDA00", // Vàng đậm
      "#FFAC41", // Cam vàng
      "#FF5757", // Đỏ tươi
      "#FF2E2E", // Đỏ đậm
      "#FF7B54", // Cam đỏ
      "#36FF00", // Xanh lá cây neon
      "#83FF61", // Xanh lá nhạt
      "#00FF94", // Xanh ngọc
      "#4D52FF", // Xanh dương đậm
      "#00B2FF", // Xanh dương sáng
      "#8C52FF", // Tím đậm
      "#D552FF", // Tím hồng
      "#FF52E5", // Hồng tím

      // Màu trắng sáng cho hiệu ứng lấp lánh
      "#FFFFFF", // Trắng tinh
      "#F9F9FF", // Trắng xanh
      "#FFF9F9", // Trắng hồng
      "#FFFBF2", // Trắng vàng

      // Màu gradient đặc biệt
      "#FF00FF", // Magenta neon
      "#FF0099", // Hồng neon
      "#FF1744", // Đỏ neon
      "#FF9100", // Cam neon
      "#FFEA00", // Vàng neon
      "#76FF03", // Xanh lá neon
      "#00E5FF", // Xanh biển neon
      "#2979FF", // Xanh dương neon
      "#651FFF", // Tím neon
      "#D500F9", // Tím hồng neon

      // Màu kim loại sáng
      "#FFD700", // Vàng kim
      "#FFC0CB", // Hồng kim
      "#E6E6FA", // Tím nhạt kim
      "#87CEFA", // Xanh biển kim
      "#98FB98", // Xanh lá kim
    ];
    return colors[Math.floor(Math.random() * colors.length)];
  }

  // Tạo màu gradient ngẫu nhiên
  getRandomGradientColor() {
    const color1 = this.getRandomColor();
    const color2 = this.getRandomColor();
    return {
      start: color1,
      end: color2,
    };
  }

  colorWithAlpha(color, alpha) {
    // Convert hex to rgba
    const r = parseInt(color.slice(1, 3), 16);
    const g = parseInt(color.slice(3, 5), 16);
    const b = parseInt(color.slice(5, 7), 16);
    return `rgba(${r}, ${g}, ${b}, ${alpha})`;
  }

  createRipple(button) {
    // Create ripple element
    const ripple = document.createElement("span");
    ripple.className = "btn-ripple";

    // Style the ripple
    ripple.style.position = "absolute";
    ripple.style.top = "50%";
    ripple.style.left = "50%";
    ripple.style.transform = "translate(-50%, -50%)";
    ripple.style.width = "0";
    ripple.style.height = "0";
    ripple.style.backgroundColor = "rgba(255, 255, 255, 0.4)";
    ripple.style.borderRadius = "50%";
    ripple.style.pointerEvents = "none";
    ripple.style.zIndex = "0";
    ripple.style.transition = "all 0.6s ease-out";

    // Make sure button has position relative
    if (window.getComputedStyle(button).position === "static") {
      button.style.position = "relative";
    }

    // Add ripple to button
    button.appendChild(ripple);

    // Animate ripple
    setTimeout(() => {
      const size = Math.max(button.offsetWidth, button.offsetHeight) * 2;
      ripple.style.width = size + "px";
      ripple.style.height = size + "px";
      ripple.style.opacity = "0";
    }, 10);

    // Remove ripple after animation
    setTimeout(() => {
      ripple.remove();
    }, 700);
  }
}

// Initialize on DOM load
document.addEventListener("DOMContentLoaded", function () {
  // Create fullscreen fireworks effect
  new FullscreenFireworks(".special-fireworks-btn");
});
