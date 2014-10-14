@echo off
REM saving paths to images
REM you can also use absolute paths. i.e "C:\something.png"
set image1="firefox1.png"
set image2="firefox2.png"

REM print what is about to happen
echo 'ConsoleImageComparison.exe %image1% %image2%'

REM execute the program
call ConsoleImageComparison.exe %image1% %image2%

REM tell what the detected difference is
echo The difference is %ERRORLEVEL%%%

pause
