USE galactic;

INSERT INTO games (id, name, author, description) VALUES
(1, 'Teeworlds', 'Robyt3', 'Teeworlds is a free online multiplayer game, available for all major operating systems. Battle with up to 16 players in a variety of game modes, including Team Deathmatch and Capture The Flag. You can even design your own maps!'),
(2, 'Sonic Robo Blast 2', 'LJSonik', 'Sonic Robo Blast 2 is a 3D open-source Sonic the Hedgehog fangame built using a modified version of the Doom Legacy port of Doom. SRB2 is closely inspired by the original Sonic games from the Sega Genesis, and attempts to recreate the design in 3D. While SRB2 isnt fully completed, it already features tons of levels, enemies, speed, and quite a lot of the fun that the original Sonic games provided.'),
(3, 'Canabalt', 'Adam Atomic', 'Escape the destruction of your city with just one button! The game that popularized the infinite runner genre is back with two-player mode, new challenges, new music, new achievements, and new leaderboards!'),
(4, 'Endless sky', 'juzzlin', 'Explore other star systems. Earn money by trading, carrying passengers, or completing missions.'),
(5, 'Dust racing', 'Vladyslav', 'Dust Racing 2D (Dustrac) is a tile-based, cross-platform 2D racing game written in Qt (C++) and OpenGL. Dust Racing 2D comes with a Qt-based level editor for easy level creation. A separate engine, MiniCore, is used for physics modeling.'),
(10, 'SuperTuxKart', 'STK Team', 'A beautifully crafted 3D open-source arcade racing game featuring a variety of mascot characters, colorful tracks, and diverse gameplay modes. Players can experience a full story-driven single-player campaign, participate in classic Grand Prix tournaments, battle in specialized arenas, or test their driving reflexes in time trials. The game fully supports local split-screen action as well as massive online multiplayer races driven by an advanced physics engine.'),
(18, 'Space eternity 3','Kamil Szkop','Explore space and fight with bosses');


INSERT INTO versions (id, id_game, caption, type, description, cli_args, is_primary, release_date, platform, download_url, exec_location, sha256_hash, alert) VALUES
-- Wersje dla Teeworlds (ID 1)
(1, 1, 'v0.7.5', 'release', 'Fixed using correct array measurements when placing egg doodads','', 1, '2025-11-11', 'windows', 'https://github.com/teeworlds/teeworlds/releases/download/0.7.5/teeworlds-0.7.5-win64.zip', 'teeworlds-0.7.5-win64\\teeworlds.exe', '83CD75884A0E8339B4910DA1BFE4D737AF794D35600CD065E7C2222750B37585', 'stable'),
(50, 1, 'v0.7.5', 'release', 'Fixed using correct array measurements when placing egg doodads','', 1, '2025-11-11', 'linux', 'https://drive.google.com/uc?id=1ghoYSIfmhuAoBW4GIW0rgL0i7WjEZmxJ&export=download', 'teeworlds-0.7.5-win64\\teeworlds', null, 'stable'),
(2, 1, 'v0.7.3.1', 'release', 'Fix platform-specific client libraries for Linux','', 0, '2025-05-10', 'windows', 'https://github.com/teeworlds/teeworlds/releases/download/0.7.3.1/teeworlds-0.7.3.1-win64.zip', 'teeworlds-0.7.3.1-win64\\teeworlds.exe', null, 'stable'),
(40, 1, 'v0.7.2.1', 'release', 'Small fixes','', 0, '2025-03-10', 'windows', 'https://github.com/teeworlds/teeworlds/releases/download/0.7.2/teeworlds-0.7.2-win64.zip', 'teeworlds-0.7.2-win64\\teeworlds.exe', null, 'stable'),

-- Wersje dla Sonic Robo Blast 2 (ID 2)
(3, 2, 'v1.22.1.5', 'release', 'Fixed nothing','', 1, '2025-11-11', 'windows', 'https://github.com/STJr/SRB2/releases/download/SRB2_release_2.2.15/SRB2-v2215-Full.zip', 'srb2win.exe', null, 'stable'),
(41, 2, 'v2.2.8', 'release', 'Fixed nothing','', 0, '2024-05-11', 'windows', 'https://github.com/STJr/SRB2/releases/download/SRB2_release_2.2.8/SRB2-v2.2.8-Full.zip', 'srb2win.exe', null, 'stable'),

