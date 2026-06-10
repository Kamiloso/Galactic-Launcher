import os
import sys

class Utils:
    BORDER = "-" * 60
    BORDER_HALF = "-" * 30

    MAX_INT_32 = 2_147_483_647

    PRD_CERT_THUMBPRINT = "990a6a6647d286c7f22badf4a1bcf534b64eb372f8daee4802fccc023cb04467"
    DEV_CERT_THUMBPRINT = "8d2df4330cc5662ea74196ab3c1958c51f0ce45ce9143f2bb8e77fc4d6126005"

    PRD_ENDPOINT = "https://api-galactic.se3.page:27687"
    DEV_ENDPOINT = "https://localhost:27687"

    @staticmethod
    def DEV_MODE():
        is_dev = any(f in sys.argv for f in {"--dev", "--debug", "--development"})
        is_vscode = os.getenv("TERM_PROGRAM") == "vscode"
        
        return is_dev or is_vscode
    
    @staticmethod
    def IS_WINDOWS():
        return os.name == 'nt'
    
    @staticmethod
    def sys_clear():
        os.system('cls' if Utils.IS_WINDOWS() else 'clear')

    @staticmethod
    def sys_pause():
        if Utils.IS_WINDOWS():
            os.system("pause")
        else:
            os.system('read -n 1 -s -r -p "Press any key to continue..."')


class TextUtils:
    @staticmethod
    def break_string(src_line: str, max_width: int) -> str:
        words = src_line.strip().split()
        if not words:
            return ""

        lines = []
        current_line = []
        current_length = 0

        for word in words:
            if len(word) > max_width:
                if current_line:
                    lines.append(" ".join(current_line))
                    current_line = []
                    current_length = 0
                
                for i in range(0, len(word), max_width):
                    chunk = word[i:i + max_width]
                    if len(chunk) == max_width:
                        lines.append(chunk)
                    else:
                        current_line.append(chunk)
                        current_length = len(chunk)
            else:
                space_padding = 1 if current_line else 0
                if current_length + space_padding + len(word) <= max_width:
                    current_line.append(word)
                    current_length += space_padding + len(word)
                else:
                    lines.append(" ".join(current_line))
                    current_line = [word]
                    current_length = len(word)

        if current_line:
            lines.append(" ".join(current_line))

        return "\n".join(lines)