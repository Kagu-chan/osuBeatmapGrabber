<?php
	
	scaffold("project", [
		"ident" => "VARCHAR(64) NOT NULL",
		"public" => "TINYINT(1) DEFAULT '1'",
		"allow_zip" => "TINYINT(1) DEFAULT '1'",
		"version" => "VARCHAR(16) DEFAULT '0'",
		"allow_current" => "TINYINT(1) DEFAULT '0'"
	]);