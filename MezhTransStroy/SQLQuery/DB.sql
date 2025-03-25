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
    Дата_Окончания DATE
);


CREATE TABLE Материалы (
    id INT PRIMARY KEY IDENTITY,
    Название NVARCHAR(255) NOT NULL,
    Единица_Измерения NVARCHAR(50),
    Стоимость DECIMAL(15,2)
);

CREATE TABLE Поставщики (
    id INT PRIMARY KEY IDENTITY,
    Название NVARCHAR(255) NOT NULL,
    Контактное_Лицо NVARCHAR(255),
    Телефон NVARCHAR(100),
    Адрес NVARCHAR(255)
);

CREATE TABLE Склад (
    id INT PRIMARY KEY IDENTITY,
    id_Материала INT,
    Количество INT,
    id_Поставщика INT,
    Дата_Поступления DATE,
    FOREIGN KEY (id_Материала) REFERENCES Материалы(id),
    FOREIGN KEY (id_Поставщика) REFERENCES Поставщики(id)
);

CREATE TABLE Заявки (
    id INT PRIMARY KEY IDENTITY,
    id_Объекта INT,
    id_Материала INT,
    Количество INT,
    Статус NVARCHAR(50) DEFAULT 'Ожидает обработки',
    FOREIGN KEY (id_Объекта) REFERENCES Строительные_Объекты(id),
    FOREIGN KEY (id_Материала) REFERENCES Материалы(id)
);

CREATE TABLE Распределение_Материалов_На_Объект (
    id INT PRIMARY KEY IDENTITY,
    id_Объекта INT,
    id_Материала INT,
    Количество INT,
    Дата_Передачи DATE,
    FOREIGN KEY (id_Объекта) REFERENCES Строительные_Объекты(id),
    FOREIGN KEY (id_Материала) REFERENCES Материалы(id)
);

CREATE TABLE Работа_На_Объекте (
    id INT PRIMARY KEY IDENTITY,
    id_Сотрудника INT,
    id_Объекта INT,
    Дата_Назначения DATE,
    FOREIGN KEY (id_Сотрудника) REFERENCES Сотрудники(id),
    FOREIGN KEY (id_Объекта) REFERENCES Строительные_Объекты(id)
);

CREATE TABLE Пользователи
(
  id INT PRIMARY KEY IDENTITY,
  Логин VARCHAR(255),
  Пароль VARCHAR(255),
  Уровень_Доступа VARCHAR(50)
);