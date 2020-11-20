@echo off

echo RHISIS PROJECT - SETUP

echo Building solution
dotnet build Rhisis.sln

echo Seting up MySQL server
set /p mysqlUsr=MySQL User:
set /p mysqlPwd=MySQL Password:
set /p mysqlDb=MySQL database name:
set /p mysqlPort=MySQL listening port:

echo MYSQL_DATABASE=%mysqlDb% > .env
echo MYSQL_USER=%mysqlUsr% >> .env
echo MYSQL_PASSWORD=%mysqlPwd% >> .env
echo MYSQL_PORT=%mysqlPort% >> .env

docker-compose up -d rhisis.database

cd bin/
echo Setting up Rhisis servers
./rhisis-cli setup
cd ..

docker-compose down
