<div align="center">
<pre>
  $$$$$$\
 $$  __$$\
 $$ /  $$ | $$$$$$\   $$$$$$\   $$$$$$$\
 $$ |  $$ |$$  __$$\ $$  __$$\ $$  _____|
 $$ |  $$ |$$ /  $$ |$$ /  $$ |\$$$$$$\
 $$ |  $$ |$$ |  $$ |$$ |  $$ | \____$$\
  $$$$$$  |$$$$$$$  |\$$$$$$  |$$$$$$$  |
  \______/ $$  ____/  \______/ \_______/
           $$ |
           $$ |
           \__|
</pre>
</div>

<h3 align="center">Asistente de preparación para oposiciones desde la consola</h3>

<p align="center">
  <a href="README.md">English version</a>
</p>

<p align="center">
  <a href="#-características">Características</a> •
  <a href="#-instalación">Instalación</a> •
  <a href="#-uso">Uso</a> •
  <a href="#-formato-de-archivo">Formato</a> •
  <a href="#-controles">Controles</a> •
  <a href="#-compatibilidad">Compatibilidad</a>
</p>

---

## 📋 Características

**Opos** es una aplicación de consola ligera y multiplataforma diseñada para agilizar tu preparación de oposiciones. Importa preguntas desde un simple archivo de texto, realiza tests con penalizaciones reales y haz seguimiento de tu progreso a lo largo del tiempo — todo desde la terminal.

### Modos de Estudio
- **Examen Normal** — Selección por temas o todos los temas
- **Repaso de Fallos** — Genera automáticamente un test con todas las preguntas que has fallado anteriormente
- **Filtrado por Temas** — Selecciona temas específicos de archivos con múltiples temas
- **Aleatorización** — Mezcla el orden de preguntas y/o posiciones de respuestas para evitar la memorización

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
- **Preguntas Más Falladas** — Top 10 de preguntas más incorrectas con su respuesta correcta

### Métricas de Rendimiento
- Temporizador por pregunta y tiempo total de examen
- Tiempo medio de respuesta en la pantalla de resultados

---

## 🚀 Instalación

### Requisitos
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) o superior

### Compilar desde el código fuente
```bash
git clone https://github.com/JorGkm/Opos.git
cd Opos
dotnet build
dotnet run
```

### Ejecutar directamente
```bash
dotnet run --project Opos.csproj
```

---

## 📝 Uso

### 1. Crear tu archivo de preguntas
Crea un archivo `.txt` (ej. `preguntas.txt`) en tu escritorio con el siguiente formato:

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
Navega por las preguntas usando el teclado. Tras cada pregunta verás el resultado inmediatamente. Al finalizar, se muestra una pantalla de resultados detallada que se guarda automáticamente.

---

## 📄 Formato de Archivo

### Estructura Obligatoria
| Elemento | Formato |
|----------|---------|
| **Tema** | `TEMA <número>` |
| **Nombre del tema** (opcional) | `TEMA 1 - La Constitución Española`<br>`TEMA 2: Derechos Fundamentales`<br>`TEMA 3 — Organización del Estado` |
| **Pregunta** | `<número>. <texto>` o `<número>- <texto>` |
| **Opciones** | `a) <texto>`<br>`b) <texto>`<br>`c) <texto>`<br>`d) <texto>` |
| **Separador de respuestas** | `### RESPUESTAS ###` |
| **Tabla de respuestas** | `PREG 1 - RESP: B` o formato tabular similar |

---

## 🎮 Controles

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

## 📊 Estadísticas

Selecciona **"Estadísticas"** en el menú principal para ver:
- Métricas generales de rendimiento
- Historial con código de color (verde = aprobado, rojo = suspenso)
- Temas más débiles con visualización de tasa de fallo
- Preguntas más frecuentemente falladas

Todos los datos se almacenan en una base de datos SQLite local `opos.db` en el directorio de la aplicación.

---

## 🖥️ Compatibilidad

| Plataforma | Soporte |
|------------|---------|
| **Windows** | ✅ Soporte completo |
| **Linux** | ✅ Soporte completo |
| **macOS** | ✅ Soporte completo |

---

## 📸 Capturas

<p align="center">
    <a href="Screenshots/OposMenu.PNG">
        <img src="Screenshots/OposMenu.PNG" width="400" alt="Menú Principal">
    </a>
</p>

<p align="center">
    <a href="Screenshots/OposPregunta.PNG">
        <img src="Screenshots/OposPregunta.PNG" width="400" alt="Pantalla de Pregunta">
    </a>
</p>

---

## 📄 Licencia

Este proyecto está licenciado bajo los términos del archivo [LICENSE](LICENSE).
