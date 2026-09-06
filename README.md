# GtsTest

API backend de gestion de tirs de mine. Un tir (`blast`) regroupe plusieurs trous (`holes`). Un trou doit etre charge avant que le tir puisse etre effectue. L'avancement est conserve sous forme d'evenements.

## Prerequis

- Docker et Docker Compose
- `curl` ou un client HTTP

## Lancer le projet avec Docker

Depuis la racine du projet :

```bash
docker compose up --build
```

L'API est disponible sur `http://localhost:5050`.

Pour lancer le conteneur en arriere-plan :

```bash
docker compose up --build -d
```

Pour arreter et supprimer le conteneur :

```bash
docker compose down
```

Les donnees sont stockees en memoire uniquement et sont perdues lorsque le conteneur est recree ou redemarre.

## Documentation interactive

Swagger UI est disponible a l'adresse :

```text
http://localhost:5050/swagger
```

Le document OpenAPI est disponible a l'adresse :

```text
http://localhost:5050/swagger/v1/swagger.json
```

## API

Dans les exemples suivants :

```bash
BASE_URL=http://localhost:5050
```

### Creer un tir

`POST /Blasts/blasts`

Requete :

```bash
curl -X POST "$BASE_URL/Blasts/blasts" \
  -H "Content-Type: application/json" \
  -d '{"name":"Tir 1"}'
```

Reponse `200 OK` :

```json
{
  "blastId": "11111111-1111-1111-1111-111111111111"
}
```

Le tir est cree avec le statut `Planned` et un evenement `BlastCreated` est ajoute.

### Ajouter un trou a un tir

`POST /Blasts/blasts/{blastId}/holes`

Requete :

```bash
curl -X POST "$BASE_URL/Blasts/blasts/11111111-1111-1111-1111-111111111111/holes" \
  -H "Content-Type: application/json" \
  -d '{
    "name":"Trou 1",
    "position":{"x":1.5,"y":2.0,"z":3.0},
    "direction":45,
    "inclination":-10
  }'
```

Reponse `200 OK` :

```json
{
  "holeId": "22222222-2222-2222-2222-222222222222"
}
```

Un evenement `HoleAdded` est ajoute.

Reponse si le tir n'existe pas : `404 Not Found`.

### Charger un trou

`PUT /Blasts/blasts/{blastId}/holes/{holeId}/charge`

Requete :

```bash
curl -X PUT "$BASE_URL/Blasts/blasts/11111111-1111-1111-1111-111111111111/holes/22222222-2222-2222-2222-222222222222/charge"
```

Reponse `200 OK` : aucun contenu.

Le trou passe au statut `Charged` et un evenement `HoleCharged` est ajoute.

Reponses d'erreur :

- `404 Not Found` si le trou n'existe pas ;
- `400 Bad Request` si le trou est deja `Charged` ou `Ready`.

### Effectuer un tir

`POST /blasts/{blastId}/fire`

Requete :

```bash
curl -X POST "$BASE_URL/blasts/11111111-1111-1111-1111-111111111111/fire"
```

Reponse `200 OK` : aucun contenu.

Le tir passe au statut `Blasted`, `dateBlasted` est renseignee et un evenement `BlastFired` est ajoute.

Le tir ne peut etre effectue que si tous ses trous sont `Charged` ou `Ready`.

Reponses d'erreur :

- `404 Not Found` si le tir n'existe pas ;
- `400 Bad Request` si un trou n'est pas charge ou si le tir est deja effectue.

### Consulter un tir

`GET /Blasts/blasts/{blastId}`

Requete :

```bash
curl "$BASE_URL/Blasts/blasts/11111111-1111-1111-1111-111111111111"
```

Reponse `200 OK` :

```json
{
  "id":"11111111-1111-1111-1111-111111111111",
  "name":"Tir 1",
  "status":"Planned",
  "dateBlasted":null,
  "holes":[
    {
      "id":"22222222-2222-2222-2222-222222222222",
      "name":"Trou 1",
      "position":{"x":1.5,"y":2.0,"z":3.0},
      "direction":45,
      "inclination":-10,
      "status":"Charged"
    }
  ]
}
```

Reponse si le tir n'existe pas : `404 Not Found`.

### Consulter l'historique d'un tir

`GET /blasts/{blastId}/history`

Cette route retourne les evenements du tir et les evenements de chacun de ses trous, tries par date d'occurrence.

Requete :

```bash
curl "$BASE_URL/blasts/11111111-1111-1111-1111-111111111111/history"
```

Reponse `200 OK` :

```json
[
  {
    "id":"33333333-3333-3333-3333-333333333333",
    "eventType":"BlastCreated",
    "aggregateId":"11111111-1111-1111-1111-111111111111",
    "occurredOn":"2026-09-07T10:00:00+00:00",
    "aggregateType":"Blast"
  },
  {
    "id":"44444444-4444-4444-4444-444444444444",
    "eventType":"HoleAdded",
    "aggregateId":"22222222-2222-2222-2222-222222222222",
    "occurredOn":"2026-09-07T10:01:00+00:00",
    "aggregateType":"Hole"
  }
]
```

Reponse si le tir n'existe pas : `404 Not Found`.

## Ordre d'utilisation recommande

1. Creer un tir.
2. Ajouter un ou plusieurs trous.
3. Charger chaque trou.
4. Effectuer le tir.
5. Consulter le tir ou son historique.

## Structure du projet

- `src/Api` : API HTTP, contrôleur et configuration Swagger.
- `src/Commands` : commandes de creation et de modification.
- `src/Query` : requetes de lecture et construction des DTO.
- `src/Core` : modeles, repositories en memoire et evenements.
- `tests` : tests unitaires.
