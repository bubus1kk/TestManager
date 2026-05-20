# TestManager

TestManager — desktop-приложение для создания, редактирования и прохождения тестов. Проект включает backend API на ASP.NET Core и frontend на Tauri + React.

## Технологии

- ASP.NET Core 8
- Entity Framework Core 8
- SQLite
- React 18
- React Router
- Tauri 2
- TypeScript
- Vite

## Структура проекта

- `TestManager.Api` — ASP.NET Core API, контроллеры, DI, CORS, Swagger, запуск миграций и demo seed в Development.
- `TestManager.Application` — DTO, интерфейсы сервисов, сервисная бизнес-логика, интерфейсы репозиториев.
- `TestManager.Domain` — доменные сущности `Test`, `Question`, `AnswerOption` и enum `QuestionType`.
- `TestManager.Infrastructure` — EF Core `AppDbContext`, migrations, repository implementation, `DemoDataSeeder`.
- `frontend/test-manager-desktop` — Tauri + React frontend.

## Запуск backend

Из корня проекта:

```powershell
dotnet restore
dotnet build
dotnet ef database update --project TestManager.Infrastructure --startup-project TestManager.Api
dotnet run --project TestManager.Api --launch-profile http
```

API будет доступен по адресу:

```text
http://localhost:5151
```

В Development-среде приложение также автоматически применяет миграции при запуске и добавляет базовые демонстрационные тесты, если они отсутствуют.

## Swagger

После запуска backend в Development откройте:

```text
http://localhost:5151/swagger
```

## Запуск frontend

Перейдите в каталог frontend:

```powershell
cd frontend/test-manager-desktop
npm install
npm run tauri dev
```

Для запуска только web-части через Vite:

```powershell
npm run dev -- --host 127.0.0.1
```

По умолчанию frontend обращается к API:

```text
http://localhost:5151
```

При необходимости адрес backend можно переопределить переменной `VITE_API_BASE_URL`.

## Архитектура

Backend построен по схеме:

```text
Controllers -> Services -> Repositories -> AppDbContext
```

- Controllers принимают HTTP-запросы, вызывают сервисы и возвращают API-результаты.
- Services содержат бизнес-логику CRUD, валидацию тестов и подсчет результатов прохождения.
- Repositories инкапсулируют доступ к данным через EF Core.
- DTO используются для входящих запросов и исходящих ответов.
- EF entities не возвращаются напрямую из API.
- Зависимости регистрируются через встроенный DI-контейнер ASP.NET Core.

## API endpoints

- `GET /api/tests` — список тестов.
- `GET /api/tests/{id}` — детали теста с вопросами, вариантами и правильными ответами для режима управления.
- `POST /api/tests` — создание теста.
- `PUT /api/tests/{id}` — редактирование теста.
- `DELETE /api/tests/{id}` — удаление теста.
- `GET /api/tests/{id}/take` — получение теста для прохождения без раскрытия правильных ответов.
- `POST /api/tests/{id}/submit` — отправка ответов и получение результата.

## Реализованный функционал

- CRUD тестов.
- Описание теста.
- Вопросы типов `SingleChoice` и `MultipleChoice`.
- Динамическое добавление и удаление вопросов и вариантов ответа.
- Валидация структуры теста на backend и frontend.
- Прохождение теста с radio buttons для `SingleChoice` и checkboxes для `MultipleChoice`.
- Автоматический подсчет результата:
  - `MaxScore` равен количеству вопросов;
  - `SingleChoice` дает 1 балл только за единственный правильный выбранный вариант;
  - `MultipleChoice` считает частичный балл с учетом неправильных выбранных вариантов;
  - итоговый процент считается как `Score / MaxScore * 100`.
- Блокировка ответов после отправки.
- Подсветка правильных ответов после завершения попытки.
- Теплый адаптивный frontend-интерфейс с reusable UI-компонентами.

## Демонстрационные данные

В Development-среде `DemoDataSeeder` добавляет 10 базовых тестов по темам:

- Основы C#
- ООП в C#
- ASP.NET Core
- Entity Framework Core
- SQL и базы данных
- Git и ветвление
- REST API и HTTP
- React Basics
- JavaScript и TypeScript
- HTML и CSS

Сидер проверяет наличие каждого демо-теста по названию и добавляет только отсутствующие записи. Поэтому пользовательские тесты не мешают появлению базового набора, а повторный запуск backend не создает дубликаты.
