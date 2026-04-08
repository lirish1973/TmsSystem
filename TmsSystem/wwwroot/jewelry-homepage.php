<?php
/**
 * Template Name: Jewelry Homepage
 * Description: Professional homepage for jewelry website with 5 categories
 * 
 * This template can be used as a WordPress page template or as a standalone PHP file
 */

// WordPress compatibility - load header if in WordPress environment
if (function_exists('get_header')) {
    get_header();
} else {
    // Standalone header
    ?>
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>Luxe Jewelry - Timeless Elegance</title>
        <link rel="stylesheet" href="<?php echo htmlspecialchars(get_stylesheet_directory_uri() ?? 'css'); ?>/jewelry-style.css">
        <link rel="preconnect" href="https://fonts.googleapis.com">
        <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
        <link href="https://fonts.googleapis.com/css2?family=Playfair+Display:wght@400;600;700&family=Montserrat:wght@300;400;500;600&display=swap" rel="stylesheet">
    </head>
    <body <?php if (function_exists('body_class')) { body_class(); } ?>>
    <?php
}
?>

<!-- Header & Navigation -->
<header class="header">
    <nav class="nav-container">
        <div class="logo">
            <h1>LUXE</h1>
            <span class="logo-subtitle">JEWELRY</span>
        </div>
        <?php
        // WordPress menu support
        if (function_exists('wp_nav_menu')) {
            wp_nav_menu(array(
                'theme_location' => 'primary',
                'container' => false,
                'menu_class' => 'nav-menu',
                'fallback_cb' => function() {
                    echo '<ul class="nav-menu">
                        <li><a href="#home">Home</a></li>
                        <li><a href="#collections">Collections</a></li>
                        <li><a href="#about">About</a></li>
                        <li><a href="#contact">Contact</a></li>
                    </ul>';
                }
            ));
        } else {
            echo '<ul class="nav-menu">
                <li><a href="#home">Home</a></li>
                <li><a href="#collections">Collections</a></li>
                <li><a href="#about">About</a></li>
                <li><a href="#contact">Contact</a></li>
            </ul>';
        }
        ?>
        <div class="nav-icons">
            <button class="icon-btn" aria-label="Search">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <circle cx="11" cy="11" r="8"></circle>
                    <path d="m21 21-4.35-4.35"></path>
                </svg>
            </button>
            <button class="icon-btn" aria-label="Cart">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <circle cx="9" cy="21" r="1"></circle>
                    <circle cx="20" cy="21" r="1"></circle>
                    <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
                </svg>
            </button>
        </div>
    </nav>
</header>

<!-- Hero Section -->
<section class="hero" id="home">
    <div class="hero-content">
        <div class="hero-text">
            <p class="hero-subtitle">Timeless Elegance</p>
            <h2 class="hero-title">Discover Your<br>Perfect Piece</h2>
            <p class="hero-description">Handcrafted luxury jewelry that tells your unique story. Each piece is a masterpiece of artistry and precision.</p>
            <div class="hero-buttons">
                <a href="#collections" class="btn btn-primary">Explore Collections</a>
                <a href="#about" class="btn btn-secondary">Our Story</a>
            </div>
        </div>
        <div class="hero-image">
            <div class="hero-image-placeholder">
                <svg width="100%" height="100%" viewBox="0 0 400 500">
                    <rect width="400" height="500" fill="#f5f5f5"/>
                    <circle cx="200" cy="250" r="80" fill="#d4af37" opacity="0.3"/>
                    <text x="200" y="260" font-family="Playfair Display" font-size="24" fill="#666" text-anchor="middle">Elegant Jewelry</text>
                </svg>
            </div>
        </div>
    </div>
</section>

