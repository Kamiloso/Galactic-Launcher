import os
import sys
import re

from datetime import date

class Utils:
    BORDER = "-" * 60
    BORDER_HALF = "-" * 30

    MIN_INT_32 = -2_147_483_648
    MAX_INT_32 = 2_147_483_647

    MIN_INT_64 = -9_223_372_036_854_775_808
    MAX_INT_64 = 9_223_372_036_854_775_807

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

    @staticmethod
    def date_to_str(date: date) -> str:
        return date.strftime("%Y-%m-%d")


class TextUtils:
    @staticmethod
    def break_string(src_line: str, max_width: int) -> str:
        if max_width <= 0:
            raise ValueError("Max width must be greater than 0.")
            
        lines_out = []
        
        paragraphs = src_line.split('\n')
        
        for p in paragraphs:
            if not p:
                lines_out.append("")
                continue
                
            tokens = re.split(r'([ \t]+)', p)
            current_line = ""
            pending_space = ""
            
            for i, token in enumerate(tokens):
                if not token:
                    continue
                
                is_space = (i % 2 != 0)
                
                if is_space:
                    pending_space += token
                else:
                    word = token
                    
                    if len(current_line) + len(pending_space) + len(word) <= max_width:
                        current_line += pending_space + word
                        pending_space = ""
                    else:
                        if current_line:
                            lines_out.append(current_line)
                            current_line = ""
                            pending_space = ""
                            
                            if len(word) <= max_width:
                                current_line = word
                            else:
                                while len(word) > max_width:
                                    lines_out.append(word[:max_width])
                                    word = word[max_width:]
                                current_line = word
                        else:
                            text_to_add = pending_space + word
                            while len(text_to_add) > max_width:
                                lines_out.append(text_to_add[:max_width])
                                text_to_add = text_to_add[max_width:]
                            current_line = text_to_add
                            pending_space = ""
            
            if current_line or pending_space:
                lines_out.append(current_line + pending_space)
                
        return "\n".join(lines_out)