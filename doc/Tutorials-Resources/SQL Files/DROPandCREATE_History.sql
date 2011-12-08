USE [icbmdb]
GO

/****** Object:  Table [dbo].[Histories]    Script Date: 12/08/2011 04:15:08 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Histories]') AND type in (N'U'))
DROP TABLE [dbo].[Histories]
GO

USE [icbmdb]
GO

/****** Object:  Table [dbo].[Histories]    Script Date: 12/08/2011 04:15:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Histories](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[timestamp] [datetime] NOT NULL,
	[username] [varchar](50) NOT NULL,
	[type] [varchar](50) NOT NULL,
	[change] [varchar](max) NOT NULL,
	[dbtable] [varchar](50) NOT NULL,
 CONSTRAINT [PK_History] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

