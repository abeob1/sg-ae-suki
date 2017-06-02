USE [HSukiGroup]
GO
/****** Object:  StoredProcedure [dbo].[sp_AB_RPT_StockTakeList]    Script Date: 01/15/2014 14:34:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER proc [dbo].[sp_AB_RPT_StockTakeList]
as
select ROW_NUMBER () OVER (Order by T0.CardCode) as Id,T0.CompanyCode,T0.CompanyName,T0.CardCode,T0.CardName,T1.OutletCode,T1.OutletName,T0.ItemCode,T0.ItemName,T3.FrgnName,T3.InvntryUom
 from Item_Supplier T0
left join Item_Outlet T1 on T1.HeaderID=T0.ID
left join OITM T3 on T3.ItemCode=T0.Itemcode
where  T1.Outletcode is not null
group by T0.CompanyCode,T0.CompanyName,T0.CardCode,T0.CardName,T1.OutletCode,T1.OutletName,T0.ItemCode,T0.ItemName,T3.FrgnName,T3.InvntryUom
order by T0.CardCode

