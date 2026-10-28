# GestionFacturas

Sistema web para la gestión de clientes y facturas, desarrollado como solución para una prueba técnica.

La aplicación permite administrar clientes, registrar y consultar facturas, filtrar facturas por cliente, gestionar los estados de las facturas y ejecutar automáticamente el proceso de recordatorios de pago.

Cuando una factura cumple las condiciones para ser procesada, el sistema genera un documento PDF de la factura, envía una notificación por correo electrónico al cliente con el PDF adjunto y posteriormente actualiza el estado de la factura.

---

# 1. Descripción del proyecto

GestionFacturas es una aplicación web compuesta por un backend desarrollado en .NET y un frontend desarrollado en Angular.

El sistema utiliza MongoDB Atlas como base de datos y está organizado siguiendo una separación de responsabilidades basada en principios de Clean Architecture, SOLID y Dependency Injection.

La solución permite:

- Registrar clientes.
- Consultar clientes.
- Editar clientes.
- Eliminar clientes.
- Registrar facturas.
- Consultar facturas.
- Consultar una factura específica.
- Editar facturas.
- Eliminar facturas.
- Filtrar facturas por cliente.
- Consultar el estado de las facturas.
- Procesar individualmente una factura.
- Procesar todas las facturas disponibles.
- Enviar correos electrónicos de recordatorio.
- Generar archivos PDF de las facturas.
- Adjuntar el PDF de la factura al correo electrónico.
- Actualizar automáticamente el estado de las facturas.
- Ejecutar pruebas unitarias sobre la lógica principal.

---

# 2. Objetivo

El objetivo principal del proyecto es implementar una solución que permita automatizar la gestión y el seguimiento de facturas de diferentes clientes.

El sistema debe identificar las facturas que pueden ser procesadas y ejecutar el flujo correspondiente de recordatorios:

```text
Pendiente
    ↓
PrimerRecordatorio
    ↓
SegundoRecordatorio
    ↓
Desactivado
```

El cambio de estado se realiza después de enviar correctamente la notificación correspondiente.

---

# 3. Tecnologías utilizadas

## Backend

- C#
- .NET 10
- ASP.NET Core Web API
- Swagger / OpenAPI
- Dependency Injection
- Clean Architecture
- SOLID

## Base de datos

- MongoDB
- MongoDB Atlas
- MongoDB.Driver

## Frontend

- Angular
- TypeScript
- HTML
- CSS

## Pruebas

- xUnit
- Moq

## Correo electrónico

- MailKit
- SMTP

## Generación de documentos

- Generación de PDF de facturas

## Control de versiones

- Git
- GitHub

---

# 4. Arquitectura

El proyecto está organizado separando las responsabilidades principales de la aplicación.

La estructura general es:

```text
GestionFacturas
│
├── GestionFacturas.Domain
│
├── GestionFacturas.Application
│
├── GestionFacturas.Infrastructure
│
├── GestionFacturas.Api
│
├── GestionFacturas.Tests
│
└── frontend-angular
```

El flujo general de la aplicación es:

```text
                    ┌─────────────────────┐
                    │   Angular Frontend  │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │ ASP.NET Core API    │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │ Application Layer   │
                    └──────────┬──────────┘
                               │
                    ┌──────────┴──────────┐
                    ▼                     ▼
          ┌──────────────────┐   ┌──────────────────┐
          │ Infrastructure   │   │ Domain           │
          │                  │   │                  │
          │ MongoDB          │   │ Entidades        │
          │ Email            │   │ Enums            │
          │ PDF              │   │ Reglas           │
          └────────┬─────────┘   └──────────────────┘
                   │
                   ▼
          ┌──────────────────┐
          │ MongoDB Atlas    │
          └──────────────────┘
```

---

# 5. Capas del proyecto

## 5.1 GestionFacturas.Domain

Contiene las entidades y enumeraciones principales del sistema.

Entidades principales:

```text
Cliente
Factura
```

Enum utilizado para el estado de las facturas:

```text
EstadoFactura
```

Los estados disponibles son:

