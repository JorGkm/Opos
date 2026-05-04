# Opos - Console Exam Preparation Tool

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

A lightweight, cross-platform console application for Spanish civil service exam preparation. Import questions from text files, take practice tests with real scoring penalties, and track your progress over time — all from the terminal.

## Features

**Opos** is designed to streamline your civil service exam preparation:

### Study Modes
- **Standard Exam** — Full topic selection or individual topics
- **Failed Questions Review** — Auto-generates a test from your missed questions
- **Topic Filtering** — Select specific topics from multi-topic files
- **Shuffle** — Randomize question order and/or answer positions

### Scoring & Penalties
Choose from four penalty systems used in official exams:

| Mode | Penalty |
|------|---------|
| Standard | No penalty for wrong answers |
| Opposition | 3 wrong answers deduct 1 correct |
| Hard | 2 wrong answers deduct 1 correct |
| Sudden Death | 1 wrong answer deducts 1 correct |

### Statistics & Progress
All results are stored in a local SQLite database:
- **General Summary** — Total exams, average score, best/worst results
- **Exam History** — Last 20 exams with date, topic, score and time
- **Weakest Topics** — Visual failure rate bars per topic
- **Most Missed** — Top 10 most frequently incorrect answers

### Performance Metrics
- Real-time timer per question and overall exam
- Average response time displayed at results screen

---

## Quick Start

```bash
git clone https://github.com/JorGkm/Opos.git
cd Opos
dotnet build
dotnet run
```

---

## Usage

### 1. Create Your Questions File
Create a `.txt` file (e.g., `questions.txt`) with the following format:

```
TOPIC 1 - The Spanish Constitution
1. What is the capital of Spain?
a) Barcelona
b) Madrid
c) Seville
d) Valencia

2. How many autonomous communities does Spain have?
a) 15
b) 17
c) 19
d) 20

### RESPUESTAS ###
PREG 1 - RESP: B
PREG 2 - RESP: B
```

### 2. Load Questions
From the main menu, select **"Load"** and provide the path to your `.txt` file. You can also drag and drop the file directly into the console.

### 3. Configure & Start
When you select **"Start"**, you'll be prompted to:
1. **Choose exam mode** — Standard exam or failed questions review
2. **Select topics** — All topics or a specific one (if multiple exist)
3. **Choose shuffling** — Randomize questions, options, both, or neither
4. **Set penalty mode** — Pick your scoring system

### 4. Answer & Review
Navigate through questions using keyboard controls (see below). After each question, you'll see instant feedback. At the end, a detailed results screen is shown and automatically saved.

---

## Controls

During the test, both methods work simultaneously:

**Visual Navigation:**
| Key | Action |
|-----|--------|
| `↑` / `↓` | Navigate between options |
| `Enter` | Confirm selected option |
| `Space` | Skip the question |

**Direct Input:**
| Key | Action |
|-----|--------|
| `A` / `B` / `C` / `D` | Answer directly |
| `S` | Skip the question |

---

## File Format

### Required Structure
| Element | Format |
|----------|---------|
| **Topic** | `TEMA <number>` |
| **Topic name** (optional) | `TOPIC 1 - The Constitution`<br>`TOPIC 2`: Fundamental Rights`<br>`TOPIC 3 — State Organization` |
| **Question** | `<number>. <text>` or `<number>- <text>` |
| **Options** | `a) <text>`<br>`b) <text>`<br>`c) <text>`<br>`d) <text>` |
| **Answers separator** | `### RESPUESTAS ###` |
| **Answers table** | `PREG 1 - RESP: B` or similar tabular format |

---

## Penalty Modes

| Mode | Penalty |
|------|---------|
| Standard | No penalty for wrong answers |
| Opposition | 3 wrong answers deduct 1 correct |
| Hard | 2 wrong answers deduct 1 correct |
| Sudden Death | 1 wrong answer deducts 1 correct |

---

## Statistics

Select **"Statistics"** from the main menu to view:
- Total exams taken, average score, best/worst results
- Color-coded exam history (green = pass, red = fail)
- Weakest topics with failure rate visualization
- Top 10 most frequently missed questions

All data is stored in a local `opos.db` SQLite database in the application directory.

---

## Compatibility

| Platform | Support |
|----------|---------|
| **Windows** | Full support |
| **Linux** | Full support |
| **macOS** | Full support |

---

## Screenshots

### Main Menu
![Main Menu](Screenshots/OposMenu.PNG)

### Question Screen
![Question Screen](Screenshots/OposPregunta.PNG)

---

## License

This project is licensed under the terms of the [LICENSE](LICENSE) file.

---
For Spanish documentation, see [README.es.md](README.es.md)
