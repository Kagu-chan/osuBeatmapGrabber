<?php
	
	//! Class for generating cached scaffolds
	class Scaffold extends Prefab
	{

		//! Base cache filename
		protected $_base;

		//! The current models file where the cache should be stored
		protected $_cacheFileModel;

		//! The current routes file where the cache should be stored
		protected $_cacheFileRoute;

		protected function __construct()
		{
			$this->_base = App::get("TEMP") . App::f3()->hash(App::get('ROOT') . App::get('BASE')) . ".";
		}

		/**
		 * Executes a scaffold generation
		 *
		 * @param string $fName The file with the scaffolds configuration
		 */
		public function execute($fName)
		{
			$this->rehash($fName);

			if (!is_file($this->_cacheFileModel)) $this->scaffoldModels($this->_cacheFileModel);
			if (!is_file($this->_cacheFileRoute)) $this->scaffoldRoutes($this->_cacheFileRoute);

			require($this->_cacheFileModel);
			require($this->_cacheFileRoute);

			$this->_cacheFileModel = NULL;
			$this->_cacheFileRoute = NULL;
		}

		/**
		 * Rehash the cache and the configuration
		 *
		 * @param string $fName The config to use
		 */
		protected function rehash($fName)
		{
			App::f3()->config($fName);
			$scaffold = App::get("scaffold");

			$mtime = App::f3()->hash(filemtime($fName) . "models");
			$rtime = App::f3()->hash(filemtime($fName) . "routes");
			foreach($scaffold as $className => &$config)
			{
				if (!is_array($config)) $config = [];

				$generate = $config["def"]?:["route","model"];
				$generate = is_array($generate)?$generate:[$generate];
				if (in_array("model", $generate))
				{
					$config["modelfile"] = empty($config["modelfile"])?FALSE:($this->getFileName("SCAFFOLDMODEL",$config["modelfile"])?:FALSE);
					$config["modeltime"] = $config["modelfile"]?filemtime($config["modelfile"]):0;
					$config["json"] = empty($config["json"])?FALSE:(is_array($config["json"])?$config["json"]:[$config["json"]]);
					$config["model"] = TRUE;
					$mtime .= App::f3()->hash($config["modeltime"]);
				}
				if (in_array("route", $generate))
				{
					$config["routefile"] = empty($config["routefile"])?FALSE:($this->getFileName("SCAFFOLDROUTE",$config["routefile"])?:FALSE);
					$config["routetime"] = $config["routefile"]?filemtime($config["routefile"]):0;
					$config["route"] = TRUE;
					$rtime .= App::f3()->hash($config["routetime"]);
				}
			}
			unset($config);
			App::set("scaffold", $scaffold);

			$this->_cacheFileModel = $this->_base . App::f3()->hash($mtime) . ".php";
			$this->_cacheFileRoute = $this->_base . App::f3()->hash($rtime) . ".php";
		}

		/**
		 * Returns the filename where the code of a scaffold should be stored
		 *
		 * @param string|bool $partial A part of the filename indicating the file or false if no partial is configured
		 * @return string|bool Returns the filename if exists or false if not exists or not configured
		 */
		protected function getFileName($key,$partial)
		{
			$file = App::get($key).(empty($partial)?"":strtolower($partial)).".php";
			return is_file($file)?$file:FALSE;
		}

		/**
		 * Generate the scaffold model code file in cache
		 *
		 * @param string $file The file to generate
		 */
		protected function scaffoldModels($file)
		{
			App::log(App::SEVERITY_TRACE, "Regenerate models cache");
			$tmpFile = "<?php\nnamespace Model;";
			foreach(App::get("scaffold") as $className => $config)
			{
				if(empty($config["model"])) continue;
				$content = is_file($config["modelfile"])?
					file_get_contents($config["modelfile"]):
					"class $className extends \\ActiveRecord {  }";
				$content = preg_replace("/(\/\/|\#)\!.*$/im", "", $content);
				$content = preg_replace(["/namespace\sModel\;/i","/<\?php/i","/\?\>/",'#/\*[^*]*\*+([^/][^*]*\*+)*/#',"/\s+/i"], ["","","",""," "], $content);
				$content = preg_replace(["/^\s/i","/\s*$/i"], ["",""], $content);
				if (is_array($json=$config["json"]))
				{
					array_walk($json, function(&$val){$val=strtoupper($val);});
					unset($val);
					if (!preg_match("/(static)?\s?(public|private|protected)?\s?(static)?\s?function\sjson/i", $content))
						$content = preg_replace("/ActiveRecord\s?\{\s?/", "ActiveRecord { protected static function json() { return ['" . implode("','",$json) . "']; } ", $content);
				}
				$tmpFile = $tmpFile . "\n$content";
			}
			file_put_contents($file, $tmpFile);
		}

		protected function scaffoldRoutes($file)
		{
			App::log(App::SEVERITY_TRACE, "Regenerate routes cache");
			$tmpFile = "<?php\nnamespace Route;";
			foreach(App::get("scaffold") as $className => $config)
			{
				if (empty($config["route"])) continue;
				$content = is_file($config["routefile"])?
					file_get_contents($config["routefile"]):
					"class $className extends \\Route {  }";
				$content = preg_replace("/(\/\/|\#)\!.*$/im", "", $content);
				$content = preg_replace(["/namespace\sRoute\;/i","/<\?php/i","/\?\>/",'#/\*[^*]*\*+([^/][^*]*\*+)*/#',"/\s+/i"], ["","","",""," "], $content);
				$content = preg_replace(["/^\s/i","/\s*$/i"], ["",""], $content);
				$tmpFile = $tmpFile . "\n$content";
			}
			file_put_contents($file, $tmpFile);
		}

	}