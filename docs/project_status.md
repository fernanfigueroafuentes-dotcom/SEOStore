SEOStore
│
├── DOMAIN
│   ├── Brand          🟢
│   ├── Category       🟢
│   ├── Product        🟢
│   └── ProductImage   🟢
│
├── APPLICATION
│   ├── Brand          🟢
│   ├── Category       🟢
│   ├── Product        🟢
│   └── ProductImage   🟢
│
├── INFRASTRUCTURE
│   ├── Brand          🟢
│   ├── Category       🟢
│   ├── Product        🟢
│   └── ProductImage   🟢
│
└── WEB
    ├── Brand          🟢
    ├── Category       🟢
    ├── Product        🟢
    └── ProductImage   🟢


CATÁLOGO
├── Brand          🟢
├── Category       🟢
├── Product        🟢
└── ProductImage   🟢

COMERCIO
├── Cart           🔴
├── CartItem       🔴
├── Order          🔴
└── OrderItem      🔴

USUARIOS
├── User           🟡
├── Authentication 🟡
└── Authorization  🔴

SEO
├── Slugs          🟡
├── Metadata       🔴
├── Sitemap        🔴
├── Canonical URLs 🔴
└── Schema.org     🔴

INFRAESTRUCTURA
├── PostgreSQL     🟢
├── Docker         🟢
├── EF Core        🟢
└── Migrations     🟢



##CURRENT TASK

Crear modelo de negocio para Cart

                    CART
                     │
          ┌──────────┴──────────┐
          ↓                     ↓
       CartItem              Product
          │
          ↓
       Quantity
          │
          ↓
      Application
          │
     ┌────┴─────┐
     ↓          ↓
   DTOs      Services
     │          │
     └────┬─────┘
          ↓
      Repository
          ↓
     Infrastructure
          ↓
      PostgreSQL
          ↓
         API