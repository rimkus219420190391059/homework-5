-- Фаза 2: перенос исторических значений и проверка целостности.
BEGIN;
UPDATE public."Auditories"
SET "CapacityInt" = "Capacity"::integer
WHERE "CapacityInt" IS NULL;

DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM public."Auditories"
        WHERE "CapacityInt" IS DISTINCT FROM "Capacity"::integer
    ) THEN
        RAISE EXCEPTION 'Capacity migration validation failed';
    END IF;
END $$;
COMMIT;
