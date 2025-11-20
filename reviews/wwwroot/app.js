const API_URL = 'https://localhost:7213/api/review';

// Initialize page
window.addEventListener('DOMContentLoaded', () => {
    const urlParams = new URLSearchParams(window.location.search);
    const productName = urlParams.get('productName');
    const productId = urlParams.get('productId');
    const userId = urlParams.get('userId');

    if (productName) {
        document.getElementById('productName').value = productName;
        document.getElementById('displayProductName').textContent = productName;
    }

    if (productId) {
        document.getElementById('productId').value = productId;
        document.getElementById('displayProductId').textContent = productId;
        loadReviews(productId);
    }

    if (userId) {
        document.getElementById('userId').value = userId;
    }
});

// Character counter
const commentField = document.getElementById('comment');
const charCountSpan = document.getElementById('charCount');

commentField.addEventListener('input', () => {
    charCountSpan.textContent = commentField.value.length;
});

// Submit review
document.getElementById('reviewForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const review = {
        productID: document.getElementById('productId').value,
        userID: document.getElementById('userId').value,
        comment: document.getElementById('comment').value,
        rating: parseInt(document.getElementById('rating').value)
    };

    try {
        const response = await fetch(`${API_URL}/product`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(review)
        });

        if (response.ok) {
            document.getElementById('message').innerHTML =
                '<div class="success">Review submitted successfully.</div>';
            document.getElementById('comment').value = '';
            document.getElementById('rating').value = '';
            charCountSpan.textContent = '0';
            loadReviews(review.productID);
            
            setTimeout(() => {
                document.getElementById('message').innerHTML = '';
            }, 5000);
        } else {
            const error = await response.text();
            document.getElementById('message').innerHTML =
                `<div class="error">Error: ${error}</div>`;
        }
    } catch (error) {
        console.error('Error:', error);
        document.getElementById('message').innerHTML =
            '<div class="error">Failed to submit review. Please ensure the API is running.</div>';
    }
});

// Load reviews
async function loadReviews(productId) {
    try {
        const response = await fetch(`${API_URL}/product/${productId}`);
        const reviews = await response.json();

        const reviewsDiv = document.getElementById('reviews');
        const statsDiv = document.getElementById('reviewStats');

        if (reviews.length === 0) {
            reviewsDiv.innerHTML = '<div class="no-reviews">No reviews yet. Be the first to review this product.</div>';
            statsDiv.innerHTML = '';
            return;
        }

        // Calculate average rating
        const avgRating = (reviews.reduce((sum, r) => sum + r.rating, 0) / reviews.length).toFixed(1);
        
        statsDiv.innerHTML = `
            <strong>${reviews.length}</strong> ${reviews.length === 1 ? 'review' : 'reviews'} · 
            Average rating: <strong>${avgRating}</strong> / 5
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
                            <span class="user-info">User: ${review.userID}</span>
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