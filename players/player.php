<?php
class Player {
	private  $name;
	private $gp;
	private $fgp;
	private $tpp;
	private $ftp;
	private $ppg;
	public function __construct($name,$gp,$fgp,$tpp,$ftp,$ppg){
		$this->name = $name;
		$this->gp = $gp;
		$this->fgp = $fgp;
		$this->tpp = $tpp;
		$this->ftp = $ftp;
		$this->ppg = $ppg;
	}
	public function getName(){
		return $this->name;
	}

	public function getGp(){
		return $this->gp;
	}
	
	public function getFgp(){
		return $this->fgp;
	}
	
	public function getTpp(){
		return $this->tpp;
	}
	
	public function getFtp(){
		return $this->ftp;
	}
	
	public function getPpg(){
		return $this->ppg;
	}
}
?>