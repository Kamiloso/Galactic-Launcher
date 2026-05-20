USE galactic;

-- 1. Dodawanie gier (w tym Twoich rzeczywistych projektów dla lepszego kontekstu testowego)
INSERT IGNORE INTO games (id, name, description) VALUES
(1, 'Space Eternity 3', 'A multiplayer game featuring vast procedural generation and deep space exploration.'),
(2, 'Larnix', 'A 2D block-building sandbox game with a custom client-server architecture.'),
(3, 'Neon Drifter', 'A fast-paced cyberpunk racing game with neon aesthetics.');

-- 2. Dodawanie tagów
INSERT IGNORE INTO tags (id, name, description) VALUES
(1, 'Multiplayer', 'Games that support multiple players interacting online.'),
(2, 'Procedural Generation', 'Games where content is generated algorithmically.'),
(3, 'Sandbox', 'Games providing the player with a great degree of creativity.'),
(4, '2D', 'Games with two-dimensional graphics and sprites.'),
(5, 'Engine-Independent', 'Projects built with custom architecture independent of standard engines.');

-- 3. Powiązania gier i tagów (Tabela łącząca)
INSERT IGNORE INTO games_tags (id_game, id_tag) VALUES
(1, 1), (1, 2), (1, 5), -- Space Eternity 3
(2, 1), (2, 3), (2, 4), (2, 5), -- Larnix
(3, 1), (3, 4); -- Neon Drifter

-- 4. Dodawanie wersji gier
INSERT IGNORE INTO versions (id, id_game, caption, type, description, is_primary, release_date, platform, download_url, exec_location, sha256_hash, alert) VALUES
(1, 1, 'v1.0.0', 'release', 'Initial stable release with full procedural universe generation.', 1, '2026-05-10', 'windows', 'https://dl.galactic.test/se3/v1.0.0-win.zip', 'SpaceEternity3.exe', 'sha256_hash_1', 'stable'),
(2, 1, 'v1.1.0-alpha', 'alpha', 'Testing new multiplayer sync engine.', 0, '2026-05-15', 'linux', 'https://dl.galactic.test/se3/v1.1.0a-linux.tar.gz', './SpaceEternity3', 'sha256_hash_2', 'alert'),
(3, 2, 'v0.5.0', 'beta', 'Early beta introducing the custom client-server architecture.', 1, '2026-04-20', 'windows', 'https://dl.galactic.test/larnix/v0.5.0-win.zip', 'Larnix.exe', 'sha256_hash_3', 'stable'),
(4, 2, 'v0.5.1-snapshot', 'snapshot', 'Fixing memory allocation bugs in the networking protocol.', 0, '2026-05-16', 'windows', 'https://dl.galactic.test/larnix/snapshot-win.zip', 'Larnix.exe', null, 'danger'),
(5, 3, 'v2.1', 'release', 'Added new neon tracks and improved rendering performance.', 1, '2025-11-11', 'macsilicon', 'https://dl.galactic.test/neon/v2.1-mac.dmg', 'NeonDrifter.app', null, 'stable');

-- 5. Dodawanie obrazów (Assetów)
INSERT IGNORE INTO images (id, id_game, download_url, type, sort_index) VALUES
(1, 1, 'https://img.galactic.test/se3/icon.png', 'icon', 0),
(2, 1, 'https://img.galactic.test/se3/banner.jpg', 'banner', 0),
(3, 1, 'https://img.galactic.test/se3/screen1.jpg', 'screenshot', 1),
(4, 1, 'https://img.galactic.test/se3/icon2.png', 'icon', 0), -- additional icon for backend testing
(5, 2, 'https://img.galactic.test/larnix/icon.png', 'icon', 0),
(6, 2, 'https://img.galactic.test/larnix/screen_build.jpg', 'screenshot', 1),
(7, 2, 'https://img.galactic.test/larnix/screen_server.jpg', 'screenshot', 2),
(8, 3, 'https://img.galactic.test/neon/banner.png', 'banner', 0);

-- 6. Dodawanie wpisów do historii logów
INSERT IGNORE INTO history (id, id_game, info) VALUES
(1, 1, 'Space Eternity 3 reached 1000 daily active players.'),
(2, 1, 'Deployed critical hotfix for procedural generation seed bug.'),
(3, 2, 'Larnix beta server stress test completed successfully.'),
(4, NULL, 'Galactic Launcher backend system initialized and running smoothly.');