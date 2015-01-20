<!DOCTYPE html>
<head>
<title>Player Search</title>
<link href="players.css" rel="stylesheet" type="text/css">
</head>

<body>
<?php
include('players.php');
include('dataConnection.php');
include('player.php');

function main($searchName){
	$players = new Players();
	$data = new DataConnection();
	$data->getData($players,$searchName);
	//parseJson($players);
	display($players 	);
}

function display($players){
	$iter = $players->getPlayerIterator();
	while($iter->hasNext()){
		$curr = $iter->getNext();
		?>
            <table>
            <tr><td>Name</td><td>GP</td><td>FGP</td><td>TPP</td><td>FTP</td><td>PPG</td></tr>
            <td><?php echo($curr->getName());?></td>
            <td><?php echo($curr->getGp());?></td>
            <td><?php echo($curr->getFgp());?></td>
            <td><?php echo($curr->getTpp());?></td>
            <td><?php echo($curr->getFtp());?></td>
            <td><?php echo($curr->getPpg());?></td>
            </tr>
            </table>
            <br />
            <hr />
            <br />
		<?php
	}
}

function displaySearchBox($currSearch){
	?>
    	<div align="center">
    	<img src="bb.jpg" width="150" height="150"/>
        <h2>NBC Players Search</h2>
        <form action="index.php" method="GET">
        <input name="q" type="text" placeholder="Search Name..." value="<?php echo($currSearch); ?>" />
        <input type="submit" value="Search"/>
        </form>
        </div>
        <br />
	<?php
}



if(isset($_REQUEST['q'])){
	displaySearchBox($_REQUEST['q']);
	main($_REQUEST['q']);
} else {
	displaySearchBox('');
}
?>
</body>
</html>