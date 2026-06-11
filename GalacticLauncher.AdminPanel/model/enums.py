class VersionType:
    VALUES = ["Unknown", "Alpha", "Beta", "Release", "Snapshot"]
    
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
            "alpha": 1,
            "beta": 2,
            "release": 3,
            "snapshot": 4,
        }.get(s.lower(), 0)


class ImageType:
    VALUES = ["Unknown", "Icon", "Banner", "Screenshot"]

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
            "icon": 1,
            "banner": 2,
            "screenshot": 3,
        }.get(s.lower(), 0)


class Platform:
    VALUES = ["Unknown", "Windows", "Linux", "MacSilicon", "MacIntel"]

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
            "windows": 1,
            "linux": 2,
            "macsilicon": 3,
            "macintel": 4,
        }.get(s.lower(), 0)


class AlertLevel:
    VALUES = ["Unknown", "Stable", "Alert", "Danger"]

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
            "stable": 1,
            "alert": 2,
            "danger": 3,
        }.get(s.lower(), 0)