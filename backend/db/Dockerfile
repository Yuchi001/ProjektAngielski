FROM mariadb:latest
COPY 01_schema.sql /docker-entrypoint-initdb.d/
COPY 02_populate_achievements.sql /docker-entrypoint-initdb.d/