// Use relative URL so it works on any server
const API_URL = '/api/review';

// Initialize page when it loads
window.addEventListener('DOMContentLoaded', () => {
    // Get parameters from URL (sent from email link)
    const urlParams = new URLSearchParams(window.location.search);
    const productId = urlParams.get('productId');
    const serviceId = urlParams.get('serviceId');
    const accountId = urlParams.get('accountId');
    const type = urlParams.get('type');

    // Check if this is for a product or service review
    if (productId) {
        // Setup for product review
        document.getElementById('productId').value = productId;
        document.getElementById('reviewType').value = 'product';
        document.getElementById('displayItemName').textContent = 'Product';
        document.getElementById('displayItemType').textContent = 'Product';
        document.getElementById('displayItemId').textContent = productId;
        loadReviews('product', productId);
    } else if (serviceId) {
        // Setup for service review
        document.getElementById('serviceId').value = serviceId;
        document.getElementById('reviewType').value = 'service';
        document.getElementById('displayItemName').textContent = 'Service';
        document.getElementById('displayItemType').textContent = 'Service';
        document.getElementById('displayItemId').textContent = serviceId;
        loadReviews('service', serviceId);
    }

    // Set account ID if provided
    if (accountId) {
        document.getElementById('accountId').value = accountId;
    }
});

// Character counter for review text
const commentField = document.getElementById('comment');
const charCountSpan = document.getElementById('charCount');

commentField.addEventListener('input', () => {
    charCountSpan.textContent = commentField.value.length;
});

// Submit review when form is submitted
document.getElementById('reviewForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const reviewType = document.getElementById('reviewType').value;
    const comment = document.getElementById('comment').value;
    const rating = parseInt(document.getElementById('rating').value);
    const accountId = parseInt(document.getElementById('accountId').value);

    // Build review object based on type
    let review;
    let endpoint;
    
    if (reviewType === 'product') {
        const productId = parseInt(document.getElementById('productId').value);
        review = {
            productID: productId,
            accountID: accountId,
            comment: comment,
            rating: rating
        };
        endpoint = `${API_URL}/product`;
    } else {
        const serviceId = parseInt(document.getElementById('serviceId').value);
        review = {
            serviceID: serviceId,
            accountID: accountId,
            comment: comment,
            rating: rating
        };
        endpoint = `${API_URL}/service`;
    }

    // Send review to API
    try {
        const response = await fetch(endpoint, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(review)
        });

        if (response.ok) {
            // Success! Show message and clear form
            document.getElementById('message').innerHTML =
                '<div class="success">✅ Review submitted successfully! Thank you!</div>';
            document.getElementById('comment').value = '';
            document.getElementById('rating').value = '';
            charCountSpan.textContent = '0';
            
            // Reload reviews to show the new one
            if (reviewType === 'product') {
                loadReviews('product', review.productID);
            } else {
                loadReviews('service', review.serviceID);
            }
            
            // Clear message after 5 seconds
            setTimeout(() => {
                document.getElementById('message').innerHTML = '';
            }, 5000);
        } else {
            const error = await response.text();
            document.getElementById('message').innerHTML =
                `<div class="error">❌ Error: ${error}</div>`;
        }
    } catch (error) {
        console.error('Error:', error);
        document.getElementById('message').innerHTML =
            '<div class="error">❌ Failed to submit review. Please try again.</div>';
    }
});

// Load and display existing reviews
async function loadReviews(type, id) {
    try {
        const endpoint = type === 'product' 
            ? `${API_URL}/product/${id}` 
            : `${API_URL}/service/${id}`;
            
        const response = await fetch(endpoint);
        const reviews = await response.json();

        const reviewsDiv = document.getElementById('reviews');
        const statsDiv = document.getElementById('reviewStats');

        // Check if there are any reviews
        if (reviews.length === 0) {
            reviewsDiv.innerHTML = '<div class="no-reviews">No reviews yet. Be the first to review!</div>';
            statsDiv.innerHTML = '';
            return;
        }

        // Calculate average rating
        const avgRating = (reviews.reduce((sum, r) => sum + r.rating, 0) / reviews.length).toFixed(1);
        
        // Display stats
        statsDiv.innerHTML = `
            <strong>${reviews.length}</strong> ${reviews.length === 1 ? 'review' : 'reviews'} · 
            Average rating: <strong>${avgRating}</strong> ⭐ / 5
        `;

        // Display reviews (newest first)
        reviewsDiv.innerHTML = reviews
            .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
            .map(review => {
                const filledStars = '★'.repeat(review.rating);
                const emptyStars = '☆'.repeat(5 - review.rating);
                return `
                    <div class="review-card">
                        <div class="review-header">
                            <span class="user-info">Customer #${review.accountID}</span>
                            <span class="rating-stars">${filledStars}${emptyStars}</span>
                        </div>
                        <p class="comment-text">${review.comment}</p>
                        <p class="review-date">${new Date(review.createdAt).toLocaleString()}</p>
                    </div>
                `;
            }).join('');

    } catch (error) {
        console.error('Error:', error);
        document.getElementById('reviews').innerHTML =
            '<div class="error">Failed to load reviews.</div>';
    }
}
