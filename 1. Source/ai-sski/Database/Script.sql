/****** Object:  Table [dbo].[Outlet_OrderAmount]    Script Date: 12/02/2013 14:35:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Outlet_OrderAmount](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CardCode] [nvarchar](50) NULL,
	[CardName] [nvarchar](100) NULL,
	[CompanyCode] [nvarchar](50) NULL,
	[CompanyName] [nvarchar](100) NULL,
	[OutletCode] [nvarchar](50) NULL,
	[OutletName] [nvarchar](100) NULL,
	[MinAmt] [decimal](18, 2) NULL,
	[MaxAmt] [decimal](18, 2) NULL,
 CONSTRAINT [PK_Outlet_Amount] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Outlet_Calendar]    Script Date: 12/02/2013 14:35:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Outlet_Calendar](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CardCode] [nvarchar](50) NULL,
	[CardName] [nvarchar](100) NULL,
	[CompanyCode] [nvarchar](50) NULL,
	[CompanyName] [nvarchar](100) NULL,
	[OutletCode] [nvarchar](50) NULL,
	[OutletName] [nvarchar](100) NULL,
	[Mon] [bit] NULL,
	[Tue] [bit] NULL,
	[Wed] [bit] NULL,
	[Thu] [bit] NULL,
	[Fri] [bit] NULL,
	[Sat] [bit] NULL,
	[Sun] [bit] NULL,
 CONSTRAINT [PK_Oulet_Calendar] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Item_Supplier]    Script Date: 12/02/2013 14:35:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Item_Supplier](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CardCode] [nvarchar](50) NULL,
	[CardName] [nvarchar](100) NULL,
	[CompanyCode] [nvarchar](50) NULL,
	[CompanyName] [nvarchar](100) NULL,
	[ItemCode] [nvarchar](50) NULL,
	[ItemName] [nvarchar](max) NULL,
	[LB] [nvarchar](100) NULL,
 CONSTRAINT [PK_Item_Supplier] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Item_Outlet]    Script Date: 12/02/2013 14:35:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Item_Outlet](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[HeaderID] [int] NULL,
	[OutletCode] [nvarchar](50) NULL,
	[OutletName] [nvarchar](100) NULL,
	[MinQty] [numeric](19, 2) NULL,
	[MaxQty] [numeric](19, 2) NULL,
	[MinAmt] [numeric](19, 2) NULL,
	[MaxAmt] [numeric](19, 2) NULL,
 CONSTRAINT [PK_Item_Outlet] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
