# Changelog

## 2026-08-18

- Checkout API: órdenes desde el carrito, pagos coordinados, direcciones y stock opcional.
- `SiteMode` Catalog/Store/Hybrid; en Catalog el checkout responde 400.
- Refresh tokens: login/register emiten par access+refresh; `/api/auth/refresh` rota y `/api/auth/logout` revoca.
- Autenticación JWT: register y login devuelven token.
- Validación de contraseña (8+ caracteres, mayúscula, minúscula y dígito) y email único.
- Errores de registro visibles (contraseña débil, email duplicado) sin filtrar excepciones internas.
- Documentación de producto: visión, arquitectura, dominio, setup, API, estado y roadmap.
- Cloudinary: si hay credenciales, las fotos ya no caen en silencio a `wwwroot/uploads`; se muestra el error de permisos Create/Upload.
- Fotos de producto más chicas, con recuadro para centrar y zoom − / + al cargar y al ver el detalle.
- Storefront público por slug, Setting en el layout, metas/OG/canonical/Schema, robots.txt y sitemap.xml.
- Alta de producto solo con rol Admin (`/cuenta/ingresar`); menú público sin “Cargar producto”.
- Páginas públicas (`/pagina/{slug}`), blog (`/blog`), editor de Setting y writes de API de catálogo solo con JWT Admin.
- 301 si cambia el slug de producto, categoría, página o nota; slugs únicos entre esas entidades.
- Admin de categorías (`/admin/categorias`) con título y meta propios; el alta de producto también pide SEO de categoría nueva.
- Swagger solo lista controladores API (`/api/...`). Páginas, blog y Setting tienen REST JSON; el admin MVC ya no redirige a HTML de login cuando el cliente pide JSON.
- Banners en el home (`/admin/banners`, `/api/banners`) y marcas en admin (`/admin/marcas`) + selector en el alta de producto.

## 2026-08-17

- Carrito completo (servicio, repositorio, API autenticada).
- Restricción única `(CartId, ProductId)` en base de datos.
- Respuestas de error genéricas en controladores; sin `ex.Message` al cliente.
- Categorías raíz: `ParentCategoryId` nulo o 0 no viola la FK.

## 2026-08-03

- Entidades Payment, Address y RefreshToken + migración.
- Identity y DbContext ampliados.

## 2026-08-01 / 02

- Solución inicial: Domain, Application, Infrastructure, Web.
- Catálogo (Product, Category, Brand, ProductImage) y `SeoEntity`.
- Migración `InitialCreate` sobre PostgreSQL.
