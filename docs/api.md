# API

Base en desarrollo: `http://localhost:5251`

Explorador: `/swagger`

Autenticación: header `Authorization: Bearer {accessToken}`

Los writes de catálogo (`POST`/`PUT`/`DELETE`) piden un JWT de un usuario con rol **Admin**. Un token de `/api/auth/register` no alcanza: en Swagger hacé login con el admin sembrado (`admin@local.test` / `Admin1234` por defecto), Authorize, y después el PUT.

Los mensajes de error al cliente son genéricos (400/401/403/404/500). No se devuelven excepciones internas.

## Auth — `/api/auth`

Registro y login exitosos:

```json
{
  "accessToken": "...",
  "tokenType": "Bearer",
  "expiresAt": "2026-08-18T21:00:00Z",
  "refreshToken": "...",
  "refreshExpiresAt": "2026-09-01T20:00:00Z"
}
```

| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| POST | `/api/auth/register` | No | Crea usuario y devuelve access + refresh |
| POST | `/api/auth/login` | No | Devuelve access + refresh |
| POST | `/api/auth/refresh` | No | Cuerpo `{ "refreshToken": "..." }`. Revoca el token usado y emite un par nuevo |
| POST | `/api/auth/logout` | No | Cuerpo `{ "refreshToken": "..." }`. Revoca el refresh |

Reglas de contraseña: mínimo 8 caracteres, una minúscula, una mayúscula y un dígito. Email único. El access token dura 60 minutos; el refresh, 14 días (`Jwt:RefreshDays`).

## Categorías — `/api/Categories`

| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| GET | `/api/Categories` | No | Listado |
| GET | `/api/Categories/{id}` | No | Detalle |
| POST | `/api/Categories` | Admin JWT | Crear |
| PUT | `/api/Categories/{id}` | Admin JWT | Actualizar |
| DELETE | `/api/Categories/{id}` | Admin JWT | Borrado lógico |

`ParentCategoryId` `null` o `0` = categoría raíz. Un padre inexistente produce error de validación. El slug es único entre productos, categorías, páginas y notas; si cambia, la URL vieja responde 301.

## Marcas — `/api/Brands`

Mismos verbos que categorías: GET público, POST/PUT/DELETE con JWT Admin.

## Productos — `/api/Products`

Mismos verbos. Writes con JWT Admin. Campos relevantes para la plantilla:

- `showPrice`: si el storefront debe mostrar precio
- `whatsAppMessage`: texto de consulta
- `published`, `featured`
- `categoryId` obligatorio, `brandId` opcional
- `stock`: `null` = no se controla; un número se descuenta en checkout

## Imágenes — `/api/products/{productId}/images`

Writes (POST/PUT/DELETE/PATCH) con JWT Admin.
| GET | `/{id}` | Una imagen |
| POST | `/` | Alta por URL/metadatos |
| POST | `/upload` | Subida a Cloudinary |
| PUT | `/{id}` | Actualizar |
| DELETE | `/{id}` | Eliminar |
| PATCH | `/{id}/primary` | Marcar como principal |

## Carrito — `/api/cart`

Requiere JWT.

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/cart` | Carrito del usuario |
| POST | `/api/cart/items` | Agregar ítem |
| PUT | `/api/cart/items/{productId}` | Cambiar cantidad |
| DELETE | `/api/cart/items/{productId}` | Quitar ítem |
| DELETE | `/api/cart` | Vaciar |

Agregar:

```json
{
  "productId": 3,
  "quantity": 1
}
```

Un mismo producto no se duplica: se incrementa la cantidad.

## Órdenes — `/api/orders`

Requiere JWT. En modo `Catalog` el checkout responde 400.

| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| POST | `/api/orders` | JWT | Checkout: crea la orden desde el carrito y lo vacía |
| GET | `/api/orders` | JWT | Pedidos del usuario |
| GET | `/api/orders/{id}` | JWT | Detalle (dueño o Admin) |
| GET | `/api/orders/all` | Admin JWT | Todos los pedidos |
| PATCH | `/api/orders/{id}/status` | Admin JWT | Cambia estado; `Cancelled` restaura stock |
| POST | `/api/orders/{id}/payments` | JWT dueño o Admin | Agrega un pago pendiente |

Checkout:

```json
{
  "addressId": 1,
  "shippingCost": 0,
  "discount": 0,
  "notes": null,
  "paymentMethod": "Coordinated"
}
```

`addressId` es opcional: si existe, se copia la dirección al pedido. `paymentMethod` opcional: si viene, se crea un pago `Pending` por el total.

Cambio de estado (Admin):

```json
{
  "status": "Confirmed"
}
```

Estados: `Pending`, `Confirmed`, `Processing`, `Shipped`, `Completed`, `Cancelled`. Un pedido `Completed` o `Cancelled` no cambia de estado.

## Pagos — `/api/payments`

| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| PATCH | `/api/payments/{id}/status` | Admin JWT | Actualiza el pago. `Completed` confirma un pedido `Pending` |

```json
{
  "status": "Completed",
  "transactionId": "optional"
}
```

## Direcciones — `/api/addresses`

Requiere JWT. Solo las del usuario autenticado.

| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| GET | `/api/addresses` | JWT | Listado |
| POST | `/api/addresses` | JWT | Crear |
| PUT | `/api/addresses/{id}` | JWT | Actualizar |
| DELETE | `/api/addresses/{id}` | JWT | Borrado lógico |

```json
{
  "street": "Av. Siempre Viva 742",
  "city": "Springfield",
  "region": "Buenos Aires",
  "postalCode": "1000",
  "isDefault": true
}
```

## Páginas — `/api/pages`

| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| GET | `/api/pages` | No | Listado |
| GET | `/api/pages/{id}` | No | Detalle |
| POST | `/api/pages` | Admin JWT | Crear |
| PUT | `/api/pages/{id}` | Admin JWT | Actualizar |
| DELETE | `/api/pages/{id}` | Admin JWT | Borrado lógico |

## Blog — `/api/blog`

Mismos verbos que páginas. Writes con JWT Admin.

## Banners — `/api/banners`

| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| GET | `/api/banners` | No | Activos, para el home |
| GET | `/api/banners/all` | Admin JWT | Todos |
| GET | `/api/banners/{id}` | No | Detalle |
| POST | `/api/banners` | Admin JWT | Crear |
| PUT | `/api/banners/{id}` | Admin JWT | Actualizar |
| DELETE | `/api/banners/{id}` | Admin JWT | Borrado lógico |

## Sitio — `/api/settings`

| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| GET | `/api/settings` | No | Marca, colores, contacto, `siteMode`, `checkoutEnabled` |
| PUT | `/api/settings` | Admin JWT | Actualizar. `siteMode`: `Catalog` \| `Store` \| `Hybrid` |

En `Catalog` el checkout está deshabilitado. `Store` e `Hybrid` lo permiten. El editor MVC de `/admin/sitio` no pisa `siteMode` si no lo envía.

## Pendiente de API

No hay endpoints REST todavía para:

- Integración Mercado Libre

`robots.txt` y `sitemap.xml` son rutas MVC (`/robots.txt`, `/sitemap.xml`), no API. El admin del storefront usa cookie en `/cuenta/ingresar`, no JWT.

Los writes de catálogo por API (`POST`/`PUT`/`DELETE`/`PATCH`) exigen JWT de un usuario con rol **Admin**. Los GET siguen públicos. Carrito, órdenes y direcciones piden JWT de cualquier usuario autenticado.
