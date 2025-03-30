/**
 * Member Index Page JavaScript
 * Handles filtering, pagination, and sorting for the members table
 */

document.addEventListener('DOMContentLoaded', function () {
    // DOM Elements
    const membersTable = document.getElementById('membersTable');
    const tableRows = document.querySelectorAll('#membersTable tbody tr');
    const memberNameFilter = document.getElementById('memberNameFilter');
    const membershipStatusFilter = document.getElementById('membershipStatusFilter');
    const recordsPerPageSelect = document.getElementById('recordsPerPage');
    const prevPageButton = document.getElementById('prevPage');
    const nextPageButton = document.getElementById('nextPage');
    const pageInfo = document.getElementById('pageInfo');
    
    // Pagination state
    let currentPage = 1;
    let recordsPerPage = parseInt(recordsPerPageSelect?.value || 10);
    
    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
    
    // Set default filter to "Good Standing"
    if (membershipStatusFilter) {
        membershipStatusFilter.value = "GoodStanding";
    }
    
    /**
     * Get rows that match the current filter criteria
     */
    function getFilteredRows() {
        const nameFilter = memberNameFilter?.value.toLowerCase() || '';
        const statusFilter = membershipStatusFilter?.value.toLowerCase() || '';
        
        return Array.from(tableRows).filter(row => {
            const name = row.dataset.name || '';
            const status = row.dataset.status || '';
            
            return name.includes(nameFilter) && 
                  (statusFilter === '' || status === statusFilter);
        });
    }
    
    /**
     * Update the table to show only the current page of filtered rows
     */
    function updateTableDisplay() {
        const filteredRows = getFilteredRows();
        const totalPages = Math.max(1, Math.ceil(filteredRows.length / recordsPerPage));
        
        // Adjust current page if needed
        if (currentPage > totalPages) {
            currentPage = totalPages;
        }
        
        const startIndex = (currentPage - 1) * recordsPerPage;
        const endIndex = Math.min(startIndex + recordsPerPage, filteredRows.length);
        
        // Hide all rows first
        tableRows.forEach(row => {
            row.style.display = 'none';
        });
        
        // Show only the rows for the current page
        filteredRows.slice(startIndex, endIndex).forEach(row => {
            row.style.display = '';
        });
        
        // Update pagination info and buttons
        if (pageInfo) {
            pageInfo.textContent = `Page ${currentPage} of ${totalPages} (${filteredRows.length} records)`;
        }
        if (prevPageButton) {
            prevPageButton.disabled = currentPage <= 1;
        }
        if (nextPageButton) {
            nextPageButton.disabled = currentPage >= totalPages;
        }
    }
    
    /**
     * Handle filter changes
     */
    function handleFilterChange() {
        currentPage = 1; // Reset to first page when filter changes
        updateTableDisplay();
    }
    
    /**
     * Handle records per page change
     */
    function handleRecordsPerPageChange() {
        recordsPerPage = parseInt(recordsPerPageSelect.value);
        currentPage = 1; // Reset to first page
        updateTableDisplay();
    }
    
    /**
     * Navigate to previous page
     */
    function goToPreviousPage() {
        if (currentPage > 1) {
            currentPage--;
            updateTableDisplay();
        }
    }
    
    /**
     * Navigate to next page
     */
    function goToNextPage() {
        const filteredRows = getFilteredRows();
        const totalPages = Math.ceil(filteredRows.length / recordsPerPage);
        
        if (currentPage < totalPages) {
            currentPage++;
            updateTableDisplay();
        }
    }
    
    // Event listeners
    if (memberNameFilter) {
        memberNameFilter.addEventListener('input', handleFilterChange);
    }
    if (membershipStatusFilter) {
        membershipStatusFilter.addEventListener('change', handleFilterChange);
    }
    if (recordsPerPageSelect) {
        recordsPerPageSelect.addEventListener('change', handleRecordsPerPageChange);
    }
    if (prevPageButton) {
        prevPageButton.addEventListener('click', goToPreviousPage);
    }
    if (nextPageButton) {
        nextPageButton.addEventListener('click', goToNextPage);
    }
    
    // Handle archive/unarchive functionality
    const archiveButtons = document.querySelectorAll('.archive-btn');
    archiveButtons.forEach(button => {
        button.addEventListener('click', function() {
            const memberId = this.dataset.id;
            const currentStatus = this.dataset.status.toLowerCase();
            const newStatus = (currentStatus === 'cancelled') ? 'GoodStanding' : 'Cancelled';
            const confirmMessage = (currentStatus === 'cancelled')
                ? "Are you sure you want to unarchive this member?"
                : "Are you sure you want to archive this member?";
                
            if (!confirm(confirmMessage)) {
                return;
            }

            // If status is being set to 'Cancelled', prompt for a reason
            let reason = "";
            if (newStatus === 'Cancelled') {
                reason = prompt("Please provide a reason for archiving this member:");
                if (!reason || reason.trim() === "") {
                    alert("Archiving reason is required.");
                    return; // Stop if no reason is provided
                }
            }
            
            fetch('/Member/ToggleArchive', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    memberId: parseInt(memberId),
                    newStatus: newStatus,
                    reason: reason // Include the reason here
                })
            })
            .then(response => {
                if (!response.ok) {
                    return response.json().then(error => { throw new Error(error.message); });
                }
                return response.json();
            })
            .then(data => {
                alert(data.message);
                location.reload();
            })
            .catch(error => {
                console.error("Error:", error);
                alert("Failed to update member status.");
            });
        });
    });


    // Update table header based on the selected status filter
    function updateTableHeader() {
        if (!membershipStatusFilter) return;
        
        const statusFilter = membershipStatusFilter.value;
        const tableHeader = document.querySelector('#membersTable thead tr');
        if (!tableHeader) return;

        // If the selected status is 'Cancelled', change the column heading for 'Member Since'
        if (statusFilter === 'Cancelled') {
            tableHeader.querySelectorAll('th')[4].textContent = 'Cancelled Date'; // Modify the 5th column header
            // Add back the sort icon that might have been removed
            let thElement = tableHeader.querySelectorAll('th')[4];
            if (!thElement.querySelector('.fas')) {
                thElement.innerHTML += ' <i class="fas fa-sort"></i>';
            }
        } else {
            // Otherwise, keep it as 'Member Since'
            tableHeader.querySelectorAll('th')[4].textContent = 'Member Since';
            // Add back the sort icon that might have been removed
            let thElement = tableHeader.querySelectorAll('th')[4];
            if (!thElement.querySelector('.fas')) {
                thElement.innerHTML += ' <i class="fas fa-sort"></i>';
            }
        }
    }

    // Add event listener to update the table header whenever the filter changes
    if (membershipStatusFilter) {
        membershipStatusFilter.addEventListener('change', updateTableHeader);
    }

    // Call this function initially when the page loads
    updateTableHeader();

    // Get the label element
    const labelElement = document.getElementById("membership-label");

    if (labelElement) {
        const status = labelElement.getAttribute("data-status").trim().toLowerCase();

        // Change the label text dynamically
        labelElement.textContent = (status === "cancelled") ? "Cancellation Date:" : "Member Since:";
    }
    
    // Initialize the table display
    updateTableDisplay();
    
    // Sort the table by name initially
    if (membersTable) {
        sortTable(1, 'asc');
    }
    
    // Listen for sort-complete event to update display
    document.addEventListener('sort-complete', updateTableDisplay);
});

