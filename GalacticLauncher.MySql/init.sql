-- SQL script to initialize the database for the Galactic Launcher backend.
-- Everything should be idempotent here, so it can be run multiple times without causing errors.

CREATE USER IF NOT EXISTS 'galactic_app'@'%' IDENTIFIED BY 'HardCodedPass!123';
REVOKE ALL PRIVILEGES, GRANT OPTION FROM 'galactic_app'@'%';
GRANT SELECT, INSERT, UPDATE, DELETE ON galactic.* TO 'galactic_app'@'%';

CREATE DATABASE IF NOT EXISTS galactic;
USE galactic;

-- table for initialization logs (for debugging purposes)

CREATE TABLE IF NOT EXISTS init_logs (
    id INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    init_time DATETIME NOT NULL);

INSERT INTO init_logs (InitTime) VALUES (CURRENT_TIMESTAMP);

-- database schema for all tables

CREATE TABLE IF NOT EXISTS users (
    id BIGINT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    google_key VARCHAR(200) NOT NULL,
    email VARCHAR(200) NOT NULL,
    name VARCHAR(50) NOT NULL,
    profile_url VARCHAR(2048) NOT NULL,
    UNIQUE (email),
    UNIQUE (google_key));

CREATE TABLE IF NOT EXISTS games (
    id BIGINT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    name VARCHAR(255) NOT NULL,
    description VARCHAR(1000) NOT NULL);

CREATE TABLE IF NOT EXISTS versions (
    id BIGINT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    version_text VARCHAR(50) NOT NULL,
    description VARCHAR(200) NOT NULL,
    is_primary BOOLEAN NOT NULL,
    release_date DATETIME NOT NULL,
    version_type INT NOT NULL,
    id_game BIGINT NOT NULL,
    FOREIGN KEY(id_game) REFERENCES games(id) ON DELETE CASCADE);

CREATE TABLE IF NOT EXISTS images (
    id BIGINT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    url VARCHAR(2048) NOT NULL,
    id_game BIGINT NOT NULL,
    FOREIGN KEY(id_game) REFERENCES games(id) ON DELETE CASCADE);

CREATE TABLE IF NOT EXISTS tags (
    id BIGINT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    name VARCHAR(50) NOT NULL,
    description VARCHAR(200) NOT NULL);

CREATE TABLE IF NOT EXISTS games_tags (
    id_game BIGINT NOT NULL,
    id_tag BIGINT NOT NULL,
    PRIMARY KEY (id_game, id_tag),
    FOREIGN KEY (id_game) REFERENCES games(id) ON DELETE CASCADE,
    FOREIGN KEY (id_tag) REFERENCES tags(id) ON DELETE CASCADE);

CREATE TABLE IF NOT EXISTS execs (
    id BIGINT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    download_url VARCHAR(2048) NOT NULL,
    exec_location VARCHAR(2048) NOT NULL,
    file_hash_sha256 CHAR(64) NOT NULL,
    platform INT NOT NULL,
    alert INT NOT NULL,
    id_version BIGINT NOT NULL,
    FOREIGN KEY(id_version) REFERENCES versions(id) ON DELETE CASCADE);

CREATE TABLE IF NOT EXISTS actions (
    id BIGINT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    type INT NOT NULL,
    time DATETIME(6) NOT NULL,
    id_user BIGINT NOT NULL,
    id_exec BIGINT NULL,
    FOREIGN KEY (id_user) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (id_exec) REFERENCES execs(id) ON DELETE SET NULL);

CREATE INDEX ix_actions_user_id ON actions (id_user);
CREATE INDEX ix_versions_game_id ON versions (id_game);
CREATE INDEX ix_images_game_id ON images (id_game);
CREATE INDEX ix_execs_version_id ON execs (id_version);

-- Add migrations here:
-- ALTER TABLE example_table ADD COLUMN new_column VARCHAR(255);