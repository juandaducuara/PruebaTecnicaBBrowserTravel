# Guía de Pruebas - OutletRentalCars API

## Requisitos previos

1. Tener instalado .NET 9 SDK
2. Tener MySQL corriendo en `localhost:3306` (usuario `root`, sin contraseña)
3. Tener MongoDB corriendo en `localhost:27017` (opcional, la API funciona sin él)

## Ejecutar la API

```bash
cd src/OutletRentalCars.API
dotnet run
```

Se abrirá automáticamente Swagger en el navegador.

## Ejecutar tests automatizados

```bash
dotnet test --verbosity normal
```

Debe mostrar **18 tests pasando** (13 unitarios + 5 integración).

---

## Datos precargados (Seed)

### Localidades

| ID | Nombre | País |
|----|--------|------|
| 1 | Bogotá - El Dorado | Colombia |
| 2 | Medellín - José María Córdova | Colombia |
| 3 | Cali - Alfonso Bonilla Aragón | Colombia |

### Vehículos

| ID | Marca | Modelo | Tipo | Estado | Localidad |
|----|-------|--------|------|--------|-----------|
| 1 | Toyota | Corolla | sedan | Disponible | Bogotá |
| 2 | Chevrolet | Tracker | suv | Disponible | Bogotá |
| 3 | Renault | Kwid | economy | Mantenimiento | Bogotá |
| 4 | Mazda | CX-5 | suv | Medellín | Disponible |
| 5 | Kia | Sportage | sport | Medellín | Disponible |
| 6 | Nissan | Versa | sedan | Cali | Disponible |

### Reserva existente

| Vehículo | Fecha inicio | Fecha fin | Cliente |
|----------|-------------|-----------|---------|
| Toyota Corolla (ID 1) | 2026-03-01 | 2026-03-05 | Juan Pérez |

---

## Pruebas en Swagger

### GET /api/vehicles/search - Búsqueda de vehículos

---

#### Prueba 1 - Vehículos disponibles en Bogotá

```
pickupLocationId: 1
dropoffLocationId: 2
pickupDate: 2026-04-01
dropoffDate: 2026-04-05
```

**Resultado esperado:** `200 OK` con 2 vehículos (Toyota Corolla y Chevrolet Tracker).
El Renault Kwid no aparece porque está en mantenimiento.

---

#### Prueba 2 - Vehículos disponibles en Medellín

```
pickupLocationId: 2
dropoffLocationId: 1
pickupDate: 2026-04-01
dropoffDate: 2026-04-05
```

**Resultado esperado:** `200 OK` con 2 vehículos (Mazda CX-5 y Kia Sportage).

---

#### Prueba 3 - Vehículos disponibles en Cali

```
pickupLocationId: 3
dropoffLocationId: 1
pickupDate: 2026-04-01
dropoffDate: 2026-04-05
```

**Resultado esperado:** `200 OK` con 1 vehículo (Nissan Versa).

---

#### Prueba 4 - Filtrar por tipo de vehículo (SUV en Bogotá)

```
pickupLocationId: 1
dropoffLocationId: 2
pickupDate: 2026-04-01
dropoffDate: 2026-04-05
vehicleType: suv
```

**Resultado esperado:** `200 OK` con 1 vehículo (Chevrolet Tracker).
El Toyota Corolla no aparece porque es tipo sedan.

---

#### Prueba 5 - Filtrar por tipo de vehículo (sport en Medellín)

```
pickupLocationId: 2
dropoffLocationId: 1
pickupDate: 2026-04-01
dropoffDate: 2026-04-05
vehicleType: sport
```

**Resultado esperado:** `200 OK` con 1 vehículo (Kia Sportage).

---

#### Prueba 6 - Excluir vehículo con reserva activa

```
pickupLocationId: 1
dropoffLocationId: 2
pickupDate: 2026-03-02
dropoffDate: 2026-03-04
```

**Resultado esperado:** `200 OK` con 1 vehículo (solo Chevrolet Tracker).
El Toyota Corolla NO aparece porque tiene una reserva activa del 1 al 5 de marzo 2026.

---

#### Prueba 7 - Error: fecha de recogida mayor que devolución

```
pickupLocationId: 1
dropoffLocationId: 2
pickupDate: 2026-04-10
dropoffDate: 2026-04-05
```

**Resultado esperado:** `400 Bad Request`
```json
{ "error": "La fecha de recogida debe ser anterior a la fecha de devolución." }
```

---

#### Prueba 8 - Error: localidad inexistente

```
pickupLocationId: 999
dropoffLocationId: 1
pickupDate: 2026-04-01
dropoffDate: 2026-04-05
```

**Resultado esperado:** `400 Bad Request`
```json
{ "error": "La localidad de recogida no existe." }
```

---

#### Prueba 9 - Error: formato de fecha inválido

```
pickupLocationId: 1
dropoffLocationId: 2
pickupDate: fechamala
dropoffDate: 2026-04-05
```

**Resultado esperado:** `400 Bad Request`
```json
{ "error": "Formato de fecha de recogida inválido. Use yyyy-MM-dd." }
```

---

### POST /api/reservations - Crear reserva

---

#### Prueba 10 - Reserva exitosa

```json
{
  "vehicleId": 2,
  "pickupLocationId": 1,
  "dropoffLocationId": 2,
  "pickupDate": "2026-05-01T10:00:00Z",
  "dropoffDate": "2026-05-05T10:00:00Z",
  "customerName": "Carlos López"
}
```

**Resultado esperado:** `201 Created`
```json
{ "id": 2 }
```

---

#### Prueba 11 - Error: conflicto de fechas (reserva duplicada)

```json
{
  "vehicleId": 1,
  "pickupLocationId": 1,
  "dropoffLocationId": 1,
  "pickupDate": "2026-03-02T10:00:00Z",
  "dropoffDate": "2026-03-04T10:00:00Z",
  "customerName": "Ana Rodríguez"
}
```

**Resultado esperado:** `409 Conflict`
```json
{ "error": "El vehículo ya tiene una reserva activa en ese rango de fechas." }
```

---

#### Prueba 12 - Error: fecha de recogida mayor que devolución

```json
{
  "vehicleId": 2,
  "pickupLocationId": 1,
  "dropoffLocationId": 2,
  "pickupDate": "2026-05-10T10:00:00Z",
  "dropoffDate": "2026-05-05T10:00:00Z",
  "customerName": "Pedro Martínez"
}
```

**Resultado esperado:** `400 Bad Request`
```json
{ "error": "La fecha de recogida debe ser anterior a la fecha de devolución." }
```

---

#### Prueba 13 - Verificar que la reserva creada excluye el vehículo

> Ejecutar esta prueba después de haber creado la reserva de la prueba 10.

```
pickupLocationId: 1
dropoffLocationId: 2
pickupDate: 2026-05-02
dropoffDate: 2026-05-04
```

**Resultado esperado:** `200 OK` y el Chevrolet Tracker (id=2) **ya no aparece** porque ahora tiene una reserva activa en ese rango. Solo debería aparecer el Toyota Corolla.

---

## Tipos de vehículo disponibles

| Tipo | Descripción |
|------|-------------|
| sedan | Vehículo de 4 puertas, ideal para ciudad |
| suv | Vehículo utilitario deportivo |
| economy | Vehículo compacto de bajo consumo |
| sport | Vehículo de alto rendimiento |
