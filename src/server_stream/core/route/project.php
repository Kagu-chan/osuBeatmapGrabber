<?php
	
	namespace Route;

		/**
		 * @inheritdoc
		 */
		class Project extends \Route
		{

			/**
			 * @inheritdoc
			 */
			public function listing()
			{
				parent::listing();
				\App::set("projects", \Model\Project::all(true));
			}

		}