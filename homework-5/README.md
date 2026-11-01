# Homework 5 — Вариант 2: Бронирование аудиторий

Миграция `Auditories.Capacity`: PostgreSQL `smallint` → `integer` по Expand and Contract.

## Допущения
- Таблица `public."Auditories"`, PK `Id uuid`, поле `Capacity smallint`.
- API бронирования: `POST /api/v1/bookings`, `GET /api/v1/bookings/{id}`, `DELETE /api/v1/bookings/{id}`.
- Перед запуском сверьте имена таблицы, колонок и маршрутов с вашим проектом.

## Порядок
1. Резервная копия и проверка восстановления.
2. `sql/01_expand.sql`.
3. Развернуть dual-write версию.
4. `sql/02_migrate.sql`, проверить равенство колонок.
5. Переключить чтение на `CapacityInt`, обновить все экземпляры.
6. После периода наблюдения выполнить `sql/03_contract.sql`.
7. Запустить sanity-тест с отдельной тестовой аудиторией.

Sanity-тест выключен по умолчанию. Для запуска нужны `RUN_PRODUCTION_SANITY=true`, `SANITY_BASE_URL`, `SANITY_API_TOKEN`, `SANITY_AUDITORY_ID`. Не используйте реальные пользовательские данные.
