-- Фаза 3. Выполнять только после обновления всех клиентов,
-- переключения чтения на CapacityInt и успешной сверки данных.
BEGIN;
ALTER TABLE public."Auditories"
    ALTER COLUMN "CapacityInt" SET NOT NULL;
ALTER TABLE public."Auditories"
    DROP COLUMN "Capacity";
ALTER TABLE public."Auditories"
    RENAME COLUMN "CapacityInt" TO "Capacity";
COMMIT;

-- Итоговая модель EF Core: public int Capacity { get; set; }
