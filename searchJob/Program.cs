using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

record JobRecord(int Id, string Title, double Salary, int Vacancies, double Competition, int ClientId);
record ClientRecord(int Id, string Name, int Age, string City, string Profession);
record UserRecord(int Id, string Email, string PasswordHash);


class Program
{
    
    const string JobsFile = "jobs.csv";
    const string ClientsFile = "clients.csv";
    const string UsersFile = "users.csv";

    
    static readonly string JobsHeader = "Id,Title,Salary,Vacancies,Competition,ClientId";
    static readonly string ClientsHeader = "Id,Name,Age,City,Profession";
    static readonly string UsersHeader = "Id,Email,PasswordHash";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        
        EnsureFileWithHeader(JobsFile, JobsHeader);
        EnsureFileWithHeader(ClientsFile, ClientsHeader);
        EnsureFileWithHeader(UsersFile, UsersHeader);

        
        var users = ReadAllUsers();
        if (users.Count == 0)
        {
            var admin = new UserRecord(GetNextIdFromFile(UsersFile), "admin", ComputeHash("1234"));
            AppendLineToFile(UsersFile, ToCsv(admin.Email, admin.PasswordHash, admin.Id.ToString()));
            users = new List<UserRecord> { admin };
            WriteAllUsers(users);
        }

        if (!LoginSystem())
        {
            Console.WriteLine("Вихід.");
            return;
        }

