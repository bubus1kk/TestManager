using Microsoft.EntityFrameworkCore;
using TestManager.Domain.Entities;

namespace TestManager.Infrastructure.Persistence;

public static class DemoDataSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var existingTitles = (await dbContext.Tests
            .Select(test => test.Title)
            .ToListAsync(cancellationToken))
            .ToHashSet();

        var missingDemoTests = CreateTests()
            .Where(test => !existingTitles.Contains(test.Title))
            .ToList();

        if (missingDemoTests.Count == 0)
        {
            return;
        }

        await dbContext.Tests.AddRangeAsync(missingDemoTests, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IReadOnlyCollection<Test> CreateTests()
    {
        return new[]
        {
            Test(
                "Основы C#: синтаксис и типы",
                "Короткая проверка базовых конструкций C#: типы данных, переменные, условия и коллекции.",
                Q("Какой тип в C# лучше всего подходит для хранения значения true или false?", QuestionType.SingleChoice,
                    A("bool", true), A("int"), A("string"), A("decimal")),
                Q("Какие утверждения о nullable reference types верны?", QuestionType.MultipleChoice,
                    A("Они помогают компилятору предупреждать о возможных null-значениях", true),
                    A("Они полностью запрещают null во время выполнения"),
                    A("Они включаются настройкой Nullable в проекте", true),
                    A("Они работают только для value types")),
                Q("Что делает оператор ?? в C#?", QuestionType.SingleChoice,
                    A("Возвращает левый операнд, если он не null, иначе правый", true),
                    A("Сравнивает два объекта по ссылке"),
                    A("Преобразует строку в число"),
                    A("Создает nullable value type")),
                Q("Какие коллекции входят в стандартные generic-коллекции .NET?", QuestionType.MultipleChoice,
                    A("List<T>", true), A("Dictionary<TKey, TValue>", true), A("ArrayList"), A("HashSet<T>", true))),

            Test(
                "ООП в C#: классы, интерфейсы, наследование",
                "Тест помогает проверить понимание инкапсуляции, полиморфизма и контрактов в C#.",
                Q("Что лучше всего описывает инкапсуляцию?", QuestionType.SingleChoice,
                    A("Сокрытие внутреннего состояния и управление доступом через методы и свойства", true),
                    A("Автоматическое создание объектов через DI"),
                    A("Наследование всех методов базового класса без изменений"),
                    A("Преобразование объекта в JSON")),
                Q("Какие элементы может содержать интерфейс в современных версиях C#?", QuestionType.MultipleChoice,
                    A("Сигнатуры методов", true), A("Свойства", true), A("Default implementation методов", true), A("Поля экземпляра")),
                Q("Для чего используется ключевое слово virtual?", QuestionType.SingleChoice,
                    A("Чтобы разрешить переопределение члена в наследнике", true),
                    A("Чтобы запретить наследование класса"),
                    A("Чтобы создать статический метод"),
                    A("Чтобы сделать поле обязательным")),
                Q("Какие утверждения про abstract class верны?", QuestionType.MultipleChoice,
                    A("Может содержать реализованные методы", true), A("Может содержать abstract-методы", true),
                    A("От нее нельзя создать экземпляр напрямую", true), A("Класс может наследоваться от нескольких abstract classes")),
                Q("Что произойдет при вызове override-метода через ссылку базового типа?", QuestionType.SingleChoice,
                    A("Будет вызвана реализация фактического типа объекта", true),
                    A("Всегда будет вызвана реализация базового класса"),
                    A("Компилятор запретит такой вызов"),
                    A("Метод станет static"))),

            Test(
                "ASP.NET Core: основы веб-приложений",
                "Практичный набор вопросов по middleware, DI, routing и конфигурации ASP.NET Core.",
                Q("Что такое middleware в ASP.NET Core?", QuestionType.SingleChoice,
                    A("Компонент конвейера обработки HTTP-запросов", true),
                    A("Тип Entity Framework migration"),
                    A("Способ описать SQL-индекс"),
                    A("Файл настроек frontend-сборки")),
                Q("Какие lifetime доступны во встроенном DI-контейнере?", QuestionType.MultipleChoice,
                    A("Singleton", true), A("Scoped", true), A("Transient", true), A("PerThread")),
                Q("Что обычно регистрирует AddControllers()?", QuestionType.SingleChoice,
                    A("Сервисы MVC для работы controller-based API", true),
                    A("SQLite database provider"),
                    A("React Router routes"),
                    A("Git hooks")),
                Q("Какие источники конфигурации типично поддерживаются ASP.NET Core?", QuestionType.MultipleChoice,
                    A("appsettings.json", true), A("Переменные окружения", true), A("User secrets в Development", true), A("CSS-файлы")),
                Q("Для чего нужен app.Environment.IsDevelopment()?", QuestionType.SingleChoice,
                    A("Чтобы включать поведение только для Development-среды", true),
                    A("Чтобы автоматически создать production-сертификат"),
                    A("Чтобы отключить маршрутизацию"),
                    A("Чтобы изменить тип всех DbSet")),
                Q("Какие утверждения о routing верны?", QuestionType.MultipleChoice,
                    A("Атрибут [HttpGet(\"{id:int}\")] задает route constraint", true),
                    A("[Route(\"api/[controller]\")] использует имя контроллера без суффикса Controller", true),
                    A("Маршруты всегда должны храниться в appsettings.json"),
                    A("MapControllers() подключает attribute routing контроллеров", true))),

            Test(
                "Entity Framework Core: модели и запросы",
                "Проверка знаний EF Core: DbContext, связи, миграции, tracking и Fluent API.",
                Q("Что представляет DbContext?", QuestionType.SingleChoice,
                    A("Единицу работы с базой и точку доступа к DbSet", true),
                    A("HTTP middleware для JSON"),
                    A("Файл миграции frontend-проекта"),
                    A("Git branch policy")),
                Q("Какие операции обычно выполняют миграции EF Core?", QuestionType.MultipleChoice,
                    A("Создают таблицы", true), A("Добавляют колонки", true), A("Создают индексы", true), A("Компилируют TypeScript")),
                Q("Что делает AsNoTracking()?", QuestionType.SingleChoice,
                    A("Отключает отслеживание сущностей для запроса", true),
                    A("Удаляет все foreign keys"),
                    A("Включает lazy loading"),
                    A("Автоматически применяет миграции")),
                Q("Какие способы настройки модели доступны в EF Core?", QuestionType.MultipleChoice,
                    A("Data Annotations", true), A("Fluent API", true), A("OnModelCreating", true), A("CSS variables")),
                Q("Что означает Cascade Delete для связи Test -> Questions?", QuestionType.SingleChoice,
                    A("При удалении теста связанные вопросы тоже удаляются", true),
                    A("Вопросы переносятся в другую таблицу"),
                    A("Удаление теста становится невозможным"),
                    A("Удаляется только первая строка")),
                Q("Какие методы помогают загрузить связанные данные?", QuestionType.MultipleChoice,
                    A("Include", true), A("ThenInclude", true), A("Select projection", true), A("Console.WriteLine")),
                Q("Что произойдет при SaveChangesAsync()?", QuestionType.SingleChoice,
                    A("EF отправит накопленные изменения в базу данных", true),
                    A("Будет создан новый DbContext"),
                    A("Все запросы станут NoTracking"),
                    A("Будет выполнен npm build"))),

            Test(
                "SQL и базы данных: запросы и связи",
                "Тест по базовым и средним темам SQL: JOIN, индексы, ограничения и транзакции.",
                Q("Какой JOIN возвращает только совпавшие строки из обеих таблиц?", QuestionType.SingleChoice,
                    A("INNER JOIN", true), A("LEFT JOIN"), A("FULL OUTER JOIN"), A("CROSS JOIN")),
                Q("Какие ограничения помогают поддерживать целостность данных?", QuestionType.MultipleChoice,
                    A("PRIMARY KEY", true), A("FOREIGN KEY", true), A("UNIQUE", true), A("ORDER BY")),
                Q("Для чего обычно используют индекс?", QuestionType.SingleChoice,
                    A("Для ускорения поиска и сортировки по выбранным колонкам", true),
                    A("Для шифрования всех строк"),
                    A("Для замены SELECT"),
                    A("Для хранения резервной копии")),
                Q("Какие команды относятся к DML?", QuestionType.MultipleChoice,
                    A("SELECT", true), A("INSERT", true), A("UPDATE", true), A("ALTER TABLE")),
                Q("Что гарантирует транзакция при успешном COMMIT?", QuestionType.SingleChoice,
                    A("Изменения становятся постоянными", true),
                    A("Все индексы удаляются"),
                    A("Запросы перестают блокироваться"),
                    A("Схема базы откатывается")),
                Q("Какие утверждения про нормализацию верны?", QuestionType.MultipleChoice,
                    A("Она уменьшает дублирование данных", true), A("Она помогает управлять связями", true),
                    A("Она всегда ускоряет любые запросы"), A("Она может потребовать JOIN в запросах", true)),
                Q("Какой оператор фильтрует группы после GROUP BY?", QuestionType.SingleChoice,
                    A("HAVING", true), A("WHERE"), A("ORDER BY"), A("LIMIT")),
                Q("Какие типы связей часто моделируются в реляционных БД?", QuestionType.MultipleChoice,
                    A("Один-ко-многим", true), A("Многие-ко-многим", true), A("Один-к-одному", true), A("CSS-to-HTML"))),

            Test(
                "Git и ветвление: рабочий процесс",
                "Набор вопросов о ветках, merge, rebase, pull requests и безопасной работе с историей.",
                Q("Что делает git status?", QuestionType.SingleChoice,
                    A("Показывает состояние рабочей директории и индекса", true),
                    A("Создает новый commit"),
                    A("Удаляет remote branch"),
                    A("Переписывает историю")),
                Q("Какие команды могут изменить историю commit'ов?", QuestionType.MultipleChoice,
                    A("git rebase", true), A("git commit --amend", true), A("git reset", true), A("git status")),
                Q("Что такое fast-forward merge?", QuestionType.SingleChoice,
                    A("Перемещение указателя ветки без создания merge commit", true),
                    A("Удаление всех конфликтов автоматически"),
                    A("Слияние только бинарных файлов"),
                    A("Создание tag после каждого commit")),
                Q("Какие файлы обычно не стоит коммитить?", QuestionType.MultipleChoice,
                    A("bin/ и obj/", true), A("node_modules/", true), A("локальные базы и логи", true), A("исходный код приложения")),
                Q("Для чего нужен pull request?", QuestionType.SingleChoice,
                    A("Для обсуждения и проверки изменений перед слиянием", true),
                    A("Для локального удаления ветки"),
                    A("Для запуска SQLite"),
                    A("Для установки npm-пакетов")),
                Q("Какие действия помогают безопасно решить конфликт?", QuestionType.MultipleChoice,
                    A("Понять обе версии изменений", true), A("Запустить тесты после исправления", true),
                    A("Удалить чужие изменения без проверки"), A("Сохранить итоговый файл без конфликтных маркеров", true)),
                Q("Что делает git fetch?", QuestionType.SingleChoice,
                    A("Получает данные с remote без автоматического слияния", true),
                    A("Всегда изменяет рабочую директорию"),
                    A("Создает новую локальную ветку с commit"),
                    A("Удаляет ignored files")),
                Q("Какие утверждения про .gitignore верны?", QuestionType.MultipleChoice,
                    A("Он помогает не добавлять сгенерированные файлы", true),
                    A("Он не удаляет уже отслеживаемые файлы автоматически", true),
                    A("Он может содержать шаблоны путей", true),
                    A("Он хранит пароли GitHub")),
                Q("Что обычно означает detached HEAD?", QuestionType.SingleChoice,
                    A("HEAD указывает на commit напрямую, а не на ветку", true),
                    A("Репозиторий потерял remote origin"),
                    A("Все файлы стали ignored"),
                    A("Merge conflict автоматически решен"))),

            Test(
                "REST API и HTTP: контракты и статусы",
                "Проверка понимания HTTP-методов, кодов ответов, REST-ресурсов и JSON-контрактов.",
                Q("Какой HTTP-метод обычно используют для получения ресурса?", QuestionType.SingleChoice,
                    A("GET", true), A("POST"), A("PATCH"), A("DELETE")),
                Q("Какие коды ответов относятся к успешным?", QuestionType.MultipleChoice,
                    A("200 OK", true), A("201 Created", true), A("204 No Content", true), A("404 Not Found")),
                Q("Что означает 400 Bad Request?", QuestionType.SingleChoice,
                    A("Клиент отправил некорректный запрос", true),
                    A("Сервер успешно создал ресурс"),
                    A("Пользователь всегда не авторизован"),
                    A("Ресурс был удален")),
                Q("Какие заголовки часто важны для JSON API?", QuestionType.MultipleChoice,
                    A("Content-Type", true), A("Accept", true), A("Authorization", true), A("font-family")),
                Q("Что лучше вернуть после успешного DELETE без тела ответа?", QuestionType.SingleChoice,
                    A("204 No Content", true), A("500 Internal Server Error"), A("302 Found"), A("418 I'm a teapot")),
                Q("Какие свойства характерны для хорошего REST endpoint?", QuestionType.MultipleChoice,
                    A("Понятный ресурсный URL", true), A("Предсказуемые HTTP-статусы", true),
                    A("Стабильный JSON-контракт", true), A("Случайные названия полей")),
                Q("Для чего нужен CORS?", QuestionType.SingleChoice,
                    A("Чтобы браузер разрешил запросы между разными origins по правилам сервера", true),
                    A("Чтобы база данных создала индекс"),
                    A("Чтобы Git объединил ветки"),
                    A("Чтобы TypeScript стал JavaScript")),
                Q("Какие методы могут изменять состояние ресурса?", QuestionType.MultipleChoice,
                    A("POST", true), A("PUT", true), A("PATCH", true), A("GET")),
                Q("Что обычно содержит DTO в API?", QuestionType.SingleChoice,
                    A("Данные контракта, которые можно безопасно отдать клиенту", true),
                    A("Полный DbContext"),
                    A("Секреты подключения к базе"),
                    A("Скомпилированный CSS")),
                Q("Какие причины использовать отдельный endpoint /take для теста?", QuestionType.MultipleChoice,
                    A("Не отдавать правильные ответы пользователю", true),
                    A("Сформировать контракт под прохождение", true),
                    A("Скрыть лишние поля редактирования", true),
                    A("Отключить routing"))),

            Test(
                "React Basics: компоненты и состояние",
                "Короткий тест по базовым концепциям React: компоненты, props, state и события.",
                Q("Что такое React component?", QuestionType.SingleChoice,
                    A("Функция или класс, возвращающие UI-описание", true),
                    A("SQL-запрос для создания таблицы"),
                    A("HTTP status code"),
                    A("Git hook")),
                Q("Какие хуки относятся к базовым React hooks?", QuestionType.MultipleChoice,
                    A("useState", true), A("useEffect", true), A("useMemo", true), A("useDatabase")),
                Q("Для чего нужен key при рендеринге списка?", QuestionType.SingleChoice,
                    A("Чтобы React стабильнее сопоставлял элементы списка", true),
                    A("Чтобы зашифровать props"),
                    A("Чтобы отправить POST-запрос"),
                    A("Чтобы создать CSS class"))),

            Test(
                "JavaScript и TypeScript: язык и типы",
                "Вопросы о современном JavaScript, TypeScript-типах, async-коде и распространенных нюансах.",
                Q("Что возвращает async function без явного Promise в коде?", QuestionType.SingleChoice,
                    A("Promise", true), A("Observable"), A("string"), A("void всегда")),
                Q("Какие значения считаются falsy в JavaScript?", QuestionType.MultipleChoice,
                    A("false", true), A("0", true), A("''", true), A("'false'")),
                Q("Что дает TypeScript interface?", QuestionType.SingleChoice,
                    A("Описание формы объекта на этапе проверки типов", true),
                    A("Автоматическую проверку типов в runtime"),
                    A("Создание HTML-формы"),
                    A("SQL-транзакцию")),
                Q("Какие способы помогают безопасно работать с null/undefined?", QuestionType.MultipleChoice,
                    A("Optional chaining", true), A("Nullish coalescing", true), A("Явные проверки", true), A("eval")),
                Q("Что делает оператор ===?", QuestionType.SingleChoice,
                    A("Сравнивает без приведения типов", true),
                    A("Всегда сравнивает только ссылки"),
                    A("Присваивает значение"),
                    A("Создает новый объект")),
                Q("Какие утверждения о union types верны?", QuestionType.MultipleChoice,
                    A("Тип может быть одним из нескольких вариантов", true),
                    A("Их удобно сужать проверками", true), A("Они существуют только в CSS"),
                    A("Они помогают описывать разные состояния", true))),

            Test(
                "HTML и CSS: структура и адаптивность",
                "Тест проверяет семантику HTML, базовую доступность, layout и адаптивные CSS-приемы.",
                Q("Какой тег лучше использовать для основной навигации?", QuestionType.SingleChoice,
                    A("nav", true), A("div всегда"), A("span"), A("code")),
                Q("Какие CSS-свойства часто применяются для адаптивных сеток?", QuestionType.MultipleChoice,
                    A("display: grid", true), A("minmax()", true), A("flex-wrap", true), A("DROP TABLE")),
                Q("Что делает box-sizing: border-box?", QuestionType.SingleChoice,
                    A("Включает padding и border в заданную ширину элемента", true),
                    A("Скрывает элемент"),
                    A("Запрещает перенос текста"),
                    A("Создает CSS-переменную")),
                Q("Какие приемы помогают не ломать верстку длинным текстом?", QuestionType.MultipleChoice,
                    A("overflow-wrap: anywhere", true), A("min-width: 0 в grid/flex", true),
                    A("word-break: break-word", true), A("Всегда задавать width: 5000px")),
                Q("Какой атрибут связывает label с input?", QuestionType.SingleChoice,
                    A("for", true), A("src"), A("href"), A("alt")),
                Q("Какие элементы считаются интерактивными?", QuestionType.MultipleChoice,
                    A("button", true), A("input", true), A("a с href", true), A("meta")),
                Q("Для чего нужны focus-visible состояния?", QuestionType.SingleChoice,
                    A("Чтобы пользователи клавиатуры видели текущий фокус", true),
                    A("Чтобы скрыть ошибки формы"),
                    A("Чтобы ускорить SQL-запросы"),
                    A("Чтобы удалить outline у всех элементов")),
                Q("Какие единицы часто подходят для адаптивной верстки?", QuestionType.MultipleChoice,
                    A("%", true), A("rem", true), A("fr", true), A("только px для всего"))),
        };
    }

    private static Test Test(string title, string description, params Question[] questions)
    {
        return new Test
        {
            Title = title,
            Description = description,
            Questions = questions.ToList()
        };
    }

    private static Question Q(string text, QuestionType type, params AnswerOption[] answerOptions)
    {
        return new Question
        {
            Text = text,
            Type = type,
            AnswerOptions = answerOptions.ToList()
        };
    }

    private static AnswerOption A(string text, bool isCorrect = false)
    {
        return new AnswerOption
        {
            Text = text,
            IsCorrect = isCorrect
        };
    }
}
