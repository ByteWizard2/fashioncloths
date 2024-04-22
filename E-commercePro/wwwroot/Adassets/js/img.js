


    //preview image
    let cropper;
    function previewImage(event) {
        const input = event.target;
    console.log("Entered preview image", input)
    let number = input.getAttribute("data-id");

    console.log(`image-preview${number}`);
    const preview = document.getElementById(`image-preview${number}`);
    const imageElement = document.querySelector(`.imagecrop${number}`);


    if (input.files && input.files[0]) {
            const reader = new FileReader();

    reader.onload = function (e) {
        //preview.src = e.target.result;

        $(document).ready(function () {
            $(`#imageModal${number}`).modal('show');
        });

    let img = document.createElement("img");
    img.id = `image`;
    img.src = e.target.result;

    imageElement.appendChild(img);

    //crop image
    cropper = new Cropper(img, {
        aspectRatio: 0,
    viewMode: 1,
    dragMode: "move",
    minContainerWidth: 450,
    minContainerHeight: 500,
    minCropBoxWidth: 400,
    minCropBoxHeight: 400,
    minCanvasHeight: 500,
    minCanvasWidth: 500,
                    /*crop(event) {
        console.log(event.detail.x);
    console.log(event.detail.y);
    console.log(event.detail.width);
    console.log(event.detail.height);
    console.log(event.detail.rotate);
    console.log(event.detail.scaleX);
    console.log(event.detail.scaleY);
                    },*/
                });

            };

    reader.readAsDataURL(input.files[0]);
        }

    const cropButton = document.getElementById(`cropButton${number}`);
        cropButton.addEventListener("click", () => {
        console.log("Entered crop button")
            let imgsrc = cropper.getCroppedCanvas().toDataURL("image/jpeg");
    console.log(imgsrc);
    preview.src = imgsrc;
    //input.value = imgsrc;
    //close modal
    var modal = document.getElementById(`imageModal${number}`);
    console.log("This is modal", modal)
    var bootstrapModal = bootstrap.Modal.getInstance(modal);
    bootstrapModal.hide();
        });
    }
