-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Generation Time: דצמבר 03, 2025 בזמן 07:19 PM
-- גרסת שרת: 10.4.32-MariaDB
-- PHP Version: 8.0.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `tsm_main`
--

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `aspnetusers`
--

CREATE TABLE `aspnetusers` (
  `Id` varchar(255) NOT NULL,
  `UserName` varchar(256) DEFAULT NULL,
  `NormalizedUserName` varchar(256) DEFAULT NULL,
  `Email` varchar(256) DEFAULT NULL,
  `NormalizedEmail` varchar(256) DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext DEFAULT NULL,
  `SecurityStamp` longtext DEFAULT NULL,
  `ConcurrencyStamp` longtext DEFAULT NULL,
  `PhoneNumber` longtext DEFAULT NULL,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime(6) DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int(11) NOT NULL,
  `FirstName` varchar(100) DEFAULT NULL,
  `LastName` varchar(100) DEFAULT NULL,
  `Address` varchar(255) DEFAULT NULL,
  `CompanyName` varchar(255) DEFAULT NULL,
  `BirthDate` date DEFAULT NULL,
  `RegistrationDate` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- הוצאת מידע עבור טבלה `aspnetusers`
--

INSERT INTO `aspnetusers` (`Id`, `UserName`, `NormalizedUserName`, `Email`, `NormalizedEmail`, `EmailConfirmed`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`, `PhoneNumber`, `PhoneNumberConfirmed`, `TwoFactorEnabled`, `LockoutEnd`, `LockoutEnabled`, `AccessFailedCount`, `FirstName`, `LastName`, `Address`, `CompanyName`, `BirthDate`, `RegistrationDate`) VALUES
('13547ca4-caff-40aa-8c42-a3a0800a631d', 'david', 'DAVID', 'ofnoacompsphotos@gmail.com', 'OFNOACOMPSPHOTOS@GMAIL.COM', 0, 'AQAAAAIAAYagAAAAEBFUXoTGwZ471vjqMzjHoLwCDwxgwXEWgeqIEH2j8hxkuRjhAOESNeuovKq2mydrqQ==', 'NDJSKJYPQHDJ7UIPJVA5TUM5RSHNGTSL', '8cc6bb6e-fa85-44df-8871-9d0e52a8d005', '0506279805', 0, 0, NULL, 1, 0, 'Liraz', 'Shitrit', 'Acre, Hashalom 29 st.', '333ofnoacomps', NULL, '2025-11-09 19:04:57'),
('4be48a5e-01a9-4bb7-995d-70590854d750', 'lili', 'LILI', 'lilinatural@gmail.com', 'LILINATURAL@GMAIL.COM', 0, 'AQAAAAIAAYagAAAAEHQQOfdgwrv4AYH6LMndTakMhVeFOkECy97g8vEfiviI13/GfG6TIe4teJzdS1x+Yg==', 'SZNPWFMB7OLFEAV2ELH252RVODRHZEXS', '47090592-db8a-4a09-9479-479aa6a06770', '058-6818560', 0, 0, NULL, 1, 0, 'לילי', 'אנג\'ל', 'מיקונס 3 תל אביב', 'try it', NULL, '2025-10-10 08:55:30'),
('8c48fbc7-1294-4a4f-85a9-1b0cd8e923b9', 'john', 'JOHN', 'john@do.com', 'JOHN@DO.COM', 0, 'AQAAAAIAAYagAAAAECc61sWCPVkiPCC4cjCjWFfSJiNzRRfclHRDk/bYlsue6HphLYYu4QbNK3nd49SOlA==', 'OV36Z3CVNQOJAUQCP6EXELIWNEY6Z2N2', '26e49a47-16db-4827-89f3-e12630665fff', '050234556', 0, 0, NULL, 1, 0, 'john', 'doe', 'לוליפופ 15 קיריית אתא', 'פופלי', '1978-11-05', '2025-11-29 21:33:05'),
('c7e4948f-5c1e-46e7-a018-4649f4db3234', 'liraz', 'LIRAZ', 'ofnoacomps@gmail.com', 'OFNOACOMPS@GMAIL.COM', 0, 'AQAAAAIAAYagAAAAEJPr34XhXTBms4Azum9sz/8FGtqeo8UY2QJ1dk1eIZZRJyopG3Bcz3NTR6vrNc9Otg==', 'WHMKCDDWNLLIGHLYXSWHCJBVPF746TGQ', 'efc4791c-502a-488b-a2b6-559ed1f6cf14', '0506279805', 0, 0, NULL, 1, 0, 'Liraz', 'Shitrit', 'Acre, Dotan 2 st.', '!!ofnoacomps Systems', '1973-11-29', '2025-09-22 17:21:01');

--
-- Indexes for dumped tables
--

--
-- אינדקסים לטבלה `aspnetusers`
--
ALTER TABLE `aspnetusers`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  ADD KEY `EmailIndex` (`NormalizedEmail`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
