# Compatibilidad de Frameworks

Este documento describe la compatibilidad de frameworks y las consideraciones de seguridad para `qckdev.Authentication.JwtBearer`.

## Frameworks Soportados

Este paquete soporta los siguientes frameworks de destino:

| Framework | Estado | Nivel de soporte |
|-----------|--------|------------------|
| .NET 10.0 |  Soportado | Actual  soporte hasta noviembre 2027 |
| .NET 8.0 |  Soportado | LTS  soporte hasta noviembre 2026 |
| .NET 6.0 |  Soportado | LTS  fin de soporte noviembre 2024 |
| .NET 5.0 |  Soportado | Fin de vida (EOL)  mayo 2022 |
| .NET Core 3.1 |  Soportado | Fin de vida (EOL)  diciembre 2022 |
| .NET Standard 2.0 |  Soportado | Compatibilidad multiplataforma |

## Versiones de Paquetes

La librería utiliza diferentes versiones de `System.IdentityModel.Tokens.Jwt` según el framework de destino:

| Framework | Paquete | Versión |
|-----------|---------|---------|
| netstandard2.0, netcoreapp3.1, net5.0 | System.IdentityModel.Tokens.Jwt | **6.34.0** |
| net6.0, net8.0, net10.0 | System.IdentityModel.Tokens.Jwt | **8.0.0** |

## Consideraciones de Seguridad

### Mitigaciones de Vulnerabilidades

Este paquete referencia explícitamente versiones parcheadas de la librería JWT para mitigar vulnerabilidades de seguridad conocidas:

#### CVE-2024-21319 (GHSA-59j7-ghrg-fj52)
- **Severidad**: Moderada (CVSS 6.8)
- **Problema**: Vulnerabilidad de denegación de servicio (DoS) en el manejo de tokens JWT
- **Componentes afectados**:
  - `System.IdentityModel.Tokens.Jwt` < 6.34.0
  - `Microsoft.IdentityModel.JsonWebTokens` < 6.34.0
- **Mitigación**:
  - Para `netstandard2.0`, `netcoreapp3.1`, `net5.0`: referencia explícita a `System.IdentityModel.Tokens.Jwt` versión **6.34.0** (mínima parcheada de la serie 6.x)
  - Para `net6.0`, `net8.0`, `net10.0`: referencia explícita a `System.IdentityModel.Tokens.Jwt` versión **8.0.0** (serie 8.x, sin vulnerabilidades conocidas)
- **Aviso**: https://github.com/advisories/GHSA-59j7-ghrg-fj52

### Estrategia de Selección de Versiones

Las versiones de paquetes se seleccionaron aplicando el enfoque de **Mínima Versión Viable (MVV)**:
-  Usa la **versión mínima** necesaria para mitigar la CVE-2024-21319
-  Versión alineada con la generación del framework (6.x para frameworks pre-.NET 6, 8.x para .NET 6+)
-  Evita actualizaciones innecesarias que puedan introducir cambios disruptivos
-  Auditorías de seguridad regulares con `dotnet list package --vulnerable`

## Análisis de Versiones de Paquetes

Esta sección compara las versiones usadas en este proyecto con las últimas disponibles para cada familia de framework, detallando qué se pierde por no actualizar.

> **Nota**: Las mejoras comunes a `netstandard2.0`, `netcoreapp3.1` y `net5.0` (serie 6.x) se unifican. Lo mismo para `net6.0`, `net8.0` y `net10.0` (serie 8.x).

---

### netstandard2.0 / netcoreapp3.1 / net5.0  System.IdentityModel.Tokens.Jwt 6.34.0  6.36.0

> Los frameworks `netcoreapp3.1` y `net5.0` están en **EOL**. La 6.36.0 es la última de la serie 6.x.

| Paquete | Versión actual | Última disponible | Diferencia |
|---------|---------------|-------------------|------------|
| System.IdentityModel.Tokens.Jwt | 6.34.0 | **6.36.0** | 2 parches |

**Qué incluyen las versiones 6.35.0  6.36.0:**

- Correcciones menores de errores en la validación de tokens
- Mejoras de rendimiento en la serialización/deserialización de claims
- Ajustes en el manejo de tokens con algoritmos de firma poco habituales
- Sin correcciones de vulnerabilidades de seguridad adicionales respecto a 6.34.0

**Recomendación**:  Los frameworks `netcoreapp3.1` y `net5.0` están en EOL. Para `netstandard2.0`, la versión 6.34.0 es suficiente desde el punto de vista de seguridad. Actualizar a 6.36.0 aporta correcciones menores de estabilidad pero ningún beneficio de seguridad.

---

### net6.0 / net8.0 / net10.0  System.IdentityModel.Tokens.Jwt 8.0.0  8.16.0

| Paquete | Versión actual | Última disponible | Diferencia |
|---------|---------------|-------------------|------------|
| System.IdentityModel.Tokens.Jwt | 8.0.0 | **8.16.0** | 16 versiones menores |

**Qué incluyen las versiones 8.1.0  8.16.0:**

