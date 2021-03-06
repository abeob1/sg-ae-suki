/****** Object:  Table [dbo].[AB_SO_Header]    Script Date: 12/27/2013 10:44:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AB_SO_Header](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CardCode] [nvarchar](50) NULL,
	[DocDate] [nvarchar](50) NULL,
	[DocDueDate] [nvarchar](50) NULL,
	[U_AB_Urgent] [nvarchar](50) NULL,
	[U_AB_UserCode] [nvarchar](50) NOT NULL,
	[U_AB_POWhsCode] [nvarchar](50) NULL,
	[LicTradNum] [nvarchar](50) NULL,
	[ShipToCode] [nvarchar](50) NULL,
	[U_AB_DriverNo] [nvarchar](50) NULL,
	[NumAtCard] [nvarchar](50) NULL,
	[SOStatus] [nvarchar](50) NULL,
	[ErrMessage] [nvarchar](max) NULL,
	[U_AB_PODocEntry] [int] NULL,
	[CreateDate] [datetime] NULL,
	[U_AB_PORemarks] [nvarchar](max) NULL,
 CONSTRAINT [PK_SO_Header] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AB_SO_Detail]    Script Date: 12/27/2013 10:44:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AB_SO_Detail](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[HeaderID] [int] NULL,
	[ItemCode] [nvarchar](50) NULL,
	[Dscription] [nvarchar](50) NULL,
	[Quantity] [decimal](18, 2) NULL,
	[Price] [decimal](18, 2) NULL,
	[LineTotal] [decimal](18, 2) NULL,
	[U_AB_POQty] [decimal](18, 2) NULL,
	[U_AB_POLineNum] [int] NULL,
 CONSTRAINT [PK_SO_Detail] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
