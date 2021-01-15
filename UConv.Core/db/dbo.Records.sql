DELETE [dbo].[Records];

CREATE TABLE [dbo].[Records] (
    [id]          INT           NOT NULL IDENTITY,
    [date]        DATETIME      NOT NULL,
    [hostname]    VARCHAR (250) NOT NULL,
    [converter]   VARCHAR (20)  NOT NULL,
    [inputValue]  FLOAT (53)    NOT NULL,
    [inputUnit]   VARCHAR (10)  NOT NULL,
    [outputValue] FLOAT (53)    NOT NULL,
    [outputUnit]  VARCHAR (10)  NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);

