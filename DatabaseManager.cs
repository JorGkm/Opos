using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace Opos;

public class DatabaseManager
{
    private readonly string _connectionString;

    public DatabaseManager()
    {
        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "opos.db");
        _connectionString = $"Data Source={dbPath}";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        string createResults = """
            CREATE TABLE IF NOT EXISTS exams (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                date TEXT NOT NULL,
                topic TEXT,
                total_questions INTEGER NOT NULL,
                correct INTEGER NOT NULL,
                wrong INTEGER NOT NULL,
                skipped INTEGER NOT NULL,
                score REAL NOT NULL,
                raw_score REAL NOT NULL,
                penalty_mode TEXT NOT NULL,
                time_seconds INTEGER NOT NULL,
                average_answer_time REAL NOT NULL
            )
            """;

        string createFailures = """
            CREATE TABLE IF NOT EXISTS failures (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                exam_id INTEGER NOT NULL,
                question_number INTEGER NOT NULL,
                question_text TEXT NOT NULL,
                topic TEXT NOT NULL,
                correct_answer TEXT NOT NULL,
                FOREIGN KEY (exam_id) REFERENCES exams(id) ON DELETE CASCADE
            )
            """;

        using var cmdResults = new SqliteCommand(createResults, connection);
        cmdResults.ExecuteNonQuery();

        using var cmdFailures = new SqliteCommand(createFailures, connection);
        cmdFailures.ExecuteNonQuery();
    }

    public void SaveExam(ExamResult result, List<Question> missedQuestions)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();
        try
        {
            string insertExam = """
                INSERT INTO exams (date, topic, total_questions, correct, wrong, skipped, score, raw_score, penalty_mode, time_seconds, average_answer_time)
                VALUES (@date, @topic, @total, @correct, @wrong, @skipped, @score, @rawScore, @penalty, @time, @avgTime)
                """;

            using var cmd = new SqliteCommand(insertExam, connection, transaction);
            cmd.Parameters.AddWithValue("@date", result.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@topic", (object?)result.Topic ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@total", result.TotalQuestions);
            cmd.Parameters.AddWithValue("@correct", result.Correct);
            cmd.Parameters.AddWithValue("@wrong", result.Wrong);
            cmd.Parameters.AddWithValue("@skipped", result.Skipped);
            cmd.Parameters.AddWithValue("@score", result.Score);
            cmd.Parameters.AddWithValue("@rawScore", result.RawScore);
            cmd.Parameters.AddWithValue("@penalty", result.PenaltyMode);
            cmd.Parameters.AddWithValue("@time", result.TimeSeconds);
            cmd.Parameters.AddWithValue("@avgTime", result.AverageAnswerTime);
            cmd.ExecuteNonQuery();

            int examId = Convert.ToInt32(new SqliteCommand("SELECT last_insert_rowid()", connection, transaction).ExecuteScalar());

            string insertFailure = """
                INSERT INTO failures (exam_id, question_number, question_text, topic, correct_answer)
                VALUES (@examId, @num, @text, @topic, @answer)
                """;

            using var cmdFailure = new SqliteCommand(insertFailure, connection, transaction);
            foreach (var question in missedQuestions)
            {
                cmdFailure.Parameters.Clear();
                cmdFailure.Parameters.AddWithValue("@examId", examId);
                cmdFailure.Parameters.AddWithValue("@num", question.QuestionNumber);
                cmdFailure.Parameters.AddWithValue("@text", question.Text ?? "");
                cmdFailure.Parameters.AddWithValue("@topic", question.Topic);
                cmdFailure.Parameters.AddWithValue("@answer", question.CorrectAnswer.ToString());
                cmdFailure.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public List<ExamResult> GetExamHistory(int limit = 50)
    {
        var results = new List<ExamResult>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var cmd = new SqliteCommand("SELECT * FROM exams ORDER BY date DESC LIMIT @limit", connection);
        cmd.Parameters.AddWithValue("@limit", limit);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            results.Add(new ExamResult
            {
                Id = reader.GetInt32(0),
                Date = DateTime.Parse(reader.GetString(1)),
                Topic = reader.IsDBNull(2) ? null : reader.GetString(2),
                TotalQuestions = reader.GetInt32(3),
                Correct = reader.GetInt32(4),
                Wrong = reader.GetInt32(5),
                Skipped = reader.GetInt32(6),
                Score = reader.GetDouble(7),
                RawScore = reader.GetDouble(8),
                PenaltyMode = reader.GetString(9),
                TimeSeconds = reader.GetInt32(10),
                AverageAnswerTime = reader.GetDouble(11)
            });
        }

        return results;
    }

    public List<TopicFailure> GetWeakestTopics()
    {
        var topics = new Dictionary<string, TopicFailure>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var cmd = new SqliteCommand("SELECT topic, COUNT(*) as failures FROM failures GROUP BY topic ORDER BY failures DESC", connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            string topic = reader.GetString(0);
            int failures = reader.GetInt32(1);
            topics[topic] = new TopicFailure { Topic = topic, WrongAnswers = failures };
        }

        using var cmdTotal = new SqliteCommand("SELECT topic, COUNT(*) as total FROM exams WHERE topic IS NOT NULL GROUP BY topic", connection);
        using var readerTotal = cmdTotal.ExecuteReader();
        while (readerTotal.Read())
        {
            string topic = readerTotal.GetString(0);
            int total = readerTotal.GetInt32(1);
            if (topics.ContainsKey(topic))
                topics[topic].TotalAttempts += total;
        }

        return new List<TopicFailure>(topics.Values);
    }

    public List<FrequentlyMissedQuestion> GetMostMissedQuestions(int limit = 10)
    {
        var questions = new List<FrequentlyMissedQuestion>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var cmd = new SqliteCommand(
            "SELECT question_text, topic, correct_answer, COUNT(*) as failures FROM failures GROUP BY question_text, topic ORDER BY failures DESC LIMIT @limit",
            connection);
        cmd.Parameters.AddWithValue("@limit", limit);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            questions.Add(new FrequentlyMissedQuestion
            {
                QuestionText = reader.GetString(0),
                Topic = reader.GetString(1),
                CorrectAnswer = reader.GetString(2)[0],
                TimesMissed = reader.GetInt32(3)
            });
        }

        return questions;
    }

    public (int totalExams, double averageScore, double bestScore, double worstScore) GetGeneralStats()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var cmd = new SqliteCommand(
            "SELECT COUNT(*), AVG(score), MAX(score), MIN(score) FROM exams", connection);
        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return (
                reader.GetInt32(0),
                reader.IsDBNull(1) ? 0 : reader.GetDouble(1),
                reader.IsDBNull(2) ? 0 : reader.GetDouble(2),
                reader.IsDBNull(3) ? 0 : reader.GetDouble(3)
            );
        }

        return (0, 0, 0, 0);
    }

    public List<UniqueMiss> GetUniqueMisses()
    {
        var misses = new List<UniqueMiss>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var cmd = new SqliteCommand(
            "SELECT DISTINCT question_number, question_text, topic, correct_answer FROM failures ORDER BY topic, question_number", connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            misses.Add(new UniqueMiss
            {
                QuestionNumber = reader.GetInt32(0),
                QuestionText = reader.GetString(1),
                Topic = reader.GetString(2),
                CorrectAnswer = reader.GetString(3)[0]
            });
        }

        return misses;
    }

    public int TotalUniqueMisses()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var cmd = new SqliteCommand(
            "SELECT COUNT(DISTINCT question_text) FROM failures", connection);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }
}
