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

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- MySQL Server corriendo en `localhost:3306` (usuario `root`, sin contraseña)
- MongoDB corriendo en `localhost:27017` (opcional, la API funciona sin él)

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
       "MySQL": "Server=localhost;Database=outletrentalcars;User=root;Password=;"
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

4. **Abrir Swagger**

   Swagger se abre automáticamente en el navegador. También puedes acceder en: `https://localhost:5001/swagger`

5. **Probar el endpoint de búsqueda**
   ```
   GET /api/vehicles/search?pickupLocationId=1&dropoffLocationId=2&pickupDate=2026-04-01&dropoffDate=2026-04-05
   ```

   También puedes filtrar por tipo de vehículo:
   ```
   GET /api/vehicles/search?pickupLocationId=1&dropoffLocationId=2&pickupDate=2026-04-01&dropoffDate=2026-04-05&vehicleType=suv
   ```

6. **Crear una reserva**
   ```
   POST /api/reservations
   Content-Type: application/json

   {
     "vehicleId": 2,
     "pickupLocationId": 1,
     "dropoffLocationId": 2,
     "pickupDate": "2026-05-01T10:00:00Z",
     "dropoffDate": "2026-05-05T10:00:00Z",
     "customerName": "Carlos López"
   }
   ```

## Ejecutar pruebas

```bash
# Todas las pruebas (18 tests: 13 unitarios + 5 integración)
dotnet test --verbosity normal

# Solo unitarias
dotnet test tests/OutletRentalCars.Tests.Unit

# Solo integración
dotnet test tests/OutletRentalCars.Tests.Integration
```

## Documentación adicional

- [PRUEBAS_SWAGGER.md](PRUEBAS_SWAGGER.md) - Guía completa con 13 casos de prueba para probar la API desde Swagger

## Datos de prueba (seed)

### Localidades

| ID | Nombre | País |
|----|--------|------|
| 1 | Bogotá - El Dorado | Colombia |
| 2 | Medellín - José María Córdova | Colombia |
| 3 | Cali - Alfonso Bonilla Aragón | Colombia |

### Vehículos

| ID | Vehículo | Tipo | Localidad | Estado |
|----|----------|------|-----------|--------|
| 1 | Toyota Corolla | sedan | Bogotá | Disponible (reserva 1-5 Mar 2026) |
| 2 | Chevrolet Tracker | suv | Bogotá | Disponible |
| 3 | Renault Kwid | economy | Bogotá | Mantenimiento |
| 4 | Mazda CX-5 | suv | Medellín | Disponible |
| 5 | Kia Sportage | sport | Medellín | Disponible |
| 6 | Nissan Versa | sedan | Cali | Disponible |

### Tipos de vehículo

| Tipo | Descripción |
|------|-------------|
| sedan | Vehículo de 4 puertas, ideal para ciudad |
| suv | Vehículo utilitario deportivo |
| economy | Vehículo compacto de bajo consumo |
| sport | Vehículo de alto rendimiento |
