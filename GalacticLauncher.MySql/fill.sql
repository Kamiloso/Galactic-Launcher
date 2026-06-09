USE galactic;

INSERT INTO games (id, name, author, description) VALUES
(1, 'Teeworlds', 'Robyt3', 'Teeworlds is a free online multiplayer game, available for all major operating systems. Battle with up to 16 players in a variety of game modes, including Team Deathmatch and Capture The Flag. You can even design your own maps!'),
(2, 'Sonic Robo Blast 2', 'LJSonik', 'Sonic Robo Blast 2 is a 3D open-source Sonic the Hedgehog fangame built using a modified version of the Doom Legacy port of Doom. SRB2 is closely inspired by the original Sonic games from the Sega Genesis, and attempts to recreate the design in 3D. While SRB2 isn\'t fully completed, it already features tons of levels, enemies, speed, and quite a lot of the fun that the original Sonic games provided.'),
(3, 'Canabalt', 'Adam Atomic', 'Escape the destruction of your city with just one button! The game that popularized the infinite runner genre is back with two-player mode, new challenges, new music, new achievements, and new leaderboards!'),
(4, 'Endless sky', 'juzzlin', 'Explore other star systems. Earn money by trading, carrying passengers, or completing missions.'),
(5, 'Dust racing', 'Vladyslav', 'Dust Racing 2D (Dustrac) is a tile-based, cross-platform 2D racing game written in Qt (C++) and OpenGL. Dust Racing 2D comes with a Qt-based level editor for easy level creation. A separate engine, MiniCore, is used for physics modeling.'),
(6, 'SDL Sopwith', 'fragglet', 'SDL Sopwith is a port of the game "Sopwith", which was originally by BMB Compuscience Canada. The original author David L. Clark has kindly released the source code under the GNU GPL.
Features:
- Uses LibSDL, so will run on most modern operating systems, and also the web (via emscripten)
- Support for loading custom mission files (new levels)
- TCP/IP multiplayer
- PC speaker emulation
- Multiple palettes that emulate a selection of old monitors
- Medals and high score table'),
(7, 'Mindustry', 'Anuken', 'A hybrid tower-defense sandbox factory game designed with deep logistical systems. Create elaborate, multi-layered supply chains using conveyor belts, conduits, and mass drivers to feed ammo into your defensive turrets, produce advanced materials for structural building, and manage power grids. Defend your core from relentless waves of enemies in the campaign, or challenge other players in intense cross-platform multiplayer matches spanning across massive, resource-rich hex maps.'),
(8, 'Shattered Pixel Dungeon', '00-Evan', 'A traditional 2D pixel-art roguelike dungeon crawler that offers incredible mechanical depth, high replayability, and randomized procedural level generation. Every single run is a unique tactical puzzle featuring fierce enemies, unique environmental hazards, and hidden secrets. Choose from four distinct playable characters—each with their own sub-classes, unique talent trees, and specialized gear configurations—and descend into the brutal depths to retrieve the elusive Amulet of Yendor.'),
(9, 'OpenTTD', 'OpenTTD Team', 'A massive business simulation game in which players earn money by transporting passengers and complex cargo across vast procedural maps via road, rail, water, and air. Acting as an open-source remake and expansion of the legendary Transport Tycoon Deluxe, it features advanced pathfinding algorithms, competitive AI opponents, and a highly customizable economic environment. Supports multiplayer sessions with up to 255 players competing to build the ultimate transport empire.'),
(10, 'SuperTuxKart', 'STK Team', 'A beautifully crafted 3D open-source arcade racing game featuring a variety of mascot characters, colorful tracks, and diverse gameplay modes. Players can experience a full story-driven single-player campaign, participate in classic Grand Prix tournaments, battle in specialized arenas, or test their driving reflexes in time trials. The game fully supports local split-screen action as well as massive online multiplayer races driven by an advanced physics engine.'),
(11, 'Cataclysm: Dark Days Ahead', 'CleverRaven', 'A brutally realistic, turn-based survival RPG set in a procedurally generated post-apocalyptic world. After the collapse of civilization, you must scavenge ruined towns for food, rare medical equipment, weapons, and tools. Build reinforced shelters, modify vehicles with heavy plating, and craft custom gear to survive against infinite hordes of zombies, Lovecraftian monsters, deadly bio-weapons, and the harsh, unforgiving changing of seasons.'),
(12, 'Xonotic', 'Team Xonotic', 'An addictive, arena-style first-person shooter combining crisp, high-speed movement mechanics with a wide array of uniquely balanced futuristic weapons. Heavily inspired by classic arena shooters like Unreal Tournament and Quake, Xonotic offers adrenaline-fueled multiplayer gameplay modes including Capture The Flag, Deathmatch, and Nexball. It is built on a highly optimized custom engine, delivering smooth competitive frame rates and deep tracking statistics.'),
(14, 'The Battle for Wesnoth', 'Wesnoth Team', 'An immersive, open-source turn-based tactical strategy game set in a rich high-fantasy universe. Players build and command a powerful army composed of various distinct factions, turning raw elven recruits, dwarven warriors, and human mages into hardened battle veterans through an intricate experience and leveling system. Features dozens of official, fully voiced narrative campaigns, user-created content, and tactical multiplayer scenarios.'),
(15, 'Unciv', 'Yairm210', 'An open-source, incredibly lightweight, and highly optimized re-implementation of the world-famous turn-based civilization-building genre. Build your empire from a primitive tribe into a global superpower by researching ancient and futuristic technologies, managing diplomatic ties, managing city production, and maneuvering military units across a hex-based map. Features absolute modding support, cross-platform saves, and highly efficient processing loops.'),
(18, 'Space eternity 3','Kamil Szkop','Explore space and fight with bosses');


INSERT INTO versions (id, id_game, caption, type, description, is_primary, release_date, platform, download_url, exec_location, sha256_hash, alert) VALUES
-- Wersje dla Teeworlds (ID 1)
(1, 1, 'v0.7.5', 'release', 'Fixed using correct array measurements when placing egg doodads', 1, '2025-11-11', 'windows', 'https://github.com/teeworlds/teeworlds/releases/download/0.7.5/teeworlds-0.7.5-win64.zip', 'teeworlds-0.7.5-win64\\teeworlds.exe', '83CD75884A0E8339B4910DA1BFE4D737AF794D35600CD065E7C2222750B37585', 'stable'),
(50, 1, 'v0.7.5', 'release', 'Fixed using correct array measurements when placing egg doodads', 1, '2025-11-11', 'linux', 'https://drive.google.com/uc?id=1ghoYSIfmhuAoBW4GIW0rgL0i7WjEZmxJ&export=download', 'teeworlds-0.7.5-win64\\teeworlds', null, 'stable'),
(2, 1, 'v0.7.3.1', 'release', 'Fix platform-specific client libraries for Linux', 0, '2025-05-10', 'windows', 'https://github.com/teeworlds/teeworlds/releases/download/0.7.3.1/teeworlds-0.7.3.1-win64.zip', 'teeworlds-0.7.3.1-win64\\teeworlds.exe', null, 'stable'),
(40, 1, 'v0.7.2.1', 'release', 'Small fixes', 0, '2025-03-10', 'windows', 'https://github.com/teeworlds/teeworlds/releases/download/0.7.2/teeworlds-0.7.2-win64.zip', 'teeworlds-0.7.2-win64\\teeworlds.exe', null, 'stable'),

-- Wersje dla Sonic Robo Blast 2 (ID 2)
(3, 2, 'v1.22.1.5', 'release', 'Fixed nothing', 1, '2025-11-11', 'windows', 'https://github.com/STJr/SRB2/releases/download/SRB2_release_2.2.15/SRB2-v2215-Full.zip', 'Sonic-2-2-15.exe', null, 'stable'),
(41, 2, 'v2.2.8', 'release', 'Fixed nothing', 0, '2024-05-11', 'windows', 'https://github.com/STJr/SRB2/releases/download/SRB2_release_2.2.8/SRB2-v2.2.8-Full.zip', 'Sonic-2-2-8.exe', null, 'stable'),

-- Wersje dla Canabalt (ID 3)
(4, 3, 'v1.1.1', 'release', 'Fixed bleeding', 1, '2025-11-11', 'windows', 'https://github.com/ninjamuffin99/canabalt-hf/releases/download/bleeding/canabalt-windows-2024-07-11-main.zip', 'Canabalt-1-1-1.exe', null, 'stable'),
(51, 3, 'v1.1.1', 'release', 'Fixed bleeding', 1, '2025-11-11', 'linux', 'https://drive.google.com/uc?id=1D0Xiq0YnRv2DDDYgeQNdLsnlN_ZtfHtc&export=download', 'Canabalt-1-1-1', null, 'stable'),

-- Wersje dla Endless sky (ID 4)
(5, 4, 'v0.10.16', 'release', 'Reverted changes to movement AI.', 1, '2025-11-11', 'windows', 'https://github.com/endless-sky/endless-sky/releases/download/v0.10.16/EndlessSky-win64-v0.10.16.zip', 'EndlessSky.exe', null, 'stable'),

-- Wersje dla Dust racing (ID 5)
(6, 5, 'v2.1.1', 'release', 'Stable update', 1, '2025-11-11', 'windows', 'https://github.com/juzzlin/DustRacing2D/releases/download/2.1.1/dustracing2d-2.1.1-windows-x86.zip', 'Dustracing.exe', null, 'stable'),
(52, 5, 'v2.1.1', 'release', 'Stable update', 1, '2025-11-11', 'linux', 'https://drive.google.com/uc?id=13VRi3vJcew6nj1Oj3AX_w_UJqZX5umg3&export=download', 'Dustracing', null, 'stable'),

-- SDL Sopwith (ID 6)
(7, 6, 'v2.9.0', 'release', 'A crash was fixed when any object was destroyed in multiplayer (thanks to @alphanumericcharter and @scandox for reporting this bug). Scoring has been fixed when killing animals (it is supposed to be negative, but because of a bug, accidentally became positive). The game now saves high scores to a system-wide high scores table (if possible; or a per-user one otherwise). The "tailspin" animation when a plane is crashing and in a nosedive was tweaked to add two additional frames of animation. Various Emscripten fixes were made; the previous release had to be rolled back from the website because of them.', 1, '2026-05-13', 'windows', 'https://github.com/fragglet/sdl-sopwith/releases/download/sdl-sopwith-2.9.0/sdl-sopwith-2.9.0-win64.zip', 'Sdl-2-9.exe', null, 'stable'),
(53, 6, 'v2.9.0', 'release', 'A crash was fixed when any object was destroyed in multiplayer (thanks to @alphanumericcharter and @scandox for reporting this bug). Scoring has been fixed when killing animals (it is supposed to be negative, but because of a bug, accidentally became positive). The game now saves high scores to a system-wide high scores table (if possible; or a per-user one otherwise). The "tailspin" animation when a plane is crashing and in a nosedive was tweaked to add two additional frames of animation. Various Emscripten fixes were made; the previous release had to be rolled back from the website because of them.', 1, '2026-05-13', 'linux', 'https://drive.google.com/uc?id=1K8KSg9AbrfLybyB0k5_Il0dDqHOxOC9p&export=download', 'Sdl-2-9', null, 'stable'),

-- Mindustry (ID 7)
(8, 7, 'v146', 'release', 'Official stable build 146. Full campaign support.', 1, '2025-12-01', 'windows', 'https://github.com/Anuken/Mindustry/archive/refs/tags/v146.zip', 'mindustry-146.exe', null, 'stable'),
(55, 7, 'v146', 'release', 'Official stable build 146. Full campaign support.', 1, '2025-12-01', 'linux', 'https://drive.google.com/uc?id=1yS-jnjoLhLW0ni1-Z7JgOqv_eAHP6rvf&export=download', 'mindustry-146', null, 'stable'),
(9, 7, 'v145.1', 'release', 'Older stable release for multiplayer compatibility.', 0, '2025-06-15', 'windows', 'https://github.com/Anuken/Mindustry/archive/refs/tags/v145.1.zip', 'mindustry-145-1.exe', null, 'stable'),
(10, 7, 'v144.3', 'release', 'Legacy milestone release of the 144 tree.', 0, '2025-02-10', 'windows', 'https://github.com/Anuken/Mindustry/archive/refs/tags/v144.3.zip', 'mindustry-144-3.exe', null, 'stable'),
(11, 7, 'v143', 'release', 'Major logistics and conveyor overhaul.', 0, '2024-09-05', 'windows', 'https://github.com/Anuken/Mindustry/archive/refs/tags/v143.zip', 'mindustry-143.exe', null, 'stable'),
(12, 7, 'v142', 'release', 'Hotfix patch for network congestion.', 0, '2024-05-18', 'windows', 'https://github.com/Anuken/Mindustry/archive/refs/tags/v142.zip', 'mindustry-142.exe', null, 'stable'),
(13, 7, 'v141', 'release', 'Erekir planet optimization update.', 0, '2024-01-12', 'windows', 'https://github.com/Anuken/Mindustry/archive/refs/tags/v141.1.zip', 'mindustry-141-1.exe', null, 'stable'),
(16, 7, 'v147', 'beta', 'Community beta testing new mechanical liquid systems.', 0, '2026-03-01', 'windows', 'https://github.com/Anuken/Mindustry/archive/refs/tags/v147.zip', 'mindustry-147.exe', null, 'danger'),
(42, 7, 'v155', 'beta', 'Upgraded desktop backend from SDL2 to SDL3 - this may cause new issues, report them if you see any', 0, '2026-06-02', 'windows', 'https://github.com/Anuken/Mindustry/archive/refs/tags/v155.zip', 'mindustry-155.exe', null, 'alert'),
(43, 7, 'v155', 'beta', 'Upgraded desktop backend from SDL2 to SDL3 - this may cause new issues, report them if you see any', 0, '2026-06-02', 'linux', 'https://drive.google.com/uc?id=18-fVaxxyfWsPWRgKoWYwvbVYc14NrLUJ&export=download', 'mindustry-155', null, 'alert'),

-- Unciv (ID 15)
(18, 15, 'v4.11.16', 'release', 'Latest performance tweaks for late-game AI processing.', 1, '2026-04-10', 'windows', 'https://github.com/yairm210/Unciv/archive/refs/tags/4.11.16.zip', 'unciv-4-11-16.exe', null, 'stable'),
(19, 15, 'v4.11.0', 'release', 'Introduced new visual UI theme system.', 0, '2026-02-28', 'windows', 'https://github.com/yairm210/Unciv/archive/refs/tags/4.11.0.zip', 'unciv-4-11.exe', null, 'stable'),
(20, 15, 'v4.10.5', 'release', 'Bugfixes regarding ocean trade route gold values.', 0, '2026-01-15', 'windows', 'https://github.com/yairm210/Unciv/archive/refs/tags/4.10.5.zip', 'unciv-4-10-5.exe', null, 'stable'),
(21, 15, 'v4.10.0', 'release', 'Major overhaul of the mod-loading framework.', 0, '2025-11-20', 'windows', 'https://github.com/yairm210/Unciv/archive/refs/tags/4.10.0.zip', 'unciv-4-10.exe', null, 'stable'),
(22, 15, 'v4.9.12', 'release', 'Hotfix for multiplayer connection timeouts.', 0, '2025-09-02', 'windows', 'https://github.com/yairm210/Unciv/archive/refs/tags/4.9.12.zip', 'unciv-4-9-12.exe', null, 'stable'),
(23, 15, 'v4.9.0', 'release', 'Added map generation options for archipelago maps.', 0, '2025-07-14', 'windows', 'https://github.com/yairm210/Unciv/archive/refs/tags/4.9.0.zip', 'unciv-4-9', null, 'stable'),
(24, 15, 'v4.8.2', 'release', 'Legacy mechanics balance update.', 0, '2025-04-01', 'windows', 'https://github.com/yairm210/Unciv/archive/refs/tags/4.8.2.zip', 'unciv-4-8-2.exe', null, 'stable'),
(25, 15, 'v4.7.0', 'release', 'Introduced custom scenario loaders.', 0, '2025-01-10', 'windows', 'https://github.com/yairm210/Unciv/archive/refs/tags/4.7.0.zip', 'unciv-4-7.exe', null, 'alert'),
(26, 15, 'v4.13.0-Alpha1', 'alpha', 'Testing global mechanics rewrites.', 0, '2026-05-12', 'windows', 'https://github.com/yairm210/Unciv/archive/refs/tags/4.13.6.zip', 'univ-4-13-6.exe', null, 'alert'),
(27, 15, 'v4.12.0-Beta3', 'beta', 'Beta build testing tactical overlay systems.', 0, '2026-05-25', 'windows', 'https://github.com/yairm210/Unciv/archive/refs/tags/4.13.7-patch1.zip', 'unciv-4-13-7.exe', null, 'danger'),
-- Shattered Pixel Dungeon (ID 8)
(28, 8, 'v2.4.2', 'release', 'Latest stable desktop release.', 1, '2026-02-10', 'windows', 'https://github.com/00-Evan/shattered-pixel-dungeon/releases/download/v2.4.2/ShatteredPD-v2.4.2-Windows.zip', 'shattered-2-4-2.exe', null, 'stable'),
(29, 8, 'v2.4.0', 'release', 'Previous major update.', 0, '2025-11-20', 'windows', 'https://github.com/00-Evan/shattered-pixel-dungeon/archive/refs/tags/v2.4.0.zip', 'shattered-2-4.exe', null, 'stable'),

-- OpenTTD (ID 9)
(30, 9, 'v14.1', 'release', 'Stable update fixing simulation memory leaks.', 1, '2026-04-01', 'windows', 'https://github.com/OpenTTD/OpenTTD/archive/refs/tags/14.1.zip', 'openttd.exe', null, 'stable'),

-- SuperTuxKart (ID 10)
(31, 10, 'v1.4', 'release', 'Stable release 1.4. Features new tracks.', 1, '2025-10-01', 'windows', 'https://github.com/supertuxkart/stk-code/releases/download/1.5-rc1/SuperTuxKart-1.5-rc1-win.zip', 'super-kart.exe', null, 'stable'),

-- Cataclysm DDA (ID 11)
(32, 11, '0.G', 'snapshot', 'The Gaiman release. Highly optimized.', 1, '2025-05-20', 'windows', 'https://github.com/CleverRaven/Cataclysm-DDA/releases/download/cdda-experimental-2026-06-07-2026/cdda-windows-with-graphics-and-sounds-x64-2026-06-07-2026.zip', 'cataclysm.exe', null, 'danger'),

-- Xonotic (ID 12)
(33, 12, 'v0.8.6', 'release', 'Latest arena build with netcode optimizations.', 1, '2025-09-11', 'windows', 'https://github.com/xonotic/xonotic/archive/refs/tags/xonotic-v0.8.6.zip', 'xonotic-8-6.exe', null, 'stable'),

-- The Battle for Wesnoth (ID 14)
(35, 14, 'v1.18.2', 'release', 'Maintenance build including campaign fixes.', 1, '2026-05-18', 'windows', 'https://github.com/wesnoth/wesnoth/archive/refs/tags/1.18.2.zip', 'winsnoth.exe', null, 'stable'),

-- Space eternity
(36, 18,'v2.3b','beta','A patch for version Release 2.3 containing a massive memory leak fix.
 It includes Windows build, Linux build and the source code. The game server is included in 
 the source code inside ServerReady directory as a node.js project as well as the authorization 
 server in the AuthorizationServer directory. Only client has changed in this update.',1, '2025-04-18', 'windows', 'https://github.com/Space-Eternity-3/Space-Eternity-3/releases/download/release%2FRelease-2.3b/SE3-Release-2.3b-win32-x86_64.zip', 'space-eternity-2-3.exe', null, 'stable');



INSERT INTO images (id, id_game, download_url, type, sort_index) VALUES
(1, 1, 'https://drive.google.com/uc?id=19IpyVBOhjIRE4tbLcYhzQ6bhXFzEJn1X&export=download','icon',0),
(2, 2, 'https://drive.google.com/uc?id=1yCbBx7WG7NWqXaqUm8Ti7QRAhy1X_Wnd&export=download','icon',0),
(3, 3, 'https://drive.google.com/uc?id=1jzexuK2J_oEIHvkDfyrky5G_xE9FzqF&export=download', 'icon', 0),
(4, 4, 'https://drive.google.com/uc?id=1-SU_jwne0QwjMuciW4c83FG5ZBTDrT8D&export=download','icon',0),
(5, 5, 'https://drive.google.com/uc?id=1tAa5xbpbviIi5xhB8VIKoKELq8I50k7k&export=download','icon',0),
(6, 6, 'https://drive.google.com/uc?id=12JW5hJlSFo9G-Xymtk3vYUjLRTetEVMD&export=download','icon',0),
(9, 4, 'https://drive.google.com/uc?id=19IpyVBOhjIRE4tbLcYhzQ6bhXFzEJn1X&export=download','icon',0),
(10,7, 'https://drive.google.com/uc?id=1NAKrmSbWcYbe2IPoDkvoF1aUsIA6mK2W&export=download','icon',0),
(11,15,'https://drive.google.com/uc?id=1TaPj57gCrtEm3Mk-yQQYiBQ4aXtcM-ZS&export=download','icon',0),
(12,14,'https://drive.google.com/uc?id=1kiLA0oBllV4yeKwQsEVrt-cvBtOH6cgQ&export=download','icon',0),
(13,12,'https://drive.google.com/uc?id=1NS_46uPE8UQZHCWmu4c6ic1UqgUOLW5T&export=download','icon',0),
(14,11,'https://drive.google.com/uc?id=1TaPj57gCrtEm3Mk-yQQYiBQ4aXtcM-ZS&export=download','icon',0),
(15,10,'https://drive.google.com/uc?id=1OE6dWng0oQl7aAQVWyL9pRT8TlkHFYcF&export=download','icon',0),
(16,9,'https://drive.google.com/uc?id=1GmM1lqlehiuLd21Ls6ozoq3VXIQOT9dL&export=download','icon',0),
(17,8,'https://drive.google.com/uc?id=1YKUwpFhEMAIDgEc0lWGUUMVxzEAZtOyq&export=download','icon',0),
(18,1,'https://drive.google.com/uc?id=1jGYjCLVmhn5JkcZwpZy44PkIsJ9bOYwE&export=download','banner',0),
(19,2,'https://drive.google.com/uc?id=1Sme3nDNfUdEHawZ8J0waUqIxZbjH4mLc&export=download','banner',0),
(20,3,'https://drive.google.com/uc?id=1QsLzvcU5oy8aFx9croalbXLWKG16F73U&export=download','banner',0),
(21,4,'https://drive.google.com/uc?id=1MlWb11qiTTUatjPbAXW176KH_enSgtRt&export=download','banner',0),
(22,5,'https://drive.google.com/uc?id=1Ox9UfjVDMq9Lm38T31ITfcFF089G5M6q&export=download','banner',0),
(23,6,'https://drive.google.com/uc?id=1Dy5nA5hA9gzq0i108e871ki6ZTC0woyI&export=download','banner',0),
(24,7,'https://drive.google.com/uc?id=1uBDk7Aho_iHS1P2HQRfy-saO7FUy8x9k&export=download','banner',0),
(25,8,'https://drive.google.com/uc?id=1emEweKlis2z_1bgexlM7zsWWlKLA2jYn&export=download','banner',0),
(26,9,'https://drive.google.com/uc?id=1gtMU-tDwASBZS5xo1-6ngm3gi438io1_&export=download','banner',0),
(27,10,'https://drive.google.com/uc?id=1jLcwgRWPxpC-_tAwvEi0Z6iqDLd8Zxsr&export=download','banner',0),
(28,11,'https://drive.google.com/uc?id=1WNh9hPklymFJQ1cyuPTazrEsgV-ti1F1&export=download','banner',0),
(29,12,'https://drive.google.com/uc?id=1HyWS9pJ3FzObLSq09jGoU8p8d2vlEpj7&export=download','banner',0),
(30,14,'https://drive.google.com/uc?id=18aTB8Hn__J8EHbV2yD_3zelkqN1UhLaV&export=download','banner',0),
(31,15,'https://drive.google.com/uc?id=17OLxXFWQ12UWDyQm0cgkG82dZzcV7Iny&export=download','banner',0),
(32,2,'https://drive.google.com/uc?id=1XR34WUORhk4bB7wdAs3i68EKb0nRqNEr&export=download','screenshot',0),
(33,2,'https://drive.google.com/uc?id=1uYGBXy3KJzxgqaAnKyys2QO8Y2fke37Y&export=download','screenshot',0),
(34,7,'https://drive.google.com/uc?id=1qDT0gVAq0Xv1wRZFl9uI5nkm29w4IpYN&export=download','screenshot',0),
(35,7,'https://drive.google.com/uc?id=1HVjHVjKANRUpXwPtxD_1YKGQoaSV-KWO&export=download','screenshot',0),
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
(6, 4),
(7, 1), (7, 3), (7, 13), (7, 18),     -- Mindustry
(8, 4), (8, 7), (8, 10), (8, 17),     -- Shattered Pixel Dungeon
(9, 1), (9, 13), (9, 20), (9, 34),    -- OpenTTD
(10, 1), (10, 9), (10, 37), (10, 40),  -- SuperTuxKart
(11, 2), (11, 7), (11, 12), (11, 21),  -- Cataclysm: DDA
(12, 1), (12, 9), (12, 14), (12, 28),  -- Xonotic
(14, 1), (14, 12), (14, 13), (14, 26), -- The Battle for Wesnoth
(15, 7), (15, 13), (15, 26), (15, 30), -- Unciv
(18,1), (18,27), (18,9); -- Space eternity 3