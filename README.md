# OutletRentalCars - Sistema de Búsqueda de Vehículos

API REST para búsqueda y reserva de vehículos de alquiler, desarrollada como prueba técnica para Browser Travel Solutions.

## Descripción

El sistema permite buscar vehículos disponibles según localidad de recogida/devolución y rango de fechas, aplicando reglas de negocio como disponibilidad por mercado, validación de reservas cruzadas y estado del vehículo.

También expone un endpoint para crear reservas, generando un evento de dominio (`VehicleReservedEvent`) manejado de forma interna (in-memory) mediante MediatR.

## Decisiones técnicas

- **Clean Architecture**: La solución se divide en 4 capas (`Domain`, `Application`, `Infrastructure`, `API`) para separar responsabilidades y facilitar el testing.
- **CQRS con MediatR**: Se usa el patrón Query/Command para separar lecturas de escrituras. `SearchVehiclesQuery` para búsqueda y `CreateReservationCommand` para reservas.
- **MySQL (EF Core + Pomelo)**: Se usa para datos transaccionales (vehículos, reservas, localidades). Se eligió Pomelo por ser el provider más maduro de MySQL para EF Core.
- **MongoDB**: Se usa para datos de configuración/catálogo (mercados y tipos de vehículo). Acceso mediante el driver oficial de MongoDB para C#.
- **Eventos de dominio**: Al crear una reserva se genera un `VehicleReservedEvent` que se publica in-memory vía MediatR Notifications. El handler actual hace logging, pero la arquitectura permite agregar más suscriptores sin modificar el comando.
- **Data Seed**: Se precargan localidades, vehículos, reservas de ejemplo, mercados y tipos de vehículo para poder probar de inmediato.

## Estructura del proyecto

```
src/
├── OutletRentalCars.API            → Web API, controllers, Program.cs
├── OutletRentalCars.Application    → Queries, Commands, Interfaces, DTOs
├── OutletRentalCars.Domain         → Entidades, Enums, Eventos de dominio
└── OutletRentalCars.Infrastructure → Repositorios, DbContext (MySQL/MongoDB), Seed
tests/
├── OutletRentalCars.Tests.Unit         → Pruebas unitarias (xUnit + Moq)
└── OutletRentalCars.Tests.Integration  → Pruebas de integración (WebApplicationFactory)
```

## Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) o superior
- MySQL Server corriendo en `localhost:3306`
- MongoDB corriendo en `localhost:27017`

## Cómo ejecutar

1. **Clonar el repositorio**
   ```bash
   git clone <url-del-repo>
   cd OutletRentalCars
   ```

2. **Configurar conexiones** (opcional, los valores por defecto apuntan a localhost)

   Editar `src/OutletRentalCars.API/appsettings.json` si es necesario:
   ```json
   {
     "ConnectionStrings": {
       "MySQL": "Server=localhost;Database=outletrentalcars;User=root;Password=root;"
     },
     "MongoDB": {
       "ConnectionString": "mongodb://localhost:27017",
       "DatabaseName": "OutletRentalCarsDb"
     }
   }
   ```

3. **Ejecutar la aplicación**
   ```bash
   dotnet run --project src/OutletRentalCars.API
   ```
   La API arranca y crea las tablas/colecciones automáticamente con datos de prueba.

4. **Probar el endpoint de búsqueda**
   ```
   GET /api/vehicles/search?pickupLocationId=a1b2c3d4-0001-0001-0001-000000000001&dropoffLocationId=a1b2c3d4-0001-0001-0001-000000000002&pickupDate=2026-04-01T10:00:00Z&dropoffDate=2026-04-05T10:00:00Z
   ```

5. **Crear una reserva**
   ```
   POST /api/reservations
   Content-Type: application/json

   {
     "vehicleId": "b1b2c3d4-0002-0002-0002-000000000002",
     "pickupLocationId": "a1b2c3d4-0001-0001-0001-000000000001",
     "dropoffLocationId": "a1b2c3d4-0001-0001-0001-000000000002",
     "pickupDate": "2026-04-10T10:00:00Z",
     "dropoffDate": "2026-04-15T10:00:00Z",
     "customerName": "Carlos Rodríguez"
   }
   ```

## Ejecutar pruebas

```bash
# Todas las pruebas
dotnet test

# Solo unitarias
dotnet test tests/OutletRentalCars.Tests.Unit

# Solo integración
dotnet test tests/OutletRentalCars.Tests.Integration
```

## Datos de prueba (seed)

| Localidad | País |
|---|---|
| Bogotá - El Dorado | Colombia |
| Medellín - José María Córdova | Colombia |
| Miami International Airport | USA |

| Vehículo | Localidad | Estado |
|---|---|---|
| Toyota Corolla 2023 | Bogotá | Disponible (con reserva del 1-5 Mar 2026) |
| Chevrolet Tracker 2024 | Bogotá | Disponible |
| Renault Kwid 2023 | Bogotá | En mantenimiento |
| Mazda CX-5 2024 | Medellín | Disponible |
| Ford Mustang 2024 | Miami | Disponible |
