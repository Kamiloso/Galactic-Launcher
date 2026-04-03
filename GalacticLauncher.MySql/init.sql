-- SQL script to initialize the database for the Galactic Launcher backend.
-- Everything should be idempotent here, so it can be run multiple times without causing errors.

CREATE USER IF NOT EXISTS 'galactic_app'@'%' IDENTIFIED BY 'HardCodedPass!123';
REVOKE ALL PRIVILEGES, GRANT OPTION FROM 'galactic_app'@'%';
GRANT SELECT, INSERT, UPDATE, DELETE ON galactic.* TO 'galactic_app'@'%';

CREATE DATABASE IF NOT EXISTS galactic;
USE galactic;

-- table for initialization logs (for debugging purposes)

CREATE TABLE IF NOT EXISTS init_logs (
    Id INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    InitTime DATETIME NOT NULL
);

INSERT INTO init_logs (InitTime) VALUES (CURRENT_TIMESTAMP);

-- database schema for all tables

CREATE TABLE IF NOT EXISTS games (
    Id BIGINT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    Name VARCHAR(255) NOT NULL,
    Description TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS versions (
    Id BIGINT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    VersionText VARCHAR(255) NOT NULL,
    Description TEXT NOT NULL,
    IsPrimary BOOLEAN NOT NULL,
    ReleaseDate DATETIME NOT NULL,
    VersionType INT NOT NULL,
    IdGame BIGINT NOT NULL,

    FOREIGN KEY(IdGame) REFERENCES games(Id)
    ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS images (
    Id BIGINT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    Url TEXT NOT NULL,
    IdGame BIGINT NOT NULL,

    FOREIGN KEY(IdGame) REFERENCES games(Id)
    ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS execs (
    Id BIGINT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    DownloadUrl TEXT NOT NULL,
    ExecLocation TEXT NOT NULL,
    FileHashSHA256 TEXT NOT NULL,
    Platform INT NOT NULL,
    Alert INT NOT NULL,
    IdVersion BIGINT NOT NULL,

    FOREIGN KEY(IdVersion) REFERENCES versions(Id)
    ON DELETE CASCADE
);

-- Add migrations here:
-- ALTER TABLE example_table ADD COLUMN new_column VARCHAR(255);