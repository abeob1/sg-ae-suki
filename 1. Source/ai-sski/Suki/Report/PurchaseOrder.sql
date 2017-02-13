USE [Test_TSakuraCatering]
GO
/****** Object:  StoredProcedure [dbo].[sp_AB_FRM_PurchaseOrder]    Script Date: 12/06/2013 23:32:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER proc [dbo].[sp_AB_FRM_PurchaseOrder]
--sp_AB_FRM_PurchaseOrder 5
@DocKey numeric(18,0)

as

select 
T2.CompnyName,T2.Phone1,T2.Phone2,T2.GlblLocNum,T2.Fax, T2.FreeZoneNo, T2.TaxIdNum,T2.LogoImage,
T2.CompnyAddr, T2.BlockF,T2.StreetF,T2.Country,T0.DocDate,T0.DocDueDate,T2.ZipCode,T9.SeriesName,

------------item detail--------------
T0.DocEntry,T1.ItemCode,T7.SuppCatNum,
T1.Dscription,T1.PriceBefDi,T1.Quantity,T1.LineTotal,T1.VatSum,
T0.[Address],T0.Address2,T0.DocNum,T0.NumAtCard,T1.unitMsr,

-----------SalesPerson----------------
T0.TaxDate,T6.mobile,T6.Fax as 'Fax2',T6.email,

-----------BP info--------------
T4.CardName, T5.Name ContactName,T4.CntctPrsn,T0.ShipToCode,T4.E_Mail, T0.Footer, T0.Header,T4.Phone1 as 'PhoneCust', 
T0.CardCode, T0.Comments,T5.Cellolar,T0.DocCur,T4.Fax as BPFax,T8.PymntGroup
from OPOR T0 with(nolock)
join POR1 T1 with(nolock) on T1.DocEntry=t0.DocEntry
join 
(
 select top(1) isnull(T0.PrintHeadr,T0.CompnyName) CompnyName,T0.Phone1,T0.Phone2,T1.GlblLocNum, T0.Fax, T0.FreeZoneNo, T0.TaxIdNum,T1.ZipCode,
  T0.CompnyAddr,T1.BlockF,T1.StreetF,T3.LogoImage,
  T2.Name Country
 from OADM T0 with(nolock) 
 join ADM1 T1 with(nolock) on 1=1
 join OCRY T2 with(nolock) on T2.Code=T1.Country
  join OADP T3 with(nolock) on 1=1
) T2 on 1=1
left join OHEM T6 with (nolock) on T6.empID=T0.OwnerCode
left join OCRD T4 with (nolock) on T4.CardCode=T0.CardCode
left join OCPR T5 with (nolock) on T5.CntctCode=T0.CntctCode
left join OITM T7 with (nolock) on T7.ItemCode=T1.ItemCode
left join OCTG T8 with (nolock) on T8.GroupNum=T0.GroupNum
left join NNM1 T9 with (nolock) on T9.Series=T0.Series
where T0.DocEntry=@DocKey



