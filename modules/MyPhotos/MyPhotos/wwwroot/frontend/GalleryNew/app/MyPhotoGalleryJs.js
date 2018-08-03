$(document).ready(function () {
    $(".gallerythumbnail").each(function (e) {
        var changeset = $(this).attr("changeset");
        var fileHash = $(this).attr("id");
        var mainImage = this;
        var image = new Image();
        image.onload = function () {
            console.log("Large image Loaded ... ", this.src);
            mainImage.src  = this.src;
        }
        image.src = "../api/myphotos/image?file_hash=" + fileHash + "&t=l&changeset_no=" + changeset;
    });
});
