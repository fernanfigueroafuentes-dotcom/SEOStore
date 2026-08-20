# Arquitectura

SEOStore usa **Clean Architecture** en .NET 9. El dominio no depende de EF Core ni de ASP.NET. La web solo orquesta HTTP; la lógica vive en Application y Domain.

## Proyectos

```
SEOStore.sln
└── src/
    ├── SEOStore.Domain          Entidades y reglas
    ├── SEOStore.Application     Casos de uso, DTOs, interfaces
    ├── SEOStore.Infrastructure  EF Core, PostgreSQL, Identity, Cloudinary
    ├── SEOStore.Web             API REST, MVC, Swagger, JWT
    └── SEOStore.Shared          Utilidades compartidas (aún mínimo)
```

## Dependencias

```
SEOStore.Web
    ├── SEOStore.Application
    ├── SEOStore.Infrastructure
    ├── SEOStore.Domain
    └── SEOStore.Shared

SEOStore.Infrastructure
    ├── SEOStore.Application   (implementa repositorios e IImageStorageService)
    └── SEOStore.Domain

SEOStore.Application
    └── SEOStore.Domain
```

Regla: Domain no referencia Infrastructure ni Web.

## Capas

### Domain

Entidades agrupadas por bounded context:

- **Catalog:** Product, Category, Brand, ProductImage
- **Commerce:** Cart, CartItem, Order, OrderItem, Payment
- **Users:** Address
- **Content:** BlogPost, Page, Banner
- **Configuration:** Setting (`SiteMode`)
- **Integrations:** MercadoLibreAuth

`BaseEntity` aporta `Id`, fechas y soft delete. `SeoEntity` añade campos SEO. Varias entidades de catálogo encapsulan creación y cambios (constructores privados y métodos de dominio).

### Application

- DTOs por feature (`Features/{Nombre}/DTOs`)
- Contratos `I*Service` e `I*Repository`
- Servicios: Category, Brand, Product, ProductImage, Cart, Order, Address, Setting, Page, Blog, Banner
- Registro: `AddApplication()`

### Infrastructure

- `ApplicationDbContext` (Identity + catálogo + comercio + contenido)
- Configuraciones Fluent API
- Repositorios
- ASP.NET Identity (`ApplicationUser`, RefreshToken)
- Address vive en Domain; EF la mapea a `Addresses`
- `CloudinaryImageStorageService`
- Migraciones EF Core
- Registro: `AddInfrastructure(configuration)`

### Web

- Controladores API (`/api/...`)
- MVC básico (`HomeController` y vistas por defecto)
- Swagger en Development
- JWT Bearer como esquema de autenticación
- Las migraciones se aplican al arrancar la aplicación

## Autenticación

Identity guarda usuarios y hashes. La API emite JWT y refresh token en registro y login.

- Clave: user secret `Jwt:Key` (mínimo 32 bytes)
- Issuer / Audience: `appsettings.json`
- Refresh: `Jwt:RefreshDays` (14 por defecto); rotación en `/api/auth/refresh`
- Contraseña: mínimo 8 caracteres, mayúscula, minúscula y dígito
- El carrito, las órdenes y las direcciones exigen `[Authorize]` JWT

## Persistencia

PostgreSQL con Npgsql. Connection string por defecto en desarrollo:

`Host=127.0.0.1;Port=5433;Database=SEOStoreDb;Username=postgres;Password=postgres`

El puerto `5433` sugiere PostgreSQL en contenedor o instancia local no estándar. Ajustar según el entorno.

## Imágenes

Las URLs públicas van por slug (`/producto/{slug}`, `/categoria/{slug}`). Las URLs se guardan en producto e `ProductImage`. La subida pasa por `IImageStorageService`: Cloudinary si hay credenciales; si no, `wwwroot/uploads`. Con Cloudinary configurado, un fallo de permisos no se oculta guardando en local. El layout inyecta metas, canonical, Open Graph y JSON-LD desde `SeoEntity` y `Setting`.

## Errores HTTP

Los controladores no exponen `ex.Message` ni detalles de PostgreSQL. El cliente recibe mensajes genéricos; el detalle queda en logs del servidor.
