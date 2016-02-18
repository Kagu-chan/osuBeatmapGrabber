<?php

	/* Request the main core code to run application */
	require("core/class/app.php");
	require("core/script/bootstrap.php");

	/* Run preconfigured application */
	App::f3()->run();

	/* Log end of command chain */
	App::log(App::SEVERITY_TRACE, "End of Request => Success\n");