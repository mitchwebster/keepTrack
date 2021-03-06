IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Entries'))
BEGIN
    DROP TABLE dbo.Entries; 
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Activities'))
BEGIN
    DROP TABLE dbo.Activities;
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Groups'))
BEGIN
    DROP TABLE dbo.Groups; 
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Users'))
BEGIN
    DROP TABLE dbo.Users; 
END


CREATE TABLE dbo.Users (
    Id INT NOT NULL IDENTITY(1,1),
    Username VARCHAR(255) NOT NULL,
    PasswordHash VARCHAR(MAX) NOT NULL,
    Role VARCHAR(255) NOT NULL,
	PRIMARY KEY (Id)
);

CREATE TABLE dbo.Groups (
    Id INT NOT NULL IDENTITY(1,1),
    Name VARCHAR(255) NOT NULL,
    UserId INT,
	Category VARCHAR(255) NULL,
	PRIMARY KEY (Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE TABLE dbo.Activities (
    Id INT NOT NULL IDENTITY(1,1),
    Name VARCHAR(255) NOT NULL,
    UserId INT,
    GroupId INT NULL,
    Category VARCHAR(255) NULL,
    MeasurementType VARCHAR(255) NULL,
	PRIMARY KEY (Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (GroupId) REFERENCES Groups(Id),
);

CREATE TABLE dbo.Entries (
    Id INT NOT NULL IDENTITY(1,1),
    EntryTimeStamp TIMESTAMP,
    Value decimal(10,3),
    ActivityId INT,
	PRIMARY KEY (Id),
    FOREIGN KEY (ActivityId) REFERENCES Activities(Id)
);