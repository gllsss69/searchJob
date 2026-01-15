using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

// Запис користувача: Id, Email, PasswordHash, Role
internal record UserRecord(int Id, string Email, string PasswordHash, string Role);

// Запис для заявки: Id, JobId, UserId, ApplicantName, City, Age, Experience, Status
internal record ApplicationRecord(int Id, int JobId, int UserId, string ApplicantName, string City, int Age, string Experience, string Status);

// Запис вакансії: Id, Title, Salary, Vacancies, Competition, ClientId
internal record JobRecord(int Id, string Title, double Salary, int Vacancies, double Competition, int ClientId);

// Запис клієнта/роботодавця: Id, Name, Age, City, Profession
record ClientRecord(int Id, string Name, int Age, string City, string Profession);

internal class Program
{
    private const string JobsFile = "jobs.csv";
    private const string ClientsFile = "clients.csv";
    private const string UsersFile = "users.csv";
    private const string AppsFile = "applications.csv";

    // Шапка вакансій
    private static readonly string JobsHeader = "Id,Title,Salary,Vacancies,Competition,ClientId";

    // Шапка клієнтів/роботодавців
    private static readonly string ClientsHeader = "Id,Name,Age,City,Profession";

    // Шапка користувачів
    private static readonly string UsersHeader = "Id,Email,PasswordHash,Role";

    // Шапка заявок
    private static readonly string AppsHeader = "Id,JobId,UserId,ApplicantName,City,Age,Experience,Status";

    // Зберігаємо поточного користувача
    private static UserRecord? currentUser = null;

    // Точка входу програми: ініціалізація файлів, створення дефолтного admin, запуск логіну та меню
    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        EnsureFileWithHeader(JobsFile, JobsHeader); // Створення файлу вакансій
        EnsureFileWithHeader(ClientsFile, ClientsHeader); // Створення файлу клієнтів/ротодавців
        EnsureFileWithHeader(UsersFile, UsersHeader); // Створення файлу користувачів
        EnsureFileWithHeader(AppsFile, AppsHeader); // Створення файлу заявок

        var users = ReadAllUsers();
        if (users.Count == 0)
        {
            // Створення дефолтного адміна
            var adminId = GetNextIdFromFile(UsersFile);
            var admin = new UserRecord(adminId, "admin", ComputeHash("1234"), "admin");

            // Записуємо 4 поля
            AppendLineToFile(UsersFile, ToCsv(admin.Id.ToString(), admin.Email, admin.PasswordHash, admin.Role));
        }

        try
        {
            if (!LoginSystem())
            {
                Console.WriteLine("Вихід.");
                return;
            }

            // Розподіл меню за роллю
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (currentUser.Role == "admin")
            {
                ShowAdminMenu();
            }
            else
            {
                ShowClientMenu();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Несподівана помилка: {ex.Message}");
            Console.ResetColor();
        }
    }

