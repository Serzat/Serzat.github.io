const projecten = [
    {
        naam: "Online Veiling Systeem",
        periode: "september 2025 - januari 2026",
        sorteerWaarde: 202601,
        beschrijving:
            "Ik heb meegewerkt aan de basisarchitectuur van het project door UML-klassendiagrammen te maken en relaties tussen entiteiten te analyseren. Daarnaast heb ik een front-end dashboard ontworpen en gerealiseerd voor de online veilingwebsite.",
        technieken: [
            "UML-klassendiagrammen",
            "HTML en CSS",
            "GitHub"
        ],
        github: "https://github.com/ChillTick/HHS-K4-G4-WebProject"
    },
    {
        naam: "Data Engineering & AI",
        periode: "februari 2026 - maart 2026",
        sorteerWaarde: 202603,
        beschrijving:
            "Ik heb relationele modellen en sterschema's gemaakt voor een datawarehouse. Daarnaast heb ik ETL-pijplijnen opgezet om de datastroom te beheren en machine-learningmodellen ontwikkeld en getest met Python.",
        technieken: [
            "Python",
            "Relationele modellen",
            "Sterschema's",
            "ETL-pijplijnen"
        ],
        github: "https://github.com/garrickLos/GreatOutdoors-HHS-klas3-groep4"
    },
    {
        naam: "Lumi Lamp - Embedded Systems & Software",
        periode: "afgerond in januari 2025",
        sorteerWaarde: 202501,
        beschrijving:
            "Binnen dit project was mijn taak het programmeren van de tijdweergave en het alarmsysteem op de MicroBit. Samen met mijn team hebben we een slimme slaaplamp en wekker ontwikkeld met twee MicroBits en een Java-desktopapplicatie. Het systeem kon onder andere licht, temperatuur en achtergrondgeluid meten.",
        technieken: [
            "MicroBit",
            "Java",
            "Python",
            "SQLite"
        ],
        github: "https://github.com/ChillTick/Lumi"
    }
];

// Creates and displays all projects on the webpage
function renderProjecten(projectenLijst) {

    // Selects the HTML element with the id "project-list"
    const projectLijst = document.querySelector("#project-list");

    // Clears the current project list before displaying it again
    // This prevents projects from appearing multiple times
    projectLijst.innerHTML = "";

    // Goes through every project in the project list
    projectenLijst.forEach((project) => {

        // Creates a new <article> element for one project
        const article = document.createElement("article");

        // Creates the project title
        const titel = document.createElement("h3");

        // Sets the project name as the title text
        titel.textContent = project.naam;

        // Adds the title to the article
        article.appendChild(titel);


        // Creates a paragraph for the project period
        const periode = document.createElement("p");

        // Adds the period to the paragraph
        // innerHTML is used so <strong> can make "Periode:" bold
        periode.innerHTML = "<strong>Periode:</strong> " + project.periode;

        // Adds the period paragraph to the article
        article.appendChild(periode);


        // Creates a paragraph for the project description
        const beschrijving = document.createElement("p");

        // Sets the description text from the project data
        beschrijving.textContent = project.beschrijving;

        // Adds the description to the article
        article.appendChild(beschrijving);


        // Creates a heading for the technologies section
        const techniekenTitel = document.createElement("h4");

        // Sets the text of the heading
        techniekenTitel.textContent = "Gebruikte technieken";

        // Adds the heading to the article
        article.appendChild(techniekenTitel);


        // Creates an unordered list for the technologies
        const techniekenLijst = document.createElement("ul");

        // Goes through every technology used in the project
        project.technieken.forEach((techniek) => {

            // Creates a new list item
            const lijstItem = document.createElement("li");

            // Puts the technology name inside the list item
            lijstItem.textContent = techniek;

            // Adds the list item to the technologies list
            techniekenLijst.appendChild(lijstItem);
        });

        // Adds the complete technologies list to the article
        article.appendChild(techniekenLijst);


        // Creates a clickable link to the project's GitHub repository
        const githubLink = document.createElement("a");

        // Sets the visible text of the link
        githubLink.textContent = "GitHub link";

        // Sets the destination of the link
        githubLink.href = project.github;

        // Adds the GitHub link to the article
        article.appendChild(githubLink);


        // Adds the completed project article to the webpage
        projectLijst.appendChild(article);
    });
}


// Sorts the projects from newest to oldest
function sorteerNieuwsteEerst() {

    // Higher sort values come first
    projecten.sort((a, b) => b.sorteerWaarde - a.sorteerWaarde);

    // Displays the projects again in the new order
    renderProjecten(projecten);
}


// Sorts the projects from oldest to newest
function sorteerOudsteEerst() {

    // Lower sort values come first
    projecten.sort((a, b) => a.sorteerWaarde - b.sorteerWaarde);

    // Displays the projects again in the new order
    renderProjecten(projecten);
}


// Selects the "Nieuwste eerst" button from the HTML
const nieuwsteKnop = document.querySelector("#nieuwste-eerst");

// Selects the "Oudste eerst" button from the HTML
const oudsteKnop = document.querySelector("#oudste-eerst");


// Runs sorteerNieuwsteEerst when the user clicks the newest button
nieuwsteKnop.addEventListener("click", sorteerNieuwsteEerst);

// Runs sorteerOudsteEerst when the user clicks the oldest button
oudsteKnop.addEventListener("click", sorteerOudsteEerst);


// Displays the projects when the page loads for the first time
renderProjecten(projecten);