-- Wersje dla Canabalt (ID 3)
(4, 3, 'v1.1.1', 'release', 'Fixed bleeding','', 1, '2025-11-11', 'windows', 'https://github.com/ninjamuffin99/canabalt-hf/releases/download/bleeding/canabalt-windows-2024-07-11-main.zip', 'canabalt.exe', null, 'stable'),
(51, 3, 'v1.1.1', 'release', 'Fixed bleeding','', 1, '2025-11-11', 'linux', 'https://drive.google.com/uc?id=1D0Xiq0YnRv2DDDYgeQNdLsnlN_ZtfHtc&export=download', 'Canabalt-1-1-1', null, 'stable'),

-- Wersje dla Endless sky (ID 4)
(5, 4, 'v0.10.16', 'release', 'Reverted changes to movement AI.','', 1, '2025-11-11', 'windows', 'https://github.com/endless-sky/endless-sky/releases/download/v0.10.16/EndlessSky-win64-v0.10.16.zip', 'Endless Sky.exe', null, 'stable'),

-- Wersje dla Dust racing (ID 5)
(6, 5, 'v2.1.1', 'release', 'Stable update','', 1, '2025-11-11', 'windows', 'https://github.com/juzzlin/DustRacing2D/releases/download/2.1.1/dustracing2d-2.1.1-windows-x86.zip', 'dustracing2d-2.1.1-windows-x86\\dustrac-game.exe', null, 'stable'),
(52, 5, 'v2.1.1', 'release', 'Stable update','', 1, '2025-11-11', 'linux', 'https://drive.google.com/uc?id=13VRi3vJcew6nj1Oj3AX_w_UJqZX5umg3&export=download', 'Dustracing', null, 'stable'),

-- SuperTuxKart (ID 10)
(31, 10, 'v1.4', 'release', 'Stable release 1.4. Features new tracks.','', 1, '2025-10-01', 'windows', 'https://github.com/supertuxkart/stk-code/releases/download/1.5-rc1/SuperTuxKart-1.5-rc1-win.zip', 'SuperTuxKart-1.5-rc1-win\\run-game.bat', null, 'stable'),


-- Space eternity
(36, 18,'v2.3b','beta','A patch for version Release 2.3 containing a massive memory leak fix.It includes Windows build, Linux build and the source code. The game server is included in the source code inside ServerReady directory as a node.js project as well as the authorization server in the AuthorizationServer directory. Only client has changed in this update.','',1, '2025-04-18', 'windows', 'https://github.com/Space-Eternity-3/Space-Eternity-3/releases/download/release%2FRelease-2.3b/SE3-Release-2.3b-win32-x86_64.zip', 'Space Eternity 3.exe', null, 'stable');



INSERT INTO images (id, id_game, download_url, type, sort_index) VALUES
(1, 1, 'https://drive.google.com/uc?id=19IpyVBOhjIRE4tbLcYhzQ6bhXFzEJn1X&export=download','icon',0),
(2, 2, 'https://drive.google.com/uc?id=1yCbBx7WG7NWqXaqUm8Ti7QRAhy1X_Wnd&export=download','icon',0),
(3, 3, 'https://drive.google.com/uc?id=1jzexuK2J_oEIHvkDfyrky5G_xE9FzqF&export=download', 'icon', 0),
(4, 4, 'https://drive.google.com/uc?id=1-SU_jwne0QwjMuciW4c83FG5ZBTDrT8D&export=download','icon',0),
(5, 5, 'https://drive.google.com/uc?id=1tAa5xbpbviIi5xhB8VIKoKELq8I50k7k&export=download','icon',0),
(9, 4, 'https://drive.google.com/uc?id=19IpyVBOhjIRE4tbLcYhzQ6bhXFzEJn1X&export=download','icon',0),
(15,10,'https://drive.google.com/uc?id=1OE6dWng0oQl7aAQVWyL9pRT8TlkHFYcF&export=download','icon',0),
(18,1,'https://drive.google.com/uc?id=1jGYjCLVmhn5JkcZwpZy44PkIsJ9bOYwE&export=download','banner',0),
(19,2,'https://drive.google.com/uc?id=1Sme3nDNfUdEHawZ8J0waUqIxZbjH4mLc&export=download','banner',0),
(20,3,'https://drive.google.com/uc?id=1QsLzvcU5oy8aFx9croalbXLWKG16F73U&export=download','banner',0),
(21,4,'https://drive.google.com/uc?id=1MlWb11qiTTUatjPbAXW176KH_enSgtRt&export=download','banner',0),
(22,5,'https://drive.google.com/uc?id=1Ox9UfjVDMq9Lm38T31ITfcFF089G5M6q&export=download','banner',0),
(27,10,'https://drive.google.com/uc?id=1jLcwgRWPxpC-_tAwvEi0Z6iqDLd8Zxsr&export=download','banner',0),
(32,2,'https://drive.google.com/uc?id=1XR34WUORhk4bB7wdAs3i68EKb0nRqNEr&export=download','screenshot',0),
(33,2,'https://drive.google.com/uc?id=1uYGBXy3KJzxgqaAnKyys2QO8Y2fke37Y&export=download','screenshot',0),
(36,1,'https://drive.google.com/uc?id=1dMGDZmgyJV9cDzFCpJPTXmI8kw5h3TcG&export=download','screenshot',0),
(37,1,'https://drive.google.com/uc?id=1JMSaaPeCvraXnCjNBWo7jFgsGCxmIBme&export=download','screenshot',0),
(38,10,'https://drive.google.com/uc?id=1ZWh89cfp2OogE7sdb9AW9CD6mVxGXtFu&export=download','screenshot',0),
(39,10,'https://drive.google.com/uc?id=1c0ZikeskWe3h-v1HL6mUmXCdFSSubfvZ&export=download','screenshot',0),
(40,18,'https://drive.google.com/uc?id=1qKHm5skSpHzXxa3O965amDtFsOwgBUNN&export=download','icon',0),
(41,18,'https://drive.google.com/uc?id=1q1WYAw3noxk68eCtS1H0SA70Bq8CCT_s&export=download','banner',0),
(42,18,'https://drive.google.com/uc?id=1_oxuyoZ99PtgezCnUSpYxxeQGqZElf0m&export=download','screenshot',0);