    // Зчитує вибір меню як ціле число в межах (min,max)
    static int ReadMenuChoice(string prompt, int min, int max)
    {
        while (true)
        {
            try
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (s == null)
                {
                    Console.WriteLine("Введення скасовано.");
                    return min;
                }

                if (int.TryParse(s.Trim(), out var choice) && choice >= min && choice <= max)
                {
                    return choice;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Введіть число від {min} до {max}.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Помилка вводу: {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    // Зчитує ціле число в діапазоні або повертає min при скасуванні
    static int ReadIntInRange(string prompt, int min, int max)
    {
        while (true)
        {
            try
            {
                string rangeText;
                if (max == int.MaxValue)
                {
                    rangeText = (min == 0) ? "(0 - назад)" : $"(≥{min})";
                }
                else
                {
                    rangeText = $"({min}-{max})";
                }

                Console.Write($"{prompt} {rangeText}: ");
                var s = Console.ReadLine();
                if (s == null)
                {
                    Console.WriteLine("Введення скасовано.");
                    return min;
                }

                if (int.TryParse(s.Trim(), out int v) && v >= min && v <= max)
                {
                    return v;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(max == int.MaxValue
                    ? $"Введіть ціле число {(min == 0 ? "або 0 для відміни" : $"≥ {min}")}."
                    : $"Введіть ціле число від {min} до {max}.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Помилка вводу: {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    // Зчитує число з плаваючою точкою в межах (min,max) або повертає min при скасуванні
    static double ReadDoubleInRange(string prompt, double min, double max)
    {
        while (true)
        {
            try
            {
                string rangeText;
                if (double.IsPositiveInfinity(max) || max == double.MaxValue)
                {
                    rangeText = (min == 0) ? "(0 - назад)" : $"(≥{min})";
                }
                else
                {
                    rangeText = $"({min}-{max})";
                }

                Console.Write($"{prompt} {rangeText}: ");
                var s = Console.ReadLine();
                if (s == null)
                {
                    Console.WriteLine("Введення скасовано.");
                    return min;
                }

                if (double.TryParse(s.Trim(), out double v) && v >= min && v <= max)
                {
                    return v;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(double.IsPositiveInfinity(max) || max == double.MaxValue
                    ? $"Введіть число >= {min}."
                    : $"Введіть число від {min} до {max}.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Помилка вводу: {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    // Читає непорожній рядок, обмежений maxLen символів
    static string ReadNonEmpty(string prompt, int maxLen = 500)
    {
        while (true)
        {
            try
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(s) && s.Trim().Length <= maxLen)
                {
                    return s.Trim();
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Поле не може бути порожнім чи надто довгим.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Помилка вводу: {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    // Зчитує ім'я (без цифр) і перевіряє довжину
    static string ReadName(string prompt, int maxLen = 100)
    {
        while (true)
        {
            var s = ReadNonEmpty(prompt, maxLen);
            bool hasDigit = false;
            foreach (var ch in s)
            {
                if (char.IsDigit(ch))
                {
                    hasDigit = true;
                    break;
                }
            }

            if (!hasDigit)
            {
                return s;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Ім'я не може містити цифри. Спробуйте ще раз.");
            Console.ResetColor();
        }
    }

    // --- Система логіну ---

    // Меню входу: вхід, реєстрація або вихід
    static bool LoginSystem()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\n--- Вхід у систему ---");
            Console.ResetColor();
            Console.WriteLine("1. Увійти");
            Console.WriteLine("2. Зареєструватися");
            Console.WriteLine("0. Вихід");
            int k = ReadMenuChoice("Виберіть дію (0-2): ", 0, 2);
            if (k == 0)
            {
                return false;
            }

            if (k == 2)
            {
                SafeExecute(RegisterUser);
                continue;
            }

            if (k == 1)
            {
                try
                {
                    string email = ReadNonEmpty("Email: ", 32).ToLower();
                    var pass = ReadPasswordNonEmpty("Пароль: ", 12);

                    var user = Authenticate(email, pass);
                    if (user != null)
                    {
                        currentUser = user; // Зберігаємо хто зайшов
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Вхід виконано!");
                        Console.ResetColor();
                        return true;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nНевірні дані. Спробуйте ще.");
                        Console.ResetColor();
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Помилка при вході: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }
    }

    // Реєстрація нового користувача з вибором ролі
    static void RegisterUser()
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\n--- Реєстрація ---");
            Console.ResetColor();
            string email = ReadNonEmpty("Email: ", 32).ToLower();

            var users = ReadAllUsers();
            bool exists = false;
            foreach (var uu in users)
            {
                if (uu.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Користувач вже існує.");
                Console.ResetColor();
                return;
            }

            var pass = ReadPasswordNonEmpty("Пароль: ", 12);

            // Вибір ролі (1 або 9)
            int rKey;
            while (true)
            {
                Console.Write("Тип акаунту (1 - Клієнт, 9 - Адміністратор): ");
                var s = Console.ReadLine();
                if (s == "9")
                {
                    rKey = 9;
                    break;
                }

                if (s == "1")
                {
                    rKey = 1;
                    break;
                }

                Console.WriteLine("Введіть 1 або 9.");
            }

            string role = (rKey == 9) ? "admin" : "client";

            var id = GetNextIdFromFile(UsersFile);
            var hash = ComputeHash(pass);

            AppendLineToFile(UsersFile, ToCsv(id.ToString(), email, hash, role));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Реєстрацію завершено ({role}).");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Помилка реєстрації: {ex.Message}");
            Console.ResetColor();
        }
    }

    // Аутентифікація: перевіряє email та пароль (хеш)
    static UserRecord? Authenticate(string email, string password)
    {
        try
        {
            var users = ReadAllUsers();
            var hash = ComputeHash(password);
            foreach (var u in users)
            {
                if (u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && u.PasswordHash == hash)
                {
                    return u;
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    // Читає всіх користувачів з файлу users.csv
    static List<UserRecord> ReadAllUsers()
    {
        var res = new List<UserRecord>();
        try
        {
            using var sr = new StreamReader(UsersFile, Encoding.UTF8);
            sr.ReadLine(); // header
            string line;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                try
                {
                    var f = ParseCsvLine(line);
                    if (f.Count < 3)
                    {
                        continue;
                    }

                    if (!int.TryParse(f[0], out int id))
                    {
                        continue;
                    }

                    // Читаємо роль. Якщо це старий файл (3 колонки), ставимо default
                    string role = "client";
                    if (f.Count >= 4)
                    {
                        role = f[3];
                    }
                    else if (f[1] == "admin")
                    {
                        role = "admin"; // Хак для старого адміна
                    }

                    res.Add(new UserRecord(id, f[1], f[2], role));
                }
                catch
                {
                    continue;
                }
            }
        }
        catch
        {
        }

        return res;
    }

    // --- НОВЕ МЕНЮ КЛІЄНТА ---

    // Відображає меню для клієнта (звичайного користувача)
    static void ShowClientMenu()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Console.WriteLine($"\n--- Головне меню ({currentUser.Email}) ---");
            Console.ResetColor();
            Console.WriteLine("1. Переглянути вакансії");
            Console.WriteLine("2. Мої заявки");
            Console.WriteLine("3. Калькулятор шансів");
            Console.WriteLine("4. Пошук");
            Console.WriteLine("5. Фільтрація вакансій"); // фільтрація додана
            Console.WriteLine("0. Вихід");

            int choice = ReadMenuChoice("Виберіть дію: ", 0, 5);
            switch (choice)
            {
                case 1: SafeExecute(ClientApplyForJob); break;
                case 2: SafeExecute(ClientCheckMyApps); break;
                case 3: SafeExecute(ShowСalculateJobMenu); break; // Ваша стара функція
                case 4: SafeExecute(SearchElementCsv); break;
                case 5: SafeExecute(ShowFilterMenu); break;
                case 0: return;
                default: Console.WriteLine("Невірний вибір."); break;
            }
        }
    }

    // Показує випадкові вакансії та дозволяє подати заявку (включає ім'я роботодавця)
    static void ClientApplyForJob()
    {
        try
        {
            var jobs = ReadAllJobs();
            if (jobs.Count == 0)
            {
                Console.WriteLine("Вакансій немає.");
                return;
            }

            // Побудова словника роботодавців
            var clientsList = ReadAllEmployers();
            var clients = new Dictionary<int, ClientRecord>();
            foreach (var c in clientsList)
            {
                if (!clients.ContainsKey(c.Id))
                {
                    clients[c.Id] = c;
                }
            }

            // Випадкові 5 вакансій - без LINQ: shuffle + take first N
            var rnd = new Random();
            var rndJobs = new List<JobRecord>(jobs);
            Shuffle(rndJobs, rnd);
            int take = Math.Min(5, rndJobs.Count);

            Console.WriteLine("\n--- Список вакансій ---");
            Console.WriteLine("{0,-4} {1,-22} {2,10} {3,10} {4,-20}", "Id", "Професія", "Зарплата", "Вакансій", "Роботодавець");
            for (int i = 0; i < take; i++)
            {
                var j = rndJobs[i];
                string employer = clients.ContainsKey(j.ClientId) ? clients[j.ClientId].Name : "-";
                Console.WriteLine("{0,-4} {1,-22} {2,10:F0} {3,10} {4,-20}", j.Id, Truncate(j.Title, 22), j.Salary, j.Vacancies, Truncate(employer, 20));
            }

            int jId = ReadIntInRange("\nВведіть ID вакансії для подачі", 0, int.MaxValue);
            if (jId == 0)
            {
                return;
            }

            JobRecord job = null;
            foreach (var jj in jobs)
            {
                if (jj.Id == jId)
                {
                    job = jj;
                    break;
                }
            }

            if (job == null)
            {
                Console.WriteLine("Вакансію не знайдено.");
                return;
            }

            Console.WriteLine($"\nПодача заявки на: {job.Title}");
            string name = ReadName("ПІБ: ", 50);
            string city = ReadNonEmpty("Місто: ", 30);
            int age = ReadIntInRange("Вік", 18, 100);
            string exp = ReadNonEmpty("Досвід: ", 500);

            int appId = GetNextIdFromFile(AppsFile);
            var app = new ApplicationRecord(appId, job.Id, currentUser.Id, name, city, age, exp, "Очікує");

            AppendLineToFile(AppsFile, ToCsv(
                app.Id.ToString(),
                app.JobId.ToString(),
                app.UserId.ToString(),
                app.ApplicantName,
                app.City,
                app.Age.ToString(),
                app.Experience,
                app.Status));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Заявку надіслано! Очікуйте рішення.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Помилка під час подачі заявки: {ex.Message}");
            Console.ResetColor();
        }
    }

    // Показує заявки поточного користувача та їх статуси
    static void ClientCheckMyApps()
    {
        SafeExecute(() =>
        {
            var apps = ReadAllApplications();
            var myApps = new List<ApplicationRecord>();
            foreach (var a in apps)
            {
                if (a.UserId == currentUser.Id)
                {
                    myApps.Add(a);
                }
            }

            var jobs = ReadAllJobs();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n--- Мої заявки ---");
            Console.ResetColor();
            if (myApps.Count == 0)
            {
                Console.WriteLine("Заявок немає.");
                return;
            }

            foreach (var a in myApps)
            {
                string jName = "Невідома";
                foreach (var j in jobs)
                {
                    if (j.Id == a.JobId)
                    {
                        jName = j.Title;

                        break;
                    }
                }

                Console.Write($"Вакансія: {jName} | Статус: ");
                if (a.Status == "Прийнято")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if (a.Status == "Відхилено")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }

                Console.WriteLine(a.Status);
                Console.ResetColor();
            }
        });
    }

    // --- Меню адміна ---

    // Показує меню адміністратора
    static void ShowAdminMenu()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n--- Меню Адміністратора ---");
            Console.ResetColor();
            Console.WriteLine("1. Розглянути нові заявки");
            Console.WriteLine("2. Керування базою даних");
            Console.WriteLine("0. Вихід");

            int input = ReadMenuChoice("Виберіть дію: ", 0, 2);
            switch (input)
            {
                case 1: SafeExecute(AdminReviewApps); break;
                case 2: SafeExecute(ShowMainMenu); break; // Ваше старе головне меню тепер тут
                case 0: return;
            }
        }
    }

    // Адмін може переглядати нові заявки та приймати/відхиляти їх
    static void AdminReviewApps()
    {
        try
        {
            var apps = ReadAllApplications();
            var pending = new List<ApplicationRecord>();
            foreach (var a in apps)
            {
                if (a.Status == "Очікує")
                {
                    pending.Add(a);
                }
            }

            var jobs = ReadAllJobs();

            if (pending.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nНових заявок немає.");
                return;
            }

            Console.WriteLine("\n--- Заявки на розгляді ---");
            foreach (var a in pending)
            {
                string jobTitle = "Невідома";
                string jobSalary = "-";
                foreach (var j in jobs)
                {
                    if (j.Id == a.JobId)
                    {
                        jobTitle = j.Title;
                        jobSalary = j.Salary.ToString("F0"); // форматування зарплати без десяткових знаків
                        break;
                    }
                }

                Console.WriteLine($"ID: {a.Id} | Кандидат: {a.ApplicantName} ({a.Age} р., {a.City})");
                Console.WriteLine($"   -> Вакансія: {jobTitle}, Зарплата: {jobSalary}, Досвід: {a.Experience}");
                Console.WriteLine(new string('-', 40));
            }

            int id = ReadIntInRange("Введіть ID заявки для рішення(0 - вихід)", 0, 50);
            if (id == 0)
            {
                return;
            }

            ApplicationRecord target = null;
            foreach (var a in apps)
            {
                if (a.Id == id && a.Status == "Очікує")
                {
                    target = a;
                    break;
                }
            }

            if (target == null)
            {
                Console.WriteLine("Не знайдено.");
                return;
            }

            Console.WriteLine("1.Прийняти  2.Відхилити");
            int dec = ReadMenuChoice("Виберіть дію: ", 1, 2);
            if (dec == 1)
            {
                target = target with { Status = "Прийнято" };
            }
            else
            {
                target = target with { Status = "Відхилено" };
            }

            int idx = -1;
            for (int i = 0; i < apps.Count; i++)
            {
                if (apps[i].Id == target.Id)
                {
                    idx = i;
                    break;
                }
            }

            if (idx >= 0)
            {
                apps[idx] = target;
            }

            var lines = new List<string> { AppsHeader };
            foreach (var a in apps)
            {
                lines.Add(ToCsv(
                    a.Id.ToString(),
                    a.JobId.ToString(),
                    a.UserId.ToString(),
                    a.ApplicantName,
                    a.City,
                    a.Age.ToString(),
                    a.Experience,
                    a.Status));
            }

            WriteAllLines(AppsFile, lines);
            Console.WriteLine("Статус оновлено.");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Помилка при розгляді заявок: {ex.Message}");
            Console.ResetColor();
        }
    }

    // Зчитує всі заявки з applications.csv
    static List<ApplicationRecord> ReadAllApplications()
    {
        var res = new List<ApplicationRecord>();
        try
        {
            using var sr = new StreamReader(AppsFile, Encoding.UTF8);
            sr.ReadLine();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var f = ParseCsvLine(line);
                if (f.Count < 8)
                {
                    continue;
                }

                if (int.TryParse(f[0], out int id) && int.TryParse(f[1], out int jId) && int.TryParse(f[2], out int uId))
                {
                    int.TryParse(f[5], out int age);
                    res.Add(new ApplicationRecord(id, jId, uId, f[3], f[4], age, f[6], f[7]));
                }
            }
        }
        catch
        {
        }

        return res;
    }

    // Гарантує наявність файлу з шапкою, якщо шапка відрізняється — переписує файл з новою шапкою
    static void EnsureFileWithHeader(string path, string header)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, header + Environment.NewLine, new UTF8Encoding(false));
            return;
        }

        try
        {
            using var sr = new StreamReader(path, Encoding.UTF8);
            string first = sr.ReadLine();
            if (first == null || !first.Trim().Equals(header, StringComparison.OrdinalIgnoreCase))
            {
                var rest = new List<string>();
                string line;
                if (first != null)
                {
                    rest.Add(first);
                }

                while ((line = sr.ReadLine()) != null)
                {
                    rest.Add(line);
                }

                using var sw = new StreamWriter(path, false, new UTF8Encoding(false));
                sw.WriteLine(header);
                foreach (var r in rest)
                {
                    if (!string.IsNullOrWhiteSpace(r))
                    {
                        sw.WriteLine(r);
                    }
                }
            }
        }
        catch
        {
            File.WriteAllText(path, header + Environment.NewLine, new UTF8Encoding(false));
        }
    }

    // Додає рядок в кінець файлу
    static void AppendLineToFile(string path, string line)
    {
        try
        {
            using var sw = new StreamWriter(path, true, new UTF8Encoding(false));
            sw.WriteLine(line);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
    }

    // Записує всі рядки у файл (перезапис)
    static void WriteAllLines(string path, IEnumerable<string> lines)
    {
        try
        {
            using var sw = new StreamWriter(path, false, new UTF8Encoding(false));
            foreach (var line in lines)
            {
                sw.WriteLine(line);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
    }

    // Екранування поля для CSV (додає лапки та дублює внутрішні лапки)
    static string EscapeCsv(string field)
    {
        if (field == null)
        {
            return string.Empty;
        }

        if (field.Contains('"'))
        {
            field = field.Replace("\"", "\"\"");
        }

        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            return $"\"{field}\"";
        }

        return field;
    }

    // Простий парсер CSV-рядка
    static List<string> ParseCsvLine(string line)
    {
        var res = new List<string>();
        if (line == null)
        {
            return res;
        }

        int i = 0;
        int n = line.Length;
        while (i < n)
        {
            if (line[i] == '"')
            {
                i++;
                var sb = new StringBuilder();
                while (i < n)
                {
                    if (line[i] == '"')
                    {
                        if (i + 1 < n && line[i + 1] == '"')
                        {
                            sb.Append('"');
                            i += 2;
                        }
                        else
                        {
                            i++;
                            break;
                        }
                    }
                    else
                    {
                        sb.Append(line[i]);
                        i++;
                    }
                }

                while (i < n && line[i] != ',')
                {
                    i++;
                }

                if (i < n && line[i] == ',')
                {
                    i++;
                }

                res.Add(sb.ToString());
            }
            else
            {
                var start = i;
                while (i < n && line[i] != ',')
                {
                    i++;
                }

                res.Add(line.Substring(start, i - start));
                if (i < n && line[i] == ',')
                {
                    i++;
                }
            }
        }

        return res;
    }

    // Збирає масив полів у CSV-рядок
    static string ToCsv(params string[] fields)
    {
        var parts = new List<string>(fields.Length);
        foreach (var f in fields)
        {
            parts.Add(EscapeCsv(f));
        }

        return string.Join(",", parts);
    }

    // Повертає наступний доступний Id на основі першої колонки файлу
    static int GetNextIdFromFile(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return 1;
            }

            using var sr = new StreamReader(path, Encoding.UTF8);
            sr.ReadLine();
            int max = 0;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var fields = ParseCsvLine(line);
                if (fields.Count > 0 && int.TryParse(fields[0], out int id))
                {
                    if (id > max)
                    {
                        max = id;
                    }
                }
            }

            return max + 1;
        }
        catch
        {
            return 1;
        }
    }

    // Обчислює SHA256 хеш рядка і повертає у HEX
    static string ComputeHash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        return Convert.ToHexString(sha.ComputeHash(bytes));
    }

    // Читання пароля з консолі з маскуванням
    static string ReadPassword()
    {
        var pass = new StringBuilder();
        ConsoleKeyInfo key;
        while (true)
        {
            key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                break;
            }

            if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
            {
                pass.Remove(pass.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                pass.Append(key.KeyChar);
                Console.Write('*');
            }
        }

        Console.WriteLine();
        return pass.ToString();
    }

    // Читання пароля з консолі з маскуванням
    static string ReadPasswordNonEmpty(string prompt, int maxLen = 100)
    {
        while (true)
        {
            try
            {
                Console.Write(prompt);
                var pass = ReadPassword();
                if (!string.IsNullOrWhiteSpace(pass) && pass.Length <= maxLen)
                {
                    return pass;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Поле не може бути порожнім чи надто довгим.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Помилка вводу: {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    // Зчитує всі вакансії з jobs.csv
    static List<JobRecord> ReadAllJobs()
    {
        var res = new List<JobRecord>();
        try
        {
            using var sr = new StreamReader(JobsFile, Encoding.UTF8);
            sr.ReadLine();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                try
                {
                    var f = ParseCsvLine(line);
                    if (f.Count < 6)
                    {
                        continue;
                    }

                    if (!int.TryParse(f[0], out int id))
                    {
                        continue;
                    }

                    var title = f[1];
                    if (!double.TryParse(f[2], out double salary))
                    {
                        continue;
                    }

                    if (!int.TryParse(f[3], out int vac))
                    {
                        continue;
                    }

                    if (!double.TryParse(f[4], out double comp))
                    {
                        continue;
                    }

                    if (!int.TryParse(f[5], out int clientId))
                    {
                        clientId = 0;
                    }

                    res.Add(new JobRecord(id, title, salary, vac, comp, clientId));
                }
                catch
                {
                    continue;
                }
            }
        }
        catch
        {
        }

        return res;
    }

    // Запис всіх вакансій у файл (перезапис)
    static void WriteAllJobs(List<JobRecord> jobs)
    {
        var lines = new List<string> { JobsHeader };
        foreach (var j in jobs)
        {
            lines.Add(ToCsv(j.Id.ToString(), j.Title, j.Salary.ToString(), j.Vacancies.ToString(), j.Competition.ToString(), j.ClientId.ToString()));
        }

        WriteAllLines(JobsFile, lines);
    }

    // Додає одну вакансію у файл jobs.csv
    static void AppendJob(JobRecord job)
    {
        AppendLineToFile(JobsFile, ToCsv(job.Id.ToString(), job.Title, job.Salary.ToString(), job.Vacancies.ToString(), job.Competition.ToString(), job.ClientId.ToString()));
    }

    // Зчитує список роботодавців з clients.csv
    static List<ClientRecord> ReadAllEmployers()
    {
        var res = new List<ClientRecord>();
        try
        {
            using var sr = new StreamReader(ClientsFile, Encoding.UTF8);
            sr.ReadLine();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var f = ParseCsvLine(line);
                if (f.Count < 5)
                {
                    continue;
                }

                if (!int.TryParse(f[0], out int id))
                {
                    continue;
                }

                int.TryParse(f[2], out int age);
                res.Add(new ClientRecord(id, f[1], age, f[3], f[4]));
            }
        }
        catch
        {
        }

        return res;
    }

    // Додає клієнта/роботодавця у файл clients.csv
    static void AppendClient(ClientRecord client)
    {
        AppendLineToFile(ClientsFile, ToCsv(client.Id.ToString(), client.Name, client.Age.ToString(), client.City, client.Profession));
    }

    // Головне меню керування базою для адміна
    static void ShowMainMenu()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n--- Керування базою ---");
            Console.ResetColor();
            Console.WriteLine("1. Калькулятор професій");
            Console.WriteLine("2. Налаштування");
            Console.WriteLine("3. Ввести нові вакансії");
            Console.WriteLine("4. Статистика");
            Console.WriteLine("5. Таблиця вакансій");
            Console.WriteLine("6. Редагувати записи");
            Console.WriteLine("0. Назад");

            int choice = ReadMenuChoice("\nВиберіть дію: ", 0, 6);
            switch (choice)
            {
                case 0: return;
                case 1: ShowСalculateJobMenu(); break;
                case 2: Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("Функція в розробці."); Console.ResetColor(); break;
                case 3: SafeExecute(EnterDataCsv); break;
                case 4: SafeExecute(ShowStatisticsCsv); break;
                case 5: SafeExecute(PrintAllAsTableCsv); break;
                case 6: SafeExecute(ManageCollectionMenuCsv); break;
            }
        }
    }

    // Меню калькулятора шансів
    static void ShowСalculateJobMenu()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n--- Калькулятор шансів ---");
            Console.ResetColor();
            var professions = new[] { "Програміст", "Дизайнер", "Менеджер", "Інженер", "Маркетолог", "Водій", "Електрик", "Лікар", "Вчитель" };
            for (int i = 0; i < professions.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {professions[i]}");
            }

            Console.WriteLine("0. Назад");
            int choice = ReadMenuChoice("Виберіть: ", 0, professions.Length);
            if (choice == 0)
            {
                return;
            }

            CalculateProfession(professions[choice - 1]);
        }
    }

    // Розрахунок рейтингу шансів для професії (калькулятор)
    static void CalculateProfession(string profession)
    {
        try
        {
            Console.WriteLine($"\nПрофесія: {profession}");
            double vacancies = ReadDoubleInRange("Кількість вакансій", 0, 100000);
            double avgSalary = ReadDoubleInRange("Середня зарплата", 0, 1000000);
            double competition = ReadDoubleInRange("Конкуренція", 0.0001, 100000);
            if (competition <= 0)
            {
                competition = 1;
            }

            double score = Math.Sqrt((avgSalary * vacancies) / competition);
            double rating = Math.Round(100.0 * (score / (score + 100)), 2);

            Console.WriteLine($"Рейтинг шансів: {rating}");
            if (rating > 50)
            {
                Console.WriteLine("Високі шанси!");
            }
            else
            {
                Console.WriteLine("Низькі шанси.");
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Помилка розрахунку: {ex.Message}");
            Console.ResetColor();
        }
    }

    // Додавання нових записів
    static void EnterDataCsv()
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n--- Додавання записів ---");
            Console.ResetColor();
            int count = ReadIntInRange("Кількість записів", 1, 50);

            for (int i = 0; i < count; i++)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Запис {i + 1}");
                Console.ResetColor();
                string title = ReadNonEmpty("Професія: ", 50);
                double salary = ReadDoubleInRange("Зарплата", 0, 1_000_000);
                int vacancies = ReadIntInRange("Вакансії", 0, 10000);
                double competition = ReadDoubleInRange("Конкуренція", 0.0, 1_000_000);

                string clientName = ReadName("Ім'я роботодавця: ", 50);
                int age = ReadIntInRange("Вік", 18, 100);
                string city = ReadNonEmpty("Місто: ", 50);

                int clientId = GetNextIdFromFile(ClientsFile);
                AppendClient(new ClientRecord(clientId, clientName, age, city, title));

                int jobId = GetNextIdFromFile(JobsFile);
                AppendJob(new JobRecord(jobId, title, salary, vacancies, competition, clientId));
                Console.WriteLine("Запис(и) додано.");
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Помилка при додаванні записів: {ex.Message}");
            Console.ResetColor();
        }
    }

    // Вивід таблиці вакансій з колонкою роботодавця
    static void PrintAllAsTableCsv()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\n--- Таблиця вакансій ---");
        Console.ResetColor();
        try
        {
            var jobs = ReadAllJobs();
            var clientsList = ReadAllEmployers();
            var clients = new Dictionary<int, ClientRecord>();
            foreach (var c in clientsList)
            {
                if (!clients.ContainsKey(c.Id))
                {
                    clients[c.Id] = c;
                }
            }

            Console.WriteLine();
            Console.WriteLine("{0,-4} {1,-20} {2,12} {3,10} {4,-20}", "Id", "Професія", "Зарплата", "Вакансій", "Роботодавець");
            jobs.Sort((a, b) => a.Id.CompareTo(b.Id));
            foreach (var j in jobs)
            {
                string cInfo = clients.ContainsKey(j.ClientId) ? clients[j.ClientId].Name : "-";
                Console.WriteLine("{0,-4} {1,-20} {2,12:F0} {3,10} {4,-20}", j.Id, Truncate(j.Title, 20), j.Salary, j.Vacancies, Truncate(cInfo, 20));
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Помилка при виводі таблиці: {ex.Message}");
            Console.ResetColor();
        }
    }

    // Меню редагування колекції вакансій
    static void ManageCollectionMenuCsv()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n--- Редагування ---");
            Console.ResetColor();
            Console.WriteLine("1. Додати  2. Показати  3. Пошук  4. Видалити  5. Редагувати  6. Сортувати  7. Фільтрація  0. Назад");
            int choice = ReadMenuChoice("\nВиберіть дію: ", 0, 7);
            switch (choice)
            {
                case 0: return;
                case 1: SafeExecute(EnterDataCsv); break;
                case 2: SafeExecute(PrintAllAsTableCsv); break;
                case 3: SafeExecute(SearchElementCsv); break;
                case 4: SafeExecute(DeleteElementCsv); break;
                case 5: SafeExecute(EditElementCsv); break;
                case 6: SafeExecute(SortAndShowCsv); break;
                case 7: SafeExecute(ShowFilterMenu); break;
            }
        }
    }

    // Пошук вакансії за ID або назвою
    static void SearchElementCsv()
    {
        Console.WriteLine("1. За ID  2. За назвою");
        int sel = ReadMenuChoice("Виберіть: ", 1, 2);
        if (sel == 1)
        {
            int id = ReadIntInRange("ID", 1, int.MaxValue);
            JobRecord found = null;
            foreach (var x in ReadAllJobs())
            {
                if (x.Id == id)
            {
                found = x;
                break;
                }
            }

            if (found != null)
            {
                PrintSingleCsv(found);
            }
            else
            {
                Console.WriteLine("Немає.");
            }
        }
        else
        {
            string p = ReadNonEmpty("Назва: ");
            var list = new List<JobRecord>();
            foreach (var x in ReadAllJobs())
            {
                if (!string.IsNullOrEmpty(p) && x.Title.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    list.Add(x);
                }
            }

            foreach (var j in list)
            {
                PrintSingleCsv(j);
            }
        }
    }

    // Друкує одну вакансію в короткому форматі
    static void PrintSingleCsv(JobRecord j)
    {
        // Find employer name
        var clients = ReadAllEmployers();
        string employer = "-";
        foreach (var c in clients)
        {
            if (c.Id == j.ClientId)
            {
                employer = c.Name;
                break;
            }
        }

        // Print job with employer (truncate employer similar to other outputs)
        Console.WriteLine($"#{j.Id} {Truncate(j.Title, 40)} Зарплата:{j.Salary:F0} Вакансій:{j.Vacancies} Роботодавець:{Truncate(employer, 20)}");
    }

    // Видалення вакансії за ID
    static void DeleteElementCsv()
    {
        int id = ReadIntInRange("ID для видалення", 1, int.MaxValue);
        var jobs = ReadAllJobs();
        if (jobs.RemoveAll(j => j.Id == id) > 0)
        {
            WriteAllJobs(jobs);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Запис видалено.");
        }
        else
        {
            Console.WriteLine("Запис не знайдено або його не існує.");
        }

        Console.ResetColor();
    }

    // Редагування вакансії за ID
    static void EditElementCsv()
    {
        int id = ReadIntInRange("ID для редагування: ", 1, int.MaxValue);
        var jobs = ReadAllJobs();
        var idx = jobs.FindIndex(j => j.Id == id);
        if (idx == -1)
        {
            Console.WriteLine("Запис не знайдено або його не існує.");
            return;
        }

        var old = jobs[idx];
        Console.Write($"Назва ({old.Title}): ");
        string t = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(t))
        {
            t = old.Title;
        }

        double sal = ReadDoubleInRange($"Зарплата ({old.Salary}): ", 0, 1_000_000);
        if (sal == 0)
        {
            sal = old.Salary;
        }

        int vac = ReadIntInRange($"Вакансії ({old.Vacancies}): ", 0, 100000);
        if (vac == 0)
        {
            vac = old.Vacancies;
        }

        jobs[idx] = new JobRecord(old.Id, t, sal, vac, old.Competition, old.ClientId);
        WriteAllJobs(jobs);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Запис оновлено.");
        Console.ResetColor();
    }

    // Сортування вакансій та вивід
    static void SortAndShowCsv()
    {
        var jobs = ReadAllJobs();
        Console.WriteLine("1. За назвою  2. За зарплатою");
        int c = ReadMenuChoice("Виберіть: ", 1, 2);
        if (c == 2)
        {
            jobs.Sort((a, b) => a.Salary.CompareTo(b.Salary));
        }
        else
        {
            jobs.Sort((a, b) => string.Compare(a.Title, b.Title));
        }

        foreach (var j in jobs)
        {
            Console.WriteLine($"{j.Title} - {j.Salary}");
        }
    }

    // Показує базову статистику по вакансіях
    static void ShowStatisticsCsv()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\n--- Статистика ---");
        Console.ResetColor();
        try
        {
            var jobs = ReadAllJobs();
            if (jobs.Count == 0)
            {
                Console.WriteLine("Немає даних.");
                return;
            }

            double totalVac = 0;
            double totalSalary = 0;
            for (int i = 0; i < jobs.Count; i++)
            {
                totalVac += jobs[i].Vacancies;
                totalSalary += jobs[i].Salary;
            }

            double avgSalary = jobs.Count > 0 ? totalSalary / jobs.Count : 0;

            Console.WriteLine($"Всього вакансій: {totalVac}");
            Console.WriteLine($"Середня зарплата: {avgSalary:F2}");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Помилка статистики: {ex.Message}");
            Console.ResetColor();
        }
    }

    // Обрізає рядок до довжини max (додає ...)
    static string Truncate(string s, int max)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }

        return s.Length <= max ? s : s.Substring(0, max - 3) + "...";
    }

    // Перемішує список (Fisher-Yates)
    static void Shuffle<T>(List<T> list, Random rnd)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            var tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }

