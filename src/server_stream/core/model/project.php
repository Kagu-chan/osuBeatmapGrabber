<?php
	
	namespace Model;

		/**
		 * @inheritdoc
		 */
		class Project extends \ActiveRecord {

			/**
			 * @inheritdoc
			 */
			protected static function json()
			{
				return ["GUID", "IDENT", "VERSION"];
			}
		}