INSERT INTO tags (id, name, description) VALUES
(1, 'Action', 'Fast-paced games requiring quick reflexes'),
(2, 'Adventure', 'Exploration and story-driven experiences'),
(3, 'Arcade', 'Classic arcade-style gameplay'),
(4, 'Platformer', 'Jumping and running through levels'),
(5, 'Racing', 'Competitive speed-based games'),
(6, 'Puzzle', 'Logic and problem-solving challenges'),
(7, 'RPG', 'Role-playing games with character progression'),
(8, 'Strategy', 'Tactical thinking and resource management'),
(9, 'Shooter', 'Combat with ranged weapons'),
(10, 'Roguelike', 'Permadeath and procedural generation'),
(11, 'Simulation', 'Realistic or systemic simulations'),
(12, 'Sandbox', 'Open-ended creative freedom'),
(13, 'Multiplayer', 'Play with or against other players'),
(14, 'Singleplayer', 'Solo gaming experience'),
(15, 'Co-op', 'Collaborative multiplayer'),
(16, 'Retro', 'Classic or nostalgic style'),
(17, 'Dungeon Crawler', 'Exploring dungeons and defeating monsters'),
(18, 'Tower Defense', 'Defending positions from waves of enemies'),
(19, 'Building', 'Constructing structures and bases'),
(20, 'Economic', 'Focus on trade and economy'),
(21, 'Survival', 'Managing resources to stay alive'),
(22, 'Puzzle-platformer', 'Combination of puzzles and platforming'),
(23, 'Open World', 'Large explorable environments'),
(24, 'Turn-based', 'Games where players take turns'),
(25, 'Real-time', 'Continuous action without turns'),
(26, 'Fantasy', 'Magical and mythical settings'),
(27, 'Sci-Fi', 'Science fiction themes'),
(28, 'Arena', 'Competitive arena-based combat'),
(29, 'Fighting', 'Hand-to-hand combat games'),
(30, 'Civilization', 'Building and managing civilizations'),
(31, 'Exploration', 'Discovering new areas and secrets'),
(32, 'Crafting', 'Creating items from resources'),
(33, 'Horror', 'Terrifying and suspenseful experiences'),
(34, 'Management', 'Overseeing systems and resources'),
(35, 'Sports', 'Athletic and competitive sports games'),
(36, 'Educational', 'Learning through gameplay'),
(37, 'Family', 'Games suitable for all ages'),
(38, 'Casual', 'Relaxed, easy-to-learn gameplay'),
(39, 'Competitive', 'High-stakes player vs player'),
(40, 'Arcade Racing', 'Fast-paced, unrealistic racing'),
(41, 'MMO', 'Massively multiplayer online'),
(42, 'MOBA', 'Multiplayer online battle arena'),
(43, 'Battle Royale', 'Last-player-standing competition'),
(44, 'Card Game', 'Card-based strategy games'),
(45, 'Rhythm', 'Music and timing-based gameplay');

INSERT INTO games_tags (id_game, id_tag) VALUES
(1, 1), (1, 4),
(2, 3), (2, 5),
(3, 2), (3, 4), (3, 3),
(4, 3),
(5, 1), (5, 4),
(10, 1), (10, 9), (10, 37), (10, 40),  -- SuperTuxKart
(18,1), (18,27), (18,9); -- Space eternity 3