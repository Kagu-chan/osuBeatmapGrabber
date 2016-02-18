<?php
	
	//! Class for generating cached scaffolds
	class Scaffold extends Prefab
	{

		//! Base cache filename
		protected $_base;

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
			$file = $this->_base . App::f3()->hash(filemtime($fName) . $fName) . ".php";

			if (!is_file($file)) $this->scaffold($fName, $file);
			require($file);
		}

		/**
		 * Generate the scaffold code file in cache
		 *
		 * @param string $fName The configuration to use
		 * @param string $file The file to generate
		 */
		protected function scaffold($fName, $file)
		{
			App::f3()->config($fName);
			$tmpFile = "<?php\nnamespace Model;";
			foreach(App::get("scaffold") as $className => $config)
			{
				if (!is_array($config)) $config = [];
				$content = is_file("core/model/".(empty($config["file"])?"":strtolower($config["file"])).".php")?
					file_get_contents("core/model/".strtolower($config["file"]).".php"):
					"class $className extends \\ActiveRecord {  }";
				$content = preg_replace("/(\/\/|\#)\!.*$/im", "", $content);
				$content = preg_replace(["/namespace\sModel\;/i","/<\?php/i","/\?\>/",'#/\*[^*]*\*+([^/][^*]*\*+)*/#',"/\s+/i"], ["","","",""," "], $content);
				$content = preg_replace(["/^\s/i","/\s*$/i"], ["",""], $content);
				if (!empty($config["json"]))
				{
					$json=is_array($config["json"])?$config["json"]:[$config["json"]];
					array_walk($json, function(&$val){$val=strtoupper($val);});
					unset($val);
					if (!preg_match("/(static)?\s?(public|private|protected)?\s?(static)?\s?function\sjson/i", $content))
						$content = preg_replace("/ActiveRecord\s?\{\s?/", "ActiveRecord { protected static function json() { return ['" . implode("','",$json) . "']; } ", $content);
				}
				$tmpFile = $tmpFile . "\n$content";
			}
			file_put_contents($file, $tmpFile);
		}

	}