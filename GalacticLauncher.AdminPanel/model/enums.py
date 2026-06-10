class VersionType:
    @staticmethod
    def to_string(n: int) -> str:
        return {
            1: "Alpha",
            2: "Beta",
            3: "Release",
            4: "Snapshot",
        }.get(n, "Unknown")

    @staticmethod
    def to_int(s: str) -> int:
        return {
            "Alpha": 1,
            "Beta": 2,
            "Release": 3,
            "Snapshot": 4,
        }.get(s, 0)


class ImageType:
    @staticmethod
    def to_string(n: int) -> str:
        return {
            1: "Icon",
            2: "Banner",
            3: "Screenshot",
        }.get(n, "Unknown")

    @staticmethod
    def to_int(s: str) -> int:
        return {
            "Icon": 1,
            "Banner": 2,
            "Screenshot": 3,
        }.get(s, 0)


class Platform:
    @staticmethod
    def to_string(n: int) -> str:
        return {
            1: "Windows",
            2: "Linux",
            3: "MacSilicon",
            4: "MacIntel",
        }.get(n, "Unknown")

    @staticmethod
    def to_int(s: str) -> int:
        return {
            "Windows": 1,
            "Linux": 2,
            "MacSilicon": 3,
            "MacIntel": 4,
        }.get(s, 0)


class AlertLevel:
    @staticmethod
    def to_string(n: int) -> str:
        return {
            1: "Stable",
            2: "Alert",
            3: "Danger",
        }.get(n, "Unknown")

    @staticmethod
    def to_int(s: str) -> int:
        return {
            "Stable": 1,
            "Alert": 2,
            "Danger": 3,
        }.get(s, 0)