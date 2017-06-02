var db = Titanium.Database.open('SukiDB');
db.execute('CREATE TABLE IF NOT EXISTS Settings (Title TEXT, Desc TEXT)');
db.execute('CREATE TABLE IF NOT EXISTS Credential (Username TEXT, Password TEXT)');
db.close();
function selectSetting(_title){
	
	db = Titanium.Database.open('SukiDB');	
	var sql = "SELECT Desc FROM Settings where Title = '" + _title + "'";
	 var results = [];
	 var resultSet = db.execute(sql);
	 while(resultSet.isValidRow()){
	 	
	 	var ret = resultSet.fieldByName('DESC');
	 	db.close();
	 	return ret;
	 }
	 db.close();
	 return '';
    
}

function insertSetting(_title, _desc) {
	
	db = Titanium.Database.open('SukiDB');
    var sql = "INSERT INTO Settings (Title, Desc) VALUES (";
    sql = sql + "'" + _title.replace("'", "''") + "', ";
    sql = sql + "'" + _desc.replace("'", "''") + "') ";
    db.execute(sql);
    var reID =db.lastInsertRowId;
    db.close();
    return reID;
}

function updateSetting(_title, _desc){
	
	db = Titanium.Database.open('SukiDB');
	var sql = "UPDATE Settings set Desc = '" + _desc + "' WHERE Title = '" + _title + "'";
	db.execute(sql);
	db.close();
}

function getCredential(){
	
	db = Titanium.Database.open('SukiDB');
	var sql = "SELECT * from Credential";
    var resultSet = db.execute(sql);
	while(resultSet.isValidRow()){
		var cre = {Username: resultSet.fieldByName('Username'),
					Password: resultSet.fieldByName('Password')};
		db.close();
		return cre;
     }
     db.close();
     return '';
}

function saveCredential(_username, _password){
	
	db = Titanium.Database.open('SukiDB');	
	var sql = "INSERT INTO Credential values('" + _username + "', '" + _password + "')";
	db.execute(sql);
	db.close();
}

function deleteCredential(){
	
	db = Titanium.Database.open('SukiDB');	
	var sql = "DELETE FROM Credential";
	db.execute(sql);
	db.close();
}

function updateCredential(_username, _password){
	
	db = Titanium.Database.open('SukiDB');
	var sql = "UPDATE Credential set Username = '" + _username + "', Password = '" + _password + "'";
	db.execute(sql);
	db.close();
}
//db.execute('CREATE TABLE IF NOT EXISTS favorites (ID INTEGER  PRIMARY KEY, TITLE TEXT, LINK TEXT, DESCRIPTION TEXT)');