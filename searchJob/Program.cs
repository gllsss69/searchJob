using System;
using System.Text;

namespace Application
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("--- Вітаємо в сервісі пошуку роботи! ---");
            Console.ResetColor();

            Console.Write("Введіть кількість вакансій: ");
            double vacancies = double.Parse(Console.ReadLine());

            Console.Write("Введіть середню зарплату: ");
            double avgSalary = double.Parse(Console.ReadLine());

            Console.Write("Введіть рівень конкуренції (людей на вакансію): ");
            double competition = double.Parse(Console.ReadLine());

            double score = Math.Sqrt((avgSalary * vacancies) / competition);
            double scale = 100.0;

            double rating = 100.0 * (score / (score + scale));
            rating = Math.Round(rating, 2);

            Console.WriteLine($"\nВаш шанс знайти роботу: {rating}%");
        }
    }
}
