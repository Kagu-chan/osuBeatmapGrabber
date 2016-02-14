<?php
	require("sreq.php");

	define("AAL", TRUE);
	$f3 = require("init.php");

	global $db;
	$db = $f3->get("DB");

	function execute($query)
	{
		global $db;

		$db->begin();
                $db->exec($query);
                $db->commit();
	}

	function scaffold($name, $columns)
	{
		global $db;

		$lines = [];
		foreach ($columns as $cName => $define)
		{
			$key = strtoupper($cName);
			$lines[] = "`$key` $define";
		}

		$insert = implode(",\n\t", $lines);

		$query = <<<EOF
CREATE TABLE IF NOT EXISTS $name (
	`GUID` VARCHAR(36),
	`CREATED` DATETIME NOT NULL,
	`MODIFIED` DATETIME NOT NULL,
	`DELETED` TINYINT(1) DEFAULT '0',
	$insert,
	PRIMARY KEY(`GUID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
EOF;

		$db->begin();
		$db->exec($query);
		$db->commit();
	}

	if (!file_exists("migrate.json"))
		file_put_contents("migrate.json", json_encode(["last" => 0]));

	$migrate = json_decode(file_get_contents("migrate.json"), true);
	$dir = implode(DIRECTORY_SEPARATOR, array(getcwd(), "dbmigrate"));
	
	$scripts = array_diff(scandir($dir), array('..', '.'));
	$latest = (int) (explode(".", array_pop($scripts))[0]);

	for ($i = $migrate["last"] + 1; $i <= $latest; $i++) { 
		$fName = sprintf("dbmigrate/%010d.php", $i);
		
		require($fName);
		$migrate["last"] = $i;
	}

	file_put_contents("migrate.json", json_encode($migrate));

	return 0;
