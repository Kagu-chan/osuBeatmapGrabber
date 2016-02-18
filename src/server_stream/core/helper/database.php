<?php
	
	namespace Helper;

		class Database extends \Prefab
		{

			private function db()
			{
				return \App::get("DB");
			}

			public function scaffold($name, $columns)
			{
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
				
				$this->exec($query);
			}

			public function exec($query)
			{
				$this->db()->begin();
				$this->db()->exec($query);
				$this->db()->commit();
			}
			
		}