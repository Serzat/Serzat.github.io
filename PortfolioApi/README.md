# PortfolioApi – WPFW Opdracht 3

## 1. Beschrijving

Voor WPFW Opdracht 3 heb ik een backend ontwikkeld voor mijn portfoliosite met ASP.NET Core Web API.

De API maakt het mogelijk om projecten en blogposts aan te maken, op te vragen, te wijzigen en te verwijderen (CRUD).

De gegevens worden met Entity Framework Core opgeslagen in een SQL Server-database.

## 2. Gebruikte technologieën

- .NET 10
- ASP.NET Core Web API (Controllers)
- C#
- Entity Framework Core
- SQL Server Express
- Swagger UI

## 3. Projectstructuur

De applicatie is opgebouwd met een meerlagenarchitectuur.

- **Controllers:** Ontvangen HTTP-verzoeken en geven passende HTTP-antwoorden terug.
- **Services:** Voeren de applicatielogica uit en gebruiken de DbContext voor databasebewerkingen.
- **Models:** Bevatten de entiteiten Project en Blogpost.
- **DTOs:** Bepalen welke gegevens de API ontvangt en terugstuurt.
- **Data:** Bevat PortfolioDbContext voor de communicatie met de database.
- **Migrations:** Bevatten de databasewijzigingen die door Entity Framework Core zijn gegenereerd.
- **Program.cs:** Registreert de services, DbContext en Swagger.

Er is gekozen voor controllers met gescheiden services, zodat de HTTP-afhandeling, applicatielogica en datatoegang overzichtelijk blijven.

## 4. API-endpoints

### Projecten

| Methode | Endpoint           | Beschrijving           |
| ------- | ------------------ | ---------------------- |
| GET     | /api/projects      | Alle projecten ophalen |
| GET     | /api/projects/{id} | Eén project ophalen    |
| POST    | /api/projects      | Nieuw project aanmaken |
| PUT     | /api/projects/{id} | Project wijzigen       |
| DELETE  | /api/projects/{id} | Project verwijderen    |

### Blogposts

| Methode | Endpoint            | Beschrijving             |
| ------- | ------------------- | ------------------------ |
| GET     | /api/blogposts      | Alle blogposts ophalen   |
| GET     | /api/blogposts/{id} | Eén blogpost ophalen     |
| POST    | /api/blogposts      | Nieuwe blogpost aanmaken |
| PUT     | /api/blogposts/{id} | Blogpost wijzigen        |
| DELETE  | /api/blogposts/{id} | Blogpost verwijderen     |

## 5. Installatie en configuratie

### Benodigdheden

- .NET 10 SDK
- SQL Server (bijvoorbeeld SQL Server Express)
- SQL Server Management Studio (optioneel, voor databasebeheer)
- Entity Framework Core CLI (`dotnet-ef`)

### Databaseverbinding

De applicatie gebruikt SQL Server met Windows Authentication.

De connectionstring wordt lokaal opgeslagen via .NET User Secrets en staat niet rechtstreeks in de broncode.

Open een terminal in de map PortfolioApi.

Initialiseer User Secrets als dit nog niet is gedaan:

`dotnet user-secrets init`

Configureer daarna de connectionstring:

`dotnet user-secrets set "ConnectionStrings:PortfolioDb" 'Server=SERVERNAAM\SQLEXPRESS;Database=PortfolioDb;Trusted_Connection=True;TrustServerCertificate=True;'`

Vervang `SERVERNAAM\SQLEXPRESS` door de naam van je eigen SQL Server-instance.

Deze configuratie is bedoeld voor de lokale ontwikkelomgeving.

### Database aanmaken

De database wordt aangemaakt met EF Core Migrations.

Voer in de projectmap uit:

`dotnet ef database update`

Hiermee worden de bestaande migrations toegepast en worden de benodigde tabellen aangemaakt of bijgewerkt.

De belangrijkste tabellen zijn:

- Projecten
- Blogposts
- \_\_EFMigrationsHistory

## 6. Applicatie starten

Open een terminal in de map PortfolioApi en voer uit:

`dotnet restore`

Start vervolgens de applicatie:

`dotnet run`

De applicatie toont in de terminal op welk localhost-adres deze draait.

Open dat adres met `/swagger` erachter om Swagger UI te gebruiken.

Swagger UI is beschikbaar wanneer de applicatie in de Development-omgeving draait.

## 7. Testen met Swagger

De API-endpoints zijn getest met Swagger UI.

Voor zowel projecten als blogposts is de volgende volgorde gebruikt:

1. POST: een nieuw item aanmaken.
2. GET: controleren of het item in de lijst staat.
3. GET op ID: het aangemaakte item ophalen.
4. PUT: de gegevens van het item wijzigen.
5. GET: controleren of de wijzigingen zijn opgeslagen.
6. De applicatie opnieuw starten en controleren of de gegevens behouden blijven.
7. DELETE: het item verwijderen.
8. GET op hetzelfde ID: controleren of 404 Not Found wordt teruggegeven.

Hiermee is getest of de CRUD-functionaliteit werkt en de gegevens in SQL Server bewaard blijven.
