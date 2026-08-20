# Modelo de dominio

## Entidades base

**BaseEntity**

- `Id` (`int`)
- `CreatedAt` / `UpdatedAt`
- `IsDeleted` (borrado lógico)

**SeoEntity** (hereda BaseEntity)

Usada por Product, Category, BlogPost y Page.

- `MetaTitle`, `MetaDescription`
- `CanonicalUrl`
- `OgTitle`, `OgDescription`, `OgImage`
- `StructuredData`
- `Index`, `Follow`

## Catálogo

### Category

Árbol de categorías (`ParentCategoryId` nulo = raíz). `Published` y `DisplayOrder` controlan visibilidad y orden. El slug se genera a partir del nombre.

### Brand

Marca opcional del producto. Tiene `Name`, `Slug`, `Description` y `LogoUrl`.

### Product

Pieza central de la plantilla.

| Campo | Rol en vitrina / tienda |
|-------|-------------------------|
| `Name`, `Slug`, `SKU` | Identidad y URL amigable |
| `Price` | Precio interno siempre existe |
| `ShowPrice` | Si es `false`, la vitrina no muestra precio |
| `WhatsAppMessage` | Mensaje de consulta (modo listado) |
| `Published` | Visible en el sitio público |
| `Featured` | Destacado en home |
| `ThumbnailUrl` | Imagen principal |
| `Stock` | Opcional. `null` = no se controla; si hay número, el checkout lo descuenta |
| `CategoryId` | Obligatoria |
| `BrandId` | Opcional |

Reglas: nombre obligatorio, categoría > 0, precio no negativo.

### ProductImage

Galería del producto. Una imagen puede marcarse como principal.

## Comercio

### Cart / CartItem

Un carrito por usuario autenticado. No se duplica el mismo producto: se suma cantidad. Precio unitario se copia al agregar (snapshot).

### Order / OrderItem

Pedido con `OrderNumber`, totales, notas, snapshot de envío (`ShippingStreet/City/Region/PostalCode`) y estado:

`Pending → Confirmed → Processing → Shipped → Completed` (o `Cancelled`)

Checkout por API: `POST /api/orders` arma el pedido desde el carrito, descuenta stock si aplica y vacía el carrito. En modo `Catalog` no se permite.

### Payment

Pago asociado a una orden: método, monto, estado, `TransactionId`. Un pago `Completed` pasa un pedido `Pending` a `Confirmed`. El método por defecto es pago coordinado (`Coordinated`).

## Contenido y sitio

Pensado para personalizar cada despliegue:

- **Banner:** hero del home (`/admin/banners`, `/api/banners`)
- **Page:** páginas estáticas (nosotros, envíos, políticas)
- **BlogPost:** contenido SEO
- **Setting:** nombre del sitio, logo, favicon, colores, contacto, redes, Analytics, Tag Manager y `SiteMode` (`Catalog` | `Store` | `Hybrid`)

Page, Blog, Banner y Setting tienen API JSON y editor admin.

## Identidad

- `ApplicationUser`: Identity + nombre/apellido
- `Address`: direcciones de envío (entidad de dominio; API en `/api/addresses`)
- `RefreshToken`: login/register emiten refresh; `/api/auth/refresh` rota el token y `/api/auth/logout` lo revoca

## Integraciones

`MercadoLibreAuth` guarda tokens OAuth de Mercado Libre. No hay servicio de sincronización aún.

## IDs

Todo el dominio usa `int` como clave. Los DTOs de Application siguen esa convención.
