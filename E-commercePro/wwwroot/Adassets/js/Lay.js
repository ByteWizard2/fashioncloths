
        /*const url = window.location.href;
    console.log("This is current url", url);
    const elem = document.querySelectorAll(".menu-item");
    url.replace("http://localhost:4000/api/v1/admin/","")*/
    document.addEventListener('DOMContentLoaded', function () {
            var menuItems = document.querySelectorAll('.menu-item');
    var currentURL = window.location.pathname;

    menuItems.forEach(function (item) {
                var itemURL = item.querySelector('a').getAttribute('href');
    if (currentURL.startsWith(itemURL)) {
        item.classList.add('active');
                }
            });
        });

