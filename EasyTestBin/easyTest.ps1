param(
[string]$easyTestPath=".\EasyTests", 
[string]$easyTestBinPath=".\EasyTestBin",
[string]$WinBinPath="XCRM.Win\bin\Debug",
[string]$WebBinPath="XCRM.Web\bin"
) 

sqllocaldb start MSSQLLocalDB

Copy-Item -Path $easyTestBinPath\*.dll -Destination $WinBinPath -exclude DevExpress.ExpressApp.EasyTest.WebAdapter*
Copy-Item -Path $easyTestBinPath\*.dll -Destination $WebBinPath -exclude DevExpress.ExpressApp.EasyTest.WinAdapter*

$testExecutorItem = Get-ChildItem -Path $easyTestBinPath -Include TestExecutor.v*.exe -File -Recurse
$testExecutorPath = $easyTestBinPath + "\" + $testExecutorItem[0].Name

& $testExecutorPath $easyTestPath

$easyTestLogPath = $easyTestPath + "\TestsLog.xml"
if(Select-String -Pattern 'Result="Failed"', 'Result="Warning"' -Path $easyTestLogPath) { exit 1 }