```text
Pendiente
PrimerRecordatorio
SegundoRecordatorio
Desactivado
```

La capa Domain no depende de infraestructura ni de la API.

---

# 5.2 GestionFacturas.Application

Contiene las interfaces y servicios que representan la lógica de aplicación.

Entre sus responsabilidades se encuentran:

- Gestión de clientes.
- Gestión de facturas.
- Procesamiento de recordatorios.
- Definición de interfaces de repositorios.
- Definición del servicio de correo.
- Definición del servicio de generación de PDF.

Interfaces principales:

```text
IClienteRepository
IFacturaRepository
IEmailService
IFacturaPdfService
```

La capa Application define qué necesita la aplicación sin depender directamente de cómo se implementa cada infraestructura.

---

# 5.3 GestionFacturas.Infrastructure

Contiene las implementaciones concretas de infraestructura.

Incluye:

- Conexión con MongoDB.
- Repositorios de clientes.
- Repositorios de facturas.
- Servicio de correo electrónico.
- Servicio de generación de PDF.

Esta capa implementa las interfaces definidas en Application.

---

# 5.4 GestionFacturas.Api

Contiene la API REST desarrollada con ASP.NET Core.

Aquí se encuentran:

- Controllers.
- Configuración de Dependency Injection.
- Configuración de MongoDB.
- Configuración del correo.
- Swagger.
- Configuración de la aplicación.

---

# 5.5 GestionFacturas.Tests

Contiene las pruebas unitarias de la aplicación.

Las pruebas utilizan:

- xUnit.
- Moq.

Se utilizan mocks para aislar la lógica de negocio de MongoDB, correo electrónico y generación de PDF.

---

# 5.6 frontend-angular

Contiene la aplicación web desarrollada en Angular.

El frontend permite interactuar con la API para:

- Administrar clientes.
- Administrar facturas.
- Crear facturas.
- Editar facturas.
- Eliminar facturas.
- Filtrar facturas.
- Visualizar estados.
- Procesar facturas individualmente.
- Procesar todas las facturas disponibles.

---

# 6. Modelo de datos

La aplicación utiliza MongoDB Atlas como sistema de persistencia.

La base de datos utilizada es:

```text
GestionFacturasDb
```

Las colecciones principales son:

```text
clientes
facturas
```

---

# 7. Configuración de MongoDB Atlas

## 7.1 Crear una cuenta

Para ejecutar el proyecto se requiere una cuenta en MongoDB Atlas.

Se debe crear un proyecto para la aplicación.

Nombre recomendado:

```text
GestionFacturas
```

---

# 7.2 Crear el cluster

Dentro del proyecto se crea un cluster de MongoDB.

Nombre utilizado en el proyecto:

```text
GestionFacturasCluster
```

El cluster es el servidor donde se almacenarán los datos de la aplicación.

---

# 7.3 Crear usuario de base de datos

Dentro de MongoDB Atlas se debe crear un usuario para que la aplicación pueda conectarse a la base de datos.

Ejemplo:

```text
Usuario: usuarioGestionFacturas
Contraseña: CONTRASEÑA_SEGURA
```

La contraseña real no  debe almacenarse en GitHub.

---

# 7.4 Configurar Network Access

En MongoDB Atlas se debe configurar el acceso de red.

Se debe agregar la dirección IP desde la cual se ejecutará la aplicación.

Durante desarrollo local se puede configurar la IP correspondiente al entorno de desarrollo.

Recomendable restringir el acceso únicamente a las IP necesarias.

---

# 7.5 Obtener la cadena de conexión

Desde MongoDB Atlas se selecciona:

```text
Database
    ↓
Connect
    ↓
Drivers
```

Se obtiene una cadena similar a:

```text
mongodb+srv://USUARIO:CONTRASEÑA@cluster.mongodb.net/
```

La cadena real debe configurarse localmente.

---

# 7.6 Crear la base de datos

La base de datos utilizada por la aplicación es:

```text
GestionFacturasDb
```

Las colecciones utilizadas son:

```text
clientes
facturas
```

