/**
 * Jewelry Homepage - Interactive Features
 * Professional Jewelry Website JavaScript
 */

(function() {
    'use strict';

    // ================================
    // Smooth Scroll for Navigation
    // ================================
    const initSmoothScroll = () => {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                const href = this.getAttribute('href');
                
                // Skip if href is just "#"
                if (href === '#') {
                    e.preventDefault();
                    return;
                }
                
                const target = document.querySelector(href);
                if (target) {
                    e.preventDefault();
                    const headerOffset = 80;
                    const elementPosition = target.getBoundingClientRect().top;
                    const offsetPosition = elementPosition + window.pageYOffset - headerOffset;

                    window.scrollTo({
                        top: offsetPosition,
                        behavior: 'smooth'
                    });
                }
            });
        });
    };

    // ================================
    // Header Scroll Effect
    // ================================
    const initHeaderScroll = () => {
        const header = document.querySelector('.header');
        let lastScroll = 0;

        window.addEventListener('scroll', () => {
            const currentScroll = window.pageYOffset;

            if (currentScroll > 100) {
                header.style.boxShadow = '0 4px 16px rgba(0,0,0,0.1)';
                header.style.padding = '0.75rem 0';
            } else {
                header.style.boxShadow = '0 2px 8px rgba(0,0,0,0.08)';
                header.style.padding = '1rem 0';
            }

            lastScroll = currentScroll;
        });
    };

    // ================================
    // Intersection Observer for Animations
    // ================================
    const initScrollAnimations = () => {
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -100px 0px'
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                }
            });
        }, observerOptions);

        // Observe elements
        const animatedElements = document.querySelectorAll('.collection-card, .testimonial-card, .featured-content');
        animatedElements.forEach(el => {
            el.style.opacity = '0';
            el.style.transform = 'translateY(30px)';
            el.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
            observer.observe(el);
        });
    };

    // ================================
    // Mobile Menu Toggle (for future enhancement)
    // ================================
    const initMobileMenu = () => {
        // Placeholder for mobile menu functionality
        // Can be expanded to include hamburger menu for mobile devices
        const navMenu = document.querySelector('.nav-menu');
        
        if (window.innerWidth <= 768) {
            console.log('Mobile view detected - menu hidden');
        }
    };

    // ================================
    // Image Lazy Loading Enhancement
    // ================================
    const initLazyLoading = () => {
        const images = document.querySelectorAll('.collection-image, .hero-image, .featured-image');
        
        const imageObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    img.classList.add('loaded');
                    observer.unobserve(img);
                }
            });
        });

        images.forEach(img => imageObserver.observe(img));
    };

    // ================================
    // Form Validation (for future contact forms)
    // ================================
    const initFormValidation = () => {
        const forms = document.querySelectorAll('form');
        
        forms.forEach(form => {
            form.addEventListener('submit', (e) => {
                e.preventDefault();
                // Add validation logic here
                console.log('Form submitted');
            });
        });
    };

    // ================================
    // Search Functionality Placeholder
    // ================================
    const initSearch = () => {
        const searchBtn = document.querySelector('.icon-btn[aria-label="Search"]');
        
        if (searchBtn) {
            searchBtn.addEventListener('click', () => {
                console.log('Search clicked - implement search overlay');
                // Future: Show search overlay/modal
            });
        }
    };

    // ================================
    // Shopping Cart Placeholder
    // ================================
    const initCart = () => {
        const cartBtn = document.querySelector('.icon-btn[aria-label="Cart"]');
        
        if (cartBtn) {
            cartBtn.addEventListener('click', () => {
                console.log('Cart clicked - implement cart sidebar');
                // Future: Show cart sidebar
            });
        }
    };

    // ================================
    // Parallax Effect for Hero Section
    // ================================
    const initParallax = () => {
        const hero = document.querySelector('.hero');
        
        if (hero) {
            window.addEventListener('scroll', () => {
                const scrolled = window.pageYOffset;
                const parallaxSpeed = 0.5;
                
                if (scrolled < window.innerHeight) {
                    hero.style.transform = `translateY(${scrolled * parallaxSpeed}px)`;
                }
            });
        }
    };

    // ================================
    // Collection Card Hover Enhancement
    // ================================
    const initCollectionCards = () => {
        const cards = document.querySelectorAll('.collection-card');
        
        cards.forEach(card => {
            card.addEventListener('mouseenter', function() {
                this.style.transition = 'all 0.4s cubic-bezier(0.4, 0, 0.2, 1)';
            });
        });
    };

    // ================================
    // Testimonials Slider (Simple Version)
    // ================================
    const initTestimonials = () => {
        const testimonials = document.querySelectorAll('.testimonial-card');
        let currentIndex = 0;

        // Add active class to first testimonial
        if (testimonials.length > 0) {
            testimonials[0].classList.add('active');
        }

        // Auto-rotate testimonials (optional feature)
        // setInterval(() => {
        //     testimonials[currentIndex].classList.remove('active');
        //     currentIndex = (currentIndex + 1) % testimonials.length;
        //     testimonials[currentIndex].classList.add('active');
        // }, 5000);
    };

    // ================================
    // Initialize All Features
    // ================================
    const init = () => {
        // Wait for DOM to be ready
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => {
                initSmoothScroll();
                initHeaderScroll();
                initScrollAnimations();
                initMobileMenu();
                initLazyLoading();
                initFormValidation();
                initSearch();
                initCart();
                initParallax();
                initCollectionCards();
                initTestimonials();
                
                console.log('Luxe Jewelry homepage initialized successfully');
            });
        } else {
            initSmoothScroll();
            initHeaderScroll();
            initScrollAnimations();
            initMobileMenu();
            initLazyLoading();
            initFormValidation();
            initSearch();
            initCart();
            initParallax();
            initCollectionCards();
            initTestimonials();
            
            console.log('Luxe Jewelry homepage initialized successfully');
        }
    };

    // Start initialization
    init();

    // ================================
    // Utility Functions
    // ================================
    
    // Debounce function for performance
    const debounce = (func, wait) => {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    };

    // Throttle function for scroll events
    const throttle = (func, limit) => {
        let inThrottle;
        return function() {
            const args = arguments;
            const context = this;
            if (!inThrottle) {
                func.apply(context, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        };
    };

})();
