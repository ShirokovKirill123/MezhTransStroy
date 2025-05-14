CREATE DATABASE Строительство;
USE Строительство;

CREATE TABLE Отделы (
    id INT PRIMARY KEY IDENTITY,
    Название NVARCHAR(100) NOT NULL
);

CREATE TABLE Сотрудники (
    id INT PRIMARY KEY IDENTITY,
    ФИО NVARCHAR(255) NOT NULL,
    Должность NVARCHAR(100) NOT NULL,
    Квалификация NVARCHAR(100),
    Дата_Приёма DATE,
    Контакты NVARCHAR(100),
    id_Отдела INT,
    FOREIGN KEY (id_Отдела) REFERENCES Отделы(id)
);

CREATE TABLE Строительные_Объекты (
    id INT PRIMARY KEY IDENTITY,
    Название NVARCHAR(255) NOT NULL,
    Адрес NVARCHAR(255) NOT NULL,
    Дата_Начала DATE,
    Дата_Окончания DATE,
	Выделенный_Бюджет DECIMAL(15,2)
);

CREATE TABLE Материалы (
    id INT PRIMARY KEY IDENTITY,
    Название NVARCHAR(255) NOT NULL,
    Единица_Измерения NVARCHAR(50),
    Стоимость DECIMAL(15,2)
);

CREATE TABLE Оборудование (
    id INT PRIMARY KEY IDENTITY,
    Название NVARCHAR(100),
    Тип NVARCHAR(50) NULL
);

CREATE TABLE Поставщики (
    id INT PRIMARY KEY IDENTITY,
    Название NVARCHAR(255) NOT NULL,
    Контактное_Лицо NVARCHAR(255),
    Телефон NVARCHAR(100),
    Адрес NVARCHAR(255)
);

CREATE TABLE Склады (
    id INT PRIMARY KEY IDENTITY,
    Номер_Склада INT UNIQUE
);

CREATE TABLE Материалы_На_Складах (
    id INT PRIMARY KEY IDENTITY,
    id_Склада INT,
    id_Материала INT,
    Количество INT,
    Стоимость_Материалов DECIMAL(15,2),
    id_Поставщика INT,
    Дата_Поступления DATE,
    FOREIGN KEY (id_Склада) REFERENCES Склады(id),
    FOREIGN KEY (id_Материала) REFERENCES Материалы(id),
    FOREIGN KEY (id_Поставщика) REFERENCES Поставщики(id)
);

CREATE TABLE Заявки (
    id INT PRIMARY KEY IDENTITY,
    id_Объекта INT,
    id_Склада INT,  
    id_Поставщика INT,
    id_Материала INT,
    Количество_Материала INT,
    Стоимость_Материалов DECIMAL(15,2),
    Статус NVARCHAR(50) DEFAULT 'Ожидает обработки', --Обработано, --Уже на объекте
	Дата_Заявки DATE DEFAULT GETDATE(),
    FOREIGN KEY (id_Объекта) REFERENCES Строительные_Объекты(id),
    FOREIGN KEY (id_Склада) REFERENCES Склады(id),
    FOREIGN KEY (id_Поставщика) REFERENCES Поставщики(id),
    FOREIGN KEY (id_Материала) REFERENCES Материалы(id)
);

CREATE TABLE Распределение_Материалов_На_Объект (
    id INT PRIMARY KEY IDENTITY,
	id_Склада INT,
    id_Объекта INT,
    id_Материала INT,
    Количество INT,
	Стоимость_Материалов DECIMAL(15,2),
    Дата_Передачи DATE,
    FOREIGN KEY (id_Объекта) REFERENCES Строительные_Объекты(id),
	FOREIGN KEY (id_Склада) REFERENCES Склады(id),
    FOREIGN KEY (id_Материала) REFERENCES Материалы(id)
);

CREATE TABLE Работа_На_Объекте (
    id INT PRIMARY KEY IDENTITY,
    id_Сотрудника INT,
    id_Объекта INT,
    Дата_Назначения DATE,
	Статус NVARCHAR(50), --Не начат, В работе, Завершён, Отложен, Отменён
    FOREIGN KEY (id_Сотрудника) REFERENCES Сотрудники(id),
    FOREIGN KEY (id_Объекта) REFERENCES Строительные_Объекты(id)
);

CREATE TABLE Затраты_На_Оборудование (
    id INT PRIMARY KEY IDENTITY,
    id_Объекта INT,
    id_Оборудования INT,
    Часы_Работы INT,
    Стоимость_в_Час DECIMAL(10,2),
    Затраты AS (Часы_Работы * Стоимость_в_Час) PERSISTED,
    FOREIGN KEY (id_Объекта) REFERENCES Строительные_Объекты(id),
    FOREIGN KEY (id_Оборудования) REFERENCES Оборудование(id)
);

CREATE TABLE Заработная_Плата_Сотрудников (
    id INT PRIMARY KEY IDENTITY,
    id_Сотрудника INT,
    Ставка_в_День DECIMAL(10,2),
    Отработано_Дней INT,
    Затраты AS (Ставка_в_День * Отработано_Дней) PERSISTED,
    FOREIGN KEY (id_Сотрудника) REFERENCES Сотрудники(id)
);

CREATE TABLE Пользователи
(
  id INT PRIMARY KEY IDENTITY,
  Логин VARCHAR(255),
  Пароль VARCHAR(255),
  Уровень_Доступа VARCHAR(50)
);

-- Таблица для уведомлений
CREATE TABLE Уведомления (
    id INT PRIMARY KEY IDENTITY,
    Текст NVARCHAR(MAX) NOT NULL,
    Дата_Создания DATETIME DEFAULT GETDATE()
);

-- Таблица для истории перемещений материалов
CREATE TABLE История_Перемещений (
    id INT PRIMARY KEY IDENTITY,
	id_Заявки INT,
    id_Склада INT,
    id_Объекта INT,
    id_Материала INT,
    Количество INT,
    Дата_Перемещения DATETIME DEFAULT GETDATE(),
    Описание NVARCHAR(MAX),
    FOREIGN KEY (id_Склада) REFERENCES Склады(id),
    FOREIGN KEY (id_Объекта) REFERENCES Строительные_Объекты(id),
    FOREIGN KEY (id_Материала) REFERENCES Материалы(id),
	FOREIGN KEY (id_Заявки) REFERENCES Заявки(id)
);
