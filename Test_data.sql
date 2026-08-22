-- 1. Роли
INSERT INTO "Roles" ("RoleId", "Name", "Description")
VALUES 
    ('11111111-1111-1111-1111-111111111111', 'Administrator', 'Полный доступ'),
    ('22222222-2222-2222-2222-222222222222', 'HR', 'Управление сотрудниками и отпусками'),
    ('33333333-3333-3333-3333-333333333333', 'Manager', 'Управление подчинёнными'),
    ('44444444-4444-4444-4444-444444444444', 'Employee', 'Сотрудник')
ON CONFLICT ("RoleId") DO NOTHING;

-- 2. Должности
INSERT INTO "Positions" ("Id", "Name", "Description")
VALUES 
    (1, 'Software Engineer', 'Разработчик ПО'),
    (2, 'Senior Software Engineer', 'Старший разработчик'),
    (3, 'Team Lead', 'Руководитель команды'),
    (4, 'Project Manager', 'Менеджер проекта'),
    (5, 'HR Manager', 'Менеджер по персоналу'),
    (6, 'QA Engineer', 'Инженер по тестированию')
ON CONFLICT ("Id") DO NOTHING;

-- 3. Глобальные настройки отпусков (уже есть в миграциях)
INSERT INTO "GlobalVacationSettings" ("Id", "DefaultVacationDays")
VALUES (1, 20)
ON CONFLICT ("Id") DO NOTHING;

-- 4. Настройки отпусков по должностям
INSERT INTO "PositionVacationSettings" ("Id", "PositionId", "VacationDays")
VALUES 
    (1, 1, 20),  -- Software Engineer: 20 дней
    (2, 2, 25),  -- Senior: 25 дней
    (3, 3, 28),  -- Team Lead: 28 дней
    (4, 4, 28),  -- Project Manager: 28 дней
    (5, 5, 30),  -- HR Manager: 30 дней
    (6, 6, 20)   -- QA: 20 дней
ON CONFLICT ("Id") DO NOTHING;

-- 5. Пользователи (с хешами паролей, все пароли = 'Test123!')
INSERT INTO "Users" ("UserId", "Email", "PasswordHash", "FirstName", "LastName", 
                     "RegistrationDate", "IsActive", "RoleId", "PositionId")
VALUES 
    -- Администратор (уже есть в миграциях)
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 
     'admin@vacationplanner.local',
     '$2a$11$lvVJUGOZBJ7qt8LWxHzPw.UX5v3wWvi/Hu5wY42j/Ixq2SlxrMk5m',
     'System', 'Administrator',
     '2026-01-01 00:00:00+00', true,
     '11111111-1111-1111-1111-111111111111', NULL),
     
    -- HR-менеджер
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
     'hr@vacationplanner.local',
     '$2a$11$lvVJUGOZBJ7qt8LWxHzPw.UX5v3wWvi/Hu5wY42j/Ixq2SlxrMk5m',
     'Anna', 'Ivanova',
     '2026-02-01 10:00:00+00', true,
     '22222222-2222-2222-2222-222222222222', 5),
     
    -- Менеджер
    ('cccccccc-cccc-cccc-cccc-cccccccccccc',
     'manager@vacationplanner.local',
     '$2a$11$lvVJUGOZBJ7qt8LWxHzPw.UX5v3wWvi/Hu5wY42j/Ixq2SlxrMk5m',
     'Petr', 'Petrov',
     '2026-03-01 10:00:00+00', true,
     '33333333-3333-3333-3333-333333333333', 3),
     
    -- Сотрудник 1
    ('dddddddd-dddd-dddd-dddd-dddddddddddd',
     'ivan.s@vacationplanner.local',
     '$2a$11$lvVJUGOZBJ7qt8LWxHzPw.UX5v3wWvi/Hu5wY42j/Ixq2SlxrMk5m',
     'Ivan', 'Sidorov',
     '2026-04-01 10:00:00+00', true,
     '44444444-4444-4444-4444-444444444444', 1),
     
    -- Сотрудник 2
    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee',
     'maria.k@vacationplanner.local',
     '$2a$11$lvVJUGOZBJ7qt8LWxHzPw.UX5v3wWvi/Hu5wY42j/Ixq2SlxrMk5m',
     'Maria', 'Kozlova',
     '2026-05-01 10:00:00+00', true,
     '44444444-4444-4444-4444-444444444444', 2),
     
    -- Сотрудник 3 (неактивный)
    ('ffffffff-ffff-ffff-ffff-ffffffffffff',
     'alexey.n@vacationplanner.local',
     '$2a$11$lvVJUGOZBJ7qt8LWxHzPw.UX5v3wWvi/Hu5wY42j/Ixq2SlxrMk5m',
     'Alexey', 'Novikov',
     '2026-06-01 10:00:00+00', false,
     '44444444-4444-4444-4444-444444444444', 6)
