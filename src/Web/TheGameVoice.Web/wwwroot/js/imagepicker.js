

document.addEventListener("DOMContentLoaded", () => {

    const galleryModal =
    document.getElementById(
    "gallery-picker-modal");

    const openGalleryBtn =
    document.getElementById(
    "open-gallery-picker");

    const closeGalleryBtn =
    document.getElementById(
    "close-gallery-picker");

    const doneGalleryBtn =
    document.getElementById(
    "done-gallery-selection");

    const selectedGalleryImages =
    new Map();

    if (openGalleryBtn)
    {
        openGalleryBtn.addEventListener(
            "click",
            () => {
                galleryModal.classList.remove(
                    "hidden");
            });
    }

    if (closeGalleryBtn)
    {
        closeGalleryBtn.addEventListener(
            "click",
            () => {
                galleryModal.classList.add(
                    "hidden");
            });
    }

    if (doneGalleryBtn)
    {
        doneGalleryBtn.addEventListener(
            "click",
            () => {
                galleryModal.classList.add(
                    "hidden");
            });
    }

    document
    .querySelectorAll(
    ".gallery-media-item")
        .forEach(item =>
    {
        item.addEventListener(
            "click",
            () => {
                const id =
                    item.dataset.id;

                const image =
                    item.dataset.image;

                if (selectedGalleryImages.has(id)) {
                    selectedGalleryImages.delete(id);

                    item.classList.remove(
                        "border-primary");
                }
                else {
                    selectedGalleryImages.set(
                        id,
                        image);

                    item.classList.add(
                        "border-primary");
                }

                renderGalleryPreview();
            });
        });

    function renderGalleryPreview()
    {
        const preview =
    document.getElementById(
    "gallery-preview");

    const hiddenInputs =
    document.getElementById(
    "gallery-hidden-inputs");

    preview.innerHTML = "";

    hiddenInputs.innerHTML = "";

    selectedGalleryImages.forEach(
            (image, id) =>
    {
        preview.innerHTML += `
                    <div class="overflow-hidden rounded-xl border">
                        <img
                            src="${image}"
                            class="aspect-video w-full object-cover" />
                    </div>
                `;

    hiddenInputs.innerHTML += `
    <input
        type="hidden"
        name="SelectedGalleryImageIds"
        value="${id}" />
    `;
            });
    }

});
