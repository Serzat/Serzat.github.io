// Selecteer de elementen
const heading = document.querySelector("h1");
const tekst = document.querySelector("p");
const output = document.getElementById("output");

// Pas de heading aan met textContent
heading.textContent = "JavaScript leren";

// Pas de heading aan met innerHTML
heading.innerHTML = "JavaScript <strong>fundamentals</strong>";

// Verander de kleur van de heading
heading.style.color = "blue";

// Maak een nieuw p-element
const nieuweTekst = document.createElement("p");

// Voeg tekst toe aan het nieuwe element
nieuweTekst.textContent = "Deze tekst is gemaakt met JavaScript.";

// Voeg het element toe aan output
output.appendChild(nieuweTekst);

// Verwijder het element
nieuweTekst.remove();