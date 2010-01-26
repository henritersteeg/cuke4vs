del %~dp0\bin\%1\Cucumber.zip

pushd %~dp0\ItemTemplates
%~dp0\..\..\tools\7zip\7za a %~dp0\bin\%1\Cucumber.zip *
popd
