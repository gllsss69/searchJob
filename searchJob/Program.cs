using System;
using System.Text;
using System.Collections.Generic;

struct JobRecord
{
    public string Title;
    public double Salary;
    public int Vacancies;
    public double Competition;

    public JobRecord(string title, double salary, int vacancies, double competition)
    {
        Title = title;
        Salary = salary;
        Vacancies = vacancies;
        Competition = competition;
    }
}

struct Client
{
    public string Name;
    public int Age;
    public string City;
    public string Profession;

    public Client(string name, int age, string city, string profession)
    {
        Name = name;
        Age = age;
        City = city;
        Profession = profession;
    }
}


class Program
{

    static JobRecord[] jobsData = new JobRecord[5];
    static Client[] clients = new Client[5];


    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        if (LoginSystem())
            ShowMainMenu();
    }


    static bool LoginSystem()
    {
        const string correctLogin = "admin";
        const string correctPass = "1234";

        int attempts = 3;

        do
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("--- Вхід у систему ---");
            Console.ResetColor();
            Console.Write("\nЛогін: ");
            string login = Console.ReadLine();

            Console.Write("Пароль: ");
            string pass = Console.ReadLine();

            if (login == correctLogin && pass == correctPass)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Вхід виконано успішно!\n");
                Console.ResetColor();
                return true;
            }

            attempts--;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Невірні дані. Залишилось спроб: {attempts}");
            Console.ResetColor();

        } while (attempts > 0);

        Console.WriteLine("Спроби вичерпано. Програма завершується.");
        return false;
    }


    static void ShowMainMenu()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("--- Головне меню ---");
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
                    EnterData(); // введення 5 записів
                    break;
                case 4:
                    ShowStatistics(); // статистика
                    break;
                case 5:
                    CreateReport(); // форматований звіт
                    break;
                case 6:
                    ManageCollectionMenu(); // нове меню керування
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

            Console.WriteLine("1. Програміст");
            Console.WriteLine("2. Дизайнер");
            Console.WriteLine("3. Менеджер");
            Console.WriteLine("4. Інженер");
            Console.WriteLine("5. Маркетолог");
            Console.WriteLine("6. Водій");
            Console.WriteLine("7. Електрик");
            Console.WriteLine("8. Лікар");
            Console.WriteLine("9. Вчитель");
            Console.WriteLine("0. Повернутись у головне меню");

            Console.Write("\nВиберіть професію (0–9): ");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Неправильний ввід! Введіть число від 0 до 9.");
                Console.ResetColor();
                continue;
            }

            switch (choice)
            {
                case 0:
                    return; // повертатись у головне меню
                case 1:
                    CalculateProfession("Програміст");
                    break;
                case 2:
                    CalculateProfession("Дизайнер");
                    break;
                case 3:
                    CalculateProfession("Менеджер");
                    break;
                case 4:
                    CalculateProfession("Інженер");
                    break;
                case 5:
                    CalculateProfession("Маркетолог");
                    break;
                case 6:
                    CalculateProfession("Водій");
                    break;
                case 7:
                    CalculateProfession("Електрик");
                    break;
                case 8:
                    CalculateProfession("Лікар");
                    break;
                case 9:
                    CalculateProfession("Вчитель");
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Неправильний вибір! Спробуйте ще раз.");
                    Console.ResetColor();
                    break;
            }
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
                Console.WriteLine("Попередження: конкуренція не може бути < 0.");
                Console.ResetColor();

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


    static void EnterData()
    {

        Console.WriteLine("\n--- Заповнення даних (5 записів) ---");
        for (int i = 0; i < 5; i++)
        {

            Console.WriteLine($"\n--- Запис {i + 1} ---");


            Console.Write("Назва професії: ");
            string title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Порожня назва — запис пропускається.");
                Console.ResetColor();
                continue;
            }


            double salary = ReadDouble("Середня зарплата (грн): ");
            if (salary < 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Зарплата не може бути від'ємною — запис пропускається.");
                Console.ResetColor();
                continue;
            }

            int vacancies = (int)ReadDouble("Кількість вакансій: ");
            if (vacancies < 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Кількість вакансій не може бути від'ємною — запис пропускається.");
                Console.ResetColor();
                continue;
            }


            double competition = ReadDouble("Рівень конкуренції: ");

            jobsData[i] = new JobRecord(title, salary, vacancies, competition);


            Console.Write("Ім'я клієнта: ");
            string clientName = Console.ReadLine();
            int age = (int)ReadDouble("Вік клієнта: ");
            Console.Write("Місто клієнта: ");
            string city = Console.ReadLine();

            clients[i] = new Client(clientName, age, city, title);



        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nВведення даних завершено.");
        Console.ResetColor();
    }

    static void ShowStatistics()
    {
        Console.WriteLine("\n--- Статистика по введених вакансіях ---");

        double sumSalary = 0;
        int filled = 0;
        double minSalary = double.MaxValue;
        double maxSalary = double.MinValue;
        int countAbove20000 = 0;

        foreach (var j in jobsData)
        {
            if (string.IsNullOrWhiteSpace(j.Title))
                continue;

            filled++;
            sumSalary += j.Salary;
            if (j.Salary < minSalary) minSalary = j.Salary;
            if (j.Salary > maxSalary) maxSalary = j.Salary;
            if (j.Salary > 20000) countAbove20000++;
        }

        if (filled == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Немає введених записів для статистики.");
            Console.ResetColor();
            return;
        }

        double avg = sumSalary / filled;

        Console.WriteLine($"Кількість заповнених записів: {filled}");
        Console.WriteLine($"Загальна сума зарплат: {sumSalary:F2} грн");
        Console.WriteLine($"Середня зарплата: {avg:F2} грн");
        Console.WriteLine($"Мінімальна зарплата: {minSalary:F2} грн");
        Console.WriteLine($"Максимальна зарплата: {maxSalary:F2} грн");
        Console.WriteLine($"Кількість зарплат більше 20000 грн: {countAbove20000}");

        
        int sumVacancies = 0;
        int minVac = int.MaxValue;
        int maxVac = int.MinValue;
        double sumCompetition = 0;
        double minComp = double.MaxValue;
        double maxComp = double.MinValue;
        int vacCount = 0;
        int compCount = 0;

        foreach (var j in jobsData)
        {
            if (string.IsNullOrWhiteSpace(j.Title)) continue;
            sumVacancies += j.Vacancies;
            if (j.Vacancies < minVac) minVac = j.Vacancies;
            if (j.Vacancies > maxVac) maxVac = j.Vacancies;
            vacCount++;

            sumCompetition += j.Competition;
            if (j.Competition < minComp) minComp = j.Competition;
            if (j.Competition > maxComp) maxComp = j.Competition;
            compCount++;
        }

        if (vacCount > 0)
        {
            Console.WriteLine($"\nВакансій: загалом {sumVacancies}, мін {minVac}, макс {maxVac}, середнє {(double)sumVacancies / vacCount:F2}");
        }
        if (compCount > 0)
        {
            Console.WriteLine($"Конкуренція: загальна {sumCompetition:F2}, мін {minComp:F2}, макс {maxComp:F2}, середнє {sumCompetition / compCount:F2}");
        }
    }


    static void CreateReport()
    {

        Console.WriteLine("\n--- Звіт ---");

        for (int i = 0; i < jobsData.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(jobsData[i].Title))
            {

                break;
            }

            Console.WriteLine($"\nЗапис #{i + 1}");
            Console.WriteLine($"Професія: {jobsData[i].Title}");
            Console.WriteLine($"Зарплата: {jobsData[i].Salary} грн");
            Console.WriteLine($"Вакансії: {jobsData[i].Vacancies}");
            Console.WriteLine($"Конкуренція: {jobsData[i].Competition}");

            Console.WriteLine($"Клієнт: {clients[i].Name}, {clients[i].Age} років, місто {clients[i].City}");

        }

        Console.WriteLine("\n--- Кінець звіту ---");
    }


    static void ManageCollectionMenu()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n--- Керування базою ---");
            Console.ResetColor();
            Console.WriteLine("1. Додати елементи");
            Console.WriteLine("2. Вивести всі елементи");
            Console.WriteLine("3. Пошук елемента");
            Console.WriteLine("4. Видалення елемента");
            Console.WriteLine("5. Сортування");
            Console.WriteLine("6. Статистика");
            Console.WriteLine("0. Повернутись у головне меню");

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
                case 0:
                    return;
                case 1:
                    AddElements();
                    break;
                case 2:
                    PrintAllAsTable();
                    break;
                case 3:
                    SearchElement();
                    break;
                case 4:
                    DeleteElement();
                    break;
                case 5:
                    SortMenu();
                    break;
                case 6:
                    ShowStatistics();
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Невідомий вибір.");
                    Console.ResetColor();
                    break;
            }
        }
    }

    static void AddElements()
    {
        int freeSlots = 0;
        for (int i = 0; i < jobsData.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(jobsData[i].Title)) freeSlots++;
        }

        if (freeSlots == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Масив вже заповнений. Спочатку видаліть елемент.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine($"\nВільних місць: {freeSlots}. Скільки додати? (1–{freeSlots})");
        int toAdd = (int)ReadDouble("Кількість для додавання: ");
        if (toAdd < 1) { Console.WriteLine("Нічого не додано."); return; }
        if (toAdd > freeSlots) toAdd = freeSlots;

        for (int k = 0; k < toAdd; k++)
        {
            int idx = -1;
            for (int i = 0; i < jobsData.Length; i++) if (string.IsNullOrWhiteSpace(jobsData[i].Title)) { idx = i; break; }

            if (idx == -1) break;

            Console.WriteLine($"\n--- Додавання запису у позицію #{idx} ---");
            Console.Write("Назва професії: ");
            string title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Порожня назва — запис пропускається.");
                Console.ResetColor();
                continue;
            }

            double salary = ReadDouble("Середня зарплата (грн): ");
            if (salary < 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Зарплата не може бути від'ємною — запис пропускається.");
                Console.ResetColor();
                continue;
            }

            int vacancies = (int)ReadDouble("Кількість вакансій: ");
            if (vacancies < 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("К-ть вакансій не може бути від'ємною — запис пропускається.");
                Console.ResetColor();
                continue;
            }

            double competition = ReadDouble("Рівень конкуренції: ");

            jobsData[idx] = new JobRecord(title, salary, vacancies, competition);

            Console.Write("Ім'я клієнта: ");
            string clientName = Console.ReadLine();
            int age = (int)ReadDouble("Вік клієнта: ");
            Console.Write("Місто клієнта: ");
            string city = Console.ReadLine();

            clients[idx] = new Client(clientName, age, city, title);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Додано на позицію #{idx}.");
            Console.ResetColor();
        }
    }

   
    static void PrintAllAsTable()
    {
        Console.WriteLine("\n--- Таблиця вакансій ---");
        Console.WriteLine("{0,-4} {1,-20} {2,12} {3,10} {4,12} {5,-15}", "№", "Професія", "Зарплата", "Вакансії", "Конкуренція", "Клієнт(місто)");
        Console.WriteLine(new string('-', 80));

        int shown = 0;
        for (int i = 0; i < jobsData.Length; i++)
        {
            var j = jobsData[i];
            if (string.IsNullOrWhiteSpace(j.Title)) continue;
            var cl = clients[i];
            string clientInfo = string.IsNullOrWhiteSpace(cl.Name) ? "-" : $"{cl.Name}({cl.City})";
            Console.WriteLine("{0,-4} {1,-20} {2,12:F2} {3,10} {4,12:F2} {5,-15}", i, Truncate(j.Title, 20), j.Salary, j.Vacancies, j.Competition, Truncate(clientInfo, 15));
            shown++;
        }
        if (shown == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Немає записів для виведення.");
            Console.ResetColor();
        }
    }

   
    static void SearchElement()
    {
        Console.WriteLine("\n--- Пошук елемента ---");
        Console.WriteLine("1. За індексом");
        Console.WriteLine("2. За назвою");
        Console.Write("\nВиберіть (1–2): ");
        string input = Console.ReadLine();
        if (!int.TryParse(input, out int mode) || (mode != 1 && mode != 2))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Неправильний вибір.");
            Console.ResetColor();
            return;
        }

        if (mode == 1)
        {
            int idx = (int)ReadDouble("Введіть індекс (0–" + (jobsData.Length - 1) + "): ");
            if (idx < 0 || idx >= jobsData.Length)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Індекс поза межами.");
                Console.ResetColor();
                return;
            }
            if (string.IsNullOrWhiteSpace(jobsData[idx].Title))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("У цій позиції запису немає.");
                Console.ResetColor();
                return;
            }
            PrintSingle(idx);
        }
        else
        {
            Console.Write("Введіть частину назви: ");
            string pattern = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(pattern))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Порожній запит.");
                Console.ResetColor();
                return;
            }
            bool found = false;
            for (int i = 0; i < jobsData.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(jobsData[i].Title) && jobsData[i].Title.IndexOf(pattern, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    PrintSingle(i);
                    found = true;
                }
            }
            if (!found)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Нічого не знайдено.");
                Console.ResetColor();
            }
        }
    }

    static void PrintSingle(int i)
    {
        Console.WriteLine($"\nЗапис #{i}");
        Console.WriteLine($"Професія: {jobsData[i].Title}");
        Console.WriteLine($"Зарплата: {jobsData[i].Salary} грн");
        Console.WriteLine($"Вакансії: {jobsData[i].Vacancies}");
        Console.WriteLine($"Конкуренція: {jobsData[i].Competition}");
        Console.WriteLine($"Клієнт: {clients[i].Name} {clients[i].Age} років, місто {clients[i].City}");
    }

   
    static void DeleteElement()
    {
        Console.WriteLine("\n--- Видалення елемента ---");
        Console.WriteLine("1. За індексом");
        Console.WriteLine("2. За назвою (перший знайдений)");
        Console.Write("\nВиберіть (1–2): ");
        string input = Console.ReadLine();
        if (!int.TryParse(input, out int mode) || (mode != 1 && mode != 2))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Неправильний вибір.");
            Console.ResetColor();
            return;
        }

        if (mode == 1)
        {
            int idx = (int)ReadDouble("Введіть індекс для видалення (0–" + (jobsData.Length - 1) + "): ");
            if (idx < 0 || idx >= jobsData.Length)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Індекс поза межами.");
                Console.ResetColor();
                return;
            }
            if (string.IsNullOrWhiteSpace(jobsData[idx].Title))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Немає запису за вказаним індексом.");
                Console.ResetColor();
                return;
            }
            ShiftLeftFrom(idx);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Елемент за індексом {idx} видалено.");
            Console.ResetColor();
        }
        else
        {
            Console.Write("Введіть назву для видалення: ");
            string pattern = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(pattern))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Порожній запит.");
                Console.ResetColor();
                return;
            }
            int foundIdx = -1;
            for (int i = 0; i < jobsData.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(jobsData[i].Title) && jobsData[i].Title.IndexOf(pattern, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    foundIdx = i;
                    break;
                }
            }
            if (foundIdx == -1)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Нічого не знайдено для видалення.");
                Console.ResetColor();
                return;
            }
            ShiftLeftFrom(foundIdx);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Елемент '{pattern}' видалено (індекс {foundIdx}).");
            Console.ResetColor();
        }
    }

   
    static void ShiftLeftFrom(int idx)
    {
        for (int i = idx; i < jobsData.Length - 1; i++)
        {
            jobsData[i] = jobsData[i + 1];
            clients[i] = clients[i + 1];
        }
        jobsData[jobsData.Length - 1] = new JobRecord(); 
        clients[clients.Length - 1] = new Client();
    }

   
    static void SortMenu()
    {
        Console.WriteLine("\n--- Сортування ---");
        Console.WriteLine("Поле сортування:");
        Console.WriteLine("1. За назвою (алфавіт)");
        Console.WriteLine("2. За зарплатою (зростання)");
        Console.WriteLine("3. За кількістю вакансій (зростання)");
        Console.Write("\nВиберіть поле (1–3): ");
        string input = Console.ReadLine();
        if (!int.TryParse(input, out int field) || field < 1 || field > 3)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Неправильний вибір.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine("\nАлгоритм сортування:");
        Console.WriteLine("1. Власний (бульбашковий)");
        Console.WriteLine("2. Вбудований");
        Console.Write("\nВиберіть (1–2): ");
        input = Console.ReadLine();
        if (!int.TryParse(input, out int algo) || (algo != 1 && algo != 2))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Неправильний вибір.");
            Console.ResetColor();
            return;
        }

        if (algo == 1)
        {
            BubbleSort(field);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Сортування бульбашкою виконано.");
            Console.ResetColor();
        }
        else
        {
            BuiltInSort(field);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Вбудоване сортування (List.Sort) виконано.");
            Console.ResetColor();
        }
    }

    static void BubbleSort(int field)
    {
        int n = jobsData.Length;
        bool swapped;
        
        int last = -1;
        for (int i = 0; i < jobsData.Length; i++) if (!string.IsNullOrWhiteSpace(jobsData[i].Title)) last = i;
        if (last <= 0) return;

        for (int pass = 0; pass <= last; pass++)
        {
            swapped = false;
            for (int i = 0; i < last - pass; i++)
            {
                if (string.IsNullOrWhiteSpace(jobsData[i].Title)) continue;
                if (string.IsNullOrWhiteSpace(jobsData[i + 1].Title)) continue;

                bool needSwap = false;
                switch (field)
                {
                    case 1:
                        if (string.Compare(jobsData[i].Title, jobsData[i + 1].Title, StringComparison.CurrentCultureIgnoreCase) > 0) needSwap = true;
                        break;
                    case 2:
                        if (jobsData[i].Salary > jobsData[i + 1].Salary) needSwap = true;
                        break;
                    case 3:
                        if (jobsData[i].Vacancies > jobsData[i + 1].Vacancies) needSwap = true;
                        break;
                }
                if (needSwap)
                {
                    var tmpJ = jobsData[i];
                    jobsData[i] = jobsData[i + 1];
                    jobsData[i + 1] = tmpJ;

                    var tmpC = clients[i];
                    clients[i] = clients[i + 1];
                    clients[i + 1] = tmpC;

                    swapped = true;
                }
            }
            if (!swapped) break;
        }
    }

   
    static void BuiltInSort(int field)
    {
       
        List<JobRecord> jlist = new List<JobRecord>();
        List<Client> clist = new List<Client>();
        for (int i = 0; i < jobsData.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(jobsData[i].Title))
            {
                jlist.Add(jobsData[i]);
                clist.Add(clients[i]);
            }
        }
        if (jlist.Count <= 1) return;

        if (field == 1)
        {
            jlist.Sort((a, b) => string.Compare(a.Title, b.Title, StringComparison.CurrentCultureIgnoreCase));
        }
        else if (field == 2)
        {
            jlist.Sort((a, b) => a.Salary.CompareTo(b.Salary));
        }
        else if (field == 3)
        {
            jlist.Sort((a, b) => a.Vacancies.CompareTo(b.Vacancies));
        }

       
        int idx = 0;
        for (int i = 0; i < jobsData.Length; i++)
        {
            jobsData[i] = new JobRecord();
            clients[i] = new Client();
        }
        for (int i = 0; i < jlist.Count && i < jobsData.Length; i++)
        {
            jobsData[i] = jlist[i];
           
            clients[i] = FindClientByProfession(jlist[i].Title);
        }
    }

   
    static Client FindClientByProfession(string profession)
    {
        for (int i = 0; i < clients.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(clients[i].Profession) && clients[i].Profession.Equals(profession, StringComparison.CurrentCultureIgnoreCase))
                return clients[i];
        }
        return new Client();
    }

    

    static double ReadDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();

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
