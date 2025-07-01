#first build the image with:
# docker build -t api .

#then run with optional environment arguments: (or defaults to appsettings.json)
#docker run -p 8080:8080 -e APPOPTIONS__DbConnectionString=blabla -e APPOPTIONS__JwtSecret=blablablab api
docker run -p 8080:8080 api