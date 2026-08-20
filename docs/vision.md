# Visión del producto

SEOStore no es una tienda de un solo cliente. Es una **plantilla reutilizable** para armar sitios de productos con SEO, identidad visual y un backend común.

Cada despliegue se adapta al negocio: algunos clientes venden por internet; otros solo necesitan que el público vea el catálogo y contacte por WhatsApp, teléfono o redes.

## Problema que resuelve

Montar un sitio distinto para cada cliente es lento y caro. SEOStore concentra:

- Catálogo (productos, categorías, marcas, imágenes)
- Datos SEO por entidad (meta, Open Graph, indexación)
- Identidad del sitio (logo, colores, contacto, analytics)
- Comercio opcional (carrito, pedidos, pagos)
- Contacto comercial (WhatsApp por producto o a nivel de sitio)

## Modos de operación

El modo se define por configuración y por producto, no por un fork del código.

### 1. Vitrina / listado

Útil para negocios que no cobran en la web: talleres, mayoristas, showrooms, catálogos B2B.

- Los productos se publican con ficha, fotos y descripción.
- El precio puede ocultarse (`ShowPrice = false`).
- La acción principal es consultar (WhatsApp, teléfono o formulario).
- Carrito, checkout y pagos permanecen desactivados o invisibles.

### 2. Tienda online

Útil para venta directa.

- Precio visible (`ShowPrice = true`).
- Carrito asociado al usuario autenticado.
- Pedidos, pagos y estados de orden (pendiente de completar en el producto).
- Checkout futuro (Mercado Pago u otro proveedor).

### 3. Híbrido

Algunos productos se compran en la web y otros se cotizan. Cada ítem controla `ShowPrice` y `WhatsAppMessage` de forma independiente.

## Capacidades que hacen la plantilla versátil

| Capacidad | Uso |
|-----------|-----|
| `ShowPrice` | Mostrar u ocultar precio en vitrina |
| `WhatsAppMessage` | CTA de consulta con mensaje prearmado |
| `Setting` | Nombre, logo, colores, WhatsApp, redes, Analytics |
| `SeoEntity` | Meta tags, Open Graph, canonical, datos estructurados |
| `Published` / `Featured` | Controlar qué se muestra y qué se destaca |
| Categorías jerárquicas | Navegar catálogos grandes |
| Cloudinary | Imágenes por cliente sin guardar archivos en el servidor |
| JWT + Identity | Cuentas de cliente y, más adelante, panel admin |

## Público objetivo

- Negocios que venden en línea y necesitan catálogo + carrito.
- Negocios que solo quieren exhibir productos y generar leads.
- Quien despliega la plantilla (desarrollador o agencia) y la personaliza por cliente.

## Fuera de alcance actual

Estas piezas están en el dominio o en el roadmap, pero no forman el MVP cerrado:

- Checkout y pasarela de pago
- Stock / inventario
- Panel de administración visual
- Sitio público con plantillas Razor/tema (hay vitrina mínima; falta tema intercambiable)
- Vistas públicas de Page y BlogPost
- Integración MercadoLibre más allá de la entidad de autenticación
- Multi-tenant en una sola base (hoy se espera un despliegue por cliente)
