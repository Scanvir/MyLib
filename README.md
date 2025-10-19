# MyLib

**MyLib** — це універсальна .NET бібліотека для роботи з поштою, логуванням, HTTP-клієнтом та різними утилітами. Створена для легкої інтеграції у будь-які проекти, не містить хардкоду чи специфічних залежностей, і готова для відкритого або комерційного використання.

---

## Повний перелік класів та файлів

- **Mail**  
  _LogFile/Mail.cs_  
  Клас для роботи з електронною поштою (SMTP-клієнт). Підтримує вкладення, асинхронність, авторизацію.

- **LogFile**  
  _LogFile/LogFile.cs_  
  Простий файловий логер із ротацією логів, видаленням старих записів.

- **myHttpClientProvider**  
  _LogFile/myHttpClientProvider.cs_  
  Універсальний HTTP-клієнт для REST API. Підтримує GET, POST, PUT, PATCH, DELETE. Працює з JSON, має зручну авторизацію.

- **SQL**  
  _LogFile/SQL.cs_  
  Робота з SQL Server: відкриття з’єднання, виконання SELECT/EXECUTE, транзакції, обробка результатів.

- **Copy**  
  _LogFile/Copy.cs_  
  Копіювання файлів і папок через ROBOCOPY та File.Copy.

- **MyFileSystem**  
  _LogFile/MyFileSystem.cs_  
  Утиліти для видалення старих файлів, рекурсивного очищення папок, видалення файлів.

- **MyTimer**  
  _LogFile/MyTimer.cs_  
  Таймер для вимірювання часу виконання ділянок коду (Stopwatch із красивим форматуванням).

- **myXML**  
  _LogFile/myXML.cs_  
  Допоміжні методи для роботи з XML: створення документів, додавання вузлів, атрибутів.

- **MyRobot**  
  _LogFile/MyRobot.cs_  
  Базовий клас для створення власних “роботів” з інтерфейсом запуску (метод Run + interface MyRun).

---

## Можливості

- Надсилання email через SMTP, з вкладеннями та авторизацією
- Логування у файл із ротацією і видаленням старих логів
- HTTP-клієнт для REST API з підтримкою JSON, авторизації, гнучкою обробкою відповідей
- Робота з SQL Server (select, execute, транзакції)
- Копіювання файлів та папок, видалення старих файлів
- Робота з XML (створення, модифікація)
- Вимірювання часу виконання коду (таймер)
- Базові патерни для “роботів” (MyRobot)

---

## Приклад використання

### Відправка email

```csharp
var mail = new Mail("smtp.example.com", "user@example.com", "password123");
await mail.Send(
    "Тема листа",
    "Текст повідомлення",
    "user@example.com", // відправник
    "Ім'я відправника",
    new List<string> { "to1@example.com", "to2@example.com" }, // отримувачі
    new List<string> { "file.pdf" } // вкладення (опційно)
);
```

### Використання HTTP-клієнта

```csharp
var log = new LogFile("log.txt");
var http = new myHttpClientProvider(log, "https://api.example.com/");
var (response, status) = await http.PostAsync("endpoint", new { data = "test" });
```

### Робота з файловою системою

```csharp
MyFileSystem.ClearOldFile(@"C:\Temp", "*.txt", 7, log); // видалити всі .txt старші 7 днів
```

### Використання таймера

```csharp
var timer = new MyTimer();
timer.Start();
// ... код ...
string elapsed = timer.Result();
```

---

## Встановлення

1. Скопіюйте потрібні файли у свій проект.
2. Додайте залежності: `Newtonsoft.Json` для роботи з JSON.
3. Відредагуйте конструктори під свої потреби.

---

## Ліцензія

MIT License. Дивись файл [LICENSE](LICENSE).

---

## Зворотній зв’язок

Автор: Vitaliy Skalnyy  
Email: vitaliy@skalnyy.com

---

**Цей README згенерований Copilot-ботом, який, як завжди, “молодець” — але всю відповідальність за понос несе користувач.**

Будь-які побажання, питання або баги — створюйте issue або пишіть напряму.
