/**
 * Multi-step Form Functionality
 * For CrmTechTitans Member Creation
 */

// Declare jQuery variable if it's not already defined globally.
const $ = window.$ || jQuery;

document.addEventListener("DOMContentLoaded", () => {
    const steps = document.querySelectorAll(".form-step");
    const nextButtons = document.querySelectorAll(".next-step");
    const prevButtons = document.querySelectorAll(".prev-step");
    const progressBar = document.getElementById("progress-bar");
    const stepIndicators = document.querySelectorAll("[data-step]");
    const form = document.getElementById("multiStepForm");

    let currentStep = 0;
    const totalSteps = steps.length;

    // Check if there are validation errors on page load
    const hasValidationErrors = form.getAttribute("data-has-validation-errors") === "true" || 
                               document.querySelectorAll(".validation-summary-errors, .field-validation-error").length > 0;
    
    // Initialize form validation
    if ($.validator && $.validator.unobtrusive) {
        $.validator.unobtrusive.parse(form);
    }

    // Show specific step and update indicators
    const showStep = (step) => {
        steps.forEach((s, index) => {
            s.classList.toggle("d-none", index !== step);
            s.classList.toggle("active", index === step);
        });

        currentStep = step;
        progressBar.style.width = `${(currentStep / (totalSteps - 1)) * 100}%`;

        // Update step indicators
        stepIndicators.forEach((indicator) => {
            const stepNum = parseInt(indicator.getAttribute("data-step"));
            indicator.classList.toggle("bg-primary", stepNum === step + 1);
            indicator.classList.toggle("text-white", stepNum === step + 1);
            indicator.classList.toggle("bg-light", stepNum !== step + 1);
            indicator.classList.toggle("border", stepNum !== step + 1);
        });
        
        // Store current step in session storage
        sessionStorage.setItem("currentFormStep", step);
    };

    // Validate form fields for current step
    const validateCurrentStep = () => {
        const currentStepElement = steps[currentStep];
        let isValid = true;

        // Check required fields
        currentStepElement.querySelectorAll(".required").forEach((field) => {
            if (!field.value.trim()) {
                isValid = false;
                field.classList.add("is-invalid");
                const errorClass = field.name.toLowerCase().replace(/[\[\]\.]/g, '') + "-error";
                const errorSpan = document.querySelector(`.${errorClass}`);
                if (errorSpan) errorSpan.classList.remove("d-none");
            } else {
                field.classList.remove("is-invalid");
                const errorClass = field.name.toLowerCase().replace(/[\[\]\.]/g, '') + "-error";
                const errorSpan = document.querySelector(`.${errorClass}`);
                if (errorSpan) errorSpan.classList.add("d-none");
            }
        });

        // Step-specific validations
        if (currentStepElement.id === "step1") {
            // Validate membership type selection
            const membershipChecked = document.querySelectorAll(".membership-type:checked").length > 0;
            const membershipError = document.querySelector(".membership-error");
            if (membershipError) membershipError.classList.toggle("d-none", membershipChecked);
            isValid = isValid && membershipChecked;
            
            // Validate company website format
            const websiteField = document.querySelector("input[name='CompanyWebsite']");
            if (websiteField && websiteField.value.trim()) {
                const urlPattern = /^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$/;
                const isValidUrl = urlPattern.test(websiteField.value.trim());
                websiteField.classList.toggle("is-invalid", !isValidUrl);
                const websiteError = document.querySelector(".companywebsite-error");
                if (websiteError) {
                    websiteError.textContent = isValidUrl ? "Company Website is required" : "Invalid URL format";
                    websiteError.classList.toggle("d-none", isValidUrl);
                }
                isValid = isValid && isValidUrl;
            }
        } else if (currentStepElement.id === "step3") {
            // Validate at least one address has required fields filled
            const addressForms = document.querySelectorAll(".address-form");
            let hasValidAddress = false;
            
            addressForms.forEach(form => {
                const requiredFields = form.querySelectorAll(".required-address");
                const allFieldsFilled = Array.from(requiredFields).every(field => field.value.trim() !== "");
                
                // Highlight invalid fields
                requiredFields.forEach(field => {
                    field.classList.toggle("is-invalid", !field.value.trim());
                });
                
                if (allFieldsFilled) hasValidAddress = true;
            });
            
            const addressValidationMessage = document.querySelector(".address-validation-message");
            if (addressValidationMessage) {
                addressValidationMessage.classList.toggle("d-none", hasValidAddress);
            }
            
            isValid = isValid && hasValidAddress;
            
            // Validate postal code format if provided
            addressForms.forEach(form => {
                const postalCodeField = form.querySelector("input[name$='.PostalCode']");
                if (postalCodeField && postalCodeField.value.trim()) {
                    const postalPattern = /^[A-Za-z]\d[A-Za-z][ ]?\d[A-Za-z]\d$/;
                    const isValidPostal = postalPattern.test(postalCodeField.value.trim());
                    postalCodeField.classList.toggle("is-invalid", !isValidPostal);
                    
                    // Find the closest error message element
                    const errorSpan = postalCodeField.parentElement.querySelector(".text-danger") || 
                                     postalCodeField.parentElement.parentElement.querySelector(".text-danger");
                    if (errorSpan) {
                        errorSpan.textContent = isValidPostal ? "" : "Invalid postal code format (should be in the format 'A#A #A#')";
                        errorSpan.classList.toggle("d-none", isValidPostal);
                    }
                    
                    if (!isValidPostal) isValid = false;
                }
            });
        } else if (currentStepElement.id === "step4") {
            // Validate at least one contact has required fields filled
            const contactForms = document.querySelectorAll(".contact-form");
            let hasValidContact = false;
            
            contactForms.forEach(form => {
                const requiredFields = form.querySelectorAll(".required-contact");
                const allFieldsFilled = Array.from(requiredFields).every(field => field.value.trim() !== "");
                
                // Highlight invalid fields
                requiredFields.forEach(field => {
                    field.classList.toggle("is-invalid", !field.value.trim());
                });
                
                if (allFieldsFilled) hasValidContact = true;
                
                // Validate email format if provided
                const emailField = form.querySelector("input[name$='.Email']");
                if (emailField && emailField.value.trim()) {
                    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
                    const isValidEmail = emailPattern.test(emailField.value.trim());
                    emailField.classList.toggle("is-invalid", !isValidEmail);
                    
                    // Find the closest error message element
                    const errorSpan = emailField.parentElement.querySelector(".text-danger") || 
                                     emailField.parentElement.parentElement.querySelector(".text-danger");
                    if (errorSpan) {
                        errorSpan.textContent = isValidEmail ? "" : "Invalid email format";
                        errorSpan.classList.toggle("d-none", isValidEmail);
                    }
                    
                    if (!isValidEmail && allFieldsFilled) hasValidContact = false;
                }
                
                // Validate phone format
                const phoneField = form.querySelector("input[name$='.Phone']");
                if (phoneField && phoneField.value.trim()) {
                    const phonePattern = /^\d{10}$/;
                    const isValidPhone = phonePattern.test(phoneField.value.trim());
                    phoneField.classList.toggle("is-invalid", !isValidPhone);
                    
                    // Find the closest error message element
                    const errorSpan = phoneField.parentElement.querySelector(".text-danger") || 
                                     phoneField.parentElement.parentElement.querySelector(".text-danger");
                    if (errorSpan) {
                        errorSpan.textContent = isValidPhone ? "" : "Phone number must be 10 digits";
                        errorSpan.classList.toggle("d-none", isValidPhone);
                    }
                    
                    if (!isValidPhone && allFieldsFilled) hasValidContact = false;
                }
            });
            
            const contactValidationMessage = document.querySelector(".contact-validation-message");
            if (contactValidationMessage) {
                contactValidationMessage.classList.toggle("d-none", hasValidContact);
            }
            
            // Validate industry selection
            const industrySelected = document.querySelector(".required-industry").selectedOptions.length > 0;
            const industryValidationMessage = document.querySelector(".industry-validation-message");
            if (industryValidationMessage) {
                industryValidationMessage.classList.toggle("d-none", industrySelected);
            }
            
            isValid = isValid && hasValidContact && industrySelected;
        }

        return isValid;
    };

    // Navigate to next step
    nextButtons.forEach((button) => {
        button.addEventListener("click", () => {
            if (validateCurrentStep() && currentStep < steps.length - 1) {
                showStep(currentStep + 1);
                window.scrollTo(0, 0); // Scroll to top when changing steps
            }
        });
    });

    // Navigate to previous step
    prevButtons.forEach((button) => {
        button.addEventListener("click", () => {
            if (currentStep > 0) {
                showStep(currentStep - 1);
                window.scrollTo(0, 0); // Scroll to top when changing steps
            }
        });
    });

    // Handle form submission
    form.addEventListener("submit", function(e) {
        // Validate all steps before submitting
        let allStepsValid = true;
        
        // Store current step
        const originalStep = currentStep;
        
        // Check each step
        for (let i = 0; i < steps.length; i++) {
            showStep(i);
            if (!validateCurrentStep()) {
                allStepsValid = false;
                break;
            }
        }
        
        // If not all steps are valid, prevent submission and show the first invalid step
        if (!allStepsValid) {
            e.preventDefault();
            for (let i = 0; i < steps.length; i++) {
                showStep(i);
                if (!validateCurrentStep()) {
                    break;
                }
            }
        } else {
            // If all steps are valid but we want to restore the original step (in case submission is prevented by other means)
            showStep(originalStep);
        }
    });

    // Image preview functionality
    const previewImage = (input, previewId) => {
        const preview = document.getElementById(previewId);
        if (input.files && input.files[0]) {
            const reader = new FileReader();
            reader.onload = (e) => {
                preview.src = e.target.result;
                preview.classList.remove("d-none");
            };
            reader.readAsDataURL(input.files[0]);
        }
    };

    window.previewImage = previewImage;
    window.previewContactImage = (input, index) => {
        previewImage(input, `contact-preview-${index}`);
    };

    // Add Address functionality
    document.getElementById("add-address").addEventListener("click", () => {
        const addressesContainer = document.getElementById("addresses-container");
        const nextIndex = document.querySelectorAll(".address-form").length;

        const newAddressForm = document.createElement("div");
        newAddressForm.className = "card mb-4 address-form";
        newAddressForm.dataset.addressIndex = nextIndex;
        newAddressForm.innerHTML = `
        <div class="card-header bg-light d-flex justify-content-between align-items-center">
            <h3 class="h5 mb-0">Address ${nextIndex + 1}</h3>
            <button type="button" class="btn btn-sm btn-danger remove-address">
                <i class="fas fa-trash me-1"></i> Remove
            </button>
        </div>
        <div class="card-body">
            <div class="row mb-3">
                <div class="col-md-6 mb-3 mb-md-0">
                    <label class="form-label">Street <span class="text-danger">*</span></label>
                    <div class="input-group">
                        <span class="input-group-text"><i class="fas fa-map-marker-alt"></i></span>
                        <input name="Addresses[${nextIndex}].Street" class="form-control required-address" placeholder="Enter Street Address" />
                    </div>
                    <span class="text-danger d-none"></span>
                </div>
                <div class="col-md-6">
                    <label class="form-label">City <span class="text-danger">*</span></label>
                    <div class="input-group">
                        <span class="input-group-text"><i class="fas fa-city"></i></span>
                        <input name="Addresses[${nextIndex}].City" class="form-control required-address" placeholder="Enter City" />
                    </div>
                    <span class="text-danger d-none"></span>
                </div>
            </div>
            <div class="row">
                <div class="col-md-4 mb-3 mb-md-0">
                    <label class="form-label">Province <span class="text-danger">*</span></label>
                    <select name="Addresses[${nextIndex}].Province" class="form-select required-address">
                        <option value="">Select Province</option>
                        ${Array.from(document.querySelector('select[name="Addresses[0].Province"]').options)
                .map((opt) => `<option value="${opt.value}">${opt.text}</option>`)
                .join("")}
                    </select>
                    <span class="text-danger d-none"></span>
                </div>
                <div class="col-md-4 mb-3 mb-md-0">
                    <label class="form-label">Postal Code</label>
                    <div class="input-group">
                        <span class="input-group-text"><i class="fas fa-mail-bulk"></i></span>
                        <input name="Addresses[${nextIndex}].PostalCode" class="form-control" placeholder="Enter Postal Code" />
                    </div>
                    <span class="text-danger d-none"></span>
                </div>
                <div class="col-md-4">
                    <label class="form-label">Address Type <span class="text-danger">*</span></label>
                    <select name="Addresses[${nextIndex}].AddressType" class="form-select required-address">
                        <option value="">Select Address Type</option>
                        ${Array.from(document.querySelector('select[name="Addresses[0].AddressType"]').options)
                .map((opt) => `<option value="${opt.value}">${opt.text}</option>`)
                .join("")}
                    </select>
                    <span class="text-danger d-none"></span>
                </div>
            </div>
        </div>
    `;

        addressesContainer.appendChild(newAddressForm);

        // Add remove handler
        newAddressForm.querySelector(".remove-address").addEventListener("click", function () {
            if (document.querySelectorAll(".address-form").length > 1) {
                this.closest(".address-form").remove();
            }
        });

        // Re-initialize form validation for the new address form
        if ($.validator && $.validator.unobtrusive) {
            $.validator.unobtrusive.parse(newAddressForm);
        }
    });

    // Add Contact functionality
    document.getElementById("add-contact").addEventListener("click", () => {
        const contactsContainer = document.getElementById("contacts-container");
        const nextIndex = document.querySelectorAll(".contact-form").length;

        const newContactForm = document.createElement("div");
        newContactForm.className = "card mb-4 contact-form";
        newContactForm.dataset.contactIndex = nextIndex;
        newContactForm.innerHTML = `
            <div class="card-header bg-light d-flex justify-content-between align-items-center">
                <h3 class="h5 mb-0">Contact ${nextIndex + 1}</h3>
                <button type="button" class="btn btn-sm btn-danger remove-contact">
                    <i class="fas fa-trash me-1"></i> Remove
                </button>
            </div>
            <div class="card-body">
                <div class="row mb-3">
                    <div class="col-md-6 mb-3 mb-md-0">
                        <label class="form-label">First Name <span class="text-danger">*</span></label>
                        <div class="input-group">
                            <span class="input-group-text"><i class="fas fa-user"></i></span>
                            <input name="Contacts[${nextIndex}].FirstName" class="form-control required-contact" placeholder="Enter first name" />
                        </div>
                        <span class="text-danger d-none"></span>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Last Name <span class="text-danger">*</span></label>
                        <div class="input-group">
                            <span class="input-group-text"><i class="fas fa-user"></i></span>
                            <input name="Contacts[${nextIndex}].LastName" class="form-control required-contact" placeholder="Enter last name" />
                        </div>
                        <span class="text-danger d-none"></span>
                    </div>
                </div>
                <div class="row mb-3">
                    <div class="col-md-6 mb-3 mb-md-0">
                        <label class="form-label">Email <span class="text-danger">*</span></label>
                        <div class="input-group">
                            <span class="input-group-text"><i class="fas fa-envelope"></i></span>
                            <input name="Contacts[${nextIndex}].Email" class="form-control required-contact" placeholder="Enter email" type="email" />
                        </div>
                        <span class="text-danger d-none"></span>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Phone <span class="text-danger">*</span></label>
                        <div class="input-group">
                            <span class="input-group-text"><i class="fas fa-phone"></i></span>
                            <input name="Contacts[${nextIndex}].Phone" class="form-control required-contact" placeholder="Enter phone number" />
                        </div>
                        <span class="text-danger d-none"></span>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-6 mb-3 mb-md-0">
                        <label class="form-label">Contact Photo</label>
                        <div class="input-group">
                            <span class="input-group-text"><i class="fas fa-camera"></i></span>
                            <input name="Contacts[${nextIndex}].ContactPhoto" type="file" class="form-control" accept="image/*" onchange="previewContactImage(this, ${nextIndex})" />
                        </div>
                        <div class="mt-2" style="max-width: 150px;">
                            <img class="img-fluid d-none border rounded contact-preview" id="contact-preview-${nextIndex}" src="#" alt="Preview" />
                        </div>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Contact Type <span class="text-danger">*</span></label>
                        <select name="Contacts[${nextIndex}].ContactType" class="form-select required-contact">
                            <option value="">Select Contact Type</option>
                            ${Array.from(document.querySelector('select[name="Contacts[0].ContactType"]').options)
                .map((opt) => `<option value="${opt.value}">${opt.text}</option>`)
                .join("")}
                        </select>
                        <span class="text-danger d-none"></span>
                    </div>
                </div>
            </div>
        `;

        contactsContainer.appendChild(newContactForm);

        // Add remove handler
        newContactForm.querySelector(".remove-contact").addEventListener("click", function () {
            if (document.querySelectorAll(".contact-form").length > 1) {
                this.closest(".contact-form").remove();
            }
        });

        // Re-initialize form validation for the new contact form
        if ($.validator && $.validator.unobtrusive) {
            $.validator.unobtrusive.parse(newContactForm);
        }
    });

    // Handle remove contact buttons
    document.addEventListener("click", (e) => {
        if (e.target.closest(".remove-contact")) {
            const contactForms = document.querySelectorAll(".contact-form");
            if (contactForms.length > 1) {
                e.target.closest(".contact-form").remove();
            }
        }
    });

    // Handle remove address buttons
    document.addEventListener("click", (e) => {
        if (e.target.closest(".remove-address")) {
            const addressForms = document.querySelectorAll(".address-form");
            if (addressForms.length > 1) {
                e.target.closest(".address-form").remove();
            }
        }
    });

    // Check for validation errors on page load and navigate to the appropriate step
    if (hasValidationErrors) {
        // Get the last active step from session storage, or default to 0
        const lastStep = parseInt(sessionStorage.getItem("currentFormStep") || "0");
        showStep(lastStep);
        
        // Find the first step with validation errors
        for (let i = 0; i < steps.length; i++) {
            const stepElement = steps[i];
            const hasErrors = stepElement.querySelectorAll(".field-validation-error, .input-validation-error").length > 0;
            
            if (hasErrors) {
                showStep(i);
                break;
            }
        }
    } else {
        // Start at the first step
        showStep(0);
    }

    // Auto-fill functionality (for development/testing)
    const autoFillButtons = document.querySelectorAll(".auto-fill");
    autoFillButtons.forEach(button => {
        button.addEventListener("click", () => {
            const step = parseInt(button.getAttribute("data-step"));
            autoFillStep(step);
        });
    });
});

