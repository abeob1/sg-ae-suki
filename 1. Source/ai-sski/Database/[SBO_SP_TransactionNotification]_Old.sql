USE [SBODemoUS]
GO
/****** Object:  StoredProcedure [dbo].[SBO_SP_TransactionNotification]    Script Date: 11/20/2013 18:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER proc [dbo].[SBO_SP_TransactionNotification] 

@object_type nvarchar(20), 				-- SBO Object Type
@transaction_type nchar(1),			-- [A]dd, [U]pdate, [D]elete, [C]ancel, C[L]ose
@num_of_cols_in_key int,
@list_of_key_cols_tab_del nvarchar(255),
@list_of_cols_val_tab_del nvarchar(255)

AS

begin

-- Return values
declare @error  int				-- Result (0 for no error)
declare @error_message nvarchar (200) 		-- Error string to be displayed
select @error = 0
select @error_message = N'Ok'

--------------------------------------------------------------------------------------------------------------------------------
select * from [AB_DeliveryOrder]
if @object_type='15' and @transaction_type='A'
begin
	insert into [AB_DeliveryOrder] (DocEntry,LineNum,DONo,SONo,ItemCode,Dscription,Quantity,RecQty, unitMsr)
	select T0.DocEntry,T1.LineNum,T0.DocNum,T1.BaseEntry,T1.ItemCode,T1.Dscription,T1.Quantity,T1.Quantity,T1.unitMsr
	from ODLN T0
	join DLN1 T1 on T0.DocEntry=T1.DocEntry
	where T0.DocEntry=@list_of_cols_val_tab_del
	
end
--------------------------------------------------------------------------------------------------------------------------------

-- Select the return values
select @error, @error_message

end
