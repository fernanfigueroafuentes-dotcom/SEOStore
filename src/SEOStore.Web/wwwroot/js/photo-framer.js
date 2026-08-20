(function () {
    const OUTPUT_WIDTH = 720;
    const OUTPUT_HEIGHT = 900;
    const MIN_ZOOM = 0.45;
    const MAX_ZOOM = 3.5;
    const ZOOM_STEP = 0.12;

    function jpegName(fileName) {
        const base = (fileName || "foto").replace(/\.[^.]+$/, "");
        return `${base}.jpg`;
    }

    class PhotoFramer {
        constructor(options) {
            this.img = options.img;
            this.viewport = options.viewport;
            this.file = options.file || null;
            this.fileName = options.fileName || this.file?.name || "foto.jpg";
            this.zoom = 1;
            this.x = 0;
            this.y = 0;
            this.dragging = false;
            this.lastX = 0;
            this.lastY = 0;
            this.bindPointer();
            this.render();
        }

        bindPointer() {
            this.viewport.addEventListener("pointerdown", (event) => {
                event.preventDefault();
                this.dragging = true;
                this.lastX = event.clientX;
                this.lastY = event.clientY;
                this.viewport.setPointerCapture(event.pointerId);
                this.viewport.classList.add("is-dragging");
            });

            this.viewport.addEventListener("pointermove", (event) => {
                if (!this.dragging)
                    return;

                this.x += event.clientX - this.lastX;
                this.y += event.clientY - this.lastY;
                this.lastX = event.clientX;
                this.lastY = event.clientY;
                this.render();
            });

            const stopDrag = (event) => {
                this.dragging = false;
                this.viewport.classList.remove("is-dragging");
                if (this.viewport.hasPointerCapture(event.pointerId))
                    this.viewport.releasePointerCapture(event.pointerId);
            };

            this.viewport.addEventListener("pointerup", stopDrag);
            this.viewport.addEventListener("pointercancel", stopDrag);
            this.viewport.addEventListener("wheel", (event) => {
                event.preventDefault();
                this.adjustZoom(event.deltaY < 0 ? ZOOM_STEP : -ZOOM_STEP);
            }, { passive: false });
        }

        fitScale() {
            const width = this.viewport.clientWidth || 1;
            const height = this.viewport.clientHeight || 1;
            const naturalWidth = this.img.naturalWidth || 1;
            const naturalHeight = this.img.naturalHeight || 1;
            return Math.min(width / naturalWidth, height / naturalHeight);
        }

        adjustZoom(delta) {
            this.zoom = Math.min(MAX_ZOOM, Math.max(MIN_ZOOM, this.zoom + delta));
            this.render();
        }

        center() {
            this.x = 0;
            this.y = 0;
            this.render();
        }

        reset() {
            this.zoom = 1;
            this.center();
        }

        render() {
            const size = this.fitScale() * this.zoom;
            const width = this.img.naturalWidth * size;
            this.img.style.width = `${width}px`;
            this.img.style.height = "auto";
            this.img.style.transform = `translate(-50%, -50%) translate(${this.x}px, ${this.y}px)`;
        }

        exportBlob() {
            return new Promise((resolve, reject) => {
                const canvas = document.createElement("canvas");
                canvas.width = OUTPUT_WIDTH;
                canvas.height = OUTPUT_HEIGHT;
                const context = canvas.getContext("2d");
                if (!context) {
                    reject(new Error("No se pudo recortar la foto."));
                    return;
                }

                context.fillStyle = "#f6f6f6";
                context.fillRect(0, 0, OUTPUT_WIDTH, OUTPUT_HEIGHT);

                const viewportWidth = this.viewport.clientWidth || 1;
                const ratio = OUTPUT_WIDTH / viewportWidth;
                const drawScale = this.fitScale() * this.zoom * ratio;
                const drawWidth = this.img.naturalWidth * drawScale;
                const drawHeight = this.img.naturalHeight * drawScale;
                const left = OUTPUT_WIDTH / 2 + this.x * ratio - drawWidth / 2;
                const top = OUTPUT_HEIGHT / 2 + this.y * ratio - drawHeight / 2;

                context.drawImage(this.img, left, top, drawWidth, drawHeight);
                canvas.toBlob((blob) => {
                    if (!blob) {
                        reject(new Error("No se pudo recortar la foto."));
                        return;
                    }

                    resolve(blob);
                }, "image/jpeg", 0.86);
            });
        }
    }

    function createEditorCard(file) {
        const card = document.createElement("div");
        card.className = "photo-framer";

        const viewport = document.createElement("div");
        viewport.className = "photo-framer-viewport";
        viewport.setAttribute("role", "img");
        viewport.setAttribute("aria-label", "Recuadro de la foto");

        const img = document.createElement("img");
        img.alt = file.name;
        img.draggable = false;

        const tools = document.createElement("div");
        tools.className = "photo-framer-tools";
        tools.innerHTML = `
            <button type="button" class="btn btn-sm btn-outline-secondary" data-action="zoom-out" aria-label="Alejar">−</button>
            <button type="button" class="btn btn-sm btn-outline-secondary" data-action="zoom-in" aria-label="Acercar">+</button>
            <button type="button" class="btn btn-sm btn-outline-secondary" data-action="center">Centrar</button>
        `;

        const caption = document.createElement("div");
        caption.className = "photo-framer-caption";
        caption.textContent = file.name;

        viewport.appendChild(img);
        card.appendChild(viewport);
        card.appendChild(tools);
        card.appendChild(caption);

        return { card, viewport, img, tools };
    }

    function wireTools(tools, framer) {
        tools.addEventListener("click", (event) => {
            const button = event.target.closest("[data-action]");
            if (!button)
                return;

            const action = button.getAttribute("data-action");
            if (action === "zoom-in")
                framer.adjustZoom(ZOOM_STEP);
            if (action === "zoom-out")
                framer.adjustZoom(-ZOOM_STEP);
            if (action === "center")
                framer.center();
            if (action === "reset")
                framer.reset();
        });
    }

    function initCreateEditor() {
        const input = document.getElementById("Photos");
        const host = document.getElementById("photo-framers");
        const form = input?.closest("form");
        if (!input || !host || !form)
            return;

        const framers = [];
        let ready = Promise.resolve();

        const clearFramers = () => {
            framers.splice(0, framers.length).forEach((item) => {
                if (item.url)
                    URL.revokeObjectURL(item.url);
            });
            host.replaceChildren();
        };

        input.addEventListener("change", () => {
            clearFramers();
            const files = Array.from(input.files || []);
            ready = Promise.all(files.map((file) => new Promise((resolve, reject) => {
                const parts = createEditorCard(file);
                const url = URL.createObjectURL(file);
                host.appendChild(parts.card);

                parts.img.addEventListener("error", () => reject(new Error("No se pudo leer la foto.")), { once: true });
                parts.img.addEventListener("load", () => {
                    const framer = new PhotoFramer({
                        img: parts.img,
                        viewport: parts.viewport,
                        file,
                        fileName: file.name
                    });
                    wireTools(parts.tools, framer);
                    framers.push({ framer, url });
                    resolve();
                }, { once: true });

                parts.img.src = url;
            })));
        });

        form.addEventListener("submit", async (event) => {
            if (form.dataset.photosReady === "1" || (input.files?.length || 0) === 0)
                return;

            if (event.defaultPrevented)
                return;

            event.preventDefault();

            try {
                await ready;
                if (framers.length === 0)
                    throw new Error("Las fotos todavía no están listas.");

                const transfer = new DataTransfer();
                for (const item of framers) {
                    const blob = await item.framer.exportBlob();
                    transfer.items.add(new File([blob], jpegName(item.framer.fileName), { type: "image/jpeg" }));
                }

                input.files = transfer.files;
                form.dataset.photosReady = "1";
                HTMLFormElement.prototype.submit.call(form);
            } catch (error) {
                console.error(error);
                form.dataset.photosReady = "";
                window.alert("No se pudieron recortar las fotos. Inténtalo de nuevo.");
            }
        });
    }

    document.addEventListener("DOMContentLoaded", () => {
        initCreateEditor();
    });
})();