#### Correcciones de Errores
- **Validación de tokens**: correcciones en la validación de `aud`, `iss` y `exp` en escenarios con múltiples emisores o audiencias
- **Manejo de claims**: correcciones en la asignación de tipos de claims personalizados y en el mapeo de claims estándar
- **Algoritmos de firma**: mejoras en el soporte de algoritmos RSA y ECDSA, y correcciones en la verificación de firmas con claves de diferentes longitudes
- **Tokens cifrados (JWE)**: correcciones en el descifrado de tokens con ciertos algoritmos de envoltura de clave
- **Compatibilidad con .NET**: ajustes de compatibilidad para las versiones más recientes del runtime de .NET

#### Mejoras de Rendimiento
- Reducción de asignaciones de memoria en la validación y generación de tokens
- Serialización de claims más eficiente mediante mejoras en el uso de `Span<T>` y buffers reutilizables
- Caché de claves de firma mejorada para reducir la carga en escenarios de alta concurrencia

#### Nuevas Funcionalidades
- Soporte mejorado para **validación asíncrona** de tokens
- API más completa para la gestión de `TokenValidationParameters`
- Mejor soporte para **tokens de larga duración** y rotación de claves
- Compatibilidad extendida con los estándares **OpenID Connect** y **OAuth 2.0**

#### Sin Vulnerabilidades de Seguridad Conocidas
- No se han registrado CVEs en la serie 8.x de `System.IdentityModel.Tokens.Jwt`

**Recomendación**:  **Actualización recomendada** a 8.16.0 para beneficiarse de las mejoras de rendimiento, las correcciones de errores acumuladas y las nuevas funcionalidades de la API de validación.

---

## Recomendaciones de Estrategia de Actualización

### Prioridad 1  Crítica (Inmediata)
- **Ninguna**  Todos los frameworks utilizan versiones sin vulnerabilidades críticas sin parchear (CVE-2024-21319 ya está mitigada)

### Prioridad 2  Alta (En el próximo mes)
- **net6.0 / net8.0 / net10.0**: Actualizar `System.IdentityModel.Tokens.Jwt` a **8.16.0** para beneficiarse de las mejoras de rendimiento y correcciones acumuladas

### Prioridad 3  Baja (Cuando sea conveniente)
- **netstandard2.0**: Actualizar a **6.36.0** para correcciones menores de estabilidad
- **netcoreapp3.1 / net5.0**: Priorizar la migración a .NET 6.0+ sobre la actualización de paquetes

### Ruta de Migración

Para aplicaciones que usen frameworks EOL:

1. **.NET Core 3.1**  Migrar a .NET 8.0 LTS
2. **.NET 5.0**  Migrar a .NET 8.0 LTS
3. **.NET 6.0**  Planificar migración a .NET 8.0 o .NET 10.0

## Diferencias con qckdev.AspNetCore.Authentication.JwtBearer

| Característica | qckdev.Authentication.JwtBearer | qckdev.AspNetCore.Authentication.JwtBearer |
|---|---|---|
| Dependencia de ASP.NET Core |  No |  Sí |
| Uso en aplicaciones de consola / escritorio |  Sí |  No |
| Middleware de autenticación |  No |  Sí |
| Generación y validación de tokens |  Sí |  Sí (a través de la infraestructura de ASP.NET Core) |

## Verificación

Para comprobar que no existen vulnerabilidades conocidas en este paquete:

```powershell
dotnet list package --vulnerable --include-transitive
```

Resultado esperado:
```
The given project has no vulnerable packages given the current sources.
```

## Guía de Migración

### Actualización desde versiones anteriores

Si actualizas desde una versión que usa `System.IdentityModel.Tokens.Jwt` < 6.34.0:

1. Actualiza la referencia al paquete:
   ```xml
   <PackageReference Include="qckdev.Authentication.JwtBearer" Version="0.2.0-alpha" />
   ```

2. No se requieren cambios en el código  la API es compatible

3. Verifica que la aplicación sigue funcionando correctamente

### Buenas Prácticas de Seguridad

1. **Tiempo de vida del token**: establece siempre tiempos de expiración apropiados
2. **Claves secretas**: usa claves fuertes generadas aleatoriamente (mínimo 256 bits)
3. **HTTPS**: usa siempre HTTPS en producción
4. **Validación**: valida todos los claims del token en el lado del servidor
5. **Actualizaciones regulares**: mantén el paquete actualizado para recibir parches de seguridad

## Recursos Adicionales

- [JWT.io](https://jwt.io/)  Decodificador y documentación de JWT
- [RFC 7519](https://tools.ietf.org/html/rfc7519)  Especificación JSON Web Token
- [Microsoft Identity Model Extensions](https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet)
- [Política de soporte de .NET](https://dotnet.microsoft.com/platform/support/policy)

## Última Actualización

Documento actualizado el: 25 de febrero de 2026

Para información más reciente, consulta el [repositorio en GitHub](https://github.com/hfrances/qckdev.Authentication.JwtBearer).
