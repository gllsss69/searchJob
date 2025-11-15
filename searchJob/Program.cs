using System;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        ShowMainMenu();
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
            Console.WriteLine("0. Вихід");

            Console.Write("\nВиберіть дію (0–2): ");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Неправильний ввід! Введіть число від 0 до 2.");
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
                    return; // поввернутись у головне меню
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

        double score = Math.Sqrt((avgSalary * vacancies) / competition);
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
