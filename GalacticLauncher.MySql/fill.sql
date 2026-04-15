USE galactic;

-- Włączamy obsługę polskich znaków (opcjonalne, ale przydatne)
SET NAMES 'utf8mb4';

-- ==========================================
-- 1. GAMES (GRY)
-- ==========================================
INSERT INTO games (id, name, description) VALUES
(1, 'Stellar Vanguard', 'Epicka symulacja kosmiczna z otwartym światem. Eksploruj tysiące układów słonecznych, handluj zasobami, walcz z piratami i buduj własną flotę statków w nieskończonym wszechświecie.'),
(2, 'Neon Cyber Ninja', 'Dynamiczna platformówka 2D w klimacie cyberpunk. Używaj cyber-katany, modyfikacji ciała i zwinności, aby pokonać megakorporację rządzącą dystopijnym miastem Neo-Tokyo.'),
(3, 'Aether Tactics', 'Turowa gra RPG osadzona w świecie dark fantasy. Zbierz drużynę najemników, rozwijaj ich umiejętności i walcz w wymagających potyczkach, gdzie każda decyzja ma znaczenie.');

-- ==========================================
-- 2. VERSIONS (WERSJE)
-- Zakładamy mapowanie VersionType: 1 = Alpha, 2 = Beta, 3 = Stable/Release
-- ==========================================
INSERT INTO versions (id, version_text, description, is_primary, release_date, version_type, id_game) VALUES
-- Stellar Vanguard (Game ID: 1)
(1, 'v1.0.0', 'Oficjalna premiera gry! Dodano tryb fabularny i nowe modele statków.', TRUE, '2025-10-15 14:00:00', 3, 1),
(2, 'v0.9.5-beta', 'Ostatnia faza beta. Poprawiono stabilność silnika fizycznego i dodano nowy sektor do eksploracji.', FALSE, '2025-08-01 10:30:00', 2, 1),

-- Neon Cyber Ninja (Game ID: 2)
(3, 'v2.1.0', 'Aktualizacja "Neon Rain". Wprowadza nowe poziomy, bossów oraz obsługę kontrolerów DualSense.', TRUE, '2026-02-20 18:00:00', 3, 2),
(4, 'v2.0.0', 'Wielka aktualizacja zmieniająca balans trudności i dodająca tryb New Game+.', FALSE, '2025-11-10 12:00:00', 3, 2),

-- Aether Tactics (Game ID: 3)
(5, 'v0.5.0-alpha', 'Wczesny dostęp. Zawiera pierwszy akt kampanii i 4 z 10 planowanych klas postaci.', TRUE, '2026-03-01 16:45:00', 1, 3);

-- ==========================================
-- 3. IMAGES (ZDJĘCIA / ZASOBY LAUNCHERA)
-- ==========================================
INSERT INTO images (id, url, id_game) VALUES
-- Stellar Vanguard (Game ID: 1)
(1, 'https://cdn.galacticlauncher.com/assets/stellar_vanguard/banner.jpg', 1),
(2, 'https://cdn.galacticlauncher.com/assets/stellar_vanguard/logo_transparent.png', 1),
(3, 'https://cdn.galacticlauncher.com/assets/stellar_vanguard/screenshot_01.jpg', 1),
(4, 'https://cdn.galacticlauncher.com/assets/stellar_vanguard/screenshot_02.jpg', 1),

-- Neon Cyber Ninja (Game ID: 2)
(5, 'https://cdn.galacticlauncher.com/assets/neon_ninja/hero_banner.gif', 2),
(6, 'https://cdn.galacticlauncher.com/assets/neon_ninja/box_art.png', 2),
(7, 'https://cdn.galacticlauncher.com/assets/neon_ninja/action_shot.jpg', 2),

-- Aether Tactics (Game ID: 3)
(8, 'https://cdn.galacticlauncher.com/assets/aether_tactics/store_header.jpg', 3),
(9, 'https://cdn.galacticlauncher.com/assets/aether_tactics/logo_main.png', 3);

-- ==========================================
-- 4. EXECS (PLIKI WYKONYWALNE)
-- Zakładamy mapowanie Platform: 1 = Windows, 2 = macOS, 3 = Linux
-- Zakładamy mapowanie Alert: 0 = Brak (OK), 1 = Ostrzeżenie (np. wirusowy false-positive), 2 = Wycofana/Zablokowana
-- ==========================================
INSERT INTO execs (id, download_url, exec_location, file_hash_sha256, platform, alert, id_version) VALUES
-- Stellar Vanguard v1.0.0 (Version ID: 1)
(1, 'https://dl.galacticlauncher.com/games/stellar_v1.0.0_win.zip', 'StellarVanguard\\StellarVanguard.exe', 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855', 1, 0, 1),
(2, 'https://dl.galacticlauncher.com/games/stellar_v1.0.0_mac.dmg', 'Stellar Vanguard.app/Contents/MacOS/StellarVanguard', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 2, 0, 1),
(3, 'https://dl.galacticlauncher.com/games/stellar_v1.0.0_linux.tar.gz', 'StellarVanguard/stellar_vanguard.x86_64', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 3, 0, 1),

-- Stellar Vanguard v0.9.5-beta (Version ID: 2)
(4, 'https://dl.galacticlauncher.com/games/stellar_v0.9.5_beta_win.zip', 'StellarVanguard_Beta\\StellarVanguard.exe', 'b5a2c96250612366ea272ffac6d9744aaf4b45aacd96a7d657d4a4cb5e48ce1c', 1, 1, 2), -- 1 w Alert (Możliwe problemy ze stabilnością)

-- Neon Cyber Ninja v2.1.0 (Version ID: 3)
(5, 'https://dl.galacticlauncher.com/games/neon_ninja_v2.1.0_win.zip', 'NeonNinja\\Neon.exe', '315f5bdb76d078c43b8ac0064e4a0164612b1fce77c869345bfc94c75894edd3', 1, 0, 3),

-- Neon Cyber Ninja v2.0.0 (Version ID: 4)
(6, 'https://dl.galacticlauncher.com/games/neon_ninja_v2.0.0_win.zip', 'NeonNinja\\Neon.exe', '9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08', 1, 0, 4),

-- Aether Tactics v0.5.0-alpha (Version ID: 5)
(7, 'https://dl.galacticlauncher.com/games/aether_v0.5.0_win.zip', 'AetherTactics\\Launcher.exe', 'c2b7df6201fdd3362399091f0a29550df3505b6a111a7f6fb1e69ed915eb9fb1', 1, 0, 5),
(8, 'https://dl.galacticlauncher.com/games/aether_v0.5.0_linux.tar.gz', 'AetherTactics/aether.sh', '4a44dc15364204a80fe80e9039455cc1608281820aa2a2893ac1bb7c83f9479b', 3, 0, 5);