# Operation aborted automatically or by the user.

class AbortError(Exception):
    def __init__(self, message: str):
        self.message = message
        super().__init__(self.message)


# Indicates a network / API error.

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


# It is not technically an "error". It is only used
# to cascade-exit multiple layers of menus without showing
# the system("pause") message.

class SoftExitError(Exception):
    def __init__(self, ttl: int = 0):
        super().__init__("Soft exit error.")
        self.throws = ttl

    def with_decremented_ttl(self):
        return SoftExitError(self.throws - 1)