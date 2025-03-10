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
    const stepIndicators = document.querySelectorAll(".step-indicator");
    const form = document.getElementById("multiStepForm");

    let currentStep = 0;
    const totalSteps = steps.length;

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

        stepIndicators.forEach((indicator, index) => {
            indicator.classList.toggle("bg-primary", index === step);
            indicator.classList.toggle("text-white", index === step);
            indicator.classList.toggle("bg-light", index !== step);
            indicator.classList.toggle("border", index !== step);
        });
    };

    // Validate form fields for current step
    const validateCurrentStep = () => {
        const currentStepElement = steps[currentStep];
        let isValid = true;

        currentStepElement.querySelectorAll(".required, .required-address, .required-contact").forEach((field) => {
            const errorSpan = field.closest(".form-group")?.querySelector(".field-validation-error");
            if (!field.value.trim()) {
                isValid = false;
                field.classList.add("is-invalid");
                if (errorSpan) errorSpan.classList.remove("d-none");
            } else {
                field.classList.remove("is-invalid");
                if (errorSpan) errorSpan.classList.add("d-none");
            }
        });

        if (currentStepElement.id === "step1") {
            const membershipChecked = document.querySelectorAll(".membership-type:checked").length > 0;
            const membershipError = document.querySelector(".membership-error");
            if (membershipError) membershipError.classList.toggle("d-none", membershipChecked);
            isValid = isValid && membershipChecked;
        }

        return isValid;
    };

    // Navigate to next step
    nextButtons.forEach((button) => {
        button.addEventListener("click", () => {
            if (validateCurrentStep() && currentStep < steps.length - 1) {
                showStep(currentStep + 1);
            }
        });
    });

    // Navigate to previous step
    prevButtons.forEach((button) => {
        button.addEventListener("click", () => {
            if (currentStep > 0) {
                showStep(currentStep - 1);
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

    // Add Existing Contact functionality
    document.getElementById("add-existing-contact").addEventListener("click", () => {
        const existingContactSelect = document.getElementById("existingContact");
        const selectedContactId = existingContactSelect.value;

        if (selectedContactId) {
            const selectedContact = Array.from(existingContactSelect.options).find(option => option.value === selectedContactId);
            const contactDetails = selectedContact.text.split(" - ");
            const [name, email] = contactDetails;
            const [firstName, lastName] = name.split(" ");

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
                                <input name="Contacts[${nextIndex}].FirstName" class="form-control required-contact" value="${firstName}" readonly />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Last Name</label>
                            <div class="input-group">
                                <span class="input-group-text"><i class="fas fa-user"></i></span>
                                <input name="Contacts[${nextIndex}].LastName" class="form-control" value="${lastName}" readonly />
                            </div>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col-md-6 mb-3 mb-md-0">
                            <label class="form-label">Email</label>
                            <div class="input-group">
                                <span class="input-group-text"><i class="fas fa-envelope"></i></span>
                                <input name="Contacts[${nextIndex}].Email" class="form-control" value="${email}" readonly />
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
        }
    });

    // Handle contact option change
    document.querySelectorAll('input[name="contactOption"]').forEach((radio) => {
        radio.addEventListener("change", (e) => {
            const newContactForm = document.getElementById("newContactForm");
            const existingContactForm = document.getElementById("existingContactForm");

            if (e.target.value === "new") {
                newContactForm.style.display = "block";
                existingContactForm.style.display = "none";
            } else {
                newContactForm.style.display = "none";
                existingContactForm.style.display = "block";
            }
        });
    });

    // Form submission validation
    form.addEventListener("submit", (e) => {
        if (!validateCurrentStep()) {
            e.preventDefault(); // Prevent form submission if validation fails
            alert('Please complete all required fields in the current step.');
        }
    });

    // Initialize the form by showing the first step
    showStep(0);

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
});
document.addEventListener("DOMContentLoaded", () => {
    // Auto-fill button functionality
    document.querySelectorAll(".auto-fill").forEach((button) => {
        button.addEventListener("click", () => {
            const currentStep = document.querySelector(".form-step.active").id.replace("step", ""); // Get current step number
            autoFillStep(currentStep);
        });
    });

    // Function to auto-fill fields based on the current step
    const autoFillStep = (step) => {
        switch (step) {
            case "1": // Step 1: Member Details
                document.querySelector('input[name="MemberName"]').value = "Tech Giants Inc.";
                document.querySelector('select[name="CompanySize"]').value = "2"; // Medium
                document.querySelector('input[name="CompanyWebsite"]').value = "https://www.techgiants.com";
                document.querySelector('input[name="ContactedBy"]').value = "John Doe";

                // Hardcode selection of 1st and 4th membership types
                const membershipCheckboxes = document.querySelectorAll('.membership-type');
                if (membershipCheckboxes.length >= 4) {
                    membershipCheckboxes[0].checked = true; // 1st membership type
                    membershipCheckboxes[3].checked = true; // 4th membership type
                }
                break;

            case "2": // Step 2: Dates and Notes
                document.querySelector('input[name="MemberSince"]').value = "2020-01-01";
                document.querySelector('input[name="LastContactDate"]').value = "2023-10-15";
                document.querySelector('textarea[name="Notes"]').value = "This is a sample note for testing purposes.";
                break;

            case "3": // Step 3: Addresses
                const addressFields = document.querySelectorAll('.address-form');
                addressFields.forEach((address, index) => {
                    address.querySelector('input[name$="Street"]').value = `${index + 1} Main St`;
                    address.querySelector('input[name$="City"]').value = "Toronto";
                    address.querySelector('input[name$="PostalCode"]').value = "M1M 1M1";
                    // Do not fill Province or AddressType
                });
                break;

            case "4": // Step 4: Contacts and Industries
                const contactFields = document.querySelectorAll('.contact-form');
                contactFields.forEach((contact, index) => {
                    contact.querySelector('input[name$="FirstName"]').value = "John";
                    contact.querySelector('input[name$="LastName"]').value = "Doe";
                    contact.querySelector('input[name$="Email"]').value = "JohnDoe@example.com";
                    contact.querySelector('input[name$="Phone"]').value = "1234567890";
                    // Do not fill ContactType
                });

                // Randomly select 2 industries
                const industrySelect = document.querySelector('select[name="SelectedIndustryIds"]');
                const industryOptions = Array.from(industrySelect.options);
                const randomIndustries = getRandomItems(industryOptions, 2);
                randomIndustries.forEach((option) => option.selected = true);
                break;

            default:
                console.warn("Invalid step for auto-fill.");
        }
    };

    // Helper function to get random items from an array
    const getRandomItems = (array, count) => {
        const shuffled = array.sort(() => 0.5 - Math.random()); // Shuffle the array
        return shuffled.slice(0, count); // Return the first `count` items
    };
});