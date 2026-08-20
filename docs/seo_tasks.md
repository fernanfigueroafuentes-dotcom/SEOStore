# Tareas SEO de la plantilla

Lista de trabajo para que SEOStore sea apta para SEO. El **bloque 1** es el mínimo para indexar. El **bloque 2** cierra una plantilla seria. Lo demás queda para más adelante.

Leyenda: 🟢 hecho · 🟡 parcial · 🔴 pendiente

## Bloque 1 — Storefront público (en curso)

Sin esto Google no tiene un sitio que rastrear.

| # | Tarea | Estado |
|---|--------|--------|
| 1.1 | Storefront separado del admin: home, listado, ficha y categoría públicos | 🟢 |
| 1.2 | `Cargar producto` fuera del menú público; `noindex` + login rol Admin | 🟢 |
| 1.3 | URLs por slug: `/producto/{slug}`, `/categoria/{slug}`, `/productos` | 🟢 |
| 1.4 | Redirección 301 de `/Catalog/Details/{id}` al slug si el producto está publicado | 🟢 |
| 1.5 | Metadata en HTML: title, description, OG, robots index/follow, canonical | 🟢 |
| 1.6 | Campos SEO editables en el alta (o fallback nombre + descripción corta) | 🟢 |
| 1.7 | `Setting` cableado: nombre, logo, favicon, colores, WhatsApp, Analytics/GTM | 🟢 |
| 1.8 | `robots.txt` (excluye `/Catalog`, `/api`, `/swagger`, `/cuenta`) | 🟢 |
| 1.9 | `sitemap.xml` (home, listado, productos y categorías `Published` + `Index`) | 🟢 |

## Bloque 2 — Plantilla SEO seria

Va junto al storefront en esta entrega cuando aplica a las vistas públicas.

| # | Tarea | Estado |
|---|--------|--------|
| 2.1 | Schema.org JSON-LD: `Organization`, `Product` (Offer solo si `ShowPrice`), `BreadcrumbList` | 🟢 |
| 2.2 | Canonical absoluto (un URL por producto; sin id) | 🟢 |
| 2.3 | Contenido indexable: H1 único, descripción HTML, texto de categoría, `alt` en fotos | 🟢 |
| 2.4 | `Published = false` o soft-delete → 404 (no URL viva) | 🟢 |
| 2.5 | Imágenes: `width`/`height`, `loading="lazy"` en listados, recorte `c_limit` (sin `g_auto`) | 🟢 |
| 2.6 | Vistas públicas de páginas (`nosotros`, envíos) y blog | 🟢 |

## Bloque 3 — Robustez posterior

| # | Tarea | Estado |
|---|--------|--------|
| 3.1 | Proteger writes de API de catálogo con rol Admin | 🟢 |
| 3.2 | Redirect 301 cuando cambia el slug | 🟢 |
| 3.3 | Unicidad de slugs entre productos, categorías y páginas | 🟢 |
| 3.4 | Editor de `Setting` en el admin (hoy se siembra desde config) | 🟢 |
| 3.5 | Página de categoría con meta propia editable en UI | 🟢 |
| 3.6 | HTTPS / www / Search Console (despliegue) | 🔴 |

## Rutas públicas vs admin

| URL | Quién | Indexable |
|-----|--------|-----------|
| `/` | Público | Sí |
| `/productos` | Público | Sí |
| `/pagina/{slug}` | Público (solo `Published`) | Según `Index` |
| `/blog` | Público | Sí |
| `/blog/{slug}` | Público (solo `Published`) | Según `Index` |
| `/admin/sitio` | Admin | `noindex` |
| `/admin/categorias` | Admin | `noindex` |
| `/admin/marcas` | Admin | `noindex` |
| `/admin/banners` | Admin | `noindex` |
| `/admin/paginas` | Admin | `noindex` |
| `/admin/blog` | Admin | `noindex` |
| `/sitemap.xml` | Público | — |
| `/robots.txt` | Público | — |
| `/Catalog/Create` | Admin | `noindex` |
| `/Catalog` | Admin (anónimo redirige a `/productos`) | `noindex` |
| `/cuenta/ingresar` | Login admin | `noindex` |
| `/api/*`, `/swagger` | Máquina / dev | Disallow |
