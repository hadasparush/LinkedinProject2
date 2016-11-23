CREATE TABLE [dbo].[profile]
(
	[name] VARCHAR(50) NOT NULL , 
    [currentTitle] VARCHAR(50) NULL, 
    [currentPosition] VARCHAR(50) NULL, 
    [summary] VARCHAR(MAX) NULL, 
    [experince] VARCHAR(MAX) NULL, 
    [education] VARCHAR(MAX) NULL, 
    [id] INT NOT NULL, 
    [skills] VARCHAR(MAX) NULL, 
    PRIMARY KEY ([id])
)
