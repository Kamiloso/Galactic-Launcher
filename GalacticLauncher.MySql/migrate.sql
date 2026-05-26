-- migrations

-- Add author field to games
ALTER TABLE games ADD COLUMN author VARCHAR(255) NOT NULL DEFAULT 'Unknown';

-- Add cli_args field to versions
ALTER TABLE versions ADD COLUMN cli_args VARCHAR(500) DEFAULT NULL;