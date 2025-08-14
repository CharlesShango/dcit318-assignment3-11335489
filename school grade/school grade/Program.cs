using System;
using System.Collections.Generic;
using System.IO;

namespace SchoolGradingSystem
{
    // Custom Exceptions
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // Student Class
    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            Score = score;
        }

        public string GetGrade()
        {
            return Score switch
            {
                >= 80 and <= 100 => "A",
                >= 70 and <= 79 => "B",
                >= 60 and <= 69 => "C",
                >= 50 and <= 59 => "D",
                < 50 => "F",
                _ => throw new ArgumentOutOfRangeException($"Invalid score: {Score}")
            };
        }
    }

    // Processor Class
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();
            int lineNumber = 0;

            using (var reader = new StreamReader(inputFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    try
                    {
                        var fields = line.Split(',');
                        if (fields.Length != 3)
                        {
                            throw new MissingFieldException(
                                $"Line {lineNumber}: Expected 3 fields but found {fields.Length}");
                        }

                        if (!int.TryParse(fields[0].Trim(), out int id))
                        {
                            throw new InvalidScoreFormatException(
                                $"Line {lineNumber}: Invalid ID format '{fields[0]}'");
                        }

                        string fullName = fields[1].Trim();
                        if (string.IsNullOrWhiteSpace(fullName))
                        {
                            throw new MissingFieldException(
                                $"Line {lineNumber}: Missing student name");
                        }

                        if (!int.TryParse(fields[2].Trim(), out int score))
                        {
                            throw new InvalidScoreFormatException(
                                $"Line {lineNumber}: Invalid score format '{fields[2]}'");
                        }

                        students.Add(new Student(id, fullName, score));
                    }
                    catch (Exception ex) when (
                        ex is MissingFieldException ||
                        ex is InvalidScoreFormatException)
                    {
                        Console.WriteLine($"Skipping line {lineNumber}: {ex.Message}");
                    }
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (var writer = new StreamWriter(outputFilePath))
            {
                writer.WriteLine("STUDENT GRADE REPORT");
                writer.WriteLine("====================");
                writer.WriteLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine();

                foreach (var student in students)
                {
                    writer.WriteLine(
                        $"{student.FullName} (ID: {student.Id}): " +
                        $"Score = {student.Score}, Grade = {student.GetGrade()}");
                }

                writer.WriteLine();
                writer.WriteLine($"Total students processed: {students.Count}");
            }
        }
    }

    // Main Program
    class Program
    {
        static void Main(string[] args)
        {
            const string inputFile = "student_scores.txt";
            const string outputFile = "grade_report.txt";

            var processor = new StudentResultProcessor();

            try
            {
                Console.WriteLine("Starting grade processing...");

                // Read and validate student data
                var students = processor.ReadStudentsFromFile(inputFile);

                // Generate report
                processor.WriteReportToFile(students, outputFile);

                Console.WriteLine($"Successfully processed {students.Count} students");
                Console.WriteLine($"Report saved to: {Path.GetFullPath(outputFile)}");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Error: Input file not found - {inputFile}");
                Console.WriteLine("Please ensure the file exists and try again.");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Data error: {ex.Message}");
                Console.WriteLine("Please fix the input file format.");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Data error: {ex.Message}");
                Console.WriteLine("Please ensure all scores are numeric values.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                Console.WriteLine("Please contact support.");
            }
        }
    }
}