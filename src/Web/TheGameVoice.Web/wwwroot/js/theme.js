/* =========================================================================
   TheGameVoice — theme switcher (dark / light)
   -------------------------------------------------------------------------
   The theme is applied as data-theme="dark|light" on <html>.

   The FIRST paint is handled by a tiny inline script in the layout <head>
   (see _Layout.cshtml / _AdminLayout.cshtml) so the page never flashes
   white before this file loads. This file only handles clicks + persistence.
   ========================================================================= */

(function () {
    "use strict";

    var STORAGE_KEY = "tgv-theme";

    function currentTheme() {
        return document.documentElement.getAttribute("data-theme") === "dark"
            ? "dark"
            : "light";
    }

    function syncButtons() {
        var isDark = currentTheme() === "dark";

        var label = isDark
            ? "Switch to light mode"
            : "Switch to dark mode";

        document
            .querySelectorAll("[data-theme-toggle]")
            .forEach(function (button) {
                button.setAttribute("aria-pressed", isDark ? "true" : "false");
                button.setAttribute("aria-label", label);
                button.setAttribute("title", label);

                var text = button.querySelector("[data-theme-label]");

                if (text) {
                    text.textContent = isDark ? "Light mode" : "Dark mode";
                }
            });
    }

    function applyTheme(theme) {
        var root = document.documentElement;

        // Animate only during the switch itself.
        root.classList.add("tgv-theme-switching");

        root.setAttribute("data-theme", theme);

        try {
            localStorage.setItem(STORAGE_KEY, theme);
        } catch (e) {
            /* private mode — the choice just won't persist */
        }

        syncButtons();

        window.setTimeout(function () {
            root.classList.remove("tgv-theme-switching");
        }, 260);
    }

    // One delegated listener: works for every toggle on the page, including
    // markup rendered later (mobile menu, partials, etc).
    document.addEventListener("click", function (event) {
        var button = event.target.closest("[data-theme-toggle]");

        if (!button) {
            return;
        }

        event.preventDefault();

        applyTheme(currentTheme() === "dark" ? "light" : "dark");
    });

    // Follow the OS only while the reader has not made an explicit choice.
    if (window.matchMedia) {
        var query = window.matchMedia("(prefers-color-scheme: dark)");

        var onChange = function (event) {
            var stored = null;

            try {
                stored = localStorage.getItem(STORAGE_KEY);
            } catch (e) { }

            if (!stored) {
                document.documentElement.setAttribute(
                    "data-theme",
                    event.matches ? "dark" : "light");

                syncButtons();
            }
        };

        if (query.addEventListener) {
            query.addEventListener("change", onChange);
        } else if (query.addListener) {
            query.addListener(onChange);
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", syncButtons);
    } else {
        syncButtons();
    }
})();
