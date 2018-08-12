$(document).ready(function () {
        $(".gallerythumbnail").each(function (e) {
            var changeset = $(this).attr("changeset");
            var fileHash = $(this).attr("file_hash");
            var mainImage = this;
            var image = new Image();
            image.onload = function () {
                mainImage.src = this.src;
            }
            image.src = "../api/myphotos/image?file_hash=" + fileHash + "&t=l&changeset_no=" + changeset;
        });
});