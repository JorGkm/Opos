# Opos - Asistente de preparación para oposiciones

Una aplicación de consola ligera y multiplataforma para preparar oposiciones. Importa preguntas desde archivos de texto, realiza tests con penalizaciones reales y haz seguimiento de tu progreso en el tiempo.

## Características

- **Examen Normal** - Selección por temas o todos los temas
- **Repaso de Fallos** - Genera automáticamente tests con preguntas falladas
- **Aleatorización** - Mezcla orden de preguntas y opciones para evitar memorización
- **Múltiples Modos de Penalización** - Estándar, Oposición, Duro, Muerte Súbita
- **Estadísticas y Progreso** - Seguimiento con barras visuales e historial
- **Internacionalización** - Soporte completo en inglés / español
- **Base de Datos SQLite** - Todos los resultados se guardan automáticamente

## Inicio Rápido

```bash
git clone https://github.com/JorGkm/Opos.git
cd Opos
dotnet build
dotnet run
```

## Uso

1. Crea un archivo de texto con tus preguntas (formato: `TEMA 1 - Nombre` / preguntas numeradas / opciones a)b)c)d) / respuestas tras `### RESPUESTAS ###`)
2. Ejecuta Opos, selecciona **Cargar** e introduce la ruta del archivo
3. Selecciona modo: Examen Normal o Repaso de Fallos
4. Configura: Filtro de temas, aleatorización, tipo de penalización
5. Realiza el test usando flechas + Enter, o entrada directa A/B/C/D

## Controles

| Acción | Tecla |
|--------|-------|
| Navegar opciones | Flechas Arriba / Abajo |
| Confirmar respuesta | Enter |
| Saltar pregunta | Espacio o S |
| Respuesta directa | A / B / C / D |

## Formato de Archivo

| Elemento | Formato |
|---------|---------|
| Tema | `TEMA <número>` |
| Nombre de tema (opcional) | `TEMA 1 - Constitución` |
| Pregunta | `1. Texto` o `1- Texto` |
| Opciones | `a) Texto` `b) Texto` `c) Texto` `d) Texto` |
| Separador de respuestas | `### RESPUESTAS ###` |
| Respuestas | `PREG 1 - RESP: B` |

## Estadísticas

Selecciona **Estadísticas** en el menú principal para ver:
- Total de exámenes, nota media, mejor/peor resultado
- Últimos 20 exámenes con fecha, tema, nota y tiempo
- Temas más débiles con visualización de tasa de fallo
- Top 10 preguntas más frecuentemente falladas

## Compatibilidad

- Windows (Soporte completo)
- Linux (Soporte completo)
- macOS (Soporte completo)

## Licencia

Este proyecto está licenciado bajo los términos del archivo [LICENSE](LICENSE).

---

For English documentation, see [README.md](README.md)
