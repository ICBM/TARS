USE [icbmdb]
GO

/****** Object:  Table [dbo].[WorkEfforts]    Script Date: 12/07/2011 19:37:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WorkEfforts]') AND type in (N'U'))
DROP TABLE [dbo].[WorkEfforts]
GO

USE [icbmdb]
GO

/****** Object:  Table [dbo].[WorkEfforts]    Script Date: 12/07/2011 19:37:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WorkEfforts](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[code] [int] NULL,
	[startDate] [datetime] NULL,
	[endDate] [datetime] NULL,
	[description] [varchar](50) NULL,
	[active] [bit] NULL,
 CONSTRAINT [PK_WorkEfforts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

