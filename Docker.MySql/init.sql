CREATE USER IF NOT EXISTS 'galactic_app'@'%' IDENTIFIED BY 'HardCodedAppPassword!123';
GRANT SELECT, INSERT, UPDATE, DELETE ON Galactic.* TO 'galactic_app'@'%';
FLUSH PRIVILEGES;