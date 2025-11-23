using System;
using System.Text;

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
            Console.WriteLine("0. Вихід");

            Console.Write("\nВиберіть дію (0–5): ");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Неправильний ввід! Введіть число від 0 до 5.");
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
}
