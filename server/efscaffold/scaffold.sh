#!/bin/bash
set -a
source .env
set +a

# Extract connection details from CONN_STR
SERVER=$(echo $CONN_STR | grep -oP "Server=\K[^;]+")
PORT=$(echo $CONN_STR | grep -oP "Port=\K[^;]+")
DB=$(echo $CONN_STR | grep -oP "DB=\K[^;]+")
DBUSER=$(echo $CONN_STR | grep -oP "UID=\K[^;]+")
DBPASS=$(echo $CONN_STR | grep -oP "PWD=\K[^;]+")

# Execute schema.sql using psql
PGPASSWORD=$DBPASS psql \
    "host=$SERVER \
    port=$PORT \
    dbname=$DB \
    user=$DBUSER \
    sslmode=require" \
    -f schema.sql

# Run EF Core scaffolding
dotnet tool install -g dotnet-ef
dotnet ef dbcontext scaffold "$CONN_STR" Npgsql.EntityFrameworkCore.PostgreSQL \
    --output-dir ./Entities \
    --context-dir . \
    --context MyDbContext \
    --no-onconfiguring \
    --namespace efscaffold.Entities \
    --context-namespace Infrastructure.Postgres.Scaffolding \
    --schema ticktick \
    --force
#    --data-annotations 
#    --force