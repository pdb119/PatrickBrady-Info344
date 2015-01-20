<?php
class DataConnection {
	public function __construct(){
		
	}
	
	public function getData($players, $searchName){
		$conn = new PDO('mysql:host=192.168.0.35;dbname=players','patb','bradypat');
		$query = $conn->prepare('SELECT * FROM players WHERE LOWER(PlayerName) LIKE ?');
		$query->execute(array('%'.strtolower($searchName).'%'));
		$result = $query->fetchAll();
		foreach($result as $row){
			$players->addPlayer($row['PlayerName'],$row['GP'],$row['FGP'],$row['TPP'],$row['TPP'],$row['PPG']);										
		}
		$conn = null;
	}
}

?>