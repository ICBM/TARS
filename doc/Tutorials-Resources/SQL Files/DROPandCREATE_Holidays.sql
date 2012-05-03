USE [icbmdb]
GO

/****** Object:  Table [dbo].[Holidays]    Script Date: 05/3/2012 05:44:20 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Holidays]') AND type in (N'U'))
DROP TABLE [dbo].[Holidays]
GO

USE [icbmdb]
GO

/****** Object:  Table [dbo].[Holidays]    Script Date: 05/3/2012 05:44:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Holidays](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[holidayName] [varchar](50) NULL,
	[date] [datetime2](7) NULL,
 CONSTRAINT [PK_Holidays] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


