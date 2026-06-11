class SaveTracker:
    def __init__(self, obj: object = None):
        self.obj: object = obj
        self.is_fresh = True

    def inform_has(self, obj: object):
        if obj is not self.obj:
            self.obj = obj
            self.is_fresh = True

    def inform_modify(self):
        self.is_fresh = False

    def inform_save(self):
        self.is_fresh = True

    def has_unsaved_changes(self) -> bool:
        return not self.is_fresh