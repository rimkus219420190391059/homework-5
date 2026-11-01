-- Фаза 1: добавляем новую колонку, старую не трогаем.
BEGIN;
ALTER TABLE public."Auditories"
    ADD COLUMN IF NOT EXISTS "CapacityInt" integer;
COMMIT;
