if (typeof scrollDownAllDivOnPage !== 'function') {
    function scrollDownAllDivOnPage(wait) {
        function smoothScrollToBottom(el, duration) {
            const start = el.scrollTop || window.scrollY;
            const end = el.scrollHeight - el.clientHeight;
            const change = end - start;
            const startTime = performance.now();

            function animateScroll(currentTime) {
                const elapsed = currentTime - startTime;
                const progress = Math.min(elapsed / duration, 1);
                const easeInOut = progress < 0.5
                    ? 2 * progress * progress
                    : -1 + (4 - 2 * progress) * progress;

                const scrollPosition = start + change * easeInOut;

                if (el === window || el === document.body || el === document.documentElement) {
                    window.scrollTo(0, scrollPosition);
                } else {
                    el.scrollTop = scrollPosition;
                }

                if (elapsed < duration) {
                    requestAnimationFrame(animateScroll);
                }
            }

            requestAnimationFrame(animateScroll);
        }

        const duration = wait * 1000;

        smoothScrollToBottom(window, duration);

        const allElements = document.querySelectorAll('*');

        allElements.forEach(el => {
            const style = window.getComputedStyle(el);
            const overflowY = style.overflowY;
            const isScrollable = (overflowY === 'scroll' || overflowY === 'auto') && el.scrollHeight > el.clientHeight;

            if (isScrollable) {
                smoothScrollToBottom(el, duration);
            }
        });
    }
}