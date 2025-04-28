// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function setImageSize(size) {
    const images = document.querySelectorAll('.gallery-image');
    images.forEach(img => {
        img.classList.remove('small', 'medium', 'large');
    img.classList.add(size);
    });
}


function updateImageSize(value) {
    const gallery = document.getElementById('imageGallery');
    const images = document.querySelectorAll('.gallery-image');
    const galleryWidth = gallery.clientWidth;
    const maxAllowedWidth = galleryWidth * 0.8; // 80% of gallery width

    let effectiveWidth = Math.min(value, maxAllowedWidth);

    images.forEach(img => {
        img.style.maxWidth = effectiveWidth + 'px'; /* force max width dynamically */
        img.style.width = effectiveWidth + 'px'; /* and current width too */
    });
}

// Attach live input event
document.getElementById('imageSizeSlider').addEventListener('input', (e) => {
    updateImageSize(e.target.value);
});

function toggleSettings() {
    const panel = document.getElementById('settingsPanel');
    panel.classList.toggle('open');
}


function openOverlay(imgElement) {
    const overlay = document.getElementById('imageOverlay');
    const overlayImage = document.getElementById('overlayImage');
    overlayImage.src = imgElement.src;
    overlay.style.display = 'flex';
}

function closeOverlay() {
    const overlay = document.getElementById('imageOverlay');
    overlay.style.display = 'none';
}

document.querySelectorAll('.gallery-image').forEach(img => {
    img.addEventListener('click', function (e) {
        openOverlay(e.target.src);
    });
});