USE [icbmdb]
GO

/****** Object:  Table [dbo].[PCA_WE]    Script Date: 11/14/2011 01:35:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PCA_WE]') AND type in (N'U'))
DROP TABLE [dbo].[PCA_WE]
GO

USE [icbmdb]
GO

/****** Object:  Table [dbo].[PCA_WE]    Script Date: 11/14/2011 01:35:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PCA_WE](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PCA] [int] NULL,
	[WE] [int] NULL,
 CONSTRAINT [PK_PCA_WE] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

