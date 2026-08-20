# Estado del proyecto

Leyenda: 🟢 listo · 🟡 parcial · 🔴 no iniciado o no usable

## Por capa (catálogo)

| Módulo | Domain | Application | Infrastructure | Web |
|--------|--------|-------------|----------------|-----|
| Brand | 🟢 | 🟢 | 🟢 | 🟢 |
| Category | 🟢 | 🟢 | 🟢 | 🟢 |
| Product | 🟢 | 🟢 | 🟢 | 🟢 |
| ProductImage | 🟢 | 🟢 | 🟢 | 🟢 |

## Por capacidad de producto

### Catálogo

| Ítem | Estado |
|------|--------|
| Brand | 🟢 API + `/admin/marcas` + alta de producto |
| Category | 🟢 |
| Product | 🟢 |
| ProductImage | 🟢 |
| ShowPrice / WhatsApp en producto | 🟢 storefront y ficha |

### Comercio

| Ítem | Estado |
|------|--------|
| Cart | 🟢 |
| CartItem | 🟢 |
| Order | 🟢 API checkout, listado y cambio de estado |
| OrderItem | 🟢 |
| Payment | 🟢 API alta y cambio de estado |
| Stock | 🟢 opcional (`null` = sin control); se descuenta en checkout |

### Usuarios

| Ítem | Estado |
|------|--------|
| User (Identity) | 🟢 |
| Authentication JWT | 🟢 access + refresh |
| Authorization (roles/admin) | 🟢 cookie MVC + JWT Admin en writes de API |
| Refresh tokens | 🟢 issue / rotate / revoke |
| Address | 🟢 API CRUD del usuario |

### Contenido y plantilla

| Ítem | Estado |
|------|--------|
| Setting (marca, colores, contacto) | 🟢 layout + editor `/admin/sitio` |
| Banner / Page / BlogPost | 🟢 Page, Blog y Banner (home + `/admin/banners`) |
| Categorías (admin SEO) | 🟢 `/admin/categorias` + metas en alta desde producto |
| Storefront (vistas públicas) | 🟢 home, listado, ficha y categoría |
| Modo vitrina vs tienda (flag de sitio) | 🟢 `SiteMode` en Setting + API; Catalog bloquea checkout |

### SEO

| Ítem | Estado |
|------|--------|
| Campos SeoEntity | 🟢 |
| Slugs | 🟢 únicos entre producto/categoría/página/nota; 301 si cambian |
| Metadata en HTML | 🟢 |
| Sitemap | 🟢 `/sitemap.xml` |
| Canonical URLs | 🟢 |
| Schema.org | 🟢 Product, Organization, BreadcrumbList |

### Infraestructura

| Ítem | Estado |
|------|--------|
| PostgreSQL + EF Core + migraciones | 🟢 |
| JWT + Identity | 🟢 |
| Cloudinary | 🟡 credenciales OK; la API key debe tener rol con Create/Upload |
| Docker / compose | 🔴 (marcado antes por error; no hay archivos) |
| CI/CD | 🔴 |

## Trabajo reciente

- Orders + Payments por API (checkout desde carrito, estados, pagos coordinados).
- Address CRUD autenticado y refresh token (login/register/refresh/logout).
- Stock opcional en producto; `SiteMode` Catalog/Store/Hybrid (sin vistas nuevas).
- Catálogo completo en API (brands, categories, products, images).
- Carrito por usuario autenticado, sin duplicar `(CartId, ProductId)`.
- JWT en register/login; validación de contraseña y email duplicado.
- Errores HTTP sin filtrar detalles internos.
- Categorías raíz: `ParentCategoryId` 0 o null no rompe la FK.

## Siguiente bloque recomendado

1. HTTPS / www / Search Console en el despliegue.
2. Docker / compose y CI.
3. Checkout y órdenes en el storefront (vistas), si hace falta.
