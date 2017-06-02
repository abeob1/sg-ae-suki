
DECLARE    
    @OldDB nvarchar(100),
    @NewDB nvarchar(100),
    @OldOutlet nvarchar(100),
    @NewOutlet nvarchar(100);
    
 SET @OldDB =  'UAT_RSukiSushi'
 SET @NewDB = 'UAT_RNihonMura'
 SET @OldOutlet = 'NM-SP';
 SET @NewOutlet = 'NM-SP1'
 
	INSERT INTO Outlet_OrderAmount
	(CardCode,CardName,CompanyCode,CompanyName,OutletCode,OutletName,MinAmt,MaxAmt)
	SELECT CardCode,CardName,'UAT_RNihonMura' AS CompanyCode,'Nihon Mura Pte. Ltd' AS CompanyName,OutletCode+'1'
	 AS OutletCode,OutletName +' 1' as OutletName ,MinAmt,MaxAmt
	FROM Outlet_OrderAmount
	WHERE OutletCode =@OldOutlet AND CompanyCode=@OldDB

	INSERT INTO Outlet_Calendar
	(CardCode,CardName,CompanyCode,CompanyName,OutletCode,OutletName,Mon,Tue,Wed,Thu,Fri,Sat,Sun)
	SELECT CardCode,CardName,'UAT_RNihonMura' AS CompanyCode,'Nihon Mura Pte. Ltd' AS CompanyName,OutletCode+'1' 
	AS OutletCode,OutletName +' 1' as OutletName ,Mon,Tue,Wed,Thu,Fri,Sat,Sun
	FROM Outlet_Calendar
	WHERE OutletCode =@OldOutlet AND CompanyCode=@OldDB