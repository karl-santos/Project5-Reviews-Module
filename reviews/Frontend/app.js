const API_URL = 'https://localhost:7213/api/review';

// Get URL parameters when page loads
window.addEventListener('DOMContentLoaded', () => {
    const urlParams = new URLSearchParams(window.location.search);
    const productName = urlParams.get('productName');
    const productId = urlParams.get('productId');
    const userId = urlParams.get('userId');

    // Pre-fill and disable if values exist
    if (productName) {
        document.getElementById('productName').value = productName;
        document.getElementById('productName').disabled = true;
    }

    if (productId) {
        document.getElementById('productId').value = productId;
        document.getElementById('productId').disabled = true;
        
        // Load reviews for this product automatically
        loadReviews(productId);
    }

    if (userId) {
        document.getElementById('userId').value = userId;
        document.getElementById('userId').disabled = true;
    }
});

// Submit a new review
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
            const data = await response.json();
            document.getElementById('message').innerHTML =
                '<p class="success">Review submitted successfully!</p>';
            document.getElementById('comment').value = '';
            document.getElementById('rating').value = '';
            loadReviews(review.productID);
        } else {
            const error = await response.text();
            document.getElementById('message').innerHTML =
                `<p class="error">Error: ${error}</p>`;
        }
    } catch (error) {
        console.error('Error:', error);
        document.getElementById('message').innerHTML =
            '<p class="error">Failed to submit review. Is the API running?</p>';
    }
});

// Load reviews for a specific product
async function loadReviews(productId) {
    try {
        const response = await fetch(`${API_URL}/product/${productId}`);
        const reviews = await response.json();

        const reviewsDiv = document.getElementById('reviews');

        if (reviews.length === 0) {
            reviewsDiv.innerHTML = '<p>No reviews yet for this product.</p>';
            return;
        }

        reviewsDiv.innerHTML = reviews.map(review => `
            <div class="review-card">
                <p><strong>Product:</strong> ${review.productID}</p>
                <p><strong>User:</strong> ${review.userID}</p>
                <p><strong>Rating:</strong> ${review.rating}/5</p>
                <p><strong>Comment:</strong> ${review.comment}</p>
                <p class="date">${new Date(review.createdAt).toLocaleString()}</p>
            </div>
        `).join('');

    } catch (error) {
        console.error('Error:', error);
        document.getElementById('reviews').innerHTML =
            '<p class="error">Failed to load reviews</p>';
    }
}