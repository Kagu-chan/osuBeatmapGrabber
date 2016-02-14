<?php
	
	defined("AAL") or die();

	$f3 = require("../core/script/fff.php");
	$f3->set('AUTOLOAD','../core;../core/class/');
	$f3->config("../conf/db.cfg");

	$f3->set('DB', new DB\SQL(
		'mysql:host=' . $f3->get("dbhost") . ';port=' . $f3->get("dbport") . ';dbname=' . $f3->get("dbname"),
	    $f3->get("dbuser"),
	    $f3->get("dbpass")
	));

	return $f3;