ON CONFLICT ("UserId") DO UPDATE SET
    "PasswordHash" = EXCLUDED."PasswordHash",
    "FirstName" = EXCLUDED."FirstName",
    "LastName" = EXCLUDED."LastName",
    "IsActive" = EXCLUDED."IsActive",
    "PositionId" = EXCLUDED."PositionId";

-- 6. Заявки на отпуск
INSERT INTO "VacationRequests" ("VacationRequestId", "UserId", "Reason", 
                                "DateFrom", "DateTo", "Status", 
                                "CreatedAt", "UpdatedAt", "Comment")
VALUES 
    -- Заявка Ивана (одобрена)
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaab01',
     'dddddddd-dddd-dddd-dddd-dddddddddddd',
     'Ежегодный оплачиваемый отпуск',
     '2026-07-01 00:00:00+00', '2026-07-14 00:00:00+00',
     2,  -- Status = Approved
     '2026-06-01 09:00:00+00', '2026-06-05 14:00:00+00',
     'Согласовано'),
     
    -- Заявка Марии (в обработке)
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaab02',
     'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee',
     'Отпуск за свой счёт',
     '2026-08-01 00:00:00+00', '2026-08-05 00:00:00+00',
     1,  -- Status = Pending
     '2026-07-10 11:00:00+00', NULL,
     NULL),
     
    -- Заявка Ивана (отклонена)
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaab03',
     'dddddddd-dddd-dddd-dddd-dddddddddddd',
     'Дополнительный отпуск',
     '2026-09-01 00:00:00+00', '2026-09-03 00:00:00+00',
     3,  -- Status = Rejected
     '2026-08-01 10:00:00+00', '2026-08-03 16:00:00+00',
     'Отказано: недостаточно дней'),
     
    -- Заявка Алексея (неактивный пользователь, но заявка есть)
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaab04',
     'ffffffff-ffff-ffff-ffff-ffffffffffff',
     'Ежегодный отпуск',
     '2026-10-01 00:00:00+00', '2026-10-15 00:00:00+00',
     1,  -- Status = Pending
     '2026-09-01 08:00:00+00', NULL,
     NULL)
ON CONFLICT ("VacationRequestId") DO NOTHING;

-- 7. Согласования заявок
INSERT INTO "VacationApprovals" ("VacationApprovalId", "VacationRequestId", 
                                 "ApprovalStage", "ApproverUserId", 
                                 "Decision", "Comment", "DecidedAt")
VALUES 
    -- Согласование заявки Ивана (одобрено менеджером)
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaac01',
     'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaab01',
     1,  -- Stage 1
     'cccccccc-cccc-cccc-cccc-cccccccccccc',
     1,  -- Decision = Approved
     'Одобряю',
     '2026-06-05 14:00:00+00'),
     
    -- Согласование заявки Ивана (одобрено HR)
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaac02',
     'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaab01',
     2,  -- Stage 2
     'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
     1,  -- Decision = Approved
     'Одобрено',
     '2026-06-05 16:00:00+00'),
     
    -- Согласование заявки Ивана (отклонено)
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaac03',
     'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaab03',
     1,  -- Stage 1
     'cccccccc-cccc-cccc-cccc-cccccccccccc',
     2,  -- Decision = Rejected
     'Недостаточно дней',
     '2026-08-03 16:00:00+00')
ON CONFLICT ("VacationApprovalId") DO NOTHING;

-- 8. Фактические отпуска
INSERT INTO "Vacations" ("VacationId", "UserId", "VacationRequestId",
                         "DateFrom", "DateTo", "VacationType", 
                         "IsPaid", "CreatedAt")
VALUES 
    -- Отпуск Ивана (по заявке)
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaad01',
     'dddddddd-dddd-dddd-dddd-dddddddddddd',
     'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaab01',
     '2026-07-01 00:00:00+00', '2026-07-14 00:00:00+00',
     'Annual', true,
     '2026-06-05 17:00:00+00'),
     
    -- Отпуск Марии (ещё не подтверждён, без заявки)
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaad02',
     'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee',
     NULL,
     '2026-08-01 00:00:00+00', '2026-08-05 00:00:00+00',
     'Unpaid', false,
     '2026-07-10 12:00:00+00')
ON CONFLICT ("VacationId") DO NOTHING;