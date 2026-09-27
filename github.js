// Selects the element where loading and error messages will be shown
const githubStatus = document.querySelector("#github-status");

// Selects the element where the GitHub information will be displayed
const githubData = document.querySelector("#github-data");


// Creates a function that gets data from the GitHub API
async function haalGithubDataOp() {

    // Shows a loading message while the data is being requested
    githubStatus.textContent = "Gegevens laden...";

    try {

        // Sends a GET request to the GitHub API
        const response = await fetch("https://api.github.com/users/Serzat");

        // Converts the JSON response into a JavaScript object
        const data = await response.json();


        // Removes the loading message
        githubStatus.textContent = "";


        // Creates a paragraph for the GitHub username
        const gebruikersnaam = document.createElement("p");

        // Adds the username from the API data
        gebruikersnaam.textContent = "Gebruikersnaam: " + data.login;

        // Adds the paragraph to the webpage
        githubData.appendChild(gebruikersnaam);


        // Creates a paragraph for the number of public repositories
        const repositories = document.createElement("p");

        // Adds the number of public repositories from the API data
        repositories.textContent = "Publieke repositories: " + data.public_repos;

        // Adds the paragraph to the webpage
        githubData.appendChild(repositories);


        // Creates a paragraph for the number of followers
        const volgers = document.createElement("p");

        // Adds the number of followers from the API data
        volgers.textContent = "Volgers: " + data.followers;

        // Adds the paragraph to the webpage
        githubData.appendChild(volgers);


        // Creates a link to the GitHub profile
        const profielLink = document.createElement("a");

        // Sets the visible text of the link
        profielLink.textContent = "Bekijk mijn GitHub-profiel";

        // Sets the destination of the link using the URL from the API
        profielLink.href = data.html_url;

        // Adds the link to the webpage
        githubData.appendChild(profielLink);

    } catch (error) {

        // Shows an error message if the API request fails
        githubStatus.textContent = "GitHub-gegevens konden niet worden geladen.";
    }
}


// Runs the function when the page loads
haalGithubDataOp();