<?php
	$uniqueId = filter_input(INPUT_SERVER, "UNIQUE_ID");
	if ($uniqueId !== NULL) {
		http_response_code(400);
		die("<h1>Bad Request!</h1>");
	}