# Cómo ejecutar el proyecto

## Requisitos

- .NET SDK 9
- PostgreSQL accesible (local o contenedor)
- (Opcional) cuenta Cloudinary para subir imágenes
- (Opcional) Docker, aún no hay `docker-compose` en el repositorio

## Base de datos

Crear la base `SEOStoreDb` o dejar que EF la cree al migrar. La cadena por defecto:

```
Host=127.0.0.1;Port=5433;Database=SEOStoreDb;Username=postgres;Password=postgres
```

Puedes cambiarla en:

- `src/SEOStore.Web/appsettings.json`
- `src/SEOStore.Web/Properties/launchSettings.json` (`ConnectionStrings__DefaultConnection`)

Al iniciar `SEOStore.Web`, se ejecuta `Database.Migrate()`. No hace falta `dotnet ef database update` en el flujo normal.

Migración manual:

```powershell
Set-Location "src"
dotnet ef database update --project SEOStore.Infrastructure --startup-project SEOStore.Web
```

## JWT

La clave de firma no va en `appsettings.json`. Está en user secrets del proyecto Web:

```powershell
dotnet user-secrets list --project src/SEOStore.Web
```

Debe existir `Jwt:Key` con al menos 32 bytes. Si falta:

```powershell
dotnet user-secrets set "Jwt:Key" "REEMPLAZAR_CON_CLAVE_LARGA_Y_ALEATORIA" --project src/SEOStore.Web
```

Issuer y audience están en `appsettings.json` (`SEOStore` / `SEOStore.Api`). Expiración: 60 minutos.

## Cloudinary

Crea un archivo `.env` en la raíz del repositorio (`SEOStore/.env`) a partir de `.env.example`:

```
CLOUDINARY_CLOUD_NAME=tu_cloud_name
CLOUDINARY_API_KEY=tu_api_key
CLOUDINARY_API_SECRET=tu_api_secret
```

También vale la URL única del dashboard:

```
CLOUDINARY_URL=cloudinary://API_KEY:API_SECRET@CLOUD_NAME
```

El archivo `.env` no se versiona. Reinicia `dotnet run` después de guardarlo. Si faltan estas claves, las fotos se guardan en `wwwroot/uploads`. Si las claves existen, las fotos van solo a Cloudinary (ya no hay fallback silencioso a disco).

La API key del *product environment* debe tener permiso de **Create/Upload**. Si Cloudinary responde `missing permissions (actions=["create"])`, en [API Keys](https://console.cloudinary.com/app/settings/api-keys) edita la key y asígnale el rol **Master Admin** (o genera una nueva con ese rol). Opcional: `CLOUDINARY_UPLOAD_PRESET` para subida unsigned.

## Arranque

```powershell
Set-Location "src/SEOStore.Web"
dotnet run
```

- API / Swagger: `http://localhost:5251/swagger`
- HTTPS (perfil https): `https://localhost:7025`
- Vitrina: `/`, `/productos`, `/producto/{slug}`, `/categoria/{slug}`
- `robots.txt` y `sitemap.xml` en la raíz del sitio
- Admin: `/cuenta/ingresar` (por defecto `admin@local.test` / `Admin1234`)
- Páginas: `/pagina/nosotros`, `/pagina/envios` (se siembran al primer arranque)
- Blog: `/blog`
- Editor: `/admin/sitio`, `/admin/paginas`, `/admin/blog`

Los POST de API de catálogo piden `Authorization: Bearer` de un usuario **Admin**. En Swagger: login en `/api/auth/login` con esa cuenta.

Cambiá el admin en `appsettings.json` (`Admin:Email` / `Admin:Password`) o en `.env` (`ADMIN_EMAIL`, `ADMIN_PASSWORD`) **antes del primer arranque**. Si el usuario ya existe, no se pisa la contraseña.

El nombre del sitio, colores y WhatsApp se siembran en `Settings` la primera vez (`Site:*` en appsettings o `SITE_NAME` / `SITE_WHATSAPP` en `.env`).

## Registro de prueba

La contraseña debe tener 8+ caracteres, una mayúscula, una minúscula y un dígito.

```json
POST /api/auth/register
{
  "email": "user@example.com",
  "password": "JwtPass123",
  "firstName": "Fer",
  "lastName": "Figueroa"
}
```

La respuesta incluye `accessToken`. En Swagger: Authorize → `Bearer {token}`.

## Compilar

```powershell
dotnet build
```

Ejecutar esto con `dotnet run` activo puede fallar porque Windows bloquea las DLL en uso. Detén el servidor y vuelve a compilar.
