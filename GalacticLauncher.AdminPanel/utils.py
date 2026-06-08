import os
import sys

class Utils:
    BORDER = "-" * 60

    PRD_CERT_THUMBPRINT = "990a6a6647d286c7f22badf4a1bcf534b64eb372f8daee4802fccc023cb04467"
    DEV_CERT_THUMBPRINT = "8d2df4330cc5662ea74196ab3c1958c51f0ce45ce9143f2bb8e77fc4d6126005"

    PRD_ENDPOINT = "https://api-galactic.se3.page:27687"
    DEV_ENDPOINT = "https://localhost:27687"

    @staticmethod
    def DEV_MODE():
        is_dev = any(f in sys.argv for f in {"--dev", "--debug", "--development"})
        is_vscode = os.getenv("TERM_PROGRAM") == "vscode"
        
        return is_dev or is_vscode