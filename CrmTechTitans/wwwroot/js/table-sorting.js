/**
 * Common Table Sorting Functionality
 * Shared script to handle table sorting across the application
 */

// Global flag to prevent concurrent sorts
let isSorting = false;

/**
 * Sort a table by the specified column
 * @param {string} tableId - The ID of the table to sort
 * @param {number} columnIndex - The index of the column to sort by
 * @param {string} initialDirection - The initial sort direction ('asc' or 'desc')
 */
function sortTable(tableId, columnIndex, initialDirection = 'asc') {
    // Prevent concurrent sorting operations
    if (isSorting) {
        console.log("Sorting operation already in progress, ignoring new request");
        return;
    }
    
    isSorting = true;
    
    try {
        const table = document.getElementById(tableId);
        if (!table) {
            console.error(`Table with ID '${tableId}' not found.`);
            return;
        }
        
        const headers = table.querySelectorAll('th');
        const tbody = table.querySelector('tbody');
        if (!tbody) {
            console.error(`Table with ID '${tableId}' does not have a tbody.`);
            return;
        }
        
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
        
        // Update icons for all sortable headers
        const sortableHeaders = Array.from(headers).filter(th => th.classList.contains('sortable'));
        
        sortableHeaders.forEach(header => {
            const iconElement = header.querySelector('.fas');
            if (iconElement) {
                if (header === headers[columnIndex]) {
                    // Set the icon for the current sorted column
                    iconElement.className = direction === 'asc' ? 'fas fa-sort-up' : 'fas fa-sort-down';
                } else {
                    // Reset other column icons to default
                    iconElement.className = 'fas fa-sort';
                }
            }
        });
        
        // Sort the rows
        rows.sort((rowA, rowB) => {
            const cellA = rowA.cells[columnIndex].textContent.trim().toLowerCase();
            const cellB = rowB.cells[columnIndex].textContent.trim().toLowerCase();
            
            // Try to parse as numbers if possible
            const numA = parseFloat(cellA);
            const numB = parseFloat(cellB);
            
            if (!isNaN(numA) && !isNaN(numB)) {
                // Numeric comparison
                return direction === 'asc' ? numA - numB : numB - numA;
            } else {
                // String comparison
                if (cellA < cellB) {
                    return direction === 'asc' ? -1 : 1;
                }
                if (cellA > cellB) {
                    return direction === 'asc' ? 1 : -1;
                }
                return 0;
            }
        });
        
        // Reorder the rows in the table
        rows.forEach(row => {
            tbody.appendChild(row);
        });
        
        // Update the display to maintain pagination
        // This assumes there's a function named updatePagination in the calling context
        if (typeof updatePagination === 'function') {
            updatePagination();
        }
        
        // Dispatch a sort-complete event for components that need to react to sorting
        const event = new Event('sort-complete');
        document.dispatchEvent(event);
        
    } finally {
        // Always reset the sorting flag when done
        isSorting = false;
    }
}

/**
 * Initialize sortable tables in the document
 */
document.addEventListener('DOMContentLoaded', function() {
    // Find all tables with sortable headers
    const tables = document.querySelectorAll('table');
    
    tables.forEach(table => {
        const tableId = table.id;
        if (!tableId) return;
        
        // Find all sortable column headers
        const sortableHeaders = table.querySelectorAll('th.sortable');
        
        // Attach click handlers
        sortableHeaders.forEach((header, index) => {
            // Find the actual column index (not just the index within sortable headers)
            const columnIndex = Array.from(table.querySelectorAll('th')).indexOf(header);
            
            // Make sure the header has a sort icon
            if (!header.querySelector('.fas')) {
                header.innerHTML += ' <i class="fas fa-sort"></i>';
            }
            
            // Override any existing click handlers with our consistent one
            header.onclick = () => sortTable(tableId, columnIndex);
        });
        
        // Optional: Sort by first sortable column initially
        const firstSortableHeader = table.querySelector('th.sortable');
        if (firstSortableHeader) {
            const columnIndex = Array.from(table.querySelectorAll('th')).indexOf(firstSortableHeader);
            sortTable(tableId, columnIndex, 'asc');
        }
    });
}); 