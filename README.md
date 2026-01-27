# searchJob

Короткий опис
- Консольна .NET 8 програма для збереження та перегляду вакансій, роботодавців, користувачів та заявок у CSV-файлах.
- Підтримує реєстрацію/вхід користувачів з ролями (`admin` / `client`), подачу заявок, перегляд і обробку заявок адміністратором.

Основні можливості
- Меню для клієнта: перегляд вакансій, подача заявки, перегляд своїх заявок, калькулятор шансів, пошук.
- Меню для адміністратора: розгляд заявок, керування записами (додавання, редагування, видалення, статистика).
- Дані зберігаються у CSV: `jobs.csv`, `clients.csv`, `users.csv`, `applications.csv`.
- Парсинг CSV з простим ескейпінгом, захист паролів через SHA256 хеш.

Вимоги
- .NET 8 SDK
- Працює у Windows / Visual Studio або через CLI

Запуск
1. Відкрити рішення у __Solution Explorer__ або запустити з терміналу:
   - dotnet build
   - dotnet run --project searchJob
2. Якщо працюєте у Visual Studio — можна використовувати вбудований термінал або __Show All Files__ у __Solution Explorer__ щоб побачити створені CSV.

Файли даних (CSV)
- `users.csv` — хедер: `Id,Email,PasswordHash,Role`
  - Id (int), Email (string), PasswordHash (hex SHA256), Role (`admin` / `client`)
- `clients.csv` — хедер: `Id,Name,Age,City,Profession`
  - Id (int), Name (string), Age (int), City (string), Profession (string)
- `jobs.csv` — хедер: `Id,Title,Salary,Vacancies,Competition,ClientId`
  - Id (int), Title (string), Salary (double), Vacancies (int), Competition (double), ClientId (int, посилання на `clients.csv`)
- `applications.csv` — хедер: `Id,JobId,UserId,ApplicantName,City,Age,Experience,Status`
  - Id (int), JobId (int), UserId (int), ApplicantName (string), City (string), Age (int), Experience (string), Status (`Очікує`/`Прийнято`/`Відхилено`)

Примітки щодо використання та налаштування
- При першому запуску програма створює файли з потрібними хедерами, якщо їх немає.
- Дефолтний адмін: email `admin`, пароль `1234` (хеш зберігається у `users.csv`). Змініть після першого входу.
- Не зберігайте `bin/` в репозиторії — `.gitignore` вже містить правило для ігнорування build-виходів.
- Якщо зміните порядок/назви колонок у CSV — потрібно оновити парсинг у коді.

Проблеми та виправлення
- Якщо у __Solution Explorer__ не видно згенерованих CSV, натисніть __Show All Files__ і включіть потрібні файли до проекту або відкрийте їх на диску.
- Помилки парсингу рядків (некоректні числа) призводять до пропуску рядка — перевірте вміст CSV на некоректні символи.
- Для відлагодження дивіться вивід у терміналі або логах Visual Studio.

Розширення / ідеї
- Заміна CSV на SQLite для кращої надійності і запитів.
- Додавання unit-тестів для парсерів та логіки авторизації.
- API-версія (ASP.NET) для доступу з веб-інтерфейсу.

Контакт / внесок
- Клон репозиторію: `https://github.com/gllsss69/searchJob`
- Pull requests вітаються — створюйте їх в репозиторії `origin`.
