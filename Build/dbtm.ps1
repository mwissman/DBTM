Properties {
	$build_dir = Split-Path $psake.build_script_file
	$base_dir = join-path $build_dir "..\"
	$testOutputDir = "$base_dir\Source\testsOutput"
	$solutionConfig= if ($env:solutionConfig) { $env:solutionConfig } else { "Debug" } 
	$version = if ($env:APPVEYOR_BUILD_VERSION) {$env:APPVEYOR_BUILD_VERSION } else { "1.0.0.0"}
	$solution="DB Transition Manager.sln"
	$nunitRunnerExe="$base_dir\packages\NUnit.Runners.2.6.3\tools\nunit-console.exe"
}

FormatTaskName (("-"*25) + "[{0}]" + ("-"*25))

Task Default -Depends Build

Task Build -Depends Clean, Compile, UnitTests, PackageCommandLine

Task Compile -Depends Clean {	
	Write-Host "Building $solution in $solutionConfig mode" -ForegroundColor Green
	Exec { msbuild "$base_dir\$solution" /t:Build /p:Configuration=$solutionConfig /v:minimal} 
}

Task Clean {
	
	
	Write-Host "Cleaning $solution" -ForegroundColor Green
	Exec { msbuild "$base_dir\$solution" /t:Clean /p:Configuration=$solutionConfig /v:minimal } 

	if (test-path $testOutputDir) 
	{
		Write-Host "Cleaning Test Directory" -ForegroundColor Green
		Remove-Item $testOutputDir\* -recurse -Force
	}
}

Task UnitTests {

	$testDlls=get-childitem -path $testOutputDir\*.* -include *test*.dll
	$testFiles ="";
	foreach($testDll in $testDlls)
	{
		$testFiles = """$testDll"" $testFiles"
	}
	write-host "Test dlls to run: $testFiles"

	exec { & $nunitRunnerExe $testFiles /framework=net-4.0}
}


Task PackageCommandLine -precondition { if ($env:APPVEYOR ) {$true } else { $false} } {
	nuget pack "$base_dir\$solution\dbtmCommandLine.nuspec" -Version $version -OutputDirectory "$base_dir\__deployPackages"
}