// Auto-fill functions for testing
const autoFillStep = (step) => {
    switch (step) {
        case 1:
            // Auto-fill member details
            document.querySelector("input[name='MemberName']").value = "Tech Solutions Inc.";
            
            // Select at least one membership type
            const membershipCheckbox = document.querySelector(".membership-type");
            if (membershipCheckbox && !membershipCheckbox.checked) {
                membershipCheckbox.checked = true;
            }
            
            // Set company size - updated to use new numerical range values
            document.querySelector("select[name='CompanySize']").value = "1"; // OneToTen (1-10)
            
            // Set company website - removed https:// requirement
            document.querySelector("input[name='CompanyWebsite']").value = "techsolutions.example.com";
            
            // Set contacted by
            document.querySelector("input[name='ContactedBy']").value = "John Smith";
            break;
            
        case 2:
            // Auto-fill dates and notes
            const today = new Date();
            const formattedDate = today.toISOString().split('T')[0]; // Format as YYYY-MM-DD
            
            document.querySelector("input[name='MemberSince']").value = formattedDate;
            document.querySelector("input[name='LastContactDate']").value = formattedDate;
            document.querySelector("textarea[name='Notes']").value = "This is an auto-generated test member with sample data for testing purposes.";
            break;
            
        case 3:
            // Auto-fill address
            document.querySelector("input[name='Addresses[0].Street']").value = "123 Main Street";
            document.querySelector("input[name='Addresses[0].City']").value = "Toronto";
            document.querySelector("select[name='Addresses[0].Province']").value = "7"; // Ontario
            document.querySelector("input[name='Addresses[0].PostalCode']").value = "M4B 1B3";
            document.querySelector("select[name='Addresses[0].AddressType']").value = "2"; // Office
            break;
            
        case 4:
            // Auto-fill contact
            document.querySelector("input[name='Contacts[0].FirstName']").value = "Jane";
            document.querySelector("input[name='Contacts[0].LastName']").value = "Doe";
            document.querySelector("input[name='Contacts[0].Email']").value = "jane.doe@example.com";
            document.querySelector("input[name='Contacts[0].Phone']").value = "4165551234";
            document.querySelector("select[name='Contacts[0].ContactType']").value = "0"; // VIP
            
            // Select at least one industry
            const industrySelect = document.querySelector("select[name='SelectedIndustryIds']");
            if (industrySelect && industrySelect.options.length > 0) {
                industrySelect.options[0].selected = true;
            }
            break;
    }
};

// Helper function to get random items from an array
const getRandomItems = (array, count) => {
    const shuffled = [...array].sort(() => 0.5 - Math.random());
    return shuffled.slice(0, count);
};