<?php
// Define jewelry categories
$jewelry_categories = array(
    array(
        'name' => 'Rings',
        'description' => 'Engagement rings, wedding bands, and statement pieces that symbolize eternal love',
        'icon' => '<circle cx="150" cy="175" r="60" fill="none" stroke="#d4af37" stroke-width="3"/><circle cx="150" cy="145" r="15" fill="#d4af37"/>',
        'link' => '#rings'
    ),
    array(
        'name' => 'Necklaces',
        'description' => 'Elegant pendants and statement necklaces that grace your neckline with sophistication',
        'icon' => '<path d="M 80 100 Q 150 150 220 100" fill="none" stroke="#d4af37" stroke-width="3"/><circle cx="150" cy="160" r="20" fill="#d4af37"/>',
        'link' => '#necklaces'
    ),
    array(
        'name' => 'Bracelets',
        'description' => 'Delicate chains and bold bangles that add grace to every gesture',
        'icon' => '<ellipse cx="150" cy="175" rx="70" ry="40" fill="none" stroke="#d4af37" stroke-width="3"/><circle cx="150" cy="135" r="8" fill="#d4af37"/><circle cx="150" cy="215" r="8" fill="#d4af37"/>',
        'link' => '#bracelets'
    ),
    array(
        'name' => 'Earrings',
        'description' => 'From subtle studs to dramatic drops, earrings that frame your beauty',
        'icon' => '<circle cx="120" cy="150" r="15" fill="#d4af37"/><path d="M 120 165 L 120 200" stroke="#d4af37" stroke-width="2"/><circle cx="180" cy="150" r="15" fill="#d4af37"/><path d="M 180 165 L 180 200" stroke="#d4af37" stroke-width="2"/>',
        'link' => '#earrings'
    ),
    array(
        'name' => 'Watches',
        'description' => 'Luxury timepieces that blend precision craftsmanship with timeless design',
        'icon' => '<rect x="100" y="140" width="100" height="80" rx="5" fill="none" stroke="#d4af37" stroke-width="3"/><circle cx="150" cy="180" r="30" fill="none" stroke="#d4af37" stroke-width="2"/><line x1="150" y1="180" x2="150" y2="160" stroke="#d4af37" stroke-width="2"/><line x1="150" y1="180" x2="165" y2="180" stroke="#d4af37" stroke-width="2"/>',
        'link' => '#watches'
    )
);
?>

<!-- Collections Section -->
<section class="collections" id="collections">
    <div class="container">
        <div class="section-header">
            <p class="section-subtitle">Our Collections</p>
            <h2 class="section-title">Exquisite Jewelry Categories</h2>
            <p class="section-description">Explore our carefully curated collections, each piece crafted with passion and precision</p>
        </div>

        <div class="collections-grid">
            <?php foreach ($jewelry_categories as $category): ?>
            <div class="collection-card">
                <div class="collection-image">
                    <div class="image-placeholder">
                        <svg width="100%" height="100%" viewBox="0 0 300 350">
                            <rect width="300" height="350" fill="#faf9f7"/>
                            <?php echo $category['icon']; ?>
                        </svg>
                    </div>
                    <div class="collection-overlay">
                        <a href="<?php echo esc_url($category['link']); ?>" class="overlay-btn">View Collection</a>
                    </div>
                </div>
                <div class="collection-info">
                    <h3 class="collection-name"><?php echo esc_html($category['name']); ?></h3>
                    <p class="collection-description"><?php echo esc_html($category['description']); ?></p>
                    <a href="<?php echo esc_url($category['link']); ?>" class="collection-link">Explore <?php echo esc_html($category['name']); ?> →</a>
                </div>
            </div>
            <?php endforeach; ?>
        </div>
    </div>
</section>

<!-- Featured Section -->
<section class="featured">
    <div class="container">
        <div class="featured-content">
            <div class="featured-image">
                <div class="image-placeholder">
                    <svg width="100%" height="100%" viewBox="0 0 500 500">
                        <rect width="500" height="500" fill="#f5f5f5"/>
                        <circle cx="250" cy="250" r="100" fill="none" stroke="#d4af37" stroke-width="4"/>
                        <circle cx="250" cy="200" r="30" fill="#d4af37"/>
                        <text x="250" y="330" font-family="Playfair Display" font-size="20" fill="#666" text-anchor="middle">Signature Collection</text>
                    </svg>
                </div>
            </div>
            <div class="featured-text">
                <p class="featured-subtitle">Signature Collection</p>
                <h2 class="featured-title">The Art of Excellence</h2>
                <p class="featured-description">Our signature collection represents the pinnacle of jewelry craftsmanship. Each piece is meticulously designed and handcrafted by master artisans using only the finest materials. From conflict-free diamonds to ethically sourced precious metals, we ensure that every creation meets our exacting standards of quality and beauty.</p>
                <ul class="featured-list">
                    <li>✦ Handcrafted by Master Artisans</li>
                    <li>✦ Ethically Sourced Materials</li>
                    <li>✦ Lifetime Quality Guarantee</li>
                    <li>✦ Custom Design Services</li>
                </ul>
                <a href="#signature" class="btn btn-primary">Discover More</a>
            </div>
        </div>
    </div>
