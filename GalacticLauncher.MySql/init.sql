-- SQL script to initialize the database for the Galactic Launcher backend.
-- Everything should be idempotent here, so it can be run multiple times without causing errors.

CREATE USER IF NOT EXISTS 'galactic_app'@'%' IDENTIFIED BY 'HardCodedAppPassword!123';
REVOKE ALL PRIVILEGES, GRANT OPTION FROM 'galactic_app'@'%';
GRANT SELECT, INSERT, UPDATE, DELETE ON galactic.* TO 'galactic_app'@'%';

CREATE DATABASE IF NOT EXISTS galactic;
USE galactic;

-- test table for initialization logs
CREATE TABLE IF NOT EXISTS init_logs (
    id INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    init_time DATETIME NOT NULL
);

INSERT INTO init_logs (init_time) VALUES (CURRENT_TIMESTAMP);

-- Add migrations here:
-- ALTER TABLE example_table ADD COLUMN new_column VARCHAR(255);