MongoDB crea las colecciones cuando se insertan documentos, si no existen previamente.

---

# 7.7 Colección clientes

La colección:

```text
clientes
```

almacena la información de los clientes.

Estructura utilizada:

```json
{
  "_id": "CLI001",
  "nombre": "Juan Perez",
  "email": "correo-prueba-1@ejemplo.com",
  "activo": true
}
```

Campos:

| Campo | Tipo | Descripción |
|---|---|---|
| `_id` | string | Identificador único del cliente |
| `nombre` | string | Nombre del cliente |
| `email` | string | Correo electrónico |
| `activo` | boolean | Indica si el cliente está activo |

---

# 7.8 Clientes de prueba

Para validar el funcionamiento con diferentes clientes se pueden registrar tres clientes:

```json
{
  "_id": "CLI001",
  "nombre": "Juan Perez",
  "email": "correo-prueba-1@ejemplo.com",
  "activo": true
}
```

```json
{
  "_id": "CLI002",
  "nombre": "Maria Gomez",
  "email": "correo-prueba-2@ejemplo.com",
  "activo": true
}
```

```json
{
  "_id": "CLI003",
  "nombre": "Carlos Rodriguez",
  "email": "correo-prueba-3@ejemplo.com",
  "activo": true
}
```

Estos datos son ejemplos para pruebas. Los correos pueden ser reemplazados por direcciones de prueba autorizadas.

---

# 7.9 Colección facturas

La colección:

```text
facturas
```

almacena las facturas asociadas a los clientes.

Estructura:

```json
{
  "_id": "FAC001",
  "clienteId": "CLI001",
  "numeroFactura": "FAC-001",
  "descripcion": "Servicio correspondiente al periodo",
  "valor": 150000,
  "fechaVencimiento": "2026-10-01T00:00:00Z",
  "estado": "primerrecordatorio"
}
```

Campos:

| Campo | Tipo | Descripción |
|---|---|---|
| `_id` | string | Identificador único de la factura |
| `clienteId` | string | Identificador del cliente |
| `numeroFactura` | string | Número visible de la factura |
| `descripcion` | string | Descripción del concepto de la factura |
| `valor` | decimal | Valor de la factura |
| `fechaVencimiento` | DateTime | Fecha de vencimiento |
| `estado` | string | Estado actual de la factura |

---

# 7.10 Estados de las facturas

Los estados válidos del sistema son:

```text
pendiente
primerrecordatorio
segundorecordatorio
desactivado
```

El estado se almacena en MongoDB como texto.

---

# 7.11 Ejemplos de facturas para pruebas

Factura pendiente:

```json
{
  "_id": "FAC001",
  "clienteId": "CLI001",
  "numeroFactura": "FAC-001",
  "descripcion": "Servicio correspondiente al periodo",
  "valor": 150000,
  "fechaVencimiento": "2027-01-01T00:00:00Z",
  "estado": "pendiente"
}
```

Factura pendiente vencida:

```json
{
  "_id": "FAC002",
  "clienteId": "CLI001",
  "numeroFactura": "FAC-002",
  "descripcion": "Factura vencida",
  "valor": 200000,
  "fechaVencimiento": "2026-01-01T00:00:00Z",
  "estado": "pendiente"
}
```

Factura en primer recordatorio:

```json
{
  "_id": "FAC003",
  "clienteId": "CLI002",
  "numeroFactura": "FAC-003",
  "descripcion": "Servicio mensual",
  "valor": 250000,
  "fechaVencimiento": "2026-09-01T00:00:00Z",
  "estado": "primerrecordatorio"
}
```

Factura en segundo recordatorio:

```json
{
  "_id": "FAC004",
  "clienteId": "CLI003",
  "numeroFactura": "FAC-004",
  "descripcion": "Servicio mensual",
  "valor": 300000,
  "fechaVencimiento": "2026-08-01T00:00:00Z",
  "estado": "segundorecordatorio"
}
```

---

# 7.12 Configuración de MongoDB en la API

La API utiliza una sección de configuración denominada:

```text
MongoDb
```

