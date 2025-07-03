# Endpoint de Fréquentation - Théâtre Vousse

## Description
Nouvel endpoint ajouté à l'API pour mesurer la fréquentation du théâtre Vousse en récupérant le nombre de billets vendus pour chaque représentation d'un spectacle donné.

## Endpoint

### URL
```
GET /api/billets/frequentation/{spectacleId}
```

### Paramètres
- `spectacleId` (int) : ID du spectacle pour lequel récupérer les données de fréquentation

### Réponse
Retourne un fichier CSV contenant les informations suivantes :
- `id_spectacle` : ID du spectacle
- `titre_spectacle` : Titre du spectacle
- `date_representation` : Date de la représentation (format YYYY-MM-DD)
- `nombre_billets_vendus` : Nombre de billets vendus pour cette représentation

### Exemple d'utilisation
```
GET /api/billets/frequentation/1
```

### Exemple de réponse CSV
```csv
id_spectacle,titre_spectacle,date_representation,nombre_billets_vendus
1,"Roméo et Juliette",2024-01-15,25
1,"Roméo et Juliette",2024-01-20,30
1,"Roméo et Juliette",2024-01-25,18
```

## Fichiers modifiés/créés

### 1. Services/DTOs/FrequentationDTO.cs
```csharp
public class FrequentationDTO
{
    public int SpectacleId { get; set; }
    public string TitreSpectacle { get; set; } = string.Empty;
    public DateOnly DateRepresentation { get; set; }
    public int NombreBilletsVendus { get; set; }
}
```

### 2. Services/Interfaces/IBilletService.cs
Ajout de la méthode :
```csharp
Task<IEnumerable<FrequentationDTO>> GetFrequentationBySpectacleAsync(int spectacleId);
```

### 3. Services/Implementations/BilletService.cs
Implémentation de la méthode qui :
- Filtre les programmations par spectacle ID
- Compte les billets pour chaque représentation
- Retourne les données formatées

### 4. WebApplication1/Controllers/BilletsController.cs
Nouvel endpoint qui :
- Récupère les données via le service
- Génère un fichier CSV avec les en-têtes appropriés
- Retourne le fichier avec le type MIME `text/csv`
- Gère les erreurs (spectacle non trouvé, erreurs serveur)

## Gestion d'erreurs

### Spectacle non trouvé
```json
{
  "message": "Aucune représentation trouvée pour ce spectacle"
}
```

### Erreur serveur
```json
{
  "message": "Erreur lors de la récupération des données de fréquentation",
  "error": "Détails de l'erreur"
}
```

## Fonctionnalités
- ✅ Récupération des données de fréquentation par spectacle
- ✅ Génération automatique de fichier CSV
- ✅ Gestion des erreurs
- ✅ Format de date standardisé (YYYY-MM-DD)
- ✅ Nom de fichier personnalisé selon l'ID du spectacle 