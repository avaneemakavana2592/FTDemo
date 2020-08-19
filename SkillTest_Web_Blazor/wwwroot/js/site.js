var indexComponent;
var myDropzone;
function initDropZone(pIndexComponent) {
    indexComponent = pIndexComponent;
    if (!myDropzone) {
        myDropzone = new Dropzone("div.my-dropzone", { url: "/fake" });//we don't want using automatic post but library need url
        Dropzone.forElement("div.my-dropzone").options.autoProcessQueue = false;//we don't want using automatic post
    }

    //Dropzone.forElement("div.drop-zone").options.previewsContainer = false;
    myDropzone.off("addedfile");
    myDropzone.on("addedfile", function (file) {
        let reader = new FileReader();
        $("#uploadFileLoader").show();
        reader.file = file;
        reader.onload = function (event) {

            //console.log(event.target.result);
            var base64 = reader.result.split(',')[1];

            console.log(base64.length);
            indexComponent.invokeMethodAsync('UploadFile', base64)
                .then((data) => {
                    myDropzone.removeFile(reader.file);

                    $("#uploadFileLoader").hide();
                })
                .catch((error) => {
                    console.error(error);
                    alert("error : " + error.toString());
                    $("#uploadFileLoader").hide();
                    if (reader.file) {
                        myDropzone.removeFile(reader.file);
                    }

                });


        };
        reader.readAsDataURL(file);


    });
}