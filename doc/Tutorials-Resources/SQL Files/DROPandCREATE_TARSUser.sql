USE [icbmdb]
GO

/****** Object:  Table [dbo].[TARSUsers]    Script Date: 03/01/2012 00:37:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TARSUsers]') AND type in (N'U'))
DROP TABLE [dbo].[TARSUsers]
GO

USE [icbmdb]
GO

/****** Object:  Table [dbo].[TARSUsers]    Script Date: 03/01/2012 00:37:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TARSUsers](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SID] [varchar](184) NULL,
	[userName] [varchar](40) NOT NULL,
	[userID] [varchar](26) NULL,
	[startDate] [datetime2](7) NULL,
	[endDate] [datetime2](7) NULL,
	[permission] [int] NULL,
	[company] [varchar](40) NULL,
	[department] [varchar](40) NULL,
	[employeeOrContractor] [char](2) NULL,
	[costAllocatedOrNot] [char](1) NULL,
 CONSTRAINT [PK_TARSUser] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