Ejemplo:

```json
{
  "MongoDb": {
    "ConnectionString": "TU_CADENA_DE_MONGODB",
    "DatabaseName": "GestionFacturasDb",
    "CollectionName": "facturas",
    "ClientesCollectionName": "clientes"
  }
}
```

La cadena real de conexión debe mantenerse privada.

No se publicaron credenciales reales en GitHub.

---

# 8. Flujo de estados

El ciclo de vida de una factura es:

```text
                    ┌──────────────┐
                    │  Pendiente   │
                    └──────┬───────┘
                           │
                  Fecha vencida
                           │
                           ▼
              ┌────────────────────────┐
              │ PrimerRecordatorio     │
              └────────────┬───────────┘
                           │
                    Procesamiento
                           │
                           ▼
              ┌────────────────────────┐
              │ SegundoRecordatorio    │
              └────────────┬───────────┘
                           │
                    Procesamiento
                           │
                           ▼
                 ┌────────────────┐
                 │  Desactivado   │
                 └────────────────┘
```

---

# 9. Reglas de procesamiento

## 9.1 Factura pendiente con fecha futura

Si:

```text
Estado = Pendiente
```

y la fecha de vencimiento todavía no ha pasado, la factura no se procesa.

Ejemplo:

```text
Estado: Pendiente
Vencimiento: Fecha futura
Resultado: No procesa
```

---

# 9.2 Factura pendiente vencida

Si:

```text
Estado = Pendiente
```

y la fecha de vencimiento ya pasó:

1. Se obtiene el cliente.
2. Se genera el PDF.
3. Se envía el primer recordatorio.
4. El PDF se adjunta al correo.
5. Si el envío es correcto, la factura cambia a:

```text
PrimerRecordatorio
```

---

# 9.3 Primer recordatorio

Si:

```text
Estado = PrimerRecordatorio
```

el sistema:

1. Obtiene el cliente.
2. Genera el PDF.
3. Envía el segundo recordatorio.
4. Adjunta el PDF.
5. Actualiza la factura a:

```text
SegundoRecordatorio
```

---

# 9.4 Segundo recordatorio

Si:

```text
Estado = SegundoRecordatorio
```

el sistema:

1. Obtiene el cliente.
2. Genera el PDF.
3. Envía el aviso de desactivación.
4. Adjunta el PDF.
5. Actualiza la factura a:

```text
Desactivado
```

---

# 9.5 Factura desactivada

Las facturas en estado:

```text
Desactivado
```

no se procesan nuevamente.

---

# 10. Procesamiento de recordatorios

El backend dispone de un proceso para ejecutar el procesamiento de las facturas disponibles.

El sistema identifica las facturas:

```text
Pendiente + vencida
PrimerRecordatorio
SegundoRecordatorio
```

No se procesan:

```text
Pendiente + fecha futura
Desactivado
```

---

# 11. Generación de PDF

Antes de enviar el correo, el sistema genera un documento PDF correspondiente a la factura.

El PDF contiene la información principal de:

- Cliente.
- Número de factura.
- Descripción.
- Valor.
- Fecha de vencimiento.
- Estado.

El archivo se adjunta al correo electrónico.

Nombre del archivo:

```text
Factura-{NumeroFactura}.pdf
```

Ejemplo:

```text
Factura-FAC-002.pdf
```

---

# 12. Envío de correos

El sistema utiliza un servicio de correo abstraído mediante una interfaz:

```text
IEmailService
```

La implementación utiliza SMTP y MailKit.

La interfaz recibe:

```text
destinatario
asunto
mensaje
adjunto
nombreArchivo
```

El PDF se adjunta como:

```text
application/pdf
```

---

# 13. Correos enviados

## Primer recordatorio

Asunto:

```text
Primer recordatorio de pago
```

El correo informa al cliente que la factura se encuentra vencida y corresponde al primer recordatorio.

---

## Segundo recordatorio

Asunto:

```text
Segundo recordatorio de pago
```

El correo informa que la factura ha pasado al segundo recordatorio.

---

## Aviso de desactivación

