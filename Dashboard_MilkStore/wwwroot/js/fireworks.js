// Fireworks effect for buttons
class Fireworks {
    constructor() {
        this.canvas = document.createElement('canvas');
        this.canvas.style.position = 'fixed';
        this.canvas.style.top = '0';
        this.canvas.style.left = '0';
        this.canvas.style.pointerEvents = 'none';
        this.canvas.style.zIndex = '9999';
        this.canvas.width = window.innerWidth;
        this.canvas.height = window.innerHeight;
        document.body.appendChild(this.canvas);
        
        this.ctx = this.canvas.getContext('2d');
        this.particles = [];
        this.isActive = false;
        this.animationId = null;
        
        // Handle window resize
        window.addEventListener('resize', () => {
            this.canvas.width = window.innerWidth;
            this.canvas.height = window.innerHeight;
        });
    }
    
    // Create a particle at the specified position
    createParticle(x, y, color) {
        return {
            x: x,
            y: y,
            color: color || this.getRandomColor(),
            radius: Math.random() * 3 + 2,
            velocity: {
                x: Math.random() * 6 - 3,
                y: Math.random() * 6 - 3
            },
            alpha: 1,
            decay: Math.random() * 0.015 + 0.01
        };
    }
    
    // Generate a random color
    getRandomColor() {
        const colors = [
            '#FF5252', '#FF4081', '#E040FB', '#7C4DFF', 
            '#536DFE', '#448AFF', '#40C4FF', '#18FFFF', 
            '#64FFDA', '#69F0AE', '#B2FF59', '#EEFF41', 
            '#FFFF00', '#FFD740', '#FFAB40', '#FF6E40'
        ];
        return colors[Math.floor(Math.random() * colors.length)];
    }
    
    // Create fireworks at the specified position
    createFireworks(x, y) {
        const particleCount = 100;
        for (let i = 0; i < particleCount; i++) {
            this.particles.push(this.createParticle(x, y));
        }
    }
    
    // Update and draw all particles
    update() {
        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
        
        for (let i = 0; i < this.particles.length; i++) {
            const p = this.particles[i];
            
            // Update position
            p.x += p.velocity.x;
            p.y += p.velocity.y;
            
            // Add gravity
            p.velocity.y += 0.05;
            
            // Reduce alpha
            p.alpha -= p.decay;
            
            // Draw particle
            this.ctx.save();
            this.ctx.globalAlpha = p.alpha;
            this.ctx.fillStyle = p.color;
            this.ctx.beginPath();
            this.ctx.arc(p.x, p.y, p.radius, 0, Math.PI * 2);
            this.ctx.closePath();
            this.ctx.fill();
            this.ctx.restore();
        }
        
        // Remove faded particles
        this.particles = this.particles.filter(p => p.alpha > 0);
        
        // Stop animation if no particles left
        if (this.particles.length === 0) {
            this.stop();
        } else if (this.isActive) {
            this.animationId = requestAnimationFrame(() => this.update());
        }
    }
    
    // Start the animation
    start(x, y) {
        if (!this.isActive) {
            this.isActive = true;
            this.createFireworks(x, y);
            this.update();
        } else {
            // If already active, just add more fireworks
            this.createFireworks(x, y);
        }
    }
    
    // Stop the animation
    stop() {
        this.isActive = false;
        if (this.animationId) {
            cancelAnimationFrame(this.animationId);
            this.animationId = null;
        }
    }
    
    // Clean up
    destroy() {
        this.stop();
        if (this.canvas && this.canvas.parentNode) {
            this.canvas.parentNode.removeChild(this.canvas);
        }
    }
}

// Initialize fireworks
document.addEventListener('DOMContentLoaded', function() {
    const fireworks = new Fireworks();
    
    // Add fireworks effect to buttons with 'fireworks-btn' class
    document.addEventListener('click', function(e) {
        if (e.target.closest('.fireworks-btn')) {
            const rect = e.target.closest('.fireworks-btn').getBoundingClientRect();
            const x = rect.left + rect.width / 2;
            const y = rect.top + rect.height / 2;
            fireworks.start(x, y);
        }
    });
});
