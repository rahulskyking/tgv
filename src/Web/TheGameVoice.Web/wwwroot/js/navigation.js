(() => {
    'use strict';

    const header = document.querySelector('[data-site-header]');
    const categories = document.querySelector('[data-category-nav]');
    const openButton = document.getElementById('mobile-menu-button');
    const closeButton = document.getElementById('mobile-menu-close');
    const overlay = document.getElementById('mobile-menu-overlay');
    const desktop = window.matchMedia('(min-width: 1024px)');
    let frame;
    let previousWidth = -1;

    function fitCategories() {
        if (!categories || !desktop.matches) return;
        const width = categories.clientWidth;
        if (!width) return;
        categories.classList.remove('tgv-nav-categories--compact');
        const links = [...categories.children];
        const gap = parseFloat(getComputedStyle(categories).columnGap) || 0;
        const needed = links.reduce((sum, link) => sum + link.getBoundingClientRect().width, 0)
            + Math.max(0, links.length - 1) * gap;
        // Never keep shrinking to force one line. The compact style bottoms out
        // at .75rem; flex wrapping is the deliberate fallback for larger menus.
        categories.classList.toggle('tgv-nav-categories--compact', needed > width + 1);
    }

    function scheduleFit() {
        cancelAnimationFrame(frame);
        frame = requestAnimationFrame(fitCategories);
    }

    if ('ResizeObserver' in window) {
        if (categories) {
            new ResizeObserver(entries => {
                const width = entries[0].contentRect.width;
                // Font/height changes must not create an observer feedback loop.
                if (Math.abs(width - previousWidth) < 0.5) return;
                previousWidth = width;
                scheduleFit();
            }).observe(categories);
        }
        if (header) {
            new ResizeObserver(() => {
                // Anchor targets must clear both rows, including wrapped ones.
                document.documentElement.style.setProperty('--tgv-header-offset', `${header.offsetHeight + 16}px`);
            }).observe(header);
        }
    }
    window.addEventListener('resize', scheduleFit);
    if (document.fonts) document.fonts.ready.then(scheduleFit);
    scheduleFit();

    if (!openButton || !closeButton || !overlay) return;
    let savedOverflow = '';

    function closeMenu(restoreFocus = true) {
        if (overlay.classList.contains('hidden')) return;
        overlay.classList.add('hidden');
        openButton.setAttribute('aria-expanded', 'false');
        document.body.style.overflow = savedOverflow;
        if (restoreFocus) openButton.focus();
    }

    openButton.addEventListener('click', () => {
        if (desktop.matches) return;
        savedOverflow = document.body.style.overflow;
        overlay.classList.remove('hidden');
        openButton.setAttribute('aria-expanded', 'true');
        document.body.style.overflow = 'hidden';
        closeButton.focus();
    });
    closeButton.addEventListener('click', () => closeMenu());
    overlay.addEventListener('click', event => {
        if (event.target.closest('a[href]')) closeMenu(false);
    });
    document.addEventListener('keydown', event => {
        if (overlay.classList.contains('hidden')) return;
        if (event.key === 'Escape') {
            event.preventDefault();
            closeMenu();
        } else if (event.key === 'Tab') {
            const focusable = [...overlay.querySelectorAll('a[href], button:not([disabled]), input:not([disabled]), [tabindex="0"]')]
                .filter(element => element.getClientRects().length);
            const first = focusable[0];
            const last = focusable[focusable.length - 1];
            if (event.shiftKey && document.activeElement === first) {
                event.preventDefault();
                last.focus();
            } else if (!event.shiftKey && document.activeElement === last) {
                event.preventDefault();
                first.focus();
            }
        }
    });
    desktop.addEventListener('change', () => {
        if (desktop.matches) closeMenu(false);
        scheduleFit();
    });
})();
