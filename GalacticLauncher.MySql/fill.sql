USE galactic;

-- 1. Dodawanie gier
INSERT IGNORE INTO games (id, name, author, description) VALUES
(4, 'Teeworlds', 'Magda', 'A free online multiplayer game, available for all major operating systems. Battle with up to 16 players in a variety of game modes, including Team Deathmatch and Capture The Flag. You can even design your own maps!'),
(5, 'Sonic 2 Robot Blast', 'Magda', "Sonic Robo Blast 2 is a 3D open-source Sonic the Hedgehog fangame built using a modified version of the Doom Legacy port of Doom. SRB2 is closely inspired by the original Sonic games from the Sega Genesis, and attempts to recreate the design in 3D. While SRB2 isn't fully completed, it already features tons of levels, enemies, speed, and quite a lot of the fun that the original Sonic games provided."),
(6, 'Canabalt', 'Kamiloso', 'Endless runner game'),
(7, 'Endless sky', 'Kamiloso', 'Explore other star systems. Earn money by trading, carrying passengers, or completing missions. Use your earnings to buy a better ship or to upgrade the weapons and engines on your current one. Blow up pirates. Take sides in a civil war. Or leave human space behind and hope to find some friendly aliens whose culture is more civilized than your own...'),
(8, 'Dust racing', 'Vladyslav', '2D racing game'),
(9, 'SDL slopwidth', 'Vladyslav', '2D planes game');

-- 2. Dodawanie tagów
INSERT IGNORE INTO tags (id, name, description) VALUES
(1, 'Multiplayer', 'Games that support multiple players interacting online.'),
(2, 'Procedural Generation', 'Games where content is generated algorithmically.'),
(3, 'Sandbox', 'Games providing the player with a great degree of creativity.'),
(4, '2D', 'Games with two-dimensional graphics and sprites.'),
(5, 'Engine-Independent', 'Projects built with custom architecture independent of standard engines.');

-- 4. Dodawanie wersji gier (Poprawione 12 wartości na wiersz i podwójne ukośniki)
INSERT IGNORE INTO versions (id, id_game, caption, type, description, is_primary, release_date, platform, download_url, exec_location, sha256_hash, alert) VALUES
(6, 4, 'v0.7.5','release','Fixed using correct array measurements when placing egg doodads',1,'2025-11-11','windows','https://github.com/teeworlds/teeworlds/releases/download/0.7.5/teeworlds-0.7.5-win64.zip','teeworlds-0.7.5-win64\\teeworlds.exe',null,'stable'),
(7, 4, 'v0.7.3.1','release','Fix platform-specific client libraries for Linux',1,'2025-11-11','windows','https://github.com/teeworlds/teeworlds/releases/download/0.7.3.1/teeworlds-0.7.3.1-win64.zip','teeworlds-0.7.3.1-win64\\teeworlds.exe',null,'stable'),
(8, 5, 'v1.22.1.5','release','Fixed nothing',1, '2025-11-11','windows','https://github.com/STJr/SRB2/releases/download/SRB2_release_2.2.15/SRB2-v2215-Full.zip','Sonic.exe',null, 'stable'),
(9, 6, 'v1.1.1','release','Fixed bleeding',1,'2025-11-11','windows','https://github.com/ninjamuffin99/canabalt-hf/releases/download/bleeding/canabalt-windows-2024-07-11-main.zip','Canabalt.exe',null,'stable'),
(10, 7, 'v0.10.16','release','Reverted changes to movement AI from the previous release, fixing various issues with ship movement introduced there.',1,'2025-11-11','windows','https://github.com/endless-sky/endless-sky/releases/download/v0.10.16/EndlessSky-win64-v0.10.16.zip', 'EndlessSky.exe',null,'stable'),
(11, 8, 'v2.1.1', 'release','',1,'2025-11-11','windows','https://github.com/juzzlin/DustRacing2D/releases/download/2.1.1/dustracing2d-2.1.1-windows-x86.zip','Dustracing.exe',null,'stable'),
(12,9, 'v1.1','release','',1,'2025-11-11','windows','https://github.com/fragglet/sdl-sopwith/releases/download/sdl-sopwith-2.9.0/sdl-sopwith-2.9.0-win64.zip','Sdl.exe',null,'stable');

-- 5. Dodawanie obrazów (Assetów)
INSERT IGNORE INTO images (id, id_game, download_url, type, sort_index) VALUES
(9, 4, 'https://drive.google.com/uc?id=19IpyVBOhjIRE4tbLcYhzQ6bhXFzEJn1X&export=download','icon',0),
(10, 5, 'https://drive.google.com/uc?id=1yCbBx7WG7NWqXaqUm8Ti7QRAhy1X_Wnd&export=download','icon',0),
(11, 6, 'https://drive.google.com/uc?id=1jzexuK2J_oEIHvkDfyrky5G_xE9FzqF&export=download', 'icon', 0),
(12, 7, 'https://drive.google.com/uc?id=1-SU_jwne0QwjMuciW4c83FG5ZBTDrT8D&export=download','icon',0),
(13, 8, 'https://drive.google.com/uc?id=1tAa5xbpbviIi5xhB8VIKoKELq8I50k7k&export=download','icon',0),
(14, 9, 'https://drive.google.com/uc?id=12JW5hJlSFo9G-Xymtk3vYUjLRTetEVMD&export=download','icon',0);