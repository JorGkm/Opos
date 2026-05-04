# Opos - Asistente de preparación para oposiciones

```
  $$$$$$\
 $$  __$$\
 $$ /  $$ | $$$$$$\   $$$$$$\   $$$$$$$\
 $$ |  $$ |$$  __$$\ $$  __$$\ $$  _____|
 $$ |  $$ |$$ /  $$ |$$ /  $$ |\$$$$$$\
 $$ |  $$ |$$ |  $$ |$$ |  $$ | \____$$\
  $$$$$$  |$$$$$$$  |\$$$$$$  |$$$$$$$  |
  \______/ $$  ____/  \______/  \_______/
           $$ |
           $$ |
           \__|
```

Una aplicación de consola ligera y multiplataforma para preparar oposiciones. Importa preguntas desde archivos de texto, realiza tests con penalizaciones reales y haz seguimiento de tu progreso en el tiempo — todo desde la terminal.

## Características

**Opos** está diseñado para agilizar tu preparación de oposiciones:

### Modos de Estudio
- **Examen Normal** — Selección por temas o todos los temas
- **Repaso de Fallos** — Genera automáticamente tests con preguntas falladas
- **Filtrado por Temas** — Selecciona temas específicos de archivos con múltiples temas
- **Aleatorización** — Mezcla el orden de preguntas y/o opciones de respuesta

### Puntuación y Penalizaciones
Elige entre cuatro sistemas de puntuación habituales en exámenes oficiales:

| Modo | Penalización |
|------|-------------|
| Estándar | Sin penalización por fallos |
| Oposición | 3 fallos restan 1 acierto |
| Duro | 2 fallos restan 1 acierto |
| Muerte Súbita | 1 fallo resta 1 acierto |

### Estadísticas y Seguimiento
Todos los resultados se almacenan en una base de datos SQLite local:
- **Resumen General** — Total de exámenes, nota media, mejor y peor resultado
- **Historial** — Últimos 20 exámenes con fecha, tema, nota y tiempo
- **Temas Más Débiles** — Barras visuales de porcentaje de fallo por tema
- **Preguntas Más Falladas** — Top 10 de preguntas más incorrectas

### Métricas de Rendimiento
- Temporizador por pregunta y tiempo total de examen
- Tiempo medio de respuesta en la pantalla de resultados

---

## Inicio Rápido

```bash
git clone https://github.com/JorGkm/Opos.git
cd Opos
dotnet build
dotnet run
```

---

## Uso

### 1. Crear tu archivo de preguntas
Crea un archivo `.txt` (ej. `preguntas.txt`) con el siguiente formato:

```
TEMA 1 - La Constitución Española
1. ¿Cuál es la capital de España?
a) Barcelona
b) Madrid
c) Sevilla
d) Valencia

2. ¿Cuántas comunidades autónomas tiene España?
a) 15
b) 17
c) 19
d) 20

### RESPUESTAS ###
PREG 1 - RESP: B
PREG 2 - RESP: B
```

### 2. Cargar preguntas
Desde el menú principal, selecciona **"Cargar"** e introduce la ruta de tu archivo `.txt`. También puedes arrastrar y soltar el archivo directamente en la consola.

### 3. Configurar e iniciar
Al seleccionar **"Comenzar"**, se te pedirá:
1. **Elegir modo** — Examen normal o repaso de fallos
2. **Seleccionar temas** — Todos o uno específico (si hay varios)
3. **Elegir aleatorización** — Preguntas, opciones, ambas o ninguna
4. **Definir penalización** — Selecciona tu sistema de puntuación

### 4. Responder y revisar
Navega por las preguntas usando el teclado (ver controles abajo). Tras cada pregunta verás el resultado inmediatamente. Al finalizar, se muestra una pantalla de resultados detallada que se guarda automáticamente.

---

## Controles

Durante el test, ambos métodos funcionan simultáneamente:

**Navegación Visual:**
| Tecla | Acción |
|-------|--------|
| `↑` / `↓` | Navegar entre opciones |
| `Enter` | Confirmar la opción seleccionada |
| `Espacio` | Saltar la pregunta |

**Entrada Directa:**
| Tecla | Acción |
|-------|--------|
| `A` / `B` / `C` / `D` | Responder directamente |
| `S` | Saltar la pregunta |

---

## Formato de Archivo

### Estructura Obligatoria
| Elemento | Formato |
|----------|---------|
| **Tema** | `TEMA <número>` |
| **Nombre del tema** (opcional) | `TEMA 1 - La Constitución Española`<br>`TEMA 2`: Derechos Fundamentales`<br>`TEMA 3 — Organización del Estado` |
| **Pregunta** | `<número>. <texto>` o `<número>- <texto>` |
| **Opciones** | `a) <texto>`<br>`b) <texto>`<br>`c) <texto>`<br>`d) <texto>` |
| **Separador de respuestas** | `### RESPUESTAS ###` |
| **Tabla de respuestas** | `PREG 1 - RESP: B` o formato tabular similar |

---

## Tipos de Penalización

| Modo | Penalización |
|------|-------------|
| Estándar | Sin penalización por fallos |
| Oposición | 3 fallos restan 1 acierto |
| Duro | 2 fallos restan 1 acierto |
| Muerte Súbita | 1 fallo resta 1 acierto |

---

## Estadísticas

Selecciona **"Estadísticas"** en el menú principal para ver:
- Total de exámenes, nota media, mejor y peor resultado
- Historial con código de color (verde = aprobado, rojo = suspenso)
- Temas más débiles con visualización de tasa de fallo
- Top 10 preguntas más frecuentemente falladas

Todos los datos se almacenan en una base de datos SQLite local `opos.db` en el directorio de la aplicación.

---

## Compatibilidad

| Plataforma | Soporte |
|------------|---------|
| **Windows** | Soporte completo |
| **Linux** | Soporte completo |
| **macOS** | Soporte completo |

---

## Capturas

### Menú Principal
![Menú Principal](Screenshots/OposMenu.PNG)

### Pantalla de Pregunta
![Pantalla de Pregunta](Screenshots/OposPregunta.PNG)

---

## Licencia

Este proyecto está licenciado bajo los términos del archivo [LICENSE](LICENSE).

---
For English documentation, see [README.md](README.md)
