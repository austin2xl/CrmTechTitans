document.addEventListener('DOMContentLoaded', function () {
    const exportExcelBtn = document.getElementById('exportExcelBtn');
    const excelExportModal = new bootstrap.Modal(document.getElementById('excelExportModal'));
    const selectAllCheckbox = document.getElementById('selectAllMembers');
    const memberCheckboxes = document.querySelectorAll('.member-checkbox');
    const selectAllFieldsCheckbox = document.getElementById('selectAllFields');
    const fieldCheckboxes = document.querySelectorAll('.field-checkbox');
    const downloadForm = document.getElementById('downloadForm');
    const downloadExcelBtn = document.getElementById('downloadExcelBtn');

    // Check if we should automatically open the export dialog (from dashboard reports card)
    if (localStorage.getItem('openExcelExport') === 'true') {
        // Clear the flag first to prevent it from triggering again on refresh
        localStorage.removeItem('openExcelExport');
        
        // Use setTimeout to ensure the modal opens after the page is fully loaded
        setTimeout(() => {
            excelExportModal.show();
        }, 500);
    }

    // Show modal when "Export to Excel" button is clicked
    exportExcelBtn.addEventListener('click', function () {
        excelExportModal.show();
    });

    // Handle "Select All Members" checkbox
    selectAllCheckbox.addEventListener('change', function () {
        memberCheckboxes.forEach(checkbox => {
            checkbox.checked = this.checked;
        });
    });

    // Update "Select All Members" checkbox when individual member checkboxes change
    memberCheckboxes.forEach(checkbox => {
        checkbox.addEventListener('change', updateSelectAllCheckbox);
    });

    function updateSelectAllCheckbox() {
        selectAllCheckbox.checked = Array.from(memberCheckboxes).every(checkbox => checkbox.checked);
    }

    // Handle "Select All Fields" checkbox
    selectAllFieldsCheckbox.addEventListener('change', function () {
        fieldCheckboxes.forEach(checkbox => {
            checkbox.checked = this.checked;
        });
    });

    // Update "Select All Fields" checkbox when individual field checkboxes change
    fieldCheckboxes.forEach(checkbox => {
        checkbox.addEventListener('change', updateSelectAllFieldsCheckbox);
    });

    function updateSelectAllFieldsCheckbox() {
        selectAllFieldsCheckbox.checked = Array.from(fieldCheckboxes).every(checkbox => checkbox.checked);
    }

    // Initial state for "Select All Fields" checkbox
    updateSelectAllFieldsCheckbox();

    // Handle form submission
    downloadExcelBtn.addEventListener('click', function () {
        const selectedMembers = document.querySelectorAll('.member-checkbox:checked');
        const selectedFields = document.querySelectorAll('.field-checkbox:checked');

        if (selectedMembers.length === 0 && !selectAllCheckbox.checked) {
            alert('Please select at least one member to download.');
            return;
        }

        if (selectedFields.length === 0) {
            alert('Please select at least one field to include in the download.');
            return;
        }

        // If validation passes, submit the form
        downloadForm.submit();
        excelExportModal.hide();
    });
});