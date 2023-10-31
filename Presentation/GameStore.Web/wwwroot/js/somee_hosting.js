$(function () {
    let intervalId, n;
    n = 0;
    intervalId = setInterval(function () {
        $('html').next('script').attr('src', ' ');
        $('center').next('script').remove();
        $('center').next('div').remove();
        n++;
        if (n > 4) {clearInterval(intervalId); }
    }, 300);
});
