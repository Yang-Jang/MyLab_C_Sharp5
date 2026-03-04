// 1. Hàm hiển thị Toast (Client-side)
function showToast(type, message) {
    const toast = document.getElementById('clientToast');
    if (!toast) return;

    const iconEl = document.getElementById('clientToastIcon');
    const titleEl = document.getElementById('clientToastTitle');
    const msgEl = document.getElementById('clientToastMsg');

    // Cấu hình nội dung
    msgEl.innerText = message;
    
    // Cấu hình Icon & Màu sắc
    let iconClass = 'fa-circle-info';
    let titleText = 'Thông báo';
    
    // Reset class cũ
    toast.className = 'sf-toast'; 

    if (type === 'success') {
        iconClass = 'fa-circle-check';
        titleText = 'Thành công';
        toast.classList.add('sf-toast-success');
    } else if (type === 'error') {
        iconClass = 'fa-circle-xmark';
        titleText = 'Lỗi';
        toast.classList.add('sf-toast-error');
    } else if (type === 'warning') {
        iconClass = 'fa-triangle-exclamation';
        titleText = 'Lưu ý';
        toast.classList.add('sf-toast-warning');
    }

    titleEl.innerText = titleText;
    iconEl.innerHTML = `<i class="fa-solid ${iconClass}"></i>`;

    // Hiển thị
    requestAnimationFrame(() => {
        toast.classList.add('show');
    });

    // Tự ẩn sau 3s
    if (window.toastTimeout) clearTimeout(window.toastTimeout);
    window.toastTimeout = setTimeout(() => {
        toast.classList.remove('show');
    }, 3000);
}

// 2. Hàm gọi API đếm số giỏ hàng
function refreshCartBadge() {
    fetch('/Cart/GetCartCount')
        .then(res => res.json())
        .then(count => {
            const badge = document.getElementById('cartBadge');
            if (badge) {
                badge.innerText = count;
                // Nếu > 0 thì hiện, = 0 thì ẩn
                if (count > 0) {
                    badge.classList.remove('d-none');
                    badge.style.display = 'inline-block'; // Đảm bảo hiện
                } else {
                    badge.classList.add('d-none');
                    badge.style.display = 'none';
                }
            }
        })
        .catch(err => console.error("Lỗi lấy số giỏ hàng:", err));
}