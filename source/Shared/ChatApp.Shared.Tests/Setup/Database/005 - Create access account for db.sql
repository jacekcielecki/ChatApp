CREATE USER developer_access WITH PASSWORD 'secret_password';
GRANT CONNECT ON DATABASE "chatapp-sql" TO developer_access;

-- Tables
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO developer_access;

-- Sequences
GRANT USAGE, SELECT, UPDATE ON ALL SEQUENCES IN SCHEMA public TO developer_access;

-- Future tables & sequences
ALTER DEFAULT PRIVILEGES IN SCHEMA public
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO developer_access;

ALTER DEFAULT PRIVILEGES IN SCHEMA public
GRANT USAGE, SELECT, UPDATE ON SEQUENCES TO developer_access;
