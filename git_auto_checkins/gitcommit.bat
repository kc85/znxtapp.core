@echo off
cd  "C:\Users\Khanin\Documents\GitHub\znxtapp.core" 

set file="C:\Users\Khanin\Google Drive\Code\znxtapp.core\gitcomment.txt"

echo "GIT COMMAND STARTED---"
git status
git add --a
git commit -F %file%

git push origin master
echo "GIT COMMAND END---"

del %file%
type NUL > %file%
echo "FILE UPDTAE END"

