#first build the image
#with appsettings.json sourced: docker build -t api .
#docker run -p 8080:8080 -e APPOPTIONS__DbConnectionString=blabla -e APPOPTIONS__JwtSecret=blablablab api
docker run -p 8080:8080 api