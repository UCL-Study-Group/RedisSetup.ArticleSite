#!/bin/bash

# Start SQL Server in the background
/opt/mssql/bin/sqlservr &

# Wait for SQL Server to start (increased wait time)
echo "Waiting for SQL Server to start..."
sleep 20s

# Check if init.sql exists and run it
if [ -f "/docker-entrypoint-initdb.d/init.sql" ]; then
    echo "Running initialization script..."
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Password123!" -C -i "/docker-entrypoint-initdb.d/init.sql"
    echo "Database initialization completed!"
else
    echo "init.sql not found at /docker-entrypoint-initdb.d/init.sql"
    ls -la /docker-entrypoint-initdb.d/ || echo "Directory doesn't exist"
fi

# Keep SQL Server running in the foreground
wait