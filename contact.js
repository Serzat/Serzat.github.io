// Selects the contact form from the HTML
const form = document.querySelector("#contact-form");


// Creates a list with the form fields that need validation
const velden = [

    // Validation settings for the name field
    {
        id: "naam",
        boodschap: "Vul minimaal 2 tekens in."
    },

    // Validation settings for the email field
    {
        id: "email",
        boodschap: "Vul een geldig e-mailadres in."
    },

    // Validation settings for the message field
    {
        id: "bericht",
        boodschap: "Schrijf minimaal 10 tekens."
    }
];


// Creates a function that validates one form field
function valideerVeld(veld) {

    // Selects the input field using the id from the velden array
    const input = document.querySelector(`#${veld.id}`);

    // Selects the error message that belongs to the input field
    const foutmelding = document.querySelector(`#${veld.id}-error`);

    // Checks if the input follows the HTML validation rules
    const geldig = input.checkValidity();

    // Sets aria-invalid to true when the field is invalid
    // This helps assistive technology understand that there is an error
    input.setAttribute("aria-invalid", String(!geldig));

    // Checks if the field is valid
    if (geldig) {

        // Removes the error message when the field is valid
        foutmelding.textContent = "";

    } else {

        // Shows the correct error message when the field is invalid
        foutmelding.textContent = veld.boodschap;
    }

    // Returns true or false so the program knows if the field is valid
    return geldig;
}


// Runs this code when the user clicks the submit button
form.addEventListener("submit", (event) => {

    // Prevents the form from actually being sent or refreshing the page
    event.preventDefault();

    // Starts with the assumption that all fields are valid
    let alleGeldig = true;


    // Goes through every field in the velden array
    velden.forEach((veld) => {

        // Validates the current field
        const geldig = valideerVeld(veld);

        // Checks if the current field is invalid
        if (!geldig) {

            // Marks the complete form as invalid
            alleGeldig = false;
        }
    });


    // Selects the element where the general form status will be shown
    const status = document.querySelector("#form-status");


    // Checks if one or more fields are invalid
    if (!alleGeldig) {

        // Shows a general error message
        status.textContent = "Er zijn nog fouten in het formulier.";

        // Stops the function here
        return;
    }


    // Shows a success message when all fields are valid
    status.textContent = "Bericht verzonden! Bedankt.";


    // Clears all fields after a successful submission
    form.reset();
});