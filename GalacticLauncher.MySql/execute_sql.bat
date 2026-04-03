@echo off
set "file=%~1"
if defined file (
	docker exec -i galactic-mysql mysql -u root -pHardCodedPass!123 galactic < "%file%"
)