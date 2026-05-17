-- ------------------------------------------------------------------------ --
-- sql script to initialize the database for the galactic launcher backend. --
-- ------------------------------------------------------------------------ --

USE galactic;

-- user setup

DROP USER IF EXISTS 'galactic_app'@'%';
CREATE USER 'galactic_app'@'%' IDENTIFIED BY 'HardCodedPass!123';
GRANT SELECT, INSERT, UPDATE, DELETE ON galactic.* TO 'galactic_app'@'%';

-- schema for all tables

CREATE TABLE games (
    id bigint primary key auto_increment not null,
    name VARCHAR(255) not null,
    description text not null
    );

CREATE TABLE versions (
    id bigint primary key auto_increment not null,
    caption VARCHAR(255) not null,
    type enum('alpha', 'beta', 'release', 'snapshot') not null,
    description text not null,
    is_primary boolean not null,
    release_date date not null default (current_date), 
    platform enum('windows', 'linux', 'macsilicon', 'macintel') not null,
    download_url text not null,
    exec_location text not null,
    alert enum('stable', 'alert', 'danger') not null,
    id_game bigint not null,
    foreign key(id_game) references games(id) on delete cascade
    );

CREATE TABLE images (
    id bigint primary key auto_increment not null,
    download_url text not null,
    type enum('icon', 'screenshot', 'banner') not null,
    sort_index int not null default 0,
    id_game bigint not null,
    foreign key(id_game) references games(id) on delete cascade
    );

CREATE TABLE tags (
    id bigint primary key auto_increment not null,
    name VARCHAR(255) not null unique,
    description text not null
    );

CREATE TABLE games_tags (
    id_game bigint not null,
    id_tag bigint not null,
    primary key(id_game, id_tag),
    foreign key(id_game) references games(id) on delete cascade,
    foreign key(id_tag) references tags(id) on delete cascade
    );

CREATE TABLE history (
    id bigint primary key auto_increment not null,
    info text not null,
    timestamp datetime not null default current_timestamp,
    id_game bigint default null,
    foreign key(id_game) references games(id) on delete cascade
    );