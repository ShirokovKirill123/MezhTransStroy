UPDATE Затраты_На_Оборудование
SET Затраты = z.Часы_Работы * o.Стоимость_в_Час
FROM Затраты_На_Оборудование z
JOIN Оборудование o ON z.id_Оборудования = o.id;

SELECT * FROM Сотрудники;
SELECT * FROM Отделы;
SELECT * FROM Строительные_Объекты;
SELECT * FROM Материалы;
SELECT * FROM Оборудование;
SELECT * FROM Оборудование_На_Складах;
SELECT * FROM Оборудование;
SELECT * FROM Поставщики;
SELECT * FROM Склады;
SELECT * FROM Материалы_На_Складах;
SELECT * FROM Заявки;
SELECT * FROM Распределение_Материалов_На_Объект;
SELECT * FROM Распределение_Оборудования_На_Объект;
SELECT * FROM Работа_На_Объекте;
SELECT * FROM Затраты_На_Оборудование;
SELECT * FROM Заработная_Плата_Сотрудников;

SELECT * FROM Пользователи;
SELECT * FROM Уведомления;
SELECT * FROM История_Перемещений_Материалов;
SELECT * FROM История_Перемещений_Оборудования;