<?php
class Players{
	private $players;
	public function __construct()
	{
		$this->players = array();
	}
	
	public function addPlayer($name, $gp, $fgp, $tpp, $ftp, $ppg){
		$this->players[] = new Player($name, $gp, $fgp, $tpp, $ftp, $ppg);
	}
	public function getPlayerIterator(){
		return new PlayerIterator($this->players);
	}
	

}

class PlayerIterator{
	private $players;
	private $i;
	public function __construct($players){
		$this->i = 0;
		$this->players = $players;
	}
	public function hasNext(){
		
		return count($this->players)>$this->i;
	}
	public function getNext(){
		$temp = $this->players[$this->i];
		$this->i = $this->i+1;
		return $temp;
	}
}
?>