Asunto:

```text
Aviso de desactivación
```

El correo informa que la factura se encuentra en segundo recordatorio y que se realizará la desactivación correspondiente.

---

# 14. API

La API se ejecuta durante el desarrollo en:

```text
http://localhost:5167
```

Swagger:

```text
http://localhost:5167/swagger
```

---

# 15. Endpoints de clientes

## Obtener todos los clientes

```http
GET /api/Clientes
```

## Obtener cliente por ID

```http
GET /api/Clientes/{id}
```

## Crear cliente

```http
POST /api/Clientes
```

## Actualizar cliente

```http
PUT /api/Clientes/{id}
```

## Eliminar cliente

```http
DELETE /api/Clientes/{id}
```

---

# 16. Endpoints de facturas

## Obtener todas las facturas

```http
GET /api/Facturas
```

## Obtener factura por ID

```http
GET /api/Facturas/{id}
```

## Obtener facturas por cliente

```http
GET /api/Facturas/cliente/{clienteId}
```

## Crear factura

```http
POST /api/Facturas
```

Al crear una nueva factura, su estado inicial es:

```text
Pendiente
```

---

## Actualizar factura

```http
PUT /api/Facturas/{id}
```

---

## Eliminar factura

```http
DELETE /api/Facturas/{id}
```

---

## Procesar todos los recordatorios

```http
POST /api/Facturas/procesar-recordatorios
```

Este endpoint procesa las facturas que cumplen las condiciones establecidas.

---

# 17. Frontend Angular

El frontend se encuentra en:

```text
frontend-angular
```

Durante el desarrollo se ejecuta normalmente en:

```text
http://localhost:4200
```

---

# 18. Funcionalidades del frontend

El frontend incluye:

## Inicio

Panel principal de navegación y resumen del sistema.

## Clientes

Permite:

- Listar clientes.
- Crear clientes.
- Editar clientes.
- Eliminar clientes.
- Consultar información.

## Facturas

Permite:

- Listar facturas.
- Crear facturas.
- Editar facturas.
- Eliminar facturas.
- Filtrar por cliente.
- Visualizar estados.
- Procesar facturas individualmente.
- Procesar todas las facturas disponibles.

---

# 19. Estados visuales

El frontend representa los estados:

```text
Pendiente
Primer recordatorio
Segundo recordatorio
Desactivado
```
---

# 20. Procesamiento individual

El usuario puede seleccionar una factura y ejecutar el procesamiento individual.

El sistema valida que la factura sea procesable.

Una factura pendiente solamente puede procesarse si su fecha de vencimiento ya pasó.

Las facturas en:

```text
PrimerRecordatorio
SegundoRecordatorio
```

pueden continuar con el siguiente paso del flujo.

---

# 21. Procesamiento masivo

El sistema también permite ejecutar el proceso de recordatorios para todas las facturas disponibles.

El backend identifica las facturas que cumplen las condiciones y procesa cada una.

---

# 22. Clean Architecture

El proyecto utiliza separación de responsabilidades para evitar acoplamiento entre las diferentes partes del sistema.

La lógica de negocio se mantiene separada de:

- Base de datos.
- Correo.
- PDF.
- API.
- Frontend.

Esto facilita el mantenimiento y las pruebas.

---

# 23. Principios SOLID

## Single Responsibility Principle

Cada clase tiene una responsabilidad específica.

Ejemplos:

```text
FacturaService
```

maneja la lógica relacionada con facturas.

```text
IEmailService
```

abstrae el envío de correos.

```text
IFacturaPdfService
```

abstrae la generación del PDF.

---

## Open/Closed Principle

Las funcionalidades pueden extenderse mediante nuevas implementaciones sin modificar innecesariamente la lógica existente.

---

## Liskov Substitution Principle

Las implementaciones concretas de las interfaces pueden sustituirse sin alterar el comportamiento esperado de los servicios.

---

## Interface Segregation Principle

Las responsabilidades se separan mediante interfaces específicas.

---

## Dependency Inversion Principle

