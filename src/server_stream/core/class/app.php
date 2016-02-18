<?php

	/**
	 * App container class
	 *
	 * This class provides the framework in an isolated scope at every point of application
	 */
	final class App {

		//! Log levels definition
		const
			//! Error severity level
			SEVERITY_ERROR = 0,
			//! Warning severity level
			SEVERITY_WARNING = 1,
			//! Notice severity level
			SEVERITY_NOTICE = 2,
			//! Trace severity level
			SEVERITY_TRACE = 3;

		//! App $_instance the instance of singleton object
		private static $_instance = NULL;

		//! Log $_logger The logger instance
		private static $_logger = NULL;

		//! bool $configured indicates if the app is configured
		public static $configured = FALSE;

		//! int $severity The log level severity
		public static $severity = NULL;

		//! Object F3 $_f3 the framework variable
		private $_f3 = NULL;

		/**
		 * The framework access function
		 *
		 * @returns Object F3
		 */
		public static function f3()
		{
			if (self::$_instance == NULL)
				self::$_instance = new App();
			return self::$_instance->g();
		}

		/**
		 * Wrapper for f3()->get
		 *
		 * @param string $key The key
		 * @return mixed The saved object
		 */
		public static function get($key)
		{
			return self::f3()->get($key);
		}

		/**
		 * Wrapper for f3()->set
		 *
		 * @param string $key The key
		 * @param mixed $value The object to store
		 */
		public static function set($key, $value)
		{
			self::f3()->set($key, $value);
		}

		/**
		 * Constructor
		 *
		 * Initiate the framework itself
		 * @return App
		 */
		private function __construct()
		{
			$this->_f3 = require('./core/script/fff.php');
		}

		/**
		 * Return the framework itself
		 *
		 * @return Object F3
		 */
		private function g()
		{
			return $this->_f3;
		}

		//! Configure the mapping routes for REST requests
		public static function configureRest()
		{
			foreach (self::get("rest") as $verb => $class)
			{
				self::f3()->route("GET $verb [sync]", $class . "->get");
				self::f3()->route("POST $verb [sync]", $class . "->post");
				self::f3()->route("PUT $verb [sync]", $class . "->put");
				self::f3()->route("PATCH $verb [sync]", $class . "->patch");
				self::f3()->route("DELETE $verb [sync]", $class . "->delete");
				self::f3()->route("HEAD $verb [sync]", $class . "->head");
				self::f3()->route("GET $verb [ajax]", $class . "->get");
				self::f3()->route("POST $verb [ajax]", $class . "->post");
				self::f3()->route("PUT $verb [ajax]", $class . "->put");
				self::f3()->route("PATCH $verb [ajax]", $class . "->patch");
				self::f3()->route("DELETE $verb [ajax]", $class . "->delete");
				self::f3()->route("HEAD $verb [ajax]", $class . "->head");
			}
		}

		//! Check if users IP address is one of the allowed in the ipaccess configuration, otherwise die with forbidden
		public static function checkAccessOrDie()
		{
			if (!in_array(self::get("IP"), self::get("ipaccess.allowedips"))) self::f3()->error(403);
		}

		/**
		 * Logs a message to the configured log file
		 * 
		 * If no severity is set, it will take the debug level from the app configuration
		 * If the app is not configured, it will break with a "405 Method not allowed!" status response
		 * If the given severity is greater then the maximum allowed severity it will break with a "500 Server Error!" status response
		 * The logger will get instanciated the first time this function is used
		 * The logger will not log if the given severity higher then the maximum configured log severity
		 * The log file is the configured LOGFILE value or "app.log" as fallback
		 *
		 * @param int $severity The log level severity
		 * @param string $message The log message
		 * @see refer<App::$severity>
		 */
		public static function log($severity, $message)
		{
			if (!self::$configured)
				self::f3()->error(405);

			if ($severity > self::SEVERITY_TRACE)
				self::f3()->error(500);

			if (!self::$_logger)
				self::$_logger = new \Log(self::get("LOGFILE"));

			if (self::$severity === NULL)
				self::$severity = self::get("DEBUG");

			if (self::$severity >= $severity)
				self::$_logger->write("[" . ["ERROR", "WARNING", "NOTICE", "TRACE"][$severity] . "] " . $message);
		}
	}