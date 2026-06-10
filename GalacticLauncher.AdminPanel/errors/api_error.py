class ApiError(Exception):
    def __init__(self, message: str, status_code: int | None = None):
        super().__init__(message)
        self.message = message
        self.status_code = status_code

    def __str__(self):
        if self.status_code is not None:
            return f"API Error! (HTTP {self.status_code}): {self.message}"
        else:
            return f"API Error! {self.message}"