USE [icbmdb]
GO

/****** Object:  Table [dbo].[PcaCodes]    Script Date: 12/07/2011 19:37:46 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PcaCodes]') AND type in (N'U'))
DROP TABLE [dbo].[PcaCodes]
GO

USE [icbmdb]
GO

/****** Object:  Table [dbo].[PcaCodes]    Script Date: 12/07/2011 19:37:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PcaCodes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[code] [int] NULL,
	[startDate] [datetime] NULL,
	[endDate] [datetime] NULL,
	[description] [varchar](max) NULL,
	[active] [bit] NULL,
 CONSTRAINT [PK_PcaCodes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