    // Utility wrapper to catch and show unexpected exceptions per action
    static void SafeExecute(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Помилка: {ex.Message}");
            Console.ResetColor();
        }
    }

    // --- Спільне меню фільтрації (ідентичне для клієнта та адміна) ---

    // Виводить список вакансій з інформацією про роботодавця
    static void PrintJobsList(IEnumerable<JobRecord> jobs)
    {
        var list = new List<JobRecord>(jobs);
        if (list.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Результатів немає.");
            Console.ResetColor();
            return;
        }

        var employers = ReadAllEmployers();
        var map = new Dictionary<int, ClientRecord>();
        foreach (var e in employers)
        {
            if (!map.ContainsKey(e.Id))
            {
                map[e.Id] = e;
            }
        }

        Console.WriteLine();
        Console.WriteLine("{0,-4} {1,-22} {2,10} {3,10} {4,-20}", "Id", "Професія", "Зарплата", "Вакансій", "Роботодавець");
        list.Sort((a, b) => a.Id.CompareTo(b.Id));
        foreach (var j in list)
        {
            string em = map.ContainsKey(j.ClientId) ? map[j.ClientId].Name : "-";
            Console.WriteLine("{0,-4} {1,-22} {2,10:F0} {3,10} {4,-20}", j.Id, Truncate(j.Title, 22), j.Salary, j.Vacancies, Truncate(em, 20));
        }
    }

    // Ідентичне меню фільтрації для клієнта й адміна
    static void ShowFilterMenu()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n--- Фільтрація вакансій ---");
            Console.ResetColor();
            Console.WriteLine("1. За назвою");
            Console.WriteLine("2. За мінімальною зарплатою");
            Console.WriteLine("3. За мінімальною кількістю вакансій");
            Console.WriteLine("0. Назад");

            int choice = ReadMenuChoice("Виберіть: ", 0, 3);
            if (choice == 0)
            {
                return;
            }

            var jobs = ReadAllJobs();
            var result = new List<JobRecord>();

            switch (choice)
            {
                case 1:
                    {
                        string q = ReadNonEmpty("Пошук у назві: ");
                        foreach (var j in jobs)
                        {
                            if (!string.IsNullOrEmpty(q) && j.Title.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                result.Add(j);
                            }
                        }

                        break;
                    }

                case 2:
                    {
                        double min = ReadDoubleInRange("Мінімальна зарплата", 0, double.MaxValue);
                        foreach (var j in jobs)
                        {
                            if (j.Salary >= min)
                            {
                                result.Add(j);
                            }
                        }

                        break;
                    }

                case 3:
                    {
                        int minVac = ReadIntInRange("Мінімальна кількість вакансій", 0, int.MaxValue);
                        foreach (var j in jobs)
                        {
                            if (j.Vacancies >= minVac)
                            {
                                result.Add(j);
                            }
                        }

                        break;
                    }
            }

            PrintJobsList(result);
        }
    }
}