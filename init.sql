-- Wait for SQL Server to be ready
WAITFOR DELAY '00:00:05';
GO

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TruthNews')
BEGIN
    CREATE DATABASE TruthNews;
END
GO

USE TruthNews;
GO

-- Create Users table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
CREATE TABLE Users (
                       Id INT PRIMARY KEY IDENTITY(1,1),
                       Name VARCHAR(100) NOT NULL,
                       Email VARCHAR(255) NOT NULL UNIQUE,
                       CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
END
GO

-- Create Articles table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Articles')
BEGIN
CREATE TABLE Articles (
                          Id INT PRIMARY KEY IDENTITY(1,1),
                          Title VARCHAR(255) NOT NULL,
                          Content TEXT NOT NULL,
                          AuthorId INT NOT NULL,
                          CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
                          FOREIGN KEY (AuthorId) REFERENCES Users(Id)
);
END
GO

-- Create Comments table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Comments')
BEGIN
CREATE TABLE Comments (
                          Id INT PRIMARY KEY IDENTITY(1,1),
                          ArticleId INT NOT NULL,
                          UserId INT NOT NULL,
                          Content TEXT NOT NULL,
                          CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
                          FOREIGN KEY (ArticleId) REFERENCES Articles(Id),
                          FOREIGN KEY (UserId) REFERENCES Users(Id)
);
END
GO

-- Seed Users
IF NOT EXISTS (SELECT * FROM Users)
BEGIN
INSERT INTO Users (Name, Email, CreatedAt) VALUES
                                               ('John Doe', 'john.doe@example.com', GETDATE()),
                                               ('Jane Smith', 'jane.smith@example.com', GETDATE()),
                                               ('Bob Johnson', 'bob.johnson@example.com', GETDATE()),
                                               ('Alice Williams', 'alice.williams@example.com', GETDATE()),
                                               ('Charlie Brown', 'charlie.brown@example.com', GETDATE());
END
GO

-- Seed Articles
IF NOT EXISTS (SELECT * FROM Articles)
BEGIN
INSERT INTO Articles (Title, Content, AuthorId, CreatedAt) VALUES
                                                               ('Getting Started with .NET 9', 'This article covers the basics of .NET 9 and its new features...', 1, GETDATE()),
                                                               ('Introduction to Redis Caching', 'Learn how to implement Redis caching in your applications...', 2, GETDATE()),
                                                               ('Docker Best Practices', 'A comprehensive guide to Docker containerization...', 1, GETDATE()),
                                                               ('SQL Server Performance Tuning', 'Tips and tricks for optimizing SQL Server queries...', 3, GETDATE()),
                                                               ('Building RESTful APIs', 'How to design and implement clean REST APIs...', 4, GETDATE());
END
GO

-- Seed Comments
IF NOT EXISTS (SELECT * FROM Comments)
BEGIN
INSERT INTO Comments (ArticleId, UserId, Content, CreatedAt) VALUES
                                                                 (1, 2, 'Great article! Very informative.', GETDATE()),
                                                                 (1, 3, 'Thanks for sharing this.', GETDATE()),
                                                                 (2, 1, 'Redis is awesome for caching!', GETDATE()),
                                                                 (2, 4, 'Can you write more about Redis clustering?', GETDATE()),
                                                                 (3, 5, 'Docker has changed the way we deploy applications.', GETDATE()),
                                                                 (4, 2, 'Very helpful performance tips!', GETDATE()),
                                                                 (5, 3, 'REST APIs are essential for modern applications.', GETDATE());
END
GO

PRINT 'Database initialization completed successfully!';