/**
 * Sort the table by the specified column
 * @param {number} columnIndex - The index of the column to sort by
 * @param {string} initialDirection - The initial sort direction ('asc' or 'desc')
 */
let isSorting = false; // Global flag to prevent concurrent sorts

function sortTable(columnIndex, initialDirection = 'asc') {
    // Prevent concurrent sorting operations
    if (isSorting) {
        console.log("Sorting operation already in progress, ignoring new request");
        return;
    }
    
    isSorting = true;
    
    try {
        const table = document.getElementById('membersTable');
        if (!table) return;
        
        const headers = table.querySelectorAll('th');
        const tbody = table.querySelector('tbody');
        if (!tbody) return;
        
        const rows = Array.from(tbody.querySelectorAll('tr'));
        
        // Determine sort direction
        let direction = initialDirection;
        if (headers[columnIndex].classList.contains('sorted-asc')) {
            direction = 'desc';
        } else if (headers[columnIndex].classList.contains('sorted-desc')) {
            direction = 'asc';
        }
        
        // Remove sort indicators from all headers
        headers.forEach(header => {
            header.classList.remove('sorted-asc', 'sorted-desc');
        });
        
        // Add sort indicator to current header
        headers[columnIndex].classList.add(direction === 'asc' ? 'sorted-asc' : 'sorted-desc');
        
        // Update sort icons for all sortable headers
        for (let i = 1; i < headers.length - 1; i++) { // Skip first (Logo) and last (Actions) columns
            const iconElement = headers[i].querySelector('.fas');
            if (iconElement) {
                if (i === columnIndex) {
                    // Set the icon for the current sorted column
                    iconElement.className = direction === 'asc' ? 'fas fa-sort-up' : 'fas fa-sort-down';
                } else {
                    // Reset other column icons to default
                    iconElement.className = 'fas fa-sort';
                }
            }
        }
        
        // Sort the rows
        rows.sort((rowA, rowB) => {
            const cellA = rowA.cells[columnIndex].textContent.trim().toLowerCase();
            const cellB = rowB.cells[columnIndex].textContent.trim().toLowerCase();
            
            if (cellA < cellB) {
                return direction === 'asc' ? -1 : 1;
            }
            if (cellA > cellB) {
                return direction === 'asc' ? 1 : -1;
            }
            return 0;
        });
        
        // Reorder the rows in the table
        rows.forEach(row => {
            tbody.appendChild(row);
        });
        
        // Update the display to maintain pagination
        const event = new Event('sort-complete');
        document.dispatchEvent(event);
    } finally {
        // Always reset the sorting flag when done
        isSorting = false;
    }
}