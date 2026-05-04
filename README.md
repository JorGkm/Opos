# Opos - Console Exam Preparation Tool

A lightweight, cross-platform console application for Spanish civil service exam preparation. Import questions from text files, take practice tests with real scoring penalties, and track your progress over time.

## Features

- **Standard Exam** - Full topic selection or individual topics
- **Failed Questions Review** - Auto-generates tests from your missed questions
- **Question & Option Shuffling** - Randomize display order to prevent memorization
- **Multiple Penalty Modes** - Standard, Opposition, Hard, Sudden Death
- **Statistics & Progress** - Track performance with visual failure bars and exam history
- **Internationalization** - Full English / Spanish language support
- **SQLite Database** - All results automatically saved locally

## Quick Start

```bash
git clone https://github.com/JorGkm/Opos.git
cd Opos
dotnet build
dotnet run
```

## Usage

1. Create a text file with your questions (format: `TEMA 1 - Topic Name` / questions numbered / options a)b)c)d) / answers after `### RESPUESTAS ###`)
2. Run Opos, select **Load** and provide the file path
3. Select exam mode: Standard or Failed Questions Review
4. Configure: Topic filter, shuffle mode, penalty type
5. Take the test using arrow keys + Enter, or direct A/B/C/D input

## Controls

| Action | Key |
|--------|-----|
| Navigate options | Up / Down arrows |
| Confirm answer | Enter |
| Skip question | Space or S |
| Direct answer | A / B / C / D |

## File Format

| Element | Format |
|---------|--------|
| Topic | `TEMA <number>` |
| Topic name (optional) | `TEMA 1 - Constitution` |
| Question | `1. Text` or `1- Text` |
| Options | `a) Text` `b) Text` `c) Text` `d) Text` |
| Answers separator | `### RESPUESTAS ###` |
| Answers | `PREG 1 - RESP: B` |

## Statistics

Select **Statistics** from the main menu to view:
- Total exams, average score, best/worst results
- Last 20 exams with date, topic, score and time
- Weakest topics with failure rate visualization
- Top 10 most frequently missed questions

## Compatibility

- Windows (Full support)
- Linux (Full support)
- macOS (Full support)

## License

This project is licensed under the terms of the [LICENSE](LICENSE) file.

---

For Spanish documentation, see [README.es.md](README.es.md)
