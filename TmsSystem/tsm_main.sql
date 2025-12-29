-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Generation Time: דצמבר 02, 2025 בזמן 07:11 PM
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
-- מבנה טבלה עבור טבלה `aspnetroleclaims`
--

CREATE TABLE `aspnetroleclaims` (
  `Id` int(11) NOT NULL,
  `RoleId` varchar(255) NOT NULL,
  `ClaimType` longtext DEFAULT NULL,
  `ClaimValue` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `aspnetroles`
--

CREATE TABLE `aspnetroles` (
  `Id` varchar(255) NOT NULL,
  `Name` varchar(256) DEFAULT NULL,
  `NormalizedName` varchar(256) DEFAULT NULL,
  `ConcurrencyStamp` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- הוצאת מידע עבור טבלה `aspnetroles`
--

INSERT INTO `aspnetroles` (`Id`, `Name`, `NormalizedName`, `ConcurrencyStamp`) VALUES
('2646d73b-d3b1-4bd2-bf36-7377d7b92e7b', 'User', 'USER', NULL),
('28acf1e6-9fda-11f0-90dd-10ffe07c3e85', 'Admin', 'ADMIN', '28acf1ef-9fda-11f0-90dd-10ffe07c3e85');

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `aspnetuserclaims`
--

CREATE TABLE `aspnetuserclaims` (
  `Id` int(11) NOT NULL,
  `UserId` varchar(255) NOT NULL,
  `ClaimType` longtext DEFAULT NULL,
  `ClaimValue` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `aspnetuserlogins`
--

CREATE TABLE `aspnetuserlogins` (
  `LoginProvider` varchar(255) NOT NULL,
  `ProviderKey` varchar(255) NOT NULL,
  `ProviderDisplayName` longtext DEFAULT NULL,
  `UserId` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `aspnetuserroles`
--

CREATE TABLE `aspnetuserroles` (
  `UserId` varchar(255) NOT NULL,
  `RoleId` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- הוצאת מידע עבור טבלה `aspnetuserroles`
--

INSERT INTO `aspnetuserroles` (`UserId`, `RoleId`) VALUES
('13547ca4-caff-40aa-8c42-a3a0800a631d', '28acf1e6-9fda-11f0-90dd-10ffe07c3e85'),
('4be48a5e-01a9-4bb7-995d-70590854d750', '28acf1e6-9fda-11f0-90dd-10ffe07c3e85'),
('8c48fbc7-1294-4a4f-85a9-1b0cd8e923b9', '2646d73b-d3b1-4bd2-bf36-7377d7b92e7b'),
('c7e4948f-5c1e-46e7-a018-4649f4db3234', '28acf1e6-9fda-11f0-90dd-10ffe07c3e85');

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
('c7e4948f-5c1e-46e7-a018-4649f4db3234', 'liraz', 'LIRAZ', 'ofnoacomps@gmail.com', 'OFNOACOMPS@GMAIL.COM', 0, 'AQAAAAIAAYagAAAAEJPr34XhXTBms4Azum9sz/8FGtqeo8UY2QJ1dk1eIZZRJyopG3Bcz3NTR6vrNc9Otg==', 'WHMKCDDWNLLIGHLYXSWHCJBVPF746TGQ', 'b909144d-ff50-4f22-b635-9d7677baf276', '0506279805', 0, 0, NULL, 1, 0, 'Liraz', 'Shitrit', 'Acre, Dotan 2 st.', 'ofnoacomps Systems', '1973-11-29', '2025-09-22 17:21:01');

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `aspnetusertokens`
--

CREATE TABLE `aspnetusertokens` (
  `UserId` varchar(255) NOT NULL,
  `LoginProvider` varchar(255) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Value` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `cancellations`
--

CREATE TABLE `cancellations` (
  `CancellationId` int(11) NOT NULL,
  `OrderId` int(11) NOT NULL,
  `ConditionText` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `customers`
--

CREATE TABLE `customers` (
  `CustomerId` int(11) NOT NULL,
  `IDNumber` varchar(20) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `Phone` varchar(50) NOT NULL,
  `FullName` varchar(255) DEFAULT NULL,
  `CustomerName` varchar(255) DEFAULT NULL,
  `CompanyName` varchar(255) DEFAULT NULL,
  `Address` varchar(500) DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- הוצאת מידע עבור טבלה `customers`
--

INSERT INTO `customers` (`CustomerId`, `IDNumber`, `Email`, `Phone`, `FullName`, `CustomerName`, `CompanyName`, `Address`, `CreatedAt`) VALUES
(1, '025686254', 'ofnoacomps@gmail.com', '0506279805', 'לירז שטרית', 'לירז שטרית', 'ofnoacomps', 'Acre, Dotan 2 st.', '2025-09-22 00:00:00'),
(2, '052462115', 'fixed@gogo.com', '054-7445214', 'שולה ויצמן', 'שולה ', 'אופקים ביטוח', 'רחוב הירקון 76 תל אביב', '2025-10-03 10:51:06'),
(3, '064852214', 'dgdfj45@gmaul.com', '052-7952241', 'ישראלה איפרגן', 'ישראלה', 'השיפוצניקים בע\"מ', 'רחוב המשושה 5 רמת אביב', '2025-10-03 10:52:13'),
(7, '18454154', 'dgsdf@dghd.com', '0562452254', 'ישראל ישראלי', 'ישראל', 'מטיילי ישראל', 'מארק שגאל 6, תל אביב', '2025-11-22 10:30:28');

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `guides`
--

CREATE TABLE `guides` (
  `GuideId` int(11) NOT NULL,
  `OrderId` int(11) DEFAULT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `Phone` varchar(50) DEFAULT NULL,
  `Email` varchar(255) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT 1,
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp(),
  `GuideName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- הוצאת מידע עבור טבלה `guides`
--

INSERT INTO `guides` (`GuideId`, `OrderId`, `Description`, `Phone`, `Email`, `IsActive`, `CreatedAt`, `GuideName`) VALUES
(4, NULL, 'מדריך עם ניסיון רב בטיולים בארץ - גר בקיבוץ מפלסים שבעוטף כ30 שנה. חקלאי לשעבר, אוהב צמחים ומטפח גינה, עד המלחמה מדריך תיירות נכנסת, יהודית ונוצרית, מתמחה בתיירות לעלייה לרגל לאתרים בעלי משמעות דתית של \"ארץ הקודש\", מדריך בעוטף מכיר את הנגב המערבי \"עוטף עזה\" הייטב כתושב האיזור כ45 שנה, בעל אוהב, אב ל5 ו3 נכדים ,איש משפחה.', '050-1234567', 'pepa@tryit.co.il', 1, '2025-11-29 10:05:56', 'פפה'),
(5, NULL, 'יונתן שלמה, עשה ועושה מילואים כבר 26 שנה כלוחם.  הוא יודע לספר על הלחימה בעזה ממקור ראשון. כמי שעושה מילואים 30 שנה, הוא יכול לספר על השינויים שהצבא עבר. יונתן מדריך טיולים כמעט אותו מספר שנים שהוא עושה מילואים. הוא מורה דרך. הוא בעל תואר בארכיאולוגיה וידע נרחב על ההיסטוריה הארץ ישראלית. יונתן היה מורה לשל\"ח, מדריך תגלית, מדריך טיולים פרטיים וחברות בארץ, ומדריך תיירים. בנוסף יונתן ליווה וניהל סיורי משלחות דיפלומטיות בישראל.', '052-7654320', 'yonatan@tryit.co.il', 1, '2025-11-29 10:06:11', 'יונתן'),
(6, NULL, 'וקר אקדמאי, אנתרופולוג ומורה דרך בהכשרתו. מדריך טיולים מעל לעשור. מתמחה בטיולי תרבות אותנטיים ואקזוטיים במגוון יעדים ביבשות אפריקה ואסיה. מגיל צעיר מטייל, טועם, מתנדב וחוקר תרבויות לאורכו ולרוחבו של הגלובס. משלב בין האקדמי לרוח ובין הרצאות מרתקות וייחודית לחוויות עמוקות ומרגשות, קלילות וכיף.', '0506224455', 'ofir@gmail.com', 1, '2025-11-29 10:12:53', 'אופיר מיטראני'),
(9, NULL, 'מדריך פיקטיבי למחיקות', NULL, NULL, 0, '2025-11-29 10:33:57', 'מדריך לא מוגדר');

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `itineraries`
--

CREATE TABLE `itineraries` (
  `ItineraryId` int(11) NOT NULL,
  `TourId` int(11) NOT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- הוצאת מידע עבור טבלה `itineraries`
--

INSERT INTO `itineraries` (`ItineraryId`, `TourId`, `Name`, `CreatedAt`) VALUES
(19, 23, 'לוח זמנים - סיור בשכונת שפירא', '2025-09-24 14:21:59'),
(20, 24, 'לוח זמנים - יום סיור בעוטף עזה - –תמיכה, גבורה וזיכרון', '2025-10-03 00:47:30'),
(21, 25, 'לוח זמנים - סיור בירושלים', '2025-10-04 15:03:31'),
(22, 26, 'לוח זמנים - סיור בעכו', '2025-10-08 09:37:13');

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `itinerary`
--

CREATE TABLE `itinerary` (
  `ItineraryId` int(11) NOT NULL,
  `OrderId` int(11) NOT NULL,
  `StartTime` time NOT NULL,
  `EndTime` time DEFAULT NULL,
  `Location` varchar(255) DEFAULT NULL,
  `Description` text DEFAULT NULL,
  `TourId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `itineraryitems`
--

CREATE TABLE `itineraryitems` (
  `ItemId` int(11) NOT NULL,
  `ItineraryId` int(11) NOT NULL,
  `OrderNum` int(11) NOT NULL,
  `StartTime` time NOT NULL,
  `EndTime` time NOT NULL,
  `Location` varchar(255) NOT NULL,
  `Description` text DEFAULT NULL,
  `TourId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- הוצאת מידע עבור טבלה `itineraryitems`
--

INSERT INTO `itineraryitems` (`ItemId`, `ItineraryId`, `OrderNum`, `StartTime`, `EndTime`, `Location`, `Description`, `TourId`) VALUES
(30, 20, 0, '07:50:00', '08:00:00', 'התכנסות', 'יציאה מנקודת האיסוף', 24),
(31, 20, 0, '09:30:00', '10:10:00', 'נקודת ההתחלה', 'קפה מאפה ושירותים (לא כלול ניתן לקנות עצמאית', 24),
(32, 20, 0, '10:10:00', '10:40:00', 'משטרת שדרות', 'נמשיך לתחנת משטרת שדרות. נכיר את סיפר הקרב ההרואי של תושבי ושוטרי שדרות. האתר כיום הוא אנדרטה המחברת בן ארוכי השבעה לאוקטובר ומורשת ישראל.', 24),
(33, 20, 0, '11:00:00', '12:30:00', 'קיבוצים בעוטף', 'סיור בקיבוץ בארי ומגן או בבארי ומפלסים.\r\nכניסה לנתיב העשרה לתמיכה בעסק מקומי והכרת הסיפור של המשפחה.', 24),
(34, 20, 0, '12:40:00', '13:50:00', 'נסיעה לתקומה וביקור באתר', 'אתר המכוניות השרופות. סיפור האמבולנס, הרכבים. זהו רגע לא פשוט וקשה. באתר מאות מכוניות של נפגעי השבעה באוקטובר, וגם רכבי ואופנועי המחבלים. באתר יש גם שירותים ומיגונית . \r\nתצפית על עזה, נשמע על הלחימה בעזה. נכיר את הטופוגרפיה המקומית ומה קרה באמת  לאורך כל הגדר בשבעה באוקטובר, היכן היא נפרצה ועוד. ', 24),
(35, 21, 0, '07:30:00', '08:00:00', 'התכנסות', 'נפגשים בתחנת הרכבת סבידור - משם נעלה על המיניבוס', 25),
(36, 21, 0, '09:00:00', '10:30:00', 'מדרחוב ממילא', 'הסיור נפתח בלב האזורים המתחדשים של ירושלים – במדרחוב ממילא, למשל, או באזור תחנת הרכבת הישנה (התחנה הראשונה), שם מרגישים את הדופק החדש של העיר. האדריכלות העכשווית, המסעדות, בתי הקפה והאומנות המקומית מציגים את הפן הרענן של ירושלים, העיר שחיה, מתחדשת ומתפתחת בלי לוותר על שורשיה. האווירה קלילה, הנוף נפתח בהדרגה, והקבוצה כבר מתחילה להרגיש את הקסם של יום אחר, מרוחק מהמשרד ומלא בתוכן וחוויה.', 25),
(37, 21, 0, '11:00:00', '12:00:00', 'ימין משה - משכנות שאננים', 'מכאן נתחיל לזלוג אל תוך השכבות של ההיסטוריה – נמשיך רגלית דרך ימין משה, ניכנס אל שכונת משכנות שאננים, נעצור בתצפיות שמספקות רגע של שקט מול העיר העתיקה ונלמד על הסיפורים שמאחורי המקומות – מי התגורר שם, מה הוביל להקמת השכונות, ואיך הפכה ירושלים למה שהיא היום. בלי למהר ובלי לעבור בין כניסות צפופות, הסיור מתנהל באוויר הפתוח, בקצב שמאפשר לשוחח, לצחוק, להתרשם ולצלם – והכל עם מדריך מנוסה שמרגיש את הקבוצה ויודע להעביר את התוכן בגובה העיניים.', 25),
(38, 21, 0, '12:30:00', '14:00:00', 'שוק מחנה יהודה', 'ארוחת צהריים בשוק מחנה יהודה, חוויה צבעונית, קולינרית ותוססת שפותחת עוד חלון אל הלב של ירושלים כולל סיור קצר בין הדוכנים', 25),
(39, 21, 0, '14:30:00', '16:00:00', 'העיר העתיקה', 'ככל שנתקדם, נגיע אל לב העיר ההיסטורית – שער יפו, הרובע היהודי, הקארדו, רחוב דוד. נטייל בתוך הסמטאות, נעצור בנקודות מפתח מרתקות, ניחשף לפינות נסתרות, לחצרות קסומות, לאבני דרך תרתי משמע, ונרגיש ממש את המעבר מן ההווה אל העבר. בלי עמידה בכניסות, בלי צורך בכרטיסים, פשוט חוויה שמובילה את הקבוצה אל תוך העיר, ממש כמו מסע בזמן, תוך שמירה על זרימה טבעית ונעימה.', 25),
(40, 21, 0, '16:35:00', '17:00:00', 'חזרה אל האוטובוס ', 'סיכום של היום וחזרה לתל אביב', 25),
(102, 19, 0, '06:30:00', '07:00:00', 'תחילת סיור', 'נפגשים ברחוב מאו מאו 8 .', 23),
(103, 19, 0, '07:20:00', '08:20:00', 'נפגשים בבית הקפה', 'את הסיור נפתח בבית קפה מקומי שמזמן הפך להרבה יותר ממקום לעצור בו על כוס אספרסו. מדובר במרכז חברתי-אקולוגי הפועל כמרחב פתוח לתושבי השכונה, מתנדבים, יזמים חברתיים ומבקרים מבחוץ. נשמע כיצד המקום הפך לזירה של מפגש בין עולמות, ונכיר את הערכים המרכזיים של השכונה – שיתוף, קיימות, הכלה וקירוב לבבות.', 23),
(104, 19, 0, '08:30:00', '10:00:00', 'מרכז בגדים חברתי – אופנה עם ערך ', 'משם נבקר בעסק חברתי מקסים שמשלב מחזור בגדים, העצמה קהילתית, תרבות ואופנה אלטרנטיבית. המקום אינו רק חנות – הוא פלטפורמה לפעילות קהילתית, שיח בין מגזרים שונים, מפגשים פתוחים וסדנאות יצירה. זוהי נקודת מפגש בין עבר להווה, בין מקומיים למהגרים, בין צעירים לוותיקים – כור היתוך אמיתי שמתבטא גם בבגדים עצמם.', 23),
(105, 19, 0, '10:30:00', '12:00:00', 'ביקור בחווה חקלאית עירונית', 'את הסיור נסיים בביקור מפתיע במיוחד: חווה חקלאית בלב שכונת שפירא. כן, שמעתם נכון – בין בנייני מגורים פשוטים, בתוך רחוב תל-אביבי רגיל, פורחת חקלאות עירונית אמיתית. החווה מהווה מרכז חינוכי, סביבתי וקהילתי שפועל עם מתנדבים מקומיים ועם בתי ספר. נשמע כיצד נוצר שיתוף פעולה יוצא דופן בין אדם לאדמה, נציץ לערוגות ירק, נזכה לטעום מתוצרי החווה החקלאית המקומית ונבין איך גם בעיר הגדולה אפשר ליצור טבע, אם רק מאמינים בזה.', 23),
(116, 22, 0, '07:00:00', '07:30:00', 'התכנסות', 'נפגשים בחניון האוטובוסים -תחילת היום', 26),
(117, 22, 0, '07:20:00', '09:00:00', 'נסיעה', 'נסיעה לעכו - בדרך הסברים מהמדריך', 26),
(118, 22, 0, '09:10:00', '12:00:00', 'עכו - העיר העתיקה', 'סיור מלווה בהמחשות מהמדריך', 26),
(119, 22, 0, '12:30:00', '13:30:00', 'מסעדת אורי בורי', 'ארוחת צהריים', 26),
(120, 22, 0, '14:00:00', '15:30:00', 'סיור בכלא עכו', 'סיור בתוך הכלא בו נכלאו אסירי המחתרות', 26);

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `offers`
--

CREATE TABLE `offers` (
  `OfferId` int(11) NOT NULL,
  `CustomerId` int(11) NOT NULL,
  `GuideId` int(11) NOT NULL,
  `TourId` int(11) NOT NULL,
  `Participants` int(11) NOT NULL,
  `TripDate` datetime NOT NULL,
  `TourDate` datetime NOT NULL,
  `PickupLocation` varchar(500) DEFAULT NULL,
  `Price` decimal(10,2) NOT NULL,
  `TotalPayment` decimal(10,2) NOT NULL,
  `PriceIncludes` varchar(2000) DEFAULT NULL,
  `PriceExcludes` varchar(2000) DEFAULT NULL,
  `SpecialRequests` varchar(2000) DEFAULT NULL,
  `LunchIncluded` tinyint(1) DEFAULT 0,
  `PaymentId` int(11) DEFAULT NULL,
  `PaymentMethodId` int(11) DEFAULT NULL,
  `CreatedAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- הוצאת מידע עבור טבלה `offers`
--

INSERT INTO `offers` (`OfferId`, `CustomerId`, `GuideId`, `TourId`, `Participants`, `TripDate`, `TourDate`, `PickupLocation`, `Price`, `TotalPayment`, `PriceIncludes`, `PriceExcludes`, `SpecialRequests`, `LunchIncluded`, `PaymentId`, `PaymentMethodId`, `CreatedAt`) VALUES
(17, 3, 4, 24, 45, '2025-10-28 00:00:00', '2025-10-28 00:00:00', 'תחנת רכבת סבידור - תל אביב', 250.00, 11250.00, 'הסעה', 'טיפים', 'ללא', 0, 3, 3, '2025-10-04 15:07:44'),
(18, 2, 4, 24, 44, '2025-10-22 00:00:00', '2025-10-22 00:00:00', 'תחנת רכבת - האוניברסיטה בתל אביב', 300.00, 13200.00, '• ארוחת צהריים\r\nהסעה צמודה\r\nמדריך צמוד', 'דמי כניסה לאתרים\r\nטיפים', 'ללא', 1, 2, 3, '2025-10-07 23:39:19'),
(20, 1, 9, 25, 40, '2025-10-29 00:00:00', '2025-10-29 00:00:00', 'תחנת רכבת - האוניברסיטה בתל אביב', 400.00, 0.00, 'הסעה צמודה לכל היום\r\nמדריך מקצועי\r\n• ארוחת צהריים', 'טיפים \r\nכניסות לאתרים', 'לא', 1, 0, 3, '2025-10-14 22:49:12');

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `orderincludes`
--

CREATE TABLE `orderincludes` (
  `IncludeId` int(11) NOT NULL,
  `OrderId` int(11) NOT NULL,
  `Type` enum('INCLUDED','NOT_INCLUDED') NOT NULL,
  `Description` varchar(500) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- הוצאת מידע עבור טבלה `orderincludes`
--

INSERT INTO `orderincludes` (`IncludeId`, `OrderId`, `Type`, `Description`) VALUES
(1, 1, 'INCLUDED', 'אוטובוס תיירים מפואר'),
(2, 1, 'INCLUDED', 'הדרכה מקצועית לאורך כל היום'),
(3, 1, 'INCLUDED', 'כניסה לאתרי הטיול'),
(4, 1, 'NOT_INCLUDED', 'ביטוח בריאות אישי'),
(5, 1, 'NOT_INCLUDED', 'הוצאות אישיות נוספות');

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `orders`
--

CREATE TABLE `orders` (
  `OrderId` int(11) NOT NULL,
  `CustomerId` int(11) NOT NULL,
  `GroupName` varchar(255) DEFAULT NULL,
  `ContactName` varchar(255) DEFAULT NULL,
  `ContactPhone` varchar(50) DEFAULT NULL,
  `ParticipantsMin` int(11) DEFAULT NULL,
  `ParticipantsMax` int(11) DEFAULT NULL,
  `PickupPoint` varchar(255) DEFAULT NULL,
  `TourDate` date DEFAULT NULL,
  `Price` decimal(10,2) DEFAULT NULL,
  `PaymentTerms` varchar(100) DEFAULT NULL,
  `TotalAmount` decimal(10,2) DEFAULT NULL,
  `SpecialRequests` text DEFAULT NULL,
  `LunchIncluded` tinyint(1) DEFAULT 0,
  `LunchMenu` varchar(255) DEFAULT NULL,
  `CreatedAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- הוצאת מידע עבור טבלה `orders`
--

INSERT INTO `orders` (`OrderId`, `CustomerId`, `GroupName`, `ContactName`, `ContactPhone`, `ParticipantsMin`, `ParticipantsMax`, `PickupPoint`, `TourDate`, `Price`, `PaymentTerms`, `TotalAmount`, `SpecialRequests`, `LunchIncluded`, `LunchMenu`, `CreatedAt`) VALUES
(1, 1, 'ארגון נעמת', 'רונית כהן', '050-1234567', 30, 40, 'רחוב הרצל 10, תל אביב', '2025-10-15', 3500.00, 'שוטף +30', 3500.00, 'בקשה מיוחדת: לשמור מקומות קדמיים באוטובוס', 1, 'ארוחת צהריים בשרית במסעדה מקומית', '2025-09-23 08:48:24');

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `paymentmethod`
--

CREATE TABLE `paymentmethod` (
  `ID` int(11) NOT NULL,
  `METHOD` varchar(255) NOT NULL,
  `PaymentMethodId` int(20) NOT NULL,
  `PaymentName` varchar(140) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- הוצאת מידע עבור טבלה `paymentmethod`
--

INSERT INTO `paymentmethod` (`ID`, `METHOD`, `PaymentMethodId`, `PaymentName`) VALUES
(1, 'מזומן', 1, 'מזומן'),
(2, 'אשראי', 2, 'אשראי'),
(3, 'העברה בנקאית', 3, 'העברה בנקאית');

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `payments`
--

CREATE TABLE `payments` (
  `PaymentId` int(11) NOT NULL,
  `OrderId` int(11) NOT NULL,
  `Amount` decimal(10,2) NOT NULL,
  `PaymentDate` date NOT NULL,
  `Method` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- הוצאת מידע עבור טבלה `payments`
--

INSERT INTO `payments` (`PaymentId`, `OrderId`, `Amount`, `PaymentDate`, `Method`) VALUES
(1, 1, 1.00, '2025-09-30', 'מזומן'),
(3, 1, 0.00, '2025-10-04', 'Pending'),
(4, 1, 0.00, '2025-10-04', 'Placeholder');

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `tourexclude`
--

CREATE TABLE `tourexclude` (
  `Id` int(11) NOT NULL,
  `TourId` int(11) NOT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `Text` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- הוצאת מידע עבור טבלה `tourexclude`
--

INSERT INTO `tourexclude` (`Id`, `TourId`, `Description`, `Text`) VALUES
(4, 24, 'קפה, מאפה', NULL),
(5, 25, 'טיפים', NULL),
(6, 25, 'כניסות לאתרים', NULL),
(7, 25, 'ביטוח', NULL),
(43, 23, 'ארוחת צהרים', NULL),
(44, 23, 'טיפים', NULL),
(45, 23, 'ביטוחים', NULL),
(52, 26, 'ארוחת צהרים - תשלום', NULL),
(53, 26, 'טיפים', NULL),
(54, 26, 'ביטוח', NULL);

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `tourinclude`
--

CREATE TABLE `tourinclude` (
  `Id` int(11) NOT NULL,
  `TourId` int(11) NOT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `Text` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- הוצאת מידע עבור טבלה `tourinclude`
--

INSERT INTO `tourinclude` (`Id`, `TourId`, `Description`, `Text`) VALUES
(12, 24, 'כולל מדריך שלנו ', NULL),
(13, 25, 'אוטובוס צמוד', NULL),
(14, 25, 'מדריך מקצועי צמוד', NULL),
(15, 25, 'עצירות מיוחדות לטעימות קולינריות', NULL),
(40, 23, 'כולל מדריך שלנו ', NULL),
(41, 23, 'מים קרים בנקודות', NULL),
(46, 26, 'כולל מדריך שלנו , אוטובוס צמוד', NULL),
(47, 26, 'הסברים מקצועיים', NULL);

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `tours`
--

CREATE TABLE `tours` (
  `TourId` int(11) NOT NULL,
  `OrderId` int(11) DEFAULT NULL,
  `Title` varchar(255) NOT NULL,
  `Description` text DEFAULT NULL,
  `TourDate` date NOT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- הוצאת מידע עבור טבלה `tours`
--

INSERT INTO `tours` (`TourId`, `OrderId`, `Title`, `Description`, `TourDate`, `CreatedAt`) VALUES
(23, NULL, 'סיור בשכונת שפירא', 'בלב תל אביב, הרחק מהמסלולים הרגילים של העיר הסואנת, מסתתרת אחת השכונות הכי מסקרנות, מגוונות ומעוררות השראה – שכונת שפירא. בין רחובות קטנים, חצרות ירוקות, סמטאות ציוריות ובנייני מגורים פשוטים, פורח לו עולם של עשייה קהילתית, קיימות סביבתית ותרבות מקומית שוקקת חיים. סיור בשכונת שפירא עם TRYIT יחשוף בפניכם את הקסם המיוחד של השכונה דרך סיפורים, אנשים ומקומות שלא תמצאו בשום מקום אחר.', '0000-00-00', '2025-09-24 14:21:59'),
(24, NULL, 'יום סיור בעוטף עזה - –תמיכה, גבורה וזיכרון', 'תודה שבחרתם לסייר ולשמוע על הגבורה, הלחימה והאסון בשבעה באוקטובר. \r\nהיוזמה שלנו לסיורים בעוטף עזה החלה בפברואר 2024, במהלך הלחימה ברצועה, מתוך תחושת שליחות ורצון להמחיש את המציאות המורכבת של האזור ולתמוך בקהילות שנפגעו.\r\nרגישות, כבוד ושותפות\r\nאנו פועלים מתוך כבוד ורגישות, מגיעים רק למקומות שביקשו שנבוא אליהם, כמו קיבוצים ומושבים שהחליטו במשותף לפתוח את שעריהם. בכל סיור אנו מקפידים לתרום ולקנות מעסקים מקומיים כדי לחזק את הכלכלה המקומית בעוטף עזה.', '0000-00-00', '2025-10-03 00:47:30'),
(25, NULL, 'סיור בירושלים', 'אם אתם מחפשים יום גיבוש שמצליח גם לרגש, גם לגבש וגם להשאיר טעם של עוד – סיור לעובדים בירושלים עם TRYIT הוא בדיוק מה שאתם צריכים. מדובר במסע מרתק בין אזורים עכשוויים ותוססים לבין סמטאות היסטוריות מלאות עומק, המשלב בין הקצב של העיר המודרנית לבין השקט של ירושלים העתיקה. הסיור בנוי בקפידה בסגנון \"מחדש לישן\", ומציע לעובדים חוויה אותנטית, דינמית, מלאת קסם ירושלמי – מבלי להכביד או לסבך את היום בכניסות לאתרים סגורים או זמני המתנה מיותרים.', '0000-00-00', '2025-10-04 15:03:31'),
(26, NULL, 'סיור בעכו', 'סיור מיוחד בעכו והעיר העתיקה, נסייר באולמות האבירים, בחומות המפורסמות שנפוליאון לא הצליח להבקיע וכמובן השוק והנמל של העיר העתיקה, עם הריחות , הטעמים והחוויות. ניתן לשלב סיור טעימות קולינרי .', '0000-00-00', '2025-10-08 09:37:13');

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `tripdays`
--

CREATE TABLE `tripdays` (
  `TripDayId` int(11) NOT NULL,
  `TripId` int(11) NOT NULL,
  `DayNumber` int(11) NOT NULL,
  `Title` varchar(255) NOT NULL,
  `Location` varchar(500) DEFAULT NULL,
  `Description` text DEFAULT NULL,
  `ImagePath` varchar(500) DEFAULT NULL,
  `DisplayOrder` int(11) NOT NULL
) ;

--
-- הוצאת מידע עבור טבלה `tripdays`
--

INSERT INTO `tripdays` (`TripDayId`, `TripId`, `DayNumber`, `Title`, `Location`, `Description`, `ImagePath`, `DisplayOrder`) VALUES
(1, 1, 1, 'יום 1 - טיסה ואכלוס באתונה', 'אתונה', 'טיסה לאתונה. קבלת פנים בשדה התעופה והעברה למלון. אכלוס במלון ומנוחה. ערב חופשי להתרשמות ראשונית מהעיר. אפשרות לארוחת ערב בטברנה יוונית מקומית ברובע פלאקה.', '/uploads/trips/e6d6a233-ce20-44e9-ab76-492b03d80f8c_אל_על_f7sgeq.jpg', 1),
(2, 1, 2, 'יום 2 - סיור באתונה העתיקה', 'אתונה', 'בוקר: סיור מודרך באקרופוליס, הפרתנון, תיאטרון דיוניסוס והאגורה העתיקה. צהריים: ארוחה חופשית ברובע מונסטירקי. אחה״צ: ביקור במוזיאון הארכיאולוגי הלאומי, הכולל אוצרות מהתקופה המיקנית והקלאסית. ערב: סיור ברובע פלאקה ופסגת ליקבטוס לנוף פנורמי של העיר.', '/uploads/trips/051bb9de-b0f6-4cb7-b2c9-9bc250f566c9_bucharest-3642825_640.jpg', 2),
(3, 1, 3, 'יום 3 - יום חופשי באתונה', 'אתונה', 'יום חופשי לבחירה. אפשרויות: ביקור במוזיאון האקרופוליס החדש, שוק מונסטירקי, קניות ברובע קולונאקי, או טיול יום לכף סוניון להתפעל ממקדש פוסידון. ערב: אפשרות למופע פולקלור יווני מסורתי עם ארוחת ערב (בתשלום נוסף).', '/uploads/trips/e430499d-0bb2-4028-b607-db005b15fc7b_Microsoft-East-Campus-Aerial-Photo.jpg', 3),
(4, 1, 4, 'יום 4 - טיסה למיקונוס', 'מיקונוס', 'בוקר: העברה לנמל התעופה וטיסה קצרה למיקונוס (כ-45 דקות). הגעה למיקונוס, אכלוס במלון ומנוחה. אחה״צ: סיור חופשי בעיר מיקונוס (חורה) - טחנות הרוח המפורסמות, שכונת קטומיליה הציורית, נמל קטן ונמל ונציה. ערב: ארוחת ערב חופשית במסעדות הרבות על קו המים.', '/uploads/trips/e0055aab-a8a0-471f-88b0-8cdd38c619bd_6P.jpg', 4),
(5, 1, 5, 'יום 5 - חופים ומסיבות במיקונוס', 'מיקונוס', 'יום חופש מלא ליהנות מחופי האי המפורסמים. המלצות: חוף Paradise Beach לאווירת מסיבות, Super Paradise למסיבות LGBTQ+, Elia Beach לאווירה רגועה יותר, Ornos Beach למשפחות. ערב: חיי לילה תוססים במועדוני האי או ארוחה רומנטית במסעדה על החוף.', '/uploads/trips/b88dc205-0ec0-4a8e-be90-f50fbf9033b0_חנוכה בספארי של ערבות טנזניה (3).jpg', 5),
(6, 1, 6, 'יום 6 - מעבורת לסנטוריני', 'סנטוריני', 'בוקר: העברה לנמל ומעבורת מהירה לסנטוריני (כ-2.5 שעות). הגעה לנמל והעברה למלון בפירה או אויה. אכלוס במלון מערות מסורתי עם נוף לקלדרה. אחה״צ: סיור חופשי באויה - הכפר הציורי ביותר עם בתי המערות הלבנים והכיפות הכחולות. ערב: שקיעה מהממת (הטובה בעולם!) מאויה או מטירת Skaros Rock.', '/uploads/trips/f29ed209-25de-4148-a80c-d891849bf648_Screenshot-2024-07-03-155435.jpg', 6),
(7, 1, 7, 'יום 7 - סיור בסנטוריני', 'סנטוריני', 'בוקר: ביקור באתר הארכיאולוגי של אקרוטירי - העיר המינואית המשומרת. צהריים: סיור ביקבי האי וטעימות יין. אחה״צ: ביקור בחוף הלבן (White Beach) או חוף קמארי השחור. ערב: שייט לשקיעה מסביב לקלדרה, כולל ארוחת ערב קלה על הסירה, שחייה במעיינות החמים הגופריתיים.', '/uploads/trips/c636ca9b-b8fd-4d3e-b0dc-25548a87a314_brasov-992817_640.jpg', 7),
(8, 1, 8, 'יום 8 - חזרה ארצה', 'סנטוריני → ישראל', 'בוקר חופשי אחרון באי. קניות מזכרות, ג׳לאטו אחרון ותמונות נוספות. צהריים: העברה לשדה התעופה בסנטוריני (או מעבורת לאתונה, תלוי בטיסה). טיסה חזרה לישראל עם זיכרונות בלתי נשכחים מיוון המופלאה.', '/uploads/trips/ce12e9b3-83fe-4cd3-859d-e5e3e6662f5b_surf-6466617_640.jpg', 8),
(9, 2, 1, 'יום 1 - טיסה והגעה למדריד', 'מדריד', 'טיסה למדריד. הגעה בבוקר/צהריים, קבלת פנים בשדה התעופה והעברה למלון. אכלוס ומנוחה. אחה״צ: סיור חופשי ראשוני ברובע סול (Puerta del Sol) - לב העיר הפועם. ערב: ארוחת ערב חופשית - מומלץ לטפאס במרקאדו דה סן מיגל או מסעדות ברובע לה לטינה.', '/uploads/trips/1a39902b-3c82-41ad-9ee3-2d150e50c325_אל_על_f7sgeq.jpg', 1),
(10, 2, 2, 'יום 2 - סיור במדריד המלכותית', 'מדריד', 'בוקר: סיור מודרך במוזיאון הפראדו - אחד ממוזיאוני האמנות הגדולים בעולם (Velázquez, Goya, El Greco). צהריים: ארוחה חופשית באזור רטירו. אחה״צ: סיור בארמון המלוכה (Palacio Real) והקתדרלה. ערב: סיור ברובע מלסניה והיפסטרים של מדריד, או טיול בפארק רטירו.', '/uploads/trips/87cd04a4-42ed-497a-8c8c-8b950cf423be_buildings-4011921_640.jpg', 2),
(11, 2, 3, 'יום 3 - מדריד - טולדו - מדריד', 'טולדו', 'טיול יום לטולדו - עיר מורשת עולמית של אונסק״ו. בוקר: נסיעה לטולדו (כ-70 ק״מ). סיור בעיר העתיקה המימי-ביניימית - קתדרלת טולדו המפוארת, בית הכנסת אל טרנסיטו, מבצר אלקסר. צהריים: ארוחה באחת המסעדות המסורתיות (תבשיל Carcamusas המפורסם). אחה״צ: המשך סיור וקניות בחנויות המזכרות. ערב: חזרה למדריד, ערב חופשי.', '/uploads/trips/2cf4907b-6112-4a9e-bdcd-ce209a8f563e_brasov-5781211_640.jpg', 3),
(12, 2, 4, 'יום 4 - מדריד לברצלונה', 'ברצלונה', 'בוקר: צ׳ק-אאוט מהמלון. נסיעה ברכבת מהירה AVE לברצלונה (כ-3 שעות נסיעה נוחה). הגעה לברצלונה והעברה למלון. אחה״צ: סיור ראשוני ברובע הגותי - הרובע העתיק עם סמטאות מרוצפות. ביקור בקתדרלה של ברצלונה. ערב: טיול בלאס רמבלאס, שוק בוקריה וארוחת ערב בנמל הישן (Port Vell).', '/uploads/trips/ac821aa8-e3ba-473d-aff0-c2ba263796e2_chicago-5347435_640.jpg', 4),
(13, 2, 5, 'יום 5 - ברצלונה של גאודי', 'ברצלונה', 'בוקר: ביקור בסגרדה פמיליה - הבזיליקה המפורסמת של אנטוני גאודי (כרטיסים מוזמנים מראש). צהריים: ארוחה ברובע גרסיה. אחה״צ: פארק גואל המדהים עם פסיפסי הקרמיקה הצבעוניים. המשך בפסאו דה גרסיה - קאסה בטיו וקאסה מילה (הבניינים המפורסמים של גאודי). ערב: חוף ברצלונטה ואוכל ים במסעדת חוף.', '/uploads/trips/f496ea89-eb05-4665-93f3-8d7e206a86f6_bucharest-140326_640.jpg', 5),
(14, 2, 6, 'יום 6 - יום חופשי בברצלונה', 'ברצלונה', 'יום חופש מלא. אפשרויות: מוזיאון פיקאסו, הר מונג׳ואיק ומבצר מונג׳ואיק, רובע בורן העליז והכנסיית סנטה מריה דל מאר, אצטדיון קאמפ נואו של ברצלונה, או פשוט קניות ברובע גרסיה ואיקסמפלה. ערב: מופע המזרקות המוזיקליות במונג׳ואיק (בעונה, חינם).', '/uploads/trips/82ace0b5-c563-469f-844a-8d81f860cf34_lisbon-8268841_640.jpg', 6),
(15, 2, 7, 'יום 7 - ברצלונה לגרנדה', 'גרנדה', 'בוקר מוקדם: צ׳ק-אאוט מהמלון בברצלונה. נסיעה ארוכה לגרנדה (כ-6 שעות נסיעה) או אפשרות לטיסה פנימית (שעה). צהריים: הפסקת ארוחה בדרך. אחה״צ-ערב: הגעה לגרנדה, אכלוס במלון. ערב חופשי לסיור ראשוני בעיר האנדלוסית המקסימה. מומלץ סיור ברובע האלבאיסין האותנטי (רובע מורי עתיק).', '/uploads/trips/10c3610c-b2c7-4559-bbed-0e0bec737d75_buildings-936589_1280.jpg', 7),
(16, 2, 8, 'יום 8 - ארמון אלהמברה', 'גרנדה', 'בוקר: ביקור בארמון אלהמברה המפורסם - פנינת האדריכלות המורית (חובה להזמין כרטיסים חודשיים מראש!). סיור בארמון נסרידס, גנרליפה, אלקסבה והגנים המרהיבים. צהריים-אחה״צ: סיור בשכונת אלבאיסין - רובע מורי עתיק עם בתים לבנים וסמטאות צרות. עליה למיראדור דה סן ניקולס לנוף הכי יפה על האלהמברה. ערב: מומלץ מופע פלמנקו במערות הסקרומונטה.', '/uploads/trips/b9f65cb6-a7f7-4e78-854d-8422db378d49_brandenburger-tor-201939_640.jpg', 8),
(17, 2, 9, 'יום 9 - גרנדה לסביליה', 'סביליה', 'בוקר: נסיעה לסביליה (כ-3 שעות). הגעה וסיור מודרך בסביליה - בירת אנדלוסיה. ביקור בקתדרלת סביליה (הגדולה ביותר בספרד) ומגדל ג׳ירלדה. אלקסר המלכותי - ארמון מורי מרהיב. צהריים: טפאס ברובע סנטה קרוז. אחה״צ: טיול ברובע היהודי ופלזה דה אספניה המפוארת. ערב: מופע פלמנקו אותנטי באחד מהטאבלאו המסורתיים (כלול במחיר).', '/uploads/trips/50d559ac-2b56-485b-bf85-c9b5fc54c5e7_city-park-blue-sky-with-downtown-skyline-background.jpg', 9),
(18, 2, 10, 'יום 10 - סביליה וחזרה למדריד', 'סביליה → מדריד → ישראל', 'בוקר חופשי אחרון בסביליה. קניות במרכז, קפה אחרון בפלזה נואבה, תמונות בפלזה דה אספניה. צהריים: העברה לתחנת הרכבת. רכבת מהירה חזרה למדריד (2.5 שעות). הגעה למדריד והעברה ישירה לשדה התעופה. טיסה חזרה לישראל עם מזוודות מלאות במזכרות, המון תמונות וזיכרונות מהטיול המדהים בספרד. Adiós España!', '/uploads/trips/42f942bf-186c-4fcf-95d7-68d24d256678_6P.jpg', 10),
(47, 3, 1, 'תל אביב – זנזיבר – סטון טאון', 'נתב\"ג', 'ניפגש בשדה התעופה, לטיסה ישירה (5.5 ש\') לזנזיבר – אי התבלינים.\r\nהאי האקזוטי והמסתורי, ששמו נודע למרחוק כאחד מהיפים בעולם, טומן בחובו היסטוריה מרתקת, רבדים עמוקים של תרבות אפריקאית צבעונית, אתרי טבע מרשימים וכמובן חופים לבנים ומים צלולים. לאחר הנחיתה וסידורים פורמליים בשדה התעופה, נסייר בעיר סטון טאון בדרכנו לבית המלון על החוף הטרופי.', '/uploads/trips/467299e5-9134-4189-9212-7c078f1c25fb_view-of-stone-town-zanzibar-tanzania-steve-outram.jpg', 1),
(48, 3, 2, ' יער ג\'וזאני ואי האסירים (אי צבי הים)', '', 'לאחר ארוחת הבוקר, ניסע אל שמורת ג\'וזני, שבמעבה היער שלה, ניתן למצוא את קופי הקולובוס האדומים, זן אנדמי לזנזיבר, המשתף פעולה בסקרנות עם מצלמות התיירים. נערוך סיור בין עצי המהגוני, נראה את קופי הקולובוס ונסייר בשמורת המנגרובים המרתקת על שביל הליכה מעץ. בחלקו השני של היום. לאחר הביקור נצא לשייט אל אי האסירים, שהתפרסם  הודות לאוכלוסיית צבי היבשה הענקים שחיים בו. נסייר בין מבני האי שהוקם כבית סוהר, אך שימש כמקלט למצורעים וכיום משמש ככפר נופש. במידה ויתאפשר, נותיר זמן לרחצה בחוף הצלול.', '/uploads/trips/a645f988-4dac-4ce0-a09a-5baecdee804a_Swimming-with-a-turtle-at-Nungwi-Beach-Easy-Travel-Tanzania-scaled-2.jpg', 2),
(49, 3, 3, 'כפרי הצפון: מרכז סירות דאו – צבי הים של נונגווי - שייט שקיעה בסירת מפרש', '', 'לאחר ארוחת הבוקר נצא בנסיעה לצפון האי ונבקר בכפר נונגווי\r\n(Nungwi). זהו כפר דייגים מסורתי ובו בתים קטנים וצנועים הבנויים\r\nמאבן האלמוג המקומית.\r\n\r\nנראה את שוק הדגים בו מתקיימות לעיתים מכירות פומביות. נבקר במרכז לבניית סירות דאו- סירות עץ בעלות מפרש משולש אשר סוד הרכבתן עובר במסורת מאב לבן ונמשיך ללגונה טבעית בה נוכל לפגוש ולהאכיל צבי-ים המצויים בסכנת הכחדה. בתום הביקור בכפר, נשוט אל הכפר השכן קנדווה (Kendwa) שהתפרסם הודות לחוף הרחב שלו שכמעט ואינו מושפע מגאות ושפל. נסיים את היום\r\nבשייט בסירת דאו מסורתית בשקיעה.', '/uploads/trips/f23217e5-dfe3-4c9a-bbe3-b258ad4680dd_Dhow-boat-2.jpg', 3),
(50, 3, 4, 'האי הנעלם והלגונה הירוקה', '', 'לאחר ארוחת בוקר נצא ליום כייף במפרץ Menai בדרום זנזיבר, הידוע כשמורת טבע גדולה של דולפינים, ומגוון של בעלי חיים ימיים וצמחים.\r\nנצא לשייט מענג בספינת דאו מסורתית בין איים טרופיים, שוניות אלמוגים מטריפות ולגונות עם עצי מנגרוב מרשימים. במהלך היום נעצור לרחצה ושיזוף על חופי הזהב המפורסמים של זנזיבר, נצלול אל תוך שוניות אלמוגים ונתענג על ארוחה מפנקת. הסיור כולל שתייה, פירות וארוחת צהריים.', '/uploads/trips/19fa10c7-b8c5-4f88-b8b6-ded97d8ba7da_8_safari-blue-zanzibar-menai-bay-fascinating-day-sea-trip-with-lunch-zanzibar.jpg', 4),
(51, 3, 5, 'שייט דולפינים ושנירקול באי מנבמה', '', 'לאחר ארוחת הבוקר ועזיבת המלון נצא לחלק הצפוני של האי ליום שייט מהנה הכולל שנירקול ותצפית על דולפינים. נעלה על סירות דאו ונשוט אל אחד האיים היפים בארכיפלג זנזיבר, מנמבה. סביב האי שמורת אלמוגים יפהפיה אליה מגיעים הדולפינים. נשנרקל בין האלמוגים, ואם יתמזל מזלנו, יצטרפו אלינו הדולפינים שחיים באזור (הסיכוי הסטטיסטי להופעתם של הדולפנים, מוערך בכ-70%). נשוט חזרה אל החוף הצפוני ניהנה מהשקיעה המרהיבה וחזרה לבית המלון.', '/uploads/trips/8622dcd8-abf0-491d-8bf8-d7326807c86d_Zanzibar-Mnemba-Island-aerial-dhow-_4_-360x240.jpg', 5),
(52, 3, 6, 'יום נופש', '', 'מאפשרים למי שרוצה ליהנות מספורט ימי ומנוחה באי זנזיבר.', '/uploads/trips/751cfa56-41a4-45e3-a0cf-7239b8f58c59_fzeen-boutique-hotel-zanzibar-general-69.jpg', 6),
(53, 3, 7, 'יום חופש באי', '', 'מאפשרים למי שרוצה ליהנות מספורט ימי ומנוחה באי זנזיבר.', '/uploads/trips/6a67ddf9-141a-480f-b4af-f0deab415f1a_jetski-Nungwi.webp', 7),
(54, 3, 8, 'זנזיבר חוות תבלינים – סטון טאון – תל אביב', '', 'יום אחרון בזנזיבר. בהתאם לשעת הטיסה, נצא לביקור באחת מחוות התבלינים הידועות של האי, בהן ניתן למצוא את המקור לרוב תבליני העולם. הסיור בחווה הוא חוויה אין סופית של צבעים, ריחות טעמים וניחוחות.\r\n\r\nנלמד על הגידולים באי ובינהם הציפורן, קינמון, וניל, פלפל, הל, כורכום ועוד רבים אחרים, על דרכי עיבודם ותפוצתם הגלובלית.\r\nנמשיך לבירת האי היפה: Stone town, \"עיר האבן\". העיר העתיקה, שבתיה בנויים מאלמוגים ואבנים, מורכבת מסבך של סמטאות מפותלות, שווקים ציוריים ובתים בסגנון אדריכלי מגוון. נשמע על שוק העבדים, שעד סוף המאה ה-19, שימש כתחנת סחר פעילה של עבדים מכל רחבי אפריקה, נראה את ביתם של ד\"ר ליוונגסטון, חוקר היבשת השחורה המפורסם, את בית ילדותו של סולן להקת קווין, פרדי מרקורי (יליד המקום) ועוד.\r\n\r\nלאחר עזיבת המלון, נצא בנסיעה לשדה התעופה הבינלאומי של זנזיבר לטיסה ישירה לתל אביב.', '/uploads/trips/0d2b5c99-c44b-4483-aab2-5af2c5147673_194227_5e4bedfebed6e-1.jpg', 8),
(62, 6, 1, 'יום 1', 'sdfgsdg', 'sdfh dsfhhdghhdsg', NULL, 1),
(63, 6, 2, 'יום 2', NULL, NULL, NULL, 2),
(64, 6, 3, 'יום 3', NULL, NULL, NULL, 3),
(65, 6, 4, 'יום 4', NULL, NULL, NULL, 4),
(66, 6, 5, 'יום 5', NULL, NULL, NULL, 5);

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `tripoffers`
--

CREATE TABLE `tripoffers` (
  `TripOfferId` int(11) NOT NULL,
  `CustomerId` int(11) NOT NULL,
  `TripId` int(11) NOT NULL,
  `OfferNumber` varchar(50) NOT NULL,
  `OfferDate` datetime NOT NULL DEFAULT current_timestamp(),
  `Participants` int(11) NOT NULL,
  `DepartureDate` date NOT NULL,
  `ReturnDate` date DEFAULT NULL,
  `PricePerPerson` decimal(10,2) NOT NULL,
  `SingleRoomSupplement` decimal(10,2) DEFAULT NULL,
  `SingleRooms` int(11) NOT NULL DEFAULT 0,
  `TotalPrice` decimal(10,2) NOT NULL,
  `PaymentMethodId` int(11) NOT NULL COMMENT 'מתייחס ל-ID בטבלת paymentmethod',
  `PaymentInstallments` int(11) DEFAULT NULL,
  `FlightIncluded` tinyint(1) NOT NULL DEFAULT 0,
  `FlightDetails` text DEFAULT NULL,
  `InsuranceIncluded` tinyint(1) NOT NULL DEFAULT 0,
  `InsurancePrice` decimal(10,2) DEFAULT NULL,
  `SpecialRequests` text DEFAULT NULL,
  `AdditionalNotes` text DEFAULT NULL,
  `Status` varchar(50) NOT NULL DEFAULT 'Pending',
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp(),
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='הצעות מחיר לטיולים';

--
-- הוצאת מידע עבור טבלה `tripoffers`
--

INSERT INTO `tripoffers` (`TripOfferId`, `CustomerId`, `TripId`, `OfferNumber`, `OfferDate`, `Participants`, `DepartureDate`, `ReturnDate`, `PricePerPerson`, `SingleRoomSupplement`, `SingleRooms`, `TotalPrice`, `PaymentMethodId`, `PaymentInstallments`, `FlightIncluded`, `FlightDetails`, `InsuranceIncluded`, `InsurancePrice`, `SpecialRequests`, `AdditionalNotes`, `Status`, `CreatedAt`, `UpdatedAt`) VALUES
(1, 1, 1, 'TRIP-20251116-1324', '2025-11-16 23:34:38', 2, '2025-12-16', '2025-12-24', 2150.00, NULL, 0, 4300.00, 2, 3, 1, 'אלעל E450 SY 15 ', 0, NULL, 'ללא', NULL, 'Approved', '2025-11-16 23:34:38', '2025-11-28 20:54:52'),
(2, 1, 2, 'TRIP-20251122-1965', '2025-11-22 16:36:52', 8, '2025-12-02', '2025-12-12', 2650.00, 400.00, 0, 21200.00, 3, 15, 1, 'elalY57  - 12.00 - 19.50\r\nelalAX75 - 17.00 - 23.50\r\nטיסות ישירות', 0, NULL, 'רגישות לאגוזים וביצים', 'קבוצה אורגנית', 'Pending', '2025-11-22 16:36:52', NULL),
(3, 1, 3, 'TRIP-20251122-4711', '2025-11-22 16:46:58', 6, '2026-01-07', '2026-01-15', 2600.00, 400.00, 0, 15600.00, 3, 10, 1, '07.01.2026 08:10\r\n07.01.2026 15:05\r\n\r\nDep: TEL AVIV TERMINAL 3\r\nArr: ZANZIBAR\r\nISRAIR AIRLINES – 6H 911 Class: T*\r\n\r\n14.01.2026 16:05\r\n14.01.2026 21:05\r\n\r\nDep: ZANZIBAR\r\nArr: TEL AVIV\r\nISRAIR AIRLINES – 6H 912 Class: T*', 0, NULL, NULL, NULL, 'Rejected', '2025-11-22 16:46:58', '2025-11-29 23:31:14'),
(4, 1, 3, 'TRIP-20251128-5077', '2025-11-28 17:30:32', 2, '2025-12-28', '2026-01-05', 4200.00, 400.00, 0, 8400.00, 3, 12, 1, 'טיסות ישירות של אלעל מנתב\"ג\r\nתביאו איתכם סנדוויצ\'ים', 0, NULL, 'אין בקשות מיוחדות', 'הערות פנימיות זה כאן', 'Pending', '2025-11-28 17:30:32', '2025-11-28 17:48:02');

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `trips`
--

CREATE TABLE `trips` (
  `TripId` int(11) NOT NULL,
  `Title` varchar(255) NOT NULL,
  `Description` text DEFAULT NULL,
  `NumberOfDays` int(11) NOT NULL,
  `GuideId` int(11) DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp(),
  `IsActive` tinyint(1) NOT NULL DEFAULT 1,
  `PricePerPerson` decimal(10,2) DEFAULT NULL,
  `PriceDescription` varchar(500) DEFAULT NULL,
  `Includes` text DEFAULT NULL,
  `Excludes` text DEFAULT NULL,
  `FlightDetails` text DEFAULT NULL
) ;

--
-- הוצאת מידע עבור טבלה `trips`
--

INSERT INTO `trips` (`TripId`, `Title`, `Description`, `NumberOfDays`, `GuideId`, `CreatedAt`, `IsActive`, `PricePerPerson`, `PriceDescription`, `Includes`, `Excludes`, `FlightDetails`) VALUES
(1, 'טיול מאורגן ליוון - אתונה והאיים', 'טיול מקיף ביוון הכולל ביקור באתונה, מיקונוס וסנטוריני. שילוב מושלם של היסטוריה, תרבות, חופים וחיי לילה תוססים.', 8, 5, '2025-11-16 22:48:12', 1, 2150.00, '2150$ לאדם בחדר זוגי (תוספת יחיד: 450$)', 'לינה ב-5 לילות במלונות 4 כוכבים באתונה ומיקונוס\r\nלינה ב-2 לילות במלון מערות בסנטוריני\r\nארוחות בוקר בכל המלונות\r\nהעברות.\r\nטיסות פנימיות באלעל\r\nמדריך ישראלי מקצועי לאורך כל הטיול\r\nכניסה לאקרופוליס והמוזיאון הארכיאולוגי\r\nסיור מודרך באתונה העתיקה\r\nשייט לשקיעה בסנטוריני\r\nביטוח רפואי בסיסי', 'כרטיסי טיסה בינלאומיים\r\nויזה (ישראלים פטורים)\r\nארוחות צהריים וערב\r\nמשקאות אלכוהוליים\r\nכניסות לאתרים נוספים שלא צוינו\r\nהוצאות אישיות וקניות\r\nטיפים למדריך ונהגים (מומלץ 50-70$ לאדם)\r\nביטוח ביטול נסיעה', 'טיסות זמינות (תל אביב ⇄ אתונה):\r\n\r\nאל על - LY361/LY362\r\nיציאה: ימים א׳,ג׳,ה׳ בשעה 14:30, נחיתה: 16:45\r\nחזרה: ימים ב׳,ד׳,ו׳ בשעה 17:45, נחיתה: 21:00\r\nמחיר: כ-950$ הלוך-חזור\r\n\r\nאיגאיאן איירליינס - A3741/A3742\r\nיציאה: יומי בשעה 11:00, נחיתה: 13:15\r\nחזרה: יומי בשעה 14:15, נחיתה: 17:30\r\nמחיר: כ-750$ הלוך-חזור\r\n\r\nויזאייר - W62551/W62552 (טיסה זולה)\r\nיציאה: ימים ב׳,ה׳,ש׳ בשעה 06:30, נחיתה: 08:45\r\nחזרה: ימים ג׳,ו׳,א׳ בשעה 09:45, נחיתה: 13:00\r\nמחיר: כ-450-600$ הלוך-חזור (ללא כבודה)'),
(2, 'טיול מאורגן לספרד - מדריד ברצלונה ואנדלוסיה', 'טיול מקיף בספרד הכולל את הערים המרכזיות והאזורים היפים ביותר. שילוב של אמנות, היסטוריה, תרבות, אדריכלות מרהיבה, אוכל מצוין ופלמנקו.', 10, 4, '2025-11-16 22:48:12', 1, 2850.00, '2850$ לאדם בחדר זוגי (תוספת יחיד: 550$)', 'לינה ב-9 לילות במלונות 4 כוכבים מרכזיים\r\nארוחות בוקר בכל המלונות\r\nהסעות באוטובוס תיירים מפואר עם WiFi\r\nמדריך ישראלי דובר עברית לכל אורך הטיול\r\nכניסה למוזיאון הפראדו במדריד\r\nכניסה לסגרדה פמיליה ופארק גואל בברצלונה\r\nכניסה לארמון אלהמברה בגרנדה\r\nכניסה לקתדרלת סביליה והאלקסר\r\nמופע פלמנקו אותנטי בסביליה כולל משקה\r\n', 'כרטיסי טיסה בינלאומיים\r\nארוחות צהריים וערב (מומלץ 40-50€ ליום)\r\nמשקאות אלכוהוליים\r\nכניסות לאתרים נוספים שלא צוינו\r\nאוידיו-גיידים באתרים (3-5€ למקום)\r\nהוצאות אישיות וקניות\r\nטיפים למדריך ונהגים (מומלץ 80-100$ לאדם לטיול)\r\nביטוח ביטול ושינויים', 'טיסות זמינות (תל אביב ⇄ מדריד):\r\n\r\nאל על - LY395/LY396\r\nיציאה: ימים א׳,ג׳,ה׳ בשעה 23:50, נחיתה: 05:30+1\r\nחזרה: ימים ב׳,ד׳,ו׳ בשעה 07:30, נחיתה: 13:45\r\nמחיר: כ-1,100$ הלוך-חזור (טיסה ישירה)\r\n\r\nאיביריה - IB6821/IB6822 (בשיתוף אל על)\r\nיציאה: יומי בשעה 14:00, נחיתה: 19:30\r\nחזרה: יומי בשעה 11:00, נחיתה: 17:15\r\nמחיר: כ-950$ הלוך-חזור (טיסה ישירה)\r\n\r\nויזאייר - W65305/W65306 (טיסת לואו-קוסט)\r\nיציאה: ימים ב׳,ה׳,ש׳ בשעה 07:45, נחיתה: 13:15\r\nחזרה: ימים ג׳,ו׳,א׳ בשעה 14:15, נחיתה: 19:45\r\nמחיר: כ-550-750$ הלוך-חזור (ללא כבודה, טיסה עצירה בברצלונה)\r\n\r\nהערה: מומלץ להזמין טיסות לפחות 3 חודשים מראש לקבלת מחירים טובים.'),
(3, ' חופשה נפלאה בזנזיבר - ינואר 26', 'הצטרפו לחופשה אקזוטית באי התבלינים זנזיבר – יעד מושלם לשילוב של חופים טרופיים, טבע מרהיב ותרבות אפריקאית ססגונית. האי המיוחד הזה מציע שילוב נדיר של שלווה, צבעים, טעמים ונופים עוצרי נשימה. בין חופי הטורקיז, השווקים הססגוניים והשקיעות הזהובות – תגלו עולם אחר של חופש ורוגע.\r\nומי שמבקש להוסיף נגיעה של הרפתקה – יכול להרחיב את החופשה ביומיים של ספארי בטנזניה, בין נופי הסוואנה הפראיים ובעלי החיים האייקוניים של אפריקה.', 8, 6, '2025-11-22 16:44:17', 1, 2600.00, '400', 'טיסות שכר ישירות הלוך ושוב לזנזיבר.\r\nלינה בבית מלון מדרגת תיירות מעולה.\r\nמדריך ישראלי מומחה ליעד.\r\nכלכלה: חצי פנסיון ( ארוחות בוקר וערב )\r\nפעילויות בזנזיבר.\r\nטיפים לנהג ומדריך מקומי.', 'ביטוחים – ביטוח רפואי.\r\nהוצאות אישיות.\r\nאשרת כניסה בסך 51$ מומלץ להוציא אשרה מראש, ניתן להוציא אשרה דרכנו בעלות של 20$ לאדם.\r\nמס תשתיות בגובה 1.5$ לאדם ללילה המשולם ישירות בבית המלון, \"מס ירוק\" בסכום זהה. סה\"כ 3.5$ לאדם ללילה.\r\nטיפים לנותני שירות אחרים: סבלים, מלצרים ומשיטי סירות.\r\nטיפ למדריך הישראלי (מקובל לתת 5$ לאדם ליום).\r\nביטוח לזנזיבר – חובה לעשות 44$ לאדם', '07.01.2026 08:10\r\n07.01.2026 15:05\r\n\r\nDep: TEL AVIV TERMINAL 3\r\nArr: ZANZIBAR\r\nISRAIR AIRLINES – 6H 911 Class: T*\r\n\r\n14.01.2026 16:05\r\n14.01.2026 21:05\r\n\r\nDep: ZANZIBAR\r\nArr: TEL AVIV\r\nISRAIR AIRLINES – 6H 912 Class: T*'),
(6, 'ias fig fi s agisgss', 'gsdz fpiuiohgsuioh osuahuiosdh uiohsdguo nhgsd', 5, 4, '2025-12-02 17:04:11', 1, 78687687.00, '86778', 'hdshdshds', 'sdfgsd fghdh  hd h', 'hds hdgh h dgh ');

-- --------------------------------------------------------

--
-- מבנה טבלה עבור טבלה `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- הוצאת מידע עבור טבלה `__efmigrationshistory`
--

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
('20250922134437_InitialIdentity', '9.0.9'),
('20250922140801_AddCustomFieldsToUsers', '9.0.9'),
('20250922164229_CreateCustomersTableOnly', '9.0.9'),
('20250922164803_MigrationCustomer', '9.0.9');

--
-- Indexes for dumped tables
--

--
-- אינדקסים לטבלה `aspnetroleclaims`
--
ALTER TABLE `aspnetroleclaims`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_AspNetRoleClaims_RoleId` (`RoleId`);

--
-- אינדקסים לטבלה `aspnetroles`
--
ALTER TABLE `aspnetroles`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `RoleNameIndex` (`NormalizedName`);

--
-- אינדקסים לטבלה `aspnetuserclaims`
--
ALTER TABLE `aspnetuserclaims`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_AspNetUserClaims_UserId` (`UserId`);

--
-- אינדקסים לטבלה `aspnetuserlogins`
--
ALTER TABLE `aspnetuserlogins`
  ADD PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  ADD KEY `IX_AspNetUserLogins_UserId` (`UserId`);

--
-- אינדקסים לטבלה `aspnetuserroles`
--
ALTER TABLE `aspnetuserroles`
  ADD PRIMARY KEY (`UserId`,`RoleId`),
  ADD KEY `IX_AspNetUserRoles_RoleId` (`RoleId`);

--
-- אינדקסים לטבלה `aspnetusers`
--
ALTER TABLE `aspnetusers`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  ADD KEY `EmailIndex` (`NormalizedEmail`);

--
-- אינדקסים לטבלה `aspnetusertokens`
--
ALTER TABLE `aspnetusertokens`
  ADD PRIMARY KEY (`UserId`,`LoginProvider`,`Name`);

--
-- אינדקסים לטבלה `cancellations`
--
ALTER TABLE `cancellations`
  ADD PRIMARY KEY (`CancellationId`),
  ADD KEY `OrderId` (`OrderId`);

--
-- אינדקסים לטבלה `customers`
--
ALTER TABLE `customers`
  ADD PRIMARY KEY (`CustomerId`),
  ADD UNIQUE KEY `uq_customers_idnumber` (`IDNumber`),
  ADD UNIQUE KEY `uq_customers_email` (`Email`);

--
-- אינדקסים לטבלה `guides`
--
ALTER TABLE `guides`
  ADD PRIMARY KEY (`GuideId`),
  ADD KEY `OrderId` (`OrderId`);

--
-- אינדקסים לטבלה `itineraries`
--
ALTER TABLE `itineraries`
  ADD PRIMARY KEY (`ItineraryId`),
  ADD UNIQUE KEY `uq_itineraries_tour` (`TourId`);

--
-- אינדקסים לטבלה `itinerary`
--
ALTER TABLE `itinerary`
  ADD PRIMARY KEY (`ItineraryId`),
  ADD UNIQUE KEY `uq_itinerary_tour` (`TourId`),
  ADD KEY `OrderId` (`OrderId`);

--
-- אינדקסים לטבלה `itineraryitems`
--
ALTER TABLE `itineraryitems`
  ADD PRIMARY KEY (`ItemId`),
  ADD KEY `idx_itineraryitems_itineraryid` (`ItineraryId`);

--
-- אינדקסים לטבלה `offers`
--
ALTER TABLE `offers`
  ADD PRIMARY KEY (`OfferId`),
  ADD KEY `CustomerId` (`CustomerId`),
  ADD KEY `GuideId` (`GuideId`),
  ADD KEY `TourId` (`TourId`),
  ADD KEY `PaymentMethodId` (`PaymentMethodId`),
  ADD KEY `offers_ibfk_4` (`PaymentId`);

--
-- אינדקסים לטבלה `orderincludes`
--
ALTER TABLE `orderincludes`
  ADD PRIMARY KEY (`IncludeId`),
  ADD KEY `OrderId` (`OrderId`);

--
-- אינדקסים לטבלה `orders`
--
ALTER TABLE `orders`
  ADD PRIMARY KEY (`OrderId`),
  ADD KEY `CustomerId` (`CustomerId`);

--
-- אינדקסים לטבלה `paymentmethod`
--
ALTER TABLE `paymentmethod`
  ADD PRIMARY KEY (`ID`);

--
-- אינדקסים לטבלה `payments`
--
ALTER TABLE `payments`
  ADD PRIMARY KEY (`PaymentId`),
  ADD KEY `OrderId` (`OrderId`);

--
-- אינדקסים לטבלה `tourexclude`
--
ALTER TABLE `tourexclude`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `TourId` (`TourId`);

--
-- אינדקסים לטבלה `tourinclude`
--
ALTER TABLE `tourinclude`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `TourId` (`TourId`);

--
-- אינדקסים לטבלה `tours`
--
ALTER TABLE `tours`
  ADD PRIMARY KEY (`TourId`),
  ADD KEY `OrderId` (`OrderId`);

--
-- אינדקסים לטבלה `tripdays`
--
ALTER TABLE `tripdays`
  ADD PRIMARY KEY (`TripDayId`),
  ADD UNIQUE KEY `uq_tripdays_trip_day` (`TripId`,`DayNumber`),
  ADD KEY `idx_tripdays_tripid` (`TripId`),
  ADD KEY `idx_tripdays_daynumber` (`DayNumber`);

--
-- אינדקסים לטבלה `tripoffers`
--
ALTER TABLE `tripoffers`
  ADD PRIMARY KEY (`TripOfferId`),
  ADD UNIQUE KEY `unique_offer_number` (`OfferNumber`),
  ADD KEY `idx_customer` (`CustomerId`),
  ADD KEY `idx_trip` (`TripId`),
  ADD KEY `idx_payment` (`PaymentMethodId`),
  ADD KEY `idx_status` (`Status`),
  ADD KEY `idx_date` (`DepartureDate`);

--
-- אינדקסים לטבלה `trips`
--
ALTER TABLE `trips`
  ADD PRIMARY KEY (`TripId`),
  ADD KEY `idx_trips_guideid` (`GuideId`);

--
-- אינדקסים לטבלה `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `aspnetroleclaims`
--
ALTER TABLE `aspnetroleclaims`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `aspnetuserclaims`
--
ALTER TABLE `aspnetuserclaims`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `cancellations`
--
ALTER TABLE `cancellations`
  MODIFY `CancellationId` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `customers`
--
ALTER TABLE `customers`
  MODIFY `CustomerId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `guides`
--
ALTER TABLE `guides`
  MODIFY `GuideId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT for table `itineraries`
--
ALTER TABLE `itineraries`
  MODIFY `ItineraryId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;

--
-- AUTO_INCREMENT for table `itinerary`
--
ALTER TABLE `itinerary`
  MODIFY `ItineraryId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `itineraryitems`
--
ALTER TABLE `itineraryitems`
  MODIFY `ItemId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=127;

--
-- AUTO_INCREMENT for table `offers`
--
ALTER TABLE `offers`
  MODIFY `OfferId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=23;

--
-- AUTO_INCREMENT for table `orderincludes`
--
ALTER TABLE `orderincludes`
  MODIFY `IncludeId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `orders`
--
ALTER TABLE `orders`
  MODIFY `OrderId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `paymentmethod`
--
ALTER TABLE `paymentmethod`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `payments`
--
ALTER TABLE `payments`
  MODIFY `PaymentId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `tourexclude`
--
ALTER TABLE `tourexclude`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=56;

--
-- AUTO_INCREMENT for table `tourinclude`
--
ALTER TABLE `tourinclude`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=51;

--
-- AUTO_INCREMENT for table `tours`
--
ALTER TABLE `tours`
  MODIFY `TourId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=29;

--
-- AUTO_INCREMENT for table `tripdays`
--
ALTER TABLE `tripdays`
  MODIFY `TripDayId` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `tripoffers`
--
ALTER TABLE `tripoffers`
  MODIFY `TripOfferId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `trips`
--
ALTER TABLE `trips`
  MODIFY `TripId` int(11) NOT NULL AUTO_INCREMENT;

--
-- הגבלות לטבלאות שהוצאו
--

--
-- הגבלות לטבלה `aspnetroleclaims`
--
ALTER TABLE `aspnetroleclaims`
  ADD CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE;

--
-- הגבלות לטבלה `aspnetuserclaims`
--
ALTER TABLE `aspnetuserclaims`
  ADD CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE;

--
-- הגבלות לטבלה `aspnetuserlogins`
--
ALTER TABLE `aspnetuserlogins`
  ADD CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE;

--
-- הגבלות לטבלה `aspnetuserroles`
--
ALTER TABLE `aspnetuserroles`
  ADD CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE;

--
-- הגבלות לטבלה `aspnetusertokens`
--
ALTER TABLE `aspnetusertokens`
  ADD CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE;

--
-- הגבלות לטבלה `cancellations`
--
ALTER TABLE `cancellations`
  ADD CONSTRAINT `cancellations_ibfk_1` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`OrderId`);

--
-- הגבלות לטבלה `guides`
--
ALTER TABLE `guides`
  ADD CONSTRAINT `guides_ibfk_1` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`OrderId`);

--
-- הגבלות לטבלה `itineraries`
--
ALTER TABLE `itineraries`
  ADD CONSTRAINT `fk_itineraries_tours` FOREIGN KEY (`TourId`) REFERENCES `tours` (`TourId`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- הגבלות לטבלה `itinerary`
--
ALTER TABLE `itinerary`
  ADD CONSTRAINT `fk_itinerary_tour` FOREIGN KEY (`TourId`) REFERENCES `tours` (`TourId`),
  ADD CONSTRAINT `itinerary_ibfk_1` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`OrderId`);

--
-- הגבלות לטבלה `itineraryitems`
--
ALTER TABLE `itineraryitems`
  ADD CONSTRAINT `fk_itineraryitems_itineraries` FOREIGN KEY (`ItineraryId`) REFERENCES `itineraries` (`ItineraryId`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- הגבלות לטבלה `offers`
--
ALTER TABLE `offers`
  ADD CONSTRAINT `offers_ibfk_1` FOREIGN KEY (`CustomerId`) REFERENCES `customers` (`CustomerId`),
  ADD CONSTRAINT `offers_ibfk_2` FOREIGN KEY (`GuideId`) REFERENCES `guides` (`GuideId`),
  ADD CONSTRAINT `offers_ibfk_3` FOREIGN KEY (`TourId`) REFERENCES `tours` (`TourId`),
  ADD CONSTRAINT `offers_ibfk_5` FOREIGN KEY (`PaymentMethodId`) REFERENCES `paymentmethod` (`ID`);

--
-- הגבלות לטבלה `orderincludes`
--
ALTER TABLE `orderincludes`
  ADD CONSTRAINT `orderincludes_ibfk_1` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`OrderId`);

--
-- הגבלות לטבלה `orders`
--
ALTER TABLE `orders`
  ADD CONSTRAINT `orders_ibfk_1` FOREIGN KEY (`CustomerId`) REFERENCES `customers` (`CustomerId`);

--
-- הגבלות לטבלה `payments`
--
ALTER TABLE `payments`
  ADD CONSTRAINT `payments_ibfk_1` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`OrderId`);

--
-- הגבלות לטבלה `tourexclude`
--
ALTER TABLE `tourexclude`
  ADD CONSTRAINT `tourexclude_ibfk_1` FOREIGN KEY (`TourId`) REFERENCES `tours` (`TourId`) ON DELETE CASCADE;

--
-- הגבלות לטבלה `tourinclude`
--
ALTER TABLE `tourinclude`
  ADD CONSTRAINT `tourinclude_ibfk_1` FOREIGN KEY (`TourId`) REFERENCES `tours` (`TourId`) ON DELETE CASCADE;

--
-- הגבלות לטבלה `tours`
--
ALTER TABLE `tours`
  ADD CONSTRAINT `tours_ibfk_1` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`OrderId`);

--
-- הגבלות לטבלה `tripdays`
--
ALTER TABLE `tripdays`
  ADD CONSTRAINT `fk_tripdays_trips` FOREIGN KEY (`TripId`) REFERENCES `trips` (`TripId`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- הגבלות לטבלה `tripoffers`
--
ALTER TABLE `tripoffers`
  ADD CONSTRAINT `fk_tripoffer_customer` FOREIGN KEY (`CustomerId`) REFERENCES `customers` (`CustomerId`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_tripoffer_payment` FOREIGN KEY (`PaymentMethodId`) REFERENCES `paymentmethod` (`ID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_tripoffer_trip` FOREIGN KEY (`TripId`) REFERENCES `trips` (`TripId`) ON UPDATE CASCADE;

--
-- הגבלות לטבלה `trips`
--
ALTER TABLE `trips`
  ADD CONSTRAINT `fk_trips_guide` FOREIGN KEY (`GuideId`) REFERENCES `guides` (`GuideId`) ON DELETE SET NULL ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
