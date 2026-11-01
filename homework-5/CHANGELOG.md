# Changelog

Формат: [Keep a Changelog](https://keepachangelog.com/ru/1.1.0/). Версионирование: [Semantic Versioning](https://semver.org/lang/ru/).

## [1.9.0] - 2026-11-01

### Added
- Добавлена `Auditories.CapacityInt` типа PostgreSQL `integer`.
- Добавлена двойная запись в старую и новую колонки на переходном этапе.
- Добавлены OpenTelemetry-теги `jira.issue` и `campus.auditory_id`.
- Добавлен sanity-тест создания и очистки тестовой брони.

### Fixed
- Устранено ограничение `smallint` после завершения Contract.

### Breaking Changes
- После Contract `Capacity` имеет тип `integer`; старые клиенты, ожидающие `smallint`, должны быть выведены из эксплуатации до Contract.
