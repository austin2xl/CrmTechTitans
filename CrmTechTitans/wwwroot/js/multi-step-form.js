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
    const hasValidationErrors = document.querySelectorAll(".validation-summary-errors, .field-validation-error").length > 0;
    
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
        } else if (currentStepElement.id === "step3") {
            // Validate at least one address has required fields filled
            const addressForms = document.querySelectorAll(".address-form");
            let hasValidAddress = false;
            
            addressForms.forEach(form => {
                const requiredFields = form.querySelectorAll(".required-address");
                const allFieldsFilled = Array.from(requiredFields).every(field => field.value.trim() !== "");
                if (allFieldsFilled) hasValidAddress = true;
            });
            
            const addressValidationMessage = document.querySelector(".address-validation-message");
            if (addressValidationMessage) {
                addressValidationMessage.classList.toggle("d-none", hasValidAddress);
            }
            
            isValid = isValid && hasValidAddress;
        } else if (currentStepElement.id === "step4") {
            // Validate at least one contact has required fields filled
            const contactForms = document.querySelectorAll(".contact-form");
            let hasValidContact = false;
            
            contactForms.forEach(form => {
                const requiredFields = form.querySelectorAll(".required-contact");
                const allFieldsFilled = Array.from(requiredFields).every(field => field.value.trim() !== "");
                if (allFieldsFilled) hasValidContact = true;
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
                </div>
                <div class="col-md-6">
                    <label class="form-label">City <span class="text-danger">*</span></label>
                    <div class="input-group">
                        <span class="input-group-text"><i class="fas fa-city"></i></span>
                        <input name="Addresses[${nextIndex}].City" class="form-control required-address" placeholder="Enter City" />
                    </div>
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
                </div>
                <div class="col-md-4 mb-3 mb-md-0">
                    <label class="form-label">Postal Code</label>
                    <div class="input-group">
                        <span class="input-group-text"><i class="fas fa-mail-bulk"></i></span>
                        <input name="Addresses[${nextIndex}].PostalCode" class="form-control" placeholder="Enter Postal Code" />
                    </div>
                </div>
                <div class="col-md-4">
                    <label class="form-label">Address Type</label>
                    <select name="Addresses[${nextIndex}].AddressType" class="form-select">
                        <option value="">Select Address Type</option>
                        ${Array.from(document.querySelector('select[name="Addresses[0].AddressType"]').options)
                .map((opt) => `<option value="${opt.value}">${opt.text}</option>`)
                .join("")}
                    </select>
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
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Last Name</label>
                        <div class="input-group">
                            <span class="input-group-text"><i class="fas fa-user"></i></span>
                            <input name="Contacts[${nextIndex}].LastName" class="form-control" placeholder="Enter last name" />
                        </div>
                    </div>
                </div>
                <div class="row mb-3">
                    <div class="col-md-6 mb-3 mb-md-0">
                        <label class="form-label">Email</label>
                        <div class="input-group">
                            <span class="input-group-text"><i class="fas fa-envelope"></i></span>
                            <input name="Contacts[${nextIndex}].Email" class="form-control" placeholder="Enter email" type="email" />
                        </div>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Phone <span class="text-danger">*</span></label>
                        <div class="input-group">
                            <span class="input-group-text"><i class="fas fa-phone"></i></span>
                            <input name="Contacts[${nextIndex}].Phone" class="form-control required-contact" placeholder="Enter phone number" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-6 mb-3 mb-md-0">
                        <label class="form-label">Contact Photo</label>
                        <div class="input-group">
                            <span class="input-group-text"><i class="fas fa-camera"></i></span>
                            <input type="file" name="ContactPicture" class="form-control" accept="image/*" onchange="previewContactImage(this, ${nextIndex})" />
                        </div>
                        <div class="mt-2" style="max-width: 150px;">
                            <img class="img-fluid d-none border rounded contact-preview" id="contact-preview-${nextIndex}" src="#" alt="Preview" />
                        </div>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Contact Type</label>
                        <select name="Contacts[${nextIndex}].ContactType" class="form-select">
                            <option value="">Select Contact Type</option>
                            ${Array.from(document.querySelector('select[name="Contacts[0].ContactType"]').options)
                .map((opt) => `<option value="${opt.value}">${opt.text}</option>`)
                .join("")}
                        </select>
                    </div>
                </div>
            </div>
        `;

        contactsContainer.appendChild(newContactForm);
        if ($.validator && $.validator.unobtrusive) {
            $.validator.unobtrusive.parse(newContactForm);
        }

        // Add remove handler
        newContactForm.querySelector(".remove-contact").addEventListener("click", () => {
            newContactForm.remove();
        });
    });

    // Form submission validation
    form.addEventListener("submit", (e) => {
        if (!validateCurrentStep()) {
            e.preventDefault(); // Prevent form submission if validation fails
            alert('Please complete all required fields in the current step before submitting.');
        } else {
            // Clear the stored step on successful submission
            sessionStorage.removeItem("currentFormStep");
        }
    });

    // Initialize the form by showing the appropriate step
    // If there are validation errors, try to restore the last step from session storage
    if (hasValidationErrors) {
        const savedStep = parseInt(sessionStorage.getItem("currentFormStep") || "0");
        showStep(savedStep);
    } else {
        // Otherwise start at the first step
        showStep(0);
        // Clear any previously stored step
        sessionStorage.removeItem("currentFormStep");
    }

    // Add event handlers to existing remove buttons
    document.querySelectorAll(".remove-address").forEach((button) => {
        button.addEventListener("click", function () {
            if (document.querySelectorAll(".address-form").length > 1) {
                this.closest(".address-form").remove();
            }
        });
    });

    document.querySelectorAll(".remove-contact").forEach((button) => {
        button.addEventListener("click", function () {
            if (document.querySelectorAll(".contact-form").length > 1) {
                this.closest(".contact-form").remove();
            }
        });
    });
    
    // Auto-fill button functionality
    document.querySelectorAll(".auto-fill").forEach((button) => {
        button.addEventListener("click", () => {
            const stepNumber = button.getAttribute("data-step");
            console.log("Auto-filling step:", stepNumber);
            autoFillStep(stepNumber);
        });
    });
});

// Function to auto-fill fields based on the current step
const autoFillStep = (step) => {
    console.log("Auto-filling step:", step);
    switch (step) {
        case "1": // Step 1: Member Details
            document.querySelector('input[name="MemberName"]').value = "Tech Giants Inc.";
            document.querySelector('select[name="CompanySize"]').value = "Medium";
            document.querySelector('input[name="CompanyWebsite"]').value = "https://www.techgiants.com";
            document.querySelector('input[name="ContactedBy"]').value = "John Doe";

            // Select first membership type
            const membershipCheckboxes = document.querySelectorAll('.membership-type');
            if (membershipCheckboxes.length > 0) {
                membershipCheckboxes[0].checked = true;
            }
            break;

        case "2": // Step 2: Dates and Notes
            document.querySelector('input[name="MemberSince"]').value = "2023-01-01";
            if (document.querySelector('input[name="LastContactDate"]')) {
                document.querySelector('input[name="LastContactDate"]').value = "2023-10-15";
            }
            document.querySelector('textarea[name="Notes"]').value = "This is a sample note for testing purposes.";
            break;

        case "3": // Step 3: Addresses
            const addressFields = document.querySelectorAll('.address-form');
            addressFields.forEach((address, index) => {
                const streetInput = address.querySelector('input[name$="Street"]');
                const cityInput = address.querySelector('input[name$="City"]');
                const provinceSelect = address.querySelector('select[name$="Province"]');
                const postalCodeInput = address.querySelector('input[name$="PostalCode"]');
                const addressTypeSelect = address.querySelector('select[name$="AddressType"]');

                if (streetInput) streetInput.value = `${index + 1} Main St`;
                if (cityInput) cityInput.value = "Toronto";
                if (provinceSelect && provinceSelect.options.length > 0) {
                    // Try to select Ontario (ON)
                    for (let i = 0; i < provinceSelect.options.length; i++) {
                        if (provinceSelect.options[i].text.includes("Ontario") || 
                            provinceSelect.options[i].text.includes("ON")) {
                            provinceSelect.value = provinceSelect.options[i].value;
                            break;
                        }
                    }
                    // If no match found, select the first non-empty option
                    if (!provinceSelect.value) {
                        for (let i = 0; i < provinceSelect.options.length; i++) {
                            if (provinceSelect.options[i].value) {
                                provinceSelect.value = provinceSelect.options[i].value;
                                break;
                            }
                        }
                    }
                }
                if (postalCodeInput) postalCodeInput.value = "M1M 1M1";
                if (addressTypeSelect && addressTypeSelect.options.length > 0) {
                    // Try to select Business or HeadOffice
                    for (let i = 0; i < addressTypeSelect.options.length; i++) {
                        if (addressTypeSelect.options[i].text.includes("Business") || 
                            addressTypeSelect.options[i].text.includes("Head")) {
                            addressTypeSelect.value = addressTypeSelect.options[i].value;
                            break;
                        }
                    }
                    // If no match found, select the first non-empty option
                    if (!addressTypeSelect.value) {
                        for (let i = 0; i < addressTypeSelect.options.length; i++) {
                            if (addressTypeSelect.options[i].value) {
                                addressTypeSelect.value = addressTypeSelect.options[i].value;
                                break;
                            }
                        }
                    }
                }
            });
            break;

        case "4": // Step 4: Contacts and Industries
            const contactFields = document.querySelectorAll('.contact-form');
            contactFields.forEach((contact, index) => {
                const firstNameInput = contact.querySelector('input[name$="FirstName"]');
                const lastNameInput = contact.querySelector('input[name$="LastName"]');
                const emailInput = contact.querySelector('input[name$="Email"]');
                const phoneInput = contact.querySelector('input[name$="Phone"]');
                const contactTypeSelect = contact.querySelector('select[name$="ContactType"]');

                if (firstNameInput) firstNameInput.value = "John";
                if (lastNameInput) lastNameInput.value = "Doe";
                if (emailInput) emailInput.value = "john.doe@example.com";
                if (phoneInput) phoneInput.value = "1234567890";
                if (contactTypeSelect && contactTypeSelect.options.length > 0) {
                    // Try to select Primary
                    for (let i = 0; i < contactTypeSelect.options.length; i++) {
                        if (contactTypeSelect.options[i].text.includes("Primary")) {
                            contactTypeSelect.value = contactTypeSelect.options[i].value;
                            break;
                        }
                    }
                    // If no match found, select the first non-empty option
                    if (!contactTypeSelect.value) {
                        for (let i = 0; i < contactTypeSelect.options.length; i++) {
                            if (contactTypeSelect.options[i].value) {
                                contactTypeSelect.value = contactTypeSelect.options[i].value;
                                break;
                            }
                        }
                    }
                }
            });

            // Select industries
            const industrySelect = document.querySelector('select[name="SelectedIndustryIds"]');
            if (industrySelect && industrySelect.options.length > 0) {
                // Select the first option
                industrySelect.options[0].selected = true;
                // If there's a second option, select it too
                if (industrySelect.options.length > 1) {
                    industrySelect.options[1].selected = true;
                }
            }
            break;

        default:
            console.warn("Invalid step for auto-fill:", step);
    }
};

// Helper function to get random items from an array
const getRandomItems = (array, count) => {
    const shuffled = array.sort(() => 0.5 - Math.random()); // Shuffle the array
    return shuffled.slice(0, count); // Return the first `count` items
};