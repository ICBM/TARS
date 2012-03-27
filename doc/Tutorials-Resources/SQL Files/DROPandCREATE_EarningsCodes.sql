USE [icbmdb]
GO

/****** Object:  Table [dbo].[EarningsCodes]    Script Date: 03/27/2012 05:35:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EarningsCodes]') AND type in (N'U'))
DROP TABLE [dbo].[EarningsCodes]
GO

USE [icbmdb]
GO

/****** Object:  Table [dbo].[EarningsCodes]    Script Date: 03/27/2012 05:35:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[EarningsCodes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[earningsCode] [nchar](3) NOT NULL,
	[description] [varchar](75) NOT NULL,
 CONSTRAINT [PK_EarningsCodes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


