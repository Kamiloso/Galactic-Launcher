# It is not technically an "error". It is only used
# to cascade-exit multiple layers of menus without showing
# the system("pause") message.

class SoftExitError(Exception):
    def __init__(self, ttl: int = 0):
        super().__init__("Soft exit error.")
        self.throws = ttl

    def with_decremented_ttl(self):
        return SoftExitError(self.throws - 1)