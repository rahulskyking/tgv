/* =========================================================================
   Admin off-canvas navigation (below 1024px)

   The sidebar keeps its markup; CSS turns it into a drawer and this file
   only flips a class on <body>. Above 1024px nothing here has any effect.
   ========================================================================= */

(function () {
    "use strict";

    var OPEN_CLASS = "tgv-admin-open";

    function isOpen() {
        return document.body.classList.contains(OPEN_CLASS);
    }

    function setOpen(open) {
        document.body.classList.toggle(OPEN_CLASS, open);

        document
            .querySelectorAll("[data-admin-nav-toggle]")
            .forEach(function (button) {
                button.setAttribute("aria-expanded", open ? "true" : "false");
                button.setAttribute(
                    "aria-label",
                    open ? "Close menu" : "Open menu");
            });
    }

    document.addEventListener("click", function (event) {
        if (event.target.closest("[data-admin-nav-toggle]")) {
            event.preventDefault();
            setOpen(!isOpen());

            return;
        }

        if (event.target.closest("[data-admin-nav-close]")) {
            setOpen(false);

            return;
        }

        // Tapping a nav link inside the drawer should close it.
        if (isOpen() && event.target.closest(".tgv-admin-sidebar a")) {
            setOpen(false);
        }
    });

    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape" && isOpen()) {
            setOpen(false);
        }
    });

    // Never leave the drawer state stuck when rotating to a wide viewport.
    window.addEventListener("resize", function () {
        if (window.innerWidth >= 1024 && isOpen()) {
            setOpen(false);
        }
    });
})();
