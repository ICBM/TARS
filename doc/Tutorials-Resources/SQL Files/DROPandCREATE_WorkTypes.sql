USE [icbmdb]
GO

/****** Object:  Table [dbo].[WorkTypes]    Script Date: 4/8/2012 01:35:43 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WorkTypes]') AND type in (N'U'))
DROP TABLE [dbo].[WorkTypes]
GO

USE [icbmdb]
GO

/****** Object:  Table [dbo].[WorkTypes]    Script Date: 4/8/2012 01:35:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WorkTypes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[WE] [int] NULL,
	[description] [varchar](100) NULL,
 CONSTRAINT [PK_WorkTypes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

