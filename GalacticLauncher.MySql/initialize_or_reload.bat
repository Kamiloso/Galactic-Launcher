:: ensures that the docker container is properly
:: configured according to the yml file and starts it
:: dont spam this while configuring database for the first time
docker compose down
docker compose up -d
pause