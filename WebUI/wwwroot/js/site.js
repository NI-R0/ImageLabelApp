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


//function updateImageSize(value) {
//    const images = document.querySelectorAll('.gallery-image');
//    images.forEach(img => {
//        img.style.maxWidth = value + 'px';
//    });
//}

function updateImageSize(value) {
    document.getElementById('imageGallery').style.setProperty('--image-width', value + 'px');
}

function toggleSettings() {
    const panel = document.getElementById('settingsPanel');
    panel.classList.toggle('open');
}