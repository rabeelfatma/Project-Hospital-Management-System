CREATE DATABASE olx;
USE olx;
-- Patients Table
CREATE TABLE patients (
    patientid INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    gender NVARCHAR(20) NOT NULL,
    address NVARCHAR(200) NOT NULL,
    password NVARCHAR(50) NOT NULL,
    email NVARCHAR(200) NOT NULL UNIQUE,
    registrationdatetime DATETIME NOT NULL
);
INSERT INTO patients (name, gender, address, password, email, registrationdatetime) VALUES
('Ali Khan', 'Male', '123 Street A, Lahore', 'pass123', 'ali.khan@example.com', '2025-06-09 08:15:00'),
('Sara Malik', 'Female', '456 Street B, Karachi', 'pass456', 'sara.malik@example.com', '2025-06-09 09:30:00'),
('Zain Abbas', 'Male', '789 Street C, Islamabad', 'pass789', 'zain.abbas@example.com', '2025-06-09 10:45:00');
--Doctors Table
CREATE TABLE doctors (
    doctorid INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    email NVARCHAR(200) NOT NULL UNIQUE,
    gender NVARCHAR(20) NOT NULL,
    password NVARCHAR(50) NOT NULL,
    specialization NVARCHAR(100) NOT NULL,
    joiningdatetime DATETIME NOT NULL
);
-- Insert Doctors Data
INSERT INTO doctors (name, email, gender, password, specialization, joiningdatetime) VALUES
('Dr. Ahmed', 'ahmed@example.com', 'Male', 'doc123', 'Cardiology', '2025-06-08 09:00:00'),
('Dr. Ayesha', 'ayesha@example.com', 'Female', 'doc456', 'Dermatology', '2025-06-08 10:00:00'),
('Dr. Bilal', 'bilal@example.com', 'Male', 'doc789', 'Neurology', '2025-06-08 11:00:00');
-- Appointments Table
CREATE TABLE appointments (
    appointmentid INT IDENTITY(1,1) PRIMARY KEY,
    patientid INT NOT NULL,
    doctorid INT NOT NULL,
    appointmentdatetime DATETIME NOT NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    FOREIGN KEY (patientid) REFERENCES patients(patientid)
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (doctorid) REFERENCES doctors(doctorid)
        ON DELETE CASCADE ON UPDATE CASCADE
);
INSERT INTO appointments (patientid, doctorid, appointmentdatetime, status) VALUES
(1, 1, '2025-06-10 10:00:00', 'Pending'),
(2, 2, '2025-06-11 11:30:00', 'Accepted'),
(3, 3, '2025-06-12 14:15:00', 'Rejected');
-- Medical Records Table
CREATE TABLE medicalrecords (
    recordid INT IDENTITY(1,1) PRIMARY KEY,
    patientid INT NOT NULL,
    doctorid INT NOT NULL,
    recorddatetime DATETIME NOT NULL,
    condition NVARCHAR(255) NOT NULL,
    FOREIGN KEY (patientid) REFERENCES patients(patientid)
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (doctorid) REFERENCES doctors(doctorid)
        ON DELETE CASCADE ON UPDATE CASCADE
);
INSERT INTO medicalrecords (patientid, doctorid, recorddatetime, condition) VALUES
(1, 1, '2025-06-09 10:30:00', 'High Blood Pressure'),
(2, 2, '2025-06-10 14:45:00', 'Skin Allergy'),
(3, 3, '2025-06-11 09:15:00', 'Migraine');
-- Bills Table
CREATE TABLE bills (
    billid INT IDENTITY(1,1) PRIMARY KEY,
    patientid INT NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    paymentstatus NVARCHAR(20) NOT NULL,
    billdatetime DATETIME NOT NULL,
    FOREIGN KEY (patientid) REFERENCES patients(patientid)
        ON DELETE CASCADE ON UPDATE CASCADE
);

INSERT INTO bills (patientid, amount, paymentstatus, billdatetime) VALUES
(1, 5000.00, 'Paid', '2025-06-09 12:00:00'),
(2, 7000.00, 'Unpaid', '2025-06-10 13:30:00'),
(3, 3000.00, 'Paid', '2025-06-11 15:45:00');
CREATE TABLE diagnosis (
    diagnosisid INT IDENTITY(1,1) PRIMARY KEY,
    patientid INT NOT NULL,
    doctorid INT NOT NULL,
    diagnosisdatetime DATETIME NOT NULL,
    condition NVARCHAR(255) NOT NULL,
    medication NVARCHAR(500),
    surgeries NVARCHAR(500)
);
INSERT INTO diagnosis (patientid, doctorid, diagnosisdatetime, condition, medication, surgeries)
VALUES 
(1, 2, GETDATE(), 'Flu', 'Paracetamol, Rest', 'None'),
(3, 4, GETDATE(), 'Fracture', 'Painkillers', 'Arm Surgery'),
(5, 2, GETDATE(), 'Diabetes', 'Insulin Therapy', 'None');



-- Users Table
CREATE TABLE users (
    userid INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(50) UNIQUE NOT NULL,
    password NVARCHAR(255) NOT NULL,
    role NVARCHAR(20) NOT NULL
);
INSERT INTO users (username, password, role)
VALUES
('saman', '123456', 'ADMIN'),
('Ali', '565656', 'Doctor');
