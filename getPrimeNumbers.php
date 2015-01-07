<!DOCTYPE html>
<html>
<title>Prime Numbers</title>
</head>

<body>
<form action="getPrimeNumbers.php" method="GET">
<input name="n">
<input type="submit" />
</form>
</html>
<?php
if(isset($_GET['n'])){
	$i = 2;
	while($i <= $_GET['n']){
		if(gmp_prob_prime($i) != 0){
			echo($i." ");
		}
		$i += 1;
	}
}
?>
</body>
</html>