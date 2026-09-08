/* =========================================================================
   TheGameVoice — admin UX helpers
   -------------------------------------------------------------------------
   1. Toasts          auto-dismiss + manual close
   2. data-confirm    a real confirmation dialog for destructive actions
   3. Dirty guard     "you have unsaved changes" + Ctrl/Cmd+S to save
   4. Bulk actions    row checkboxes → floating action bar
   ========================================================================= */

(function () {
    "use strict";

    /* ---------- 1. TOASTS ------------------------------------------------ */

    function initToasts() {
        document.querySelectorAll(".tgv-toast").forEach(function (toast) {
            var hide = function () {
                toast.classList.add("tgv-toast--leaving");
                window.setTimeout(function () { toast.remove(); }, 250);
            };

            var timer = window.setTimeout(hide, 6000);

            toast.addEventListener("mouseenter", function () {
                window.clearTimeout(timer);
            });

            var close = toast.querySelector("[data-toast-close]");

            if (close) {
                close.addEventListener("click", hide);
            }
        });
    }


    /* ---------- 2. CONFIRM DIALOG ---------------------------------------- */
    /*  Usage:  <button data-confirm="Publish this article?"
                        data-confirm-action="Publish"
                        data-confirm-tone="danger">                          */

    var pending = null;

    function buildDialog() {
        var overlay = document.createElement("div");

        overlay.className = "tgv-dialog-overlay";
        overlay.innerHTML =
            '<div class="tgv-dialog" role="dialog" aria-modal="true" aria-labelledby="tgv-dialog-title">' +
                '<h2 class="tgv-dialog__title" id="tgv-dialog-title"></h2>' +
                '<p class="tgv-dialog__text"></p>' +
                '<div class="tgv-dialog__actions">' +
                    '<button type="button" class="tgv-dialog__cancel">Cancel</button>' +
                    '<button type="button" class="tgv-dialog__ok"></button>' +
                '</div>' +
            '</div>';

        document.body.appendChild(overlay);

        overlay.addEventListener("click", function (event) {
            if (event.target === overlay || event.target.closest(".tgv-dialog__cancel")) {
                close();
            }
        });

        overlay.querySelector(".tgv-dialog__ok").addEventListener("click", function () {
            var target = pending;

            close();

            if (!target) {
                return;
            }

            // Let the original element do its thing without re-asking.
            target.dataset.confirmed = "true";

            if (target.tagName === "A") {
                window.location.href = target.href;
            } else if (target.form) {
                // Preserve the button's name/value for the server.
                if (target.name) {
                    var hidden = document.createElement("input");

                    hidden.type = "hidden";
                    hidden.name = target.name;
                    hidden.value = target.value;

                    target.form.appendChild(hidden);
                }

                window.tgvSkipDirtyGuard = true;
                target.form.submit();
            } else {
                target.click();
            }
        });

        return overlay;
    }

    var overlayEl = null;

    function close() {
        if (overlayEl) {
            overlayEl.classList.remove("is-open");
        }

        pending = null;
    }

    function ask(element) {
        overlayEl = overlayEl || buildDialog();

        var tone = element.dataset.confirmTone || "danger";

        overlayEl.querySelector(".tgv-dialog__title").textContent =
            element.dataset.confirmTitle || "Are you sure?";

        overlayEl.querySelector(".tgv-dialog__text").textContent =
            element.dataset.confirm || "";

        var ok = overlayEl.querySelector(".tgv-dialog__ok");

        ok.textContent = element.dataset.confirmAction || "Confirm";
        ok.className = "tgv-dialog__ok tgv-dialog__ok--" + tone;

        pending = element;

        overlayEl.classList.add("is-open");
        ok.focus();
    }

    document.addEventListener("click", function (event) {
        var trigger = event.target.closest("[data-confirm]");

        if (!trigger || trigger.dataset.confirmed === "true") {
            return;
        }

        event.preventDefault();
        event.stopPropagation();

        ask(trigger);
    }, true);

    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape") {
            close();
        }
    });


    /* ---------- 3. DIRTY GUARD + Ctrl/Cmd+S ------------------------------ */

    function initDirtyGuard() {
        var form = document.querySelector("[data-dirty-guard]");

        if (!form) {
            return;
        }

        var dirty = false;

        var markDirty = function () { dirty = true; };

        form.addEventListener("input", markDirty);
        form.addEventListener("change", markDirty);

        // TinyMCE lives in an iframe, so it needs its own hook.
        if (window.tinymce) {
            window.tinymce.on("AddEditor", function (event) {
                event.editor.on("Dirty", markDirty);
            });
        }

        form.addEventListener("submit", function () {
            dirty = false;
        });

        window.addEventListener("beforeunload", function (event) {
            if (!dirty || window.tgvSkipDirtyGuard) {
                return;
            }

            event.preventDefault();
            event.returnValue = "";
        });

        // Ctrl/Cmd + S saves instead of opening the browser's save dialog.
        document.addEventListener("keydown", function (event) {
            if (!(event.ctrlKey || event.metaKey) || event.key.toLowerCase() !== "s") {
                return;
            }

            event.preventDefault();

            var save =
                form.querySelector("[data-save-button]") ||
                form.querySelector("button[type='submit']");

            if (save) {
                if (window.tinymce) {
                    window.tinymce.triggerSave();
                }

                dirty = false;
                save.click();
            }
        });
    }


    /* ---------- 4. BULK ACTIONS ------------------------------------------ */

    function initBulk() {
        var bar = document.querySelector("[data-bulk-bar]");

        if (!bar) {
            return;
        }

        var master = document.querySelector("[data-bulk-master]");
        var count = bar.querySelector("[data-bulk-count]");

        var boxes = function () {
            return Array.prototype.slice.call(
                document.querySelectorAll("[data-bulk-item]"));
        };

        var selected = function () {
            return boxes().filter(function (b) { return b.checked; });
        };

        var sync = function () {
            var chosen = selected();

            count.textContent = chosen.length;
            bar.classList.toggle("is-visible", chosen.length > 0);

            if (master) {
                master.checked = chosen.length > 0 && chosen.length === boxes().length;
                master.indeterminate = chosen.length > 0 && chosen.length < boxes().length;
            }

            // Keep the hidden inputs in the bulk form in sync.
            var holder = bar.querySelector("[data-bulk-ids]");

            holder.innerHTML = "";

            chosen.forEach(function (box) {
                var input = document.createElement("input");

                input.type = "hidden";
                input.name = "ids";
                input.value = box.value;

                holder.appendChild(input);
            });

            chosen.forEach(function (box) {
                box.closest("tr").classList.add("is-selected");
            });

            boxes().filter(function (b) { return !b.checked; })
                .forEach(function (box) {
                    box.closest("tr").classList.remove("is-selected");
                });
        };

        if (master) {
            master.addEventListener("change", function () {
                boxes().forEach(function (box) { box.checked = master.checked; });
                sync();
            });
        }

        document.addEventListener("change", function (event) {
            if (event.target.matches("[data-bulk-item]")) {
                sync();
            }
        });

        var clear = bar.querySelector("[data-bulk-clear]");

        if (clear) {
            clear.addEventListener("click", function () {
                boxes().forEach(function (box) { box.checked = false; });
                sync();
            });
        }

        sync();
    }


    /* ---------- BOOT ------------------------------------------------------ */

    function boot() {
        initToasts();
        initDirtyGuard();
        initBulk();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", boot);
    } else {
        boot();
    }
})();
