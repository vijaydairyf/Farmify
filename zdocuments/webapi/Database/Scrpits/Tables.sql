DROP TABLE IF EXISTS dbo.Users; 
GO
CREATE TABLE dbo.Users (
	   Id INT IDENTITY(1,1)
	 , FirstName NVARCHAR(100)
	 , LastName NVARCHAR(100)
	 , Email NVARCHAR(100)
	 , Phone NVARCHAR(20)
	 , UserType INT --1. Agent, 2. Farmer, 3. Investor, 
	 , OTP INT
	 , CreatedDate DATETIME DEFAULT(GETDATE())
	 , PasswordSalt VARBINARY(300)
	 , PasswordHash VARBINARY(300))
GO
DROP TABLE IF EXISTS dbo.UserType; 
GO
CREATE TABLE dbo.UserType(
	   Id INT IDENTITY(1,1)
	 , UserType NVARCHAR(20) NOT NULL
	 , CreatedDate DATETIME DEFAULT(GETDATE())
	 , CreatedBy NVARCHAR(100) NOT NULL)
GO
INSERT INTO dbo.UserType(UserType, CreatedBy)
SELECT 'Farmer', 'Naresh.Viriyala' UNION
SELECT 'Agent', 'Naresh.Viriyala' UNION
SELECT 'Investor', 'Naresh.Viriyala' 
GO
DROP TABLE IF EXISTS dbo.DeviceType; 
GO
CREATE TABLE dbo.DeviceType(
	   Id INT IDENTITY(1,1)
	 , DeviceType NVARCHAR(20) NOT NULL
	 , CreatedDate DATETIME DEFAULT(GETDATE())
	 , CreatedBy NVARCHAR(100) NOT NULL)
GO
INSERT INTO dbo.DeviceType(DeviceType, CreatedBy)
SELECT 'Android', 'Naresh.Viriyala' UNION
SELECT 'Iphone', 'Naresh.Viriyala' UNION
SELECT 'Windows', 'Naresh.Viriyala' 
GO
DROP TABLE IF EXISTS dbo.tbl_login_history; 
GO
CREATE TABLE dbo.tbl_login_history(
	   Id INT IDENTITY(1, 1) PRIMARY KEY
	 , UserId INT
	 , DeviceID NVARCHAR(100) 
	 , DeviceType INT --1. Android, 2. Iphone, 3. Windows
	 , DeviceVersion NVARCHAR(20) -- Lollipop, Kitkat
	 , AppVersion NVARCHAR(10)
	 , LoginTime DATETIME DEFAULT(GETDATE())
	 , Latitude DECIMAL(12,9)
	 , Longitude DECIMAL(12,9))
GO
DROP TABLE IF EXISTS dbo.tbl_errorlog; 
GO
CREATE TABLE dbo.tbl_errorlog(
	   Id INT IDENTITY(1,1)
	 , UserId INT
	 , Loginid INT
	 , CodeFile NVARCHAR(100)
	 --, ClassName NVARCHAR(500)
	 , MethodName NVARCHAR(500)
	 , Errordesc NVARCHAR(2000)
	 , InsertTime DATETIME DEFAULT(GETDATE()))
GO
DROP TABLE IF EXISTS dbo.tbl_mstr_caste; 
GO
CREATE TABLE dbo.tbl_mstr_caste(
	   Id INT IDENTITY(1,1) PRIMARY KEY
	 , CasteType NVARCHAR(20) NOT NULL
	 , CreatedDate DATETIME DEFAULT(GETDATE())
	 , CreatedBy NVARCHAR(100) NOT NULL)
GO
INSERT INTO dbo.tbl_mstr_caste(CasteType, CreatedBy)
SELECT 'General', 'Naresh.Viriyala' UNION
SELECT 'OBC', 'Naresh.Viriyala' UNION
SELECT 'SC', 'Naresh.Viriyala' UNION
SELECT 'ST', 'Naresh.Viriyala' UNION
SELECT 'Other', 'Naresh.Viriyala' 
GO
DROP TABLE IF EXISTS dbo.tbl_mstr_farmertype; 
GO
CREATE TABLE dbo.tbl_mstr_farmertype(
	   Id INT IDENTITY(1,1) PRIMARY KEY
	 , FarmerType NVARCHAR(20) NOT NULL
	 , CreatedDate DATETIME DEFAULT(GETDATE())
	 , CreatedBy NVARCHAR(100) NOT NULL)
GO
INSERT INTO dbo.tbl_mstr_farmertype(FarmerType, CreatedBy)
SELECT 'Small', 'Naresh.Viriyala' UNION
SELECT 'Marginal', 'Naresh.Viriyala' UNION
SELECT 'Other', 'Naresh.Viriyala' 
GO
DROP TABLE IF EXISTS dbo.tbl_mstr_farmercategory; 
GO
CREATE TABLE dbo.tbl_mstr_farmercategory(
	   Id INT IDENTITY(1,1) PRIMARY KEY
	 , FarmerCategory NVARCHAR(20) NOT NULL
	 , CreatedDate DATETIME DEFAULT(GETDATE())
	 , CreatedBy NVARCHAR(100) NOT NULL)
GO
INSERT INTO dbo.tbl_mstr_farmercategory(FarmerCategory, CreatedBy)
SELECT 'Owner', 'Naresh.Viriyala' UNION
SELECT 'Tenant', 'Naresh.Viriyala' UNION
SELECT 'SharedCropper', 'Naresh.Viriyala' 
GO
DROP TABLE IF EXISTS dbo.tbl_mstr_bankaccounttype
GO
CREATE TABLE dbo.tbl_mstr_bankaccounttype(
	   Id INT IDENTITY(1,1) PRIMARY KEY
	 , AccountType NVARCHAR(20) NOT NULL
	 , CreatedDate DATETIME DEFAULT(GETDATE())
	 , CreatedBy NVARCHAR(100) NOT NULL)
GO
INSERT INTO dbo.tbl_mstr_bankaccounttype(AccountType, CreatedBy)
SELECT 'Savings', 'Naresh.Viriyala' UNION
SELECT 'Current', 'Naresh.Viriyala' UNION
SELECT 'Other', 'Naresh.Viriyala' 