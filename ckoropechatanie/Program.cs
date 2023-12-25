namespace ckoropechatanie
{
    using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace TypingTest
{
    public class User
    {
        public string Name { get; set; }
        public int CharactersPerMinute { get; set; }
        public int CharactersPerSecond { get; set; }
    }

    public static class Scoreboard
    {
        private const string ScoreboardFilePath = "scoreboard.json";
        private static List<User> users = new List<User>();

        public static void LoadScoreboard()
        {
            if (File.Exists(ScoreboardFilePath))
            {
                string jsonData = File.ReadAllText(ScoreboardFilePath);
                users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(jsonData);
            }
        }

        public static void SaveScoreboard()
        {
            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(users);
            File.WriteAllText(ScoreboardFilePath, jsonData);
        }

        public static void AddUser(User user)
        {
            users.Add(user);
            SaveScoreboard();
        }

        public static void DisplayScoreboard()
        {
            Console.WriteLine("Таблица результатов:");
            foreach (var user in users)
            {
                Console.WriteLine($"Имя: {user.Name}, Зн/мин: {user.CharactersPerMinute}, Зн/сек: {user.CharactersPerSecond}");
            }
        }
    }

    public class TypingTest
    {
        private static Stopwatch stopwatch;
        private static Thread timerThread;
        private static string targetText;

        public static void StartTypingTest()
        {
            Console.Write("Введите ваше имя: ");
            string name = Console.ReadLine();

            InitializeTimerAndTargetText();

            Console.WriteLine("Наберите следующий текст:");
            Console.WriteLine(targetText);
            Console.WriteLine("Нажмите Enter, чтобы начать...");

            Console.ReadLine();

            Console.WriteLine("Начните печатать прямо сейчас:");
            Console.ForegroundColor = ConsoleColor.Green;

            stopwatch.Start();

            string typedText = Console.ReadLine();

            stopwatch.Stop();
            Console.ResetColor();

            int charactersTyped = typedText.Length;
            double elapsedMinutes = stopwatch.Elapsed.TotalMinutes;
            double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;

            int charactersPerMinute = (int)(charactersTyped / elapsedMinutes);
            int charactersPerSecond = (int)(charactersTyped / elapsedSeconds);

            User user = new User
            {
                Name = name,
                CharactersPerMinute = charactersPerMinute,
                CharactersPerSecond = charactersPerSecond
            };

            Scoreboard.AddUser(user);

            Console.WriteLine("Тест завершен!");
            Scoreboard.DisplayScoreboard();
        }

        private static void InitializeTimerAndTargetText()
        {
            stopwatch = new Stopwatch();
            timerThread = new Thread(() => StartTimerThread());
            targetText = "Это целевой текст, который вам нужно напечатать точно.";

            timerThread.Start();
        }

        private static void StartTimerThread()
        {
            while (stopwatch.IsRunning)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.WriteLine($"Прошедшее время: {stopwatch.Elapsed:mm\\:ss}");

                Thread.Sleep(100);
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Scoreboard.LoadScoreboard();

            bool repeatTest = true;

            while (repeatTest)
            {
                TypingTest.StartTypingTest();

                Console.WriteLine("Хотите повторить тест? (д/н): ");
                string repeatInput = Console.ReadLine();
                repeatTest = repeatInput.Equals("д", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
}