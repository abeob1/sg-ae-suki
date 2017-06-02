
/****** Object:  Table [dbo].[AB_DeliveryOrder]    Script Date: 11/20/2013 17:59:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AB_DeliveryOrder](
	[DocEntry] [int] NULL,
	[LineNum] [int] NULL,
	[DONo] [nvarchar](50) NULL,
	[SONo] [nvarchar](50) NULL,
	[ItemCode] [nvarchar](50) NULL,
	[Dscription] [nvarchar](500) NULL,
	[Quantity] [numeric](18, 0) NULL,
	[RecQty] [numeric](18, 0) NULL,
	[unitMsr] [nvarchar](50) NULL
) ON [PRIMARY]

GO


