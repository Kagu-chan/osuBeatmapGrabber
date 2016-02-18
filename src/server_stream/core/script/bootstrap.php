<?php

	/**
	 * Define the autoload directory to load the application core library 
	 *
	 * In 'core/class/' we have the default classes and namespaces, mostly plugins and the core F3 library
	 * In 'core/' we have stored all classes for routing and mvc pattern
	 */
	App::set('AUTOLOAD','core/;core/class/');

	//! BASEPATH is the default request path for avoiding conflicts between routing engine and css requests / page links
	App::set('BASEPATH', App::get("SCHEME") . '://' . App::get("HOST") . App::get("BASE") . '/');

	//! Set the default logfile for the logger
	App::set("LOGFILE", "app.log");
	
	//! Load the main configuration of the web service like app assets or routing configuration
	App::f3()->config('./conf/globals.cfg');
	App::f3()->config('./conf/routes.cfg');
	App::f3()->config('./conf/rest.cfg');
	App::f3()->config('./conf/redirects.cfg');
	App::f3()->config('./conf/db.cfg');
	App::f3()->config('./conf/ips.cfg');

	//! Configure ReST Routing Objects
	App::configureRest();

	//! Define the app as configured
	App::$configured = TRUE;
	App::log(App::SEVERITY_TRACE, "Configured App via configuration files");

	//! Autogenerate Scaffold Files
	Scaffold::instance()->execute("./conf/scaffold.cfg");

	//! Instanciate the database connection
	App::set('DB', new DB\SQL(
		'mysql:host=' . App::get("dbhost") . ';port=' . App::get("dbport") . ';dbname=' . App::get("dbname"),
	    App::get("dbuser"),
	    App::get("dbpass")
	));
	App::log(App::SEVERITY_TRACE, "Connected to database at host " . App::get("dbhost"));

	//! Convert array required properties to array if they are not present as array
	if (!is_array(App::get("INCLUDEJS"))) App::set("INCLUDEJS", [App::get("INCLUDEJS")]);
	if (!is_array(App::get("INCLUDECSS"))) App::set("INCLUDECSS", [App::get("INCLUDECSS") => "all"]);
	if (!is_array(App::get("ipaccess.allowedips"))) App::set("ipaccess.allowedips", [App::get("ipaccess.allowedips")]);

	//! Prepare the compression for CSS files
	$css = [];
	foreach (App::get("INCLUDECSS") as $file => $media)
	{
		if (!isset($css[$media])) $css[$media] = [];
		$css[$media][] = "$file.css";
	}
	foreach ($css as $key => &$value) $value = implode(",", $value);
	
	App::set("INCLUDEJS", implode(".js,", App::get("INCLUDEJS")) . ".js");

	App::set("INCLUDECSS", $css);
	App::f3()->route('GET /minify/@type',
		function($f3, $args) {
			App::log(App::SEVERITY_TRACE, "Serve minified files [" . $args['type'] . "](" . filter_input(INPUT_GET, "files") . ")");
			$path = $f3->get('UI') . $args['type'] . '/';
			$files = str_replace('../', '', filter_input(INPUT_GET, "files"));
			echo Web::instance()->minify($files, null, true, $path);
		},
		App::get("STYLECACHE")
	);

	/** 
	 * Decide the HTML client language
	 *
	 * Only available languages are accepted
	 * The first client accepted language will be the desired language if available
	 * In all other cases it fallback to the configured fallback language
	 */
	$lang = explode(",", App::get('LANGUAGE'))[0];
	if (!is_array(App::get("LANGAVAILABLE")) || !in_array($lang, App::get("LANGAVAILABLE"))) $lang = App::get("FALLBACK");
	App::set('HTMLLANG', $lang);
	App::log(App::SEVERITY_TRACE, "Defined client language to $lang based on " . App::get('LANGUAGE'));