Los servicios dependen de abstracciones y no directamente de implementaciones concretas.

Por ejemplo:

```text
FacturaService
       ↓
IFacturaRepository
```

en lugar de depender directamente de una implementación específica de MongoDB.

---

# 24. Dependency Injection

La aplicación utiliza Dependency Injection para registrar y proporcionar las dependencias necesarias.

Entre las dependencias utilizadas se encuentran:

```text
IFacturaRepository
IClienteRepository
IEmailService
IFacturaPdfService
```

Esto permite:

- Reducir acoplamiento.
- Facilitar pruebas unitarias.
- Cambiar implementaciones.
- Mejorar mantenibilidad.

---

# 25. Pruebas unitarias

El proyecto incluye pruebas unitarias utilizando:

```text
xUnit
Moq
```

Los repositorios y servicios externos se simulan mediante mocks.

Se validan escenarios relacionados con:

- Procesamiento de facturas.
- Estados.
- Envío de correos.
- Generación de PDF.
- Actualización del estado.

Para ejecutar las pruebas:

```powershell
dotnet test
```

El resultado esperado corresponde a todas las pruebas implementadas correctamente.

---

# 26. Configuración del correo

La configuración del correo se encuentra en:

```text
appsettings.json
```

Ejemplo:

```json
{
  "Email": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "TU_CORREO",
    "Password": "TU_PASSWORD",
    "From": "TU_CORREO"
  }
}
```

Las credenciales reales deben configurarse únicamente en el entorno local o mediante un mecanismo seguro de configuración.

No se deben publicar contraseñas reales en GitHub.

---

# 27. Seguridad de configuración

El repositorio no debe contener:

- Contraseñas reales.
- Cadenas privadas de MongoDB.
- Tokens.
- API keys.
- Credenciales SMTP.

Los valores sensibles deben reemplazarse por variables de entorno, Secret Manager u otro mecanismo seguro en ambientes de producción.

El archivo:

```text
appsettings.Development.json
```

se encuentra incluido en `.gitignore` para evitar publicar configuraciones locales.

---

# 28. Archivo .gitignore

El proyecto utiliza `.gitignore` para evitar subir archivos generados o información local.

Entre los archivos y carpetas ignorados se encuentran:

```text
.vs/
bin/
obj/
node_modules/
dist/
.angular/
.cache/
.env
*.user
*.suo
*.log
```

Esto permite mantener el repositorio limpio.

---

# 29. Instalación del proyecto

## Requisitos

Se requiere instalar:

- .NET SDK 10.
- Node.js.
- Angular CLI.
- Git.
- Una cuenta de MongoDB Atlas.
- Un servidor SMTP o cuenta de correo configurada.

---

# 30. Clonar el repositorio

```powershell
git clone https://github.com/HolmanMedina/GestionFacturas.git
```

Ingresar al proyecto:

```powershell
cd GestionFacturas
```

---

# 31. Configurar MongoDB

Modificar:

```text
GestionFacturas.Api/appsettings.json
```

Configurar:

```json
"MongoDb": {
  "ConnectionString": "TU_CADENA_DE_MONGODB",
  "DatabaseName": "GestionFacturasDb",
  "CollectionName": "facturas",
  "ClientesCollectionName": "clientes"
}
```

---

# 32. Configurar correo

Configurar:

```json
"Email": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "Username": "TU_CORREO",
  "Password": "TU_PASSWORD",
  "From": "TU_CORREO"
}
```

Utilizar credenciales válidas y seguras.

---

# 33. Ejecutar backend

Desde la raíz:

```powershell
dotnet restore
```

Después:

```powershell
dotnet build
```

Ejecutar la API:

```powershell
dotnet run --project GestionFacturas.Api
```

La API estará disponible en:

```text
http://localhost:5167
```

Swagger:

```text
http://localhost:5167/swagger
```

---

# 34. Ejecutar pruebas

Desde la raíz del proyecto:

```powershell
dotnet test
```

El comando ejecutará las pruebas del proyecto:

```text
GestionFacturas.Tests
```

---

# 35. Ejecutar frontend

