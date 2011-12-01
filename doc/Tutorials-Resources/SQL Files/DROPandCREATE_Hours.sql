USE [icbmdb]
GO

/****** Object:  Table [dbo].[Hours]    Script Date: 11/30/2011 18:11:29 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Hours]') AND type in (N'U'))
DROP TABLE [dbo].[Hours]
GO

USE [icbmdb]
GO

/****** Object:  Table [dbo].[Hours]    Script Date: 11/30/2011 18:11:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Hours](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[we] [int] NULL,
	[task] [int] NULL,
	[hours] [float] NULL,
	[hoursType] [nchar](10) NULL,
	[approved] [bit] NULL,
	[timestamp] [datetime] NULL,
	[description] [nchar](10) NULL,
	[creator] [nchar](10) NULL,
 CONSTRAINT [PK_Hours] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


