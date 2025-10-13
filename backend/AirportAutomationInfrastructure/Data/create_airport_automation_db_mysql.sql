-- Drop the database if it exists
DROP DATABASE IF EXISTS AirportAutomation;

-- Create the database
CREATE DATABASE AirportAutomation;

-- Use the database
USE AirportAutomation;

-- Begin a transaction
START TRANSACTION;

-- Create the tables
CREATE TABLE Passenger (
    Id INT NOT NULL AUTO_INCREMENT,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    UPRN VARCHAR(13) NOT NULL UNIQUE,
    Passport VARCHAR(9) NOT NULL UNIQUE,
    Address VARCHAR(200) NOT NULL,
    Phone VARCHAR(30) NOT NULL,
    PRIMARY KEY (Id)
);

CREATE TABLE TravelClass (
    Id INT NOT NULL AUTO_INCREMENT,
    Type VARCHAR(20) NOT NULL,
    PRIMARY KEY (Id)
);

CREATE TABLE Destination (
    Id INT NOT NULL AUTO_INCREMENT,
    City VARCHAR(255) NOT NULL,
    Airport VARCHAR(255) NOT NULL,
    PRIMARY KEY (Id)
);

CREATE TABLE Pilot (
    Id INT NOT NULL AUTO_INCREMENT,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    UPRN VARCHAR(13) NOT NULL,
    FlyingHours INT NOT NULL,
    PRIMARY KEY (Id)
);

CREATE TABLE Airline (
    Id INT NOT NULL AUTO_INCREMENT,
    Name VARCHAR(255) NOT NULL,
    PRIMARY KEY (Id)
);

CREATE TABLE Flight (
    Id INT NOT NULL AUTO_INCREMENT,
    DepartureDate DATE NOT NULL,
    DepartureTime TIME NOT NULL,
    AirlineId INT,
    DestinationId INT,
    PilotId INT,
    PRIMARY KEY (Id),
    FOREIGN KEY (AirlineId) REFERENCES Airline (Id),
    FOREIGN KEY (DestinationId) REFERENCES Destination (Id),
    FOREIGN KEY (PilotId) REFERENCES Pilot (Id)
);

CREATE TABLE PlaneTicket (
    Id INT NOT NULL AUTO_INCREMENT,
    Price DECIMAL(8,2) NOT NULL,
    PurchaseDate DATE NOT NULL,
    SeatNumber INT NOT NULL,
    PassengerId INT,
    TravelClassId INT,
    FlightId INT,
    PRIMARY KEY (Id),
    FOREIGN KEY (PassengerId) REFERENCES Passenger (Id),
    FOREIGN KEY (TravelClassId) REFERENCES TravelClass (Id),
    FOREIGN KEY (FlightId) REFERENCES Flight (Id)
);

CREATE TABLE ApiUser (
    ApiUserId INT NOT NULL AUTO_INCREMENT,
    UserName VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(100) NOT NULL,
    Roles VARCHAR(50) NOT NULL,
    PRIMARY KEY (ApiUserId)
);

-- Insert data into the tables
INSERT INTO Passenger (FirstName, LastName, UPRN, Passport, Address, Phone)
VALUES
    ('John', 'Doe', '1234567890123', 'P12345678', '123 Main Street, New York, United States', '123-456-7890'),
    ('Jane', 'Smith', '9876543210987', 'S87654321', '456 Elm Street, London, United Kingdom', '987-654-3210'),
    ('David', 'Johnson', '4567890123456', 'P98765432', '789 Oak Avenue, Paris, France', '456-789-0123'),
    ('Sarah', 'Williams', '8901234567890', 'S54321098', '321 Pine Road, Sydney, Australia', '890-123-4567'),
    ('Michael', 'Brown', '5678901234567', 'P87654321', '654 Maple Lane, Tokyo, Japan', '567-890-1234'),
    ('Emily', 'Jones', '2345678901234', 'S09876543', '987 Cedar Drive, Rome, Italy', '234-567-8901'),
    ('Daniel', 'Davis', '7890123456789', 'P43210987', '456 Birch Street, Berlin, Germany', '789-012-3456'),
    ('Olivia', 'Miller', '3456789012345', 'S76543210', '321 Oak Avenue, Madrid, Spain', '345-678-9012'),
    ('Matthew', 'Wilson', '9012345678901', 'P21098765', '654 Pine Road, Toronto, Canada', '901-234-5678'),
    ('Sophia', 'Anderson', '6789012345678', 'S54321198', '987 Elm Street, Dubai, United Arab Emirates', '678-901-2345'),
    ('William', 'Johnson', '1234567890124', 'P12345679', '124 Oak Street, New York, United States', '111-222-3333'),
    ('Emma', 'Davis', '9876543210988', 'S87654322', '457 Maple Lane, London, United Kingdom', '444-555-6666'),
    ('Liam', 'Brown', '4567890123457', 'P98765433', '790 Pine Road, Paris, France', '777-888-9999'),
    ('Ava', 'Williams', '8901234567891', 'S54321099', '322 Cedar Drive, Sydney, Australia', '000-111-2222'),
    ('James', 'Smith', '5678901234568', 'P87654324', '655 Elm Street, Tokyo, Japan', '333-444-5555'),
    ('Charlotte', 'Jones', '2345678901235', 'S09876544', '988 Main Street, Rome, Italy', '666-777-8888'),
    ('Lucas', 'Taylor', '7890123456790', 'P43210988', '457 Birch Street, Berlin, Germany', '999-000-1111'),
    ('Amelia', 'White', '3456789012346', 'S76543211', '322 Oak Avenue, Madrid, Spain', '222-333-4444'),
    ('Benjamin', 'Clark', '9012345678902', 'P21098766', '655 Pine Road, Toronto, Canada', '555-666-7777'),
    ('Mia', 'Harris', '6789012345679', 'S54321000', '988 Elm Street, Dubai, United Arab Emirates', '888-999-0000'),
    ('Emma', 'Johnson', '1357924680246', 'P13579246', '567 Pine Street, New York, United States', '111-222-3333'),
    ('Daniel', 'Martinez', '2468013579135', 'S24680135', '789 Elm Street, Los Angeles, United States', '444-555-6666'),
    ('Olivia', 'Garcia', '9876543210001', 'P98765442', '123 Oak Avenue, Chicago, United States', '777-888-9999'),
    ('Liam', 'Rodriguez', '5555555555555', 'S55555555', '456 Birch Lane, Miami, United States', '000-111-2222'),
    ('Sophia', 'Brown', '1231231231231', 'P12312312', '789 Cedar Drive, San Francisco, United States', '333-444-5555');

