# Önskemål om feedback: att skapa och parkera fordon

Hej! Jag skulle gärna vilja ha er feedback på fordonsklasserna och på flödet för att skapa fordon i garageapplikationen. Jag har byggt en factory för varje fordonstyp, men när jag provade att skapa och parkera ett nytt fordon i konsolen kändes det som väldigt mycket arbete för personalen. Jag vill veta vilka uppgifter som faktiskt behöver anges direkt och hur ni skulle förenkla flödet.

## Så fungerar det i dag

1. Användaren kan välja **Create and list vehicles** i huvudmenyn eller **Create and park vehicle** inne i ett garage. Det går även att välja en särskild parkeringsplats eller läsa in ett fordon från en JSON-fil.
2. I `VehicleCreator` väljer användaren fordonstyp, till exempel bil, motorcykel, flygplan, buss, båt eller lastbil. Varje typ har en egen draft där fälten fylls i via en meny. Motorer kan läggas till, men ett fordon kan också skapas utan motor.
3. `VehicleDraft.GetMissingFields()` och motsvarande metod för fordonstypen kontrollerar vilka fält som saknas. Gemensamma obligatoriska fält är bland annat registreringsnummer, registreringsland, tillverkare, modell, år, färg, vikt och mått. För en bil krävs även VIN, karosstyp, antal dörrar och säten. Andra fordonstyper har sina egna fält, till exempel HIN för båt eller skrovets serienummer för flygplan.
4. `VehicleDraftMapper` skickar draftens värden till rätt factory, exempelvis `CarFactory.CreateCar(...)`. Factoryn skapar rätt `Vehicle`-subklass med dess specifikation, mått och eventuella komponenter som kabin eller cockpit. Regler i domänen kan avvisa ogiltiga kombinationer.
5. `VehicleCreationService` kontrollerar bland annat om registreringsnumret redan finns. Om användaren skapade fordonet från ett garage försöker tjänsten parkera det där, eventuellt på den valda platsen. Fordonet läggs i katalogen först när skapandet och eventuell parkering har lyckats.
6. Ett fordon som bara har skapats kan parkeras senare via garagemenyn. Då visar systemet fordon som är tillgängliga och får plats. Katalogen visar status som `[AVAILABLE]` eller `[PARKED]`.

Kod att börja titta på: `Garage.Console/Menus/VehicleCreator.cs`, `Garage.Console/Drafts/VehicleDraft.cs`, de typanpassade drafts, `Garage.Console/Mapping/VehicleDraftMapper.cs`, `Vehicle/Factories/` och `Garage/Services/VehicleCreationService.cs`.

## Det jag vill ha feedback på

- Hur skulle ni göra det snabbt för en anställd att ta emot och parkera ett fordon som inte redan finns i katalogen?
- Vilka uppgifter måste personalen ange vid ankomst, och vilka kan vara frivilliga eller kompletteras senare? Räcker exempelvis registreringsnummer, registreringsland och fordonstyp för ett första steg?
- Behöver VIN, HIN och skrov-/chassinummer vara obligatoriska när man bara vill parkera, eller bör de vara valfria?
- Ska vikt och mått krävas direkt, särskilt eftersom vissa garage har parkeringsplatser med storleksgränser? Hur bör systemet hantera ett fordon vars mått ännu inte är kända?
- Är det bättre att ha ett kort flöde för **skapa och parkera** och ett separat flöde för fullständig fordonsregistrering? Finns det en enklare lösning?
- Är ansvaret rimligt fördelat mellan draft, mapper, factory, domänklasser och `VehicleCreationService`? Factory-metoderna tar många argument; hur skulle ni utforma dem?

Jag är särskilt intresserad av konkreta förslag på vilka fält ni skulle visa först i konsolen och vad som bör hända om information saknas. Ni behöver inte implementera något för att ge feedback; jag vill gärna diskutera utformningen innan jag ändrar modellen eller menyerna.
