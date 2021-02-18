//$(document).ready(function () {
//    CKEDITOR.replace('Description', {
//        height: 400,
//        filebrowserUploadUrl: "/admin/uploadckeditor"
//    });
//});
//CKEDITOR.editorConfig = function (config) {
//    config.removeDialogTabs = 'image:advanced;image:Link;link:advanced;link:upload';
//    config.filebrowserImageUploadUrl = '/Admin/UploadCKEditor' //Action for Uploding image  
//};

//editor.on('fileUploadResponse', function (evt) {
//    // Prevent the default response handler.
//    evt.stop();

//    // Get XHR and response.
//    var data = evt.data,
//        xhr = data.fileLoader.xhr,
//        response = xhr.responseText.split('|');

//    if (response[1]) {
//        // An error occurred during upload.
//        data.message = response[1];
//        evt.cancel();
//    } else {
//        data.url = response[0];
//    }
//});

$(document).ready(function () {
    $('#formAdd')
        .submit(function (e) {
            e.preventDefault();
            $.ajax({
                url: '/admin/uploadpicturefilebrowser',
                type: 'POST',
                data: new FormData(this),
                processData: false,
                contentType: false,
                success: function (data) {
                    $('#uploadPicture').val('');
                    $('#fileExplorer').append(`<div class="thumbnail"><img class="ml-2" src="/images/uploads/${data.data}" alt="thumb" title="${data.data}"
                                                width="120" height="100" /><br />${data.data}<form action="/admin/deleteimage" method="post">
                                                <input type="hidden" name="name" value="${data.data}"/><input style="margin-left:30px;" type="submit" value="Удалить"/></form></div>`);
                }


            });
        });
});