        ShowMainMenu();
    }

   

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
                if (first != null) rest.Add(first);
                while ((line = sr.ReadLine()) != null) rest.Add(line);

                
                using var sw = new StreamWriter(path, false, new UTF8Encoding(false));
                sw.WriteLine(header);
                foreach (var r in rest) if (!string.IsNullOrWhiteSpace(r)) sw.WriteLine(r);
            }
        }
        catch
        {
           
            File.WriteAllText(path, header + Environment.NewLine, new UTF8Encoding(false));
        }
    }

    static void AppendLineToFile(string path, string line)
    {
        try
        {
            using var sw = new StreamWriter(path, true, new UTF8Encoding(false));
            sw.WriteLine(line);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Помилка дозапису у файл {path}: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void WriteAllLines(string path, IEnumerable<string> lines)
    {
        try
        {
            using var sw = new StreamWriter(path, false, new UTF8Encoding(false));
            foreach (var line in lines) sw.WriteLine(line);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Помилка запису файлу {path}: {ex.Message}");
            Console.ResetColor();
        }
    }

    static string EscapeCsv(string field)
    {
        if (field == null) return "";
        if (field.Contains('"'))
            field = field.Replace("\"", "\"\"");
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            return $"\"{field}\"";
        return field;
    }

    static List<string> ParseCsvLine(string line)
    {
        var res = new List<string>();
        if (line == null) return res;
        int i = 0; int n = line.Length;
        while (i < n)
        {
            if (line[i] == '"')
            {
                
                i++; var sb = new StringBuilder();
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
               
                while (i < n && line[i] != ',') i++;
                if (i < n && line[i] == ',') i++;
                res.Add(sb.ToString());
            }
            else
            {
                var start = i;
                while (i < n && line[i] != ',') i++;
                res.Add(line.Substring(start, i - start));
                if (i < n && line[i] == ',') i++;
            }
        }
        return res;
    }

    static string ToCsv(params string[] fields)
    {
        var parts = new List<string>(fields.Length);
        foreach (var f in fields) parts.Add(EscapeCsv(f));
        return string.Join(",", parts);
    }

    
    static int GetNextIdFromFile(string path)
    {
        try
        {
            if (!File.Exists(path)) return 1;
            using var sr = new StreamReader(path, Encoding.UTF8);
            string header = sr.ReadLine();
            int max = 0;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                try
                {
                    var fields = ParseCsvLine(line);
                    if (fields.Count == 0) continue;
                    if (int.TryParse(fields[0], out int id))
                        if (id > max) max = id;
                }
                catch { continue; }
            }
            return max + 1;
        }
        catch
        {
            return 1;
        }
    }

   

  

    static bool LoginSystem()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("--- Вхід у систему ---");
            Console.ResetColor();
            Console.WriteLine("1. Увійти");
            Console.WriteLine("2. Зареєструватися");
            Console.WriteLine("0. Вихід");
            Console.Write("Виберіть (0-2): ");
            var k = Console.ReadLine();
            if (k == "0") return false;
            if (k == "2") { RegisterUser(); continue; }
            if (k == "1")
            {
                Console.Write("Email: ");
                var email = Console.ReadLine()?.Trim().ToLower();
                Console.Write("Пароль: ");
                var pass = ReadPassword();

                if (Authenticate(email, pass))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Вхід виконано успішно!\n");
                    Console.ResetColor();
                    return true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Невірні дані. Спробуйте ще.");
                    Console.ResetColor();
                }
            }
        }
    }

    static void RegisterUser()
    {
        Console.WriteLine("\n--- Реєстрація ---");
        Console.Write("Email: ");
        var email = Console.ReadLine()?.Trim().ToLower();
        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Порожній email. Скасовано.");
            return;
        }
        var users = ReadAllUsers();
        bool exists = false;
        foreach (var user in users)
        {
            if (user.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
            {
                exists = true;
                break;
            }
        }
        if (exists)
        {
            Console.WriteLine("Користувач з таким email вже існує.");
            return;
        }
        Console.Write("Пароль: ");
        var pass = ReadPassword();
        if (string.IsNullOrWhiteSpace(pass)) { Console.WriteLine("Порожній пароль. Скасовано."); return; }

        var id = GetNextIdFromFile(UsersFile);
        var hash = ComputeHash(pass);
        var u = new UserRecord(id, email, hash);
       
        AppendLineToFile(UsersFile, ToCsv(id.ToString(), email, hash));
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Реєстрацію завершено.");
        Console.ResetColor();
    }

    static bool Authenticate(string email, string password)
    {
        try
        {
            var users = ReadAllUsers();
            var hash = ComputeHash(password);
            foreach (var u in users)
            {
                if (u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && u.PasswordHash == hash)
                    return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    static List<UserRecord> ReadAllUsers()
    {
        var res = new List<UserRecord>();
        try
        {
            using var sr = new StreamReader(UsersFile, Encoding.UTF8);
            string header = sr.ReadLine();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                try
                {
                    var f = ParseCsvLine(line);
                   
                    if (f.Count < 3) continue;
                    if (!int.TryParse(f[0], out int id)) continue;
                    var email = f[1];
                    var ph = f[2];
                    res.Add(new UserRecord(id, email, ph));
                }
                catch { continue; }
            }
        }
        catch { }
        return res;
    }

    static void WriteAllUsers(List<UserRecord> users)
    {
        var lines = new List<string> { UsersHeader };
        foreach (var u in users)
            lines.Add(ToCsv(u.Id.ToString(), u.Email, u.PasswordHash));
        WriteAllLines(UsersFile, lines);
    }

    static string ComputeHash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash); 
    }

    static string ReadPassword()
    {
       
        var pass = new StringBuilder();
        ConsoleKeyInfo key;
        while (true)
        {
            key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter) break;
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

   
   

    static List<JobRecord> ReadAllJobs()
    {
        var res = new List<JobRecord>();
        try
        {
            using var sr = new StreamReader(JobsFile, Encoding.UTF8);
            string header = sr.ReadLine();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                try
                {
                    var f = ParseCsvLine(line);
                
                    if (f.Count < 6) continue;
                    if (!int.TryParse(f[0], out int id)) continue;
                    var title = f[1];
                    if (!double.TryParse(f[2], out double salary)) continue;
                    if (!int.TryParse(f[3], out int vac)) continue;
                    if (!double.TryParse(f[4], out double comp)) continue;
                    if (!int.TryParse(f[5], out int clientId)) clientId = 0;
                    res.Add(new JobRecord(id, title, salary, vac, comp, clientId));
                }
                catch { continue; }
            }
        }
        catch { }
        return res;
    }

    static void WriteAllJobs(List<JobRecord> jobs)
    {
        var lines = new List<string> { JobsHeader };
        foreach (var j in jobs)
            lines.Add(ToCsv(j.Id.ToString(), j.Title, j.Salary.ToString(), j.Vacancies.ToString(), j.Competition.ToString(), j.ClientId.ToString()));
        WriteAllLines(JobsFile, lines);
    }

    static void AppendJob(JobRecord job)
    {
        AppendLineToFile(JobsFile, ToCsv(job.Id.ToString(), job.Title, job.Salary.ToString(), job.Vacancies.ToString(), job.Competition.ToString(), job.ClientId.ToString()));
    }

    static List<ClientRecord> ReadAllClients()
    {
        var res = new List<ClientRecord>();
        try
        {
            using var sr = new StreamReader(ClientsFile, Encoding.UTF8);
            string header = sr.ReadLine();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                try
                {
                    var f = ParseCsvLine(line);
                    if (f.Count < 5) continue;
                    if (!int.TryParse(f[0], out int id)) continue;
                    var name = f[1];
                    if (!int.TryParse(f[2], out int age)) age = 0;
                    var city = f[3];
                    var prof = f[4];
                    res.Add(new ClientRecord(id, name, age, city, prof));
                }
                catch { continue; }
            }
        }
        catch { }
        return res;
    }

   
    static void AppendClient(ClientRecord client)
    {
        AppendLineToFile(ClientsFile, ToCsv(client.Id.ToString(), client.Name, client.Age.ToString(), client.City, client.Profession));
    }




    static void ShowMainMenu()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n--- Головне меню ---");
            Console.ResetColor();

            Console.WriteLine("\n1. Знайти роботу");
            Console.WriteLine("2. Налаштування");
            Console.WriteLine("3. Ввести/оновити вакансії");
            Console.WriteLine("4. Статистика");
            Console.WriteLine("5. Звіт");
            Console.WriteLine("6. Керування базою");
            Console.WriteLine("0. Вихід");

            Console.Write("\nВиберіть дію (0–6): ");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Неправильний ввід! Введіть число від 0 до 6.");
                Console.ResetColor();
                continue;
            }

            switch (choice)
            {
                case 0:
                    Console.WriteLine("Вихід з програми...");
                    return;
                case 1:
                    ShowJobMenu();
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Налаштування: Функція в розробці.");
                    Console.ResetColor();
                    break;
                case 3:
                    EnterDataCsv();
                    break;
                case 4:
                    ShowStatisticsCsv();
                    break;
                case 5:
                    PrintAllAsTableCsv();
                    break;
                case 6:
                    ManageCollectionMenuCsv();
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Неправильний вибір! Спробуйте ще раз.");
                    Console.ResetColor();
                    break;
            }
        }
    }



    static void ShowJobMenu()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n--- Меню професій ---");
            Console.ResetColor();

            var professions = new[] { "Програміст", "Дизайнер", "Менеджер", "Інженер", "Маркетолог", "Водій", "Електрик", "Лікар", "Вчитель"};
            for (int i = 0; i < professions.Length; i++) Console.WriteLine($"{i + 1}. {professions[i]}");
            Console.WriteLine("0. Повернутись у головне меню");

            Console.Write("\nВиберіть професію (0–9): ");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int choice) || choice < 0 || choice > professions.Length)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Неправильний ввід! Введіть число від 0 до 9.");
                Console.ResetColor();
                continue;
            }
            if (choice == 0) return;
            string profession = professions[choice - 1];
            CalculateProfession(profession);
        }
    }

    static void CalculateProfession(string profession)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\nВи обрали професію: {profession}");
        Console.ResetColor();

        double vacancies = ReadDouble("Введіть кількість вакансій: ");
        double avgSalary = ReadDouble("Введіть середню зарплату: ");
        double competition = ReadDouble("Введіть рівень конкуренції (людей на вакансію): ");

        double score = 0.0;
        try
        {
            if (competition <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Попередження: конкуренція не може бути <= 0, використано 1.");
                Console.ResetColor();
                competition = 1;
            }
            score = Math.Sqrt((avgSalary * vacancies) / competition);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Помилка у розрахунку: {ex.Message}");
            Console.ResetColor();
        }

        double scale = 100.0;
        double rating = Math.Round(100.0 * (score / (score + scale)), 2);

        Console.WriteLine($"\nВаш рейтинг шансів: {rating}");

        if (rating > 50)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Високі шанси знайти роботу!");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Низькі шанси знайти роботу.");
        }
        Console.ResetColor();
    }

  

    

    static void EnterDataCsv()
    {
        Console.WriteLine("\n--- Додавання нових записів (через CSV) ---");
        int count = (int)ReadDouble("Скільки записів додати? ");
        if (count <= 0) { Console.WriteLine("Нічого не додано."); return; }

        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"\n--- Запис {i + 1} ---");
            Console.Write("Назва професії: ");
            string title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title)) { Console.WriteLine("Пропущено порожню назву."); continue; }

            double salary = ReadDouble("Середня зарплата (грн): ");
            if (salary < 0) { Console.WriteLine("Пропущено через від'ємну зарплату."); continue; }

            int vacancies = (int)ReadDouble("Кількість вакансій: ");
            if (vacancies < 0) { Console.WriteLine("Пропущено через від'ємну к-ть вакансій."); continue; }

            double competition = ReadDouble("Рівень конкуренції: ");

            Console.Write("Ім'я клієнта: ");
            string clientName = Console.ReadLine();
            int age = (int)ReadDouble("Вік клієнта: ");
            Console.Write("Місто клієнта: ");
            string city = Console.ReadLine();

            
            int clientId = GetNextIdFromFile(ClientsFile);
            var client = new ClientRecord(clientId, clientName, age, city, title);
            AppendClient(client);

          
            int jobId = GetNextIdFromFile(JobsFile);
            var job = new JobRecord(jobId, title, salary, vacancies, competition, clientId);
            AppendJob(job);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Записи додано.");
            Console.ResetColor();
        }
    }

    

    static void PrintAllAsTableCsv()
    {
        var jobs = ReadAllJobs();
        var clientsList = ReadAllClients();
        var clients = new Dictionary<int, ClientRecord>();
        foreach (var c in clientsList)
        {
            if (!clients.ContainsKey(c.Id)) clients[c.Id] = c;
        }

        Console.WriteLine("\n--- Таблиця вакансій ---");
        Console.WriteLine("{0,-4} {1,-25} {2,12} {3,10} {4,12} {5,-20}", "Id", "Професія", "Зарплата", "Вакансії", "Конкуренція", "Клієнт(місто)");
        Console.WriteLine(new string('-', 95));
        if (jobs.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Немає записів.");
            Console.ResetColor();
            return;
        }

       
        jobs.Sort((a, b) => a.Id.CompareTo(b.Id));

        foreach (var j in jobs)
        {
            string clientInfo = "-";
            if (clients.TryGetValue(j.ClientId, out var c))
                clientInfo = $"{Truncate(c.Name, 12)}({Truncate(c.City, 6)})";
            Console.WriteLine("{0,-4} {1,-25} {2,12:F2} {3,10} {4,12:F2} {5,-20}", j.Id, Truncate(j.Title, 25), j.Salary, j.Vacancies, j.Competition, clientInfo);
        }
    }


    static void ManageCollectionMenuCsv()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n--- Керування базою (CSV) ---");
            Console.ResetColor();
            Console.WriteLine("1. Додати вакансію");
            Console.WriteLine("2. Вивести всі вакансії");
            Console.WriteLine("3. Пошук вакансії");
            Console.WriteLine("4. Видалити вакансію");
            Console.WriteLine("5. Редагувати вакансію");
            Console.WriteLine("6. Сортування (тимчасове виведення)");
            Console.WriteLine("0. Повернутись");

            Console.Write("\nВиберіть (0–6): ");
            string input = Console.ReadLine();
            if (!int.TryParse(input, out int choice))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Неправильний ввід!");
                Console.ResetColor();
                continue;
            }
            switch (choice)
            {
                case 0: return;
                case 1: EnterDataCsv(); break;
                case 2: PrintAllAsTableCsv(); break;
                case 3: SearchElementCsv(); break;
                case 4: DeleteElementCsv(); break;
                case 5: EditElementCsv(); break;
                case 6: SortAndShowCsv(); break;
                default:
                Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Невідомий вибір."); Console.ResetColor(); break;
            }
        }
    }

    static void SearchElementCsv()
    {
        Console.WriteLine("\n--- Пошук ---");
        Console.WriteLine("1. За Id");
        Console.WriteLine("2. За назвою (частково)");
        Console.Write("Виберіть (1-2): ");
        var s = Console.ReadLine();
        if (s == "1")
        {
            int id = (int)ReadDouble("Введіть Id: ");
            var job = (JobRecord?)null;
            var jobs = ReadAllJobs();
            foreach (var j in jobs) if (j.Id == id) { job = j; break; }
            if (job == null) { Console.WriteLine("Не знайдено."); return; }
            PrintSingleCsv(job);
        }
        else if (s == "2")
        {
            Console.Write("Введіть частину назви: ");
            var pattern = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(pattern)) { Console.WriteLine("Порожній запит."); return; }
            var allJobs = ReadAllJobs();
            var list = new List<JobRecord>();
            foreach (var j in allJobs)
            {
                if (j.Title.IndexOf(pattern, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    list.Add(j);
            }
            if (list.Count == 0) { Console.WriteLine("Не знайдено."); return; }
            foreach (var j in list) PrintSingleCsv(j);
        }
        else Console.WriteLine("Невірний вибір.");
    }

    static void PrintSingleCsv(JobRecord j)
    {
        var clients = ReadAllClients();
        ClientRecord? client = null;
        foreach (var c in clients) if (c.Id == j.ClientId) { client = c; break; }
        Console.WriteLine($"\nЗапис Id {j.Id}\nПрофесія: {j.Title}\nЗарплата: {j.Salary}\nВакансії: {j.Vacancies}\nКонкуренція: {j.Competition}");
        if (client != null)
            Console.WriteLine($"Клієнт: {client.Name}, {client.Age} років, {client.City}");
    }

    static void DeleteElementCsv()
    {
        Console.WriteLine("\n--- Видалення вакансії ---");
        int id = (int)ReadDouble("Id для видалення: ");
        var jobs = ReadAllJobs();
        JobRecord? job = null;
        foreach (var j in jobs) if (j.Id == id) { job = j; break; }
        if (job == null) { Console.WriteLine("Не знайдено запису."); return; }
        Console.Write($"Підтвердіть видалення Id {id} (y/n): ");
        if (Console.ReadLine()?.ToLower() != "y") { Console.WriteLine("Скасовано."); return; }
        jobs.RemoveAll(j => j.Id == id);
        WriteAllJobs(jobs);
        Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("Видалено."); Console.ResetColor();
    }

    static void EditElementCsv()
    {
        Console.WriteLine("\n--- Редагування вакансії ---");
        int id = (int)ReadDouble("Id для редагування: ");
        var jobs = ReadAllJobs();
        JobRecord? job = null;
        foreach (var j in jobs) if (j.Id == id) { job = j; break; }
        if (job == null) { Console.WriteLine("Не знайдено."); return; }

        Console.WriteLine("Залиште порожнім, щоб не змінювати значення.");
        Console.Write($"Назва ({job.Title}): ");
        var newTitle = Console.ReadLine();
        Console.Write($"Зарплата ({job.Salary}): ");
        var newSalaryStr = Console.ReadLine();
        Console.Write($"Вакансії ({job.Vacancies}): ");
        var newVacStr = Console.ReadLine();
        Console.Write($"Конкуренція ({job.Competition}): ");
        var newCompStr = Console.ReadLine();

        string title = string.IsNullOrWhiteSpace(newTitle) ? job.Title : newTitle;
        double salary = job.Salary; if (double.TryParse(newSalaryStr, out double tmpS)) salary = tmpS;
        int vac = job.Vacancies; if (int.TryParse(newVacStr, out int tmpV)) vac = tmpV;
        double comp = job.Competition; if (double.TryParse(newCompStr, out double tmpC)) comp = tmpC;

        var idx = jobs.FindIndex(j => j.Id == id);
        jobs[idx] = new JobRecord(job.Id, title, salary, vac, comp, job.ClientId);
        WriteAllJobs(jobs);
        Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("Оновлено."); Console.ResetColor();
    }

    static void SortAndShowCsv()
    {
        var jobs = ReadAllJobs();
        if (jobs.Count == 0) { Console.WriteLine("Немає даних."); return; }
        Console.WriteLine("Поле сортування: 1-Title 2-Salary 3-Vacancies");
        var s = Console.ReadLine();
        var sorted = new List<JobRecord>(jobs);
        if (s == "1") sorted.Sort((a, b) => StringComparer.CurrentCultureIgnoreCase.Compare(a.Title, b.Title));
        else if (s == "2") sorted.Sort((a, b) => a.Salary.CompareTo(b.Salary));
        else if (s == "3") sorted.Sort((a, b) => a.Vacancies.CompareTo(b.Vacancies));
        else { Console.WriteLine("Невірно."); return; }

        Console.WriteLine("{0,-4} {1,-25} {2,12} {3,10} {4,12}", "Id", "Професія", "Зарплата", "Вакансії", "Конкуренція");
        Console.WriteLine(new string('-', 80));
        foreach (var j in sorted) Console.WriteLine("{0,-4} {1,-25} {2,12:F2} {3,10} {4,12:F2}", j.Id, Truncate(j.Title, 25), j.Salary, j.Vacancies, j.Competition);
    }



    static void ShowStatisticsCsv()
    {
        var jobs = ReadAllJobs();
        if (jobs.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Немає даних для статистики.");
            Console.ResetColor();
            return;
        }

        double sumSalary = 0;
        double minSalary = double.MaxValue;
        double maxSalary = double.MinValue;
        double sumVacancies = 0;
        int minVac = int.MaxValue;
        int maxVac = int.MinValue;
        double sumCompetition = 0;
        double minCompetition = double.MaxValue;
        double maxCompetition = double.MinValue;

        foreach (var j in jobs)
        {
            sumSalary += j.Salary;
            if (j.Salary < minSalary) minSalary = j.Salary;
            if (j.Salary > maxSalary) maxSalary = j.Salary;

            sumVacancies += j.Vacancies;
            if (j.Vacancies < minVac) minVac = j.Vacancies;
            if (j.Vacancies > maxVac) maxVac = j.Vacancies;

            sumCompetition += j.Competition;
            if (j.Competition < minCompetition) minCompetition = j.Competition;
            if (j.Competition > maxCompetition) maxCompetition = j.Competition;
        }

        double avgSalary = sumSalary / jobs.Count;
        double avgVacancies = sumVacancies / jobs.Count;
        double avgCompetition = sumCompetition / jobs.Count;

        Console.WriteLine("\n--- Статистика по зарплатах ---");
        Console.WriteLine($"Кількість: {jobs.Count}");
        Console.WriteLine($"Загальна сума зарплат: {sumSalary:F2}");
        Console.WriteLine($"Середня зарплата: {avgSalary:F2}");
        Console.WriteLine($"Мінімальна: {minSalary:F2}");
        Console.WriteLine($"Максимальна: {maxSalary:F2}");

        Console.WriteLine("\n--- Статистика по вакансіям ---");
        Console.WriteLine($"Загальна кількість вакансій: {sumVacancies}");
        Console.WriteLine($"Мін вакансій: {minVac}");
        Console.WriteLine($"Макс вакансій: {maxVac}");
        Console.WriteLine($"Середня вакансій на запис: {avgVacancies:F2}");

        Console.WriteLine("\n--- Статистика по конкуренції ---");
        Console.WriteLine($"Загалом: {sumCompetition:F2}");
        Console.WriteLine($"Мін: {minCompetition:F2}");
        Console.WriteLine($"Макс: {maxCompetition:F2}");
        Console.WriteLine($"Середнє: {avgCompetition:F2}");
    }


   

    static double ReadDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return 0;
            if (double.TryParse(input, out double result))
                return result;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Неправильний ввід! Введіть число.");
            Console.ResetColor();
        }
    }

    static string Truncate(string s, int max)
    {
        if (string.IsNullOrEmpty(s)) return s;
        if (s.Length <= max) return s;
        return s.Substring(0, max - 3) + "...";
    }

  
}