</section>

<?php
// Testimonials data
$testimonials = array(
    array(
        'text' => 'The most beautiful engagement ring I\'ve ever seen. The craftsmanship is impeccable and the service was exceptional.',
        'author' => 'Sarah M.'
    ),
    array(
        'text' => 'Luxe Jewelry transformed my grandmother\'s heirloom into a stunning modern piece. Their attention to detail is remarkable.',
        'author' => 'David R.'
    ),
    array(
        'text' => 'From design consultation to the final piece, every step was perfect. This is where luxury meets personalization.',
        'author' => 'Maria L.'
    )
);
?>

<!-- Testimonials Section -->
<section class="testimonials">
    <div class="container">
        <div class="section-header">
            <p class="section-subtitle">Testimonials</p>
            <h2 class="section-title">What Our Clients Say</h2>
        </div>
        <div class="testimonials-grid">
            <?php foreach ($testimonials as $testimonial): ?>
            <div class="testimonial-card">
                <div class="stars">★★★★★</div>
                <p class="testimonial-text">"<?php echo esc_html($testimonial['text']); ?>"</p>
                <p class="testimonial-author">— <?php echo esc_html($testimonial['author']); ?></p>
            </div>
            <?php endforeach; ?>
        </div>
    </div>
</section>

<!-- CTA Section -->
<section class="cta">
    <div class="container">
        <div class="cta-content">
            <h2 class="cta-title">Begin Your Journey</h2>
            <p class="cta-description">Visit our showroom or schedule a private consultation with our jewelry specialists</p>
            <div class="cta-buttons">
                <a href="#contact" class="btn btn-light">Book Appointment</a>
                <a href="#collections" class="btn btn-outline">Browse Collections</a>
            </div>
        </div>
    </div>
</section>

<!-- Footer -->
<footer class="footer">
    <div class="container">
        <div class="footer-grid">
            <div class="footer-col">
                <div class="footer-logo">
                    <h3>LUXE</h3>
                    <span>JEWELRY</span>
                </div>
                <p class="footer-description">Crafting timeless elegance since 1950. Every piece tells a story of passion, precision, and perfection.</p>
            </div>
            <div class="footer-col">
                <h4>Collections</h4>
                <ul class="footer-links">
                    <?php foreach ($jewelry_categories as $category): ?>
                    <li><a href="<?php echo esc_url($category['link']); ?>"><?php echo esc_html($category['name']); ?></a></li>
                    <?php endforeach; ?>
                </ul>
            </div>
            <div class="footer-col">
                <h4>About</h4>
                <ul class="footer-links">
                    <li><a href="#about">Our Story</a></li>
                    <li><a href="#craftsmanship">Craftsmanship</a></li>
                    <li><a href="#sustainability">Sustainability</a></li>
                    <li><a href="#warranty">Warranty</a></li>
                </ul>
            </div>
            <div class="footer-col">
                <h4>Contact</h4>
                <ul class="footer-links">
                    <li>123 Luxury Lane</li>
                    <li>New York, NY 10001</li>
                    <li>Phone: (555) 123-4567</li>
                    <li>Email: info@luxejewelry.com</li>
                </ul>
            </div>
        </div>
        <div class="footer-bottom">
            <p>&copy; <?php echo date('Y'); ?> Luxe Jewelry. All rights reserved.</p>
            <div class="social-links">
                <a href="#" aria-label="Facebook">FB</a>
                <a href="#" aria-label="Instagram">IG</a>
                <a href="#" aria-label="Pinterest">PIN</a>
            </div>
        </div>
    </div>
</footer>

<?php
// WordPress compatibility - load footer if in WordPress environment
if (function_exists('get_footer')) {
    get_footer();
} else {
    // Standalone footer
    ?>
    <script src="<?php echo htmlspecialchars(get_stylesheet_directory_uri() ?? 'js'); ?>/jewelry-script.js"></script>
    </body>
    </html>
    <?php
}
?>
