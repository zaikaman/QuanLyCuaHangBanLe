/**
 * Toast Helper - Hệ thống thông báo toast hiện đại
 * Sử dụng: toast.success('Thành công!'), toast.error('Lỗi!'), toast.warning('Cảnh báo!'), toast.info('Thông tin!')
 */

// Đảm bảo toast functions đã được định nghĩa trong _Layout.cshtml
if (typeof window.showToast === 'undefined') {
    console.error('Toast system not found! Make sure _Layout.cshtml is loaded.');
}

/**
 * Custom Confirm Dialog - Thay thế confirm() mặc định
 * @param {string} message - Nội dung xác nhận
 * @param {string} title - Tiêu đề dialog (mặc định: "Xác nhận")
 * @param {Object} options - Tùy chọn {confirmText, cancelText, type}
 * @returns {Promise<boolean>} - true nếu người dùng xác nhận, false nếu hủy
 */
window.showConfirm = function(message, title = 'Xác nhận', options = {}) {
    return new Promise((resolve) => {
        // Tạo modal overlay
        const overlay = document.createElement('div');
        overlay.className = 'confirm-overlay';
        overlay.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: rgba(0, 0, 0, 0.5);
            backdrop-filter: blur(4px);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 10002;
            animation: fadeIn 0.2s ease-out;
        `;

        // Tạo confirm dialog
        const dialog = document.createElement('div');
        dialog.className = 'confirm-dialog';
        dialog.style.cssText = `
            background: var(--card-bg);
            border-radius: 16px;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
            width: 90%;
            max-width: 450px;
            animation: slideUp 0.3s ease-out;
            overflow: hidden;
        `;

        const confirmType = options.type || 'warning';
        const iconColors = {
            danger: '#ef4444',
            warning: '#f59e0b',
            info: '#3b82f6',
            success: '#10b981'
        };
        const iconClasses = {
            danger: 'fa-exclamation-circle',
            warning: 'fa-exclamation-triangle',
            info: 'fa-info-circle',
            success: 'fa-check-circle'
        };

        dialog.innerHTML = `
            <div style="padding: 32px 24px 24px; text-align: center;">
                <div style="
                    width: 64px;
                    height: 64px;
                    background: ${iconColors[confirmType]}15;
                    border-radius: 50%;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    margin: 0 auto 20px;
                ">
                    <i class="fas ${iconClasses[confirmType]}" style="
                        font-size: 32px;
                        color: ${iconColors[confirmType]};
                    "></i>
                </div>
                <h3 style="
                    margin: 0 0 12px;
                    font-size: 1.4rem;
                    font-weight: 700;
                    color: var(--text-primary);
                ">${title}</h3>
                <p style="
                    margin: 0;
                    font-size: 1rem;
                    color: var(--text-secondary);
                    line-height: 1.6;
                ">${message}</p>
            </div>
            <div style="
                padding: 20px 24px;
                background: var(--bg-secondary);
                display: flex;
                gap: 12px;
                border-top: 1px solid var(--border-primary);
            ">
                <button class="confirm-cancel-btn" style="
                    flex: 1;
                    padding: 12px 24px;
                    border: 2px solid var(--border-secondary);
                    background: var(--bg-tertiary);
                    color: var(--text-primary);
                    border-radius: 10px;
                    font-weight: 600;
                    font-size: 1rem;
                    cursor: pointer;
                    transition: all 0.2s;
                ">
                    ${options.cancelText || 'Hủy'}
                </button>
                <button class="confirm-ok-btn" style="
                    flex: 1;
                    padding: 12px 24px;
                    border: none;
                    background: ${iconColors[confirmType]};
                    color: white;
                    border-radius: 10px;
                    font-weight: 600;
                    font-size: 1rem;
                    cursor: pointer;
                    transition: all 0.2s;
                ">
                    ${options.confirmText || 'Xác nhận'}
                </button>
            </div>
        `;

        overlay.appendChild(dialog);
        document.body.appendChild(overlay);

        // Thêm animation keyframes nếu chưa có
        if (!document.querySelector('#confirm-animations')) {
            const style = document.createElement('style');
            style.id = 'confirm-animations';
            style.textContent = `
                @keyframes fadeIn {
                    from { opacity: 0; }
                    to { opacity: 1; }
                }
                @keyframes slideUp {
                    from { 
                        transform: translateY(20px) scale(0.95);
                        opacity: 0;
                    }
                    to { 
                        transform: translateY(0) scale(1);
                        opacity: 1;
                    }
                }
                .confirm-cancel-btn:hover {
                    background: var(--bg-hover) !important;
                    border-color: var(--border-hover) !important;
                    transform: translateY(-2px);
                }
                .confirm-ok-btn:hover {
                    filter: brightness(1.1);
                    transform: translateY(-2px);
                    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
                }
            `;
            document.head.appendChild(style);
        }

        // Xử lý sự kiện
        const cancelBtn = dialog.querySelector('.confirm-cancel-btn');
        const okBtn = dialog.querySelector('.confirm-ok-btn');

        const closeDialog = (result) => {
            overlay.style.animation = 'fadeIn 0.2s ease-out reverse';
            dialog.style.animation = 'slideUp 0.2s ease-out reverse';
            setTimeout(() => {
                overlay.remove();
                resolve(result);
            }, 200);
        };

        cancelBtn.addEventListener('click', () => closeDialog(false));
        okBtn.addEventListener('click', () => closeDialog(true));
        
        // Click ngoài dialog để đóng
        overlay.addEventListener('click', (e) => {
            if (e.target === overlay) {
                closeDialog(false);
            }
        });

        // ESC để đóng
        const escHandler = (e) => {
            if (e.key === 'Escape') {
                closeDialog(false);
                document.removeEventListener('keydown', escHandler);
            }
        };
        document.addEventListener('keydown', escHandler);

        // Focus vào nút xác nhận
        setTimeout(() => okBtn.focus(), 100);
    });
};

/**
 * Convenience wrapper for showConfirm
 */
window.confirmDialog = {
    danger: (message, title) => showConfirm(message, title || 'Xác nhận xóa', { 
        type: 'danger',
        confirmText: 'Xóa',
        cancelText: 'Hủy'
    }),
    warning: (message, title) => showConfirm(message, title || 'Cảnh báo', { 
        type: 'warning',
        confirmText: 'Tiếp tục',
        cancelText: 'Hủy'
    }),
    info: (message, title) => showConfirm(message, title || 'Thông tin', { 
        type: 'info',
        confirmText: 'OK',
        cancelText: 'Hủy'
    }),
    success: (message, title) => showConfirm(message, title || 'Xác nhận', { 
        type: 'success',
        confirmText: 'Xác nhận',
        cancelText: 'Hủy'
    })
};

// Export để sử dụng ở nơi khác nếu cần
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { showConfirm, confirmDialog };
}
