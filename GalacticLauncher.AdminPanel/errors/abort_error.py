class AbortError(Exception):
    def __init__(self, message: str, is_user_abort: bool = False):
        self.message = message
        self.is_user_abort = is_user_abort
        super().__init__(self.message)