# Roadmap

La plantilla se construye por fases. Cada fase debe servir a **los dos modos**: vitrina y tienda. Lo específico de checkout no bloquea un primer despliegue tipo catálogo.

## Hecho

- FASE 1 Domain (entidades de catálogo, comercio, contenido, SEO)
- FASE 2 Application (catálogo + carrito)
- FASE 3 Infrastructure (EF, PostgreSQL, Identity, repositorios, Cloudinary)
- FASE 4 Web/API (CRUD catálogo, auth JWT, carrito, manejo de errores)

## En curso / siguiente

### FASE 4.1 — Plantilla configurable

- API de `Setting` (nombre, logo, colores, WhatsApp, redes, analytics) (hecho)
- Modo de sitio: `Catalog` | `Store` | `Hybrid` (hecho en API)
- Roles: `Admin` vs cliente (hecho)
- Bloquear POST/PUT/DELETE de catálogo a Admin (hecho)

### FASE 4.2 — Storefront

- Layout con colores y logo de `Setting` (hecho)
- Home: destacados + categorías + banners (hecho)
- Listado por categoría / ficha por slug (hecho)
- Páginas estáticas y blog (hecho)
- 301 al cambiar slug y unicidad cruzada (hecho)
- Admin de categorías, marcas y banners (hecho)

### FASE 5 — Commerce completo

- Cart (hecho)
- Orders API: crear desde carrito, listar, cambiar estado (hecho; sin vistas)
- Payments API: pago coordinado + cambio de estado (hecho; sin vistas)
- Stock opcional (hecho)
- Checkout desactivable cuando el modo es `Catalog` (hecho)
- Checkout y órdenes en el storefront (vistas) — pendiente a propósito

### FASE 6 — SEO

- Rutas públicas por slug (hecho)
- Metadata, Open Graph, canonical, robots, sitemap, Schema.org (hecho)
- Vistas públicas de Page/BlogPost (hecho)
- `Index` / `Follow` respetados en meta robots (hecho)
- 301 al cambiar slug; unicidad entre producto, categoría, página y nota (hecho)

### FASE 7 — Deployment

- Dockerfile + docker-compose (app + PostgreSQL)
- CI/CD
- Variables de entorno por cliente (connection string, JWT, Cloudinary, WhatsApp)
- Guía de despliegue en VPS

## Ideas posteriores

- Multi-tenant (varios clientes en una sola instancia)
- Sincronización Mercado Libre
- Temas visuales intercambiables
- Formulario de consulta además de WhatsApp
