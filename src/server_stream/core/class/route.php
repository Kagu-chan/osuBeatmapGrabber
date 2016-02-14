<?php
	
	/**
	 * Base Routing Class
	 *
	 * This class represents a type of base controller class with default REST and Controller functions.
	 */
	abstract class Route
	{

		//! callback $beforeRenderCallback This callback will get called before render process in afterRoute function
		protected $beforeRenderCallback = NULL;

		/**
		 * List a subset of records
		 *
		 * Per default it sets JSON to true
		 */
		public function listing() {
			\App::set("JSON", TRUE);
			\App::set("ACTION", "list");
			\Helper\Routing::instance()->generateListingResult();
		}

		/**
		 * index action
		 *
		 * Do nothing except render an index view
		 */
		public function index() { /* NOP */ }

		/**
		 * REST GET
		 */
		public function get() {
			\App::set("ACTION", "show");
			\Helper\Routing::instance()->generateShowResult();
		}

	    public function post() {
	    	\App::set("ACTION", "show");
	    }

	    public function put() {
	    	\App::set("ACTION", "show");
	    }

	    public function patch() {
	    	\App::set("ACTION", "show");
	    }

	    public function delete() {
	    	\App::set("ACTION", "list");
	    }
	    
	    public function head() {
	    	\App::set("ACTION", "show");
	    }

	    /**
	     * before route action
	     *
	     * Called in front of every action
	     * Defines some render constants
	     */
	    public function beforeRoute() {
	    	list($prepath, $controller, $action) = explode("/", App::get("PATH"));
	    	App::set("PREPATH", $prepath); /* UNUSED! */
	    	App::set("CONTROLLER", $controller);
	    	App::set("ACTION", $action);
	    	App::set("RENDERTEMPLATE", FALSE);
	    	App::set("CURRENTTITLE", FALSE);
	    	App::set("JSON", App::get("AJAX"));
	    	App::log(App::SEVERITY_TRACE, "Default route desired to [$controller] => [$action] from [" . App::get("PATH") . "](" . (App::get("AJAX") ? "AJAX" : "SYNC") . ")");
	    }

	    /**
	     * after route action
	     *
	     * Called after every action
	     * Rehash the final view and render it
	     */
	    public function afterRoute() {
	    	$this->setContent();
	    	$this->setTitle();

	    	if ($this->beforeRenderCallback !== NULL && is_callable($this->beforeRenderCallback)) $this->beforeRenderCallback();

	    	$this->render();
	    }

	    //! Set the HTML title constant from action or CURRENTTITLE constant
	    protected function setTitle()
	    {
	    	$currentTitle = App::get("CURRENTTITLE");
	    	$controllerName = App::get("CONTROLLER");

	    	App::set("CURRENTTITLE", $currentTitle ? $currentTitle : ucfirst(strtolower($controllerName ? $controllerName : "home")));
	    }

	    //! Calculate the render view if RENDERTEMPLATE is not set
	    protected function setContent()
	    {
	    	$renderTemplate = App::get("RENDERTEMPLATE");
	    	$controllerName = App::get("CONTROLLER");
	    	$actionName = App::get("ACTION");

	    	$targetTemplate = implode(DIRECTORY_SEPARATOR, [
	    		$controllerName ? $controllerName : "home",
	    		$actionName ? $actionName : "index"
	    	]) . ".htm";

	    	App::set("CONTENT", $renderTemplate ? $renderTemplate : $targetTemplate);
	    }

	    //! Render the final view regarding the JSON constant
	    protected function render()
	    {
	    	$layout = App::get("JSON") === TRUE ? App::get("CONTENT") : "layout.htm";
	    	App::log(App::SEVERITY_TRACE, "Render file $layout" . ($layout == "layout.htm" ? " with subset " . App::get("CONTENT") : ""));

    		echo Template::instance()->render($layout);
	    }
	}