INSERT INTO TravelClass (Type)
VALUES
    ('Economy'),
    ('Premium Economy'),
    ('Business'),
    ('First');

INSERT INTO Destination (City, Airport)
VALUES
    ('New York', 'John F. Kennedy International Airport'),
    ('London', 'Heathrow Airport'),
    ('Paris', 'Charles de Gaulle Airport'),
    ('Sydney', 'Sydney Airport'),
    ('Belgrade', 'Belgrade Nikola Tesla Airport'),
    ('Rome', 'Leonardo da Vinci-Fiumicino Airport'),
    ('Tokyo', 'Narita International Airport'),
    ('Dubai', 'Dubai International Airport'),
    ('Beijing', 'Beijing Capital International Airport'),
    ('Los Angeles', 'Los Angeles International Airport'),
    ('Singapore', 'Changi Airport'),
    ('Istanbul', 'Istanbul Airport'),
    ('Amsterdam', 'Amsterdam Airport Schiphol'),
    ('Hong Kong', 'Hong Kong International Airport'),
    ('Mumbai', 'Chhatrapati Shivaji Maharaj International Airport'),
    ('Toronto', 'Toronto Pearson International Airport');

INSERT INTO Pilot (FirstName, LastName, UPRN, FlyingHours)
VALUES
    ('Alex', 'Walker', '1234567890123', 10),
    ('Olivia', 'Harris', '9876543210987', 8),
    ('Ethan', 'Clark', '4567890123456', 12),
    ('Sophia', 'White', '8901234567890', 15),
    ('Liam', 'Martin', '5678901234567', 6),
    ('Ava', 'Anderson', '2345678901234', 9),
    ('Noah', 'Thompson', '7890123456789', 11),
    ('Emma', 'Baker', '2345678901235', 14),
    ('Mason', 'Hill', '7890123456788', 18),
    ('Chloe', 'Young', '3456789012344', 20),
    ('Carter', 'Walker', '9012345678900', 13),
    ('Grace', 'Lewis', '6789012345671', 16);

INSERT INTO Airline (Name)
VALUES
    ('Delta Air Lines'),
    ('American Airlines'),
    ('United Airlines'),
    ('Lufthansa'),
    ('Emirates'),
    ('British Airways'),
    ('Air France'),
    ('Qatar Airways'),
    ('Singapore Airlines'),
    ('Cathay Pacific'),
    ('Virgin Atlantic'),
    ('Southwest Airlines'),
    ('JetBlue Airways'),
    ('KLM Royal Dutch Airlines'),
    ('ANA All Nippon Airways');

INSERT INTO Flight (DepartureDate, DepartureTime, AirlineId, DestinationId, PilotId)
VALUES
    ('2025-08-07', '08:30:00', 1, 1, 1),
    ('2025-08-08', '12:45:00', 2, 2, 2),
    ('2025-08-09', '16:20:00', 3, 3, 3),
    ('2025-08-10', '09:15:00', 4, 4, 4),
    ('2025-08-11', '14:55:00', 5, 5, 5),
    ('2025-08-12', '10:10:00', 6, 6, 6),
    ('2025-08-13', '11:30:00', 7, 7, 7),
    ('2025-08-14', '15:40:00', 8, 8, 8),
    ('2025-08-15', '18:20:00', 9, 9, 9),
    ('2025-08-16', '20:05:00', 10, 10, 10);

INSERT INTO PlaneTicket (Price, PurchaseDate, SeatNumber, PassengerId, TravelClassId, FlightId)
VALUES
    (200.00, '2025-07-07', 15, 1, 1, 1),
    (300.00, '2025-07-08', 10, 2, 2, 2),
    (400.00, '2025-07-09', 7, 3, 3, 3),
    (500.00, '2025-07-10', 3, 4, 4, 4),
    (600.00, '2025-07-11', 6, 5, 2, 5),
    (700.00, '2025-07-12', 12, 6, 3, 6),
    (800.00, '2025-07-13', 5, 7, 1, 7),
    (900.00, '2025-07-14', 8, 8, 2, 8),
    (1000.00, '2025-07-15', 11, 9, 3, 9),
    (1100.00, '2025-07-16', 2, 10, 1, 10);

-- Username and Password are the same!
INSERT INTO ApiUser (UserName, Password, Roles)
VALUES
    ('og', '$2a$12$LCGwPPXCmcID1OU/ZkQtoOfEC5UMWxmvMPX4ja27X8eZ/tdTZ1v1y', 'SuperAdmin'),
    ('aa', '$2a$12$f4CJk4AtefsjoVLDmeGSxeM.K9exNoxMheSHIWyORCmIujPTMJuTG', 'Admin'),
    ('uu', '$2a$12$CcRyVR.Kzb10vj8hNOU9QOH7MtZ7d2BaxkCwSLVqADf/fRCHIf2ty', 'User');

-- Commit the transaction
COMMIT;