<?php
	
	namespace Route;

		/**
		 * @inheritdoc
		 */
		class Cron extends \Route { 

			private $_migrateFile;
			private $_migrateDir;

			public function beforeRoute()
			{
				\App::checkAccessOrDie();

				parent::beforeRoute();
			}

			private function securePath($path)
			{
				return implode(DIRECTORY_SEPARATOR, [\App::get("ROOT"), $path]);
			}

			public function cache()
			{
				\App::f3()->clear('CACHE');
			}

			public function migrate()
			{
				$this->_migrateFile = $this->securePath("cron/migrate.json");
				$this->_migrateDir = $this->securePath("cron/dbmigrate");


				if (!file_exists($this->_migrateFile))
					file_put_contents($this->_migrateFile, json_encode(["last" => 0]));

				$migrationState = json_decode(file_get_contents($this->_migrateFile), true);
	
				$scripts = array_diff(scandir($this->_migrateDir), array('..', '.'));
				$latest = (int) (explode(".", array_pop($scripts))[0]);

				for ($i = $migrationState["last"] + 1; $i <= $latest; $i++)
				{
					$fName = $this->_migrateDir . DIRECTORY_SEPARATOR . sprintf("%010d.php", $i);
		
					require($fName);
					$migrationState["last"] = $i;
				}

				file_put_contents($this->_migrateFile, json_encode($migrationState));
				exit();
			}

		}