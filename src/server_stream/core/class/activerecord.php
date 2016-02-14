<?php
	
	/**
	 * Base Model Class
	 *
	 * Provides a subset of useful functions working with the database
	 */
	abstract class ActiveRecord
	{
		/**
		 * Decides which field should be present in a json result
		 *
		 * @return array<string> List of properties
		 */
		protected static function json() { return []; }

		/**
		 * Returns an array only containing the desired properties
		 *
		 * @param DB\SQL\Mapper $object The source
		 * @param array<string> $keys List of required properties
		 * @return array<mixed> Array only containing the desired properties
		 */
		private static function filterKeys($object, $keys)
		{
			$result = [];
			foreach ($keys as $key) {
				$result[$key] = $object->$key;
			}

			return $result;
		}

		/**
		 * Get the JSON array representation of a list of objects or an single object
		 *
		 * @param DB\SQL\Mapper | array<DB\SQL\Mapper> Single object or list of objects
		 * @return array<array> The filtered result list
		 * @see refer<ActiveRecord::filterKeys>
		 */
		protected static function getJson($objects)
		{
			$class = get_called_class();
			$result = [];

			if (is_array($objects))
				foreach ($objects as $current)
					$result[] = self::filterKeys($current, $class::json());
			else
				$result[] = self::filterKeys($objects, $class::json());

			return $result;
		}

		/**
		 * Returns the name of the table for this object
		 *
		 * @return string The table name
		 */
		private static function table()
		{
			$name = get_called_class();
			$tupel = explode("\\", $name);
			return strtolower($tupel[count($tupel) - 1]);
		}

		/**
		 * Gets the database instance for this table
		 *
		 * @return DB\SQL\Mapper The database mapper instance
		 * @see refer<ActiveRecord::table>
		 */
		private static function getDBInstance()
		{
			$table = self::table();
			\App::log(\App::SEVERITY_TRACE, "Retrive database mapper for `$table`");

			$instance = new \DB\SQL\Mapper(\App::get("DB"), $table, \App::get("DBCACHE"));
			return $instance;
		}

		/**
		 * Load all not deleted records of this object from the database
		 *
		 * @param bool $json=false If true, then the result contains an array with filtered object arrays
		 * @return array<DB\SQL\Mapper> | array<array> List of arrays or list of objects
		 * @see refer<ActiveRecord::getJson>
		 */
		public static function all($json=false)
		{
			$result = self::getDBInstance()->select("*", array('DELETED=?', 0));
			\App::log(\App::SEVERITY_TRACE, "Loaded all (" . count($result) . " result[s]) datasets from table [convert to json is set to " . ($json ? "TRUE]" : "FALSE]"));

			return $json ? self::getJson($result) : $result;
		}

		/**
		 * Load a single record by given field from database
		 *
		 * @param string $field the field name to compare with
		 * @param mixed the value the filed should have
		 * @param bool $json=false If true, then the result contains an array with filtered object arrays
		 * @return array<DB\SQL\Mapper> | array<array> List of arrays or list of objects
		 * @see refer<ActiveRecord::getJson>
		 */
		public static function by($field, $value, $json=false)
		{
			$result = self::getDBInstance()->select("*", array(strtoupper($field) . '=? AND DELETED=?', $value, 0));
			\App::log(\App::SEVERITY_TRACE, "Loaded by $field [$value] (" . count($result) . " result[s]) from table [convert to json is set to " . ($json ? "TRUE]" : "FALSE]"));

			return $json ? self::getJson($result) : $result;
		}

	}