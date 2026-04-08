# Professional Jewelry Website Homepage

## Overview
This is a professional, elegant homepage designed for a luxury jewelry website. The homepage features 5 main jewelry categories and is built with modern web technologies.

## Features

### 🎨 Design Highlights
- **Elegant & Professional**: Sophisticated design with a luxury aesthetic
- **5 Jewelry Categories**:
  1. **Rings** - Engagement rings, wedding bands, and statement pieces
  2. **Necklaces** - Elegant pendants and statement necklaces
  3. **Bracelets** - Delicate chains and bold bangles
  4. **Earrings** - Studs to dramatic drops
  5. **Watches** - Luxury timepieces

### ✨ Key Sections
1. **Hero Section** - Eye-catching introduction with call-to-action buttons
2. **Collections Grid** - Beautiful showcase of all 5 jewelry categories
3. **Featured Section** - Signature collection highlights
4. **Testimonials** - Customer reviews and social proof
5. **Call-to-Action** - Appointment booking and browse options
6. **Footer** - Complete site navigation and contact information

### 📱 Responsive Design
- Fully responsive layout that works on all devices
- Mobile-first approach with breakpoints at 1024px, 768px, and 480px
- Touch-friendly navigation and buttons

### 🎭 Interactive Features
- Smooth scrolling navigation
- Hover effects on collection cards
- Animated scroll-in effects
- Parallax hero section
- Interactive buttons and links

## Files Included

### 1. HTML Version
**File**: `jewelry-homepage.html`
- Standalone HTML file
- Can be used as-is or integrated into any CMS
- Complete with all sections and content

### 2. PHP/WordPress Version
**File**: `jewelry-homepage.php`
- WordPress-compatible template
- Can be used as a WordPress page template
- Works standalone as well
- Includes WordPress functions with fallbacks

### 3. CSS Styling
**File**: `css/jewelry-style.css`
- Complete styling for all components
- CSS custom properties (variables) for easy customization
- Responsive media queries
- Smooth animations and transitions

### 4. JavaScript
**File**: `js/jewelry-script.js`
- Interactive features and enhancements
- Smooth scroll navigation
- Scroll animations
- Header effects
- Extensible for future features

## Installation & Usage

### For Standalone Use
1. Copy all files to your web server:
   - `jewelry-homepage.html` (or `jewelry-homepage.php`)
   - `css/jewelry-style.css`
   - `js/jewelry-script.js`

2. Open `jewelry-homepage.html` in a web browser

3. Customize content as needed:
   - Update text content in the HTML/PHP file
   - Modify colors in the CSS `:root` variables
   - Add your own images by replacing the SVG placeholders

### For WordPress Integration
1. Copy files to your WordPress theme directory:
   ```
   wp-content/themes/your-theme/
   ├── jewelry-homepage.php
   ├── css/jewelry-style.css
   └── js/jewelry-script.js
   ```

2. Update the file paths in `jewelry-homepage.php` to match your theme structure

3. Create a new page in WordPress and select "Jewelry Homepage" as the template

4. Alternatively, use this as your front page template by renaming it to `front-page.php`

### For ASP.NET Integration (Current Repository)
The files are placed in `TmsSystem/wwwroot/` for easy access:
- Access HTML version: `/jewelry-homepage.html`
- Access PHP version: `/jewelry-homepage.php` (if PHP is configured)
- Static assets are in `/css/` and `/js/` folders

## Customization Guide

### Colors
Edit CSS variables in `jewelry-style.css`:
```css
:root {
    --primary-color: #d4af37;  /* Gold - change to your brand color */
    --secondary-color: #1a1a1a; /* Deep Black */
    --text-color: #333;
    --bg-light: #faf9f7;
}
```

### Fonts
Current fonts (Google Fonts):
- **Headings**: Playfair Display
- **Body**: Montserrat

To change fonts, update the Google Fonts link in the HTML/PHP file and the CSS variables.

### Content
Edit directly in the HTML or PHP file:
- Hero section text
- Category descriptions
- Testimonials
- Contact information
- Footer content

### Images
Replace the SVG placeholders with real images:
1. Remove the `<div class="image-placeholder">` with SVG
2. Add `<img src="your-image.jpg" alt="description">`
3. Ensure images are optimized for web use

## Browser Support
- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)
- Mobile browsers (iOS Safari, Chrome Mobile)

## Performance
- Lightweight design (~41KB total for CSS + JS)
- Uses CSS Grid and Flexbox for layout
- Optimized animations
- No external dependencies (except Google Fonts)
- Fast load times

## Accessibility
- Semantic HTML5 markup
- ARIA labels on interactive elements
- Keyboard navigation support
- High contrast color ratios
- Screen reader friendly

## Future Enhancements
The code is structured to easily add:
- Image galleries for each category
- Shopping cart functionality
- Product search
- Mobile hamburger menu
- Product filtering
- Lightbox/modal for images
- Form validation for contact/appointment forms
- Integration with e-commerce platforms

## Support
For questions or customization help, refer to the code comments or create an issue in the repository.

## License
This code is provided as part of the TmsSystem project.

---

**Created**: January 2026  
**Version**: 1.0  
**Last Updated**: January 16, 2026
