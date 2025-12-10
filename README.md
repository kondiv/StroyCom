# StroyCom
API на .NET 10, построенное по принципам **Vertical Slice Architecture**.

## Технологический стек

### Основные фреймворки
- **.NET 10** - LTS версия
- **ASP.NET Core** - Современный веб-фреймворк

### Архитектура и паттерны
- **Vertical Slice Architecture** - Архитектура вертикальных срезов вместо горизонтальных слоев
- **MediatR** - Идеально подходит для VSA, обработка каждого запроса как изолированной функциональности
- **FluentValidation** - Fluent валидация для проверки запросов
- **Ardalis.Result** - Стандартизированный паттерн результата для согласованных ответов API

### Данные и безопасность
- **Entity Framework Core** - ORM для доступа к данным
- **PostgreSQL** - База данных
- **JWT аутентификация** - Аутентификация на основе токенов с куками

## Архитектура Vertical Slice

Проект организован по функциональным вертикальным срезам, где каждый срез содержит всю логику для конкретной функциональности

Каждый срез включает:
- Command/Query (MediatR)
- Handler (бизнес-логика)
- Validator (FluentValidation, в основном для команд)

## Аутентификация

### Реализация JWT
- **Простой JWT flow** только с access токенами
- **Хранение токенов в куках** для веб-приложений
- **Ролевая модель** (Engineer, Manager, Admin)

## Доступные эндпоинты

### Аутентификация
- BaseUrl http://localhost:8080
- POST /api/v1/auth/sing-in
- POST /api/v1/auth/log-in

### Пользователи
- BaseUrl http://localhost:8080
- GET /api/v1/users/{id}:exists

### Заказы
- BaseUrl http://localhost:8081
- GET /api/v1/orders
- GET /api/v1/orders/{id}
- POST /api/v1/orders
- POST /api/v1/orders/{id}/status:change
- DELETE /api/v1/orders/{id}

### Через Gateway
- Аутентификация: https://localhost:8082/auth/{**catch-all}
- Заказы: https://localhost:8082/orders/{**catch-all}

## Unit тесты
Unit тесты реализованы для логики регистрации и авторизации
