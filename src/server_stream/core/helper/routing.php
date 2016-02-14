<?php
	
	namespace Helper;

		class Routing extends \Prefab
		{

			public function generateListingResult()
			{
				$table = $this->findTablename();
				\App::set($table, $this->loadAllFor($table, \App::get("JSON")));
			}

			public function generateShowResult()
			{
				$table = $this->findTablename();
				\App::set("record", $this->loadByGUID($table, \App::get("JSON")));
			}

			protected function findTablename()
			{
				return \App::get("CONTROLLER");
			}

			protected function getModel($tableName)
			{
				return "\\Model\\" . ucfirst(strtolower($tableName));
			}

			protected function loadAllFor($tableName, $json)
			{
				$className = $this->getModel($tableName);
				return $className::all($json);
			}

			protected function loadByGUID($tableName, $json)
			{
				$className = $this->getModel($tableName);
				return $className::by("GUID", \App::get("PARAMS.guid"), $json);
			}

		}