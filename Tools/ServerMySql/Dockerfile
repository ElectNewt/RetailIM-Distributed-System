FROM mysql:5.6

ENV MYSQL_DATABASE retailim
ENV MYSQL_ROOT_PASSWORD=test

## all scripst into   docker-entrypoint-initdb.d/ are executed automatically
COPY ./Database/ ./docker-entrypoint-initdb.d/