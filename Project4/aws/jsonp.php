<?php
include('players.php');
include('dataConnection.php');
include('player.php');

function main($searchName){
	$players = new Players();
	$data = new DataConnection();
	$data->getData($players,$searchName);
	//parseJson($players);
	display($players);
}

function display($players){	
	$jsonData['results'] = array();
	$iter = $players->getPlayerIterator();
	while($iter->hasNext()){
		$curr = $iter->getNext();
		$newJson['name'] = $curr->getName();
		$newJson['gp'] = $curr->getGp();
		$newJson['fgp'] = $curr->getFgp();
		$newJson['tpp'] = $curr->getTpp();
		$newJson['ftp'] = $curr->getFtp();
		$newJson['ppg'] = $curr->getPpg();
		$jsonData['results'][] = $newJson;
	}
	echo("pa1Return(".json_encode($jsonData).");");
}
main($_REQUEST['q']);
?>