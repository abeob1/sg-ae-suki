Use UAT_HSukiGroup
DECLARE
	@ID int,
	@CardCode nvarchar(100),
    @CardName varchar(100), 
    @CompanyCode nvarchar(100),
    @CompanyName nvarchar(100),
    @ItemCode nvarchar(100),
    @ItemName nvarchar(100),
    @LB nvarchar(100),
    @OldDB nvarchar(100),
    @NewDB nvarchar(100),
    @OldOutlet nvarchar(100),
    @NewOutlet nvarchar(100);
    
 SET @OldDB =  'UAT_RSukiSushi'
 SET @NewDB = 'UAT_RNihonMura'
 SET @OldOutlet = 'NM-SP';
 SET @NewOutlet = 'NM-SP1';

DECLARE copy_cursor CURSOR FOR 
SELECT ID,CardCode, CardName, CompanyCode, CompanyName,ItemCode,ItemName,LB
FROM Item_Supplier
WHERE CompanyCode = @OldDB

OPEN copy_cursor

FETCH NEXT FROM copy_cursor 
INTO @ID,@CardCode, @CardName,@CompanyCode,@CompanyName,@ItemCode,@ItemName,@LB

WHILE @@FETCH_STATUS = 0
BEGIN
	--INSERT HEADER
	Declare @MaxID  AS Int 
	IF NOT EXISTS (SELECT CardCode FROM Item_Supplier WHERE CardCode=@CardCode 
								AND CompanyCode =@NewDB 
								AND ItemCode = @ItemCode )
	BEGIN 
		INSERT INTO Item_Supplier
		(CardCode, CardName, CompanyCode, CompanyName,ItemCode,ItemName,LB)
		VALUES (@CardCode, @CardName,@NewDB,'Nihon Mura Pte. Ltd',@ItemCode,@ItemName,@LB)
		SET @MaxID = (SELECT MAX(ID) FROM Item_Supplier)
	END
	ELSE
	BEGIN
		SET @MaxID  = (SELECT ID FROM Item_Supplier 
		WHERE CardCode=@CardCode AND CompanyCode =@NewDB AND ItemCode = @ItemCode)
	END
	--INSERT DETAIL
	DECLARE @OuletCode nvarchar(100)
	DECLARE outlet_cursor CURSOR FOR 
		SELECT OutletCode
		FROM Item_Outlet
		WHERE HeaderID =@ID AND OutletCode =@OldOutlet

		OPEN outlet_cursor
		FETCH NEXT FROM outlet_cursor INTO @OuletCode
	    
		IF NOT EXISTS (SELECT OutletCode FROM Item_Outlet JOIN Item_Supplier ON Item_Outlet.HeaderID = Item_Supplier.ID
							 WHERE OutletCode=@NewOutlet
							 AND CompanyCode =@NewDB
							  AND ItemCode=@ItemCode
							  AND CardCode = @CardCode)
		BEGIN
			--INSERT OUTLET
			INSERT INTO Item_Outlet
			(HeaderID, OutletCode,OutletName,MinQty,MaxQty,MinAmt,MaxAmt)
			SELECT HeaderID,OutletCode+'1' AS OutletCode,OutletName,MinQty,MaxQty,MinAmt,MaxAmt
			FROM Item_Outlet
			WHERE HeaderID = @ID AND OutletCode =@OldOutlet
			--UPDATE NEW ID
			UPDATE Item_Outlet SET HeaderID = @MaxID
			 WHERE HeaderID =@ID AND OutletCode=@NewOutlet
		END
		FETCH NEXT FROM outlet_cursor INTO @OuletCode
		CLOSE outlet_cursor
		DEALLOCATE outlet_cursor

	FETCH NEXT FROM copy_cursor 
	INTO @ID,@CardCode, @CardName,@CompanyCode,@CompanyName,@ItemCode,@ItemName,@LB
END 
CLOSE copy_cursor;
DEALLOCATE copy_cursor;