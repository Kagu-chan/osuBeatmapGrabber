<?php
	
	namespace Route;

		/**
		 * @inheritdoc
		 */
		class Home extends \Route
		{

			/**
			 * index action
			 *
			 * Send status "400 Bad Request!"
			 */
			public function index()
			{
				\App::log(\App::SEVERITY_NOTICE, "Access to Home/index action");
				\App::f3()->error(400);
			}

		}