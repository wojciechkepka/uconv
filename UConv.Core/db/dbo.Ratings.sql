CREATE TABLE [dbo].[Ratings] (
    [Id]     INT           NOT NULL IDENTITY,
    [date]   DATETIME      NOT NULL,
    [name]   VARCHAR (250) NOT NULL,
    [rating] INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