Entrar a:

```powershell
cd frontend-angular
```

Instalar dependencias:

```powershell
npm install
```

Ejecutar:

```powershell
ng serve
```

Abrir:

```text
http://localhost:4200
```

---

# 36. Comunicación Frontend - Backend

El frontend consume la API ASP.NET Core.

Durante el desarrollo la API utiliza:

```text
http://localhost:5167
```

El frontend utiliza:

```text
http://localhost:4200
```

La comunicación permite consultar y modificar clientes y facturas.

---

# 37. Flujo completo de una factura

Ejemplo:

```text
1. Se crea una factura.
       ↓
2. Estado inicial:
   Pendiente
       ↓
3. La fecha de vencimiento pasa.
       ↓
4. Se procesa la factura.
       ↓
5. Se genera PDF.
       ↓
6. Se envía primer recordatorio.
       ↓
7. Estado:
   PrimerRecordatorio
       ↓
8. Se procesa nuevamente.
       ↓
9. Se genera PDF.
       ↓
10. Se envía segundo recordatorio.
       ↓
11. Estado:
    SegundoRecordatorio
       ↓
12. Se procesa nuevamente.
       ↓
13. Se genera PDF.
       ↓
14. Se envía aviso de desactivación.
       ↓
15. Estado:
    Desactivado
```

---

# 38. Manejo de una factura pendiente no vencida

Si una factura tiene:

```text
Estado: Pendiente
Fecha de vencimiento: Fecha futura
```

el sistema no envía ningún correo ni cambia el estado.

Esto evita procesar anticipadamente una factura que todavía no ha vencido.

---

# 39. Manejo de errores

La aplicación valida situaciones como:

- Factura inexistente.
- Cliente inexistente.
- Factura no disponible para procesamiento.
- Factura pendiente todavía no vencida.
- Datos inválidos.
- Errores relacionados con servicios externos.

---

# 40. Base de datos y relaciones

MongoDB utiliza documentos independientes y la relación entre una factura y un cliente se realiza mediante:

```text
factura.clienteId
```

Ejemplo:

```text
Factura
clienteId = CLI001
```

corresponde al cliente:

```text
Cliente
_id = CLI001
```

Este enfoque permite consultar las facturas de un cliente utilizando su identificador.

---

# 41. Datos de prueba

Para realizar pruebas funcionales se pueden utilizar:

### Cliente 1

```text
ID: CLI001
Nombre: Juan Perez
```

### Cliente 2

```text
ID: CLI002
Nombre: Maria Gomez
```

### Cliente 3

```text
ID: CLI003
Nombre: Carlos Rodriguez
```

Se pueden asociar diferentes facturas a cada cliente para comprobar el filtrado y el procesamiento.

---

# 42. Consideraciones para producción

Para un ambiente productivo se recomienda:

- Utilizar variables de entorno.
- Configurar HTTPS.
- Restringir el acceso de MongoDB Atlas.
- Utilizar credenciales con permisos mínimos.
- Configurar correctamente CORS.
- Utilizar un servicio SMTP confiable.
- Configurar logs.
- Utilizar secretos administrados por la plataforma de despliegue.
- Realizar respaldos de la base de datos.

---

# 43. Estructura del repositorio

```text
GestionFacturas/
│
├── GestionFacturas.Api/
│   ├── Controllers/
│   ├── Program.cs
│   └── appsettings.json
│
├── GestionFacturas.Application/
│   ├── Interfaces/
│   └── Services/
│
├── GestionFacturas.Domain/
│   ├── Entities/
│   └── Enums/
│
├── GestionFacturas.Infrastructure/
│   ├── MongoDb/
│   └── Email/
│
├── GestionFacturas.Tests/
│
├── frontend-angular/
│
├── .gitignore
├── GestionFacturas.sln
└── README.md
```

---

# 44. Repositorio

Repositorio del proyecto:

https://github.com/HolmanMedina/GestionFacturas

---

# 45. Autor

**Holman Duvan Medina Montero**

Proyecto desarrollado como prueba técnica de desarrollo de software.

---