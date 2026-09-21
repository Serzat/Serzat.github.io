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


function renderProjecten(projectenLijst) {

    const projectLijst = document.querySelector("#project-list");

    // Verwijdert de huidige projecten voordat de lijst opnieuw wordt getoond
    projectLijst.innerHTML = "";

    projectenLijst.forEach((project) => {

        const article = document.createElement("article");

        const titel = document.createElement("h3");
        titel.textContent = project.naam;
        article.appendChild(titel);

        const periode = document.createElement("p");
        periode.innerHTML = "<strong>Periode:</strong> " + project.periode;
        article.appendChild(periode);

        const beschrijving = document.createElement("p");
        beschrijving.textContent = project.beschrijving;
        article.appendChild(beschrijving);

        const techniekenTitel = document.createElement("h4");
        techniekenTitel.textContent = "Gebruikte technieken";
        article.appendChild(techniekenTitel);

        const techniekenLijst = document.createElement("ul");

        project.technieken.forEach((techniek) => {

            const lijstItem = document.createElement("li");
            lijstItem.textContent = techniek;

            techniekenLijst.appendChild(lijstItem);
        });

        article.appendChild(techniekenLijst);

        const githubLink = document.createElement("a");
        githubLink.textContent = "GitHub link";
        githubLink.href = project.github;
        article.appendChild(githubLink);

        projectLijst.appendChild(article);
    });
}


function sorteerNieuwsteEerst() {

    projecten.sort((a, b) => b.sorteerWaarde - a.sorteerWaarde);

    renderProjecten(projecten);
}


function sorteerOudsteEerst() {

    projecten.sort((a, b) => a.sorteerWaarde - b.sorteerWaarde);

    renderProjecten(projecten);
}


const nieuwsteKnop = document.querySelector("#nieuwste-eerst");
const oudsteKnop = document.querySelector("#oudste-eerst");

nieuwsteKnop.addEventListener("click", sorteerNieuwsteEerst);
oudsteKnop.addEventListener("click", sorteerOudsteEerst);


renderProjecten(projecten);