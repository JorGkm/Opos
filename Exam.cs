using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Opos;

public class Exam
{
    public List<Question>? Questions;
    public Stopwatch Timer;

    public Exam(List<Question> questions)
    {
        Timer = new();
        Questions = questions;
    }
}
