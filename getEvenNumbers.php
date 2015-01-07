<!DOCTYPE html>
<html>
<title>Even Numbers</title>
</head>

<body>
<form action="getEvenNumbers.php" method="GET">
<input name="n">
<input type="submit" />
</form>
</html>
<?php
if(isset($_REQUEST['n'])){
	$i = 2;
	while($i <= $_REQUEST['n']){
		echo($i." ");
		$i += 2;
	}
}
?>
</body>
</html>