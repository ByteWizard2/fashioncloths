$(document).ready(function () {
    // Add click event handler to each thumbnail image
    $('.product__details__pic__left').on('click', 'img', function () {
        // Get the source of the clicked thumbnail image
        var imageUrl = $(this).attr('src');
        // Update the large image with the clicked image
        $('.product__details__pic__slider').html('<div class="product__big__img"><img src="' + imageUrl + '" alt=""></div>');
    });
});