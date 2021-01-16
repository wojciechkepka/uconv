CREATE TABLE [dbo].[Records] (
    [id]          INT           IDENTITY (1, 1) NOT NULL,
    [date]        DATETIME      NOT NULL,
    [hostname]    VARCHAR (250) NOT NULL,
    [converter]   VARCHAR (32)  NOT NULL,
    [inputValue]  FLOAT (53)    NOT NULL,
    [inputUnit]   VARCHAR (32)  NOT NULL,
    [outputValue] FLOAT (53)    NOT NULL,
    [outputUnit]  VARCHAR (32)  NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);

