USE [icbmdb]
GO

/****** Object:  Table [dbo].[WorkEfforts]    Script Date: 11/14/2011 01:35:54 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WorkEfforts]') AND type in (N'U'))
DROP TABLE [dbo].[WorkEfforts]
GO

USE [icbmdb]
GO

/****** Object:  Table [dbo].[WorkEfforts]    Script Date: 11/14/2011 01:35:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WorkEfforts](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[code] [int] NULL,
	[startDate] [datetime] NULL,
	[endDate] [datetime] NULL,
	[creator] [nchar](10) NULL,
	[description] [nchar](10) NULL,
	[files] [nchar](10) NULL,
 CONSTRAINT [PK_WorkEfforts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
