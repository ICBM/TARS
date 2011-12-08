USE [icbmdb]
GO

/****** Object:  Table [dbo].[TARSUser]    Script Date: 12/07/2011 23:35:11 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TARSUser]') AND type in (N'U'))
DROP TABLE [dbo].[TARSUser]
GO

USE [icbmdb]
GO

/****** Object:  Table [dbo].[TARSUser]    Script Date: 12/07/2011 23:35:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TARSUser](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[un] [varchar](50) NULL,
	[permission] [int] NULL,
 CONSTRAINT [PK_TARSUser] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

