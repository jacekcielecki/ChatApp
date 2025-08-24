CREATE USER developer_access WITH PASSWORD 'secret_password';
GRANT ALL PRIVILEGES ON DATABASE "chatapp-sql" TO developer_access;