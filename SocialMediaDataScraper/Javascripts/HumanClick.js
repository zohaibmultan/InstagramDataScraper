function simulateHumanClick(element, options = {}) {
    // Default options for click simulation
    const defaults = {
        x: element.getBoundingClientRect().left + (Math.random() * element.offsetWidth),
        y: element.getBoundingClientRect().top + (Math.random() * element.offsetHeight),
        button: 'left', // 'left', 'right', or 'middle'
        bubbles: true,
        cancelable: true,
        composed: true
    };

    const opts = { ...defaults, ...options };

    // Create and dispatch mouse events to simulate human-like interaction
    const mouseEvents = [
        new MouseEvent('mouseover', {
            view: window,
            bubbles: true,
            cancelable: true,
            clientX: opts.x,
            clientY: opts.y
        }),
        new MouseEvent('mousemove', {
            view: window,
            bubbles: true,
            cancelable: true,
            clientX: opts.x + (Math.random() * 5 - 2.5), // Small random movement
            clientY: opts.y + (Math.random() * 5 - 2.5)
        }),
        new MouseEvent('mousedown', {
            view: window,
            bubbles: true,
            cancelable: true,
            clientX: opts.x,
            clientY: opts.y,
            button: opts.button === 'left' ? 0 : opts.button === 'right' ? 2 : 1
        }),
        new MouseEvent('click', {
            view: window,
            bubbles: opts.bubbles,
            cancelable: opts.cancelable,
            clientX: opts.x,
            clientY: opts.y,
            button: opts.button === 'left' ? 0 : opts.button === 'right' ? 2 : 1,
            composed: opts.composed
        }),
        new MouseEvent('mouseup', {
            view: window,
            bubbles: true,
            cancelable: true,
            clientX: opts.x,
            clientY: opts.y,
            button: opts.button === 'left' ? 0 : opts.button === 'right' ? 2 : 1
        }),
        new MouseEvent('mouseout', {
            view: window,
            bubbles: true,
            cancelable: true,
            clientX: opts.x,
            clientY: opts.y
        })
    ];

    // Simulate slight delay between events to mimic human behavior
    mouseEvents.forEach((event, index) => {
        setTimeout(() => {
            element.dispatchEvent(event);
        }, index * (10 + Math.random() * 20)); // Random delay between 10-30ms
    });

    // Also dispatch a generic Event for broader compatibility
    const clickEvent = new Event('click', {
        bubbles: opts.bubbles,
        cancelable: opts.cancelable,
        composed: opts.composed
    });
    setTimeout(() => {
        element.dispatchEvent(clickEvent);
    }, mouseEvents.length * 20);
}

// Function to select a random clickable element
function getRandomClickableElement() {
    const clickableSelectors = [
        'a[href]',
        'button:not([disabled])',
        'input[type="button"]:not([disabled])',
        'input[type="submit"]:not([disabled])',
        '[onclick]',
        '[role="button"]:not([disabled])'
    ];
    const elements = document.querySelectorAll(clickableSelectors.join(','));
    const visibleElements = Array.from(elements).filter(el => {
        const rect = el.getBoundingClientRect();
        return rect.width > 0 && rect.height > 0 && window.getComputedStyle(el).display !== 'none';
    });
    return visibleElements[Math.floor(Math.random() * visibleElements.length)];
}

// Automatically simulate a click on a random element
function autoClick() {
    const element = getRandomClickableElement();
    if (element) {
        simulateHumanClick(element);
        console.log('Simulated click on:', element);
    } else {
        console.log('No clickable elements found on the page.');
    }
}

// Execute auto-click immediately
autoClick();