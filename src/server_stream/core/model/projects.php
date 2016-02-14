<?php
	
	namespace Model;

		/**
		 * @inheritdoc
		 */
		class Projects extends \ActiveRecord {

			/**
			 * @inheritdoc
			 */
			protected static function json()
			{
				return ["GUID", "IDENT", "VERSION"];
			}
		}