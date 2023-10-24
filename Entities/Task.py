class Task:
    next_id = 1

    def __init__(self, title, description, assigned):
        self.id = Task.next_id
        self.title = title
        self.description = description
        self.assigned = assigned

        Task.next_id += 1

    pass

    def to_json(self):
        return {
            "id": self.id,
            "title": self.title,
            "description": self.description,
            "assigned": self.assigned
        }
